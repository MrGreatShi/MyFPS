using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 60f;
    public float lifeTime = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        rb.AddForce(-transform.right * speed, ForceMode.Impulse);
        GetComponent<PooledObject>()?.Despawn(lifeTime);
    }

    void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}
