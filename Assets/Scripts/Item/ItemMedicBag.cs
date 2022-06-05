using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMedicBag : ItemBase
{
    [SerializeField] GameObject hpEffectPrefab;
    [SerializeField] int increaseHP = 50;
    [SerializeField] float moveDistance = 0.2f;
    [SerializeField] float pingpongSpeed = 0.5f;
    [SerializeField] float rotateSpeed = 50;

    private IEnumerator Start()
    {
        float y = transform.position.y;

        while (true)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
            transform.position = pos;

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {
        entity.GetComponent<PlayerStatus>().IncreaseHP(increaseHP);

        Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
