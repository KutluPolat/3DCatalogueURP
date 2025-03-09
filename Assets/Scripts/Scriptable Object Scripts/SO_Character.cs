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
    // Kiþilik Özellikleri
    Brave = 0,          // Cesur
    Cowardly = 1,       // Korkak
    Charismatic = 2,    // Karizmatik
    Manipulative = 3,   // Manipülatif
    Compassionate = 4,  // Þefkatli
    Ruthless = 5,       // Acýmasýz
    Loyal = 6,          // Sadýk
    BetrayalProne = 7,  // Ýhanete Meyilli
    Ambitious = 8,      // Hýrslý
    Calm = 9,           // Sakin
    HotTempered = 10,    // Sinirli
    Loud = 11,           // Sesli

    // Zihinsel Yetenekler
    Intelligent = 12,    // Zeki
    Strategist = 13,     // Stratejist
    Observant = 14,      // Gözlemci
    Creative = 15,       // Yaratýcý
    Deceptive = 16,      // Aldatýcý

    // Fiziksel Özellikler
    Strong = 17,         // Güçlü
    Agile = 18,          // Çevik
    Resilient = 19,      // Dayanýklý
    Weak = 20,           // Zayýf
    QuickReflexes = 21,  // Hýzlý Refleksler

    // Sosyal Özellikler
    Friendly = 22,       // Dost canlýsý
    Introverted = 23,    // Ýçedönük
    Extroverted = 24,    // Dýþadönük
    Persuasive = 25,     // Ýkna Edici
    Reserved = 26,       // Çekingen

    // Diðer Özellikler
    Mysterious = 27,     // Gizemli
    Determined = 28,     // Kararlý
    Fearless = 29,       // Korkusuz
    Curious = 30,        // Meraklý
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