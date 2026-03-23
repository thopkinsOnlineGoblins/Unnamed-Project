using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LockOnSystem : MonoBehaviour
{
    [Header("Lock-On Settings")]
    public float castRadius = 5f;

    [Header("Lock-On Mode")]
    [Tooltip("False = Closest (cycles nearest to farthest), True = Vision (cone only)")]
    public bool visionMode = false;
    public float visionAngle = 45f;
    public float maxDistance = 5f;

    [Header("Debug")]
    public bool debugVisible = true;

    public GameObject TargetEnemy;

    [SerializeField] private List<GameObject> _sortedEnemies = new List<GameObject>();
    private int _cycleIndex = -1;

    void Update()
    {
        RefreshEnemies();

        if (Input.GetKeyDown(KeyCode.T))
            CycleTarget();

        if (Input.GetKeyDown(KeyCode.Y))
            TargetEnemy = null;

        CheckDistance();
    }

    void RefreshEnemies()
    {
        List<GameObject> enemies = visionMode ? GetVisionEnemies() : GetEnemies();
        _sortedEnemies = enemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .ToList();
    }

    void CycleTarget()
    {
        if (_sortedEnemies.Count == 0)
        {
            TargetEnemy = null;
            _cycleIndex = -1;
            return;
        }

        if (TargetEnemy != null)
        {
            int existingIndex = _sortedEnemies.IndexOf(TargetEnemy);
            _cycleIndex = existingIndex != -1 ? existingIndex : -1;
        }
        else
        {
            _cycleIndex = -1;
        }

        _cycleIndex = (_cycleIndex + 1) % _sortedEnemies.Count;
        TargetEnemy = _sortedEnemies[_cycleIndex];
    }

    void CheckDistance()
    {
        if (TargetEnemy != null)
            if (Vector3.Distance(transform.position, TargetEnemy.transform.position) > maxDistance)
                TargetEnemy = null;
    }

    // --- Detection ---

    List<GameObject> GetEnemies()
    {
        var result = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, castRadius);
        foreach (var col in colliders)
            if (col.CompareTag("Enemy"))
                result.Add(col.gameObject);
        return result;
    }

    List<GameObject> GetVisionEnemies()
    {
        var result = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, castRadius);
        foreach (var col in colliders)
            if (col.CompareTag("Enemy") && AngleFromForward(col.transform.position) <= visionAngle)
                result.Add(col.gameObject);
        return result;
    }

    // --- Helpers ---

    float AngleFromForward(Vector3 point)
    {
        Vector3 dir = (point - transform.position).normalized;
        return Vector3.Angle(transform.forward, dir);
    }

    // --- Debug Gizmos ---

    void OnDrawGizmos()
    {
        if (!debugVisible) return;

        if (!visionMode)
            DrawSphere();
        else
            DrawVisionCone();

        if (TargetEnemy == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, TargetEnemy.transform.position);
        Gizmos.DrawWireSphere(TargetEnemy.transform.position, 0.5f);
    }

    void DrawSphere()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, castRadius);
    }

    void DrawVisionCone()
    {
        Vector3 forward = transform.forward * castRadius;

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
        Vector3 leftEdge = Quaternion.AngleAxis(-visionAngle, transform.up) * forward;
        Vector3 rightEdge = Quaternion.AngleAxis(visionAngle, transform.up) * forward;
        Gizmos.DrawLine(transform.position, transform.position + leftEdge);
        Gizmos.DrawLine(transform.position, transform.position + rightEdge);
        Gizmos.DrawLine(transform.position, transform.position + forward);

        int arcSteps = 24;
        for (int i = 0; i < arcSteps; i++)
        {
            float a1 = Mathf.Lerp(-visionAngle, visionAngle, (float)i / arcSteps);
            float a2 = Mathf.Lerp(-visionAngle, visionAngle, (float)(i + 1) / arcSteps);
            Vector3 p1 = transform.position + Quaternion.AngleAxis(a1, transform.up) * forward;
            Vector3 p2 = transform.position + Quaternion.AngleAxis(a2, transform.up) * forward;
            Gizmos.DrawLine(p1, p2);
        }
    }
}