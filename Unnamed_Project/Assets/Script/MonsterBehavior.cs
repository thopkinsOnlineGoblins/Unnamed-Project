using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{

    public Transform player;
    public float orbitRadius = 2f;
    public float orbitSpeed = 2f;

    float angle;

    public float maxHunger = 100f;
    public float currentHunger = 100f;
    public float hungerDrain = 5f;

    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public enum CreatureState
    {
        Idle,
        Combat,
        Hungry
    }

    public CreatureState currentState = CreatureState.Idle;

    void Update()
    {
        DrainFood();
        Think();
    }

    void Think() 
    {
        switch (currentState)
        {
            case CreatureState.Idle:
                IdleBehavior();
                break;

            case CreatureState.Combat:
                CombatBehavior();
                break;

            case CreatureState.Hungry:
                HungryBehavior();
                break;
        }
    }

    void IdleBehavior() 
    {
        angle += orbitSpeed * Time.deltaTime;

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * orbitRadius;

        transform.position = player.position + offset;
    }
    void CombatBehavior() 
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3 direction = (mouseWorld - player.position).normalized;

        float combatDistance = 2f;

        transform.position = player.position + direction * combatDistance;
    }
    void HungryBehavior() 
    {
        orbitSpeed = 0.5f;
    }

    void DrainFood()
    {
        currentHunger -= hungerDrain * Time.deltaTime;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        if (currentHunger <= 0)
        {
            currentState = CreatureState.Hungry;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}
