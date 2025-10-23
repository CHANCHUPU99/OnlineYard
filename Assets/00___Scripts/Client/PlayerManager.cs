using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;
using JsonHelp;
using System.IO;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;
using Unity.Burst.CompilerServices;
using PlayFab;
using PlayFab.ClientModels;

/// <summary>
/// Gestiona la información del jugador, su apariencia (ropa), movimiento y sincronización de datos a través
/// de la red utilizando Photon PUN.
/// </summary>
public class PlayerManager : MonoBehaviourPunCallbacks
{
    #region Variables
    [SerializeField] PlayerMovement plMov;
    [SerializeField] TextMeshPro publicName;
    [Header("User Info")]
    [SerializeField] private SelectedClothes[] selectedClothes;
    public SelectedClothes SkinSl { get => selectedClothes[0]; set => selectedClothes[0] = value; }
    [SerializeField] private string _name;
    public string Name { get => _name; set => _name = value; }
    [SerializeField] private string _claseActual;
    public string Materia { get => _claseActual; set => _claseActual = value; }
    private PhotonView _photonView;
    public Camera _camera;
    #endregion

    /// <summary>
    /// Se llama el script cargador. Se utiliza para obtener la referencia al componente <see cref="PhotonView"/>.
    /// </summary>
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Se llama al inicio, después de Awake.
    /// Inicializa el jugador, configura su componente <see cref="PlayerMovement"/>,
    /// carga la vestimenta, y maneja la lógica específica para el jugador local (asignación de TagObject,
    /// activación de cámara, sincronización inicial).
    /// </summary>
    void Start()
    {
        EntryRoom();

        publicName = GetComponentInChildren<TextMeshPro>();

        // Valida PlayerMovement
        plMov = GetComponent<PlayerMovement>();
        if (plMov == null)
        {
            Debug.LogError("PlayerMovement no encontrado en los hijos de PlayerManager.");
            return;
        }

        // Validar `PlayerAnimation`
        if (plMov.PlayerAnimation == null)
        {
            Debug.LogError("PlayerAnimation no asignado en PlayerMovement.");
            return;
        }

        // Inicializar `selectedClothes`
        if (selectedClothes == null || selectedClothes.Length == 0)
        {
            selectedClothes = new SelectedClothes[1];
            selectedClothes[0] = new SelectedClothes();
            Debug.Log("selectedClothes inicializado con valores predeterminados.");
        }

        if (photonView.IsMine)
        {
            // Asigna `TagObject` al jugador local
            PhotonNetwork.LocalPlayer.TagObject = gameObject;
            Debug.Log($"TagObject asignado: {PhotonNetwork.LocalPlayer.NickName} (Objeto: {gameObject.name})");

            // Notificar a los dem�s jugadores sobre este `TagObject`
            photonView.RPC("RPC_AssignTagObject", RpcTarget.OthersBuffered, photonView.ViewID);

            // Activa la c�mara solo si pertenece al jugador local
            if (_camera == null)
            {
                Debug.LogError(" No se asign� una c�mara al jugador.");
                return;
            }
            _camera.gameObject.SetActive(true);

            ApplyLocalDress();
            NotifyOtherPlayersOfDress();
            SetNameInOthers();
        }
        else
        {
            // Desactiva la c�mara para otros jugadores
            if (_camera != null)
            {
                _camera.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Método RPC que se llana en los otros clientes para sincronizar el TagObject.
    /// Asigna el GameObject de la PhotonView correspondiente al TagObject de su propietario.
    /// </summary>
    /// <param name="viewID">El PhotonView ID del objeto cuyo TagObject debe ser asignado.</param>
    [PunRPC]
    void RPC_AssignTagObject(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            targetView.Owner.TagObject = targetView.gameObject;
            Debug.Log($"TagObject sincronizado para {targetView.Owner.NickName} (ID: {viewID})");
        }
        else
        {
            Debug.LogError($"No se pudo encontrar el PhotonView para el ID {viewID}");
        }
    }

    /// <summary>
    /// Inicializa la clase comprobando la existencia de un archivo JSON de skin y luego buscando la info
    /// de la vetsimenta.
    /// </summary>
    public void Initilize()
    {
        JsonHelper.Comprobacion();
        InfoSearch();
    }

    /// <summary>
    /// Carga la info de la vestimenta del jugador desde un archivo JSON local.
    /// El archivo se espera en Application.streamingAssetsPath + "/SelectedSkin.json".
    /// </summary>
    public void InfoSearch()
    {
        // Recopilar informaci�n
        string url = Application.streamingAssetsPath + "/SelectedSkin.json";
        string json = File.ReadAllText(url);
        selectedClothes = JsonHelper.FromJson<SelectedClothes>(json);
    }

    /// <summary>
    /// Viste el jugador aplicando un objeto <see cref="selectedClothes"/> proporcionado.
    /// También inicializa el componente <see cref="PlayerMovement"/> después de vestir.
    /// </summary>
    /// <param name="sl">El objeto <see cref="SelectedClothes"/> que define la vestimenta.</param>
    public void Dress(SelectedClothes sl)
    {
        plMov.PlayerAnimation.GetDress(sl);
        plMov.Initialize();
    }

    /// <summary>
    /// Viste al jugador utilizando la vestimeneta almacenada en el primer elemento del array <see cref="selectedClothes"/>.
    /// También inicializa el componente <see cref="PlayerMovement"/> después de vestir.
    /// </summary>
    public void Dress()
    {
        plMov.PlayerAnimation.GetDress(selectedClothes[0].skin, selectedClothes[0].hair, selectedClothes[0].shirt, selectedClothes[0].pants, selectedClothes[0].shoes, selectedClothes[0].acce);
        plMov.Initialize();
    }

    /// <summary>
    /// Realiza acciones al entrar a una sala (de juego o lobby).
    /// Inicializa <see cref="PlayerMovement"/>, registra este <see cref="PlayerManager"/> con <see cref="InfoServerManager"/>,
    /// y pasa la referencia del GameObject padre del nombre público al <see cref="PlayerMovement"/>.
    /// </summary>
    void EntryRoom()
    {
        plMov = GetComponentInChildren<PlayerMovement>();
        plMov.Initialize();
        InfoServerManager.Instance.RegisterPlayer(this);
        plMov.PassVar(publicName.transform.parent.gameObject);
    }

    /// <summary>
    /// Realiza acciones al salir de una sala.
    /// Desregistra este <see cref="PlayerManager"/> del <see cref="InfoServerManager"/>.
    /// </summary>
    void ExitRoom()
    {
        InfoServerManager.Instance.UnRegisterPlayer(this);
    }

    /// <summary>
    /// Aplica la vestimenta almacenada localmente al <see cref="PlayerAnimation"/> del jugador.
    /// </summary>
    private void ApplyLocalDress()
    {
        if (selectedClothes.Length > 0)
        {
            var clothes = selectedClothes[0];
            plMov.PlayerAnimation.GetDress(clothes.skin, clothes.hair, clothes.shirt, clothes.pants, clothes.shoes,clothes.acce);
        }
    }

    /// <summary>
    /// Sincorniza el nombre del jugdaor local con todos los demás clientes en la red utilizando un RPC.
    /// </summary>
    private void SetNameInOthers()
    {
        photonView.RPC("SyncName", RpcTarget.AllBuffered, photonView.ViewID);
    }

    /// <summary>
    /// Método RPC que se llama en todos los clientes para sincronizar el nombre del jugador.
    /// Inicia una corutina para establecer el nombre.
    /// </summary>
    /// <param name="viewID">El PhotonView ID del jugador cuyo nombre se está sincronizando.</param>
    [PunRPC]
    private void SyncName(int viewID)
    {
        Debug.Log($"SyncName llamado para ViewID {viewID}");
        StartCoroutine(SetMyName(viewID));
    }

    /// <summary>
    /// Corutina que espera hasta que 'publicName' y '_name' no sean nulos, y luego el nombre
    /// público del jugador basándose en su ViewID.
    /// </summary>
    /// <param name="viewID">El PhotonView ID del jugador cuyo nombre se va a establecer.</param>
    /// <returns></returns>
    IEnumerator SetMyName(int viewID)
    {
        yield return new WaitUntil(() => publicName != null && _name != null);
        if (photonView.ViewID != viewID)
        {
            Debug.LogWarning($"ViewID {viewID} no coincide con el ViewID local {photonView.ViewID}");
            yield break;
        }
        Name = viewID.ToString("#########");
        publicName.text = viewID.ToString("#########");
    }

    /// <summary>
    /// Notifica a todos los demás jugadores sobre la vestimenta actual del jugador local mediante un RPC,
    /// enviando los IDs de las prendas.
    /// </summary>
    private void NotifyOtherPlayersOfDress()
    {
        if (selectedClothes.Length > 0)
        {
            var clothes = selectedClothes[0];
            photonView.RPC("SyncDress", RpcTarget.AllBuffered, photonView.ViewID, clothes.skin, clothes.hair, clothes.shirt, clothes.pants, clothes.shoes);
        }
    }

    /// <summary>
    /// Método RPC que se llama en todos los clientes para sincronizar la vestimenta de un jugador.
    /// Inicia una corutina apra aplicar ña vestimenta.
    /// </summary>
    /// <param name="viewID">El PhotonView ID del jugador cuya vestimenta se está sincronizando.</param>
    /// <param name="skin">ID de la piel.</param>
    /// <param name="hair">ID del cabello.</param>
    /// <param name="shirt">ID de la playera.</param>
    /// <param name="pants">ID de los pants.</param>
    /// <param name="shoes">ID de los zapatos.</param>
    [PunRPC]
    private void SyncDress(int viewID, int skin, int hair, int shirt, int pants, int shoes)
    {
        Debug.Log($"SyncDress llamado para ViewID {viewID}");
        StartCoroutine(WaitAndSync(viewID, skin, hair, shirt, pants, shoes));
    }

    /// <summary>
    /// Corutina que espera que los componentes necesarios (<see cref="plMov"/> y <see cref="plMov.PlayerAnimation"/>)
    /// est[en disponibles, y luegos aplica la vestimenta sincronizada a la animación del jugador.
    /// </summary>
    /// <param name="viewID">El PhotonView ID del jugador.</param>
    /// <param name="skin">ID de la piel a aplicar.</param>
    /// <param name="hair">ID del cabello a aplicar.</param>
    /// <param name="shirt">ID de la playera a aplicar.</param>
    /// <param name="pants">ID de los pants a aplicar.</param>
    /// <param name="shoes">ID de los zapatos a aplicar.</param>
    /// <returns></returns>
    private IEnumerator WaitAndSync(int viewID, int skin, int hair, int shirt, int pants, int shoes)
    {
        Debug.Log($" Iniciando WaitAndSync para ViewID {viewID}");

        yield return new WaitUntil(() => plMov != null && plMov.PlayerAnimation != null);

        if (photonView.ViewID != viewID)
        {
            Debug.LogWarning($"ViewID {viewID} no coincide con el ViewID local {photonView.ViewID}");
            yield break;
        }

        var clothes = selectedClothes[0];
        clothes.skin = skin;
        clothes.hair = hair;
        clothes.shirt = shirt;
        clothes.pants = pants;
        clothes.shoes = shoes;

        Debug.Log($"Aplicando vestimenta: Skin {skin}, Hair {hair}, Shirt {shirt}, Pants {pants}, Shoes {shoes}");
        plMov.PlayerAnimation.GetDress(clothes.skin, clothes.hair, clothes.shirt, clothes.pants, clothes.shoes, clothes.acce);

        Debug.Log($"Vestimenta sincronizada para ViewID {viewID}");
    }
}
