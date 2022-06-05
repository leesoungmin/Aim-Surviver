using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 4f;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    //오브젝트가 활성화 될 때
    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect");
    }

    //오브젝트가 비활성화 될 때
    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect");
    }

    IEnumerator OnFadeEffect()
    {
        while (true)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
