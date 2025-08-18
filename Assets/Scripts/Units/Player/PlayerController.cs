using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public PlayerState currentState;
    bool isJumpPressed = false;
    bool isRollPressed = false;
    bool isDead = false; // Kiểm tra nhân vật đã chết chưa
    bool isGrounded = false; // Kiểm tra nhân vật đã chạm đất chưa
    bool stateCompleted = false; // Kiểm tra trạng thái đã hoàn thành chưa

    Rigidbody2D rb;

    Animator animator;
    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Roll,
        Die
    }

// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        isJumpPressed = Input.GetButtonDown("Jump");
        isRollPressed = Input.GetKeyDown(KeyCode.S);
        isGrounded = Physics2D.OverlapCircle(transform.GetChild(1).position, 0.1f, LayerMask.GetMask("Ground"));
        Debug.Log("currentState: " + currentState);
        Debug.Log("stateCompleted: " + stateCompleted);
        //if (!stateCompleted) 

        //    CloseState(currentState);

        CheckState();
    }

    
    private void CheckState()
    {   
        if (isDead)
        {
            UpdateState(PlayerState.Die);
        }
        else if (isGrounded)
        {
            if (isJumpPressed)
            {
                UpdateState(PlayerState.Jump);
                return;
            }
            else if (isRollPressed || !transform.GetChild(2).gameObject.activeSelf)
            {
                UpdateState(PlayerState.Roll);
                return;
            }
            else
            {
                UpdateState(PlayerState.Run);
                return;
            }
        }
        else
        {
            if (isRollPressed)
            {
                UpdateState(PlayerState.Roll);
                return;
            }
            else if (rb.linearVelocityY < 0.1f)
            {
                UpdateState(PlayerState.Fall);
                return;
            }
        }
    }

    void UpdateState(PlayerState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStateChanged();
        }
    }

    private void OnStateChanged()
    {
        HandlePhysics(currentState);
        PlayAnim(currentState);
        stateCompleted = false; // Reset stateCompleted when changing state
    }

    private void CloseState(PlayerState currentState)
    {
        stateCompleted = true;
        if(currentState == PlayerState.Roll)
        {
            if(isDead) stateCompleted = true;
            else if(isJumpPressed && isGrounded) stateCompleted = true;
            return;
        }

        if(currentState == PlayerState.Die)
        {
            return;
        }

        if (currentState == PlayerState.Jump)
        {
            if (rb.linearVelocity.y < 0.1f || isRollPressed)
            {
                stateCompleted = true; // Đặt stateCompleted thành true khi nhảy đã hoàn thành
            }
        }

        if (currentState == PlayerState.Fall)
        {
            if (isGrounded || isRollPressed)
            {
                stateCompleted = true; // Đặt stateCompleted thành true khi rơi đã hoàn thành
            }
        }

        if (currentState == PlayerState.Run)
        {
            if (isJumpPressed || isRollPressed || !isGrounded)
            {
                stateCompleted = true; // Đặt stateCompleted thành true khi chạy đã hoàn thành
            }
        }


        
    }

    private void PlayAnim(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                // Play idle animation
                animator.CrossFade("Idle", 0.1f);
                break;
            case PlayerState.Run:
                // Play running animation
                animator.CrossFade("Run", 0.1f);
                break;
            case PlayerState.Jump:
                // Play jumping animation
                animator.CrossFade("Jump", 0.1f);
                break;
            case PlayerState.Fall:
                // Play falling animation
                animator.CrossFade("Fall", 0.1f);
                break;
            case PlayerState.Roll:
                // Play rolling animation
                animator.CrossFade("Roll", 0.1f);
                StartCoroutine(Wait(1f)); // Start coroutine to wait before transitioning back to Run
                break;
            case PlayerState.Die:
                // Play dead animation
                animator.CrossFade("Die", 0.1f);

                break;
        }
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        stateCompleted = true; // Đặt stateCompleted thành true sau khi đợi
        UpdateState(PlayerState.Run); // Chuyển về trạng thái Idle sau khi hoàn thành Roll
        // Chuyển về trạng thái Idle sau khi hoàn thành Roll
    }

    private void HandlePhysics(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.Idle:
                // Handle idle physics
                if (!transform.GetChild(2).gameObject.activeSelf)
                {
                    transform.GetChild(2).gameObject.SetActive(true); // Enable collider when idle
                    transform.GetChild(3).gameObject.SetActive(false);
                }
                break;
            case PlayerState.Run:
                // Handle running physics
                if (!transform.GetChild(2).gameObject.activeSelf)
                {
                    transform.GetChild(2).gameObject.SetActive(true); // Enable collider when idle
                    transform.GetChild(3).gameObject.SetActive(false);
                }
                break;
            case PlayerState.Jump:
                // Handle jumping physics
                if (!transform.GetChild(2).gameObject.activeSelf)
                {
                    transform.GetChild(2).gameObject.SetActive(true); // Enable collider when idle
                    transform.GetChild(3).gameObject.SetActive(false);
                }
                rb.linearVelocity = new Vector2(0f, 10f); // Apply jump force
                break;
            case PlayerState.Fall:
                // Handle falling physics
                if (!transform.GetChild(2).gameObject.activeSelf)
                {
                    transform.GetChild(2).gameObject.SetActive(true); // Enable collider when idle
                    transform.GetChild(3).gameObject.SetActive(false);
                }
                break;
            case PlayerState.Roll:
                // Handle rolling physics
                transform.GetChild(2).gameObject.SetActive(false); // Disable collider when rolling
                transform.GetChild(3).gameObject.SetActive(true);
                if (!isGrounded)
                {
                    rb.linearVelocity = new Vector2(0f, -5f); // Apply forward force when rolling in air
                }
                break;
            case PlayerState.Die:
                // Handle death physics
                rb.linearVelocity = new Vector2(-5f, 10f); // Apply upward force on death
                transform.GetChild(2).gameObject.SetActive(false); // Disable collider when dead
                transform.GetChild(3).gameObject.SetActive(false); // Disable collider when dead    
                break;
            }
        }

}
