using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningGestion : MonoBehaviour
{
    [Header("TWEAKING")]
    [Tooltip("chaleur produit par l'objet lorsqu'il brule ")]
    public float heatProduce;
    [Tooltip("chaleur qu'il faut à l'objet pour bruler ")]
    public float heatNeeded;
    [Tooltip("L'objet brûle-t-il de base ?")]
    public bool isBurning;
    [Tooltip("L'objet brûle-t-il à l'infini ?")]
    public bool PermanentBurn;

    [SerializeField]
    private float totalHeat;

    private void Awake()
    {
        if (PermanentBurn)
        {
            totalHeat = Mathf.Infinity;
            isBurning = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        BurningGestion refBG = other.GetComponent<BurningGestion>();
        if (refBG != null && isBurning)
        {
            refBG.Heat(heatProduce * Time.deltaTime);
        }
    }

    public void Heat(float burnAmount)
    {
        if (totalHeat < heatNeeded)
        {
            totalHeat += burnAmount;
        }
        if (totalHeat >= heatNeeded && !isBurning)
        {
            StartBurning();
        }
    }


    public void StartBurning()
    {
        isBurning = true;
    }

    public void Burn()
    {
        totalHeat -= Time.deltaTime;
        if (totalHeat <= 0)
        {
            StopBurning();
        }
    }

    public void StopBurning()
    {
        isBurning = false;
    }

    private void FixedUpdate()
    {
        if (isBurning)
        {
            Burn();
        }
    }
}
