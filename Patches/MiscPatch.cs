using HarmonyLib;
using TMPro;
using WuLin;
using HaxxToyBox.GUI;
using static WuLin.GameCharacterInstance;
using WuLin.GameFrameworks;
using HaxxToyBox.Config;
using System.Runtime.CompilerServices;

using Decimal = Il2CppSystem.Decimal;
using Object = UnityEngine.Object;
using Transform = UnityEngine.Transform;

namespace HaxxToyBox.Patches;

public class MiscPatch
{
    

    public static bool isInitMap = false;
    public static bool isInitAbilityButton = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DlcButton), "OnClick")]
    public static bool OnClick_PrePatch()
    {
        ToyBoxBehaviour.Instance.rolePanel.SetActive(false);
        ToyBoxBehaviour.Instance.buttonBot.gameObject.SetActive(false);

        ToyBoxBehaviour.Instance.dlcPanel.SetActive(true);
        ToyBoxBehaviour.Instance.buttonBotDLC.gameObject.SetActive(true);
        return true;
    }
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(GameCharacterInstance), "CouldLearnKungfu")]
    //public static bool CouldLearnKungfu_PrePatch(ref GameCharacterInstance.EquipingCheckResult __result)
    //{
    //    __result = GameCharacterInstance.EquipingCheckResult.Ok;
    //    return false;
    //}
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SaveObjectGameTime), "AddDeltaTime")]
    public static bool AddDeltaTime_PrePatch()
    {
        if (!MiscPanel.Instance) return true;

        return !MiscPanel.Instance.TimeFreezed;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(BattleManager), "LeaveBattle")]
    public static void LeaveBattle_PrePatch()
    {
        if (MiscPanel.Instance && MiscPanel.Instance.RecoverEnabled) {
            PlayerTeamManager.Instance.PlayerDataInstance.FullyRecover();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(RoamingManager), "GetNpcBySightPoint")]
    public static void GetNpcBySightPoint_PostPatch(ref Il2CppSystem.Collections.Generic.List<Npc> __result)
    {
        if (MiscPanel.Instance && MiscPanel.Instance.NoCombat) {
            __result.Clear();
        }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StealthManager), "GetPerceptionSpeed")]
    public static void GetPerceptionSpeed_PostPatch(ref float __result)
    {
        if (MiscPanel.Instance && MiscPanel.Instance.NoCombat) {
            __result = 0;
        }
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StealthPerceptComponent), "OnFound")]
    public static bool OnFound_PrePatch()
    {
        if (!MiscPanel.Instance) return true;

        return !MiscPanel.Instance.NoCombat;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameCharacterInstance), "ChangeAdditionProp")]
    public static bool ChangeAdditionProp_PrePatch(string key, ref Il2CppSystem.Decimal value)
    {
        if (MiscPanel.Instance && key.Contains("能力经验_")) {
            value *= MiscPanel.Instance.ExpMultiple;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GiftingWithNpcUI), "Awake")]
    public static void GiftingWithNpcUIAwake_PostPatch(GiftingWithNpcUI __instance)
    {
        var buttonTemp = UiSingletonPrefab<EscUI>.Instance.main_Resume.gameObject;

        var button = UnityEngine.Object.Instantiate(buttonTemp, __instance.transform, false);
        button.name = "IncRelation";
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 40);
        button.GetComponent<RectTransform>().localPosition = new Vector3(-10, 120, 0);
        button.GetComponentInChildren<TextMeshProUGUI>().text = "满好感";
        button.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
        button.GetComponent<Button>().onClick.RemoveAllListeners();
        button.GetComponent<Button>().onClick.AddListener(delegate {
            var source = GameCharacterInstance.RelationModifySource.Gift;
            GiftingWithNpcManager.npc?.ModifyRelationWithPlayer(100, source);
        });
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GiftingWithNpcUI), "Update")]
    public static bool GiftingWithNpcUIUpdate_PrePatch(GiftingWithNpcUI __instance)
    {
        if (!MiscPanel.Instance) return true;

        var button = __instance.transform.Find("IncRelation");
        button?.gameObject.SetActive(MiscPanel.Instance.RelationEnabled);
        return true;
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(Role), "UpdateSpeed")]
    public static void RoleUpdateSpeed_PostPatch(Role __instance)
    {
        if (MiscPanel.Instance && __instance == RoamingManager.Instance.player) {
            __instance.speed *= MiscPanel.Instance.WalkSpeed;
        }
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), "IsAnyModActivedInHistroy", MethodType.Getter)]
    public static void IsAnyModActivedInHistroy_PostPatch(ref bool __result)
    {
        if (MiscPanel.Instance && MiscPanel.Instance.EnableAchieve) {
            __result = false;
        }
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(InGameTimeManager), "get_TotalQuaterPassedAsInt")]
    public static bool get_TotalQuaterPassedAsInt_PrePatch(ref int __result)
    {
        if (!ConfigManager.isHook)
        {
            __result = 0x9999999;
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RoamingUI), "Show")]
    public static bool Show_PrePatch(ref bool isInstant)
    {
        ToyBoxBehaviour.Instance.rolePanel.SetActive(true);
        ToyBoxBehaviour.Instance.buttonBot.gameObject.SetActive(true);

        ToyBoxBehaviour.Instance.dlcPanel.SetActive(false);
        ToyBoxBehaviour.Instance.buttonBotDLC.gameObject.SetActive(false);

        #region 初始化地图
        ToyBox.LogMessage("RoamingUI.Show()");
        if (!isInitMap)
        {
            UIWorldMapManager.Instance.SetCurMap(1000);

            ToyBoxBehaviour.myworld = Object.Instantiate<GameObject>(UIWorldMapManager.Instance.gameObject, UIWorldMapManager.Instance.transform.GetParent(), false).GetComponent<UIWorldMapManager>();

            ToyBoxBehaviour.myworld.curMapUI = UIWorldMapManager.Instance.curMapUI;
            //TrainerComponent.myworld.curMapUI = new UIWorldMap();
            ToyBoxBehaviour.myworld.isTimeStopper = UIWorldMapManager.Instance.isTimeStopper;
            ToyBoxBehaviour.myworld.isInGameTimeStopper = UIWorldMapManager.Instance.isInGameTimeStopper;
            ToyBoxBehaviour.myworld.ignoreBackToMenu = UIWorldMapManager.Instance.ignoreBackToMenu;
            ToyBoxBehaviour.myworld.autoHideOnStoryIn = UIWorldMapManager.Instance.autoHideOnStoryIn;
            ToyBoxBehaviour.myworld.serializationData = UIWorldMapManager.Instance.serializationData;
            ToyBoxBehaviour.myworld.uiBlockers = UIWorldMapManager.Instance.uiBlockers;
            ToyBoxBehaviour.myworld.cachedCGHelper = UIWorldMapManager.Instance.cachedCGHelper;
            ToyBoxBehaviour.myworld.consoleMsg = UIWorldMapManager.Instance.consoleMsg;
            ToyBoxBehaviour.myworld.saveMarkList = UIWorldMapManager.Instance.saveMarkList;
            ToyBoxBehaviour.myworld.listWorldMap = UIWorldMapManager.Instance.listWorldMap;
            ToyBoxBehaviour.myworld.autoClearLeftLogs = UIWorldMapManager.Instance.autoClearLeftLogs;
            ToyBoxBehaviour.myworld.mIsBlocking = UIWorldMapManager.Instance.mIsBlocking;
            ToyBoxBehaviour.myworld.isLock = UIWorldMapManager.Instance.isLock;
            ToyBoxBehaviour.myworld.SetCurMap(1000);
            ToyBox.LogMessage("地图初始化成功");
            isInitMap = true;
        }
        #endregion
        if (!isInitAbilityButton)
        {
            #region 能力_采集_采矿
            Transform RoleTransform = UiSingletonPrefab<UIMenuPanel>.Instance.transform;
            Transform AddButton = RoleTransform.Find("UIRoleMenuPanel/LeftPanel/ZzPointEditGroup/ZzPointEditItems/ZzPointEditItem_1/Add");
            Transform SubButton = RoleTransform.Find("UIRoleMenuPanel/LeftPanel/ZzPointEditGroup/ZzPointEditItems/ZzPointEditItem_1/Sub");
            Transform Ability = RoleTransform.Find("UIAbility/up/content/能力_采集_采矿");

            GameObject AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            GameObject AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;

            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());


            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);
            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                //if (TrainerComponent.MainPlayer.GetAbilityLevelUpCost("能力_采集_采矿") == 0)
                //{
                //    AbilityAddButton.GetComponent<Button>().interactable = false;
                //    ToyBox.ToyBoxMessage("err add");
                //    return;
                //}
                //AbilitySubButton.GetComponent<Button>().interactable = true;
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_采矿", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                //if (TrainerComponent.MainPlayer.GetAbilityLevelUpCost("能力_采集_采矿") == 100)
                //{
                //    AbilitySubButton.GetComponent<Button>().interactable = false;
                //    ToyBox.ToyBoxMessage("err sub");
                //    return;
                //}
                //AbilityAddButton.GetComponent<Button>().interactable = true;

                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_采矿", -1);
                ToyBox.LogMessage("sub");
            });
            #endregion

            #region 能力_采集_伐木

            Ability = RoleTransform.Find("UIAbility/up/content/能力_采集_伐木");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_伐木", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_伐木", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_采集_采药

            Ability = RoleTransform.Find("UIAbility/up/content/能力_采集_采药");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_采药", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_采药", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_采集_钓鱼

            Ability = RoleTransform.Find("UIAbility/up/content/能力_采集_钓鱼");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_钓鱼", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_钓鱼", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_采集_捉虫

            Ability = RoleTransform.Find("UIAbility/up/content/能力_采集_捉虫");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_捉虫", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_采集_捉虫", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_制造_兵器

            Ability = RoleTransform.Find("UIAbility/up/content/能力_制造_兵器");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_兵器", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_兵器", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_制造_护甲

            Ability = RoleTransform.Find("UIAbility/up/content/能力_制造_护甲");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_护甲", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_护甲", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_制造_食物

            Ability = RoleTransform.Find("UIAbility/up/content/能力_制造_食物");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_食物", 1);
                ToyBox.LogMessage("能力_制造_食物 add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_食物", -1);
                ToyBox.LogMessage("能力_制造_食物 sub");
            });

            #endregion

            #region 能力_制造_丹药

            Ability = RoleTransform.Find("UIAbility/up/content/能力_制造_丹药");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_丹药", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_丹药", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_制造_合成

            Ability = RoleTransform.Find("UIAbility/up/content/能力_制造_合成");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_合成", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_制造_合成", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_鉴定_字画

            Ability = RoleTransform.Find("UIAbility/up/content/能力_鉴定_字画");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_字画", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_字画", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_鉴定_书籍

            Ability = RoleTransform.Find("UIAbility/up/content/能力_鉴定_书籍");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_书籍", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_书籍", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_鉴定_古董

            Ability = RoleTransform.Find("UIAbility/up/content/能力_鉴定_古董");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_古董", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_古董", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_鉴定_饰品

            Ability = RoleTransform.Find("UIAbility/up/content/能力_鉴定_饰品");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_饰品", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_饰品", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_鉴定_丹药

            Ability = RoleTransform.Find("UIAbility/up/content/能力_鉴定_丹药");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_丹药", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_鉴定_丹药", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_生存_口才

            Ability = RoleTransform.Find("UIAbility/up/content/能力_生存_口才");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_口才", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_口才", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_生存_经商

            Ability = RoleTransform.Find("UIAbility/up/content/能力_生存_经商");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_经商", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_经商", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_生存_探索

            Ability = RoleTransform.Find("UIAbility/up/content/能力_生存_探索");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_探索", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_探索", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_生存_识图

            Ability = RoleTransform.Find("UIAbility/up/content/能力_生存_识图");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_识图", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_识图", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_生存_驯兽

            Ability = RoleTransform.Find("UIAbility/up/content/能力_生存_驯兽");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_驯兽", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_生存_驯兽", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_市井_暗取

            Ability = RoleTransform.Find("UIAbility/up/content/能力_市井_暗取");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_暗取", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_暗取", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_市井_点穴

            Ability = RoleTransform.Find("UIAbility/up/content/能力_市井_点穴");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_点穴", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_点穴", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_市井_下毒

            Ability = RoleTransform.Find("UIAbility/up/content/能力_市井_下毒");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_下毒", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_下毒", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_市井_机关

            Ability = RoleTransform.Find("UIAbility/up/content/能力_市井_机关");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_机关", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_机关", -1);
                ToyBox.LogMessage("sub");
            });

            #endregion

            #region 能力_市井_突袭

            Ability = RoleTransform.Find("UIAbility/up/content/能力_市井_突袭");
            AbilityAddButton = Object.Instantiate<Transform>(AddButton, Ability, false).gameObject;
            AbilitySubButton = Object.Instantiate<Transform>(SubButton, Ability, false).gameObject;
            Object.DestroyImmediate(AbilityAddButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilityAddButton.GetComponent<HotKeyBinder>());

            Object.DestroyImmediate(AbilitySubButton.GetComponent<Button>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<EventTriggerDelegate>());
            Object.DestroyImmediate(AbilitySubButton.GetComponent<HotKeyBinder>());

            AbilityAddButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, 30f, 0f);
            AbilitySubButton.GetComponent<RectTransform>().localPosition = new Vector3(100f, -30f, 0f);

            AbilityAddButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_突袭", 1);
                ToyBox.LogMessage("add");
            });
            AbilitySubButton.AddComponent<Button>().onClick.AddListener(delegate ()
            {
                RoamingManager.Instance.player.gameCharacterInstance.ChangeAdditionPropWithLog("能力_市井_突袭", -1);
                ToyBox.LogMessage("sub");
            });
            isInitAbilityButton = true;
        }
        #endregion




        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UIWorldMap), "OnClickMove", new System.Type[] { typeof(Transform), typeof(bool), typeof(Vector2) })]
    public static bool OnClickMove_PrePatch(Transform clickTag, ref bool jump)
    {
        if (ToyBoxBehaviour.WorldMap || ToyBoxBehaviour.WorldMap1 || ToyBoxBehaviour.WorldMap2 || ToyBoxBehaviour.WorldMap3 || ToyBoxBehaviour.WorldMap4 || ToyBoxBehaviour.WorldMap5)
        {
            jump = true;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UIWorldMapManager), "OnClickEnter")]
    public static void OnClickEnter_PrePatch()
    {
        ToyBox.LogMessage("OnClickEnter");
        ToyBox.LogMessage("OnClickEnter");
        ToyBox.LogMessage("OnClickEnter");
        ToyBox.LogMessage("OnClickEnter");

        ToyBoxBehaviour.myworld.Hide(false);
        ToyBoxBehaviour.WorldMap = false;
        ToyBoxBehaviour.WorldMap1 = false;
        ToyBoxBehaviour.WorldMap2 = false;
        ToyBoxBehaviour.WorldMap3 = false;
        ToyBoxBehaviour.WorldMap4 = false;
        ToyBoxBehaviour.WorldMap5 = false;

    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuUI), "Show")]
    public static void ShowMainMenu_PrePatch(ref bool isInstant)
    {
        ToyBox.LogMessage("初始化修改器更新日志！！！！！！！！！");
        GameObject MyWindow = Object.Instantiate<GameObject>(MainMenuUI.Instance.annonceButton, MainMenuUI.Instance.annonceButton.transform.GetParent(), false);

        MyWindow.GetComponent<RectTransform>().localPosition = new Vector3(480f, 480f, 0f);
        MyWindow.GetComponentInChildren<TextMeshProUGUI>().text = "修改器";
        MyWindow.GetComponent<Button>().onClick.RemoveAllListeners();
        MyWindow.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            string UpInfo = "交流群：760709686  By x1a0reN\r\nLeftControl+F8   呼出控制台（控制台命令在压缩包内）\r\n\r\n2025/8/7\r\nv3.7.5 版本日志\r\n【声明】\r\n修改器暂时不会添加新功能，只会维持基本更新，后面就不加更新日志啦，谢谢大家支持\r\n\r\n----------------------------------------------------\r\n\r\n2025/3/15\r\nv3.7.1 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2025/2/19\r\nv3.7.0 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2025/1/22\r\nv3.6.9 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/12/23\r\nv3.6.8 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/11/30\r\nv3.6.7 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n2、删除了默认自带的无条件学习武学\r\n\r\n----------------------------------------------------\r\n\r\n2024/11/23\r\nv3.6.6 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/8/25\r\nv3.6.5 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/8/15\r\nv3.6.4 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n2、修复无限武学多了不生效的问题\r\n3、以往版本所有BUG在这个新版本基本都得到解决，出现问题无脑更新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/7/8\r\nv3.6.2 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\nps：最近有点懒，等13号回家之后再大更\r\n\r\n----------------------------------------------------\r\n\r\n2024/6/28\r\nv3.6.1 版本日志\r\n【修复】\r\n1、添加物品中分类消失问题\r\n\r\n----------------------------------------------------\r\n\r\n2024/6/27\r\nv3.6.0 版本日志\r\n【增加】\r\n1、新增DLC修改模块\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n2、杂项功能中贡献显示错误问题\r\n\r\n----------------------------------------------------\r\n\r\n\r\n2024/4/15\r\nv3.5.1 版本日志\r\n【修复】\r\n1、适配最新版本游戏\r\n2、修复呼出快捷地图关闭时进入大地图会自动瞬移的问题\r\n\r\n----------------------------------------------------\r\n\r\n2024/3/17\r\nv3.5.0 版本日志\r\n\r\n【增加】\r\n1、“人物属性”面板增加“反击”\r\n2、“人物属性”面板增加“领悟点”\r\n\r\n@贝希摩斯(1185838120)提出\r\n3、“杂项功能”面板增加“解锁全部配方”\r\n\r\n@君.(2207229615) 提出\r\n4、增加“添加Buff”面板，可以添加游戏中除开局外所有的后天Buff(包括MOD中的Buff，有些游戏自带的不生效)\r\n【修复】\r\n1、适配最新版本游戏\r\n2、修复在“物品添加”→“装备”→“全部”下搜索宠物装备搜索不到的问题\r\n3、修复在各种搜索中切换类型时不自动搜索的问题\r\n\r\n----------------------------------------------------\r\n\r\n2024/2/26\r\nv3.4.1 版本日志\r\n\r\n【增加】\r\n1、增加背包物品快捷清除\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2024/2/9\r\nv3.4 版本日志\r\n\r\n【增加】\r\n1、增加宠物装备\r\n2、增加新地图“西域地区”（新地图呼出热键为F8，修改请去“Steam\\steamapps\\common\\WulinSH\\BepInEx\\config\\HaxxToyBox.cfg”下修改）\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2024/2/9\r\nv3.3.2 版本日志\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2024/1/12\r\nv3.3.1 版本日志\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/20\r\nv3.3.0 版本日志\r\n\r\n【增加】\r\n1、修复小bug、适配游戏\r\n2、增加NPC添加界面\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/16\r\nv3.2.0 版本日志\r\n\r\n【增加】\r\n1、适配游戏\r\n2、增加解锁全部地图\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/11\r\nv3.1.0 版本日志\r\n\r\n【增加】\r\n1、UI界面适配多分辨率\r\n2、增加了江湖历练、领悟点的修改\r\n3、增加装备添加时属性最高\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/8\r\nv3.0.0 版本日志\r\n\r\n【增加】\r\n1、重新绘制UI，解决卡顿问题\r\n2、增加了天赋搜索、物品搜索、武学搜索\r\n3、增加了热键呼出地图\r\n4、增加了刷新任务栏\r\n5、增加了能力等级随意调整\r\n\r\n【说明】\r\n1、无限武学目前存在bug 等待下次版本更新\r\n2、NPC添加功能 等待下次版本更新\r\n\r\n----------------------------------------------------\r\n\r\n2023/11/17\r\nv2.6.1 版本日志\r\n\r\n【修复】\r\n1、修复WASD无法移动问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/11/17\r\nv2.6.0 版本日志\r\n\r\n【增加】\r\n1、增加了手动呼出控制台功能\r\n\r\n【修复】\r\n1、适配了V1.0正式版本\r\n\r\n【调整】\r\n1、人物属性“实战能力”改为“反击”\r\n2、暂时关闭“武学”界面，过段时间恢复\r\n3、因为游戏内部UI更新，修改器的初始化也改为了在进入游戏后初始化，所以第一次TAB呼出可能会卡顿几秒，之后就好了\r\n\r\nBTW:这段时间在写别的作业所以比较忙，新版本大更新对UI、功能做了很多调整，所以我需要重新去整理和逆向，时间有限请多包涵！（WBG夺冠直接开肝）\r\n\r\n----------------------------------------------------\r\n\r\n2023/10/26\r\nv2.5.1 版本日志\r\n\r\n【增加】\r\n1、增加了能力界面编辑等级功能\r\n2、增加了战斗中可随时撤退功能（必须要等第一次集气结束才可以撤退，不然会出现异常）\r\n\r\n【修复】\r\n1、修复了添加NPC再离队会出现异常问题\r\n\r\n【调整】\r\n1、取消了五倍能力经验\r\n\r\n----------------------------------------------------\r\n\r\n2023/10/19\r\nv2.4.3 版本日志\r\n\r\n【调整】\r\n1、适配了 V0.9.1017b47 版本\r\n2、优化了如果没有触发NPC剧情就添加名字不能正常显示问题\r\n3、取消了添加物品中数量自动记忆（现在打开关闭会重置为1）\r\n\r\n----------------------------------------------------\r\n\r\n2023/10/16\r\nv2.4.2 版本日志\r\n\r\n【增加】\r\n1、增加识别MOD中的天赋、武学、物品数据\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/30\r\nv2.4.1 版本日志\r\n\r\n【增加】\r\n1、增加添加NPC界面以及入队功能（部分NPC无法添加或添加有问题）\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/27\r\nv2.3.5 版本日志\r\n\r\n【增加】\r\n1、增加添加物品搜索功能\r\n\r\n【调整】\r\n1、优化添加物品界面显示逻辑\r\n2、输入数量时不再需要回车确认\r\n3、打开时记忆上次浏览物品分类、子分类、第几页\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/27\r\nv2.3.4 版本日志\r\n\r\n【修复】\r\n1、适配了 V0.9.0926b46 版本\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/26\r\nv2.3.3 版本日志\r\n【增加】\r\n1、优化添加物品分类\r\n2、添加装备时所有附加属性全部完美\r\n\r\n【修复】\r\n1、修复金色饰品无法添加BUG\r\n2、修复刷新任务热键与西南地区地图热键冲突BUG（还是不行的请先删除原来的配置文件）\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/25\r\nv2.3.2 版本日志\r\n【增加】\r\n1、添加装备时自动添加满级锻造属性\r\n\r\n【修复】\r\n1、修复武学功能自动添加兰亭序的BUG\r\n2、修复刷新任务的BUG\r\n\r\n【调整】\r\n1、武学功能恢复\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/23\r\nv2.3.1 版本日志\r\n\r\n【修复】\r\n1、适配了 V0.9.0923b45 版本\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/23\r\nv2.3 版本日志\r\n【增加】\r\n1、增加了西南地图\r\n\r\n【修复】\r\n1、适配了 V0.9.0922b44 版本\r\n\r\n【调整】\r\n1、武学功能暂时关闭(终于理解haxx为什么不想更新 －_－|| )\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/11\r\nv2.2.1 版本日志\r\n【修复】\r\n1、适配了 V0.8.0909b42 版本\r\n2、修复一些bug\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/7\r\nv2.2 版本日志\r\n【新增】\r\n1、地图传送(重大更新)\r\n2、门派贡献编辑\r\n3、一键解锁所有地图\r\n4、功能热键修改\r\n\r\n【优化】\r\n1、人物速度调节按钮遮挡问题\r\n\r\n【修复】\r\n1、人物速度调节在进入地图时会失控\r\nTips:建议呼出地图界面时配合大地图瞬移一块使用\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/4\r\nv2.1 版本日志\r\n【新增】\r\n1、武学无上限(可手动学习武学)\r\n2、在游玩MOD情况下也可以解锁成就\r\n3、使用物品无限制/装备无Debuff\r\n\r\n【优化】\r\n1、增加武学非常卡顿问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/3\r\nv2.0 版本日志\r\n【新增】\r\n1、自己和队友增加武学功能\r\n2、突破武学上限\r\n3、人物移动速度 x2/x4/x6/x8\r\n4、战斗倍速 x6/x8\r\n5、自动解锁自由模式\r\n6、任务无限刷新\r\n7、更换框架，之前的功能重新加\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/1\r\nv1.3 版本日志\r\n\r\n【新增】\r\n1、武学可以一键学习(可以无限学习，但最多只能上阵15个)\r\n2、武学上限变为15（战斗界面最多可以加载15个）\r\n\r\n【优化】\r\n1、修改器界面\r\n2、武学上限变为15（战斗界面最多可以加载15个）\r\n\r\n【修复】\r\n1、门派之外的任务无法刷新问题\r\n2、修改铜钱也会修改门派贡献问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/8/31\r\nv1.2 版本日志\r\n\r\n【新增】\r\n1、一键恢复内力、生命\r\n2、门派贡献编辑\r\n\r\n【优化】\r\n1、战斗后自动回复（现在可以回满生命、内力、体力、心情）\r\n\r\n【修复】\r\n1、角色无法WASD走路问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/8/31\r\nv1.1 版本日志\r\n\r\n【新增】\r\n1、解锁全成就\r\n2、任务无限刷新（已接取不会刷新，若想刷新，请先放弃）\r\n3、一键恢复体力、心情\r\n4、战斗时倍速最大为4\r\n\r\n【优化】\r\n1、战斗后自动回复（现在可以回满生命、内力、体力、心情）\r\n\r\n【修复】\r\n1、人物速度在进入新地图时会重置\r\n\r\n\r\n----------------------------------------------------\r\n\r\n\r\n2023/8/30\r\nv1.0版本日志\r\n\r\n【新增】\r\n1、物品添加\r\n2、天赋编辑\r\n3、属性编辑\r\n4、杂项等";
            GameInfoUI.Instance.SetButtonsGroupActive(false);
            GameInfoUI.Instance.ShowWindow(true, "关于本修改器  Version:3.7.5", UpInfo);
        });

        if (ToyBoxBehaviour.isNewVersion)
        {
            string UpInfo = "交流群：760709686  By x1a0reN\r\nLeftControl+F8   呼出控制台（控制台命令在压缩包内）\r\n\r\n2025/8/7\r\nv3.7.5 版本日志\r\n【声明】\r\n修改器暂时不会添加新功能，只会维持基本更新，后面就不加更新日志啦，谢谢大家支持\r\n\r\n----------------------------------------------------\r\n\r\n2025/3/15\r\nv3.7.1 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2025/2/19\r\nv3.7.0 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2025/1/22\r\nv3.6.9 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/12/23\r\nv3.6.8 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/11/30\r\nv3.6.7 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n2、删除了默认自带的无条件学习武学\r\n\r\n----------------------------------------------------\r\n\r\n2024/11/23\r\nv3.6.6 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/8/25\r\nv3.6.5 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/8/15\r\nv3.6.4 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n2、修复无限武学多了不生效的问题\r\n3、以往版本所有BUG在这个新版本基本都得到解决，出现问题无脑更新版本\r\n\r\n----------------------------------------------------\r\n\r\n2024/7/8\r\nv3.6.2 版本日志\r\n【修复】\r\n1、日常适配最新版本\r\n\r\nps：最近有点懒，等13号回家之后再大更\r\n\r\n----------------------------------------------------\r\n\r\n2024/6/28\r\nv3.6.1 版本日志\r\n【修复】\r\n1、添加物品中分类消失问题\r\n\r\n----------------------------------------------------\r\n\r\n2024/6/27\r\nv3.6.0 版本日志\r\n【增加】\r\n1、新增DLC修改模块\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n2、杂项功能中贡献显示错误问题\r\n\r\n----------------------------------------------------\r\n\r\n\r\n2024/4/15\r\nv3.5.1 版本日志\r\n【修复】\r\n1、适配最新版本游戏\r\n2、修复呼出快捷地图关闭时进入大地图会自动瞬移的问题\r\n\r\n----------------------------------------------------\r\n\r\n2024/3/17\r\nv3.5.0 版本日志\r\n\r\n【增加】\r\n1、“人物属性”面板增加“反击”\r\n2、“人物属性”面板增加“领悟点”\r\n\r\n@贝希摩斯(1185838120)提出\r\n3、“杂项功能”面板增加“解锁全部配方”\r\n\r\n@君.(2207229615) 提出\r\n4、增加“添加Buff”面板，可以添加游戏中除开局外所有的后天Buff(包括MOD中的Buff，有些游戏自带的不生效)\r\n【修复】\r\n1、适配最新版本游戏\r\n2、修复在“物品添加”→“装备”→“全部”下搜索宠物装备搜索不到的问题\r\n3、修复在各种搜索中切换类型时不自动搜索的问题\r\n\r\n----------------------------------------------------\r\n\r\n2024/2/26\r\nv3.4.1 版本日志\r\n\r\n【增加】\r\n1、增加背包物品快捷清除\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2024/2/9\r\nv3.4 版本日志\r\n\r\n【增加】\r\n1、增加宠物装备\r\n2、增加新地图“西域地区”（新地图呼出热键为F8，修改请去“Steam\\steamapps\\common\\WulinSH\\BepInEx\\config\\HaxxToyBox.cfg”下修改）\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2024/2/9\r\nv3.3.2 版本日志\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2024/1/12\r\nv3.3.1 版本日志\r\n\r\n【修复】\r\n1、适配最新版本游戏\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/20\r\nv3.3.0 版本日志\r\n\r\n【增加】\r\n1、修复小bug、适配游戏\r\n2、增加NPC添加界面\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/16\r\nv3.2.0 版本日志\r\n\r\n【增加】\r\n1、适配游戏\r\n2、增加解锁全部地图\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/11\r\nv3.1.0 版本日志\r\n\r\n【增加】\r\n1、UI界面适配多分辨率\r\n2、增加了江湖历练、领悟点的修改\r\n3、增加装备添加时属性最高\r\n\r\n----------------------------------------------------\r\n\r\n2023/12/8\r\nv3.0.0 版本日志\r\n\r\n【增加】\r\n1、重新绘制UI，解决卡顿问题\r\n2、增加了天赋搜索、物品搜索、武学搜索\r\n3、增加了热键呼出地图\r\n4、增加了刷新任务栏\r\n5、增加了能力等级随意调整\r\n\r\n【说明】\r\n1、无限武学目前存在bug 等待下次版本更新\r\n2、NPC添加功能 等待下次版本更新\r\n\r\n----------------------------------------------------\r\n\r\n2023/11/17\r\nv2.6.1 版本日志\r\n\r\n【修复】\r\n1、修复WASD无法移动问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/11/17\r\nv2.6.0 版本日志\r\n\r\n【增加】\r\n1、增加了手动呼出控制台功能\r\n\r\n【修复】\r\n1、适配了V1.0正式版本\r\n\r\n【调整】\r\n1、人物属性“实战能力”改为“反击”\r\n2、暂时关闭“武学”界面，过段时间恢复\r\n3、因为游戏内部UI更新，修改器的初始化也改为了在进入游戏后初始化，所以第一次TAB呼出可能会卡顿几秒，之后就好了\r\n\r\nBTW:这段时间在写别的作业所以比较忙，新版本大更新对UI、功能做了很多调整，所以我需要重新去整理和逆向，时间有限请多包涵！（WBG夺冠直接开肝）\r\n\r\n----------------------------------------------------\r\n\r\n2023/10/26\r\nv2.5.1 版本日志\r\n\r\n【增加】\r\n1、增加了能力界面编辑等级功能\r\n2、增加了战斗中可随时撤退功能（必须要等第一次集气结束才可以撤退，不然会出现异常）\r\n\r\n【修复】\r\n1、修复了添加NPC再离队会出现异常问题\r\n\r\n【调整】\r\n1、取消了五倍能力经验\r\n\r\n----------------------------------------------------\r\n\r\n2023/10/19\r\nv2.4.3 版本日志\r\n\r\n【调整】\r\n1、适配了 V0.9.1017b47 版本\r\n2、优化了如果没有触发NPC剧情就添加名字不能正常显示问题\r\n3、取消了添加物品中数量自动记忆（现在打开关闭会重置为1）\r\n\r\n----------------------------------------------------\r\n\r\n2023/10/16\r\nv2.4.2 版本日志\r\n\r\n【增加】\r\n1、增加识别MOD中的天赋、武学、物品数据\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/30\r\nv2.4.1 版本日志\r\n\r\n【增加】\r\n1、增加添加NPC界面以及入队功能（部分NPC无法添加或添加有问题）\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/27\r\nv2.3.5 版本日志\r\n\r\n【增加】\r\n1、增加添加物品搜索功能\r\n\r\n【调整】\r\n1、优化添加物品界面显示逻辑\r\n2、输入数量时不再需要回车确认\r\n3、打开时记忆上次浏览物品分类、子分类、第几页\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/27\r\nv2.3.4 版本日志\r\n\r\n【修复】\r\n1、适配了 V0.9.0926b46 版本\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/26\r\nv2.3.3 版本日志\r\n【增加】\r\n1、优化添加物品分类\r\n2、添加装备时所有附加属性全部完美\r\n\r\n【修复】\r\n1、修复金色饰品无法添加BUG\r\n2、修复刷新任务热键与西南地区地图热键冲突BUG（还是不行的请先删除原来的配置文件）\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/25\r\nv2.3.2 版本日志\r\n【增加】\r\n1、添加装备时自动添加满级锻造属性\r\n\r\n【修复】\r\n1、修复武学功能自动添加兰亭序的BUG\r\n2、修复刷新任务的BUG\r\n\r\n【调整】\r\n1、武学功能恢复\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/23\r\nv2.3.1 版本日志\r\n\r\n【修复】\r\n1、适配了 V0.9.0923b45 版本\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/23\r\nv2.3 版本日志\r\n【增加】\r\n1、增加了西南地图\r\n\r\n【修复】\r\n1、适配了 V0.9.0922b44 版本\r\n\r\n【调整】\r\n1、武学功能暂时关闭(终于理解haxx为什么不想更新 －_－|| )\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/11\r\nv2.2.1 版本日志\r\n【修复】\r\n1、适配了 V0.8.0909b42 版本\r\n2、修复一些bug\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/7\r\nv2.2 版本日志\r\n【新增】\r\n1、地图传送(重大更新)\r\n2、门派贡献编辑\r\n3、一键解锁所有地图\r\n4、功能热键修改\r\n\r\n【优化】\r\n1、人物速度调节按钮遮挡问题\r\n\r\n【修复】\r\n1、人物速度调节在进入地图时会失控\r\nTips:建议呼出地图界面时配合大地图瞬移一块使用\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/4\r\nv2.1 版本日志\r\n【新增】\r\n1、武学无上限(可手动学习武学)\r\n2、在游玩MOD情况下也可以解锁成就\r\n3、使用物品无限制/装备无Debuff\r\n\r\n【优化】\r\n1、增加武学非常卡顿问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/3\r\nv2.0 版本日志\r\n【新增】\r\n1、自己和队友增加武学功能\r\n2、突破武学上限\r\n3、人物移动速度 x2/x4/x6/x8\r\n4、战斗倍速 x6/x8\r\n5、自动解锁自由模式\r\n6、任务无限刷新\r\n7、更换框架，之前的功能重新加\r\n\r\n----------------------------------------------------\r\n\r\n2023/9/1\r\nv1.3 版本日志\r\n\r\n【新增】\r\n1、武学可以一键学习(可以无限学习，但最多只能上阵15个)\r\n2、武学上限变为15（战斗界面最多可以加载15个）\r\n\r\n【优化】\r\n1、修改器界面\r\n2、武学上限变为15（战斗界面最多可以加载15个）\r\n\r\n【修复】\r\n1、门派之外的任务无法刷新问题\r\n2、修改铜钱也会修改门派贡献问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/8/31\r\nv1.2 版本日志\r\n\r\n【新增】\r\n1、一键恢复内力、生命\r\n2、门派贡献编辑\r\n\r\n【优化】\r\n1、战斗后自动回复（现在可以回满生命、内力、体力、心情）\r\n\r\n【修复】\r\n1、角色无法WASD走路问题\r\n\r\n----------------------------------------------------\r\n\r\n2023/8/31\r\nv1.1 版本日志\r\n\r\n【新增】\r\n1、解锁全成就\r\n2、任务无限刷新（已接取不会刷新，若想刷新，请先放弃）\r\n3、一键恢复体力、心情\r\n4、战斗时倍速最大为4\r\n\r\n【优化】\r\n1、战斗后自动回复（现在可以回满生命、内力、体力、心情）\r\n\r\n【修复】\r\n1、人物速度在进入新地图时会重置\r\n\r\n\r\n----------------------------------------------------\r\n\r\n\r\n2023/8/30\r\nv1.0版本日志\r\n\r\n【新增】\r\n1、物品添加\r\n2、天赋编辑\r\n3、属性编辑\r\n4、杂项等";
            GameInfoUI.Instance.SetButtonsGroupActive(false);
            GameInfoUI.Instance.ShowWindow(true, "关于本修改器  Version:3.7.5", UpInfo);
        }


    }

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(UISlgPlayerCreate), "CostPoints")]
    //public static bool CostPoints_PrePatch(ref Int32 costPoint, Boolean isFeature)
    //{

    //    costPoint = -costPoint;

    //    return true;
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(BattleUI), "OnSpeedButtonClickHandler")]
    //public static void BattleUISwitchSpeed_PrePatch()
    //{
    //    if (MiscPanel.Instance) return;

    //    UIMiscPanel.battleSpeed = (UIMiscPanel.battleSpeed * 2) % 7;
    //}

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(BattleUI), "SwitchSpeed")]
    //public static void BattleUISwitchSpeed_PostPatch(BattleUI __instance)
    //{
    //    if (MiscPanel.Instance) return;

    //    __instance.speedText.text = $"x{UIMiscPanel.battleSpeed}";
    //    Time.timeScale = UIMiscPanel.battleSpeed;

    //}

}
