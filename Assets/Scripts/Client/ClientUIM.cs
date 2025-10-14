using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon; 
using Photon.Pun;
using System;

/// <summary>
/// Gestiona la interfaz de usuario (UI) del cliente en un entorno multijugador con Photon PUN.
/// Permite mostrar y ocultar paneles, configurar textos y botones, y gestionar notificaciones.
/// Implmementa <see cref="MonoBehaviourPunCallBacks"/> para callbacks de red de Photon.
/// Tambien implementa <see cref="INotifications"/> para un sistema de patrón Observador.
/// </summary>
public class ClientUIM : MonoBehaviourPunCallbacks, INotifications
{
    #region Variables
    [SerializeField] GameObject _canvasPanel;
    [SerializeField] TextMeshProUGUI _panelText;
    [SerializeField] Button _button;
    [SerializeField] GameObject _objName;

    private bool _canPushButton;
    List<IObserver> _observers = new List<IObserver>();
    #endregion
    
    /// <summary>
    /// Se llama una vez al inicio del ciclo de vida del script.
    /// Si esta instancia del cliente es el dueño (IsMine) de este objeto en la red,
    /// busca el panel UI, se suscribe al InfoServerManager y activa el GameObject del nombre.
    /// </summary>
    void Start()
    {
        if (photonView.IsMine)
        {
            FindPanel();
            InfoServerManager.Instance.SuscribeUIM(this);
            _objName.SetActive(true);

        }
    }

    /// <summary>
    /// Añade un observador a la lista de observadores.
    /// Este método forma parte de la implementación del patrón Observador donde ClientUIM es el 'Subject' (observable).
    /// </summary>
    /// <param name="obs">La instancia del observador a añadir.</param>
    public void AddObserver(IObserver obs)
    {
        _observers.Add(obs);
    }

    /// <summary>
    /// Establece la referencia al GameObject que se usa para mostrar el nombre.
    /// </summary>
    /// <param name="obj">El GameObject que representará el nombre.</param>
    public void SetNameObj(GameObject obj)
    {
        _objName = obj;
        //_objName.SetActive(false);
    } 

