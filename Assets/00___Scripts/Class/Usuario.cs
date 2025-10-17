using System;

/// <summary>
/// Representa la informaci[on básica de un usuario dentro del sistema.
/// Esta clase es serializable, lo que permite guardar y recuperar datos del usuario.
/// </summary>
[System.Serializable]
public class Usuario
{
    public int id;
    public string nombre;
    public string email;
    public string tipo_usuario;
    public string fecha_registro;
}
