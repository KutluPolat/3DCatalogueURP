using KPFramework;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    public static TypeWriter Instance { get; private set; }

    public bool IsWriting { get; private set; }

    private Coroutine _cacheCoroutine;
    private bool fastForward;

    private void Awake()
    {
        IsWriting = false;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void FastForward()
    {
        fastForward = true;
    }

    public void Clear(TMP_Text textComponent)
    {
        if (_cacheCoroutine != null)
            StopCoroutine(_cacheCoroutine);

        textComponent.text = string.Empty;
    }

    public void Write(TMP_Text textComponent, string instantWrittenPrefix, string text, SO_Voice voice)
    {
        if (string.IsNullOrEmpty(text))
        {
            DebugUtility.LogError(ErrorType.MethodParameterIsNull, "text for typewriter");
            return;
        }

        fastForward = false;

        if (_cacheCoroutine != null)
            StopCoroutine(_cacheCoroutine);

        textComponent.text = string.Empty;

        if (!string.IsNullOrEmpty(instantWrittenPrefix))
            textComponent.text += instantWrittenPrefix;

        _cacheCoroutine = StartCoroutine(TypeWrite(textComponent, text, voice));
    }

    private IEnumerator TypeWrite(TMP_Text textComponent, string text, SO_Voice voice)
    {
        IsWriting = true;
        float charDelay = voice != null ? voice.CharDelay : 0.01f;

        // initialize
        fastForward = false;

        foreach (char i in text)
        {
            textComponent.text += i.ToString();

            if (voice == null)
            {
                DebugUtility.LogError(ErrorType.ComponentNotFound, "Voice");
                continue;
            }

            // play char by char with sound.
            if (!fastForward)
            {
                AudioManager.Instance.PlayVoice(voice);
                //yield return new WaitForSeconds(char.IsPunctuation(i) ? voice.SentenceDelay : voice.CharDelay);
                yield return new WaitForSeconds(charDelay);
            }
        }

        // play a final sound if fast forwarded
        if (fastForward)
        {
            AudioManager.Instance.PlayVoice(voice);
        }

        // cleaning
        _cacheCoroutine = null;
        IsWriting = false;
    }
}