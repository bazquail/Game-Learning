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
    Rigidbody objRb;
    Rigidbody rb;
    Animator animator;
    bool isRunning;
    float lastY = 0;
    Vector3 posDif;
    float deltaTheta = 0;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        float test = 2f;
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float objx = 0;
        float objz = 0;
        float rbXvel = 0;
        float rbZvel = 0;
        if (objRb != null && deltaTheta != 0)
        {
            deltaTheta = objRb.rotation.eulerAngles.y - lastY;
            lastY = objRb.rotation.eulerAngles.y;

            float objxPos = objRb.position.x;
            float objzPos = objRb.position.z;

            float pxPos = rb.position.x;
            float pzPos = rb.position.z;

            posDif = new Vector3(pxPos - objxPos, 0, pzPos - objzPos);

            float xDir = 1;
            float zDir = 1;

            if (posDif.x > 0 && posDif.z > 0)
            {
                // x dec, z inc
                zDir *= -1;
            } 
            else if (posDif.x < 0 && posDif.z > 0)
            {
                // x dec z dec by sin(x/z), cos(x/z) respectively
                xDir *= -1;
                zDir *= 1;
            }
            else if (posDif.x < 0 && posDif.z < 0)
            {
                // x inc, z dec
                xDir *= -1;
            }
            else if (posDif.x > 0 && posDif.z < 0)
            {
                // x inc, z inc
                zDir *= -1;
            }
            if (deltaTheta < 0)
            {
                xDir *= -1;
                zDir *= -1;
            }
            float angle = (float) Math.Acos(posDif.x/posDif.magnitude);
            float rads = objRb.angularVelocity.y;
            rbXvel = (float) (xDir*posDif.magnitude*rads*Math.Cos(angle));
            rbZvel = (float) (zDir*posDif.magnitude*rads*Math.Sin(angle));
            Debug.Log($"{angle} {rbXvel} {rbZvel}");

            objx = objRb.velocity.x;
            objz = objRb.velocity.z;

        }
        else if (objRb != null)
        {
            objx = objRb.velocity.x;
            objz = objRb.velocity.z;

            deltaTheta = objRb.rotation.eulerAngles.y - lastY;
            lastY = objRb.rotation.eulerAngles.y;
        }
        else
        {
            deltaTheta = 0;
            lastY = 0;
        }
        test += (float) Math.Sqrt(objx*objx + objz*objz);

        bool movingX = (rb.velocity.x > test) || (rb.velocity.x < -test);
        bool movingZ = (rb.velocity.z > test) || (rb.velocity.z < -test);
        
        if ((movingX || movingZ) && isGrounded)
        {
            isRunning = true;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
            animator.ResetTrigger("LandRun");
        }

        if (!movingX && !movingZ)
        {
            animator.SetBool("IsIdle", true);
        }

        
        float firstVx = rb.velocity.x;
        float firstVz = rb.velocity.z;
        float secondVx = (moveDir.x * moveSpeed) + objx + rbXvel;
        float secondVz = (moveDir.z * moveSpeed) + objz + rbZvel;

        rb.velocity = new Vector3(Mathf.Lerp(firstVx, secondVx, 0.4f), rb.velocity.y, Mathf.Lerp(firstVz, secondVz, 0.4f));

        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 0.25f);
            animator.SetBool("IsIdle", false);
        }
        else
        {
            //rb.velocity = new Vector3(rb.velocity.x * slowDown, rb.velocity.y, rb.velocity.z * slowDown);
            isRunning = false;
        }
        Gravity();
        GroundCheck();
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
            GetVelocityOfHit(hit);
            isGrounded = true;
            animator.SetBool("IsFalling", false);

            if (isRunning && timeSinceLastJump > 0.2)
            {
                animator.SetTrigger("LandRun");
                animator.ResetTrigger("Jump");
            }
            else if (timeSinceLastJump > 0.2)
            {
                animator.SetTrigger("Land");
                animator.ResetTrigger("Jump");
            }

            timeSinceLastJump = 0;
        }
        else
        {
            objRb = null;
            isGrounded = false;
            animator.SetBool("IsRunning", false);
            timeSinceLastJump += Time.deltaTime;
            if (timeSinceLastJump > 0.4)
            {
                animator.SetBool("IsFalling", true);
            }
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            animator.ResetTrigger("Land");
            animator.ResetTrigger("LandRun");
            animator.SetTrigger("Jump");
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
        }
    }

    void GetVelocityOfHit(RaycastHit hit)
    {
        objRb = hit.transform.gameObject.GetComponent<Rigidbody>();
        lastY = objRb.rotation.eulerAngles.y;

        // will need to get velocity in x and z of object bean is on
        // then set the beans "0" velocity to that when checking for its velocity
        // essentially, have a set velocity method that will do everything (the lerps, 0 checks) as above but adding in the components of the objects velocity
    }
}
