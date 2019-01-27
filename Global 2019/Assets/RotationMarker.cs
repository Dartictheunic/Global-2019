using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMarker : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(-Input.GetAxisRaw("Horizontal"), 0, -Input.GetAxisRaw("Vertical")) + player.transform.position;
        transform.rotation = Quaternion.identity;
    }
}
