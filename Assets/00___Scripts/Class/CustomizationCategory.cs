using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomizationCategory
{
    public string name; // Nombre de la categoría, para identificarla
    public List<Sprite> sprites = new List<Sprite>(); // Lista de opciones de sprites
}