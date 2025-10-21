using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Nombres exactos de los prefabs dentro de Resources/")]
    public string playerPrefabName = "PlayerCustom";
    public string managerPrefabName = "ServerManagers";

    private void Start()
    {
        // Si ya estamos dentro de una sala cuando carga la escena
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Ya estamos en una sala al iniciar escena, instanciando manualmente...");
            InstantiatePrefabs();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Jugador unido a la sala. Instanciando objetos...");
        InstantiatePrefabs();
    }

    private void InstantiatePrefabs()
    {
        Vector2 spawnPosition = new Vector2(0, 0);

        // Cargar prefabs desde Resources
        GameObject managerPrefab = Resources.Load<GameObject>(managerPrefabName);
        GameObject playerPrefab = Resources.Load<GameObject>(playerPrefabName);

        if (managerPrefab != null)
        {
            Debug.Log("Instanciando ManagerObject...");
            PhotonNetwork.Instantiate(managerPrefab.name, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError($" No se encontró el prefab {managerPrefabName} en Resources/");
        }

        if (playerPrefab != null)
        {
            Debug.Log("Instanciando PlayerObject...");
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError($" No se encontró el prefab {playerPrefabName} en Resources/");
        }
    }
}
