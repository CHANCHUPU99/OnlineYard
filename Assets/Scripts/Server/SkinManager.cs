using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;
    public List<ClosetCategory> closet;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // No destruir el GameManager al cambiar de escena
        }
        else
        {
            Destroy(gameObject);  // Si ya hay una instancia, destruir la nueva
        }
    }

    public AnimatorOverrideController ConsultSkin(int n)
    {
        return closet[0].oAnimators[n];
    }
    public AnimatorOverrideController ConsultHair(int n)
    {
        return closet[1].oAnimators[n];
    }
    public AnimatorOverrideController ConsultShirt(int n)
    {
        return closet[2].oAnimators[n];
    }
    public AnimatorOverrideController ConsultPants(int n)
    {
        return closet[3].oAnimators[n];
    }
    public AnimatorOverrideController ConsultShoes(int n)
    {
        return closet[4].oAnimators[n];
    }
    public AnimatorOverrideController ConsultAcce(int n)
    {
        return closet[5].oAnimators[n];
    }
}
