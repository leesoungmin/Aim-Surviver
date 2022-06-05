using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.0f;
    [SerializeField] Vector3 moveDirection = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void MoveTo(Vector3 dir)
    {
        moveDirection = dir;
    }
}
