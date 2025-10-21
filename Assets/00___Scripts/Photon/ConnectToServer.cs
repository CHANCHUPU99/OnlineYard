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
        Debug.Log("Datos del jugador recibidos desde PlayFab.");

        if (result.Data != null && result.Data.ContainsKey("SelectedSkin"))
        {
            string json = result.Data["SelectedSkin"].Value;
            Debug.Log(" Skin obtenida desde PlayFab: " + json);

            // 2️⃣ Guardar la skin globalmente en el PlayerDataManager
            PlayerDataManager.Instance.selectedSkin = JsonHelp.JsonHelper.FromJson<SelectedClothes>(json);

            Debug.Log(" Skin almacenada en PlayerDataManager.");
        }
        else
        {
            Debug.Log(" No se encontró ninguna skin guardada. Usando valores por defecto.");

            // Si no hay skin guardada, inicializa una vacía
            PlayerDataManager.Instance.selectedSkin = new SelectedClothes[1];
            PlayerDataManager.Instance.selectedSkin[0] = new SelectedClothes();
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnDataError(PlayFabError error)
    {
        Debug.LogWarning(" Error al obtener los datos del jugador: " + error.GenerateErrorReport());

        // Si falla la carga de datos, seguimos igual con una skin vacía
        PlayerDataManager.Instance.selectedSkin = new SelectedClothes[1];
        PlayerDataManager.Instance.selectedSkin[0] = new SelectedClothes();

        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Se invoca cuando el cliente se ha conectado exitosamente al servidor maestro de Photon.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("🌐 Conectado al servidor maestro de Photon.");
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Se invoca cuando el cliente se ha unido exitosamente a un lobby de Photon.
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("✅ Se ha unido al lobby, cargando escena del juego...");
        SceneManager.LoadScene("Game_Test");
    }
}
