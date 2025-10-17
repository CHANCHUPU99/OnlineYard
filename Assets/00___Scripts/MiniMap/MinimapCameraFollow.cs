using Photon.Pun;
using UnityEngine;

/// <summary>
/// Controla que la cámara del minimapa siga al jugador local en un entorno de red Photon.
/// Asegura que la cámara siempre esté centrada en la posición Y y X del jugador, manteniendo
/// su propia altura (coordenada Z).
/// </summary>
public class MinimapCameraFollow : MonoBehaviour
{
    private Transform playerTransform;

    /// <summary>
    /// Se llama al inicio del ciclo de vida del script.
    /// Busca el GameObject del jugador local (el que tiene un <see cref="PhotonView"/> propio)
    /// y guarda su transform para seguirlo.
    /// </summary>
    void Start()
    {
        // Buscar al jugador local de Photon
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                playerTransform = player.transform;
                break;
            }
        }
    }

    /// <summary>
    /// Se llama una vez por frame, después de que todos los métodos Update han sido ejecutados.
    /// Es ideal apra lógicas de seguimiento de cámara para evitar "lag" o movimientos entrecortados.
    /// </summary>
    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Mantener la c�mara del minimapa enfocada en el jugador local
            Vector3 newPosition = playerTransform.position;
            newPosition.z = transform.position.z; // Mantener la altura/z original
            transform.position = newPosition;
        }
    }
}
