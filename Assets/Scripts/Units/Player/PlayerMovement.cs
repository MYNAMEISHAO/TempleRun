using System.Collections;
using UnityEngine;

public class PlayerMovement : Entity
{
    public PlayerConfig pConfig;
    public GameSpeedConfig gameSpeedConfig;

    public float jumpForce;
    private float speedChar;
    private float coyoteCounter = 0;
    private int jumpCount = 0; // Số lần nhảy đã thực hiện
    private Rigidbody2D rb;

    //Dành cho mục đích kiểm tra nhân vật đã chạm đất chưa
    private Transform groundCheck;
    public LayerMask groundLayer;              // Chọn layer ground
    public float groundCheckRadius = 0.1f;
    private bool isGrounded = false;

    //Kiểm tra đã ấn nhảy chưa
    private bool isJumpPressed = false;

    private bool isRollPressed = false;

    public bool isDead = false; // Kiểm tra nhân vật đã chết chưa
    private bool inDeathAnim = false; // Kiểm tra đang trong hoạt ảnh chết chưa
    public Animator animator;
    private string currentAnim = "";
    private float minY;
    PlayerState state;


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

        ChangeAnimation("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.instance.state != GameState.Playing)
        //{
        //    return; // Thoát khỏi hàm Update ngay
        //}
        if (canMove)
        {
            isRollPressed = Input.GetKeyDown(KeyCode.S);
            isJumpPressed = Input.GetButtonDown("Jump");

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            HandleInput();
            CheckAnimation();
        }
        else
        {
            if (!canAnimate)
            {
                PauseAnimation();
            }
        }
    }
    public void HandleInput()
    {
        if (transform.position.y < minY)
        {
            isDead = true; // Set isDead to true if player falls below minY
        }
        if (isDead && !inDeathAnim)
        {
            ChangeAnimation("Die");
            rb.linearVelocity = new Vector3(0, 10f, 0f);

            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);

            inDeathAnim = true;

            // gọi coroutine để xử lý sau khi anim chết xong
            StartCoroutine(WaitForDeathAnim());

            SoundManager.instance.StopMusic();
            SoundManager.instance.PlayPlayerDie();

            return;
        }


        if (isJumpPressed)
        {

            SoundManager.instance.PlayJump();
            if (isGrounded)
            {
                ChangeAnimation("Jump");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpCount = 1;
                return;
            }
            else if (jumpCount != 2)
            {
                ChangeAnimation("Jump");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce - 2);
                jumpCount = 2;
            }
        }
        if (isGrounded)
        {
            jumpCount = 0; // Reset jump count when grounded
            if (currentAnim == "Run")
            {
                transform.GetChild(2).gameObject.SetActive(true); // Tắt collider khi chết
                transform.GetChild(3).gameObject.SetActive(false); // Tắt collider khi chết
            }
        }

        if (isRollPressed)
        {
            transform.GetChild(2).gameObject.SetActive(false); // Tắt collider khi chết
            transform.GetChild(3).gameObject.SetActive(true); // Tắt collider khi chết
            ChangeAnimation("Roll");
            if (!isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -jumpForce);
                return;
            }

        }
    }

    IEnumerator WaitForDeathAnim()
    {
        yield return new WaitForSeconds(1f);
        isDead = false; // Reset isDead to false after death animation
        inDeathAnim = false; // Reset inDeathAnim to false after death animation
        GameManager.instance.UpdateGameState(GameState.GameOver);
        transform.gameObject.SetActive(false);
    }
    public void ChangeAnimation(string animation, float crossFade = 0.2f, float time = 0)
    {
        if (time > 0)
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
        animator.speed = 1f; // Đặt tốc độ hoạt hình về mặc định
        if (currentAnim == "Die")
        {
            return;
        }
        if (currentAnim == "Roll")
        {
            return;
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

    private void PauseAnimation()
    {
        if (animator != null)
        {
            animator.speed = 0f; // Dừng hoạt ảnh
        }
    }

    enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Roll,
        Die
    }
}
