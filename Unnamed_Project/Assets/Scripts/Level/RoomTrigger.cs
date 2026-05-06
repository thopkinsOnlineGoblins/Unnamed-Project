using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyWave
{
    public GameObject prefab;
    public int count = 1;
    public string displayName;
    [TextArea(2, 4)] public string description;
}

public class RoomTrigger : MonoBehaviour
{
    [Header("Enemies")]
    public List<EnemyWave> waves = new List<EnemyWave>();
    public List<Transform> spawnPoints;

    [Header("Door")]
    public GameObject door;

    [Header("Intro Timing")]
    public float introDuration = 3.5f;
    public float introFadeTime = 0.5f;

    bool _activated;
    PlayerMovement _player;
    LockOnSystem _lockOn;

    void OnTriggerEnter(Collider other)
    {
        if (_activated || !other.CompareTag("Player")) return;
        _activated = true;

        _player = other.GetComponentInParent<PlayerMovement>();
        _lockOn  = other.GetComponentInParent<LockOnSystem>();
        if (door != null) door.SetActive(true);
        StartCoroutine(RunRoom());
    }

    IEnumerator RunRoom()
    {
        for (int i = 0; i < waves.Count; i++)
            yield return StartCoroutine(RunWave(waves[i], i));

        OpenDoor();
    }

    IEnumerator RunWave(EnemyWave wave, int waveIndex)
    {
        int count = Mathf.Max(1, wave.count);
        Transform sp = (spawnPoints != null && waveIndex < spawnPoints.Count)
            ? spawnPoints[waveIndex]
            : transform;

        for (int i = 0; i < count; i++)
            yield return StartCoroutine(SpawnAndFight(wave, sp, isFirst: i == 0));
    }

    // First enemy in a wave: freeze player, show intro card, then start combat.
    // Subsequent enemies in the same wave: brief pause then straight to combat.
    IEnumerator SpawnAndFight(EnemyWave wave, Transform sp, bool isFirst)
    {
        if (isFirst)
        {
            SetFrozen(true);
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
        }

        // Snap spawn position to the nearest valid NavMesh point
        Vector3 spawnPos = sp.position;
        Debug.DrawRay(sp.position, Vector3.up * 3f, Color.red, 10f);
        if (NavMesh.SamplePosition(sp.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            Debug.LogWarning($"[RoomTrigger] No NavMesh found within 3m of spawn point '{sp.name}' at {sp.position}. Enemy may not navigate correctly.");
        }

        var go = Instantiate(wave.prefab, spawnPos, sp.rotation);
        FXHelper.SpawnBurst(spawnPos + Vector3.up * 0.8f, new Color(0.5f, 0f, 1f));
        _lockOn?.ForceTarget(go);

        bool dead = false;
        var health = go.GetComponent<HealthComponent>();
        if (health != null) health.OnDeath += () => dead = true;
        else dead = true;

        if (isFirst)
        {
            if (!string.IsNullOrWhiteSpace(wave.displayName) && RoomIntroUI.Instance != null)
                yield return StartCoroutine(RoomIntroUI.Instance.Show(wave.displayName, wave.description, introDuration, introFadeTime));
            else
                yield return new WaitForSeconds(introDuration);
            SetFrozen(false);
        }

        go.GetComponent<RusherAI>()?.WakeUp();
        go.GetComponent<SwingerAI>()?.WakeUp();

        while (!dead)
        {
            if (go == null) break;
            yield return null;
        }
    }

    void SetFrozen(bool frozen)
    {
        if (_player != null) _player.IsFrozen = frozen;
    }

    void OpenDoor()
    {
        SetFrozen(false);
        if (door != null) door.SetActive(false);
    }
}
