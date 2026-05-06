using UnityEngine;

[CreateAssetMenu(fileName = "EnemySoundConfig", menuName = "Audio/Enemy Sound Config")]
public class EnemySoundConfig : ScriptableObject
{
    public AudioClip windupSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
}