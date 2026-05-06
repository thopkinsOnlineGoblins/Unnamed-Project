using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Rigidbody))]
public class SwingerAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float approachStopDistance = 2.5f;

    [Header("Attack")]
    public float attackRange = 2.8f;
    public float attackDamage = 12f;
    public float swingArcAngle = 100f;

    [Header("Timing")]
    public float windupDuration = 1.2f;
    public float swingDuration = 0.25f;
    public float cooldownDuration = 1.8f;

    [Header("Sound Effects")]
    public EnemySoundConfig sounds;
    public AudioSource audioSource;

    enum State { Approach, Windup, Swing, Cooldown }
    State _state = State.Approach;
    float _stateTimer;

    NavMeshAgent _agent;
    Transform _player;
    bool _isAwake;
    bool _isDead;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = moveSpeed;
        _agent.stoppingDistance = approachStopDistance;
        var rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    void Start()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        GetComponent<HealthComponent>().OnDeath += Die;
    }

    public void WakeUp() => _isAwake = true;

    void Update()
    {
        if (!_isAwake || _isDead || _player == null) return;
        if (!_agent.enabled || !_agent.isOnNavMesh) return;

        _stateTimer -= Time.deltaTime;

        switch (_state)
        {
            case State.Approach:
                _agent.SetDestination(_player.position);
                if (Vector3.Distance(transform.position, _player.position) <= approachStopDistance)
                    EnterState(State.Windup, windupDuration);
                break;

            case State.Windup:
                _agent.ResetPath();
                FacePlayer();
                if (_stateTimer <= 0f)
                    Swing();
                break;

            case State.Swing:
                if (_stateTimer <= 0f)
                    EnterState(State.Cooldown, cooldownDuration);
                break;

            case State.Cooldown:
                if (_stateTimer <= 0f)
                    EnterState(State.Approach, 0f);
                break;
        }
    }

    void EnterState(State next, float duration)
    {
        // Play windup sound exactly when entering the Windup state
        if (next == State.Windup) PlaySound(sounds?.windupSound);

        _state = next;
        _stateTimer = duration;
    }

    void Swing()
    {
        EnterState(State.Swing, swingDuration);
        PlaySound(sounds?.attackSound);

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;
            float angle = Vector3.Angle(transform.forward, hit.transform.position - transform.position);
            if (angle <= swingArcAngle * 0.5f)
                hit.GetComponent<HealthComponent>()?.TakeDamage(attackDamage);
        }
    }

    void FacePlayer()
    {
        Vector3 dir = _player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 6f);
    }

    void Die()
    {
        _isDead = true;
        _agent.enabled = false;
        PlaySound(sounds?.deathSound);
        Destroy(gameObject, 0.1f);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource != null)
            audioSource.PlayOneShot(clip);
        else
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}