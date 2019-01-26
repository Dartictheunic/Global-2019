using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IamPickable : MonoBehaviour
{
    public GameObject inRangeToPick()
    {
        return transform.gameObject; 
    }
}
