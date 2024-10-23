using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class AdobeMapGenerator : MonoBehaviour
{
    [Header("청크 설정")]
    [Header("게임 맵의 주요 덩어리들입니다.\n마인크래프트의 청크랑 유사합니다.")]
    [SerializeField] List<AdobeMapChunk> chunkPrefab;
    [Header("특정 로직이 적용된 덩어리입니다.")]
    [SerializeField] List<AdobeMapChunk> registeredPrefab;
    [SerializeField] AdobeMapChunk dirtRoadPrefab;
    [SerializeField] AdobeMapChunk cobbleRoadPrefab;
    
    [SerializeField] Vector2 chunkSize;


    [Header("맵 설정")]
    [SerializeField] Vector2 mapSize;
    [SerializeField] int padding; // 경계 블록의 중심 지점에서부터 추가로 붙는 외부 블록 층수
    [SerializeField] int radius; //중심 지점에서 경계 블록의 중심 지점까지
    int middleX;
    int middleY;
    float radiusX;
    float radiusY;
    bool[,] isInstantiated;
    int[,] mapPrefabIndexes;
    int[,] mapChunkType;
    List<(Vector2, GameObject)> instantiatedChunkList; // O(Xsize * Ysize)보다 더 줄일 수 있는 탐색을 위한 자료구조입니다.
    Vector3 pivot;

    [Header("디버깅")]
    [SerializeField] bool isDebuggingMethodCall;
    [SerializeField] bool isDebuggingArrangeMap;

    // Start is called before the first frame update
    void Start()
    {
        InitField();
        BakeMap();
        ArrangeMap();


    }

    // Update is called once per frame
    void Update()
    {
        // 매번 플레이어의 위치를 검사
        // pivot 기준으로 엄청 멀어졌다 느끼면 ArrangeMap() 호출

        if (IsNeedMapUpdate())
        {
            ArrangeMap();
        }

        //MapUpdateHandler(1, 1);
    }

    bool IsNeedMapUpdate()
    {
        Vector3 position = AdobePlayerReference.playerInstance.transform.position;

        if (Mathf.Abs((position - pivot).x) > radiusX) return true;
        if (Mathf.Abs((position - pivot).z) > radiusY) return true;
        return false;
    }

    void BakeMap()
    {
        int m_xSize = (int)mapSize.x;
        int m_ySize = (int)mapSize.y;

        mapPrefabIndexes = new int[m_xSize, m_ySize];

        float max = 0.0f;
        for (int index = 0; index < chunkPrefab.Count; ++index)
        {
            max += chunkPrefab[index].weight;
        }

        for (int x = 0; x < m_xSize; ++x)
        {
            for (int y = 0; y < m_ySize; ++y)
            {
                float one = Random.Range(0.0f, max);
                int index = 0;
                for (; index < chunkPrefab.Count - 1; ++index)
                {
                    if (one <= chunkPrefab[index].weight) break;

                    one -= chunkPrefab[index].weight;
                }

                mapPrefabIndexes[x, y] = index;
            }
        }

        mapPrefabIndexes[middleX, middleY] = 0;
        Debug.Log("맵 베이킹 성공");
    }

    void ArrangeMap()
    {
        if (isDebuggingMethodCall)
        {
            Debug.Log("AdobeMapGenerator.ArrangeMap()");
        }

        // 자신의 위치 가져옴
        Vector3 position = AdobePlayerReference.playerInstance.transform.position;
        int posiitonIndexX = middleX + (int)Mathf.Floor((position.x + chunkSize.x / 2) / chunkSize.x);
        int posiitonIndexY = middleY + (int)Mathf.Floor((position.z + chunkSize.y / 2) / chunkSize.y);

        Debug.Log($"posIndex = {posiitonIndexX} {posiitonIndexY}");
        Debug.Log($"xa = {(int)Mathf.Floor((position.x - chunkSize.x / 2) / chunkSize.x)}");
        pivot = new Vector3((posiitonIndexX - middleX) * chunkSize.x, 0, (posiitonIndexY - middleY) * chunkSize.y);

        // 청크 생성
        int m_size = 1 + radius * 2 + padding * 2;
        for (int x = 0; x < m_size; ++x)
        {
            int xIndex = x - radius - padding + posiitonIndexX;
            if (xIndex < 0 || xIndex >= (int)mapSize.x) continue;

            for (int y = 0; y < m_size; ++y)
            {
                int yIndex = y - radius - padding + posiitonIndexY;
                if (yIndex < 0 || yIndex >= (int)mapSize.y) continue;

                if (isInstantiated[xIndex, yIndex]) continue;
                isInstantiated[xIndex, yIndex] = true;

                Vector3 generatePosition = new Vector3(
                    (xIndex - middleX) * chunkSize.x,
                    -0.5f,
                    (yIndex - middleY) * chunkSize.y
                    );

                if (isDebuggingArrangeMap)
                {
                    Debug.Log($"AdobeMapGenerator.ArrangeMap() : {xIndex} / {yIndex}, 생성 좌표 : {generatePosition}");
                }


                GameObject m_chunk = Instantiate(
                    chunkPrefab[mapPrefabIndexes[xIndex, yIndex]].prefab,
                    generatePosition,
                    chunkPrefab[mapPrefabIndexes[xIndex, yIndex]].prefab.transform.rotation
                    );

                instantiatedChunkList.Add((new Vector2(xIndex, yIndex), m_chunk));
            }
        }

        // 멀어진 청크 잘라내기
        for (int index = instantiatedChunkList.Count - 1; index >= 0; --index)
        {
            (Vector2 m_oneIndex, GameObject m_chunk) = instantiatedChunkList[index];

            int x = Mathf.FloorToInt(m_oneIndex.x);
            int y = Mathf.FloorToInt(m_oneIndex.y);



        }
    }

    void InitField()
    {
        isInstantiated = new bool[(int)mapSize.x, (int)mapSize.y];
        middleX = (int)(mapSize.x / 2);
        middleY = (int)(mapSize.y / 2);
        radiusX = chunkSize.x * radius + chunkSize.x / 2;
        radiusY = chunkSize.y * radius + chunkSize.y / 2;

        pivot = new Vector3(0, 0, 0);
        instantiatedChunkList = new List<(Vector2, GameObject)>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pivot, 1);
        Gizmos.color = Color.green;


    }
}
