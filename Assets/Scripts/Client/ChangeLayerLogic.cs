using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeLayerLogic : MonoBehaviourPunCallbacks
{
    [Tooltip("Esta en el piso Superior?: True / False ")]
    [SerializeField] bool sup = false;
    [Tooltip("Array de objetos pertenecientes al Jugador, todo lo que cambiara de piso al pasar por la escalera")]
    public GameObject[] _objLy;
    [Tooltip("Camara del minimapa, para que sus target sean las capas destino: superior/inferior")]
    public Camera minimapCamera;
    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            // El jugador iniciara en un piso asignado por default, el cero
            StartLevelC(0);
        }
    }
    /// <summary>
    /// El jugador cambiara de piso, cambiando sus layers 0 es Inferior  mayor que 0 sera superior
    /// </summary>
    /// <param name="z"> numero de piso </param>
    public void StartLevelC(float z)
    {
        StartCoroutine(ChangeLevel(z));
    }
    /// <summary>
    /// Corrutina de cambio de piso, solo utilizado por StartLevelC()
    /// </summary>
    /// <param name="z"> numero se piso</param>
    /// <returns></returns>
    public IEnumerator ChangeLevel(float z)
    {
        yield return new WaitForSeconds(0.1f);
        if (z > 0) // Piso superior
        {
            yield return new WaitForSeconds(0.2f);
            sup = true;
            Camera.main.cullingMask = LayerMask.GetMask("Default", "PlayerSuperior", "PisoSuperior");
            minimapCamera.cullingMask = LayerMask.GetMask("Default", "PlayerSuperior", "PisoSuperior");

        }
        else // Piso inferior
        {
            sup = false;
            Camera.main.cullingMask = LayerMask.GetMask("Default", "PlayerInferior", "PisoInferior");
            minimapCamera.cullingMask = LayerMask.GetMask("Default", "PlayerInferior", "PisoInferior");
        }

        NotifyChangeLayer();
    }

    /// <summary>
    /// Cambio de piso notificado atraves de photon para coordinar el cambio de piso
    /// </summary>
    void NotifyChangeLayer()
    {
        photonView.RPC("ChangePlayerFloor", RpcTarget.AllBuffered, sup);
    }

    /// <summary>
    /// Cambia el layermask destino de cada componente del jugador
    /// </summary>
    /// <param name="newSup"> es equivalente a sup (Esta en piso superior?) </param>
    [PunRPC]
    public void ChangePlayerFloor(bool newSup)
    {
        sup = newSup;

        foreach (GameObject obj in _objLy)
        {
            if (!sup)
            {
                obj.layer = LayerMask.NameToLayer("PlayerInferior");
            }
            else
            {
                obj.layer = LayerMask.NameToLayer("PlayerSuperior");
            }
        }
    }
}
