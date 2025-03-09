using KPFramework;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Kutlu/Character")]
public class SO_Character : ScriptableObject
{
    public SO_Class Class => unitClass;
    public SO_Voice Voice => voice;
    public CharacterName CharacterName => characterName;
    public Sprite SplashArt => splashArt;

    private Data_Unit dataUnit;
    public Data_Unit Data_Unit
    {
        get
        {
            LoadDataIfNull();
            return dataUnit;
        }
    }

    private void LoadDataIfNull()
    {
        if (dataUnit == null)
        {
            var unitData = GetUnitData(characterName, new Data_Unit(new Data_VitalProps(), new Data_UnitProgression(startLevel)));
            dataUnit = unitData;
        }
    }

    private Data_Unit GetUnitData(CharacterName charName, Data_Unit defaultData)
    {
        var UnitDatas = SaveSystem.CurrentSE.UnitDatas;
        UnitDatas.InitializeDict();
        if (UnitDatas.TryGetValue(charName, out Data_Unit foundData))
        {
            return foundData;
        }
        else
        {
            return defaultData;
        }
    }

    private void SetUnitData(CharacterName charName, Data_Unit unitData)
    {
        var UnitDatas = SaveSystem.CurrentSE.UnitDatas;
        UnitDatas.InitializeDict();
        if (UnitDatas.ContainsKey(charName))
        {
            UnitDatas.Modify(charName, unitData);
        }
    }

    [SerializeField] private int startLevel = 1;
    [SerializeField] private SO_Class unitClass;

    [SerializeField] private Sprite splashArt;
    [SerializeField] private SO_Voice voice;
    [SerializeField] private string description;
    [SerializeField] private string descriptionLocalizationKey;
    [SerializeField] private CharacterName characterName;
    [SerializeField] private Nationality nationality;
    [SerializeField] private Characteristics[] characteristics;
}

public enum Nationality
{
    // To the person reading this: Please avoid reading this specific script in future updates to prevent spoiling the content for yourself.
    NotImportant = 0,
#if UNITY_EDITOR
    Nyambe = 1, // Real Continent: africa - Established Country: Federation of Yambele
    Frysta = 2, // Real Continent: antarctica - Established Country: Frystalon
    Azura = 3, // Real Continent: asia - Established Country: Azurian Union
    Eurydor = 4, // Real Continent: europe - Established Country: Eurydor Union
    NorthNoramis = 5, // Real Continent: north america - Established Country: United States Of Azcalor
    SouthNoramis = 6, // Real Continent: north america - Established Country: United States Of Azcalor
    Oceathralia = 7, // Real Continent: australia - Established Country: Ocealia
#endif
}

public enum Characteristics
{
    // Ki�ilik �zellikleri
    Brave = 0,          // Cesur
    Cowardly = 1,       // Korkak
    Charismatic = 2,    // Karizmatik
    Manipulative = 3,   // Manip�latif
    Compassionate = 4,  // �efkatli
    Ruthless = 5,       // Ac�mas�z
    Loyal = 6,          // Sad�k
    BetrayalProne = 7,  // �hanete Meyilli
    Ambitious = 8,      // H�rsl�
    Calm = 9,           // Sakin
    HotTempered = 10,    // Sinirli
    Loud = 11,           // Sesli

    // Zihinsel Yetenekler
    Intelligent = 12,    // Zeki
    Strategist = 13,     // Stratejist
    Observant = 14,      // G�zlemci
    Creative = 15,       // Yarat�c�
    Deceptive = 16,      // Aldat�c�

    // Fiziksel �zellikler
    Strong = 17,         // G��l�
    Agile = 18,          // �evik
    Resilient = 19,      // Dayan�kl�
    Weak = 20,           // Zay�f
    QuickReflexes = 21,  // H�zl� Refleksler

    // Sosyal �zellikler
    Friendly = 22,       // Dost canl�s�
    Introverted = 23,    // ��ed�n�k
    Extroverted = 24,    // D��ad�n�k
    Persuasive = 25,     // �kna Edici
    Reserved = 26,       // �ekingen

    // Di�er �zellikler
    Mysterious = 27,     // Gizemli
    Determined = 28,     // Kararl�
    Fearless = 29,       // Korkusuz
    Curious = 30,        // Merakl�
    Unpredictable = 31   // Tahmin Edilemez
}

public enum CharacterName
{
    None = 0,
    // main character(s)
    Althea = 1,
    Narrator = 100,
    RobotSnowman = 1000,
    Uggha = 1001,
    Buggha = 1002,
}