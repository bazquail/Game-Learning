using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1000f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] float slowDown = 0.9f;
    [SerializeField] float maxVelocity = 10f;
    [SerializeField] GameInput gameInput;
    Rigidbody rb;
    bool isWalking;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        isWalking = moveDir != Vector3.zero;

        float moveDistance = moveSpeed * Time.deltaTime;

        if (isWalking && (rb.velocity.magnitude < maxVelocity))
        {
            rb.AddForce(moveDir * moveDistance);
        }
        if (!isWalking)
        {
            rb.velocity = rb.velocity * slowDown;
        }
        //this.GetComponent<Rigidbody>().AddTorque(moveDir * rotateSpeed);
    }

    public bool IsWalking() 
    {
        return isWalking;
    }
}
