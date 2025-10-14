using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Proporciona métodos de utilidad estáticos para la serialización y deserialización de arrays JSON
/// utilizando <see cref="UnityEngine.JsonUtility"/>.
/// Esta clase es útil cuando la API devuelve un array JSON como raíz, ya que JsonUtility no puede
/// deserializar directamente un array raíz sin un "wrapper" o envoltorio.
/// </summary>
public class JsonHelpers : MonoBehaviour
{
    /// <summary>
    /// Deserializa una cadena JSON que contiene un array de objetos de tipo <typeparamref name="T"/>.
    /// Este método ajusta la cadena JSON para envolver el array dentro de un objeto con una clave "usuarios",
    /// lo que permite que <see cref="UnityEngine.JsonUtility"/> lo procese correctamente.
    /// </summary>
    /// <param name="json">La cadena JSON que representa el array.</param>
    /// <typeparam name="T">El tipo de los objetos dentro del array JSON.</typeparam>
    /// <returns>Un array de objetos de tipo <typeparam name="T">.</typeparam></returns>
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"usuarios\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.usuarios;
    }

    /// <summary>
    /// Una clase interna privada utilizada como un "wrapper" o envoltorio para permitir a
    /// <see cref="UnityEngine.JsonUtility"/> serializar/deserializar arrays de objetos cuando
    /// el array es la raíz del JSON.
    /// </summary>
    /// <typeparam name="T">El tipo de los elementos que contendrá el array.</typeparam>
    [Serializable]
    private class Wrapper<T>
    {
        public T[] usuarios;
    }
}
