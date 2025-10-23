using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class PlayFabRegister : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_InputField mailInputField; 
    [SerializeField] TMP_InputField passwordInputField;
    public GameObject registerButton;
    public string usernameVar;
   
    public void registerNewUser(){

        PlayFabSettings.staticSettings.TitleId = "139614";
        string nombre = nameInputField.text;
        string email = mailInputField.text;
        string password = passwordInputField.text;

        if(!email.Contains("@") || !email.Contains(".")){
            Debug.LogWarning("introduce una direccion de correo electronico valida");
        }

        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = nombre,
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false,
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, onRegisterSuccess, onRegisterError);
        usernameVar = nameInputField.text;
    }

    public void onRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("se registro correctamente");
        registerButton.SetActive(true);
        SceneManager.LoadScene("CharacterCreation_Game");
    }
    public void onRegisterError(PlayFabError error)
    {
        Debug.LogError(" Error al registrar usuario: " + error.GenerateErrorReport());
    }
}
