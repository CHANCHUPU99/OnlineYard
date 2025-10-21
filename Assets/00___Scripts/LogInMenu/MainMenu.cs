using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
/// <summary>
/// Gestiona la funcionalidad básica del menú principal, como inicar el juego.
/// Este script se adjuntaría a un objeto en la escena del menú principal.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
       // PlayFabClientAPI.RegisterPlayFabUser();
    }

    public void userRegister()
    {
        SceneManager.LoadScene("UserRegister");
    }
}
