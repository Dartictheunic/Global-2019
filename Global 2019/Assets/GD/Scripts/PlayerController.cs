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
    [Tooltip("Vitesse maximale du joueur en position verticale")]
    public float maxVerticalPositionSpeed;
    [Tooltip("Vitesse de descente du piqué")]
    public float fastFallSpeed;
    [Tooltip("Bounciness du joueur")]
    public float verticalPositionBounciness;
    [Range(0f, 2f)]
    [Tooltip("Comment la gravité affecte le joueur en mode normal")]
    public float baseGravityModifier;
    [Range(0f, 2f)]
    [Tooltip("Comment la gravité affecte le joueur en mode Rebond (couverture verticale)")]
    public float verticalPositionGravityModifier;
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
    public AnimationCurve playerVerticalVelocity;
    public AnimationCurve downVelocityAccumulatedDuringJumpCurve;
    public float downVelocityAccumulatedDuringJump;
    public float actualBounciness;
    #region private variables
    bool isJumping;
    bool isFalling;
    bool canSwap;
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

    public void HitGround(GameObject objectTag)
    {

        if (objectTag.tag == "Ground")
        {
            canJump = true;
            if (isJumping)
            {
                isJumping = false;
            }

            if (!canSwap && actualPlayerState == PlayerState.normal)
            {
                canSwap = true;
            }
        }

        if (actualPlayerState == PlayerState.bouncing)
        {
            if (objectTag.GetComponent<Trampoline>() == null)
            {
                AddForcedVerticalVelocity(downVelocityAccumulatedDuringJump * verticalPositionBounciness);
                ResetBouncingState();
            }
        }
    }

    public void ResetBouncingState()
    {
        downVelocityAccumulatedDuringJump = 0f;
        isFalling = false;
        SwitchPlayerState(PlayerState.normal);
    }

    public void AddForcedVerticalVelocity(float force)
    {
        ResetPlayerVerticalVelocity();
        playerBody.AddForce(0, force * transform.lossyScale.y, 0);
    }

    public void ResetPlayerVerticalVelocity()
    {
        playerBody.velocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z);
    }


    void Update()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            StartJump();
        }

        if (isJumping)
        {
            UpdateJump();
        }
    }

    public void StartJump()
    {
        isJumping = true;
        canJump = false;
        currentJumpTime = 0f;
        downVelocityAccumulatedDuringJump = 0f;
    }

    public void UpdateJump()
    {
        currentJumpTime += Time.deltaTime;
        playerBody.velocity += Vector3.Scale(new Vector3(0, jumpForce * jumpCurve.Evaluate(currentJumpTime), 0), transform.lossyScale);
        if (playerBody.velocity.y < 0)
        {
            var actualVelocity = Mathf.Abs(playerBody.velocity.y);
            if (actualVelocity > downVelocityAccumulatedDuringJump)
            {
                downVelocityAccumulatedDuringJump = actualVelocity;
            }
        }

        if (Input.GetButtonDown("Swap") && canSwap)
        {
            SwitchPlayerState(PlayerState.bouncing);
        }
    }

    private void FixedUpdate()
    {
        MovePlayerOnXZPlan();
        ApplyGravityOnPlayer();
        playerVerticalVelocity.AddKey(Time.time, playerBody.velocity.y);
        downVelocityAccumulatedDuringJumpCurve.AddKey(Time.time, downVelocityAccumulatedDuringJump);
        if (isFalling)
        {
            var actualForce = Mathf.Abs(playerBody.velocity.y);
            if (actualForce > downVelocityAccumulatedDuringJump && playerBody.velocity.y < 0)
            {
                downVelocityAccumulatedDuringJump = actualForce;

            }
        }
    }

    public void Fastfall()
    {
        isJumping = false;
        ResetPlayerVerticalVelocity();
        playerBody.AddForce(Vector3.Scale(new Vector3( 0, -fastFallSpeed, 0), transform.lossyScale)) ;
        isFalling = true;
    }

    public void MovePlayerOnXZPlan()
    {
        switch (actualPlayerState)
        {
            case PlayerState.normal:
                {
                    playerBody.velocity = Vector3.Lerp(playerBody.velocity, MultiplyVectorValues(PlayerInputTransformed(), maxSpeed), Time.deltaTime * baseSpeed);
                }
                break;

            case PlayerState.bouncing:
                {
                    playerBody.velocity = Vector3.Lerp(playerBody.velocity,MultiplyVectorValues(PlayerInputTransformed(), maxVerticalPositionSpeed), Time.deltaTime * baseSpeed);
                }
                break;
        }
    }

    public Vector3 MultiplyVectorValues(Vector3 vectorUsed, float speedToUse)
    {
        Vector3 vectorNeeded = new Vector3(vectorUsed.x * speedToUse, vectorUsed.y / transform.lossyScale.y, vectorUsed.z * speedToUse);
        return vectorNeeded;
    }

    public Vector3 PlayerInputTransformed()
    {
        Vector3 rawInput = Vector3.Scale(new Vector3(Input.GetAxisRaw("Horizontal"), playerBody.velocity.y, Input.GetAxisRaw("Vertical")), transform.lossyScale);
        return rawInput;
    }

    public void ApplyGravityOnPlayer()
    {
        playerBody.AddForce(Vector3.Scale( new Vector3(Physics.gravity.x, Physics.gravity.y * actualGravityModifier, Physics.gravity.z), transform.lossyScale));
    }

    public void SwitchPlayerState(PlayerState newstate)
    {
        if (newstate != actualPlayerState)
        {
            switch(newstate)
            {
                case PlayerState.normal:
                    {
                        if (actualGravityModifier != baseGravityModifier)
                        {
                            actualGravityModifier = baseGravityModifier;
                        }

                        if (!canJump)
                        {
                            canJump = true;
                        }

                        actualBounciness = 1;

                    } break;

                case PlayerState.bouncing:
                    {
                        if (actualGravityModifier != verticalPositionGravityModifier)
                        {
                            actualGravityModifier = verticalPositionGravityModifier;
                        }

                        if (isJumping)
                        {
                            Fastfall();
                        }
                        actualBounciness = verticalPositionBounciness;

                    }
                    break;
            }

            canSwap = false;
            actualPlayerState = newstate;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.position.y < groundPos.position.y)
        {
            HitGround(collision.gameObject);
        }
    }

    public enum PlayerState
    {
        normal,
        bouncing
    }
}
