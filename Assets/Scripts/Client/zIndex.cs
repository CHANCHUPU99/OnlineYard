using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gestiona la coordenada z (profundidad) de un objeto en el espacio 3D,
/// permitiendo modificarla para controlar el orden de renderizado en juegos 2D con perspectiva.
/// </summary>
public class zIndex : MonoBehaviour
{
    public float zIn;

    /// <summary>
    /// Se llama una vez al inicio del clico de vida del script.
    /// Inicializa <see cref="zIn"/> con la poición Z actual del transform del objeto.
    /// </summary>
    void Start()
    {
        zIn = transform.position.z;
    }

    /// <summary>
    /// Incrementa la coordenada Z del objeto en 1 unidad.
    /// Esto moverá el objeto "más lejos" en el eje Z si se usa en un contexo de perspectiva,
    /// o cambiará el orden de renderizado si la cámara lo interpreta así.
    /// </summary>
    public void zSum()
    {
        zIn++;
        transform.position = new Vector3(transform.position.x,transform.position.y,zIn);
    }
    
    /// <summary>
    /// Decrementa la coordenada Z del objeto en 1 unidad.
    /// Esto moverá el objeto "más cerca" en el eje Z si se usa en un contexto de perspectiva,
    /// o cambiará el orden de renderizado.
    /// </summary>
    public void zRes()
    {
        zIn--;
        transform.position = new Vector3(transform.position.x,transform.position.y,zIn);
    }


}
