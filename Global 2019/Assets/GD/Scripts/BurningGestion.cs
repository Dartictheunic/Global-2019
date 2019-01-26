using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningGestion : MonoBehaviour
{
    [Header("TWEAKING")]
    [Tooltip("temps que l'objet brule avant de s'éteindre , après qu'il n'y ai plus de chaleur ")]
    public float timeBurning;
    [Tooltip("chaleur produit par l'objet lorsqu'il brule ")]
    public float heatProduce;
    [Tooltip("chaleur qu'il faut à l'objet pour bruler ")]
    public float heatNeeded;


    [Header("DEBUG")]
    [SerializeField]
    private List<GameObject> objectWithBurnAround;
    [SerializeField]
    private List<float> heatAround;
    [SerializeField]
    private List<BurningGestion> refBurning;


    [SerializeField]
    private bool isBurning;
    [SerializeField]
    private float totalHeat;
    [SerializeField]
    private bool checkBurning;


    private void OnTriggerStay(Collider other)
    {
        BurningGestion refBG = other.GetComponent<BurningGestion>();
        if (refBG != null)
        {
            checkBurning = true; 
            objectWithBurnAround.Add(other.gameObject);
            refBurning.Add(refBG);
            HeatCheck();
        }



    }

    private void OnTriggerExit(Collider other)
    {
        BurningGestion refBG = other.GetComponent<BurningGestion>();
        if (refBG != null)
        {
            objectWithBurnAround.Remove(other.gameObject);
            refBurning.Remove(refBG);
            HeatCheck();
        }
    }

    //Dans l'update 
    private void HeatCheck()
    {
            Debug.Log(checkBurning);
        if (checkBurning)
        {
            if (objectWithBurnAround.Count > 0)
            {
                for (int i = 0; i < objectWithBurnAround.Count - 1; i++)
                {
                    if (refBurning[i].isBurning == true)
                    {
                        totalHeat = totalHeat + refBurning[i].heatProduce;
                    }

                }
            }
            HeatCheck();
        }
    }


    private void Update()
    {
        if (totalHeat >= heatNeeded)
        {
            isBurning = true;
        }
        else
        {
            isBurning = false;
        }

        if (objectWithBurnAround.Count == 0)
        {
            checkBurning = false;
        }
        else if (checkBurning == false)
        {
            checkBurning = true; 
        }
    }
}
