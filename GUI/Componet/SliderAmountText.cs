using TMPro;

namespace HaxxToyBox.GUI;

[RegisterInIl2Cpp]
public class SliderAmountText : MonoBehaviour
{
    private Slider _slider;
    private Text _text;

    public string Suffix;
    public bool WholeNumber = true;
    
    public SliderAmountText(IntPtr ptr) : base(ptr) { }

    private void Awake()
    {
        _text = GetComponent<Text>();
        _slider = transform.parent.GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        SetAmountText(_slider.value);
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        SetAmountText(value);
    }

    private void SetAmountText(float value)
    {
        ToyBox.LogMessage("Slider Value: " + value);
        if (WholeNumber)
            _text.text = $"{(int)value}{Suffix}";
        else
            _text.text = $"{Math.Round(value, 2)}{Suffix}";
    }
}
