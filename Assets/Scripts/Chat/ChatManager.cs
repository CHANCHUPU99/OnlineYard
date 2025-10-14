using Photon.Pun; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime; 

/// <summary>
/// Gestiona la funcionalidad del chat en el juego, permitiendo mensajes globales y privados
/// utilizando Photon PUN 2.
/// </summary>
public class ChatManager : MonoBehaviourPunCallbacks
{
    #region Variables
    public static ChatManager instance;

    public TMP_InputField chatInput;
    public TextMeshProUGUI chatContent; 
    public TMP_Dropdown playerDropdown;

    private PhotonView view;
    private List<string> _messages = new List<string>(); 
    private float _buildDelay = 0f;      
    private int _maximumMessages = 14;
    private GameObject chatPanel; 
    private Dictionary<int, string> players = new Dictionary<int, string>();
    private int targetPlayerId = -1; 

    PlayerManager playerManager; 
    public bool isActiveChat; 
    #endregion

    #region Unity Method
    /// <summary>
    /// Se llama al iniciar el script, inicializa el singleton, busca componentes 
    /// de la UI, y configura el estado inicial del chat.
    /// /// </summary>
    void Start()
    {
        // Inicialización del patrón Singleton.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("chat existing: " + instance.gameObject);
            instance.IamChatM();
            Destroy(gameObject); // Destruye esta instancia duplicada.
        }

        playerManager = FindObjectOfType<PlayerManager>();
        chatPanel = GameObject.Find("WindowChat");
        view = GetComponent<PhotonView>(); 
        chatPanel.SetActive(false); // Oculta el panel de chat del inicio.

        // Actualiza la lista de jugadores después de un breve retraso para asegurar que todo esté inicializado.
        Invoke(nameof(UpdatePlayerList), 0.5f);

