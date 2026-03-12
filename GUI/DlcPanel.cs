using GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class DlcPanel : MonoBehaviour
{
    private ToggleGroup _roleList;

    public GameCharacterInstance Character = null;

    public Dlc1FactionInstance Faction = null;

    public Il2CppSystem.Collections.Generic.List<Int32> MembersIndex = new();

    public Il2CppSystem.Collections.Generic.List<GameCharacterInstance> Members = new();


    public GameObject LeftInfoGroup;
    public GameObject RightInfoGroup;
    public GameObject BottomInfoGroup;
    public GameObject AdditionInfoGroup;

    public Transform BuffList;
    public GameObject BuffEntryPrefab;

    readonly string[] leftRoleInfoKeys = {
        "gongji", "qinggong", "quanzhang", "shuadao", "duanbing", "mingzhong", "baoji", "shizhannengli", "xinqing"
    };

    readonly string[] rightRoleInfoKeys = {
        "fangyu", "jiqi", "yujian", "changbing", "yinlv", "shanbi", "gedang", "hubo"
    };

    readonly string[] additionRoleInfoKeys = {
        "hp", "mp", "bili", "tizhi", "minjie", "wuxing", "fuyuan","fanji"
    };

    readonly string[] percentageKeys = {
        "mingzhong", "baoji", "shanbi", "gedang", "hubo"
    };

    readonly Dictionary<string, string> keyToLabelMap = new() {
        {"gongji", "攻击"}, {"qinggong", "轻功"}, {"quanzhang", "拳掌"}, {"shuadao", "耍刀"},
        {"duanbing", "短兵"}, {"mingzhong", "命中"}, {"baoji", "暴击"}, {"yishu", "医术"},
        {"anqi", "暗器"}, {"wuxuechangshi", "武学常识"}, {"hp", "生命"}, {"mp", "内力"},
        {"point", "冲穴点数"}, {"exp", "经验"}, {"lv", "等级"}, {"fangyu", "防御"}, {"jiqi", "集气速度"},
        {"yujian", "御剑"}, {"changbing", "长兵"}, {"yinlv", "乐器"}, {"shanbi", "闪避"},
        {"gedang", "格挡"}, {"dushu", "毒术"}, {"hubo", "互搏"}, {"shizhannengli", "实战能力"},
        {"bili", "臂力"}, {"tizhi", "体质"}, {"minjie", "敏捷"}, {"wuxing", "悟性"},
        {"fuyuan", "福缘"}, {"rende", "仁德"}, {"yiqi", "义气"}, {"lijie", "礼节"},
        {"xinyong", "信用"}, {"zhihui", "智慧"}, {"yongqi", "勇气"}, {"replv", "名声级别"},
        {"repexp", "名声经验"}, {"coin", "金币"},{"jianghulilian", "江湖历练"},{ "fanji","反击"},{"xinqing","DLC1心情"}
     };

    public PopupPanel Popup;
    public static DlcPanel Instance { get; private set; }

    public DlcPanel(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        Instance = this;
        Popup = gameObject.AddComponent<PopupPanel>();
        _roleList = transform.Find("RoleList/ScrollView/Viewport/Content").GetComponent<ToggleGroup>();
        LeftInfoGroup = transform.Find("RoleInfo/LeftInfo").gameObject;
        RightInfoGroup = transform.Find("RoleInfo/RightInfo").gameObject;
        AdditionInfoGroup = transform.Find("RoleInfo/AdditionInfo").gameObject;

        BuffList = transform.Find("Traits/Viewport/Content");
        BuffEntryPrefab = transform.Find("Traits/Viewport/EntryPrefab").gameObject;
        BuffEntryPrefab.AddComponent<DlcBuffDelEntry>();
        BuffEntryPrefab.SetActive(false);
        SetupRoleList();
        UpdateRoleInfo(Dlc1SlgManager.Instance.factionInAction.Leader);
    }

    private void SetupRoleList()
    {
        Faction = Dlc1SlgManager.Instance.factionInAction;
        // 获取势力成员并排序
        MembersIndex = UISlgMain.Instance.SortMembers(Faction.members);
        // 获取势力成员实例
        foreach (var index in MembersIndex)
        {
            var member = GameCharacterManager.Instance.GetCharacter(index);
            Members.Add(member);

            ToyBox.LogMessage(member.FullName);
        }

        var numRole = MembersIndex.Count;

        for (int i = 0; i < _roleList.transform.childCount; i++)
        {
            var entry = _roleList.transform.GetChild(i);
            entry.gameObject.SetActive(i < numRole);
            if (i >= numRole) continue;

            var role = GameCharacterManager.Instance.GetCharacter(MembersIndex[i]);

            entry.Find("Content/Avatar/Avatar").GetComponent<Image>().sprite = role.GetPortrait(GameCharacterInstance.PortraitType.Small);
            entry.Find("Content/NameText").GetComponent<TextMeshProUGUI>().text = role.FullName;

            var toggle = entry.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate (bool isOn) {
                if (isOn)
                {
                    UpdateRoleInfo(role);
                }
            });
        }
    }



    public void UpdateRoleInfo(GameCharacterInstance charac = null)
    {
        if (charac != null)
            Character = charac;
        //ToyBox.LogMessage($"{character.FullName} Selected.");

        UpdateInfoGroup(LeftInfoGroup, leftRoleInfoKeys);
        UpdateInfoGroup(RightInfoGroup, rightRoleInfoKeys);
        UpdateInfoGroup(AdditionInfoGroup, additionRoleInfoKeys);

        UpdateBuffList();
    }

    private void UpdateInfoGroup(GameObject group, string[] keys)
    {
        if (Character == null) return;

        int iterations = Mathf.Min(group.transform.childCount, keys.Length);
        var propSource = GameCharacterInstance.FinalPropSource.Origin;
        for (int i = 0; i < iterations; i++)
        {
            Transform child = group.transform.GetChild(i);
            var inputObj = child.GetComponentInChildren<TMP_InputField>();
            if (inputObj == null) continue;

            string formatstr = percentageKeys.Contains(keys[i]) ? "F3" : "";

            string propKey = keyToLabelMap[keys[i]];
            ToyBox.LogMessage(propKey);
            inputObj.text = Character.GetFinalPropAsDecimal(propKey, propSource).ToString(formatstr);

            inputObj.onValueChanged.RemoveAllListeners();
            inputObj.onValueChanged.AddListener(delegate (string input) {
                bool ret = ChangeProperty(propKey, input);
                if (!ret) inputObj.text = inputObj.m_OriginalText;
            });
        }
    }
    private bool ChangeProperty(string propKey, string input)
    {
        ToyBox.LogMessage(propKey);
        if (Character == null ||
            !Il2CppSystem.Decimal.TryParse(input, out Il2CppSystem.Decimal value))
        {
            return false;
        }

        if (!Character.m_originProps.ContainsKey(propKey))
            Character.m_originProps.Add(propKey, 0);

        var diff = value - Character.m_originProps[propKey];
        Character.ChangeOriginProp(propKey, diff);

        return true;
    }
    private void UpdateBuffList()
    {
        if (Character == null) return;

        var buffs = Character.buffLib.GetAllBuffs();

        for (int i = 0; i < buffs.Count; i++)
        {
            var buff = buffs[i];
            GameObject entry;
            if (i >= BuffList.childCount)
            {
                entry = Instantiate(BuffEntryPrefab, BuffList);
            }
            else
            {
                entry = BuffList.GetChild(i).gameObject;
            }
            entry.SetActive(true);
            entry.GetComponent<DlcBuffDelEntry>().SetBuff(buff);
        }

        for (int i = buffs.Count; i < BuffList.childCount; i++)
        {
            BuffList.GetChild(i).gameObject.SetActive(false);
        }

    }
    private void OnEnable()
    {
        Faction.backPack.AddItem(303001);
        //Faction.backPack.ChangeCurrency(CurrencyType.slg, 99999);

        //var itemsConfig = BaseDataClass.GetGameData<ItemDataScriptObject>().data;
        //foreach (var kvp in itemsConfig)
        //{
        //    var itemData = kvp.value;
        //    ToyBox.LogMessage(itemData.Uid);
        //    ToyBox.LogMessage(itemData.UName);

        //}

        SetupRoleList();

        //UpdateRoleInfo();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Popup.Open();

    }

    public void Hide()
    {
        Popup.Close();
        gameObject.SetActive(false);

    }

}
