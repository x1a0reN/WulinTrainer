using GameData;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
public class DlcItemPanel : MonoBehaviour
{
    private InputField _numberInput;
    private InputField _searchInput;
    private ToggleGroup _typeGroup;
    private ToggleGroup _subtypeGroup;

    private InfinityScrollDlcItemData _infinityScroll;
    
    private ItemType[][] _typeList = {
        new ItemType[] { ItemType.Equip,
            ItemType.Equip_Weapon, ItemType.Equip_Armor, ItemType.Equip_Amulet},
        new ItemType[] { ItemType.Consumeable_SlgFaction},
        new ItemType[] { ItemType.Consumeable_SlgPersonal},
        new ItemType[] { ItemType.Misc },
    };
    private Dictionary<ItemType, List<ItemData>> _classifiedItems = new();

    private int _selectedType = 0;

    public List<ItemData> ItemList = new();
    public int Number = 1;
    public int g_selectedType = 0;
    public int g_subtype = 0;
    public string g_keywords = "";

    public static DlcItemPanel Instance { get; private set; }

    public static Il2CppReferenceArray<ItemForgeEffectData> buffData;
    public DlcItemPanel(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        Instance = this;
        _typeGroup = transform.Find("TypeGroup/Toggles").GetComponent<ToggleGroup>();
        _subtypeGroup = transform.Find("SubtypeGroup").GetComponent<ToggleGroup>();
        for (int i = 0; i < _typeGroup.transform.childCount; i++)
        {
            var toggle = _typeGroup.transform.GetChild(i).GetComponent<Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            var type = i;
            toggle.onValueChanged.AddListener((bool value) => {
                if (value)
                {
                    g_selectedType = type;
                    UpdateItemList(type, keywords: g_keywords);
                    UpdateSubToggles(type);
                }
            });
        }

        for (int i = 0; i < _subtypeGroup.transform.childCount; i++)
        {
            var toggle = _subtypeGroup.transform.GetChild(i).GetComponent<Toggle>();
            int subtype = i;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((bool value) => {
                if (value)
                {
                    g_subtype = subtype;
                    g_selectedType = _selectedType;
                    UpdateItemList(_selectedType, subtype, g_keywords);
                }
            });
        }
        _numberInput = transform.Find("NumInput").GetComponent<InputField>();
        _numberInput.onValueChanged.RemoveAllListeners();
        _numberInput.onValueChanged.AddListener((string input) => {
            int.TryParse(input, out Number);
            Number = Mathf.Clamp(Number, 1, 9999);
        });

        _searchInput = transform.Find("SearchInput").GetComponent<InputField>();
        _searchInput.onValueChanged.RemoveAllListeners();
        _searchInput.onValueChanged.AddListener((string input) =>
        {
            //搜索记忆功能
            g_keywords = input;

            UpdateItemList(g_selectedType, g_subtype, input);
        });
        var scrollView = transform.Find("ScrollView").gameObject;
        var entryPrefab = transform.Find("ScrollView/Viewport/EntryPrefab").gameObject;
        entryPrefab.AddComponent<DlcItemEntry>();
        _infinityScroll = scrollView.AddComponent<InfinityScrollDlcItemData>();
        _infinityScroll.ItemPrefab = entryPrefab;
        _infinityScroll.Columns = 11;
        _infinityScroll.SpaceX = 25;
        _infinityScroll.SpaceY = 25;

        LoadItemData();

        UpdateItemList(0);
        UpdateSubToggles(0);
    }

    private void OnEnable()
    {
        ToyBox.LogMessage("Buff");
        buffData = BaseDataClass.GetGameData<ItemForgeEffectDataScriptObject>().ItemForgeEffectData;
    }
    private void LoadItemData()
    {
#if DEBUGMODE
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif

        foreach (var group in _typeList)
        {
            ItemType mainType = group[0];
            if (!_classifiedItems.ContainsKey(mainType))
            {
                _classifiedItems[mainType] = new List<ItemData>();
            }
        }

        var itemsConfig = GameConfig.Instance.ItemDataScriptObject.ItemData;
        foreach (var itemData in itemsConfig)
        {
            foreach (var group in _typeList)
            {
                ItemType mainType = group[0];
                if ((itemData.Type & mainType) == itemData.Type)
                {
                    _classifiedItems[mainType].Add(itemData);
                    break;
                }
                else {
                    ToyBox.LogMessage(itemData.UName);
                    ToyBox.LogMessage(itemData.Type);
                    ToyBox.LogMessage(mainType);
                }
            }
        }

        foreach (var list in _classifiedItems.Values)
        {
            list.Sort((a, b) => a.Piror.CompareTo(b.Piror));
        }

#if DEBUGMODE
        stopwatch.Stop();
        ToyBox.LogMessage("LoadItemData Execution time: " + stopwatch.ElapsedMilliseconds + "ms");
#endif
    }

    private void UpdateSubToggles(int type)
    {

#if DEBUGMODE
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif

        var toggles = _subtypeGroup.transform;

        int togglenum = _typeList[type].Length;
        switch (type)
        {
            case 0:
                toggles.GetChild(1).GetComponentInChildren<Text>().text = "武器";
                toggles.GetChild(2).GetComponentInChildren<Text>().text = "内甲";
                toggles.GetChild(3).GetComponentInChildren<Text>().text = "配饰";
                toggles.GetChild(4).GetComponentInChildren<Text>().text = "宠物";
                break;
            case 1:
                toggles.GetChild(1).GetComponentInChildren<Text>().text = "外功";
                toggles.GetChild(2).GetComponentInChildren<Text>().text = "内功";
                break;
            case 2:
                toggles.GetChild(1).GetComponentInChildren<Text>().text = "食物";
                toggles.GetChild(2).GetComponentInChildren<Text>().text = "丹药";
                toggles.GetChild(3).GetComponentInChildren<Text>().text = "药品";
                toggles.GetChild(4).GetComponentInChildren<Text>().text = "配方";
                break;
        }

        for (int i = 0; i < toggles.childCount; i++)
        {
            toggles.GetChild(i).gameObject.SetActive(i < togglenum);
        }

        toggles.GetChild(0).GetComponent<Toggle>().isOn = true;

#if DEBUGMODE
        stopwatch.Stop();
        ToyBox.LogMessage("UpdateSubToggles Execution time: " + stopwatch.ElapsedMilliseconds + "ms");
#endif
    }


    private void UpdateItemList(int maintype, int subType = 0, string keywords = "")
    {

        if (maintype < 0 || maintype >= _typeList.Length) return;

#if DEBUGMODE
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif
        ToyBox.LogMessage(maintype);
        ToyBox.LogMessage(subType);
        ToyBox.LogMessage(keywords);

        _selectedType = maintype;
        var type = _typeList[maintype][0];
        if (keywords != "")
        {
            ItemList = _classifiedItems[type].Where(x =>
        (x.Type & _typeList[maintype][subType]) == x.Type && x.GetName().Contains(keywords))
        .ToList();
        }
        else
        {
            ItemList = _classifiedItems[type].Where(x =>
        (x.Type & _typeList[maintype][subType]) == x.Type)
        .ToList();
        }


        _infinityScroll.Data = ItemList;

#if DEBUGMODE
        stopwatch.Stop();
        ToyBox.LogMessage("UpdateItemList Execution time: " + stopwatch.ElapsedMilliseconds + "ms");
#endif
    }

}
