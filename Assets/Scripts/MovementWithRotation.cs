using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
/*
public class MyKeyAttribute : Attribute
{
    public MyKeyAttribute(KeyCode keys)
    {
        Input.GetKey(keys);
    }
}

public enum Key {
           [MyKey(KeyCode.W)]
           W,
           [MyKey(KeyCode.A)]
           A,
           [MyKey(KeyCode.S)]
           S,
           [MyKey(KeyCode.D)]
           D,
           NONE
}
*/
public class MovementWithRotation : MonoBehaviour {

    [SerializeField]
    float speed = 0.2f;
    [SerializeField]
    float turnSpeed = 2.0f;

    //Key keys;
    /*
    private void Start()
    {
        keys = Key.W;
    }
    */
    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position += this.transform.up * this.speed/100;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position -= this.transform.up * this.speed/100;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(0, 0, this.turnSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(0, 0, -this.turnSpeed);
        }
        /*
        switch (keys)
        {
            case Key.W:
                this.transform.position += this.transform.up * this.speed;
                break;
            case Key.S:
                this.transform.position -= this.transform.up * this.speed;
                break;
            case Key.A:
                this.transform.Rotate(0, -this.turnSpeed, 0);
                break;
            case Key.D:
                this.transform.Rotate(0, this.turnSpeed, 0);
                break;
        }
        */
    }
}
