using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;

/// <summary>
/// Gestiona la comunicación con una API externa para obtener datos de alumnos.
/// Realiza peticiones HTTP GET y muestra los resultados en un elemento de interfaz de usuario TextMeshPro.
/// </summary>
public class APIManager : MonoBehaviour
{
    [SerializeField] private string baseURL = "http://localhost:3000"; // URL de la API
    [SerializeField] private TextMeshProUGUI usuariosText; // UI donde se mostrar�n los datos

    /// <summary>
    /// Se llama al inicio del ciclo de vida del script.
    /// Inicia la corrutina para obtener la lista de alumnos desde la API.
    /// </summary>
    private void Start()
    {
        StartCoroutine(GetAlumnos()); 
    }

    /// <summary>
    /// Corrutina para realizar una petición HTTP GET a la API y obtener los datos de los alumnos.
    /// Procesa la respuesta JSON y la muestra en la UI.
    /// </summary>
    /// <returns> Un <see cref="IEnumerator"/> para ser usado como corrutina.</returns>
    IEnumerator GetAlumnos()
    {
        string url = $"{baseURL}/alumnos"; // Ruta corregida

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;

                // Deserializa el JSON correctamente
                AlumnosWrapper alumnosWrapper = JsonUtility.FromJson<AlumnosWrapper>($"{{\"alumnos\":{json}}}");

                if (alumnosWrapper != null && alumnosWrapper.alumnos.Length > 0)
                {
                    MostrarAlumnos(alumnosWrapper.alumnos);
                }
                else
                {
                    Debug.LogWarning("No se recibieron alumnos.");
                    usuariosText.text = "No hay alumnos disponibles.";
                }
            }
            else
            {
                Debug.LogError($"Error en la petici�n: {request.error}\nURL: {url}\nResponse: {request.downloadHandler.text}");
                usuariosText.text = "Error al cargar alumnos.";
            }
        }
    }
    /// <summary>
    /// Muestra la info de los alumnos proprocionados en el elemento de UI <see cref="usuariosText"/>.
    /// Formeatea la info para incluir nickname, nombre corto y nombre completo.
    /// </summary>
    /// <param name="alumnos"> Un array de objetos <see cref="Alumno" para mostrar./></param>
    void MostrarAlumnos(Alumno[] alumnos)
    {
        usuariosText.text = "Alumnos:\n";
        foreach (Alumno alumno in alumnos)
        {
            string nombreCorto = string.IsNullOrEmpty(alumno.nombre_corto) ? "N/A" : alumno.nombre_corto;
            string nombreCompleto = string.IsNullOrEmpty(alumno.nombre_completo) ? "N/A" : alumno.nombre_completo;

            usuariosText.text += $"{alumno.nickname}: {nombreCorto} ({nombreCompleto})\n";
        }
    }

    /// <summary>
    /// Una clase auxiliar utilizada para envolver el array JSON de alumnos.
    /// <see cref="UnityEngine.JsonUtility"/> requiere que los arrays sean campos de una clase para
    /// ser deserializados desde un JSON raíz.
    /// </summary>
    [System.Serializable]
    private class AlumnosWrapper
    {
        public Alumno[] alumnos;
    }

    /// <summary>
    /// Representa la estructura de datos de un alumno, tal como se espera recibir desde la API.
    /// Es serializable para permitir la deserialización automática desde JSON.
    /// </summary>
    [System.Serializable]
    private class Alumno
    {
        public string nickname;
        public string nombre_corto;
        public string nombre_completo;
    }
}
