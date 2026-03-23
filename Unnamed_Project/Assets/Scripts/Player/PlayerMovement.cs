using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    LockOnSystem lockOn;
    Camera cam;
    public float moveSpeed = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lockOn = GetComponent<LockOnSystem>();
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");
        CharacterMovement(xMove, zMove);
        HandleLockOnRotation();
    }

    void CharacterMovement(float xMove, float zMove)
    {
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        // Flatten to XZ so slope/camera pitch doesn't affect movement direction
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camRight * xMove + camForward * zMove).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }

    void HandleLockOnRotation()
    {
        if (lockOn == null || lockOn.TargetEnemy == null) return;

        Vector3 directionToEnemy = lockOn.TargetEnemy.transform.position - transform.position;
        directionToEnemy.y = 0f;

        if (directionToEnemy.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(directionToEnemy);
    }
}