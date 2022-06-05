using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Vector3 moveForece;

    [SerializeField] float jumpForce;
    [SerializeField] float gravity; 

    CharacterController characterController;

    public float MoveSpeed
    {
        set => moveSpeed = Mathf.Max(0, value);
        get => moveSpeed;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!characterController.isGrounded)
        {
            moveForece.y += gravity * Time.deltaTime;
        }

        characterController.Move(moveForece * Time.deltaTime);
    }

    public void MoveTo(Vector3 dir)
    {
        dir = transform.rotation * new Vector3(dir.x, 0, dir.z);
        moveForece = new Vector3(dir.x * moveSpeed, moveForece.y, dir.z * moveSpeed);
    }

    public void Jump()
    {
        if(characterController.isGrounded)
        {
            moveForece.y = jumpForce;
        }
    }
}
