using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager Instance;
    [SerializeField] private AudioClip _clickSFX; // Referencia al AudioClip para el sonido de clic
    [SerializeField] private AudioSource _audioSource; // Referencia al AudioSource para reproducir los sonidos

    void Awake()
    {
        // Se asegura de que solo haya una instancia del AudioManager
        if (Instance == null) // La condicion revisa si la Instancia es nula. Su se cumple, pasa al bloque de codigo dentro del if.
        {
            Instance = this; // Asigna "Instance" a la instancia actual del AudioManager "this", que se refiere al objeto que contiene este script.
            DontDestroyOnLoad(gameObject); // DontDestroyOnLoad es un método de Unity que evita que el objeto al que está adjunto sea destruido al
            //  cargar una nueva escena. Esto es útil para mantener el AudioManager activo a lo largo de toda la vida del juego, incluso cuando se cambian las escenas.
        }
        else // Si ya existe una instancia del AudioManager, se destruye el objeto actual para evitar duplicados.
        {
            Destroy(gameObject); // Destruye el objeto al que está adjunto este script, ya que no se permite tener más de una instancia 
            // del AudioManager en el juego.
        }
    }

    public void PlayClickSFX()
    {
        // Reproduce el sonido de clic utilizando el AudioSource del objeto al que está adjunto este script.
        // El método PlayOneShot reproduce un clip de audio sin interrumpir el audio que ya se esté reproduciendo.
        _audioSource.PlayOneShot(_clickSFX);
    }
}
