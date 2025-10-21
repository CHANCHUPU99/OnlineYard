using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using JsonHelp;
using PlayFab.ClientModels;
using PlayFab;

/// <summary>
/// Gestiona la interfaz de usuario y la lógica para la selección de la skin de un personaje.
/// Permite al jugador elegir deiferentes elementos de vestimenta y gaurdar su selección.
/// </summary>
public class SkinSelectorManager : MonoBehaviour
{
    #region Variables
    public List<CustomizationCategory> categories;
    public SelectedClothes[] selectedSkins;
    private int cat = 0;
    private int val;
    public bool[] isSelected = new bool[5];
    Dictionary<int,Action> actions;

    [Header("UI Elements")]
    public Image[] srSelect;
    public List<Image> optionsUI;
    public TextMeshProUGUI text;
    #endregion
    
    /// <summary>
    /// Se llama al inicio del ciclo de la vida del script.
    /// Inicializa el diccionario de acciones y carga la información de la vestimenta guardada.
    /// </summary>
    private void Start()
    {
        actions = new Dictionary<int, Action>
        {
            { 0, () => SelectOption(selectedSkins[0].skin ) },
            { 1, () => SelectOption(selectedSkins[0].hair ) },
            { 2, () => SelectOption(selectedSkins[0].shirt) },
            { 3, () => SelectOption(selectedSkins[0].pants) },
            { 4, () => SelectOption(selectedSkins[0].shoes) },
            { 5, () => SelectOption(selectedSkins[0].acce) },
        };
            
        ReadInfo();
    }

