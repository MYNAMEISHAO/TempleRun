using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerConfig pConfig;
    private float coyoteCounter = 0;
    private Rigidbody2D rb;
    private Transform groundCheck;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump");
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, pConfig.jumpForce);

    }
}
