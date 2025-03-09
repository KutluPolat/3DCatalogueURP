using KPFramework;
using UnityEngine;

/*
 * TO-DO: TERRAIN KULLANMAYI OGREN!!
 * 
 * 
 * TO-DO: PROJEYI URP YAP
 * TO-DO: MATERYALLERÝ URPYE ÇEVÝR (TÜM MATERYALLERÝ SEÇÝP YAP)
 * TO-DO: GPU OCCLUSION CULLING
 * TO-DO:
    Detect Hardware Capabilities: Implement a system to detect the 
    player's GPU capabilities at runtime. If the GPU does not support compute shaders, 
    disable GPU Occlusion Culling and consider using alternative optimization techniques.
     
  * TO-DO:
    Provide Configuration Options: Allow players to adjust graphics settings, 
    including the option to enable or disable GPU Occlusion Culling, 
    based on their hardware capabilities.
 */

[System.Serializable]
public class Data_Options
{
    public float MusicVolume = 0.5f;
    public float SoundsVolume = 1f;
    public float VoiceVolume = 0.5f;
    public int GraphicsQuality  = 2;
    public int TargetFrameRate = 60;
    public int VSyncCount = 0;
    public int Vibrations = 6;
    public Language Language = Language.English;


    public Data_Options(SystemLanguage currentSystemLanguage)
    {
        switch (currentSystemLanguage)
        {
            case SystemLanguage.Turkish:
                Language = Language.Turkish;
                break;
            case SystemLanguage.English:
                Language = Language.English;
                break;
            case SystemLanguage.Spanish:
                Language = Language.Spanish;
                break;
            case SystemLanguage.Portuguese:
                Language = Language.Portuguese;
                break;
            case SystemLanguage.German:
                Language = Language.German;
                break;
            case SystemLanguage.French:
                Language = Language.French;
                break;
            case SystemLanguage.Italian:
                Language = Language.Italian;
                break;
            case SystemLanguage.Polish:
                Language = Language.Polish;
                break;
            case SystemLanguage.Russian:
                Language = Language.Russian;
                break;
            case SystemLanguage.ChineseSimplified:
                Language = Language.ChineseSimplified;
                break;
            case SystemLanguage.ChineseTraditional:
                Language = Language.ChineseTraditional;
                break;
            case SystemLanguage.Japanese:
                Language = Language.Japanese;
                break;
            case SystemLanguage.Korean:
                Language = Language.Korean;
                break;
            case SystemLanguage.Hindi:
                Language = Language.Indian;
                break;
            case SystemLanguage.Arabic:
                Language = Language.Arabic;
                break;
            case SystemLanguage.Indonesian:
                Language = Language.Indonesian;
                break;
            case SystemLanguage.Thai:
                Language = Language.Thai;
                break;
            case SystemLanguage.Vietnamese:
                Language = Language.Vietnamese;
                break;

            default:
                Language = Language.English;
                break;
        }
    }
}