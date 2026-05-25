using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1. SINGLETON (Para acceder fácilmente desde otras clases)
    public static GameManager Instance;

    // Método Awake se llama cuando la instancia del script se carga
    private void Awake()
    {
        //PlayerPrefs.DeleteAll(); // BORRAR ESTA LINEA DESPUES DE PRUEBAS: Esta línea borra todos los datos guardados en PlayerPrefs, lo cual es útil para pruebas pero debe ser eliminada en la versión final del juego para no borrar el progreso de los jugadores.
        // Si no hay una instancia de la clase, asigna esta instancia. Si ya existe una, destruye el objeto duplicado.
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Carga el valor de energía guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 0 si no hay ninguno guardado.
        Energy = double.Parse(PlayerPrefs.GetString("PlayerEnergy", "0"));
        ClickMultiplierCost = double.Parse(PlayerPrefs.GetString("ClickMultiplierCost", "50")); // Carga el valor del costo del upgrade de multiplicador de clics guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 50 si no hay ninguno guardado.
        IdleMultiplierCost = double.Parse(PlayerPrefs.GetString("IdleMultiplierCost", "100")); // Carga el valor del costo del upgrade de multiplicador de energía pasiva guardado
        ClickMultiplierLevel = PlayerPrefs.GetInt("ClickMultiplierLevel", 0); // Carga el valor del nivel del upgrade de multiplicador de clics guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 0 si no hay ninguno guardado.
        IdleMultiplierLevel = PlayerPrefs.GetInt("IdleMultiplierLevel", 0); // Carga el valor del nivel del upgrade de multiplicador de energía pasiva guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 0 si no hay ninguno guardado.
        CurrentMechaPartIndex = PlayerPrefs.GetInt("CurrentMechaPartIndex", 0); // Carga el valor del índice de la parte del mecha actual guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 0 si no hay ninguno guardado.

        // Si el nivel del upgrade de multiplicador de energía pasiva es mayor a 0, habilitamos el multiplicador de energía pasiva y calculamos el monto a agregar al multiplicador según el nivel del upgrade.
        if (IdleMultiplierLevel > 0)
        {
            _isEnabledIdleMultiplier = true; // Si el nivel del upgrade de multiplicador de energía pasiva es mayor a 0, habilitamos el multiplicador de energía pasiva.
            _idleEnergyMultiplierAmount = Math.Pow(2.0, IdleMultiplierLevel - 1); // Calculamos el monto a agregar al multiplicador de energía pasiva según el nivel del upgrade. Cada nivel duplica el monto del upgrade anterior (1, 2, 4, 8, etc.).
            StartCoroutine(IdleEnergyCoroutine()); // Iniciamos la corrutina de energía pasiva para que comience a agregar energía cada segundo.
        }
    }

    // 2. CAMPOS (Variables y propiedades)
    [SerializeField]
    // Variable privada para almacenar la energía del jugador
    private double _energy;

    [SerializeField]
    private double _multiplier = 1.0; // Multiplicador de clics, empieza en 1 (sin bonus)

    [SerializeField]
    private double _clickMultiplierCost, _idleMultiplierCost; // Costo del upgrade de multiplicador de clics y energía pasiva, respectivamente. Se pueden ajustar para hacer el juego más o menos difícil.

    [SerializeField]
    private double _clickMultiplierCostFactor, _idleMultiplierCostFactor; // Factor por el cual se multiplicará el costo de los upgrades cada vez que se compren, para hacer que los upgrades sean progresivamente más caros.

    [SerializeField]
    private int _clickMultiplierLevel = 0, _idleMultiplierLevel = 0; // Nivel actual del upgrade de multiplicador de clics y energía pasiva, respectivamente. Se pueden usar para mostrar el nivel en la UI o para calcular el costo de los upgrades.

    [SerializeField]
    private bool _isEnabledIdleMultiplier; //Verifica si el upgrade de multiplicador de energía pasiva está habilitado o no

    [SerializeField]
    private GameObject floatingTextPrefab; // Prefab del texto flotante para mostrar la cantidad de energía ganada por segundo (idle energy)

    [SerializeField]
    private Transform canvasTransform; // Referencia al transform del canvas para instanciar el texto flotante como hijo del canvas

    [SerializeField]
    private Transform _cubeTransform; // Referencia al transform del cubo para posicionar el texto flotante sobre el cubo

    [SerializeField]
    private MechaPartData[] mechaParts; // Array para almacenar los datos de las partes del mecha, que se pueden asignar desde el inspector. Esto es útil para mostrar las partes del mecha en la UI o para usarlas en la lógica del juego.

    [SerializeField]
    private int _currentMechaPartIndex = 0; // Índice para llevar un seguimiento de la parte del mecha actual. Esto se puede usar para mostrar la parte del mecha correspondiente en la UI o para cambiar la parte del mecha que se muestra.

    [SerializeField]
    private Transform victoryPanel; // Referencia al panel de victoria para mostrarlo cuando el jugador compre todas las partes del mecha.

    private bool _isGameFinished = false; // Variable para verificar si el juego ha terminado, para evitar que se sigan comprando partes del mecha o mostrando el panel de victoria después de que el jugador haya ganado.

    // 3. EVENTOS (Las señales de radio que otras clases pueden escuchar)

    // Evento que se dispara cuando la energía cambia. Esto permite que otras clases se enteren de los cambios en la energía.
    // En particular, este evento espera un metodo que reciba un float como parametro, que representará 
    // el nuevo valor de la energía después del cambio.
    public event Action<double> OnEnergyChanged;

    // Evento que se dispara cuando se compra una parte del mecha. Esto permite que otras clases se enteren de las compras de partes del mecha.
    public event Action OnMechaPartBought;

    // 4. PROPIEDADES (Los guardias de las variables)
    // Propiedad para acceder y modificar la energía
    public double Energy
    {
        // El getter devuelve el valor actual de la energía, mientras que el setter permite modificar la energía pero
        // con una protección para evitar valores negativos.
        get { return _energy; }
        set
        {
            if (value < 0)
            {
                _energy = 0;
            }
            else
            {
                _energy = value;
            }
            PlayerPrefs.SetString("PlayerEnergy", _energy.ToString()); // Guardamos el valor de energía en PlayerPrefs cada vez que se actualiza, para persistencia.
        }
    }

    public double ClickMultiplierCost
    {
        get { return _clickMultiplierCost; }
        set
        {
            if (value < 0)
            {
                _clickMultiplierCost = 0;
            }
            else
            {
                _clickMultiplierCost = value;
            }
            PlayerPrefs.SetString("ClickMultiplierCost", _clickMultiplierCost.ToString()); // Guardamos el valor del costo del upgrade de multiplicador de clics en PlayerPrefs cada vez que se actualiza, para persistencia.
        }
    }

    public double IdleMultiplierCost
    {
        get { return _idleMultiplierCost; }
        set
        {
            if (value < 0)
            {
                _idleMultiplierCost = 0;
            }
            else
            {
                _idleMultiplierCost = value;
            }
            PlayerPrefs.SetString("IdleMultiplierCost", _idleMultiplierCost.ToString()); // Guardamos el valor del costo del upgrade de multiplicador de energía pasiva en PlayerPrefs cada vez que se actualiza, para persistencia.
        }
    }

    public int ClickMultiplierLevel
    {
        get { return _clickMultiplierLevel; }
        set
        {
            if (value < 0)
            {
                _clickMultiplierLevel = 0;
            }
            else
            {
                _clickMultiplierLevel = value;
            }
            PlayerPrefs.SetInt("ClickMultiplierLevel", _clickMultiplierLevel); // Guardamos el valor del nivel del upgrade de multiplicador de clics en PlayerPrefs cada vez que se actualiza, para persistencia.
        }
    }

    public int IdleMultiplierLevel
    {
        get { return _idleMultiplierLevel; }
        set
        {
            if (value < 0)
            {
                _idleMultiplierLevel = 0;
            }
            else
            {
                _idleMultiplierLevel = value;
            }
            PlayerPrefs.SetInt("IdleMultiplierLevel", _idleMultiplierLevel); // Guardamos el valor del nivel del upgrade de multiplicador de energía pasiva en PlayerPrefs cada vez que se actualiza, para persistencia.
        }
    }

    public int CurrentMechaPartIndex
    {
        get { return _currentMechaPartIndex; }
        set
        {
            if (value < 0)
            {
                _currentMechaPartIndex = 0;
            }
            else
            {
                _currentMechaPartIndex = value;
            }
            PlayerPrefs.SetInt("CurrentMechaPartIndex", _currentMechaPartIndex); // Guardamos el valor del índice de la parte del mecha actual en PlayerPrefs cada vez que se actualiza, para persistencia.
        }
    }

    public bool IsGameFinished
    {
        get { return _isGameFinished; }
        set { _isGameFinished = value; }
    }

    // 5. MÉTODOS (Las acciones que puede realizar la clase/ la logic de negocio)

    // Metodo que cambia la energía del jugador. 
    // El metodo cumple con la firma del evento, ya que recibe un float (el nuevo valor de energía) y no devuelve nada (void).
    public void AddEnergy(double amount)
    {
        // A. Modificamos el valor (usando la Property para que proteja de negativos)
        Energy += amount * _multiplier;

        // B. Gritamos el cambio a los oyentes
        // El signo "?" revisa si alguien está escuchando. Si nadie escucha, no hace nada (evita errores).
        // EL Invoke(Energy) es lo que realmente dispara el evento, y le pasa el valor actual de la energía a los oyentes.
        // En este caso, el oyente es el UIManager, que se suscribió al evento y tiene un método que se llama UpdateEnergyUI, 
        // el cual recibe el valor de energía para actualizar la UI. El event constantemente revisa si el valor
        // de energía ha cambiado, y si es así, llama a UpdateEnergyUI con el nuevo valor de energía.
        OnEnergyChanged?.Invoke(Energy);

        ShowFloatingText(amount * _multiplier); // Llamamos al método para mostrar el texto flotante cada vez que se agrega energía (ya sea por clics o por energía pasiva).

        // C. (Opcional) Un log para nosotros mismos
        //Debug.Log($"[GameManager] Energía actual: {Energy}");
    }

    // Metodo para aplicar el upgrade de multiplicador de clics. Recibe el monto a agregar al multiplicador y el costo del upgrade.
    // Devuelve un booleano para indicar si el upgrade se aplicó correctamente (true) o no (false, por falta de energía).
    public bool AddMultiplier(double amountToAdd)
    {
        if (Energy >= ClickMultiplierCost)
        {
            Energy -= ClickMultiplierCost;
            _multiplier *= amountToAdd;
            ClickMultiplierCost *= _clickMultiplierCostFactor; // Aumentamos el costo del upgrade para la siguiente compra, multiplicándolo por 1.15 (puedes ajustar este valor para hacer el juego más o menos difícil).
            ClickMultiplierLevel++; // Aumentamos el nivel del upgrade de multiplicador de clics para mostrarlo en la UI o para calcular el costo de los upgrades.

            // Después de modificar la energía, también debemos notificar a los oyentes del cambio, 
            // ya que la energía se ha reducido debido al costo del upgrade.
            // El "?" asegura que solo se intente invocar el evento si hay oyentes suscritos, evitando errores si no hay ninguno.
            OnEnergyChanged?.Invoke(Energy);

            // Devuelve true para indicar que el upgrade se aplicó correctamente.
            return true;
        }
        // Si no hay suficiente energía para pagar el costo del upgrade, no se aplica y se devuelve false.
        return false;
    }

    [SerializeField] double _idleEnergyMultiplierAmount = 1.0; // El monto a agregar al multiplicador de energía pasiva (idle energy) por cada upgrade comprado.
    public bool AddIdleMutliplier(double amountToAdd)
    {
        // Este método es similar a AddMultiplier, pero se utiliza para aplicar el upgrade de multiplicador de energía pasiva (idle energy).
        // Se verifica si el jugador tiene suficiente energía para pagar el costo del upgrade.
        if (Energy >= IdleMultiplierCost)
        {
            // Si hay suficiente energía, se resta el costo del upgrade de la energía actual.
            Energy -= IdleMultiplierCost;
            IdleMultiplierCost *= _idleMultiplierCostFactor; // Aumentamos el costo del upgrade para la siguiente compra, multiplicándolo por 1.15 (puedes ajustar este valor para hacer el juego más o menos difícil).
            IdleMultiplierLevel++; // Aumentamos el nivel del upgrade de multiplicador de energía pasiva para mostrarlo en la UI o para calcular el costo de los upgrades.

            if (!_isEnabledIdleMultiplier)
            {
                // Si el upgrade de multiplicador de energía pasiva no está habilitado, lo habilitamos y establecemos el monto a agregar al multiplicador.
                _isEnabledIdleMultiplier = true;

                // Iniciamos la corrutina que se encargará de agregar energía pasiva cada segundo, multiplicada por el monto del upgrade.
                StartCoroutine(IdleEnergyCoroutine());
            }
            else
            {
                // Si el upgrade de multiplicador de energía pasiva ya está habilitado, simplemente aumentamos el monto a agregar al multiplicador.
                _idleEnergyMultiplierAmount *= 2.0;
            }

            // Después de modificar la energía, también debemos notificar a los oyentes del cambio, 
            // ya que la energía se ha reducido debido al costo del upgrade.
            // El "?" asegura que solo se intente invocar el evento si hay oyentes suscritos, evitando errores si no hay ninguno.
            OnEnergyChanged?.Invoke(Energy);
            Debug.Log("Idle Multiplier Upgrade Applied! " + amountToAdd);

            // Devuelve true para indicar que el upgrade se aplicó correctamente.
            return true;

        }
        else
        {
            // Devuelve false para indicar que el upgrade no se aplicó debido a la falta de energía.
            return false;
        }
    }

    // Metodo para comprar la parte del mecha actual. Verifica si el jugador tiene suficiente energía para comprar la parte del mecha, 
    // y si es así, resta el costo de la energía, avanza al siguiente índice de parte del mecha y notifica a los oyentes del cambio de energía.
    public void BuyMechaPart()
    {
        // Primero verificamos si el índice de la parte del mecha actual es mayor o igual al número total de partes del mecha disponibles, 
        // para evitar errores de índice fuera de rango y para saber si el jugador ya ha comprado todas las partes del mecha.
        if (CurrentMechaPartIndex >= mechaParts.Length)
        {
            // El return aquí es importante para asegurarnos de que el método se detenga y no intente acceder a una parte del mecha 
            // que no existe, lo cual causaría un error.
            return;
        }

        // Si el jugador no ha comprado todas las partes del mecha, verificamos si tiene suficiente energía 
        // para comprar la parte del mecha actual.
        if (_energy >= mechaParts[CurrentMechaPartIndex].cost)
        {
            Energy -= mechaParts[CurrentMechaPartIndex].cost; // Si el jugador tiene suficiente energía, se resta el costo de la parte del mecha de la energía actual.
            OnEnergyChanged?.Invoke(Energy); // Notificamos a los oyentes del cambio de energía después de comprar la parte del mecha. El "?" asegura que solo se intente invocar el evento si hay oyentes suscritos, evitando errores si no hay ninguno.
            CurrentMechaPartIndex++; // Avanzamos al siguiente índice para la próxima parte del mecha.
            OnMechaPartBought?.Invoke(); // Notificamos a los oyentes que se ha comprado una parte del mecha, para que puedan actualizar la UI o realizar otras acciones relacionadas con la compra de partes del mecha.
        }

        if (CurrentMechaPartIndex >= mechaParts.Length)
        {
            // Si el jugador ya ha comprado todas las partes del mecha, mostramos un mensaje de felicitaciones y terminamos el método 
            // sin hacer nada más.
            victoryPanel.gameObject.SetActive(true); // Activamos el panel de victoria para mostrarlo al jugador.

            _isGameFinished = true; // Marcamos el juego como terminado.
            PlayerPrefs.DeleteAll(); // Limpiamos los datos guardados en PlayerPrefs para que el jugador pueda empezar de nuevo si quiere jugar otra vez después de ganar.

            ShowVictoryPanel(); // Llamamos al método para mostrar el panel de victoria con el efecto de fade-in.

            // El return aquí es importante para asegurarnos de que el método se detenga y no intente acceder a una parte del mecha 
            // que no existe, lo cual causaría un error.
        }
    }

    // Método para mostrar el texto flotante de energía para ser llamado desde el AddEnergy y desde la corrutina de energía pasiva (idle energy).
    private void ShowFloatingText(double amount)
    {
        // Convierte la posición del cubo en el mundo a una posición en la pantalla (coordenadas de píxeles) para colocar el texto flotante correctamente.
        var cubePos = Camera.main.WorldToScreenPoint(_cubeTransform.position);

        // Se crea una variable para almacenar la instancia del texto flotante que se va a crear. 
        // Se utiliza el método Instantiate para crear una nueva instancia del prefab de texto flotante en la posición calculada (cubePos)
        // y con una rotación por defecto (Quaternion.identity). Además, se establece el canvas como el padre del texto flotante para que
        // se renderice correctamente en la UI.
        var floatingText = Instantiate(floatingTextPrefab, cubePos, Quaternion.identity, canvasTransform);

        // Llama al método SetText del script FloatingText para establecer el texto que mostrará el monto de energía ganada.
        floatingText.GetComponent<FloatingText>().SetText("+" + UIManager.Instance.EnergyAmountFormatter(amount));
    }

    // Este método arranca la corrutina. Lo llamas UNA vez cuando el juego termina.
    public void ShowVictoryPanel()
    {
        StartCoroutine(FadeInPanel());
    }

    // La corrutina: hace el fade poquito a poquito, frame por frame.
    IEnumerator FadeInPanel()
    {
        // Agarramos el componente Image del panel para poder tocarle el color
        var panelImage = victoryPanel.GetComponent<UnityEngine.UI.Image>();

        var panelText = victoryPanel.GetComponentsInChildren<UnityEngine.UI.Graphic>();

        // El color actual del panel (negro con alpha en 0 = invisible)
        Color color = panelImage.color;

        // Mientras el alpha sea menor que nuestro objetivo (0.86 = 220/255)...
        while (color.a < 0.9f)
        {
            // ...le sumamos un poquito al alpha cada frame.
            // Time.deltaTime hace que suba suavecito, no de golpe.
            color.a += Time.deltaTime * 0.5f; // el 0.5 es la velocidad del fade

            // Le devolvemos el color modificado al panel
            panelImage.color = color;

            // Bajamos el volumen del BGM al mismo ritmo
            AudioManager.Instance.GetBGMAudioSource().volume -= Time.deltaTime * 0.15f;

            foreach(var text in panelText)
            {
                Color textColor = text.color;
                textColor.a += Time.deltaTime * 0.5f;
                text.color = textColor;
            }

            // "yield return null" = pausar aquí, esperar al siguiente frame, 
            // y volver a entrar al while desde arriba.
            // SIN ESTO, el while se ejecuta completo en UN frame = no hay animación.
            yield return null;
        }

        // Cuando sale del while, el fade terminó. 
        // Aquí podrías activar el texto de victoria, por ejemplo.
    }

    private IEnumerator IdleEnergyCoroutine()
    {
        while (_isEnabledIdleMultiplier && !_isGameFinished) // La corrutina seguirá ejecutándose mientras el upgrade de multiplicador de energía pasiva esté habilitado y el juego no haya terminado.
        {
            yield return new WaitForSeconds(1f); // Espera 1 segundo
            Energy += _idleEnergyMultiplierAmount; // Agrega 1 de energía cada segundo
            OnEnergyChanged?.Invoke(Energy); // Notifica a los oyentes del cambio de energía
            ShowFloatingText(_idleEnergyMultiplierAmount); // Muestra el texto flotante cada vez que se agrega energía pasiva    
        }
    }
}



