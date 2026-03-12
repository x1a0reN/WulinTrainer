using GameData;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine.UI;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class ItemEntry : MonoBehaviour
{
    private ItemData _data;

    public ItemData Data {
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

    public ItemEntry(IntPtr ptr) : base(ptr) { }

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
            for (int i = 0; i < ItemPanel.Instance.Number; i++)
            {
                GameItemInstance Iteminstance = new GameItemInstance();
                Iteminstance.modifyData = ItemModifyData.CreateForgeModify(gameData, 5);
                if (Iteminstance.modifyData != null)
                {
                    foreach (var item in Iteminstance.modifyData.forgeEffect.Keys)
                    {
                        //遍历ID
                        //log.LogMessage(item);
                        foreach (ItemForgeEffectData x in ItemPanel.buffData)
                        {
                            if (x.BonusExt != null)
                            {
                                if (item == x.Uid)
                                {
                                    //log.LogMessage("UID: " + x.Uid);
                                    //log.LogMessage("UName: " + x.UName);
                                    //log.LogMessage("Key:" + x.BonusExt.key);
                                    //foreach (var item2 in x.BonusExt.dValues)
                                    //{
                                    //    log.LogMessage("Value: " + item2);
                                    //}
                                    //log.LogMessage("Max Value:" + x.BonusExt.dValues[1]);
                                    Iteminstance.modifyData.forgeEffect.Values.dictionary[item].dValues[0] = x.BonusExt.dValues[1];
                                    Iteminstance.modifyData.forgeEffect.Values.dictionary[item].values[0] = x.BonusExt.values[1];

                                    //foreach (var buffvalue in Iteminstance.modifyData.forgeEffect.Values)
                                    //{
                                    //	//KeyWithValues
                                    //	log.LogMessage(buffvalue.key);
                                    //	log.LogMessage(buffvalue.dValues[0]);
                                    //	buffvalue.dValues[0] = x.BonusExt.dValues[1];
                                    //	buffvalue.values[0] = x.BonusExt.values[1];

                                    //}
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

            PlayerTeamManager instance = MonoSingleton<PlayerTeamManager>.Instance;
            if (instance != null)
            {
                instance.PickupPack(gameItemPack, true, false, false, false);
                ToyBox.LogMessage("Equip!!!!!!!!!!!");
            }
        }
        else
        {
            GameItemPack gameItemPack = new GameItemPack();
            gameItemPack.AddItem(gameData, ItemPanel.Instance.Number, false);
            PlayerTeamManager instance = MonoSingleton<PlayerTeamManager>.Instance;
            if (instance != null)
            {
                instance.PickupPack(gameItemPack, true, false, false, false);
                ToyBox.LogMessage("Other!!!!!!!!!!!");
            }
        }

        //var pack = new GameItemPack();
        //pack.AddItem(_data, ItemPanel.Instance.Number);
        //PlayerTeamManager.Instance?.PickupPack(pack);
    }
}
