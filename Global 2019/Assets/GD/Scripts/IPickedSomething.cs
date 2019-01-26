using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPickedSomething : MonoBehaviour
{
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
        if (Input.GetKeyDown("f") && poolingAnObject == true)
        {
            distancesToPlayer.Clear();
            GameObject lastObject = inSlot;
            inSlot = slotVide;
            Drop(lastObject);
        }
        else if (Input.GetKeyDown("f") && poolingAnObject == false && objectInRange.Count > 1)
        {
            for (int i = 0; i < objectInRange.Count; i++)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, objectInRange[i].transform.position);
                distancesToPlayer.Add(distanceToPlayer);

            }

            ReorganizeLists();

            inSlot = objectInRange[0];
            PickUp(inSlot);
        }

        if (Input.GetKeyDown("f") && poolingAnObject == false && objectInRange.Count == 1)
        {
            inSlot = objectInRange[0];
            PickUp(inSlot);
        }



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
        child.transform.position = main.transform.position;
        child.GetComponent<Rigidbody>().isKinematic = true;
        child.GetComponent<Collider>().isTrigger = true;
        child.transform.parent = main.transform;
        poolingAnObject = true;
        print("Pickup");
        print(child.name);
    }

    private void Drop(GameObject lastObject)
    {
        lastObject.transform.parent = lastObject.transform;
        lastObject.GetComponent<Rigidbody>().isKinematic = false;
        lastObject.GetComponent<Collider>().isTrigger = false;
        poolingAnObject = false;
        print("drop");

    }
}
