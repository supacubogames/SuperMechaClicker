using TMPro;
using UnityEngine;

public class ClickMultiplier : MonoBehaviour, IUpgrade
{   
    [field: SerializeField] public string Name {get; private set;}
    [field: SerializeField] public string Description {get; private set;}

    [SerializeField] private double _multiplierAmount; // El factor por el cual se multiplicarán los clics
    [SerializeField] private TextMeshProUGUI _buttonTextCost; // Referencia al texto del botón para mostrar el nombre y el costo

    public void ApplyUpgrade()
    {
        // Metodo para aplicar el upgrade de multiplicador de clics
        // para aumentar el multiplicador de clics en el juego.
        bool upgradeApplied = GameManager.Instance.AddMultiplier(_multiplierAmount);

        // El bool upgradeApplied se utiliza para verificar si el upgrade se aplicó correctamente, es decir, si el jugador tenía suficiente energía para comprarlo.
        if(upgradeApplied)
        {
            // Si el upgrade se aplicó correctamente, aumentamos el costo del upgrade para la siguiente compra.
            _buttonTextCost.text = "Level: " + GameManager.Instance.ClickMultiplierLevel.ToString() + " - Cost: " + UIManager.Instance.EnergyAmountFormatter(GameManager.Instance.ClickMultiplierCost); // Actualizamos el texto del botón con el nuevo costo formateado
            Debug.Log("Click Multiplier Upgrade Applied!");
        }
        else
        {
            Debug.Log("Not enough energy to apply Click Multiplier Upgrade.");
        }
    }

    void Start()
    {
        _buttonTextCost.text = "Multiplier Level: " + GameManager.Instance.ClickMultiplierLevel.ToString() + " - Cost: " + UIManager.Instance.EnergyAmountFormatter(GameManager.Instance.ClickMultiplierCost); // Inicializamos el texto del botón con el costo formateado al iniciar.
    }
        
}
