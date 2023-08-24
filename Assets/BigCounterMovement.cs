using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCounterMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] int speed = 5;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(1, 0, 1);
        rb.angularVelocity = new Vector3(0, 2f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        rb.angularVelocity = new Vector3(0, 2f, 0);
        if (rb.position.z < 0)
        {
            rb.velocity = new Vector3(-speed, 0, rb.velocity.z);
        }
        else
        {
            rb.velocity = new Vector3(speed, 0, rb.velocity.z);
        }
        if (rb.position.x < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, speed);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, -speed);
        }
    }
}
