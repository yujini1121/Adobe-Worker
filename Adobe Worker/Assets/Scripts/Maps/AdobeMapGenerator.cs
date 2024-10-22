using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class AdobeMapGenerator : MonoBehaviour
{
    [Header("ûũ ����")]
    [Header("���� ���� �ֿ� ������Դϴ�.\n����ũ����Ʈ�� ûũ�� �����մϴ�.")]
    [SerializeField] List<AdobeMapChunk> chunkPrefab;
    [Header("Ư�� ������ ����� ����Դϴ�.")]
    [SerializeField] List<AdobeMapChunk> registeredPrefab;
    [SerializeField] AdobeMapChunk dirtRoadPrefab;
    [SerializeField] AdobeMapChunk cobbleRoadPrefab;
    
    [SerializeField] Vector2 chunkSize;


    [Header("�� ����")]
    [SerializeField] Vector2 mapSize;
    [SerializeField] int padding; // ��� ����� �߽� ������������ �߰��� �ٴ� �ܺ� ��� ����
    [SerializeField] int radius; //�߽� �������� ��� ����� �߽� ��������
    int middleX;
    int middleY;
    float radiusX;
    float radiusY;
    bool[,] isInstantiated;
    int[,] mapPrefabIndexes;
    int[,] mapChunkType;
    List<(Vector2, GameObject)> instantiatedChunkList; // O(Xsize * Ysize)���� �� ���� �� �ִ� Ž���� ���� �ڷᱸ���Դϴ�.
    Vector3 pivot;

    [Header("�����")]
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
        // �Ź� �÷��̾��� ��ġ�� �˻�
        // pivot �������� ��û �־����� ������ ArrangeMap() ȣ��

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
        Debug.Log("�� ����ŷ ����");
    }

    void ArrangeMap()
    {
        if (isDebuggingMethodCall)
        {
            Debug.Log("AdobeMapGenerator.ArrangeMap()");
        }

        // �ڽ��� ��ġ ������
        Vector3 position = AdobePlayerReference.playerInstance.transform.position;
        int posiitonIndexX = middleX + (int)Mathf.Floor((position.x + chunkSize.x / 2) / chunkSize.x);
        int posiitonIndexY = middleY + (int)Mathf.Floor((position.z + chunkSize.y / 2) / chunkSize.y);

        Debug.Log($"posIndex = {posiitonIndexX} {posiitonIndexY}");
        Debug.Log($"xa = {(int)Mathf.Floor((position.x - chunkSize.x / 2) / chunkSize.x)}");
        pivot = new Vector3((posiitonIndexX - middleX) * chunkSize.x, 0, (posiitonIndexY - middleY) * chunkSize.y);

        // ûũ ����
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
                    Debug.Log($"AdobeMapGenerator.ArrangeMap() : {xIndex} / {yIndex}, ���� ��ǥ : {generatePosition}");
                }


                GameObject m_chunk = Instantiate(
                    chunkPrefab[mapPrefabIndexes[xIndex, yIndex]].prefab,
                    generatePosition,
                    chunkPrefab[mapPrefabIndexes[xIndex, yIndex]].prefab.transform.rotation
                    );

                instantiatedChunkList.Add((new Vector2(xIndex, yIndex), m_chunk));
            }
        }

        // �־��� ûũ �߶󳻱�
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
