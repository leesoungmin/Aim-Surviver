﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType { Normal = 0, Obstacle, Enemy, InteractionObject}

public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField] GameObject[] impactPrefab;
    MemoryPool[] memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool[impactPrefab.Length];
        for(int i =0; i<impactPrefab.Length; ++i)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        if(hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("InteractionObject"))
        {
            Color color = hit.transform.GetComponentInChildren<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
        }
    }

    public void OnSpawnImpact(ImpactType type, Vector3 pos, Quaternion rot, Color color = new Color())
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = pos;
        item.transform.rotation = rot;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

        if(type == ImpactType.InteractionObject)
        {
            ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
            main.startColor = color;
        }
    }
}
