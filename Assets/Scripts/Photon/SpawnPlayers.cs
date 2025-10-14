using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Gestiona la instanciación del jugador cuando un cliente se une a una sala de Photon.
/// Cada cliente instanciará su propio prefab de jugador en una posición predeterminada.
/// </summary>
public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    /// <summary>
    /// Se llama al inicio del ciclo de vida del script.
    /// Instancia al prefab del jugador para el cliente local a través de la red Photon, en una posición fija (0,0)
    /// con rotación por defecto.
    /// </summary>
    private void Start()
    {
        Vector2 randomPosition = new Vector2(0,0);
        
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

    }

}