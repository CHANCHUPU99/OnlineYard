using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
public class PlayFabSetUserDescription : MonoBehaviour
{
    [SerializeField] TMP_InputField fullNameInputField;
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField userDescriptionInputField;
    public void SetUserData()
    {
        string fullName = fullNameInputField.text;
        string username = usernameInputField.text;
        string userDescription = userDescriptionInputField.text;
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "name", fullName },
                { "username", username },
                { "userDescription", userDescription }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log(" Datos del usuario actualizados correctamente en PlayFab.");
                if (PlayerDataManager.Instance != null)
                {
                    PlayerDataManager.Instance.SetProfileData(fullName, username, userDescription);
                }
                SceneManager.LoadScene("CharacterCreation_Game");
            },
            error =>
            {
                Debug.LogError(" Error al actualizar los datos del usuario:");
                Debug.LogError(error.GenerateErrorReport());
            });
    }
}
   

