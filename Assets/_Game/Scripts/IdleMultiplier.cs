using TMPro;
using UnityEngine;

public class IdleMultiplier : MonoBehaviour, IUpgrade
{
    [field: SerializeField] public string Name {get; private set;}
    [field: SerializeField] public string Description {get; private set;}

    [SerializeField] private float _idleMultiplierAmount; // El factor por el cual se multiplicará la energía pasiva
    [SerializeField] private TextMeshProUGUI _buttonTextCost; // Referencia al texto del botón para mostrar el nombre y el costo

    public void ApplyUpgrade()
    {
        // Metodo para aplicar el upgrade de multiplicador de energía pasiva
        // para aumentar el multiplicador de energía pasiva en el juego.
        bool upgradeApplied = GameManager.Instance.AddIdleMutliplier(_idleMultiplierAmount);
        
        // El bool upgradeApplied se utiliza para verificar si el upgrade se aplicó correctamente, es decir, si el jugador tenía suficiente energía para comprarlo.
        if(upgradeApplied)
        {
            // Si el upgrade se aplicó correctamente, aumentamos el costo del upgrade para la siguiente compra.
            _buttonTextCost.text = "Idle Level: " + GameManager.Instance.IdleMultiplierLevel.ToString() + " - Cost: " + CostAmountFormatter(GameManager.Instance.IdleMultiplierCost);
            Debug.Log("Idle Multiplier Upgrade Applied!" + GameManager.Instance.IdleMultiplierCost);
        }
        else
        {
            // Si no hay suficiente energía para pagar el costo del upgrade, no se aplica y se puede mostrar un mensaje de error o simplemente no hacer nada.
            Debug.Log("Not enough energy to apply Idle Multiplier Upgrade.");
        }
    }

    void Start()
    {
        _buttonTextCost.text = "Idle Level: " + GameManager.Instance.IdleMultiplierLevel.ToString() + " - Cost: " + CostAmountFormatter(GameManager.Instance.IdleMultiplierCost);
    }

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
