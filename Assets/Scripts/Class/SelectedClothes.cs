
using System;

/// <summary>
/// Representa la selección actual de prendas de vestir y apariencia para un personaje.
/// Esta clase es serializable, lo que permite guardar y cargar la configuración de la ropa.
/// </summary>
[Serializable]
public class SelectedClothes
{
    #region Variables
    public int[] viewID;
    public int skin;
    public int hair;
    public int shirt;
    public int pants;
    public int shoes;
    public int acce;
    #endregion

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SelectedClothes"/>
    /// con todas las propiedades de ropa establecidas a 0 (por defecto o no seleccionado).
    /// </summary>
    public SelectedClothes()
    {
        skin = 0;
        hair = 0;
        shirt = 0;
        pants = 0;
        shoes = 0;
        acce = 0;
    }
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SelectedClothes"/>
    /// con IDs específicos para cada prenda y parte del cuerpo.
    /// </summary>
    /// <param name="a">ID para la piel.</param>
    /// <param name="b">ID para el cabello.</param>
    /// <param name="c">ID para la camisa.</param>
    /// <param name="d">ID para los pantalones.</param>
    /// <param name="e">ID para los zapatos.</param>
    /// <param name="f">ID para el accesorio.</param>
    public SelectedClothes(int a, int b, int c, int d, int e, int f)
    {
        skin = a;
        hair = b;
        shirt = c;
        pants = d;
        shoes = e;
        acce = f;
    }
    
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SelectedClothes"/>
    /// copiando los valores de otra instancia existente de <see cref="SelectedClothes"/>
    /// </summary>
    /// <param name="sl">La instancia de <see cref="SelectedClothes"/> de la cual copiar los valores.</param>
    public SelectedClothes( SelectedClothes sl)
    {
        skin = sl.skin;
        hair = sl.hair;
        shirt = sl.hair;
        pants = sl.pants;
        shoes = sl.shoes;
        acce = sl.acce;
    }
}
