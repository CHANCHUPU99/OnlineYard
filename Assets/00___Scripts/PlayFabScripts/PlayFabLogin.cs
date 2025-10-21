using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class PlayFabLogin : MonoBehaviour
{
    [Header("PlayFab Settings")]
    [SerializeField] private string titleId = "139614"; 
    private const string CUSTOM_ID_KEY = "PLAYFAB_CUSTOM_ID";

    void Start()
    {
        titleId = "139614";
        // Configura el Title ID
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = titleId;

        LoginWithCustomID();
    }

    void LoginWithCustomID()
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

        Debug.Log(" Iniciando sesión en PlayFab...");
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($" Login exitoso. PlayFab ID: {result.PlayFabId}");
        // Luego de autenticarse en PlayFab, conectamos a Photon
      //  PhotonNetwork.ConnectUsingSettings();
    }

    void OnLoginError(PlayFabError error)
    {
        Debug.LogError($" Error de login en PlayFab: {error.GenerateErrorReport()}");
    }
}
