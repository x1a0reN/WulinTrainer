using GameData;
using TMPro;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class BuffEntry : MonoBehaviour
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

    public BuffEntry(IntPtr ptr) : base(ptr) { }

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
        var character = RolePanel.Instance?.Character;
        if (character == null) return;

        foreach (var buff in character.buffLib.GetAllBuffs())
        {
            ToyBox.LogMessage(buff.Templete.Uid);
        }
        character.ApplyBuff(Data.Uid);
    }
}

public class BuffDelEntry : MonoBehaviour
{
    private BuffData traitData;

    public TextMeshProUGUI nameText;
    public Button delButton;

    static BuffDelEntry()
    {
        ClassInjector.RegisterTypeInIl2Cpp<TraitDelEntry>();
    }

    public BuffDelEntry(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        nameText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        delButton = transform.Find("DelButton").GetComponent<Button>();
        delButton.onClick.RemoveAllListeners();
        delButton.onClick.AddListener(OnClickDel);
    }

    public void SetTrait(BuffData data)
    {
        traitData = data;
        nameText.text = data.GetName();
    }

    private void OnClickDel()
    {
        var character = RolePanel.Instance?.Character;
        if (character == null) return;

        //var traits = character.GetAllTrait();

        //if (traits.Contains(traitData))
        //{
        //    traits.Remove(traitData);
        //    character.SetTraits(traits);

        //    RolePanel.Instance.UpdateRoleInfo();
        //}
    }
}
