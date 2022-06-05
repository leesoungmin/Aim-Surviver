using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMagazine : ItemBase
{
    [SerializeField] GameObject magazineEffectPrefab;
    [SerializeField] int increaseMagazine = 2;
    [SerializeField] float rotateSpeed = 50;

    private IEnumerator Start()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {
        entity.GetComponentInChildren<WeaponAssaultlife>().IncreaseMagazine(increaseMagazine);

        Instantiate(magazineEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
