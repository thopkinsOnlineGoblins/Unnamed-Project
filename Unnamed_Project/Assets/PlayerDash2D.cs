using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash2D : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isDashing;
    private float dashTimeLeft;
    private float lastDashTime;

    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Press Left Shift (default "Fire3") to dash
        if (Input.GetButtonDown("Fire3") && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;

            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;

        float direction = moveInput != 0 ? moveInput : transform.localScale.x;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.AddForce(new Vector2(direction * dashForce, 0f), ForceMode2D.Impulse);
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}