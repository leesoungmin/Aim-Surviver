using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] GameObject enemySpawnPointPrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float enemySpawnTime = 1;
    [SerializeField] float enemySpawnLatency = 1;       //타일 생성후 적의 등장하기까지의 대기 시간

    MemoryPool spawnPointMemoryPool;
    MemoryPool enemyMemoryPool;

    int numberOfEnemiesSpawnedAtOnce = 1;       //동시에 생성되는 적의 숫자
    Vector2Int mapSize = new Vector2Int(100, 100);  //맵 크기

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }

    IEnumerator SpawnTile()
    {
        int curNum = 0;
        int maxNum = 50;

        while (true)
        {
            for (int i =0; i < numberOfEnemiesSpawnedAtOnce; ++i)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();

                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 1
                    , Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));

                StartCoroutine("EnemySpawn", item);
            }

            curNum++;

            if(curNum >= maxNum)
            {
                curNum = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }
            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    IEnumerator EnemySpawn(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        item.GetComponent<EnemyFSM>().Setup(target, this);

        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }
}
