using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed = 2f;   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        CharacterMovement(xMove, zMove);
    }

    void CharacterMovement(float xMove, float zMove)
    {
        Vector3 moveDirection = (transform.right * xMove + transform.forward * zMove).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }
}
