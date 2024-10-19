using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeMapGenerator : MonoBehaviour
{
    [Header("청크 설정")]
    [Header("게임 맵의 주요 덩어리들입니다.\n마인크래프트의 청크랑 유사합니다.")]
    [SerializeField] List<AdobeMapChunk> chunkPrefab;
    [Header("특정 로직이 적용된 덩어리입니다.")]
    [SerializeField] List<AdobeMapChunk> registeredPrefab;

    
    [SerializeField] Vector2 chunkSize;


    [Header("맵 설정")]
    [SerializeField] Vector2 mapSize;
    [SerializeField] int padding; // 경계 블록의 중심 지점에서부터 추가로 붙는 외부 블록 층수
    [SerializeField] int radius; //중심 지점에서 경계 블록의 중심 지점까지
    //int middle = 
    bool[,] isInstantiated;
    int[,] mapPrefabIndexes;

    System.Action<int, int> MapUpdateHandler;
    

    // Start is called before the first frame update
    void Start()
    {
        BakeMap();
        InitField();
        ArrangeMap();


    }

    // Update is called once per frame
    void Update()
    {
        // 매번 플레이어의 위치를 검사

        //MapUpdateHandler(1, 1);
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
        Debug.Log("맵 베이킹 성공");
    }

    void ArrangeMap()
    {
        
    }

    void InitField()
    {
        isInstantiated = new bool[1 + radius * 2 + padding * 2, 1 + radius * 2 + padding * 2];
        
        for (int x = 0; x < isInstantiated.GetLength(0); ++x)
        {
            for (int y = 0; y < isInstantiated.GetLength(1); ++y)
            {
                bool m_value = true;

                if (x < padding) m_value = false;
                if (x > radius * 2 + padding) m_value = false;
                if (y < padding) m_value = false;
                if (y > radius * 2 + padding) m_value = false;

                isInstantiated[x, y] = m_value;
            }
        }

    }
}
