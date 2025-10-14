using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoServerManager : MonoBehaviour, IObserver
{
    public static InfoServerManager Instance;
    public List<PlayerManager> players = new List<PlayerManager>();

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

    public int CantPlayers()
    {
        return players.Count;
    }


    public void RegisterPlayer(PlayerManager player)
    {
        //player.SuscribeNotification(this);
        players.Add(player);
        player.Initilize();
        player.Dress(player.SkinSl);
    }

    public void UnRegisterPlayer(PlayerManager player)
    {
        //player.UnSuscribeNotification(this);
        players.Remove(player);
    }

    public void SuscribeUIM(INotifications notify)
    {
        notify.AddObserver(this);
    }

    public void Updated(INotifications notify, int idEvent)
    {
        switch (idEvent)
        {
            case int w when (w < 16):
                ClientUIM temp = notify as ClientUIM;

                temp.PanelLogic(idEvent);
                break;
            default:
                break;
        }


    }

}
