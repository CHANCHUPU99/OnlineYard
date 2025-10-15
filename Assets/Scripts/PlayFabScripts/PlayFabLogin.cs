using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField] private string titleId = "TU_TITLE_ID_AQUI"; // <- reemplaza por tu Title ID real
    private const string CUSTOM_ID_KEY = "PLAYFAB_CUSTOM_ID";
    public static string PlayFabId { get; private set; }

    void Start()
    {
        // Configurar el Title ID de PlayFab
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = titleId;

        Login();
    }

    void Login()
    {
        string customId = PlayerPrefs.GetString(CUSTOM_ID_KEY, string.Empty);

        if (string.IsNullOrEmpty(customId))
        {
            customId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(CUSTOM_ID_KEY, customId);
            PlayerPrefs.Save();
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        PlayFabId = result.PlayFabId;
        Debug.Log($"? Login exitoso en PlayFab. PlayFabId: {PlayFabId}");

        // Si tienes el PhotonConnector en el mismo GameObject, se conecta automáticamente:
        PhotonConnector connector = GetComponent<PhotonConnector>();
        //if (connector != null)
        //    connector.ConnectToPhoton(PlayFabId);
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"? Error de login: {error.GenerateErrorReport()}");
    }
}