    /// <summary>
    /// Controla la visibilidad de las opciones de personalización y el texto asociado.
    /// Si una categoría ya está seleccionada, la deselecciona; de lo contrario, la selecciona.
    /// </summary>
    /// <param name="n">El indice de la categoria de personalización a mostrar/ocultar.</param>
    public void SelectShow(int n)
    {
        HideOption();

        for (int i = 0; i < isSelected.Length; i++)
        {
            isSelected[i] = isSelected[i] ? false : isSelected[i];
        }
        
        if (isSelected[n])
        {
            isSelected[n] = false;
            text.gameObject.SetActive(true);
        }
        else
        {
            isSelected[n] = true;
            text.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Oculta todos los GameObject asociados a las opcioens de personalización en la UI.
    /// </summary>
    public void HideOption()
    {
        for (int i = 0; i < optionsUI.Count; i++)
        {
            optionsUI[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Muestra las opciones de personalización para una categoría específica.
    /// Actualiza la categoría actual y rellena las imágenes de opciones de UI con los sprites de esa categoria.
    /// </summary>
    /// <param name="n">El índice de la categoría a mostrar.</param>
    public void ShowOption(int n)
    {
        SelectShow(n);
        cat = n;
        val = 0;
        foreach (var sprite in categories[cat].sprites)
        {
            if (val >= optionsUI.Count) break;
            optionsUI[val].gameObject.SetActive(true);
            optionsUI[val].sprite = sprite;
            val++;
        }

    }
    
    public Sprite Blank;
    /// <summary>
    /// Selecciona una opción específica (sprite) dentro de la categoría actualmente activa.
    /// Actualiza el ID de la prenda en <see cref="selectedSkins"/> y maneja la lógica de exclusión mutua
    /// entre cabello y accesorios si es necesario.
    /// </summary>
    /// <param name="n">El inidice de la opción (sprite) seleccionada dentro de la categoria actual.</param>
    public void SelectOption(int n)
    {
        UISprites(n);

        switch (cat)
        {
            case 0:
                selectedSkins[0].skin = n;
                Debug.Log(selectedSkins[0].skin);
                break;
            case 1:
                if (selectedSkins[0].acce > 2 && n != 0)
                {
                    selectedSkins[0].acce = 0;
                    UISprites(0, 5);
                }
                selectedSkins[0].hair = n;
                Debug.Log(selectedSkins[0].hair);

                break;
            case 2:
                selectedSkins[0].shirt = n;
                Debug.Log(selectedSkins[0].shirt);

                break;
            case 3:
                selectedSkins[0].pants = n;
                Debug.Log(selectedSkins[0].pants);

                break;
            case 4:
                selectedSkins[0].shoes = n;
                Debug.Log(selectedSkins[0].shoes);

                break;
            case 5:
                if (n > 2 && selectedSkins[0].hair != 0)
                {
                    selectedSkins[0].hair = 0;
                    UISprites(0, 1);
                }
                selectedSkins[0].acce = n;
                Debug.Log(selectedSkins[0].acce);
                break;
            default:
                Debug.Log("No category selected");
                break;
        }

    }

    /// <summary>
    /// Actualiza el sprite de la UI que muestra la selección actual para la categoría activa.
    /// Si la opción es 0 y la categoría es cabello o accesorio, usa un sprite en blanco.
    /// </summary>
    /// <param name="n">El indice del sprite a mostrar.</param>
    private void UISprites(int n)
    {
        if ((cat == 1 || cat == 5) && n == 0)
        {
            srSelect[cat].sprite = Blank;
        }
        else
        {
            srSelect[cat].sprite = categories[cat].sprites[n];
        }
    }
    
    /// <summary>
    /// Sobrecarga de <see cref="UISprites(int)"/> que permite especificar la categoría a actualizar.
    /// Útil para actualizar la UI de una categoría diferente a la actualmente activa.
    /// </summary>
    /// <param name="n">El indice del sprite a mostrar.</param>
    /// <param name="cat">El indice de la categoría a actualizar en la UI.</param>
    private void UISprites(int n, int cat)
    {
        if ((cat == 1 || cat == 5) && n == 0)
        {
            srSelect[cat].sprite = Blank;
        }
        else
        {
            srSelect[cat].sprite = categories[cat].sprites[n];
        }
    }

    /// <summary>
    /// Método invocado al "iniciar sesión" o confirmar la selección de vestimenta.
    /// Su función principal es guardar la selección actual.
    /// </summary>
    public void LogIn()
    {
        //Pasar info del player al player Manager
        SaveSelection();
        SceneManager.LoadScene("LoadingScreen_Test");
    }

    /// <summary>
    /// Guarda la selección actual de vestimenta (<see cref="selectedSkins"/>) en un archivo JSON.
    /// El archivo se guarda en la ruta de StreamingAssets.
    /// </summary>
    public void SaveSelection()
    {
        string json = JsonHelp.JsonHelper.ToJson(selectedSkins, true);
        string url = Application.streamingAssetsPath + "/SelectedSkin.json";
        File.WriteAllText(url, json);

        saveSelectionToPlayFab(json);
    }
    public void saveSelectionToPlayFab(string json)
    {
      //  string json = JsonHelp.JsonHelper.ToJson(selectedSkins, true);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { "SelectedSkin", json }
        }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log(" Skin guardada en PlayFab exitosamente."),
            error => Debug.LogError(" Error al guardar skin en PlayFab: " + error.GenerateErrorReport()));
    }

    public void applySavedSelection(string outfitName){
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
       result =>
       {
           if (result.Data != null && result.Data.ContainsKey("SelectedSkin"))
           {
               string json = result.Data["SelectedSkin"].Value;
               selectedSkins = JsonHelp.JsonHelper.FromJson<SelectedClothes>(json);
               AssingSprites();
               Debug.Log(" Skin cargada desde PlayFab correctamente.");
           }
           else
           {
               Debug.Log(" No se encontró ninguna skin guardada en PlayFab. Se usará la predeterminada.");
               ReGenSkin();
           }
       },
       error => Debug.LogError(" Error al cargar skin desde PlayFab: " + error.GenerateErrorReport())
   );
    }
    /// <summary>
    /// Regenera el directorio StreamingAssets si no existe.
    /// </summary>
    public void ReGenDirectory()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath);
    }
    
    /// <summary>
    /// Reinicia la seleccion de vestimenta a un estado por defecto (todos los IDs a 0)
    /// y luego guarda esta nueva selección.
    /// </summary>
    public void ReGenSkin()
    {
        selectedSkins = new SelectedClothes[1];
        selectedSkins[0] = new SelectedClothes();
        SaveSelection();
    }

