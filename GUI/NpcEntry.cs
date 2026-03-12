using GameData;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine.UI;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class NpcEntry : MonoBehaviour
{
    private CharacterPoolData _data;

    public CharacterPoolData Data
    {
        get => _data;
        set
        {
            if (value == _data || value == null) return;

            _data = value;
            _nameText.text = _data.UName;
            _icon.sprite = GameCharacterManager.GetPortrait(_data.Uid, null, GameCharacterInstance.PortraitType.Small, null, null); ;
        }
    }

    private Button _button;
    private Image _icon;
    private TextMeshProUGUI _nameText;

    public NpcEntry(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        _nameText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _icon = transform.Find("Button/Icon").GetComponent<Image>();

        _button = transform.Find("Button").GetComponent<Button>();
        _button.gameObject.AddComponent<FadeButtonWrapper>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (_data == null) return;
        CharacterPoolData gameData = _data;

        //ItemData gameData = UIItemPanel.GetGameData(tmpItem.tid);
        ToyBox.LogMessage(_data.Uid);
        ToyBox.LogMessage(_data.UName);
        ToyBox.LogMessage(_data.PersonalTitle);
        ToyBox.LogMessage(_data.NpcType);

        GameCharacterInstance AddCharacter = GameCharacterManager.Instance.GetCharacter(_data.Uid);
        //GameCharacterInstance AddCharacter = GameCharacterManager.Instance.CreateCharacter(AllCharacterdata[tmpItem.tid]);
        //AddCharacter.SetGivenName(tmpItem.NameText.text);
        //AddCharacter.SetSurName("");

        PlayerTeamManager.Instance.AddCharacter(AddCharacter);
    }
}
