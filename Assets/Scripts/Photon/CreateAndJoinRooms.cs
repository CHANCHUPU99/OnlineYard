using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Gestiona la funcionalidad para crear y unirse a salas de juego en Photon.
/// Permite a los jugadores crear una sala con un nombre específico, unirse a una existente,
/// o unirse a una sala aleatoria.
/// Hereda de <see cref="MonoBehaviourPunCallbacks"/> para manejar los callbacks de Photon.
/// </summary>
public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
     
{
    public InputField createInput;
    public InputField joinInput;
    public string roomName = "LabExample";

    /// <summary>
    /// Intenta crear una sala de Photon utilizando el texto ingresado en el <see cref="createInput"/>.
    /// Este método se asignaría a un botoón "Crear Sala" en la UI.
    /// </summary>
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    /// <summary>
    /// Intenta unirse a una sala de Photon utilizando el texto ingresando en el <see cref="joinInput"/>.
    /// Este método se asignó a un botón en la UI.
    /// </summary>
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    /// <summary>
    /// Intenta unirse a una sala aleatoria de Photon.
    /// Este método se asignó a un botón en la UI.
    /// </summary>
    public void EntryRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Callback en Photon que se invoca cuando el intento de unirse a una sala aleatoria falla.
    /// Si no se encuentra una sala aleatoria disponibles, se crea una nueva sala con el nombre "ArtekOnline".
    /// </summary>
    /// <param name="returnCode">Código de error retornado por el servidor.</param>
    /// <param name="message">Mensaje de error.</param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("ArtekOnline");
    }

    /// <summary>
    /// Callback de Photon que se invoca cuando el cliente se ha unido exitosamente a una sala.
    /// Una vez unido a la sala, carga la escena del juego real, cuyo nombre está definifo por <see cref="roomName"/>.
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(roomName);
    }
}
