using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeMapGenerator : MonoBehaviour
{
    [Header("ûũ ����")]
    [Header("���� ���� �ֿ� ������Դϴ�.\n����ũ����Ʈ�� ûũ�� �����մϴ�.")]
    [SerializeField] List<AdobeMapChunk> chunkPrefab;
    [Header("Ư�� ������ ����� ����Դϴ�.")]
    [SerializeField] List<AdobeMapChunk> registeredPrefab;

    
    [SerializeField] Vector2 chunkSize;


    [Header("�� ����")]
    [SerializeField] Vector2 mapSize;
    [SerializeField] int padding; // ��� ����� �߽� ������������ �߰��� �ٴ� �ܺ� ��� ����
    [SerializeField] int radius; //�߽� �������� ��� ����� �߽� ��������
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
        // �Ź� �÷��̾��� ��ġ�� �˻�

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
        Debug.Log("�� ����ŷ ����");
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
