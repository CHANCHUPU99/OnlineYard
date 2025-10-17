using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Gestiona una burbuja de chat que aparece sobre la cabeza de un jugador para mostrar mensajes en el mundo del juego.
/// </summary>
public class PlayerChatBubble : MonoBehaviourPun
{
    public GameObject chatBubble;  
    public TextMeshPro chatText;  
    public float bubbleTime = 15; 

    private Coroutine hideCoroutine;

    /// <summary>
    /// Método de Unity llamado al inicio del ciclo de vida del script.
    /// Realiza una comprobación inicial de los componentes  y oculta la burbuja por defecto.
    /// </summary>
    void Start()
    {
        if (chatBubble == null || chatText == null)
        {
            Debug.LogError("No est� asignado  " + gameObject.name);
            return;
        }

        // Asegura que la burbuja esté oculta al empezar.
        chatBubble.SetActive(false); 
    }

    /// <summary>
    /// [PunRPC] Muestra la burbuja de chat con un mensaje específico.
    /// Este método se invoca a través de la red para que todos los jugadores vean la burbuja.
    /// </summary>
    /// <param name="message">El texto que se mostrará en la burbuja de chat.</param>
    /// <remarks>
    /// Si hay un burbuja visible, éste método detendrá el temporizador anterior y comenzará uno nuevo,
    /// reiniciando así el tiempo de visibilidad de la burbuja con el nuevo mensaje.
    /// </remarks>
    [PunRPC]
    public void ShowChatBubble(string message)
    {
        if (chatBubble == null || chatText == null)
        {
            Debug.LogError("No se puede mostrar el mensaje " + gameObject.name);
            return;
        }

        Debug.Log("Mostrando mensaje en " + gameObject.name + ": " + message);

        chatBubble.SetActive(true);
        chatText.text = message;

        // Si ya existe una corrutina para ocultar la burbuja, la detenemos.
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        // Iniciamos una nueva corrutina para ocultar la burbuja después del tiempo especificado.
        hideCoroutine = StartCoroutine(HideBubbleAfterDelay(bubbleTime));
    }

    /// <summary>
    /// Corrutina que espera un tiempo determinado antes de ocultar la burbuja de chat.
    /// </summary>
    /// <param name="delay">El tiempo en segundos que se debe esperar antes de ocultar la burbuja.</param>
    /// <returns>Un IEnumerator para la ejecución de la corrutina.</returns>
    private IEnumerator HideBubbleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        chatBubble.SetActive(false);
    }
}
