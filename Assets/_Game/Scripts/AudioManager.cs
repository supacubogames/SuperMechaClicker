using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager Instance;
    [SerializeField] private AudioClip _clickSFX; // Referencia al AudioClip para el sonido de clic
    [SerializeField] private AudioSource _audioSourceBGM; // Referencia al AudioSource para reproducir la música de fondo
    [SerializeField] private AudioSource _audioSourceSFX; // Referencia al AudioSource para reproducir los sonidos
    [SerializeField] private AudioClip _BGM; // Referencia al AudioClip para la música de fondo

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

    void Start()
    {
        foreach(var button in FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None))
        {
            button.onClick.AddListener(() => PlayClickSFX());
        }

        PlayBGM(); // Llama al método para reproducir la música de fondo al iniciar el juego.
    }

    public void PlayClickSFX()
    {
        // Reproduce el sonido de clic utilizando el AudioSource del objeto al que está adjunto este script.
        // El método PlayOneShot reproduce un clip de audio sin interrumpir el audio que ya se esté reproduciendo.
        _audioSourceSFX.PlayOneShot(_clickSFX);
    }

    public void PlayBGM()
    {
        _audioSourceBGM.clip = _BGM; // Asigna el clip de música de fondo al AudioSource.
        _audioSourceBGM.loop = true; // Configura el AudioSource para que la música de fondo se repita en loop.
        _audioSourceBGM.Play(); // Reproduce la música de fondo.
    }

    public AudioSource GetBGMAudioSource()
    {
        return _audioSourceBGM; // Devuelve la referencia al AudioSource que reproduce la música de fondo.
    }

}
