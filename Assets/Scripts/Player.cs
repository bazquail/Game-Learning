using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] float slowDown = 0.85f;
    [SerializeField] float maxVelocity = 10f;
    [SerializeField] float jumpHeight = 20f;
    [SerializeField] float gravityScale = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float raycastDist = 0.6f;
    [SerializeField] GameInput gameInput;
    bool isWalkingAnim = false;
    bool isGrounded = true;
    Rigidbody rb;
    Animator animator;
    bool isWalking;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Gravity();
        GroundCheck();
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        isWalking = moveDir != Vector3.zero;

        if (isWalking && isGrounded)
        {
            animator.SetBool("IsRunning 0", true);
        }
        else
        {
            animator.SetBool("IsRunning 0", false);
        }
        if (isWalking)
        {
            float firstVx = rb.velocity.x;
            float firstVz = rb.velocity.z;
            float secondVx = moveDir.x * moveSpeed;
            float secondVz = moveDir.z * moveSpeed;

            rb.velocity = new Vector3(Mathf.Lerp(firstVx, secondVx, 0.1f), rb.velocity.y, Mathf.Lerp(firstVz, secondVz, 0.1f));
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.25f);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x * slowDown, rb.velocity.y, rb.velocity.z * slowDown);
        }
    }

    void Gravity()
    {
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
    }

    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDist, groundLayer))
        {
            isGrounded = true;
        }
        else{
            isGrounded = false;
            animator.SetBool("IsRunning 0", false);
        }
    }

    public bool IsWalkingAnim() 
    {
        return isWalkingAnim;
    }

    public void Jump()
    {
        Debug.Log("This one is running, the jump method in player");
        if (isGrounded)
        {
            Debug.Log("The jump method");
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
        }
    }
}
