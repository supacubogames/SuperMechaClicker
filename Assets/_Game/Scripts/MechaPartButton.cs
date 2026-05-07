using System;
using Unity.VisualScripting;
using UnityEngine;

public class MechaPartButton : MonoBehaviour
{
    [SerializeField] private int _partIndex;
    [SerializeField] private MechaPartData _partData;
    [SerializeField] private TMPro.TMP_Text _buttonText;

    private void OnEnable()
    {
        Debug.Log("OnEnable de MechaPartButton ejecutado");

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
            GetComponent<UnityEngine.UI.Image>().color = Color.green;
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false; // Desactivamos el botón para que no se pueda interactuar con él, ya que la parte ya está equipada.
        }
        else if (_partIndex == GameManager.Instance.CurrentMechaPartIndex)
        {
            // Parte actual
            _buttonText.text = _partData.partName + "\n" + UIManager.Instance.EnergyAmountFormatter(_partData.cost) + " Credits";
            GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            // Parte bloqueada
            _buttonText.text = _partData.partName + "\n" + UIManager.Instance.EnergyAmountFormatter(_partData.cost) + " Credits";
            GetComponent<UnityEngine.UI.Image>().color = Color.gray;
        }
    }
}
