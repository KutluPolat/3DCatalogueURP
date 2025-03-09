using DG.Tweening;
using KPFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    SFX = 0, // This is considered as "Sound" in options
    Environment = 1, // This is considered as "Sound" in options
    Music = 20,
    Voice = 21,
}

public class AudioManager : ScriptBase
{
    [System.Serializable]
    private struct Audio
    {
        public AudioNames AudioName;
        [SerializeField] private AudioClip[] AudioClip;

        public AudioClip GetClip()
        {
            if (AudioClip.Length > 0)
                return AudioClip[Random.Range(0, AudioClip.Length)];
            else
                return null;
        }
    }

    private struct ActiveAudio
    {
        public AudioType AudioType;
        public AudioNames AudioName;
        public AudioSource AudioSource;

        public ActiveAudio(AudioType audioType, AudioNames audioName, AudioSource audioSource)
        {
            AudioType = audioType;
            AudioName = audioName;
            AudioSource = audioSource;
        }
    }

    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup mixerGroupSFX;
    [SerializeField] private AudioMixerGroup mixerGroupMusic;
    [SerializeField] private AudioMixerGroup mixerGroupVoice;
    [SerializeField] private AudioMixerGroup mixerGroupEnvironment;

    float LinearToDecibels(float linear)
    {
        // Avoid log of zero
        if (linear <= 0.0001f)
            return -80f; // Minimum dB in Unity mixers is around -80dB

        return Mathf.Log10(linear) * 20f;
    }


    [SerializeField] private Audio[] Audios;

