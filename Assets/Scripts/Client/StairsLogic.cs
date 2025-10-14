using UnityEngine;

/// <summary>
/// Gestiona la lógica de las escaleras en el juego, determinando si un jugador está subiendo/bajando
/// o moviéndose a través de ellas, basándose en la posición de entrada.
/// </summary>
public class StairsLogic : MonoBehaviour 
{
    [Header("Tipo de Escalera")]
    public StairsType type;

    /// <summary>
    /// Determina la dirección relativa de entrada a la escalera basándose en la posición del jugador.
    /// Por ejemplo, para una escalera 'Top', retorna true si la posición está por debajo de las escaleras,
    /// indicando que se está "subiendo" a ella.
    /// </summary>
    /// <param name="position1">La posición del jugador al interactuar con la escalera. </param>
    /// <returns>
    /// <c>true</c> si la posición indica que el jugador está moviendose en la dirección "positiva"
    /// de la escalera (subiedo, yendo a la derecha, etc); de lo contrario, <c>false</c>
    /// </returns>
    public bool UpDown(Vector2 position1)
    {
        float x = position1.x;
        float y = position1.y;

        switch (type)
        {
            case StairsType.Top:

                return y < this.transform.position.y;

            case StairsType.Down:

                return y > this.transform.position.y;

            case StairsType.Left:

                return x > this.transform.position.x;

            case StairsType.Right:

                return x < this.transform.position.x;

            default:
                    Debug.LogError("Type Not Assingned" + this.name.ToString());
                return false;

        }
    }

}
