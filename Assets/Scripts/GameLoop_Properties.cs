using KPFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLoop
{
    private UnitHandler PLAYER;
    public UnitHandler Player
    {
        get
        {
            if (PLAYER == null)
                PLAYER = FindFirstObjectByType<UnitHandler>(FindObjectsInactive.Include);

            return PLAYER;
        }
    }


    //private static Dictionary<VoiceType, Voice> DICT_VOICES;
    //public static Voice GetVoice(VoiceType voiceType)
    //{
    //    if (DICT_VOICES == null)
    //    {
    //        DICT_VOICES = new();

    //        foreach (Voice voice in Resources.LoadAll<Voice>("Voices"))
    //        {
    //            if (!DICT_VOICES.TryAdd(voice.VoiceType, voice))
    //            {
    //                Debug.LogWarning("A voice type duplication detected.");
    //            }
    //        }
    //    }

    //    if (DICT_VOICES.ContainsKey(voiceType))
    //    {
    //        return DICT_VOICES[voiceType];
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Voice type '{voiceType}' is not found. Returning default voice type.");
    //        return DICT_VOICES[VoiceType.Man];
    //    }
    //}

    [SerializeField] private SO_Character[] characters;
    private Dictionary<CharacterName, SO_Character> DICT_CHARACTERS;
    public SO_Character GetCharacter(CharacterName characterName)
    {
        if (DICT_CHARACTERS == null)
        {
            DICT_CHARACTERS = new();

            foreach (SO_Character character in characters)
            {
                if (!DICT_CHARACTERS.TryAdd(character.CharacterName, character))
                {
                    DebugUtility.LogError(ErrorType.DuplicatedKey, character.CharacterName.ToString());
                }
            }
        }

        if (DICT_CHARACTERS.ContainsKey(characterName))
        {
            return DICT_CHARACTERS[characterName];
        }
        else
        {
            DebugUtility.LogError(ErrorType.KeyNotFound, characterName.ToString());
            return null;
        }
    }

    [SerializeField] private SO_Voice[] voices;
    private Dictionary<VoiceType, SO_Voice> DICT_VOICES;
    public SO_Voice GetVoice(VoiceType voiceType)
    {
        if (DICT_VOICES == null)
        {
            DICT_VOICES = new();

            foreach (SO_Voice voice in voices)
            {
                if (!DICT_VOICES.TryAdd(voice.VoiceType, voice))
                {
                    DebugUtility.LogError(ErrorType.DuplicatedKey, voice.VoiceType.ToString());
                }
            }
        }

        if (DICT_VOICES.ContainsKey(voiceType))
        {
            return DICT_VOICES[voiceType];
        }
        else
        {
            DebugUtility.LogError(ErrorType.KeyNotFound, voiceType.ToString());
            return null;
        }
    }

    [SerializeField] private SO_Area[] areas;
    private Dictionary<string, SO_Area> DICT_AREAS;
    public SO_Area GetArea(string areaName)
    {
        if (DICT_AREAS == null)
        {
            DICT_AREAS = new();

            foreach (SO_Area area in areas)
            {
                if (!DICT_AREAS.TryAdd(area.AreaTitle, area))
                {
                    DebugUtility.LogError(ErrorType.DuplicatedKey, area.AreaTitle);
                }
            }
        }

        if (DICT_AREAS.ContainsKey(areaName))
        {
            return DICT_AREAS[areaName];
        }
        else
        {
            DebugUtility.LogError(ErrorType.KeyNotFound, areaName.ToString());
            return null;
        }
    }

    public SO_Area GetAreaByIndex(int index)
    {
        return areas[index];
    }

    public SO_Area GetAreaFirst() => GetAreaByIndex(0);

    public int NumArea => areas.Length;
}
