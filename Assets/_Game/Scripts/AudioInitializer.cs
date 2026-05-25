using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(var button in FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None))
        {
            button.onClick.AddListener(() => AudioManager.Instance.PlayClickSFX());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
