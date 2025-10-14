using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la conexión inicial del cliente al servidor de Photon y el manejo de los eventos de conexión.
/// Hereda de <see cref="MonoBehaviourPunCallbacks"/> para recibir callbacks de Photon.
/// </summary>
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Se llana al inicio del ciclo de vida del script.
    /// Inicia el proceso de conexión al servidor maestro de Photon usando la configuración predeterminada.
    /// </summary>
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Se invoca cuando el cliente se ha conectado exitosamente al servidor maestro de Photon.
    /// Después de una conexión exitosa, el cliente intenta unirse a un lobby.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Se invoca cuando el cliente se ha unido exitosamente a un lobby de Photon.
    /// Una vez en el lobby, carga la escena de "CharacterSelection" para que el jugador elija su personaje.
    /// </summary>
    public override void OnJoinedLobby()
    {

        SceneManager.LoadScene("CharacterSelection");
    }

}
