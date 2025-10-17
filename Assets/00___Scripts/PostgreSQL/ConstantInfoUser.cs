using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mantiene y gestiona la info constante del usuario a lo largo de las diferentes escenas del juego.
/// Implementa el patrón Singleton para asegurar una única instancia global accesible.
/// </summary>
public class ConstantInfoUser : MonoBehaviour
{
    public static ConstantInfoUser instance;

    private static string _nickName;
    private static string _matricula;
    private static string _shortName;
    private static string _fullName;

    public string NickName { get => _nickName; set => _nickName = value; }
    public string Matricula { get => _matricula; set => _matricula = value; }
    public string ShortName { get => _shortName; set => _shortName = value; }
    public string FullName { get => _fullName; set => _fullName = value; }

    /// <summary>
    /// Se llama cuando el script es cargado.
    /// Implementa la lógica del patrón Singleton: asegura que solo una instancia de <see cref="ConstantInfoUser"/>
    /// exista y persiste entre escenas.
    /// Si ya existe una instancia, destruye este GameObject duplicado.
    /// </summary>
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
