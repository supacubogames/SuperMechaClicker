using System.Collections;
using UnityEngine;

public class ClickerButton : MonoBehaviour
{
    [SerializeField] private ParticleSystem _sparks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        GameManager.Instance.AddEnergy(1);

        // Cambiamos la escala del botón para dar una sensación de "presionado".
        gameObject.transform.localScale = new Vector3(3f, 3f);
        _sparks.Play();
    }

    void OnMouseUp()
    {
        // Volvemos a la escala original del botón cuando se suelta el mouse.
        //gameObject.transform.localScale = new Vector3(2f, 2f);
        StartCoroutine(ButtonAnimation());
    }

    IEnumerator ButtonAnimation()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 25; // Ajusta la velocidad de la animación aquí
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(2f, 2f), t);
        yield return null;
        }
    }

}
