using GameData;
using TMPro;
using WuLin;
using HaxxToyBox.Config;
using System;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class MiscPanel : MonoBehaviour
{
    public static MiscPanel Instance { get; private set; }

    //private Button _timeFreezeSwitch;
    private Switch _timeFreezeSwitch;
    private Switch _recoverSwitch;
    private Switch _noCombatSwitch;
    private Switch _relationSwitch;
    private Switch _enableAchieveSwitch;
    private Switch _ultimateMartialSwitch;

    private Slider _battleSpeedSlider;
    private Slider walkspeedSlider;
    private Slider gamespeedSlider;
    private TMP_InputField _coinInput;
    private TMP_InputField FTributePointInput;
    private TMP_InputField JianghuLilianInput;
    private TMP_InputField LingwudianInput;

    private Dropdown _packTypeDropDown;
    private Button _removeItems;
    public PackItemType _removeType;

    private InputKeyUGUI _toggleKeyUI;
    private InputKeyUGUI _speedUpKeyUI;
    private InputKeyUGUI _speedDownKeyUI;
    private InputKeyUGUI _recoverKeyUI;
    private InputKeyUGUI _refreshbountyKeyUI;
    private InputKeyUGUI _map1KeyUI;
    private InputKeyUGUI _map2KeyUI;
    private InputKeyUGUI _map3KeyUI;
    private InputKeyUGUI _map4KeyUI;
    private InputKeyUGUI _map5KeyUI;

    public int ExpMultiple = 1;
    public int _FTributePoint = 1;
    public int WalkSpeed = 1;
    public int BattleSpeed = 1;
    public int GameSpeed = 1;
    public static UIWorldMapManager myworld;
    public bool TimeFreezed => _timeFreezeSwitch.IsToggled();
    public bool RecoverEnabled => _recoverSwitch.IsToggled();
    public bool NoCombat => _noCombatSwitch.IsToggled();
    public bool RelationEnabled => _relationSwitch.IsToggled();
    public bool EnableAchieve => _enableAchieveSwitch.IsToggled();
    public bool UltimateMartial => _ultimateMartialSwitch.IsToggled();


    //public bool TimeFreezed = false;
    //public bool RecoverEnabled = false;
    //public bool NoCombat = false;
    //public bool RelationEnabled = false;
    //public bool EnableAchieve = false;
    //public bool UltimateMartial = false;
    public enum PackItemType
    {
        Equip,
        Martial,
        Consumables,
        Material,
        Map,
        Other,
        All,
    }

    public MiscPanel(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        Instance = this;

        _timeFreezeSwitch = transform.Find("SwitchFunc/TimeFreeze/Switch").gameObject.AddComponent<Switch>();
        _recoverSwitch = transform.Find("SwitchFunc/Recover/Switch").gameObject.AddComponent<Switch>();
        _noCombatSwitch = transform.Find("SwitchFunc/NoCombat/Switch").gameObject.AddComponent<Switch>();
        _relationSwitch = transform.Find("SwitchFunc/Friendship/Switch").gameObject.AddComponent<Switch>();
        _enableAchieveSwitch = transform.Find("SwitchFunc/EnableAchievement/Switch").gameObject.AddComponent<Switch>();
        _ultimateMartialSwitch = transform.Find("SwitchFunc/UltimateMartial/Switch").gameObject.AddComponent<Switch>();

        _removeType = PackItemType.All;

        _packTypeDropDown = transform.Find("DropDownFunc/RemovePackItems/PackTypeDropdown").gameObject.GetComponent<Dropdown>();
        _packTypeDropDown.onValueChanged.RemoveAllListeners();
        _packTypeDropDown.onValueChanged.AddListener((int index) => {
            if (index == 0)
            {
                index = 6;
            }
            else
            {
                index -= 1;
            }
            switch ((PackItemType)index)
            {
                case PackItemType.All:
                    ToyBox.LogMessage("全部");
                    _removeType = PackItemType.All;
                    break;
                case PackItemType.Equip:
                    ToyBox.LogMessage("装备");
                    _removeType = PackItemType.Equip;
                    break;
                case PackItemType.Martial:
                    ToyBox.LogMessage("秘籍");
                    _removeType = PackItemType.Martial;
                    break;
                case PackItemType.Consumables:
                    ToyBox.LogMessage("消耗品");
                    _removeType = PackItemType.Consumables;
                    break;
                case PackItemType.Material:
                    ToyBox.LogMessage("材料");
                    _removeType = PackItemType.Consumables;
                    break;
                case PackItemType.Map:
                    ToyBox.LogMessage("地图");
                    _removeType = PackItemType.Map;
                    break;
                case PackItemType.Other:
                    ToyBox.LogMessage("其他");
                    _removeType = PackItemType.Other;
                    break;
                default:
                    ToyBox.LogMessage("未知");
                    _removeType = PackItemType.All;
                    break;
            }

        });

        _removeItems = transform.Find("DropDownFunc/RemovePackItems/Remove").gameObject.GetComponent<Button>();
        _removeItems.onClick.RemoveAllListeners();
        _removeItems.onClick.AddListener(() =>
        {
            PlayerTeamManager instance = MonoSingleton<PlayerTeamManager>.Instance;
            if (instance != null)
            {
                switch (_removeType)
                {
                    case PackItemType.All:
                        foreach (PackItemType type in Enum.GetValues(typeof(PackItemType)))
                        {
                            if (type != PackItemType.All)
                            {
                                var pack_items_all = instance.GetItemsByPackTag((int)type, 0);
                                foreach (var item in pack_items_all)
                                {
                                    ToyBox.LogMessage(item.ItemName);
                                    ToyBox.LogMessage(item.Stack);
                                    instance.RemoveItem(item.m_templeteId, item.Stack);
                                }
                            }
                        }
                        break;
                    default:
                        var pack_items = instance.GetItemsByPackTag((int)_removeType, 0);
                        foreach (var item in pack_items)
                        {
                            ToyBox.LogMessage(item.ItemName);
                            ToyBox.LogMessage(item.Stack);
                            instance.RemoveItem(item.m_templeteId, item.Stack);
                        }
                        break;
                }
            }
        });

        //_timeFreezeSwitch = transform.Find("SwitchFunc/TimeFreeze/Switch").gameObject.GetComponent<Button>();
        //_recoverSwitch = transform.Find("SwitchFunc/Recover/Switch").gameObject.GetComponent<Button>();
        //_noCombatSwitch = transform.Find("SwitchFunc/NoCombat/Switch").gameObject.GetComponent<Button>();
        //_relationSwitch = transform.Find("SwitchFunc/Friendship/Switch").gameObject.GetComponent<Button>();
        //_enableAchieveSwitch = transform.Find("SwitchFunc/EnableAchievement/Switch").gameObject.GetComponent<Button>();
        //_ultimateMartialSwitch = transform.Find("SwitchFunc/UltimateMartial/Switch").gameObject.GetComponent<Button>();

        //_timeFreezeSwitch.onClick.AddListener(() => {
        //    TimeFreezed = !TimeFreezed;
        //    ToyBox.LogMessage("TimeFreezed:" + TimeFreezed);
        //});

        //_recoverSwitch.onClick.AddListener(() => {
        //    RecoverEnabled = !RecoverEnabled;
        //    ToyBox.LogMessage("RecoverEnabled:" + RecoverEnabled);
        //});

        //_noCombatSwitch.onClick.AddListener(() => {
        //    NoCombat = !NoCombat;
        //    ToyBox.LogMessage("NoCombat:" + NoCombat);
        //});

        //_relationSwitch.onClick.AddListener(() => {
        //    RelationEnabled = !RelationEnabled;
        //    ToyBox.LogMessage("RelationEnabled:" + RelationEnabled);
        //});

        //_enableAchieveSwitch.onClick.AddListener(() => {
        //    EnableAchieve = !EnableAchieve;
        //    ToyBox.LogMessage("EnableAchieve:" + EnableAchieve);
        //});

        //_ultimateMartialSwitch.onClick.AddListener(() =>
        //{
        //    UltimateMartial = !UltimateMartial;
        //    ToyBox.LogMessage("UltimateMartial:" + UltimateMartial);
        //});


        //var expInput = transform.Find("InputFunc/SkillExp/NumInput").GetComponent<TMP_InputField>();
        //expInput.onValueChanged.RemoveAllListeners();
        //expInput.onValueChanged.AddListener((string input) => {
        //    int.TryParse(input, out ExpMultiple);
        //    ExpMultiple = Mathf.Clamp(ExpMultiple, 1, 1000);
        //});

        FTributePointInput = transform.Find("InputFunc/Contribute/NumInput").GetComponent<TMP_InputField>();
        FTributePointInput.onValueChanged.RemoveAllListeners();
        FTributePointInput.onValueChanged.AddListener((string input) => {
            if (!long.TryParse(input, out long _FTributePoint))
                FTributePointInput.text = FTributePointInput.m_OriginalText;
            else
            {
                var teamInventory = MonoSingleton<PlayerTeamManager>.Instance.TeamInventory;
                teamInventory.SetCurrency(CurrencyType.FTributePoint, _FTributePoint);
            }
        });

        _coinInput = transform.Find("InputFunc/Gold/NumInput").GetComponent<TMP_InputField>();
        _coinInput.onValueChanged.RemoveAllListeners();
        _coinInput.onValueChanged.AddListener((string input) => {
            if (!long.TryParse(input, out long value))
                _coinInput.text = _coinInput.m_OriginalText;
            else {
                var inventory = MonoSingleton<PlayerTeamManager>.Instance.TeamInventory;
                inventory.SetCurrency(CurrencyType.Coin, value * 1000);
            }
        });

        JianghuLilianInput = transform.Find("InputFunc/jianghulilian/NumInput").GetComponent<TMP_InputField>();
        JianghuLilianInput.onValueChanged.RemoveAllListeners();
        JianghuLilianInput.onValueChanged.AddListener((string input) =>
        {
            var Character = PlayerTeamManager.Instance.GetTeamMemberByIndex(0);
            if (Character == null ||!Il2CppSystem.Decimal.TryParse(input, out Il2CppSystem.Decimal value))
            {
                JianghuLilianInput.text = JianghuLilianInput.m_OriginalText;
                return;
            }

            if (!Character.m_originProps.ContainsKey("江湖历练"))
                Character.m_originProps.Add("江湖历练", 0);

            var diff = value - Character.m_originProps["江湖历练"];
            Character.ChangeOriginProp("江湖历练", diff);

        });

        LingwudianInput = transform.Find("InputFunc/lingwudian/NumInput").GetComponent<TMP_InputField>();
        LingwudianInput.onValueChanged.RemoveAllListeners();
        LingwudianInput.onValueChanged.AddListener((string input) =>
        {
            var Character = PlayerTeamManager.Instance.GetTeamMemberByIndex(0);
            if (Character == null || !Il2CppSystem.Decimal.TryParse(input, out Il2CppSystem.Decimal value))
            {
                LingwudianInput.text = LingwudianInput.m_OriginalText;
                return;
            }

            if (!Character.m_originProps.ContainsKey("冲穴点数"))
                Character.m_originProps.Add("冲穴点数", 0);

            var diff = value - Character.m_originProps["冲穴点数"];
            Character.ChangeOriginProp("冲穴点数", diff);
        });


        walkspeedSlider = transform.Find("SliderFunc/WalkSpeed/Slider").GetComponent<Slider>();
        //Text text_WalkSpeed = walkspeedSlider.Find("Text").gameObject.GetComponent<Text>();
        walkspeedSlider.transform.Find("Text").gameObject.AddComponent<SliderAmountText>();
        walkspeedSlider.onValueChanged.AddListener((float value) => {
            WalkSpeed = (int)value;
            //var player = RoamingManager.Instance?.player;
            //if (player == null) return;

            //if (!player.SpeedKey.ContainsKey("toybox")) {
            //    player.SpeedKey.Add("toybox", value);
            //}
            //else {
            //    player.SpeedKey["toybox"] = value;
            //}
        });

        _battleSpeedSlider = transform.Find("SliderFunc/BattleSpeed/Slider").GetComponent<Slider>();
        _battleSpeedSlider.transform.Find("Text").gameObject.AddComponent<SliderAmountText>();
        _battleSpeedSlider.onValueChanged.AddListener((float value) =>
        {
            GameTimer.Instance.AddOrSetTimeScale(this, value);
            BattleSpeed = (int)value;
        });

        //gamespeedSlider = transform.Find("SliderFunc/GameSpeed/Slider").GetComponent<Slider>();
        //gamespeedSlider.transform.Find("Text").gameObject.AddComponent<SliderAmountText>();
        //gamespeedSlider.onValueChanged.AddListener((float value) => {
        //    Time.timeScale = value;
        //    GameSpeed = (int)value;
        //});

        var buttonAchievements = transform.Find("ButtonFunc/Achievement").gameObject;
        buttonAchievements.AddComponent<FadeButtonWrapper>();
        buttonAchievements.GetComponent<Button>().onClick.AddListener(() => {
            var achievementDB = BaseDataClass.GetGameData<AchievementDataScriptObject>().data;
            foreach (var id in achievementDB.Keys) {
                MonoSingleton<AchievementManager>.Instance.Complate(id);
            }
        });

        var buttonRecover = transform.Find("ButtonFunc/Recover").gameObject;
        buttonRecover.AddComponent<FadeButtonWrapper>();
        buttonRecover.GetComponent<Button>().onClick.AddListener(UnlockAllMap);

        var buttonLearnAllRecipe = transform.Find("ButtonFunc/LearnAllRecipe").gameObject;
        buttonLearnAllRecipe.AddComponent<FadeButtonWrapper>();
        buttonLearnAllRecipe.GetComponent<Button>().onClick.AddListener(UnlockAllRecipe);

        _toggleKeyUI = transform.Find("ConfigFunc/PanelToggle").gameObject.AddComponent<InputKeyUGUI>();
        _speedUpKeyUI = transform.Find("ConfigFunc/SpeedupToggle").gameObject.AddComponent<InputKeyUGUI>();
        _speedDownKeyUI = transform.Find("ConfigFunc/SpeeddownToggle").gameObject.AddComponent<InputKeyUGUI>();
        _recoverKeyUI = transform.Find("ConfigFunc/Recover").gameObject.AddComponent<InputKeyUGUI>();
        _refreshbountyKeyUI = transform.Find("ConfigFunc/RefreshBounty").gameObject.AddComponent<InputKeyUGUI>();
        _map1KeyUI = transform.Find("ConfigFunc/Map1").gameObject.AddComponent<InputKeyUGUI>();
        _map2KeyUI = transform.Find("ConfigFunc/Map2").gameObject.AddComponent<InputKeyUGUI>();
        _map3KeyUI = transform.Find("ConfigFunc/Map3").gameObject.AddComponent<InputKeyUGUI>();
        _map4KeyUI = transform.Find("ConfigFunc/Map4").gameObject.AddComponent<InputKeyUGUI>();
        _map5KeyUI = transform.Find("ConfigFunc/Map5").gameObject.AddComponent<InputKeyUGUI>();

        BindInputKey(_toggleKeyUI, ConfigManager.Canvas_Toggle);
        BindInputKey(_speedUpKeyUI, ConfigManager.SpeedUp_Toggle);
        BindInputKey(_speedDownKeyUI, ConfigManager.SpeedDown_Toggle);
        BindInputKey(_recoverKeyUI, ConfigManager.Recover_Toggle);
        BindInputKey(_refreshbountyKeyUI, ConfigManager.RefreshBounty_Toggle);
        BindInputKey(_map1KeyUI, ConfigManager.Map0);
        BindInputKey(_map2KeyUI, ConfigManager.Map1);
        BindInputKey(_map3KeyUI, ConfigManager.Map2);
        BindInputKey(_map4KeyUI, ConfigManager.Map3);
        BindInputKey(_map5KeyUI, ConfigManager.Map4);
    }

    private void BindInputKey(InputKeyUGUI obj, ConfigElement config)
    {
        obj.Key = config.Value;
        obj.AllowAbortWithCancelButton = true;
        obj.OnChanged += (KeyCode key, KeyCode modifierKey) => config.Value = key;
    }

    private void OnEnable()
    {
        var inventory = PlayerTeamManager.Instance?.TeamInventory;
        var propSource = GameCharacterInstance.FinalPropSource.Origin;
        string formatstr = "";
        if (inventory != null) {
            _coinInput.SetTextWithoutNotify((inventory.GetCurrency(CurrencyType.Coin) / 1000).ToString());
            FTributePointInput.SetTextWithoutNotify((inventory.GetCurrency(CurrencyType.FTributePoint)).ToString());
        }
        var Character = PlayerTeamManager.Instance.GetTeamMemberByIndex(0);
        JianghuLilianInput.SetTextWithoutNotify(Character.GetFinalPropAsDecimal("江湖历练", propSource).ToString(formatstr));
        LingwudianInput.SetTextWithoutNotify(Character.GetFinalPropAsDecimal("冲穴点数", propSource).ToString(formatstr));
        
        _battleSpeedSlider.value = BattleSpeed;
        walkspeedSlider.value = WalkSpeed;
        //gamespeedSlider.value = GameSpeed;
    }

    public static void RecoverAll()
    {
        var teamManager = PlayerTeamManager.Instance;
        if (teamManager == null) return;
        teamManager.ModifyProp("队伍体力", 100);
        teamManager.ModifyProp("队伍心情", 100);
        for (int i = 0; i < teamManager.TeamSize; i++) {
            teamManager.GetTeamMemberByIndex(i).FullyRecover();
        }
    }

    public static void UnlockAllMap()
    {
        int WorldMapID = 1000;
        Transform UI_MAP_Transform = ToyBoxBehaviour.myworld.transform;
        for (int i = 0; i < ToyBoxBehaviour.myworld.transform.childCount; i++)
        {
            if (UI_MAP_Transform.GetChild(i).name.Contains("Map"))
            {
                for (int j = 0; j < UI_MAP_Transform.GetChild(i).GetComponent<UIWorldMap>().CityIcon.childCount; j++)
                {
                    ToyBoxBehaviour.myworld.SetCityVisible(WorldMapID, int.Parse(UI_MAP_Transform.GetChild(i).GetComponent<UIWorldMap>().CityIcon.GetChild(j).name), true, true);
                }
                WorldMapID++;
            }

        }
    }
    public static void UnlockAllRecipe()
    {
        var RecipeData = BaseDataClass.GetGameData<NewRecipeDataScriptObject>().NewRecipeData;
        foreach (var recipe in RecipeData)
        {
            ToyBox.LogMessage(recipe.Uid);
            ToyBox.LogMessage(recipe.UName);
            PlayerTeamManager.Instance.LearnRecipe(recipe.Uid);
        }
    }
    public static void SpeedDown()
    {
        if (Instance == null) return;
        int min = (int)Instance._battleSpeedSlider.minValue;
        Instance.BattleSpeed = Math.Max(Instance.BattleSpeed-1, min);

        GameTimer.Instance.AddOrSetTimeScale(Instance, Instance.BattleSpeed);
    }

    public static void SpeedUp()
    {
        if (Instance == null) return;
        int max = (int)Instance._battleSpeedSlider.maxValue;
        Instance.BattleSpeed = Math.Min(Instance.BattleSpeed+1, max);

        GameTimer.Instance.AddOrSetTimeScale(Instance, Instance.BattleSpeed);
    }

}
