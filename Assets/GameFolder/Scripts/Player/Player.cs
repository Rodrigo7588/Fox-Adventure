using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb2d;

    public Animator playerAnim;
    [SerializeField] private LayerMask groundMask;

    private float moveInput;
    public float moveSpeed;

    public float jumpForce;
    [SerializeField] private bool onGround;
    private bool wasOnGround;
    private bool isJump;

    //ground circle collider
    private Collider2D[] colliders_1, colliders_2;
    private float groundCheckRadius = 0.036f * 2f;       //tamanho do circulo no pé do palyer
    public Transform[] groundCheck;

    //slopes system ---------
    public PhysicsMaterial2D noFriction, friction;
    public float slopeCheckDistance;
    private float slopeAngle;
    private bool onSlope;

    //slide -----------
    [Header("Slide System----------")]
    public Transform wallCheck;
    private bool isColliderWall;
    public float wallCheckDistance;
    private bool isSliding;
    [Header("velocidade queda parede")]
    public float wallSlideSpeed;
    [Header("velocidade pulo parede")]
    public float wallJumpForce;

    private bool onSliding; //indica que esta pulando fora da parede


    void Start()
    {

        rb2d = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        InputSystem();
        checkGround();
        Animations();
        Slopes();
        Slide();
    }

    private void FixedUpdate()
    {
        if (!onSliding)
            Move();
    }

    private void Move()
    {
        if (onSlope && !isJump)
        {
            //movimento com slope/rampa
            rb2d.gravityScale = 20f;
            if (rb2d.velocity.y < -2f)
            {
                rb2d.velocity = new Vector2(moveInput * moveSpeed, -9f);
            }
            else
            {
                rb2d.velocity = new Vector2(moveInput * moveSpeed, rb2d.velocity.y);
            }
        }
        else
        {
            rb2d.gravityScale = 3f;
            rb2d.velocity = new Vector2(moveInput * moveSpeed, rb2d.velocity.y);
        }

    }

    private void InputSystem()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // virar player na direção que está olhando
        if (moveInput != 0f && !onSliding)
        {
            transform.localScale = new Vector3(moveInput, 1f, 1f);
        }

        // pulo normal
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            Jump();
        }

        // puo na parede
        if (Input.GetKeyDown(KeyCode.Space) && isSliding)
        {
            rb2d.velocity = Vector2.zero;
            rb2d.velocity = new Vector2(wallJumpForce * -moveInput, wallJumpForce);
            onSliding = true;
            StartCoroutine(jumpSlide());
        }
    }

    IEnumerator jumpSlide()
    {
        transform.localScale = new Vector3(-moveInput, 1f, 1f);
        yield return new WaitForSeconds(0.3f);
        onSliding = false;
    }

    void checkGround()
    {
        colliders_1 = Physics2D.OverlapCircleAll(groundCheck[0].position, groundCheckRadius, groundMask);
        colliders_2 = Physics2D.OverlapCircleAll(groundCheck[1].position, groundCheckRadius, groundMask);

        if (onGround && !wasOnGround) { isJump = false; }
        ;

        wasOnGround = onGround;

        if (colliders_1.Length > 0 || colliders_2.Length > 0)
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }
    }

    private void Jump()
    {
        isJump = true;
        rb2d.gravityScale = 3f;
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
    }

    private void Slopes()
    {
        RaycastHit2D hitSlope = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, groundMask);
        Debug.DrawRay(transform.position, Vector2.down * slopeCheckDistance, Color.red);

        if (hitSlope)
        {
            slopeAngle = Vector2.Angle(hitSlope.normal, Vector2.up); //pega o angulo do slope

            print(slopeAngle);

            onSlope = slopeAngle != 0;

            if (onSlope && moveInput == 0)
            {
                rb2d.sharedMaterial = friction;
            }
            else
            {
                rb2d.sharedMaterial = noFriction;
            }
        }
    }

    private void Slide()
    {
        isColliderWall = Physics2D.Raycast(wallCheck.position, wallCheck.TransformDirection(Vector2.right), wallCheckDistance, groundMask);
        Debug.DrawRay(wallCheck.position, wallCheck.TransformDirection(Vector2.right) * wallCheckDistance, Color.red);
        if (isColliderWall && !onGround && rb2d.velocity.y < 0 && moveInput != 0)
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        // faz slide na parede caindo
        if (isSliding && rb2d.velocity.y < -wallSlideSpeed)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, -wallSlideSpeed);
        }
    }

    private void Animations()
    {
        playerAnim.SetFloat("SpeedX", Mathf.Abs(moveInput));
        playerAnim.SetFloat("SpeedY", rb2d.velocity.y);
        playerAnim.SetBool("onGround", onGround);
        playerAnim.SetBool("isSliding", isSliding);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 8)
        {
            GameController.instance.RestartGame();
        }
    }
}
