using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Photon.Pun;
using System;
using Unity.VisualScripting;

/// <summary>
/// Controla el movimiento del jugador, la interacción con la UI, la colisión con otros objetos y la
/// sincronización de movimiento en un entorno multijugador.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private PlayerAnimation _animators;
    [SerializeField] private CapsuleCollider2D _colider;
     public ChangeLayerLogic _chLayerLogic;

    public PlayerAnimation PlayerAnimation { get { return _animators; } }
    ClientUIM _UIMCanvas;
    [Header("Variables")]
    [SerializeField] private float _walkSpeed = 3f;

    //Photon
    PhotonView view;

    public Vector2 _movement;

    /// <summary>
    /// Se llama al inicio, después de Awake.Obtiene la referencia al <see cref="PhotonView"/>.
    /// </summary>
    private void Start()
    {

        view = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Inicializa las referencias a los componentes del jugador.
    /// Este método debe ser llamado explícitamente después de la creación del objeto.
    /// </summary>
    public void Initialize()
    {
        _colider = GetComponent<CapsuleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _UIMCanvas = GetComponent<ClientUIM>();
        _animators = GetComponent<PlayerAnimation>();
        _chLayerLogic = GetComponent<ChangeLayerLogic>();
    }

    /// <summary>
    /// Se llama una vez por frame. Maneja la entrada de moviemiento si este es el jugador local.
    /// </summary>
    private void Update()
    {

        if (view.IsMine)
        {
            Movement();
        }
    }

    bool CanX = true; 
    bool CanY = true;

    public Vector2 Mov = new Vector2(); // Vector temporal para almacenar la entrada de movimiento cruda del usuario.
    public Vector2[] StackArray = new Vector2[3]; /* Un array para almacenar un historial de los últimos movimientos,
                                                    posiblemente para lógica de colisiones. */

    /// <summary>
    /// Procesa la entrada del jugador para determinar el movimiento.
    /// Actualiza el vector de movimiento y controla las animaciones.
    /// Considera si el chat está activo para detener el movimiento.
    /// </summary>
    private void Movement()
    {
        
        //Seteamos las variables horizontal y vertical del input a nuestro Vector de movimiento
        Mov = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


        if (CanX && !ChatManager.instance.isActiveChat)
        {
            _movement.x = Mov.x * _walkSpeed;
        }
        else
        {
            _movement.x = 0;
        }

        if (CanY && !ChatManager.instance.isActiveChat)
        {
            _movement.y = Mov.y * _walkSpeed;
        }
        else
        {
            _movement.y = 0;
        }

        _animators.Animations(_movement.x / _walkSpeed, _movement.y / _walkSpeed);

        StackArray = movements.ToArray();

        if (StackArray.Length >= 3 && (!CanX || !CanY))
        {
            if (Mov != StackArray[2])
            {
                CanY = true;
                CanX = true;
            }
        }
    }
    
    /// <summary>
    /// Se llama a intervalosde tiempos fijos y uniformes. Se utiliza para aplicar fuerzas físicas.
    /// </summary>
    private void FixedUpdate()
    {
        SetVelocity();
    }

    /// <summary>
    /// Aplica el vector de movimiento actual (_movement) al <see cref="Rigidbody2D"/> del jugador.
    /// </summary>
    private void SetVelocity() 
    {
        _rb.velocity = _movement;
    }

    /// <summary>
    /// Mueve el jugador a una posición objetivo de forma suave.
    /// Desactiva el collider y restringe el movimiento durante la transición.
    /// Comúnmente usado para transiciones de escaleras o teletransporte suave.
    /// </summary>
    /// <param name="target">La posición <see cref="Vector3"/> a la que el jugdor debe moverse. </param>
    /// <param name="st">El tipo de escaleras o dirección de la animación durante el moviemiento. </param>
    /// <returns> Un <see cref="IEnumerator"/> para ser usado como una corutina. </returns>
    public IEnumerator MoveToPosition(Vector3 target, StairsType st)
    {
        
        _colider.enabled = false;
        CanX = false;
        CanY = false;

        float time = 0f;
        Vector3 startPosition = transform.position;

        while (time < 1f)
        {
            _animators.AnimAuto(st);
            time += Time.deltaTime; 
            transform.position = Vector3.Lerp(startPosition, target, time); 
            yield return null;
        }
        transform.position = target;

        _animators.Animations(0,0);
        _colider.enabled = true;
        CanX = true;
        CanY = true;
    }

    /// <summary>
    /// Detecta la dirección de las escaleras y calcula la posición ojetivo y el tipo de animación.
    /// Inicia la corutina <see cref="MoveToPosition(Vector 3, StairsType)"/> para mover al jugador.
    /// </summary>
    /// <param name="st">La lógica de las escaleras (<see cref="StairsLogic"/> que colisionó. </param>
    /// <param name="b">Un bool que indica la dirección de entrada/salida en la escalera. </param>
    /// <param name="obj">La posición <see cref="Vector2"/> del objeto de la escalera. </param>
    private void DetectDirection(StairsLogic st, bool b, Vector2 obj)
    {
        Vector3 TargetDest = new Vector3();
        StairsType _temp;
        switch (st.type)
        {
            case StairsType.Top:
                if (b)
                {
                    TargetDest = new Vector3(obj.x, obj.y +1.5f, this.transform.position.z +1);
                    _temp = StairsType.Top;
                }
                else
                {
                    TargetDest = new Vector3(obj.x, obj.y -1.5f, this.transform.position.z-1);
                    _temp = StairsType.Down;
                }
                break;
            case StairsType.Down:
                if (b)
                {
                    TargetDest = new Vector3(obj.x, obj.y -1.5f, this.transform.position.z + 1);
                    _temp = StairsType.Down;
                }
                else
                {
                    TargetDest = new Vector3(obj.x, obj.y +1.5f, this.transform.position.z-1);
                    _temp = StairsType.Top;
                }
                break;
            case StairsType.Left:
                if (b)
                {
                    TargetDest = new Vector3(obj.x -1.5f, obj.y, this.transform.position.z + 1);
                    _temp = StairsType.Left;
                }
                else
                {
                    TargetDest = new Vector3(obj.x +1.5f, obj.y, this.transform.position.z-1);
                    _temp = StairsType.Right;
                }
                break;
            case StairsType.Right:
                if (b)
                {
                    TargetDest = new Vector3(obj.x +1.5f, obj.y, this.transform.position.z + 1);
                    _temp = StairsType.Right;
                }
                else
                {
                    TargetDest = new Vector3(obj.x - 1.5f, obj.y, this.transform.position.z - 1);
                    _temp = StairsType.Left;
                }
                break;
            default:
                TargetDest = transform.position + new Vector3(0, 0, 0); 
                _temp = StairsType.Top;
                break;
        }

        if (view.IsMine)
        {
            _chLayerLogic.StartLevelC(TargetDest.z);
        }



        StartCoroutine(MoveToPosition(TargetDest,_temp));

    }

    /// <summary>
    /// Pasa una referencia de GameObject al componente <see cref="ClientUIM"/>.
    /// Probablemente para que el UI pueda interactuar con ese objeto.
    /// </summary>
    /// <param name="obj">El GameObject a pasar.</param>
    public void PassVar(GameObject obj)
    {
        _UIMCanvas.SetNameObj(obj);
    }

    /// <summary>
    /// Se llama cuando este collider 2D entra en contacto con otro collider 2D marcado como Trigger.
    /// Maneja interacciones con sillas, NPCs y escaleras.
    /// </summary>
    /// <param name="collision">El <see cref="Collider2D"/> del objeto con el que se colisionó.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Silla"))
        {
            _UIMCanvas.Notify(1);
        }

        if (collision.CompareTag("NPC"))
        {
            _UIMCanvas.GetDialog(collision.GetComponent<NPCDialog>());
        }

        if (collision.transform.CompareTag("Stairs"))
        {
            StairsLogic logic = collision.GetComponent<StairsLogic>();
            Vector2 entrada = transform.position;
            Vector2 obj = collision.transform.position;

            DetectDirection(logic, logic.UpDown(entrada), obj);

        }
    }

