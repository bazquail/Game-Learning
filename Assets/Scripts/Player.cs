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
    [SerializeField] float slowDown = 0.8f;
    [SerializeField] float maxVelocity = 10f;
    [SerializeField] float jumpHeight = 20f;
    [SerializeField] float gravityScale = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float raycastDist = 0.7f;
    [SerializeField] GameInput gameInput;
    float timeSinceLastJump = 0;
    float timeSinceLanding = 0;
    bool isGrounded = true;
    Rigidbody rb;
    Animator animator;
    bool isRunning;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        float test = 4f;
        Gravity();
        GroundCheck();
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        bool movingX = (rb.velocity.x > test) || (rb.velocity.x < -test);
        bool movingZ = (rb.velocity.z > test) || (rb.velocity.z < -test);
        
        if ((movingX || movingZ) && isGrounded)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (moveDir != Vector3.zero)
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
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
            if (isRunning)
            {
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsRunning", false);
            }

            if (timeSinceLastJump > 0.4)
            {
                animator.SetBool("IsLanding", true);
                timeSinceLanding = 0;
            }
            if (timeSinceLanding > 0.1)
            {
                animator.SetBool("IsLanding", false);
            }
            timeSinceLastJump = 0;
            timeSinceLanding += Time.deltaTime;
        }
        else{
            isGrounded = false;
            animator.SetBool("IsRunning", false);
            timeSinceLastJump += Time.deltaTime;
            Debug.Log(timeSinceLastJump);
            if (timeSinceLastJump > 0.4)
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            animator.SetBool("IsLanding", false);
            animator.SetBool("IsJumping", true);
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
        }
    }
}
