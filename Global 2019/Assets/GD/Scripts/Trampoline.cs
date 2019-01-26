using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Tooltip("Force du rebond du trampoline")]
    public float pushForce;


    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AddForcedVerticalVelocity(pushForce * Mathf.Abs(player.downVelocityAccumulatedDuringJump) * player.actualBounciness);
            if (player.actualPlayerState == PlayerController.PlayerState.bouncing)
            {
                player.ResetBouncingState();
            }
        }
    }

}
