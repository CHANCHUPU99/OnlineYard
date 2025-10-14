using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa una categor[ia para organizar y agrupar difernetes tipos de ropa o accesorios en un sistema de armario.
/// Esta clase es serializable para poder ser confugurada directamente desde el inspector de Unity.
/// </summary>
[System.Serializable]
public class ClosetCategory
{
    public string name;
    public List<AnimatorOverrideController> oAnimators;

}
