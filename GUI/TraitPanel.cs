using GameData;
using UnityEngine.UI;
using WuLin;
using static Doozy.Engine.UI.Nodes.WaitNode;
using static Il2CppSystem.Diagnostics.Tracing.TplEtwProvider;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class TraitPanel : MonoBehaviour
{
    //private bool needUpdate = true;

    private InfinityScrollTraitData _infinityScroll;

    public PopupPanel Popup;

    public List<TraitData> Traits;

    public static TraitPanel Instance { get; private set; }

    public TraitPanel(IntPtr ptr) : base(ptr) { }
    private Dictionary<ItemType, List<ItemData>> _classifiedItems = new();
    private void Awake()
    {
        Instance = this;

        //Popup = gameObject.AddComponent<PopupPanel>();
        var close = transform.Find("PopupBase/Top/CloseButton").GetComponent<Button>();
        close.gameObject.AddComponent<FadeButtonWrapper>();
        close.onClick.AddListener(() => {
            Hide();
        });

        var search_keywords = transform.Find("BottomBar/InputField").GetComponent<InputField>();
        search_keywords.onValueChanged.AddListener((string input) =>
        {
            var traitDB = BaseDataClass.GetGameData<TraitDataScriptObject>().data;
            var temp = new List<TraitData>();
            temp.Clear();
            if (input != "")
            {
                foreach (var item in traitDB)
                {
                    if (item.Value.UName.Contains(input) || item.Value.GetInfo().Contains(input))
                    {
                        temp.Add(item.Value);
                    }
                }
                if (temp.Count == 0)
                {
                    _infinityScroll.Data = new List<TraitData>();
                    return;
                }

                Traits = new List<TraitData>();
                foreach (var trait in temp)
                {
                    Traits.Add(trait);
                }
            }
            else {
                Traits = new List<TraitData>();
                foreach (var trait in traitDB.Values)
                {
                    Traits.Add(trait);
                }
            }

            Traits.Sort((a, b) => b.Rarity.CompareTo(a.Rarity));

            _infinityScroll.Data = Traits;

        });

        var scrollView = transform.Find("ScrollView").gameObject;
        var entryPrefab = transform.Find("ScrollView/Viewport/EntryPrefab").gameObject;

        entryPrefab.AddComponent<TraitEntry>();
        entryPrefab.SetActive(false);
        _infinityScroll = scrollView.AddComponent<InfinityScrollTraitData>();

        var contentLayout = transform.Find("ScrollView/Viewport/Content").gameObject.GetComponent<GridLayoutGroup>();


        _infinityScroll.ItemPrefab = entryPrefab;
        _infinityScroll.SpaceY = 10;
    }
   
    private void Start()
    {
        var traitDB = BaseDataClass.GetGameData<TraitDataScriptObject>().data;

        Traits = new List<TraitData>();
        foreach (var trait in traitDB.Values) {
            Traits.Add(trait);
        }
        Traits.Sort((a, b) => b.Rarity.CompareTo(a.Rarity));

        _infinityScroll.Data = Traits;
        // infinityScroll.SetTotalItems(traits.Count);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        //Popup.Open();

        //if (needUpdate) {
        //    UpdateTraitList();
        //}
    }

    public void Hide()
    {
        //Popup.Close();
        gameObject.SetActive(false);

        RolePanel.Instance.UpdateRoleInfo();
    }

    private void UpdateTraitList()
    {
        //for (int i = 0; i < traits.Count; i++) {
        //    var trait = traits[i];
        //    GameObject entry = null;
        //    if (i >= scrollView.childCount) {
        //        entry = Instantiate(entryPrefab, scrollView);
        //    }
        //    else {
        //        entry = scrollView.GetChild(i).gameObject;
        //    }
        //    entry.SetActive(true);
        //    entry.GetComponent<TraitEntry>().SetTrait(trait);
        //}

        //for (int i = traits.Count; i < scrollView.childCount; i++) {
        //    scrollView.GetChild(i).gameObject.SetActive(false);
        //}
    }
}
