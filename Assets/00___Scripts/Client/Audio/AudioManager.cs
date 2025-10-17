using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona la reproducción de música y efectos de sonido en todo el juego.
/// Implementa el patrón Singleton para asegurar que solo haya una instancia de AudioManager.
/// Permite reproducir sonidos por nombre, controlar el volumen y persistir la configuración de volumen.
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region Variable
    public static AudioManager Instance;
    public Sound[] musicSounds,sfxSounds;
    public AudioSource musicSource, sfxSource;

    public float defaultVolume = .5f;
    public float SavedMusicVolume = .5f;
    public float SavedSFXVolume = .5f;
    public bool init;
    #endregion


/// <summary>
/// Se llama cuando el script es cargado. Implementa la lógica del patrón Singleton:
/// asegura que solo una instancia de AudioManager exista y persiste entre escenas.
/// También carga los volúmenes de música y SFX guardados de <see cref="PlayerPrefs"/>.
/// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);

        }
            DontDestroyOnLoad(gameObject);

        SavedSFXVolume = PlayerPrefs.GetFloat("SFXVolume",defaultVolume);
        SavedMusicVolume = PlayerPrefs.GetFloat("MusicVolume",defaultVolume);
    }

/// <summary>
/// Guarda los volúmenes actuales de música y efectos de sonido en <see cref="PlayerPrefs"/>
/// para persistirlos entre sesiones de juego.
/// </summary>
    public void SetVolumePref()
    {
        PlayerPrefs.SetFloat("SFXVolume", SavedSFXVolume);
        PlayerPrefs.SetFloat("MusicVolume", SavedMusicVolume);
    }

/// <summary>
/// Se llama al inicio del ciclo de vida del script.
/// Inicia la carga de la música basada en la escena actual.
/// </summary>
   private void Start()
    {
        ChargeMusicLevel(SceneManager.GetActiveScene().buildIndex);
    }

/// <summary>
/// Carga y reproduce la música adecuada basándose en el índice de la escena actual.
/// Mapea indices de escena a nombres de pistas de música.
/// </summary>
/// <param name="musicType">El indice de la escena actual. </param>
    public void ChargeMusicLevel(int musicType)
    {
        switch (musicType)//(SceneManager.GetActiveScene().buildIndex)
        {
            case int n when n <= 3:
                PlayMusic("MainTheme");
                init = true;
                break;
            case 4:
                PlayMusic("Vestidor");
                init = true;
                break;
            case 5:
                PlayMusic("Salon");
                init = true;
                break;
            case 6:
                PlayMusic("Direccion");
                init = true;
                break;
            case 7:
                PlayMusic("Relax");
                init = true;
                break;
            default:
                PlayMusic("MainTheme");
                init = true;
                break;
        }
    }

/// <summary>
/// Busca una pista de música por su nombre y la reproduce en la fuente de música.
/// Si la pista no se encuentra, registra un mensaje de depuración.
/// </summary>
/// <param name="name">El nombre de la pista de música a reproducir. </param>
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    } 

/// <summary>
/// Busca un efecto de sonido por su nombre y lo reproduce una sola vez en la fuente de SFX.
/// SI el SFX no se encuentra, registra un mensaje de depuración.
/// </summary>
/// <param name="name">El nombre del efecto de sonido a reproducir.</param>
    public void PlaySounds(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

/// <summary>
/// Ajusta el volumen de la música. El volumen se aplica con una curva cuadrada para una percepción más lineal.
/// También guarda el nuevo volumen de la música.
/// </summary>
/// <param name="volume">El nuevo nivel de volumen paraa la música. </param>
    public void MusicVolume(float volume)
    {
        musicSource.volume = volume*volume;
        SavedMusicVolume = volume;
        SetVolumePref();
    }
/// <summary>
/// Ajusta el volumen de los efectos de sonidos. El volumen se aplica con una curva cuadrada para una percepción mas lineal.
/// También guarda el nuevo volumen de los SFX.
/// </summary>
/// <param name="volume"></param>
    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume*volume;
        SavedSFXVolume = volume;
        SetVolumePref();

    }
}
