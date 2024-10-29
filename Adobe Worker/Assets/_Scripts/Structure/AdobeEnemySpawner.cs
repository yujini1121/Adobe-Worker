using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdobeEnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> Enemies;
    [SerializeField] float spawnPeriod;
    [SerializeField] float spawnPositionRange;
    [SerializeField] float spawnContitionDistnace;
    float NextSpawnedTime;
    float spawnDistanceSquare;

    // Start is called before the first frame update
    void Start()
    {
        spawnDistanceSquare = spawnContitionDistnace * spawnContitionDistnace;
        NextSpawnedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsNeedSpawn())
        {
            NextSpawnedTime = Time.time + spawnPeriod;
            Spawn();
        }
    }

    void Spawn()
    {
        int index = Random.Range(0, Enemies.Count);
        Vector3 pos = transform.position;
        pos.x += Mathf.Cos(Random.Range(0.0f, 2.0f)) * spawnPositionRange; 
        pos.z += Mathf.Sin(Random.Range(0.0f, 2.0f)) * spawnPositionRange;

        Instantiate(Enemies[index], pos, Quaternion.identity);
    }

    bool IsNeedSpawn()
    {
        if (NextSpawnedTime > Time.time) return false;
        if ((transform.position - AdobePlayerReference.playerInstance.transform.position).sqrMagnitude > spawnDistanceSquare) return false;
        return true;
    }
}
