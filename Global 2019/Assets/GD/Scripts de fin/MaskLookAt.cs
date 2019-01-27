using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskLookAt : MonoBehaviour
{
    public Transform camTransform;
    public GameObject mask;

    private void LateUpdate()
    {
        transform.LookAt(camTransform);

        RaycastHit hit;
        Debug.DrawLine(transform.position, camTransform.position, Color.red, 5f); 

        if (Physics.Linecast(transform.position, camTransform.position, out hit))
        {
            if (hit.collider.transform.parent.GetComponent<MeshRenderer>() != null && hit.collider.transform.parent.GetComponent<MeshRenderer>().material.shader.name == "Custom/Wall")
            {
                mask.SetActive(true);
            }


        }


        else if (mask.activeInHierarchy)
        {
            mask.SetActive(false);
        }
    }

}