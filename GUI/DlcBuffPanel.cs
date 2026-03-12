using GameData;
using UnityEngine.UI;
using WuLin;
using static Doozy.Engine.UI.Nodes.WaitNode;
using static Il2CppSystem.Diagnostics.Tracing.TplEtwProvider;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class DlcBuffPanel : MonoBehaviour
{
    //private bool needUpdate = true;

    private InfinityScrollDlcBuffData _infinityScroll;

    public PopupPanel Popup;

    public List<BuffData> Buffs;

    public static DlcBuffPanel Instance { get; private set; }

    public DlcBuffPanel(IntPtr ptr) : base(ptr) { }

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
            var BuffDB = BaseDataClass.GetGameData<BuffDataScriptObject>().data;
            var temp = new List<BuffData>();
            temp.Clear();
            if (input != "")
            {
                foreach (var item in BuffDB)
                {
                    if (item.Value.UName.Contains(input) || item.Value.GetInfo().Contains(input))
                    {
                        temp.Add(item.Value);
                    }
                }
                Buffs = new List<BuffData>();
                foreach (var Buff in temp)
                {
                    Buffs.Add(Buff);
                }
            }
            else
            {
                Buffs = new List<BuffData>();
                foreach (var Buff in BuffDB.Values)
                {
                    Buffs.Add(Buff);
                }
            }

            Buffs.Sort((a, b) => b.Rarity.CompareTo(a.Rarity));

            _infinityScroll.Data = Buffs;

        });

        var scrollView = transform.Find("ScrollView").gameObject;
        var entryPrefab = transform.Find("ScrollView/Viewport/EntryPrefab").gameObject;

        entryPrefab.AddComponent<DlcBuffEntry>();
        entryPrefab.SetActive(false);
        _infinityScroll = scrollView.AddComponent<InfinityScrollDlcBuffData>();

        var contentLayout = transform.Find("ScrollView/Viewport/Content").gameObject.GetComponent<GridLayoutGroup>();


        _infinityScroll.ItemPrefab = entryPrefab;
        _infinityScroll.SpaceY = 10;
    }

    private void Start()
    {
        var BuffDB = BaseDataClass.GetGameData<BuffDataScriptObject>().data;

        Buffs = new List<BuffData>();
        foreach (var Buff in BuffDB.Values)
        {
            Buffs.Add(Buff);
        }
        Buffs.Sort((a, b) => b.Rarity.CompareTo(a.Rarity));

        _infinityScroll.Data = Buffs;
    }

    public void Show()
    {
        gameObject.SetActive(true);

    }

    public void Hide()
    {
        //Popup.Close();
        gameObject.SetActive(false);

        DlcPanel.Instance.UpdateRoleInfo();
    }
}
