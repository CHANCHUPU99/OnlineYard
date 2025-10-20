using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;

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

    //Cambio aqui(esto no estaba comentado)
    void Start()
    {
        //  Cargar los datos del jugador antes de conectar a Photon
        LoadPlayerCustomization();
    }

    /// <summary>
    /// Obtiene la personalización del jugador desde PlayFab antes de conectarse a Photon.
    /// </summary>
    private void LoadPlayerCustomization()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnDataError);
    }

    private void OnDataReceived(GetUserDataResult result)
    {
        Debug.Log(" Datos del jugador recibidos desde PlayFab.");

        // Aquí puedes acceder a tus claves guardadas (por ejemplo, "SkinColor", "Outfit", etc.)
        if (result.Data != null)
        {
            if (result.Data.ContainsKey("SelectedOutfit"))
            {
                string outfit = result.Data["SelectedOutfit"].Value;
                Debug.Log("Skin cargada: " + outfit);

                // Si tienes un SkinSelectorManager global o persistente:
                var skinManager = FindObjectOfType<SkinSelectorManager>();
                if (skinManager != null)
                {
                    skinManager.applySavedSelection(outfit);
                }
            }
        }

        //  Una vez que cargamos los datos, conectamos a Photon
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnDataError(PlayFabError error)
    {
        Debug.LogWarning("⚠️ Error al obtener los datos del jugador: " + error.GenerateErrorReport());
        // Aunque haya error, seguimos y nos conectamos
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Se invoca cuando el cliente se ha conectado exitosamente al servidor maestro de Photon.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Se invoca cuando el cliente se ha unido exitosamente a un lobby de Photon.
    /// </summary>
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Game_Test");
    }
}