    /// <summary>
    /// Lee la info. de la vestimenta guardada desde un archivo JSON.
    /// Si el directorio o el archivo no existen, los crea o los regenera.
    /// Luego, asigna los sprites correspondientes a la UI.
    /// </summary>
    public void ReadInfo()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Debug.LogWarning("Directory not found: StreamingAssets");
            Debug.LogWarning("Creating Directory StreamingAssets");
            ReGenDirectory();
        }

        string url = Application.streamingAssetsPath + "/SelectedSkin.json";
        if (!File.Exists(url))
        {
            Debug.LogWarning("File not found: " + url);
            Debug.Log("Creating a new skin");
            ReGenSkin();
            return;
        }

        AssignSpritesUI(url);
    }

    /// <summary>
    /// Lee el archivo JSON a la URL proporcionada, deserializa la información de la vestimenta.
    /// y luego llama a <see cref="AssingSprites"/> para actualizar la UI con los sprites guardados.
    /// Maneja posibles errores de lectura o parseo del JSON.
    /// </summary>
    /// <param name="url">La ruta completa del archivo JSON a leer.</param>
    private void AssignSpritesUI(string url)
    {
        try
        {
            string json = File.ReadAllText(url);
            selectedSkins = JsonHelper.FromJson<SelectedClothes>(json);
            AssingSprites();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to read or parse JSON: " + ex.Message);
        }
    }

    /// <summary>
    /// Asigna los sprites de las prendas seleccionadas a los elementos de UI de visualización.
    /// Itera a través de las categorías y usa el direccionario de acciones para aplicar la selección.
    /// </summary>
    private void AssingSprites()
    {
        for (int i = 0; i < 6; i++)
        {
            cat = i;
            if (actions.ContainsKey(cat))
            {
                actions[cat].Invoke();
            }
            else
            {
                Debug.LogError("Invalid category");
            }
        }
    }
}

namespace JsonHelp
{
    /// <summary>
    /// Clase estática de utilidad para serializar y deserializar arrays de objetos a/desde JSON.,
    /// utilizando la funcionalidad <see cref="UnityEngine.JsonUtility"/>
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Deserealiza una cadeha JSON que contiene un array de objetos de tipo <typeparamref name="T"/>.
        /// Requiere que el JSON esté envuelto en una estructura en una estructura con un campo "Items".
        /// </summary>
        /// <param name="json">La cadena JSON a deserializar.</param>
        /// <typeparam name="T">El tipo de los objetos dentro del array.</typeparam>
        /// <returns>Un array de obejetos de tipo <typeparamref name="T"/>.</returns>
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }
    
        /// <summary>
        /// Serializa un array de objeto de tipo <typeparamref name="T"/> a una cadena JSON.
        /// El array se envuelve en una estructura con un campo "Items" para la serialización.
        /// </summary>
        /// <param name="array">El array de objetos a serialzizar.</param>
        /// <typeparam name="T">El tipo de los objetos dentro de array.</typeparam>
        /// <returns></returns>
        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }
    
        /// <summary>
        /// Serializa un array de objetos de tipo <typeparamref name="T"/> a una cadena JSON,
        /// con la opción de formatear la salida para que sea legible (pretty print).
        /// El array se envuelve en una estructura con un campo "Items" para la serialización.
        /// </summary>
        /// <param name="array">El array de objetos a serializar.</param>
        /// <param name="prettyPrint">Si es <c>true</c>, el JSON se formateará con identación para facilitar la lectura.</param>
        /// <typeparam name="T">El tipo de los objetos dentro del array.</typeparam>
        /// <returns>Una cadena JSON que representa el array.</returns>
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        /// <summary>
        /// Realiza una comprobación básica de la existencia del directorio StreamingAssets
        /// y del archivo "SelectedSkin.json" dentro de él.
        /// Registra advertencias o errores si no se encuentran.
        public static void Comprobacion()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Debug.LogWarning("Directory not found: StreamingAssets");
                Debug.LogWarning("Creating Directory StreamingAssets");
            }
             
            string url = Application.streamingAssetsPath + "/SelectedSkin.json";

            if (!File.Exists(url))
            {
                Debug.LogWarning("File not found: " + url);
                Debug.LogError("No Exist Skin File, Restart the app to fix");
            }
        }
    
        /// <summary>
        /// Una clase interna privada utilizada como un "wrapper" o envoltorio
        /// para serializar/deserializar arrays de objetos con <see cref="UnityEngine.JsonUtility"/>.
        /// <see cref="UnityEngine.JsonUtility"/> no puede serializar directamente arrays raíz.
        /// </summary>
        /// <typeparam name="T">El tipo de los elementos que contendrá el array.</typeparam>
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
