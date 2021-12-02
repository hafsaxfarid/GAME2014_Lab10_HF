using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Touch Input")]
    public Joystick joystick;

    [Range(0.01f, 1.0f)]
    public float sensitivity;

    [Header("Movement")]
    public float horizontalForce;
    public float verticalForce;
    public bool isGrounded;
    public Transform groundOrigin;
    public float groundRadius;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;
    public Transform spawnPoint;

    [Range(0.1f, 0.9f)]
    public float airControlFactor;

    [Header("Animation")]
    public PlayerAnimationState state;

    [Header("Audio FX")]
    public AudioSource jumpSound;

    [Header("Dust Trail")]
    public ParticleSystem dustTrail;
    public Color dustTrailColor;

    private Rigidbody2D playerRB;
    private Animator playerAnimationController;
    private string animationState = "AnimationState";

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimationController = GetComponent<Animator>();
        jumpSound = GetComponent<AudioSource>();
        dustTrail = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        Move();
        CheckIsGrounded();
        WallHit();
    }

    private void Move()
    {
        float x = (Input.GetAxisRaw("Horizontal") + joystick.Horizontal) * sensitivity;

        if (isGrounded)
        {
            // keyboard input    
            float y = (Input.GetAxisRaw("Vertical") + joystick.Vertical) * sensitivity;
            float jump = Input.GetAxisRaw("Jump") + ((UIController.jumpButtonDown) ? 1.0f : 0.0f);

            if (jump > 0)
            {
                jumpSound.Play();
            }

            if (x != 0)
            {
                x = FlipAnimation(x);
                playerAnimationController.SetInteger(animationState, (int)PlayerAnimationState.RUN); // run state
                state = PlayerAnimationState.RUN;
                CreateTrail();
            }
            else
            {
                playerAnimationController.SetInteger(animationState, (int)PlayerAnimationState.IDLE); // idle state
                state = PlayerAnimationState.IDLE;
            }

            float horizontalMoveForce = x * horizontalForce;
            float jumpMoveForce = jump * verticalForce;

            float mass = playerRB.mass * playerRB.gravityScale;

            playerRB.AddForce(new Vector2(horizontalMoveForce, jumpMoveForce) * mass);
            playerRB.velocity *= 0.99f;
        }
        else
        {
            playerAnimationController.SetInteger(animationState, (int)PlayerAnimationState.JUMP); // jump state
            state = PlayerAnimationState.JUMP;

            if (x != 0)
            {
                x = FlipAnimation(x);

                float horizontalMoveForce = x * horizontalForce * airControlFactor;
                float mass = playerRB.mass * playerRB.gravityScale;

                playerRB.AddForce(new Vector2(horizontalMoveForce, 0.0f) * mass);
            }
            CreateTrail();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(other.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            transform.SetParent(null);
        }
    }

    public void CheckIsGrounded()
    {
        RaycastHit2D hit = Physics2D.CircleCast(groundOrigin.position, groundRadius, Vector2.down, groundRadius, groundLayerMask);
        isGrounded = (hit) ? true : false;
    }

    public void WallHit()
    {
        RaycastHit2D hit = Physics2D.CircleCast(groundOrigin.position, groundRadius, Vector2.down, groundRadius, wallLayerMask);

        if (hit)
        {
            isGrounded = true;
        }
    }

    private float FlipAnimation(float x)
    {
        x = (x > 0) ? 1 : -1;

        transform.localScale = new Vector2(x, 1.0f);
        return x;
    }

    public void CreateTrail()
    {
        dustTrail.GetComponent<Renderer>().material.SetColor("_Color", dustTrailColor);
        dustTrail.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundOrigin.position, groundRadius);
    }
}