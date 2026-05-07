using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MechaPartButton : MonoBehaviour
{
    [SerializeField] private int _partIndex;
    [SerializeField] private MechaPartData _partData;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private void OnEnable()
    {


    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMechaPartBought -= UpdateVisualState;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMechaPartBought += UpdateVisualState;
            Debug.Log("Suscrito al evento OnMechaPartBought en MechaPartButton");
        }
        else
        {
            Debug.LogWarning("GameManager.Instance es null en MechaPartButton. No se pudo suscribir al evento OnMechaPartBought.");
        }
        _buttonText.text = _partData.partName + "\n" + UIManager.Instance.EnergyAmountFormatter(_partData.cost) + " Credits";
        UpdateVisualState();
    }

    void UpdateVisualState()
    {
        if (_partIndex < GameManager.Instance.CurrentMechaPartIndex)
        {
            // Parte ya equipada
            _buttonText.text = _partData.partName + "\nComprado";
            _buttonText.color = Color.black;
            GetComponent<UnityEngine.UI.Image>().color = Color.lightGreen;

            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false; // Desactivamos el botón para que no se pueda interactuar con él, ya que la parte ya está equipada.
        }
        else if (_partIndex == GameManager.Instance.CurrentMechaPartIndex)
        {
            // Parte actual
            _buttonText.text = _partData.partName + "\n" + UIManager.Instance.EnergyAmountFormatter(_partData.cost) + " Credits";
            //_buttonText.color = Color.green;
            //GetComponent<UnityEngine.UI.Image>().color = Color.black;
        }
        else
        {
            // Parte bloqueada
            _buttonText.text = _partData.partName + "\n" + UIManager.Instance.EnergyAmountFormatter(_partData.cost) + " Credits";
            //_buttonText.color = Color.red;
            //GetComponent<UnityEngine.UI.Image>().color = Color.black;
        }
    }
}
