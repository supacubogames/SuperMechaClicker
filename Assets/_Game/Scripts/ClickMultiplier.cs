using TMPro;
using UnityEngine;

public class ClickMultiplier : MonoBehaviour, IUpgrade
{   
    [field: SerializeField] public string Name {get; private set;}
    [field: SerializeField] public string Description {get; private set;}

    [SerializeField] private float _multiplierAmount; // El factor por el cual se multiplicarán los clics
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
            _buttonTextCost.text = "Level: " + GameManager.Instance.ClickMultiplierLevel.ToString() + " - Cost: " + CostAmountFormatter(GameManager.Instance.ClickMultiplierCost); // Actualizamos el texto del botón con el nuevo costo formateado
            Debug.Log("Click Multiplier Upgrade Applied!");
        }
        else
        {
            Debug.Log("Not enough energy to apply Click Multiplier Upgrade.");
        }
    }

    void Start()
    {
        _buttonTextCost.text = "Multiplier Level: " + GameManager.Instance.ClickMultiplierLevel.ToString() + " - Cost: " + CostAmountFormatter(GameManager.Instance.ClickMultiplierCost); // Inicializamos el texto del botón con el costo formateado al iniciar.
    }

    // Método para formatear el costo del upgrade, similar al formateo de energía en el UIManager, pero adaptado para costos.
    private string CostAmountFormatter(double cost)
    {
        if(cost >= 1000000000000000000000000000000000000000000d)
        {
            return (cost / 1000000000000000000000000000000000000000000000d).ToString("F2") + "Vg";
        }
        else if(cost >= 1000000000000000000000000000000000000f)
        {
            return (cost / 1000000000000000000000000000000000000f).ToString("F2") + "U";
        }
        else if(cost >= 1000000000000000000000000000000f)
        {
            return (cost / 1000000000000000000000000000000f).ToString("F2") + "No";
        }  
        else if(cost >= 1000000000000000000000000000f)
        {
            return (cost / 1000000000000000000000000000f).ToString("F2") + "Oc";
        }
        else if(cost >= 1000000000000000000000000f)
        {
            return (cost / 1000000000000000000000000f).ToString("F2") + "Sp";
        }
        else if(cost >= 1000000000000000000000f)
        {
            return (cost / 1000000000000000000000f).ToString("F2") + "Sx";
        }   
        else if(cost >= 1000000000000000000f)
        {
            return (cost / 1000000000000000000f).ToString("F2") + "Qi";
        }

        else if(cost >= 1000000000000000f)
        {
            return (cost / 1000000000000000f).ToString("F2") + "Qa";
        }
        else if(cost >= 1000000000000f)
        {
            return (cost / 1000000000000f).ToString("F2") + "T";
        }
        else if(cost >= 1000000000f)
        {
            return (cost / 1000000000f).ToString("F2") + "B";
        }
        else if(cost >= 1000000f)
        {
            return (cost / 1000000f).ToString("F2") + "M";
        }
        else if(cost >= 1000f)
        {
            return (cost / 1000f).ToString("F2") + "K";
        }
        else
        {
            return cost.ToString("F0");
        }
    }
}
