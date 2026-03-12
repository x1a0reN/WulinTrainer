using GameData;
using TMPro;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class DlcBuffEntry : MonoBehaviour
{
    private BuffData _data;
    public BuffData Data
    {
        get => _data;
        set
        {
            if (value == _data || value == null) return;
            _data = value;
            _nameText.text = _data.GetNameWithColor();
            _rankText.text = GetRankText(_data.Rarity);
            _descriptionText.text = _data.GetInfo();
        }
    }

    private TextMeshProUGUI _rankText;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _descriptionText;
    private Button _addButton;

    public DlcBuffEntry(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        _rankText = transform.Find("RankText").GetComponent<TextMeshProUGUI>();
        _nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        _descriptionText = transform.Find("DesText").GetComponent<TextMeshProUGUI>();

        _addButton = transform.Find("Button").GetComponent<Button>();
        _addButton.onClick.RemoveAllListeners();
        _addButton.onClick.AddListener(OnClickAdd);
    }

    private string GetRankText(int rank)
    {
        string color;
        switch (rank)
        {
            case 3:
                color = "orange";
                break;
            case 2:
                color = "purple";
                break;
            case 1:
                color = "#87CEEB";
                break;
            default:
                return $"#<size=20>{rank}</size>";
        }

        return $"<color={color}>#<size=20>{rank}</size></color>";
    }

    private void OnClickAdd()
    {
        var character = DlcPanel.Instance?.Character;
        if (character == null) return;

        character.buffLib.AddBuff(Data);
    }
}

public class DlcBuffDelEntry : MonoBehaviour
{
    private BuffInstance buffInstance;

    public TextMeshProUGUI nameText;
    public Button delButton;

    static DlcBuffDelEntry()
    {
        ClassInjector.RegisterTypeInIl2Cpp<DlcBuffDelEntry>();
    }

    public DlcBuffDelEntry(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        nameText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        delButton = transform.Find("DelButton").GetComponent<Button>();
        delButton.onClick.RemoveAllListeners();
        delButton.onClick.AddListener(OnClickDel);
    }

    public void SetBuff(BuffInstance data)
    {
        buffInstance = data;
        nameText.text = data.Templete.GetName();
    }

    private void OnClickDel()
    {
        var character = DlcPanel.Instance?.Character;
        if (character == null) return;

        character.buffLib.TryRemoveBuff(buffInstance);
        DlcPanel.Instance.UpdateRoleInfo();
    }
}
