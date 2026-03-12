using GameData;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Xml;
using TMPro;
using WuLin;
using static Il2CppSystem.Diagnostics.Tracing.TplEtwProvider;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
public class NpcPanel : MonoBehaviour
{
    private InputField _searchInput;

    private InfinityScrollNpcData _infinityScroll;

    public List<CharacterPoolData> CharacterList = new();
    public List<CharacterPoolData> CharacterListPool = new();

    public string g_keywords = "";

    public static NpcPanel Instance { get; private set; }

    public static Il2CppReferenceArray<ItemForgeEffectData> buffData;
    public NpcPanel(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        Instance = this;

        _searchInput = transform.Find("SearchInput").GetComponent<InputField>();
        _searchInput.onValueChanged.RemoveAllListeners();
        _searchInput.onValueChanged.AddListener((string input) =>
        {
            var keyword = input;
            if (keyword == "") { _infinityScroll.Data = CharacterListPool; }
            else
            {
                this.CharacterList = (from x in CharacterListPool
                                      where x.UName.Contains(keyword)
                                       select x).ToList<CharacterPoolData>();
                _infinityScroll.Data = CharacterList;
            }
        });
        var scrollView = transform.Find("ScrollView").gameObject;
        var entryPrefab = transform.Find("ScrollView/Viewport/EntryPrefab").gameObject;
        entryPrefab.AddComponent<NpcEntry>();
        _infinityScroll = scrollView.AddComponent<InfinityScrollNpcData>();
        _infinityScroll.ItemPrefab = entryPrefab;
        _infinityScroll.Columns = 12;
        _infinityScroll.SpaceX = 25;
        _infinityScroll.SpaceY = 25;

        var CharacterData = BaseDataClass.GetGameData<CharacterPoolDataScriptObject>().CharacterPoolData;

        foreach (var character in CharacterData)
        {
            CharacterList.Add(character);
            CharacterListPool.Add(character);
        }
        _infinityScroll.Data = CharacterListPool;
    }

    private void OnEnable()
    {
        //ToyBox.LogMessage("Buff");
        //buffData = BaseDataClass.GetGameData<ItemForgeEffectDataScriptObject>().ItemForgeEffectData;
    }

}
