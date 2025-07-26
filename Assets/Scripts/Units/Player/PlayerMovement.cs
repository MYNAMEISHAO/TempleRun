using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerConfig pConfig;
    private float coyoteCounter = 0;
    private Rigidbody2D rb;
    private Transform groundCheck;
    private bool isJumppressed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        isJumppressed = Input.GetButtonDown("Jump");
        if (isJumppressed )
        {
            Jump(); 
           
        }

        

    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(0, 10f);

    }

}