    private Dictionary<AudioType, List<ActiveAudio>> dictActiveAudios = new();
    private ObjectPool<AudioSource> poolAudioSource;
    private Dictionary<AudioNames, Audio> _dictAudios = new();
    private Data_Options options;
    private AudioType[] allAudioTypes;
    private int numAudioPlayed;
    private float audioSourcesLastClearedTime;
    private float lastTimeVoicePlayed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Audio audio in Audios)
            _dictAudios.Add(audio.AudioName, audio);

        var prefabAudioSource = new GameObject("PREFAB-AudioSource").AddComponent<AudioSource>();
        prefabAudioSource.transform.SetParent(transform);
        prefabAudioSource.playOnAwake = false;
        prefabAudioSource.loop = false;
        prefabAudioSource.gameObject.SetActive(false);

        poolAudioSource = new ObjectPool<AudioSource>(1, transform, prefabAudioSource);
        options = SaveSystem.Options;

        allAudioTypes = (AudioType[])System.Enum.GetValues(typeof(AudioType));
        foreach (AudioType audioType in allAudioTypes)
        {
            dictActiveAudios.Add(audioType, new());
        }
    }

    private void Update()
    {
        if (Time.time - audioSourcesLastClearedTime < 1f)
            return;
        audioSourcesLastClearedTime = Time.time;

        foreach (var audioType in allAudioTypes)
        {
            var activeAudios = dictActiveAudios[audioType];
            for (int i = activeAudios.Count - 1; i >= 0; i--)
            {
                var activeAudio = activeAudios[i];
                if (!activeAudio.AudioSource.isPlaying)
                {
                    poolAudioSource.Push(activeAudio.AudioSource);
                    activeAudios.RemoveAt(i);
                }
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        AddEvent(EventName.AudioOptionsChanged, (o) =>
        {
            if (o is Data_Options newOptions)
            {
                options = newOptions;
                UpdateMixerVolumes(options);
            }
        });


    }

    private void Start()
    {
        UpdateMixerVolumes(SaveSystem.Options);
    }

    void UpdateMixerVolumes(Data_Options options)
    {
        audioMixer.SetFloat("VolumeOfSFX", LinearToDecibels(options.SoundsVolume));
        audioMixer.SetFloat("VolumeOfEnvironment", LinearToDecibels(options.SoundsVolume));
        audioMixer.SetFloat("VolumeOfMusic", LinearToDecibels(options.MusicVolume));
        audioMixer.SetFloat("VolumeOfVoice", LinearToDecibels(options.VoiceVolume));
    }

    public void PlayVoice(SO_Voice voice)
    {
        if (Instance == null) return;
        if (voice != null)
        {
            lastTimeVoicePlayed = Time.time;
            PlaySound(AudioType.Voice, AudioNames.None, voice.VoiceClip, false, voice.RandomPitch);
        }
        else
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(voice));
        }
    }

    public void PlayVoice(AudioClip voiceClip, float pitch = 1f)
    {
        if (Instance == null) return;
        if (voiceClip != null)
        {
            lastTimeVoicePlayed = Time.time;
            PlaySound(AudioType.Voice, AudioNames.None, voiceClip, false, pitch);
        }
        else
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(voiceClip));
        }
    }

    public void PlaySFX(AudioNames targetAudioName, float pitch = 1f)
    {
        if (Instance == null) return;
        if (_dictAudios.ContainsKey(targetAudioName))
        {
            PlaySound(AudioType.SFX, targetAudioName, _dictAudios[targetAudioName].GetClip(), false, pitch);
        }
        else
        {
            DebugUtility.LogError(ErrorType.KeyNotFound, targetAudioName.ToString());
        }
    }

    public AudioSource PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (Instance == null) return null;
        if (clip != null)
        {
            return PlaySound(AudioType.SFX, AudioNames.None, clip, false, pitch);
        }
        else
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(clip));
        }

        return null;
    }

    public void PlayEnvironment(AudioNames targetAudioName, bool loop, float pitch = 1f)
    {
        if (Instance == null) return;
        if (_dictAudios.ContainsKey(targetAudioName))
        {
            PlaySound(AudioType.Environment, targetAudioName, _dictAudios[targetAudioName].GetClip(), loop, pitch);
        }
        else
        {
            DebugUtility.LogError(ErrorType.KeyNotFound, targetAudioName.ToString());
        }
    }

    public AudioSource PlayMusic(AudioNames targetAudioName, bool loop, float pitch = 1f)
    {
        if (Instance == null) return null;
        if (_dictAudios.ContainsKey(targetAudioName))
        {
            return PlaySound(AudioType.Music, targetAudioName, _dictAudios[targetAudioName].GetClip(), loop, pitch);
        }
        else
        {
            DebugUtility.LogError(ErrorType.KeyNotFound, targetAudioName.ToString());
        }

        return null;
    }

    public AudioSource PlayMusic(AudioClip clip, bool loop, float pitch = 1f)
    {
        if (Instance == null) return null;
        if (clip != null)
        {
            return PlaySound(AudioType.Music, AudioNames.None, clip, loop, pitch);
        }
        else
        {
            DebugUtility.LogError(ErrorType.ComponentNotFound, nameof(clip));
        }

        return null;
    }

    public void StopSpecificAudioFirst(AudioType audioType, AudioNames stoppedAudioName, float fadeDuration)
    {
        foreach (var activeAudio in dictActiveAudios[audioType])
        {
            if (activeAudio.AudioName == stoppedAudioName)
            {
                if (fadeDuration <= 0f)
                {
                    activeAudio.AudioSource.Stop();
                }
                else
                {
                    activeAudio.AudioSource.DOFade(0f, fadeDuration).OnComplete(() => activeAudio.AudioSource.Stop());
                }

                return;
            }
        }
    }

    public void StopSpecificAudioAll(AudioType audioType, AudioNames stoppedAudioName)
    {
        foreach (var activeAudio in dictActiveAudios[audioType])
        {
            if (activeAudio.AudioName == stoppedAudioName)
            {
                activeAudio.AudioSource.Stop();
            }
        }
    }

    public void StopAllActiveAudios()
    {
        foreach (var dictActiveAudio in dictActiveAudios)
        {
            foreach (var activeAudio in dictActiveAudio.Value)
            {
                activeAudio.AudioSource.Stop();
            }
        }
    }

    public AudioSource PlaySound(AudioType audioType, AudioNames audioName, AudioClip clip, bool loop, float pitch = 1f, float volume = 1f)
    {
        if (clip == null)
        {
            DebugUtility.LogError(ErrorType.MethodParameterIsNull, nameof(AudioClip));
            return null;
        }

        AudioMixerGroup mixerGroup = null;
        bool interruptable = false;

        switch (audioType)
        {
            case AudioType.SFX:
                interruptable = true;
                mixerGroup = mixerGroupSFX; break;
            case AudioType.Music:
                mixerGroup = mixerGroupMusic; break;
            case AudioType.Environment:
                mixerGroup = mixerGroupEnvironment; break;
            case AudioType.Voice:
                interruptable = true;
                mixerGroup = mixerGroupVoice; break;
            default:
                mixerGroup = null;
                DebugUtility.LogError(ErrorType.SwitchCaseNotFound, audioType.ToString());
                return null;
        }

        if (interruptable && volume == 0f)
            return null;

        var audioSource = poolAudioSource.Pop();
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
        
        dictActiveAudios[audioType].Add(new ActiveAudio(audioType, audioName, audioSource));

        return audioSource;
    }

    public void OnNextPanel()
    {
        PlaySFX(AudioNames.ButtonClicked, 1.2f);
    }
}