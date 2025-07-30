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

    //Kiểm tra đã ấn nhảy chưa
    private bool isJumppressed = false;
    private bool isGameStart = true;

    public Animator animator;
    private string currentAnim = "";

    
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

        ChangeAnimation("Run");
    }

    // Update is called once per frame
    void Update()
    {

        isJumppressed = Input.GetButtonDown("Jump");
        if (isJumppressed )
        {
            Jump();
            
        }
        
        CheckAnimation();

    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(0, jumpForce);
        
    }

    private void ChangeAnimation(string animation, float crossFade = 0.2f)
    {
        if (currentAnim != animation)
        {
            currentAnim = animation;
            animator.CrossFade(animation, crossFade);
        }
    }

    private void CheckAnimation()
    {
        Debug.Log("velocity la " + rb.linearVelocityY);
        string state = CheckState();
        if (state == "Jump")
        {
            ChangeAnimation("Jump");
        }
        else if (state == "Fall")
        {
            ChangeAnimation("Fall");
        }
        else if(state == "Idle")
        {
            ChangeAnimation("Idle");
        }
        else
        {
            ChangeAnimation("Run");
        }
    }

    private string CheckState()
    {
        if (rb.linearVelocityY > 0.1f)
        {
            return "Jump";
        }
        if (rb.linearVelocityY < -0.1f)
        {
            return "Fall";
        }
        if(Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) && !isGameStart)
        {
            return "Idle";
        }
        return "Run";
        
    }
        
}
