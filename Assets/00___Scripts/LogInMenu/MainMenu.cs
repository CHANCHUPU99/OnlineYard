using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
/// <summary>
/// Gestiona la funcionalidad básica del menú principal, como inicar el juego.
/// Este script se adjuntaría a un objeto en la escena del menú principal.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passwordInputField;
    public GameObject loginButton;

    private void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "139614"; 
    }

    //  Llamado cuando el usuario presiona el botón "Iniciar Sesión"
    public void LoginUser()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Debes llenar todos los campos para iniciar sesión.");
            return;
        }

        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }

   
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log(" Inicio de sesión exitoso. ID de jugador: " + result.PlayFabId);

        // Activamos el botón (por si estaba deshabilitado)
        loginButton.SetActive(true);

        //  Guardar el ID del jugador si quieres usarlo después
        PlayerPrefs.SetString("PlayFabId", result.PlayFabId);

        // 🔹 Redirigir a la escena de carga (para que ConnectToServer se encargue del resto)
        SceneManager.LoadScene("LoadingScreen_Test");
        //SceneManager.LoadScene("UserDescription");
    }

    // 🔹 Si hubo un error al iniciar sesión
    private void OnLoginError(PlayFabError error)
    {
        Debug.LogError(" Error al iniciar sesión: " + error.GenerateErrorReport());
    }
    public void userRegister(){
        SceneManager.LoadScene("UserRegister");
    }
}
