using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float playerRadius = 0.9f;
    [SerializeField] float playerHeight = 3f;
    [SerializeField] GameInput gameInput;
    bool isWalking;
    private void Update()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirX = new Vector3(inputVector.x, 0f, 0f);
        Vector3 moveDirY = new Vector3(0f, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMoveX = CanMove(moveDirX, moveDistance);
        bool canMoveY = CanMove(moveDirY, moveDistance);

        Vector3 moveDir = new Vector3(0f, 0f, 0f);
        if (canMoveX) 
        {
            moveDir.x = inputVector.x;
        }
        if (canMoveY)
        {
            moveDir.z = inputVector.y;
        }

        transform.position += moveDir * moveDistance;

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    bool CanMove(Vector3 dir, float dist)
    {
        return !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, dir, dist);
    }

    public bool IsWalking() 
    {
        return isWalking;
    }
}
