using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public const bool I_HAVE_PYTHON_EXE = true;
    private static StringBuilder sharedStringBuilder = new StringBuilder();
    private static bool startReceive = false;
    private static readonly object lockObjectAnswer = new object();
    // 파이썬 코드가 리턴하는 값 : "함수번호 / 출력 문자열"
    [SerializeField] private int milisecondForProcessExit = 3000;
    private bool isExited;
    private System.Diagnostics.Process pythonProcess;
    private System.IO.StreamWriter pythonInput;
    private System.IO.StreamReader pythonOutput;
    private System.Threading.CancellationTokenSource cancellationTokenSource;
    private SynchronizationContext unityContext;

    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private TextMeshProUGUI npcResponseText;
    [SerializeField] private bool isDebugging = true;
    AiFunction aiFunction;

    public static string GetPath(string fileName)
        => $"Assets/_Scripts/PythonFiles/{fileName}.py";

    public void OnSubmitButtonClicked()
    {
        string userInput = userInputField.text;
        SendInputToPython(userInput);
    }
    private void Awake()
    {
        npcResponseText.text = "Halo";
    }
    // Start is called before the first frame update
    void Start()
    {
        StartPythonProcess();
        unityContext = SynchronizationContext.Current;
        aiFunction = GetComponent<AiFunction>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    SendInputToPython("Sup merchant, anything you have to sell?"); // 예시 입력
        //}

        if (Input.GetKeyDown(KeyCode.Numlock))
        {
            Debug.Log("NpcController.FinishPython() : 호출되기 직전");
            Task.Run(() => FinishPython()); // 프로세스 종료
        }
    }

    void OnDestroy()
    {
        Task.Run(() => FinishPython());
    }

    // Python 프로세스를 시작
    private void StartPythonProcess()
    {
        if (isDebugging)
        {
            Debug.Log("NpcController.StartPythonProcess() : 호출됨");
        }

        if (pythonProcess != null)
        {
            Debug.LogWarning("Python process is already running.");
            return;
        }

        pythonProcess = new System.Diagnostics.Process()
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "python", // 이건 무슨 프로그램이냐?
                Arguments = GetPath("PythonLlmTestFour"), // 파이썬 파일 이름
                //WorkingDirectory = "Assets/PythonFiles",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardInputEncoding = Encoding.UTF8
            }
        };

        pythonProcess.Start();

        pythonInput = pythonProcess.StandardInput;
        pythonOutput = pythonProcess.StandardOutput;
        Debug.Log($"기본 입력 및 출력 인코딩 {pythonInput.Encoding} {pythonOutput.CurrentEncoding}");

        cancellationTokenSource = new System.Threading.CancellationTokenSource();
        Task.Run(() => M_ReadPythonOutputAsync(cancellationTokenSource.Token));
    }

    // Python 프로세스에서 출력 읽기 (비동기)
    private async Task M_ReadPythonOutputAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && !pythonProcess.HasExited)
        {
            try
            {
                string line = await pythonOutput.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    Debug.Log($"Python Output: 출력문: {line}");

                    bool isEvent = line.StartsWith("EVENT: ");
                    string parameter = line.Substring(7);
                    if (isEvent && parameter == "receive start")
                    {
                        startReceive = true;
                    }
                    else if (isEvent && parameter == "receive end")
                    {
                        startReceive = false;
                        Debug.Log($"문자열 끄집어내기 : {sharedStringBuilder.ToString()}");
                        int m_functionNumber = int.Parse(sharedStringBuilder.ToString().Split('/')[0]);

                        lock (lockObjectAnswer)
                        {
                            unityContext.Post(_ => {
                                int m_index = sharedStringBuilder.ToString().IndexOf('/');

                                npcResponseText.text = sharedStringBuilder.ToString().Substring(m_index + 1);
                                AiAction(m_functionNumber);
                                sharedStringBuilder.Clear();
                            }, null);
                        }
                    }

                    if (isEvent)
                    {
                        Debug.Log($"Python Output: 파라미터: {parameter}");
                    }
                    if (startReceive && parameter != "receive start")
                    {
                        lock (lockObjectAnswer)
                        {
                            sharedStringBuilder.Append($"{parameter}");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log($"<!>NpcController.M_ReadPythonOutputAsync() : 예외 발생! {ex.Message}");
                Debug.Log($"<!>NpcController.M_ReadPythonOutputAsync() : 예외 스텍 트레이스 : {ex.StackTrace}입니다.");
                Debug.Log($"<!>NpcController.M_ReadPythonOutputAsync() : 예외 소스 : {ex.Source}에 발생함!");

                break;
            }
        }
    }
    //ㅁㅁ
    public void SendInputToPython(string input)
    {
        if (pythonInput == null || pythonProcess.HasExited)
        {
            Debug.LogError("Python process is not running.");
            return;
        }

        pythonInput.WriteLine(input);
        pythonInput.Flush();
        Debug.Log($"Sent to Python: {input}");
    }

    // Python 프로세스 종료
    public async Task FinishPython()
    {
        if (isDebugging)
        {
            Debug.Log($"NpcController.FinishPython() : 호출됨. 타임아웃은 {milisecondForProcessExit}밀리세컨드 입니다.");
        }
        if (pythonProcess == null)
        {
            Debug.LogWarning("Python process is not running.");
            return;
        }
        cancellationTokenSource.Cancel(); // 출력 읽기 작업 취소
        if (isDebugging)
        {
            Debug.Log($"NpcController.FinishPython() : cancellationTokenSource.Cancel(); 통과");
        }



        // 상냥하게 Python 프로세스 종료 시도 + 대기
        try
        {
            Task taskWaitForExit = Task.Run(
                () =>
                {
                    isExited = pythonProcess.WaitForExit(milisecondForProcessExit);
                });
            await taskWaitForExit;
            if (isDebugging)
            {
                Debug.Log($"NpcController.FinishPython() : await taskWaitForExit; 통과");
            }

            if (
                await Task.WhenAny( // 조건 1. 이 두 Task 중에서
                    taskWaitForExit, // Task 1. 프로세스 종료 Task.
                    Task.Delay(milisecondForProcessExit) // Task 2. 딜레이 테스크
                    ) == taskWaitForExit // 가장 먼저 종료된 프로세스가 프로세스 종료 Task 인가?

                && pythonProcess.HasExited // 조건 2. 그리고 파이썬 프로세스가 실제로 종료되었는가?
                )
            {
                if (isDebugging)
                {
                    Debug.Log("Python 프로세스가 정상적으로 종료되었습니다.");
                }
            }
            else
            {
                if (isDebugging)
                {
                    Debug.Log("<?>NpcController.FinishPython() : 악! 타임아웃이 지났는데 파이썬 프로세스 아직 안 주거써요!");
                }
                pythonProcess.Kill();
                if (isDebugging)
                {
                    Debug.Log($"NpcController.FinishPython() : pythonProcess.Kill(); 통과");
                }
                await Task.Run(() => pythonProcess.WaitForExit());
                if (isDebugging)
                {
                    Debug.Log("DEBUG_NpcController.FinishPython() : 프로세스를 강제 종료했습니다.");
                }
            }
        }
        catch (System.Exception ex)
        {
            // 코드에 불이 났다고 개발자에가 알림. 하지만 개발자인 나도 저 불을 어떻게 꺼야 하는지 모를 수 있지만
            Debug.Log($"<!>NpcController.FinishPython() : 예외! {ex.Message}");
            // 로그 코드는 없는 것 보단 나음
        }
        finally
        {
            // 없어지기전에 일단 빨대(스트림) 다 빼
            if (pythonInput != null)
            {
                pythonInput.Flush();  // 입력 스트림에 남아있는 데이터를 모두 처리
                pythonInput.Close();  // 스트림을 닫습니다.
            }
            if (pythonOutput != null)
            {
                pythonOutput.Close();  // 출력 스트림을 닫습니다.
            }
            if (isDebugging)
            {
                Debug.Log($"NpcController.FinishPython() : 입력 및 출력 스트림 종료");
            }

            pythonProcess.Dispose(); // 프로세스 자원을 쓰레기통에 버려버림
            pythonProcess = null; // 나중에 가비지 콜렉터가 알아서 하겠지.
        }
        Debug.Log("Python process has been terminated.");
    }

    private void AiAction(int functionNumner)
    {
        switch (functionNumner)
        {
            case 0:
                Debug.Log("0번 실행");
                break;
            case 1:
                Debug.Log("1번 실행");
                break;
            case 2:
                Debug.Log("2번 실행");
                break;
            case 3:
                Debug.Log("3번 실행");
                break;
            default:
                Debug.Log($"내가 모르는 함수야! functionNumner = {functionNumner}");
                break;
        }
    }
}
