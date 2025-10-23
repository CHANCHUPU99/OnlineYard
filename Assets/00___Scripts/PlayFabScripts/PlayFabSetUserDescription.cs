using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
public class PlayFabSetUserDescription : MonoBehaviour
{
    [SerializeField] TMP_InputField userNameInputField;
    [SerializeField] TMP_InputField userDescriptionInputField;
    
    void uploadUserDescription(){

    }

    //
    public void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string,string>()
            {
                {"name", "Ivan Osvaldo Flores Carmona" },
                {"userDescription", "hola, soy ivan" }
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error =>
        {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }
   
}
