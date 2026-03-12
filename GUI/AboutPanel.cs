using GameData;
using TMPro;
using UnityEngine.UI;
using WuLin;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
internal class AboutPanel : MonoBehaviour
{
    public static AboutPanel Instance { get; private set; }

    public AboutPanel(IntPtr ptr) : base(ptr) { }

    public GameObject tips;

    public bool _initialized = false;
    private void Awake()
    {
        ToyBox.LogMessage("Awake AboutPanel！");

        Instance = this;

        tips = transform.Find("Top/Tips").gameObject;


    }

    private void OnEnable()
    {
        if (_initialized) { tips.gameObject.SetActive(false); }
        _initialized = true;
        ToyBox.LogMessage("Enable AboutPanel！");
    }

}
