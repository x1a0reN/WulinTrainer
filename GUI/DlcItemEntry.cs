using GameData;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine.UI;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class DlcItemEntry : MonoBehaviour
{
    private ItemData _data;

    public ItemData Data
    {
        get => _data;
        set
        {
            if (value == _data || value == null) return;

            _data = value;
            _nameText.text = _data.GetName(true);
            _icon.sprite = _data.GetIcon();
        }
    }

    private Button _button;
    private Image _icon;
    private TextMeshProUGUI _nameText;

    public DlcItemEntry(IntPtr ptr) : base(ptr) { }

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
        ItemData gameData = _data;
        ToyBox.LogMessage("Type: " + gameData.Type);
        if (gameData.Type < ItemType.Equip && gameData.Type != ItemType.Equip_Weapon_None)
        {
            GameItemPack gameItemPack = new GameItemPack();
            for (int i = 0; i < DlcItemPanel.Instance.Number; i++)
            {
                GameItemInstance Iteminstance = new GameItemInstance();
                Iteminstance.modifyData = ItemModifyData.CreateForgeModify(gameData, 5);
                if (Iteminstance.modifyData != null)
                {
                    foreach (var item in Iteminstance.modifyData.forgeEffect.Keys)
                    {
                        //遍历ID
                        foreach (ItemForgeEffectData x in DlcItemPanel.buffData)
                        {
                            if (x.BonusExt != null)
                            {
                                if (item == x.Uid)
                                {
                                    Iteminstance.modifyData.forgeEffect.Values.dictionary[item].dValues[0] = x.BonusExt.dValues[1];
                                    Iteminstance.modifyData.forgeEffect.Values.dictionary[item].values[0] = x.BonusExt.values[1];
                                }
                            }
                        }
                    }
                }

                Iteminstance.m_stack = 1;
                Iteminstance.m_templeteId = gameData.Uid;
                Iteminstance._IsNew_k__BackingField = true;
                Iteminstance._IsForbidden_k__BackingField = false;
                ToyBox.LogMessage("Iteminstance: " + Iteminstance.m_templeteId + " | ");
                gameItemPack.AddItem(Iteminstance);
            }

            Dlc1SlgManager.Instance.factionInAction.PickupPack(gameItemPack,true);
            ToyBox.LogMessage("Equip!!!!!!!!!!!");
        }
        else
        {
            GameItemPack gameItemPack = new GameItemPack();
            gameItemPack.AddItem(gameData, DlcItemPanel.Instance.Number, false);
            Dlc1SlgManager.Instance.factionInAction.PickupPack(gameItemPack, true);
            ToyBox.LogMessage("Other!!!!!!!!!!!");
        }
    }
}
