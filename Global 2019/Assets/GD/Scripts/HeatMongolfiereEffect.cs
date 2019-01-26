using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMongolfiereEffect : MonoBehaviour, IHeatEffect
{
    [Tooltip("Force à laquelle le joueur va être soulevé par les flammes (la distance est la taille du collider)")]
    public float pushForce;
    bool isActivated;

    public void CheckParentIsHeatable()
    {
        if (GetComponentInParent<BurningGestion>() == null)
        {
            throw new System.Exception("Le parent ne peut pas brûler !");
        }
    }

    public void StartHeatEffect()
    {
        isActivated = true;
    }

    public void StopHeatEffect()
    {
        isActivated = false;
    }

    void Start()
    {
        CheckParentIsHeatable();
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActivated)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddForcedVerticalVelocity(pushForce);
            }
        }
    }
}