    /// <summary>
    /// Busca y asigna las referencias a los componentes a la UI (Panel, Botón, Texto).
    /// Espera que la jerarquía de la UI sea "Canvas/PanelAbrirMeet".
    /// </summary>
    public void FindPanel()
    {
        _canvasPanel = GameObject.Find("Canvas/PanelAbrirMeet").gameObject;
        if (_canvasPanel != null)
        {
            _button = _canvasPanel.GetComponentInChildren<Button>();
            _panelText = _canvasPanel.GetComponentInChildren<TextMeshProUGUI>();

            if (_button != null && _panelText != null)
            {
                Debug.Log("Canvas cargado");
                _canvasPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("El bot�n o el texto no se encontraron.");
            }
        }
        else
        {
            Debug.LogError("El panel no se encontr�.");
        }
    }

/// <summary>
/// Configurar la función que se ejecutará cuando se haga clic en el botón basandose en el ID
/// del panel proporcionado.
/// </summary>
/// <param name="panelID">El ID que determina la acción del botón.</param>
    void SetFunction(int panelID)
    {
        switch (panelID)
        {
            case 0:
                _button.onClick.RemoveAllListeners();
                break;
            case 1:
                _button.onClick.AddListener(OpenTest);
                break;
            case 2:
                _button.gameObject.SetActive(true);
                break;
            case 3:
                _button.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

/// <summary>
/// Abre una URL predefinida en el navegador web (en este caso, Google Calendar).
/// </summary>
    void OpenTest()
    {
        Application.OpenURL("https://calendar.google.com/calendar/u/0/r");
    }

/// <summary>
/// Controla la lógica de visualización y el contenido del panel de la UI basándose en
/// un ID de panel específico.
/// </summary>
/// <param name="panelID">El ID del panel para aplicar la lógica.</param>
    public void PanelLogic(int panelID)
    {
        if (_canvasPanel == null)
        {
            FindPanel();
        }
        else
        {

            switch (panelID)
            {
                case 0:
                    _panelText.text = " ";
                    SetFunction(2);
                    SetFunction(0);
                    _canvasPanel.SetActive(false);
                    _objName.SetActive(true);
                    break;
                case 1:
                    _objName.SetActive(false);
                    _canvasPanel.SetActive(true);
                    SetFunction(2);
                    _panelText.text = "Bienvenido " + photonView.ViewID.ToString("#########") + " recuerda tomar agua!";
                    SetFunction(panelID);
                    break;
                default:
                    break;
            }
        }
    }

/// <summary>
/// Muestra un panel de diálogo con el nombre y el texto de un NPC.
/// Desactiva el objeto del nombre del jugador y oculta el botón del panel.
/// </summary>
/// <param name="npc">El objeto NPCDialog que contiene el nombre y el diálogo del NPC.</param>
    public void ShowNPCPanel(NPCDialog npc)
    {
        if (_canvasPanel==null)
        {
            FindPanel();
        }
        else
        {
            _objName.SetActive(false);
            _canvasPanel.SetActive(true);
            _panelText.text = npc.name + ": " + npc.dialog;
            SetFunction(3);
        }
    }

/// <summary>
/// Suscribe un observador a la lista de notificaciones.
/// Esto es parte de la implementación de <see cref="INotifications"/>
/// </summary>
/// <param name="observer">La instancia del observador a suscribir.</param>
    public void SuscribeNotification(IObserver observer)
    {
        _observers.Add(observer);
    }


/// <summary>
/// Desuscribe un observador de la lista de notificaiones.
/// Esto es parte de la implementación de <see cref="INotifications"/>
/// </summary>
/// <param name="observer">La instancia del observador a desuscribir.</param>
    public void UnSuscribeNotification(IObserver observer)
    {
        _observers.Remove(observer);
    }

/// <summary>
/// Notifica a todos los observadores suscritos sobre un evento específico.
/// Ese método se invoca para desencadenar actualizaciones de los observadores.
/// </summary>
/// <param name="idEvent">El ID del evento que se está notificando.</param>
    public void Notify(int idEvent)
    {
        switch (idEvent)
        {
            default:
                foreach (IObserver observer in _observers)
                {
                    observer.Updated(this, idEvent);
                }
                break;
        }
    }

/// <summary>
/// Método RPC de Photon que se llama en todos los clientes conectados.
/// Intenta asignar un panel basandose en un ViewID.
/// </summary>
/// <param name="viewID">El Photon ViewID que se espera que coincida con el ViewID local para proceder.</param>
    [PunRPC]
    void AssignPanel(int viewID)
    {
        StartCoroutine(WaitAndSync(viewID));
    }

/// <summary>
/// Corutina que espera un momento y luego verifica si el ViewID pasado coincide con el ViewID local.
/// Si no coincide, muestra una advertencia. Su propósito exacto de "sincronización" no es claro solo con este fragmento.
/// </summary>
/// <param name="viewID">El viewID para verificar contra el ViewID de esta PhotonView.</param>
/// <returns></returns>
    private IEnumerator WaitAndSync(int viewID)
    {
        if (photonView.ViewID != viewID)
        {
            Debug.LogWarning($"ViewID {viewID} no coincide con el ViewID local {photonView.ViewID}");
            yield break;
        }
    }

/// <summary>
/// Muestra un panel de diálogo con el contenido de un NPC.
/// Este método es un wrapper para <see cref="ShowNPCPanel"/>.
/// </summary>
/// <param name="npc">El objeto NPCDialog que contiene la información del NPC.</param>
    internal void GetDialog(NPCDialog npc)
    {
        ShowNPCPanel(npc);
    }

/// <summary>
/// Restablece el panel de la UI a su estado por defecto.
/// Este método es un wrapper para <see cref="PanelLogic"/> con el ID 0.
/// </summary>
    internal void UnSetDialog()
    {
        PanelLogic(0);
    }
}

