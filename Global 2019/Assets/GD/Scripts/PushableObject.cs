using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour, IPushable
{
    Rigidbody myBody;

    private void Start()
    {
        myBody = GetComponent<Rigidbody>();
    }
    public void Push(Vector3 velocity)
    {
        myBody.velocity = velocity;
    }
}