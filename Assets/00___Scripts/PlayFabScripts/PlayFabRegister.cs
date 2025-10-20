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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void registerNewUser(){

        PlayFabSettings.staticSettings.TitleId = "139614";
        string nombre = nameInputField.text;
        string email = mailInputField.text;
        string password = passwordInputField.text;


        if(!email.Contains("@") || !email.Contains(".")){
            Debug.LogWarning("introduce una direccion de correo electronico valida");
        }
        //if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nombre))
        //{
           
        //    return;
        //}

        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = nombre,
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false,
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, onRegisterSuccess, onRegisterError);
    }

    public void onRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("se registro correctamente");
        registerButton.SetActive(true);
        SceneManager.LoadScene("MainMenu_Game");
    }
    public void onRegisterError(PlayFabError error)
    {
        Debug.LogError(" Error al registrar usuario: " + error.GenerateErrorReport());
    }
}
