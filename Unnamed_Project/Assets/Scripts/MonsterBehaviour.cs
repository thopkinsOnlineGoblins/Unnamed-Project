using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MonsterBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float directionChangeTime = 2f;
    [SerializeField] private float minPauseTime = 0.5f;
    [SerializeField] private float maxPauseTime = 1.5f;

    [Header("Boundary Settings")]
    [SerializeField] private Vector2 boundaryMin = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 boundaryMax = new Vector2(10f, 10f);

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnStart = true;

    // Components
    private Rigidbody2D rb;
    private Vector2 currentDirection;
    private float nextDirectionChange;
    private bool isMoving = true;

    private void Start()
    {
        // Get or add Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // No gravity for top-down
            rb.freezeRotation = true; // Prevent rotation from physics
        }

        if (spawnOnStart)
        {
            SpawnRandomly();
        }

        // Set initial random direction
        ChangeDirection();
        nextDirectionChange = Time.time + directionChangeTime;
    }

    private void Update()
    {
        // Check if it's time to change direction
        if (Time.time >= nextDirectionChange)
        {
            if (isMoving)
            {
                // Random chance to pause or change direction
                if (Random.value < 0.3f) // 30% chance to pause
                {
                    StartPause();
                }
                else
                {
                    ChangeDirection();
                }
            }
            else
            {
                // Resume moving after pause
                isMoving = true;
                ChangeDirection();
            }

            nextDirectionChange = Time.time + directionChangeTime;
        }

        // Keep NPC within boundaries
        KeepWithinBoundaries();
    }

    private void FixedUpdate()
    {
        if (isMoving && rb != null)
        {
            // Apply movement
            rb.linearVelocity = currentDirection * moveSpeed;
        }
        else if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Spawn the NPC at a random position within the boundary
    /// </summary>
    public void SpawnRandomly()
    {
        float randomX = Random.Range(boundaryMin.x, boundaryMax.x);
        float randomY = Random.Range(boundaryMin.y, boundaryMax.y);

        transform.position = new Vector3(randomX, randomY, 0);
    }

    /// <summary>
    /// Change the movement direction to a random vector
    /// </summary>
    private void ChangeDirection()
    {
        // Generate random direction (normalized)
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        currentDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // Optional: Rotate sprite to face movement direction
        if (GetComponent<SpriteRenderer>() != null)
        {
            float rotationAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotationAngle - 90); // Adjust based on sprite orientation
        }
    }

    /// <summary>
    /// Start a pause in movement
    /// </summary>
    private void StartPause()
    {
        isMoving = false;
        float pauseDuration = Random.Range(minPauseTime, maxPauseTime);
        nextDirectionChange = Time.time + pauseDuration;
    }

    /// <summary>
    /// Keep the NPC within the defined boundaries
    /// </summary>
    private void KeepWithinBoundaries()
    {
        Vector3 position = transform.position;
        bool hitBoundary = false;

        // Check X boundaries
        if (position.x < boundaryMin.x)
        {
            position.x = boundaryMin.x;
            hitBoundary = true;
        }
        else if (position.x > boundaryMax.x)
        {
            position.x = boundaryMax.x;
            hitBoundary = true;
        }

        // Check Y boundaries
        if (position.y < boundaryMin.y)
        {
            position.y = boundaryMin.y;
            hitBoundary = true;
        }
        else if (position.y > boundaryMax.y)
        {
            position.y = boundaryMax.y;
            hitBoundary = true;
        }

        // If we hit a boundary, change direction
        if (hitBoundary)
        {
            transform.position = position;
            ChangeDirection();
        }
    }

    /// <summary>
    /// Public method to set new boundaries
    /// </summary>
    public void SetBoundaries(Vector2 min, Vector2 max)
    {
        boundaryMin = min;
        boundaryMax = max;
    }

    /// <summary>
    /// Public method to respawn the NPC at a new random location
    /// </summary>
    public void Respawn()
    {
        SpawnRandomly();
        ChangeDirection();
        isMoving = true;
    }

    // Visualize boundaries in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(
            (boundaryMin.x + boundaryMax.x) / 2,
            (boundaryMin.y + boundaryMax.y) / 2,
            0
        );
        Vector3 size = new Vector3(
            boundaryMax.x - boundaryMin.x,
            boundaryMax.y - boundaryMin.y,
            0
        );
        Gizmos.DrawWireCube(center, size);
    }
}
