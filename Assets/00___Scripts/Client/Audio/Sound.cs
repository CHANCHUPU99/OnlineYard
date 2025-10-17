using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa una definición de sonido individual, asociando un nombre con un clip de audio.
/// Esta clase es serializable, lo que permite configurarla directamente desde el Inspector de Unity.
/// </summary>
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
