using System;
using System.Collections;
using Unity.VisualScripting;
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
        ClickMultiplierCost = double.Parse(PlayerPrefs.GetString("ClickMultiplierCost", "1.15")); // Carga el valor del costo del upgrade de multiplicador de clics guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 1.15 si no hay ninguno guardado.
        IdleMultiplierCost = double.Parse(PlayerPrefs.GetString("IdleMultiplierCost", "1.15")); // Carga el valor del costo del upgrade de multiplicador de energía pasiva guardado
        ClickMultiplierLevel = PlayerPrefs.GetInt("ClickMultiplierLevel", 0); // Carga el valor del nivel del upgrade de multiplicador de clics guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 0 si no hay ninguno guardado.
        IdleMultiplierLevel = PlayerPrefs.GetInt("IdleMultiplierLevel", 0); // Carga el valor del nivel del upgrade de multiplicador de energía pasiva guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 0 si no hay ninguno guardado.
        CurrentMechaPartIndex = PlayerPrefs.GetInt("CurrentMechaPartIndex", 0); // Carga el valor del índice de la parte del mecha actual guardado en PlayerPrefs al iniciar el juego, con un valor por defecto de 0 si no hay ninguno guardado.
    
        // Si el nivel del upgrade de multiplicador de energía pasiva es mayor a 0, habilitamos el multiplicador de energía pasiva y calculamos el monto a agregar al multiplicador según el nivel del upgrade.
        if(IdleMultiplierLevel > 0)
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
    private double _clickMultiplierCost = 1.15d, _idleMultiplierCost = 1.15d; // Costo del upgrade de multiplicador de clics y energía pasiva, respectivamente. Se pueden ajustar para hacer el juego más o menos difícil.

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
    // 3. EVENTOS (Las señales de radio que otras clases pueden escuchar)

    [SerializeField]
    private int _currentMechaPartIndex = 0; // Índice para llevar un seguimiento de la parte del mecha actual. Esto se puede usar para mostrar la parte del mecha correspondiente en la UI o para cambiar la parte del mecha que se muestra.
    
    // Evento que se dispara cuando la energía cambia. Esto permite que otras clases se enteren de los cambios en la energía.
    // En particular, este evento espera un metodo que reciba un float como parametro, que representará 
    // el nuevo valor de la energía después del cambio.
    public event Action<double> OnEnergyChanged;

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
        Debug.Log($"[GameManager] Energía actual: {Energy}");
    }

    // Metodo para aplicar el upgrade de multiplicador de clics. Recibe el monto a agregar al multiplicador y el costo del upgrade.
    // Devuelve un booleano para indicar si el upgrade se aplicó correctamente (true) o no (false, por falta de energía).
    public bool AddMultiplier(double amountToAdd)
    {
        if (Energy >= ClickMultiplierCost)
        {
            Energy -= ClickMultiplierCost;
            _multiplier *= amountToAdd;
            ClickMultiplierCost *= 1.15d; // Aumentamos el costo del upgrade para la siguiente compra, multiplicándolo por 1.15 (puedes ajustar este valor para hacer el juego más o menos difícil).
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
            IdleMultiplierCost *= 1.15d; // Aumentamos el costo del upgrade para la siguiente compra, multiplicándolo por 1.15 (puedes ajustar este valor para hacer el juego más o menos difícil).
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

    public void BuyMechaPart()
    {
        if(CurrentMechaPartIndex >= mechaParts.Length)
        {
            Debug.Log("Juego terminado, Felicidades!");
            return;
        }

        if(_energy >= mechaParts[CurrentMechaPartIndex].cost)
        {
            Energy -= mechaParts[CurrentMechaPartIndex].cost;
            OnEnergyChanged?.Invoke(Energy); // Notificamos a los oyentes del cambio de energía después de comprar la parte del mecha.
            CurrentMechaPartIndex++; // Avanzamos al siguiente índice para la próxima parte del mecha.
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

    private IEnumerator IdleEnergyCoroutine()
    {
        while (_isEnabledIdleMultiplier)
        {
            yield return new WaitForSeconds(1f); // Espera 1 segundo
            Energy += _idleEnergyMultiplierAmount; // Agrega 1 de energía cada segundo
            OnEnergyChanged?.Invoke(Energy); // Notifica a los oyentes del cambio de energía
            ShowFloatingText(_idleEnergyMultiplierAmount); // Muestra el texto flotante cada vez que se agrega energía pasiva    
        }
    }
}