        // Se suscribe al evento de cambio de valor del menú desplegable.
        playerDropdown.onValueChanged.AddListener(OnPlayerSelected);
    }

    /// <summary>
    /// Se llama en cada fotograma. Presiona la tecla TAB para mostrar u ocultar el panel del chat.
    /// /// </summary>
    private void Update()
    {
        // Si se presiona la tecla TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (PhotonNetwork.InRoom) // Solo funciona si el jugador está en una sala.
            {
                if (chatPanel.activeSelf)
                {
                    isActiveChat = false; 
                    chatPanel.SetActive(false); 
                }
                else
                {
                    chatPanel.SetActive(true); 
                    isActiveChat = true;
                    chatContent.maxVisibleLines = _maximumMessages;
                    if (_messages.Count > _maximumMessages) 
                    {
                        _messages.RemoveAt(0); 
                    }
                    if (_buildDelay < Time.time)
                    {
                        BuildChatContents(); 
                        _buildDelay = Time.time + 0.25f;
                    }
                    chatInput.ActivateInputField(); // Activa el campo de entrada para escribir.
                }
            }
            else if (_messages.Count > 0)  // Si no está en una sala, limpia los mensajes.
            {
                _messages.Clear();
                chatContent.text = "";
            }
        }
    }
     #endregion

    #region Photon Callbacks
     /// <summary>
     /// Callback de Photon que se ejecuta cuando un nuevo jugador entra a la sala.
     /// </summary>
     /// <param name="newPlayer">El jugador que ha entrado en la sala.</param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"Nuevo jugador entr�: {newPlayer.NickName}");
        Invoke(nameof(UpdatePlayerList), 1f); // Actualiza la lista de jugadores con un restraso.
    }

    /// <summary>
    /// Callback de Photon que se ejecuta cuando un jugador abandona la sala.
    /// </summary>
    /// <param name="otherPlayer">El jugador que ha abandonado la sala.</param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"Jugador sali�: {otherPlayer.NickName}");
        Invoke(nameof(UpdatePlayerList), 1f); // Actualiza la lista de jugadores.
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Método de depuración para confirmar que GameObject es la instancia activa del ChatManager.
    /// </summary>
    public void IamChatM()
    {
        Debug.LogWarning("Im Chat Manager");
    }

    /// <summary>
    /// Actualiza la lista de jugadores en el menú desplegable para mensajes privados.
    /// </summary>
    public void UpdatePlayerList()
    {
        players.Clear(); 
        playerDropdown.ClearOptions(); 

        List<string> options = new List<string>(); 
        options.Add("Chat Global"); 

        Debug.Log($"Hay {PhotonNetwork.PlayerList.Length} jugadores en la sala.");

        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.TagObject != null)
            {
                GameObject playerObject = player.TagObject as GameObject; 
                PhotonView pv = playerObject.GetComponent<PhotonView>();

                if (pv != null && !player.IsLocal)
                {
                    players[pv.ViewID] = player.NickName; 
                    options.Add(player.NickName + " (ID: " + pv.ViewID + ")");
                    Debug.Log($"Jugador detectado: {player.NickName} (ID: {pv.ViewID})");
                }
                else if (player.IsLocal)
                {
                    Debug.Log("No agregamos al jugador local en el Dropdown.");
                }
            }
            else
            {
                Debug.LogWarning($" El jugador {player.NickName} no tiene un TagObject asignado.");
            }
        }

        playerDropdown.AddOptions(options);
        playerDropdown.value = 0; // Por defecto, selecciona "Chat Global".

        Debug.Log("Lista de jugadores actualizada.");
    }

    /// <summary>
    /// Se ejecuta cuando se selecciona una opción en le menú desplegable de jugadores.
    /// </summary>
    /// <param name="index">El índice de la opción seleccionada.</param>
    public void OnPlayerSelected(int index)
    {
        if (index == 0)
        {
            targetPlayerId = -1; // -1 representa el chat global.
            Debug.Log(" Enviando mensajes al chat global");
        }
        else
        {
            int selectedIndex = index - 1; // Ajuste del índice porque "Chat Global" es el primero.
            if (selectedIndex < players.Keys.Count)
            {
                targetPlayerId = players.Keys.ElementAt(selectedIndex);
                Debug.Log($" Enviando mensajes a: {players[targetPlayerId]} (ID: {targetPlayerId})");
            }
            else
            {
                Debug.LogError($"Error: �ndice {selectedIndex} fuera de rango en la lista de jugadores.");
            }
        }
    }

    /// <summary>
    /// Envía un mensaje de chat. Puede ser global  o privado dependiendo del jugador seleccionado.
    /// </summary>
    /// /// <param name="msg">El contenido del mensaje a enviar.</param>
    public void SendChat(string msg)
    {
        string sender = playerManager.Name; 
        string formattedMessage;
        bool isPrivate = (targetPlayerId != -1);

        Debug.Log($"Intentando enviar mensaje. Privado: {isPrivate}, ID Destino: {targetPlayerId}, Dropdown Seleccionado: {playerDropdown.value}");

        if (!isPrivate)
        {
            formattedMessage = sender + ": " + msg; 
            view.RPC("RPC_AddNewMessage", RpcTarget.All, formattedMessage); 
            Debug.Log("Mensaje global enviado: " + formattedMessage);

            // Muestra una burbuja de chat sobre el jugador que envía el mensaje.
            if (PhotonNetwork.LocalPlayer.TagObject != null)
            {
                GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
                if (player != null)
                {
                    PlayerChatBubble chatBubble = player.GetComponent<PlayerChatBubble>();
                    if (chatBubble != null)
                    {
                        chatBubble.photonView.RPC("ShowChatBubble", RpcTarget.All, msg);
                        Debug.Log("Burbuja de chat mostrada.");
                    }
                }
            }
        }
        else
        {
            formattedMessage = "[Privado] " + sender + ": " + msg; 
            PhotonView targetView = PhotonView.Find(targetPlayerId); 

            if (targetView != null)
            {
                // Envía el mensaje privado al jugador objetivo.
                view.RPC("RPC_ReceivePrivateMessage", targetView.Owner, formattedMessage); 
                Debug.Log($"Mensaje privado enviado a {players[targetPlayerId]} (ID: {targetPlayerId})");

                // Muestra el mensaje enviado en la propia ventana de chat del remitente.
                RPC_ReceivePrivateMessage(formattedMessage);
            }
            else
            {
                Debug.LogError("No se encontr� el jugador con ID " + targetPlayerId);
            }
        }
    }

    /// <sumary>
    /// Método llamado por la UI para enviar el texto de campo de entrada.
    /// </summary>
    public void SubmitChat()
    {
        string blankCheck = chatInput.text;
        blankCheck = Regex.Replace(blankCheck, @"\s", ""); 
        if (blankCheck == "")
        {
            chatInput.ActivateInputField();
            chatInput.text = ""; 
            return;
        }
        SendChat(chatInput.text);
        chatInput.ActivateInputField();
        chatInput.text = "";
    }
    #endregion
    
    #region RPC Methods
    /// <summary>
    /// [PunRPC] Añade un nuevo mensaje a la lista de mensajes de todos los clientes.
    /// </summary>
    /// /// <param name="msg">El mensaje formateado a añadir.</param>
    [PunRPC]
    void RPC_AddNewMessage(string msg)
    {
        _messages.Add(msg);
        if (_messages.Count > _maximumMessages)
        {
            _messages.RemoveAt(0); 
        }
        BuildChatContents(); 
    }

    /// <summary>
    /// [PunRPC] Recibe un mensaje privado y lo añade a la lista de mensajes del cliente receptor.
    /// </summary>
    /// <param name="msg">El mensaje privado formateado a añadir.</param>
    [PunRPC]
    void RPC_ReceivePrivateMessage(string msg)
    {
        _messages.Add(msg); 
        BuildChatContents(); 
        Debug.Log("Mensaje privado recibido: " + msg);
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Reconstruye el contenido del chat en la UI a partir de la lista de mensajes.
    /// </summary>
    void BuildChatContents() 
    {
        string NewContents = "";
        foreach (string s in _messages) 
        {
            NewContents += s + "\n"; 
        }
        chatContent.text = NewContents; 
    }
    #endregion
}
