using UnityEngine;

[CreateAssetMenu(fileName = "RoomSoundConfig", menuName = "Audio/Room Sound Config")]
public class RoomSoundConfig : ScriptableObject
{
    public AudioClip doorCloseSound;
    public AudioClip spawnSound;
    public AudioClip waveCompleteSound;
    public AudioClip roomCompleteSound;
}