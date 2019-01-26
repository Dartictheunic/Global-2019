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
        switch (actualPlayerState)
        {
            case PlayerState.normal:
                {
                    canJump = true;
                    if (isJumping)
                    {
                        isJumping = false;
                    }
                }
                break;

            case PlayerState.bouncing:
                {

                }
                break;
        }
    }

    public void Bounce()
    {
        switch (actualPlayerState)
        {
            case PlayerState.normal:
                {

                }
                break;

            case PlayerState.bouncing:
                {
                    playerBody.velocity += new Vector3(0, -playerBody.velocity.y * 1.9f, 0);
                    canJump = true;
                    if (isJumping)
                    {
                        isJumping = false;
                    }
                }
                break;
        }
    }


    void Update()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            StartJump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            SwitchPlayerState(PlayerState.bouncing);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            SwitchPlayerState(PlayerState.normal);
        }
    }

    public void StartJump()
    {
        switch (actualPlayerState)
        {
            case PlayerState.normal:
                {
                    isJumping = true;
                    canJump = false;
                    currentJumpTime = 0f;
                }
                break;

            case PlayerState.bouncing:
                {
                    isJumping = true;
                    canJump = false;
                    currentJumpTime = 0f;
                    playerBody.velocity += new Vector3(0, -playerBody.velocity.y * 1.9f, 0);
                }
                break;
        }
    }

    public void UpdateJump()
    {
        

        switch (actualPlayerState)
        {
            case PlayerState.normal:
                {
                    currentJumpTime += Time.deltaTime;
                    playerBody.velocity += new Vector3(0, jumpForce * jumpCurve.Evaluate(currentJumpTime), 0);

                    if (currentJumpTime >= jumpTime)
                    {
                        isJumping = false;
                    }
                }
                break;

            case PlayerState.bouncing:
                {
                    currentJumpTime += Time.deltaTime;
                    playerBody.velocity += new Vector3(0, - jumpForce, 0);

                    if (currentJumpTime >= jumpTime)
                    {
                        isJumping = false;
                    }
                }
                break;
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
        switch (actualPlayerState)
        {
            case PlayerState.normal:
                {
                    playerBody.velocity = Vector3.Lerp(playerBody.velocity, PlayerInputTransformed() * maxSpeed, Time.deltaTime * baseSpeed);
                }
                break;

            case PlayerState.bouncing:
                {
                    playerBody.velocity = Vector3.Lerp(playerBody.velocity, new Vector3(PlayerInputTransformed().x * maxVerticalPositionSpeed, playerBody.velocity.y, PlayerInputTransformed().z * maxVerticalPositionSpeed), Time.deltaTime * baseSpeed);
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

                    } break;

                case PlayerState.bouncing:
                    {
                        if (actualGravityModifier != verticalPositionGravityModifier)
                        {
                            actualGravityModifier = verticalPositionGravityModifier;
                        }

                        if (!canJump)
                        {
                            canJump = true;
                        }
                    }
                    break;
            }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground" && other.transform.position.y < groundPos.position.y)
        {
            Bounce();
        }
    }

    public enum PlayerState
    {
        normal,
        bouncing
    }
}
