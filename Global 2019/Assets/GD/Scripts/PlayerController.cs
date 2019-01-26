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

    #region private variables
    bool isJumping;
    bool isFalling;
    bool canSwap;
    float actualGravityModifier;
    float jumpTime;
    float currentJumpTime;
    Rigidbody playerBody;
    [SerializeField]
    float accumulatedVelocityDuringFastFall;
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

        if (!canSwap && actualPlayerState == PlayerState.normal)
        {
            canSwap = true;
        }

        if (actualPlayerState == PlayerState.bouncing)
        {
            Debug.Log("Suce");
            playerBody.velocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z);
            playerBody.AddForce(0, accumulatedVelocityDuringFastFall, 0);
            accumulatedVelocityDuringFastFall = 0f;
            SwitchPlayerState(PlayerState.normal);
        }
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
    }

    public void UpdateJump()
    {
        currentJumpTime += Time.deltaTime;
        playerBody.velocity += new Vector3(0, jumpForce * jumpCurve.Evaluate(currentJumpTime), 0);

        if (Input.GetKeyDown(KeyCode.E) && canSwap)
        {
            SwitchPlayerState(PlayerState.bouncing);
        }
    }

    private void FixedUpdate()
    {
        MovePlayerOnXZPlan();
        ApplyGravityOnPlayer();

        if (isFalling)
        {
            accumulatedVelocityDuringFastFall += Mathf.Abs(playerBody.velocity.y);
        }
    }

    public void Fastfall()
    {
        isJumping = false;
        playerBody.AddForce(0, -fastFallSpeed, 0);
        isFalling = true;
    }

    public void MovePlayerOnXZPlan()
    {
        switch (actualPlayerState)
        {
            case PlayerState.normal:
                {
                    playerBody.velocity = Vector3.Lerp(playerBody.velocity, PlayerInputTransformed() * maxSpeed, Time.deltaTime * baseSpeed);
                }
                break;

            case PlayerState.bouncing:
                {
                    playerBody.velocity = Vector3.Lerp(playerBody.velocity, PlayerInputTransformed() * maxVerticalPositionSpeed, Time.deltaTime * baseSpeed);
                }
                break;
        }
    }

    public Vector3 PlayerInputTransformed()
    {
        Vector3 rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        return rawInput;
    }

    public void ApplyGravityOnPlayer()
    {
        playerBody.AddForce(Physics.gravity.x, Physics.gravity.y * actualGravityModifier, Physics.gravity.z);
        Debug.Log(actualGravityModifier);
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

                    }
                    break;
            }

            canSwap = false;
            actualPlayerState = newstate;
        }
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
