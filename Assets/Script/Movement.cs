using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Rigidbody rb;

    public float speed;

    Vector2 moveDir;
    bool isMoving;

    void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();    

    }
    /*
    private void Update()
    {
        moveDir = playernput.moveInput.normalized;
        isMoving = Convert.ToBoolean(moveDir.magnitude);

    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(x:moveDir.x * speed, y:moveDir.y * speed);
    }
    */
}
