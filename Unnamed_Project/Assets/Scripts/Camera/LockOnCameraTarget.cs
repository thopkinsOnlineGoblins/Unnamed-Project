using UnityEngine;

public class LockOnCameraTarget : MonoBehaviour
{
    public Transform player;
    public float enemyWeight = 0.5f; // 0 = fully on player, 0.5 = midpoint, 1 = fully on enemy

    LockOnSystem lockOn;

    void Start()
    {
        lockOn = player.GetComponent<LockOnSystem>();
    }

    void LateUpdate()
    {
        if (lockOn == null || lockOn.TargetEnemy == null)
        {
            transform.position = player.position;
            return;
        }

        transform.position = Vector3.Lerp(
            player.position,
            lockOn.TargetEnemy.transform.position,
            enemyWeight
        );
    }
}