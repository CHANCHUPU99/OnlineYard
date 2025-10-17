using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerObject;
    public GameObject ManagerObject;


    private void Start()
    {
        Vector2 randomPosition = new Vector2(0, 0);

        PhotonNetwork.Instantiate(ManagerObject.name, randomPosition, Quaternion.identity);
        PhotonNetwork.Instantiate(PlayerObject.name, randomPosition, Quaternion.identity);

    }
}
