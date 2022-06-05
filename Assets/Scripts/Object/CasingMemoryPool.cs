using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField] GameObject caseingPrefab;

    MemoryPool memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool(caseingPrefab);
    }

    public void SpawnCasing(Vector3 pos, Vector3 dir)
    {
        GameObject item = memoryPool.ActivatePoolItem();
        item.transform.position = pos;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(memoryPool, dir);
    }
}
