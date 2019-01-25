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
    [Range(0f, 20f)]
    [Tooltip("Force du saut")]
    public float jumpForce;
    [Tooltip("Courbe du saut")]
    public AnimationCurve jumpCurve;

    [Space(20)]
    [Header("Liens à faire")]
    public Transform groundPos;


    [Space(20)]
    [Header("Variables Debug prog")]
    public PlayerState actualPlayerState = PlayerState.normal;
    public bool canJump;

    #region private variables
    bool isJumping;
    float actualGravityModifier;
    float jumpTime;
    float currentJumpTime;
    Rigidbody playerBody;
    #endregion

    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        SwitchPlayerState(PlayerState.normal);
        jumpTime = jumpCurve.keys[jumpCurve.length -1].time;
    }

    public void HitGround()
    {
        canJump = true;
        if (isJumping)
        {
            isJumping = false;
        }
    }


    void Update()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            StartJump();
        }
    }

    public void StartJump()
    {
        isJumping = true;
        canJump = false;
        currentJumpTime = 0f;
    }

    public void UpdateJump()
    {
        currentJumpTime += Time.deltaTime;
        playerBody.velocity += new Vector3(0, jumpForce * jumpCurve.Evaluate(currentJumpTime), 0);

        if(currentJumpTime >= jumpTime)
        {
            isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        ApplyGravityOnPlayer();
        MovePlayerOnXZPlan();

        if(isJumping)
        {
            UpdateJump();
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && collision.gameObject.transform.position.y < groundPos.position.y)
        {
            HitGround();
        }
    }

    public enum PlayerState
    {
        normal,
        bouncing
    }
}
