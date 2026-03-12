using GameData;
using TMPro;
using WuLin;
using HaxxToyBox.Config;
using System;
using UniverseLib;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class DlcMiscPanel : MonoBehaviour
{
    public static DlcMiscPanel Instance { get; private set; }

    private TMP_InputField SLGCoinInput;
    private TMP_InputField SLGTributeInput;
    private TMP_InputField SLGFoodInput;
    private TMP_InputField SLGMineInput;
    private TMP_InputField SLGDrugInput;
    private TMP_InputField SLGWoodInput;


    public DlcMiscPanel(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        Instance = this;

        SLGCoinInput = transform.Find("InputFunc/SLGCoin/NumInput").GetComponent<TMP_InputField>();
        SLGCoinInput.onValueChanged.RemoveAllListeners();
        SLGCoinInput.onValueChanged.AddListener((string input) => {
            if (!long.TryParse(input, out long value))
                SLGCoinInput.text = SLGCoinInput.m_OriginalText;
            else
            {
                Dlc1SlgManager.Instance.factionInAction.backPack.ChangeCurrency(CurrencyType.SLGCoin, value);
            }
        });

        SLGTributeInput = transform.Find("InputFunc/SLGTribute/NumInput").GetComponent<TMP_InputField>();
        SLGTributeInput.onValueChanged.RemoveAllListeners();
        SLGTributeInput.onValueChanged.AddListener((string input) => {
            if (!long.TryParse(input, out long value))
                SLGTributeInput.text = SLGTributeInput.m_OriginalText;
            else
            {
                Dlc1SlgManager.Instance.factionInAction.backPack.ChangeCurrency(CurrencyType.SLGTribute, value);
            }
        });

        SLGFoodInput = transform.Find("InputFunc/SLGFood/NumInput").GetComponent<TMP_InputField>();
        SLGFoodInput.onValueChanged.RemoveAllListeners();
        SLGFoodInput.onValueChanged.AddListener((string input) =>
        {
            if (!long.TryParse(input, out long value))
                SLGFoodInput.text = SLGFoodInput.m_OriginalText;
            else
            {
                Dlc1SlgManager.Instance.factionInAction.backPack.ChangeCurrency(CurrencyType.SLGFood, value);
            }
        });

        SLGMineInput = transform.Find("InputFunc/SLGMine/NumInput").GetComponent<TMP_InputField>();
        SLGMineInput.onValueChanged.RemoveAllListeners();
        SLGMineInput.onValueChanged.AddListener((string input) =>
        {
            if (!long.TryParse(input, out long value))
                SLGMineInput.text = SLGMineInput.m_OriginalText;
            else
            {
                Dlc1SlgManager.Instance.factionInAction.backPack.ChangeCurrency(CurrencyType.SLGMine, value);
            }
        });

        SLGDrugInput = transform.Find("InputFunc/SLGDrug/NumInput").GetComponent<TMP_InputField>();
        SLGDrugInput.onValueChanged.RemoveAllListeners();
        SLGDrugInput.onValueChanged.AddListener((string input) =>
        {
            if (!long.TryParse(input, out long value))
                SLGDrugInput.text = SLGDrugInput.m_OriginalText;
            else
            {
                Dlc1SlgManager.Instance.factionInAction.backPack.ChangeCurrency(CurrencyType.SLGDrug, value);
            }
        });

        SLGWoodInput = transform.Find("InputFunc/SLGWood/NumInput").GetComponent<TMP_InputField>();
        SLGWoodInput.onValueChanged.RemoveAllListeners();
        SLGWoodInput.onValueChanged.AddListener((string input) =>
        {
            if (!long.TryParse(input, out long value))
                SLGWoodInput.text = SLGWoodInput.m_OriginalText;
            else
            {
                Dlc1SlgManager.Instance.factionInAction.backPack.ChangeCurrency(CurrencyType.SLGWood, value);
            }
        });


    }

    private void OnEnable()
    {
        var FactionBack = Dlc1SlgManager.Instance.factionInAction.backPack;

        SLGCoinInput.SetTextWithoutNotify(FactionBack.GetCurrency(CurrencyType.SLGCoin).ToString());
        SLGTributeInput.SetTextWithoutNotify(FactionBack.GetCurrency(CurrencyType.SLGTribute).ToString());
        SLGFoodInput.SetTextWithoutNotify(FactionBack.GetCurrency(CurrencyType.SLGFood).ToString());
        SLGMineInput.SetTextWithoutNotify(FactionBack.GetCurrency(CurrencyType.SLGMine).ToString());
        SLGDrugInput.SetTextWithoutNotify(FactionBack.GetCurrency(CurrencyType.SLGDrug).ToString());
        SLGWoodInput.SetTextWithoutNotify(FactionBack.GetCurrency(CurrencyType.SLGWood).ToString());
    }

    public static void RecoverAll()
    {
        var teamManager = PlayerTeamManager.Instance;
        if (teamManager == null) return;
        teamManager.ModifyProp("队伍体力", 100);
        teamManager.ModifyProp("队伍心情", 100);
        for (int i = 0; i < teamManager.TeamSize; i++)
        {
            teamManager.GetTeamMemberByIndex(i).FullyRecover();
        }
    }
}
