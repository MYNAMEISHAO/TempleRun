using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerConfig pConfig;
    public GameSpeedConfig gameSpeedConfig;


    public float jumpForce;
    private float speedChar;
    private float coyoteCounter = 0;
    private Rigidbody2D rb;

    //Dành cho mục đích kiểm tra nhân vật đã chạm đất chưa
    private Transform groundCheck;
    public LayerMask groundLayer;              // Chọn layer ground
    public float groundCheckRadius = 0.1f;
    private bool isGrounded = false;

    //Kiểm tra đã ấn nhảy chưa
    private bool isJumpPressed = false;
    private bool isGameStart = true;

    private bool isRollPressed = false;

    public bool isDead = false; // Kiểm tra nhân vật đã chết chưa
    public Animator animator;
    private string currentAnim = "";
    private float minY;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get rigibody
        rb = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).GetComponent<Animator>();

        gameSpeedConfig = GameObject.Find("GameSpeed").GetComponent<GameSpeedConfig>();

        speedChar = gameSpeedConfig.speedOverTime.Evaluate(gameSpeedConfig.totalTime);
        pConfig = new PlayerConfig();
        jumpForce = pConfig.jumpForce;

        groundCheck = transform.GetChild(1).GetComponent<Transform>();


        minY = pConfig.minHeight;

        ChangeAnimation("Run");
    }

    // Update is called once per frame
    void Update()
    {
        isRollPressed = Input.GetKeyDown(KeyCode.S);
        isJumpPressed = Input.GetButtonDown("Jump");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        HandleInput();
        CheckAnimation();

    }

    public void HandleInput()
    {
        if (transform.position.y < minY)
        {
            isDead = true;
            transform.gameObject.SetActive(false);
            //GameManager.Instance
            return;
        }

        if (isJumpPressed && isGrounded)
        {
            ChangeAnimation("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            return;
        }

        if (isRollPressed)
        {
            ChangeAnimation("Roll");
            if (!isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -jumpForce);
                return;
            }

        }
    }


    public void ChangeAnimation(string animation, float crossFade = 0.2f, float time = 0)
    {
        if(time > 0)
        {
            StartCoroutine(Wait());
        }
        else
        {
            Validate();
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossFade);
            Validate();
        }

        void Validate()
        {
            if (currentAnim != animation)
            {
                currentAnim = animation;
                animator.CrossFade(animation, crossFade);
            }
        }
        
    }

    private void CheckAnimation()
    {
        if (currentAnim == "Roll")
        {
            return;
        }
        if (isDead)
        {
            ChangeAnimation("Die");
        }
        else
        {
            if (rb.linearVelocityY > 0.1f)
            {
                ChangeAnimation("Jump");
            }
            else if (!isGrounded)
            {
                ChangeAnimation("Fall");
            }
            else if (isGrounded)
            {
                ChangeAnimation("Run");
            }
            
        }
    }
}
