using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Variables GameFeel")]
    [Tooltip("Vitesse à laquelle le joueur se déplace")]
    public float baseSpeed;
    [Tooltip("Vitesse maximale du joueur")]
    public float maxSpeed;
    [Range(0f, 2f)]
    [Tooltip("Comment la gravité affecte le joueur en mode normal")]
    public float baseGravityModifier;
    [Range(0f, 2f)]
    [Tooltip("Comment la gravité affecte le joueur en mode Rebond (couverture verticale)")]
    public float bouncingGravityModifier;
    [Header("Variables Debug prog")]
    public PlayerState actualPlayerState = PlayerState.normal;


    #region private variables
    Rigidbody playerBody;
    float actualGravityModifier;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        SwitchPlayerState(PlayerState.normal);
    }

    // Update is called once per frame
    void Update()
    {
        switch(actualPlayerState)
        {
            case PlayerState.normal:
                {

                } break;

            case PlayerState.bouncing:
                {

                } break;
        }
    }

    private void FixedUpdate()
    {
        ApplyGravityOnPlayer();
        MovePlayerOnXZPlan();
    }

    public void MovePlayerOnXZPlan()
    {
        playerBody.velocity = Vector3.Lerp(playerBody.velocity, PlayerInputTransformed() * maxSpeed, Time.deltaTime * baseSpeed);
    }

    public Vector3 PlayerInputTransformed()
    {
        Vector3 rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        return rawInput;
    }

    public void ApplyGravityOnPlayer()
    {
        playerBody.AddForce(Physics.gravity.x, Physics.gravity.y * actualGravityModifier, Physics.gravity.z);
    }

    public void SwitchPlayerState(PlayerState newstate)
    {
        switch(newstate)
        {
            case PlayerState.normal:
                {
                    if (actualGravityModifier != baseGravityModifier)
                    {
                        actualGravityModifier = baseGravityModifier;
                    }

                } break;

            case PlayerState.bouncing:
                {
                    if (actualGravityModifier != bouncingGravityModifier)
                    {
                        actualGravityModifier = bouncingGravityModifier;
                    }

                }
                break;
        }

        actualPlayerState = newstate;

    }


    public enum PlayerState
    {
        normal,
        bouncing
    }
}
