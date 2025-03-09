using KPFramework;
using UnityEngine;

[CreateAssetMenu(fileName = "New Voice", menuName = "Kutlu/Voice")]
public class SO_Voice : ScriptableObject
{
    public VoiceType VoiceType => voiceType;
    public AudioClip VoiceClip => voiceClip;
    public float RandomPitch => Random.Range(pitchRange.x, pitchRange.y);
    public float CharDelay => charDelay;
    public float SentenceDelay => sentenceDelay;

    [SerializeField] private VoiceType voiceType;
    [SerializeField] private AudioClip voiceClip;
    [SerializeField] private Vector2 pitchRange = new(0.9f, 1.1f);
    [SerializeField] private float sentenceDelay = 0.1f, charDelay = 0.05f;
}
