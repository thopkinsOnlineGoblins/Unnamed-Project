using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Rigidbody))]
public class RusherAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float stopRange = 1.0f;

    [Header("Combat")]
    public float contactRange = 1.2f;
    public float contactDamage = 10f;
    public float damageInterval = 0.6f;

    [Header("Sound Effects")]
    public EnemySoundConfig sounds;
    public AudioSource audioSource;

    NavMeshAgent _agent;
    Transform _player;
    float _damageTimer;
    bool _isAwake;
    bool _isDead;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = moveSpeed;
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

        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist > stopRange + 0.1f)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }
        else
        {
            _agent.isStopped = true;
        }

        _damageTimer -= Time.deltaTime;
        if (_damageTimer <= 0f && dist <= contactRange)
        {
            _damageTimer = damageInterval;
            PlaySound(sounds?.attackSound);
            _player.GetComponent<HealthComponent>()?.TakeDamage(contactDamage);
        }
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
}