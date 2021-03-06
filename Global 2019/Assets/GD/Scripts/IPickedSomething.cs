﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPickedSomething : MonoBehaviour
{
    public PlayerController myPlayer;
    [SerializeField]

    private List<float> distancesToPlayer;
    [SerializeField]
    private List<GameObject> objectInRange;
    [SerializeField]
    private bool poolingAnObject;

    [Header("REFERENCE A FAIRE")]
    public Transform main;
    public GameObject slotVide;

    [SerializeField]
    private GameObject inSlot;

    private Vector3 slotDifferenceAtPickup;
    private Transform slotOldTransform;
    private void Start()
    {
        distancesToPlayer = new List<float>();
        objectInRange = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IamPickable>() != null)
        {
            IamPickable tREf = other.GetComponent<IamPickable>();
            objectInRange.Add(tREf.inRangeToPick());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IamPickable>() != null)
        {
            IamPickable tREf = other.GetComponent<IamPickable>();
            objectInRange.Remove(tREf.inRangeToPick());
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Push") && poolingAnObject == false && objectInRange.Count > 1)
        {
            for (int i = 0; i < objectInRange.Count; i++)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, objectInRange[i].transform.position);
                distancesToPlayer.Add(distanceToPlayer);
            }

            ReorganizeLists();

            inSlot = objectInRange[0];
            PickUp(inSlot);
            myPlayer.playerAnim.SetTrigger("pickup");
        }

        else if (Input.GetButtonDown("Push") && poolingAnObject == false && objectInRange.Count == 1)
        {
            inSlot = objectInRange[0];
            PickUp(inSlot);
            myPlayer.playerAnim.SetTrigger("pickup");
        }

        else if (Input.GetButtonDown("Push") && poolingAnObject == true)
        {
            distancesToPlayer.Clear();
            Drop(inSlot);
            myPlayer.playerAnim.SetTrigger("pickup");
        }

        if (poolingAnObject==false)
        {
            inSlot = slotVide;
        }

        inSlot.transform.position = main.transform.position;



    }

    public void ReorganizeLists()
    {
        for (int i = 0; i < distancesToPlayer.Count - 1; i++)
        {
            if (distancesToPlayer[i] > distancesToPlayer[i + 1])
            {
                GameObject monCul = objectInRange[i];
                objectInRange[i] = objectInRange[i + 1];
                objectInRange[i + 1] = monCul;
                float tI = distancesToPlayer[i];
                distancesToPlayer[i] = distancesToPlayer[i + 1];
                distancesToPlayer[i + 1] = tI;
                i = 0;
                ReorganizeLists();
            }

        }
    }


    private void PickUp(GameObject child)
    {
        poolingAnObject = true;
       // slotOldTransform = child.transform.parent;
       // slotDifferenceAtPickup = child.transform.position - transform.position;
        child.transform.position = main.transform.position;
        child.GetComponent<Collider>().isTrigger = true;
       // child.transform.parent = main.transform;
    }

    private void Drop(GameObject lastObject)
    {
        lastObject.transform.parent = slotOldTransform;
        lastObject.transform.position += slotDifferenceAtPickup;
        lastObject.GetComponent<Collider>().isTrigger = false;
        lastObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        poolingAnObject = false;
        inSlot = slotVide;
    }
}
