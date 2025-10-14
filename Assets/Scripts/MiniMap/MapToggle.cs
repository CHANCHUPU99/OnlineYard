using UnityEngine;

/// <summary>
/// Gestiona la visibilidad de los paneles del mapa y los "pisos" del mapa, permitiendo
/// al jugador alternar la vista del mapa completo con una tecla.
/// </summary>
public class MapToggle : MonoBehaviour
{
    public GameObject[] floors;
    public GameObject fullMapPanel;
    private ChatManager chatManager;

    /// <summary>
    /// Se llama al inicia del ciclo de vida del script.
    /// Inicializa la visibilidad de los pies y el panel del mapa, y obtiene una
    /// referencia al <see cref="chatManager"/> en la escena.
    /// </summary>
    void Start()
    {
            floors[0].SetActive(true);
            floors[1].SetActive(true);
        fullMapPanel.SetActive(false);

        chatManager = FindObjectOfType<ChatManager>();
    }

    /// <summary>
    /// Se llama una vez por cada frame.
    /// Detecta la pulsación de la tecla 'M' para alternar la visibilidad del panel del mapa completo,
    /// solo si el chat está activo.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && (chatManager == null || !chatManager.isActiveChat))
        {
            fullMapPanel.SetActive(!fullMapPanel.activeSelf);
        }
    }
}
