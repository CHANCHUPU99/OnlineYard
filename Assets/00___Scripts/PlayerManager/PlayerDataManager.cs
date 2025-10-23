using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    // --- Datos de perfil del usuario ---
    public string fullName;
    public string username;
    public string userDescription;

    // --- Datos de personalización ---
    public SelectedClothes[] selectedSkin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método opcional para actualizar datos de usuario desde PlayFab
    public void SetProfileData(string name, string uname, string desc)
    {
        fullName = name;
        username = uname;
        userDescription = desc;
    }

    // Método opcional para actualizar skin
    public void SetSkin(SelectedClothes[] skinData)
    {
        selectedSkin = skinData;
    }
}
