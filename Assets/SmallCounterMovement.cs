using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCounterMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] int time = 4;

    float cumulativeTime;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(-4, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (cumulativeTime > time)
        {
            rb.velocity *= -1;
            cumulativeTime = 0;
        }
        cumulativeTime += Time.deltaTime;
    }
}
