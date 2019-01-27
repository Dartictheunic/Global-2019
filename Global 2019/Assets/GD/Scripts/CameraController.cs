using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minInput;
    public float turningSpeed;
    public Transform player;
    Vector3 playerLastPos;
    Vector3 playerNewPos;

    private void Awake()
    {
        playerNewPos = player.position;
        playerLastPos = player.position;
    }
    private void FixedUpdate()
    {
        //transform.LookAt(player, Vector3.up);
        if (Mathf.Abs(Input.GetAxisRaw("CamX")) > minInput)
        {
            transform.RotateAround(player.position, player.up, Input.GetAxisRaw("CamX") * turningSpeed);
        }

        if (Mathf.Abs(Input.GetAxisRaw("CamY")) > minInput)
        {
            transform.RotateAround(player.position, transform.right, Input.GetAxisRaw("CamY") * turningSpeed);
        }

    }

    private void LateUpdate()
    {
        playerNewPos = player.position;
        transform.position += playerNewPos - playerLastPos;
        playerLastPos = player.position;
    }
}
