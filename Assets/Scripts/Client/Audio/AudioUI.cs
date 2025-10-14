using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona la interfaz de usuario para el control de volumen de la música y los efectos de sonido.
/// Se conecta con el <see cref="AudioManager.Instance"/> para aplicar los cambios de volumen.
/// </summary>
public class AudioUI : MonoBehaviour
{
    public float DefaultVolume = 0.5f;
    public Slider _musicSlider, _sfxslider;
    
    /// <summary>
    /// Se llama cuando el script es cargado. Se utiliza para inicializar los sliders con un valor
    /// predeterminado y para buscar sus referencias en la jerarquía de la escena.
    /// </summary>
    private void Awake()
    {
        _sfxslider.value = DefaultVolume;
        _musicSlider.value = DefaultVolume;
        _musicSlider = GameObject.Find("/Canvas/OptionsMenu/MusicSlider").GetComponent<Slider>();
        _sfxslider = GameObject.Find("/Canvas/OptionsMenu/SFXSlider").GetComponent<Slider>();


    }
    
    /// <summary>
    /// Se llama al inicio del ciclo de vida del script.
    /// Carga los volúmenes guardados del <see cref="AudioManager.Instance"/> y los aplica a los sliders.
    /// También desactiva el GameObject al que está adjunto este script por defecto. 
    /// </summary>
    private void Start()
    {
        _sfxslider.value = AudioManager.Instance.SavedSFXVolume;
        _musicSlider.value = AudioManager.Instance.SavedMusicVolume;
        gameObject.SetActive(false);

    }

    /// <summary>
    /// Se invoca cuando el valor del slider de música cambia.
    /// Llama al método <see cref="AudioManager.MusicVolume(float)"/> para actualizar el volumen de la música.
    /// Este método debe ser asignado al evento OnValueChanged del slider de música en el Inspector.
    /// </summary>
    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }
    
    /// <summary>
    /// Se invoca cuando el valor del slider de efectos de sonidos cambia.
    /// Llama al método <see cref="AudioManager.SFXVolume(float)"/> para actualizar el volumen de los SFX.
    /// Este método debe ser asignado al evento OnValueChanged del slider de SFX en el inspector.
    /// </summary>
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxslider.value);
    }
}
