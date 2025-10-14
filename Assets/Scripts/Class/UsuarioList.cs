using System;

/// <summary>
/// Una clase contenedora para una colección de objetos <see cref="Usuario"/>.
/// Utilizada para serializar y deserializar una lista de usuarios.
/// </summary>
[System.Serializable]
public class UsuarioList
{
    public Usuario[] usuarios; 
}
