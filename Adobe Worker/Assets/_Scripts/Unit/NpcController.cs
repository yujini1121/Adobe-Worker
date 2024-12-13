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
    // ���̽� �ڵ尡 �����ϴ� �� : "�Լ���ȣ / ��� ���ڿ�"
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
        //    SendInputToPython("Sup merchant, anything you have to sell?"); // ���� �Է�
        //}

        if (Input.GetKeyDown(KeyCode.Numlock))
        {
            Debug.Log("NpcController.FinishPython() : ȣ��Ǳ� ����");
            Task.Run(() => FinishPython()); // ���μ��� ����
        }
    }

    void OnDestroy()
    {
        Task.Run(() => FinishPython());
    }

    // Python ���μ����� ����
    private void StartPythonProcess()
    {
        if (isDebugging)
        {
            Debug.Log("NpcController.StartPythonProcess() : ȣ���");
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
                FileName = "python", // �̰� ���� ���α׷��̳�?
                Arguments = GetPath("PythonLlmTestFour"), // ���̽� ���� �̸�
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
        Debug.Log($"�⺻ �Է� �� ��� ���ڵ� {pythonInput.Encoding} {pythonOutput.CurrentEncoding}");

        cancellationTokenSource = new System.Threading.CancellationTokenSource();
        Task.Run(() => M_ReadPythonOutputAsync(cancellationTokenSource.Token));
    }

    // Python ���μ������� ��� �б� (�񵿱�)
    private async Task M_ReadPythonOutputAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && !pythonProcess.HasExited)
        {
            try
            {
                string line = await pythonOutput.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    Debug.Log($"Python Output: ��¹�: {line}");

                    bool isEvent = line.StartsWith("EVENT: ");
                    string parameter = line.Substring(7);
                    if (isEvent && parameter == "receive start")
                    {
                        startReceive = true;
                    }
                    else if (isEvent && parameter == "receive end")
                    {
                        startReceive = false;
                        Debug.Log($"���ڿ� ������� : {sharedStringBuilder.ToString()}");
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
                        Debug.Log($"Python Output: �Ķ����: {parameter}");
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
                Debug.Log($"<!>NpcController.M_ReadPythonOutputAsync() : ���� �߻�! {ex.Message}");
                Debug.Log($"<!>NpcController.M_ReadPythonOutputAsync() : ���� ���� Ʈ���̽� : {ex.StackTrace}�Դϴ�.");
                Debug.Log($"<!>NpcController.M_ReadPythonOutputAsync() : ���� �ҽ� : {ex.Source}�� �߻���!");

                break;
            }
        }
    }
    //����
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

    // Python ���μ��� ����
    public async Task FinishPython()
    {
        if (isDebugging)
        {
            Debug.Log($"NpcController.FinishPython() : ȣ���. Ÿ�Ӿƿ��� {milisecondForProcessExit}�и������� �Դϴ�.");
        }
        if (pythonProcess == null)
        {
            Debug.LogWarning("Python process is not running.");
            return;
        }
        cancellationTokenSource.Cancel(); // ��� �б� �۾� ���
        if (isDebugging)
        {
            Debug.Log($"NpcController.FinishPython() : cancellationTokenSource.Cancel(); ���");
        }



        // ����ϰ� Python ���μ��� ���� �õ� + ���
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
                Debug.Log($"NpcController.FinishPython() : await taskWaitForExit; ���");
            }

            if (
                await Task.WhenAny( // ���� 1. �� �� Task �߿���
                    taskWaitForExit, // Task 1. ���μ��� ���� Task.
                    Task.Delay(milisecondForProcessExit) // Task 2. ������ �׽�ũ
                    ) == taskWaitForExit // ���� ���� ����� ���μ����� ���μ��� ���� Task �ΰ�?

                && pythonProcess.HasExited // ���� 2. �׸��� ���̽� ���μ����� ������ ����Ǿ��°�?
                )
            {
                if (isDebugging)
                {
                    Debug.Log("Python ���μ����� ���������� ����Ǿ����ϴ�.");
                }
            }
            else
            {
                if (isDebugging)
                {
                    Debug.Log("<?>NpcController.FinishPython() : ��! Ÿ�Ӿƿ��� �����µ� ���̽� ���μ��� ���� �� �ְŽ��!");
                }
                pythonProcess.Kill();
                if (isDebugging)
                {
                    Debug.Log($"NpcController.FinishPython() : pythonProcess.Kill(); ���");
                }
                await Task.Run(() => pythonProcess.WaitForExit());
                if (isDebugging)
                {
                    Debug.Log("DEBUG_NpcController.FinishPython() : ���μ����� ���� �����߽��ϴ�.");
                }
            }
        }
        catch (System.Exception ex)
        {
            // �ڵ忡 ���� ���ٰ� �����ڿ��� �˸�. ������ �������� ���� �� ���� ��� ���� �ϴ��� �� �� ������
            Debug.Log($"<!>NpcController.FinishPython() : ����! {ex.Message}");
            // �α� �ڵ�� ���� �� ���� ����
        }
        finally
        {
            // ������������ �ϴ� ����(��Ʈ��) �� ��
            if (pythonInput != null)
            {
                pythonInput.Flush();  // �Է� ��Ʈ���� �����ִ� �����͸� ��� ó��
                pythonInput.Close();  // ��Ʈ���� �ݽ��ϴ�.
            }
            if (pythonOutput != null)
            {
                pythonOutput.Close();  // ��� ��Ʈ���� �ݽ��ϴ�.
            }
            if (isDebugging)
            {
                Debug.Log($"NpcController.FinishPython() : �Է� �� ��� ��Ʈ�� ����");
            }

            pythonProcess.Dispose(); // ���μ��� �ڿ��� �������뿡 ��������
            pythonProcess = null; // ���߿� ������ �ݷ��Ͱ� �˾Ƽ� �ϰ���.
        }
        Debug.Log("Python process has been terminated.");
    }

    private void AiAction(int functionNumner)
    {
        switch (functionNumner)
        {
            case 0:
                Debug.Log("0�� ����");
                break;
            case 1:
                Debug.Log("1�� ����");
                break;
            case 2:
                Debug.Log("2�� ����");
                break;
            case 3:
                Debug.Log("3�� ����");
                break;
            default:
                Debug.Log($"���� �𸣴� �Լ���! functionNumner = {functionNumner}");
                break;
        }
    }
}
