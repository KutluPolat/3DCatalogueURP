namespace KPFramework
{
    public enum Class
    {
        Undefined = 0,
        Civillian = 1000,

        // Vitality Based
        Juggernaut = 1,      // V++++ | S+    | M
        Knight = 2,          // V+++  | S++   | M
        Battlemage = 3,      // V+++  | S+    | M+

        // Stamina Based
        Swiftshot = 4,       // V+    | S++++ | M
        Duelist = 5,         // V++   | S+++  | M
        Spellblade = 6,      // V+    | S++   | M++

        // Mental Based
        Voidcaster = 7,      // V+    | S     | M++++
        Spellweaver = 8,     // V++   | S     | M+++
        MysticGuardian = 9,  // V+++  | S     | M++
    }

    public enum CharGrade
    {
        Peasant = 0, // F 0 - 15
        Adventurer = 1, // D 16 - 30
        Hero = 2, // C 31 - 45
        Legend = 3, // B 46 - 60
        Myth = 4, // A 61 - 75
        Demigod = 5, // S 76 - 90
        God = 6, // SSS 91+
    }

    public enum Stat
    {
        Vitality = 0,
        Stamina = 1,
        Mental = 2,
    }

    public enum SaveType
    {
        AutoSave = 0,
        QuickSave = 1,
        ManualSave = 2,
    }

    public enum CollectableName
    {
        Barrel = 1000,
        Crate = 1001,
        Chest = 1002,
    }

    public enum QuestName
    {

    }

    public enum Fact
    {
        RealName = 0,
    }

    public enum SplashArtPosition
    {
        Closed = 0,
        Left = 1,
        Right = 2,
    }

    public enum AnimType
    {
        Idle = 0,
        Walk = 1,
        Run = 2,
        Dance = 3,
    }

    public enum AudioNames
    {
        None = 0,
        ButtonPointerEnter = 3,
        ButtonClicked = 4,
        Note = 100,
        
        Environment_CreepyWhiteNoise = 101,
        
        VFX_HyperDriveStart = 500,
        VFX_EngineIgnition = 501,
        Environment_CreepyWoods = 502,
        Environment_PeacefulWoods = 503,
        VFX_Success = 504,
        VFX_ToBeContinue = 505,
        Music_CheesyComedy = 506,
        VFX_Whoosh = 507,
        VFX_SlideOnDirt = 508,
        VFX_GameStart = 509,
    }

    public enum VoiceType
    {
        NONE = -1,
        Witch = 0,
        Troll = 1,
        Boy = 2,
        Man = 3,
        OldMan = 4,
        Girl = 5,
        Woman = 6,
        OldWoman = 7,
        Narrator = 8,
        NarratorInDream = 9,
        Eldavoid = 10,
        Radiant = 11,
        Robot = 12,
        GoblinElder = 13,
        GoblinBoy = 14,
    }

    public enum TextAssetColumns
    {
        CharID = 0,
        CharName = 1,
        Key = 2,
        Languages = 3, // 3 and after
    }

    public enum Language
    {
        Turkish = 0,
        English = 1,
        Spanish = 2,
        Portuguese = 3,
        German = 4,
        French = 5,
        Italian = 6,
        Polish = 7,
        Russian = 8,
        ChineseSimplified = 9,
        ChineseTraditional = 10,
        Japanese = 11,
        Korean = 12,
        Indian = 13,
        Arabic = 14,
        Bengali = 15,
        Indonesian = 16,
        Thai = 17,
        Vietnamese = 18,
    }

    public enum SpawnPointName
    {
        NotSelected = 0,
        InitialPoint = 1,
        Veltenburg_MyHouse = 2,
        Veltenburg_SchoolEntrance = 3,
    }

    public enum Chapter
    {
        Unselected = 0,
        WakeUp = 1,
        Christmas = 2,
        PrimitiveVillage = 3,
    }

    public enum UIElement
    {
        Unselected = -1,
        MainMenu = 0,
        Options = 1,
        //OptionsOpener = 2,
        MainTalkBalloon = 3,
        MiniMenuPopUp = 4,
        GUI = 5,
        Credits = 6,
        SaveSlotsPanel = 7,
    }

    public enum UIPanel
    {
        Unselected = -1,
        Initial = 0,
        Options = 1,
        Dialogue = 2,
        Credits = 3,
        ManageSaveSlotsPanel = 4,
        InGame = 5,
        Menu = 6,
    }

    public enum ErrorType
    {
        SwitchCaseNotFound = 0,
        WrongEventParameter = 1,
        WaitForSingleton = 2,
        KeyNotFound = 3,
        NotInitialized = 4,
        //UILocalizationKeyIsBypassed = 5,
        NotImplemented = 6,
        ComponentNotFound = 7,
        MethodParameterIsNull = 8,
        Localization = 9,
        DuplicatedKey = 10,
        TagShouldBe = 11,
        MajorError = 12,
    }

    public enum DebugType
    {
        EditorOnly,
        ForLater,
        Error,
        Warning,
    }

    public enum EventName
    {
        Unidentified = -1,
        UnusedEvent = 0,
        TriggerUIAction = 1,
        AudioOptionsChanged = 3,
        Vibrate = 7,
        //UISetButtonInteractable = 8,
        LanguageChanged = 9,
        HideInGame = 10,
        ShowInGame = 11,
        LoadAreaCompleted = 12,
        LoadAreaStarted = 13,
        //SaveGame = 14,
        //LoadGame = 15,
        TriggerUIGoBack = 16,
        UpdateSaveEntry = 17,
        LoadVariablesFromSE = 18,
    }
}