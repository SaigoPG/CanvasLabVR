using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderToTMPText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    private void Start()
    {
        // Actualiza el texto inmediatamente al iniciar
        UpdateText(slider.value);

        // Suscribe el método al evento del slider
        slider.onValueChanged.AddListener(UpdateText);
    }

    private void UpdateText(float value)
    {
        // Convierte el valor del slider a texto
        valueText.text = value.ToString("0"); // Sin decimales
        // Si deseas decimales: value.ToString("0.0")
    }
}