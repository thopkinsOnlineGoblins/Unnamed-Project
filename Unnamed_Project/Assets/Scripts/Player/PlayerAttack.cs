using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.8f;
    public float attackDamage = 34f;
    public float attackCooldown = 0.5f;

    [Header("Sound Effects")]
    public AudioClip swingSound;   // plays every attack attempt
    public AudioClip hitSound;     // plays when an enemy is actually hit
    public AudioSource audioSource;

    float _cooldownTimer;

    void Update()
    {
        _cooldownTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && _cooldownTimer <= 0f)
            Attack();
    }

    void Attack()
    {
        _cooldownTimer = attackCooldown;

        PlaySound(swingSound);

        bool hitAnything = false;
        Vector3 hitCenter = transform.position + transform.forward * (attackRange * 0.5f);
        Collider[] hits = Physics.OverlapSphere(hitCenter, attackRange * 0.5f);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            var health = hit.GetComponentInParent<HealthComponent>();
            if (health == null) continue;
            health.TakeDamage(attackDamage);
            FXHelper.SpawnBurst(hit.transform.position + Vector3.up, new Color(1f, 0.35f, 0f));
            hitAnything = true;
        }

        if (hitAnything) PlaySound(hitSound);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (attackRange * 0.5f), attackRange * 0.5f);
    }
}