/// <summary>
/// Se llama cuando este collider 2D deja de estar en contacto con otro collider 2D marcado como Trigger.
/// Maneja la salida de interacciones como sillas y NPCs.
/// </summary>
/// <param name="collision">El <see cref="Collider2D"/> del objeto que dejó de colisionar. </param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Silla"))
        {
            _UIMCanvas.Notify(0);
        }

        if (collision.CompareTag("NPC"))
        {
            _UIMCanvas.UnSetDialog();
        }
    }

    //Physics Logic Player - Player

    Queue<Vector2> movements = new Queue<Vector2>();

    /// <summary>
    /// Se llama una vez por cada frame que un collider permanece un contacto con otro collider 2D.
    /// Implementa lógica para ajustar el movimiento cuando se colisiona con otro jugador, potencialmente para
    /// evitar que se "atasquen"
    /// </summary>
    /// <param name="collision">La info de la colisión</param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision != null && collision.transform.CompareTag("Player"))
        {
            Vector2 dif = collision.transform.position - transform.position;

            if (Math.Abs(dif.x) < Math.Abs(dif.y) && CanY)
            {
                CanY = false;
                CanX = true;
            } else if (Math.Abs(dif.x) > Math.Abs(dif.y) && CanX)
            {
                CanY = true;
                CanX = false;
            }

            movements.Enqueue(Mov);

            if (movements.Count > 3)
            {
                movements.Dequeue();
            }

        }

        
    }
    
    /// <summary>
    /// Se llama cuando este collider 2D deja de estar en contacto con otro collider 2D.
    /// Restablece la capacidad de movimiento en ambos ejes cuando el jugador deja de colisionar con otro jugador.
    /// </summary>
    /// <param name="collision">La info de la colisión. </param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && collision != null)
        {
            CanY = true;
            CanX = true;
        }
    }
}

