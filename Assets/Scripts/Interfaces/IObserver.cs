/// <summary>
/// Define un contrato para un "Observador" en el patrón de diseño Observador.
/// Cualquier clase que implemente esta interfaz es capaz de recibir notificaciones de un Sujeto
/// (<see cref="INotifications"/>) al que se ha suscrito.
/// </summary>
public interface IObserver
{
    public void Updated(INotifications notify, int idEvent);

    public void SuscribeUIM(INotifications notify);

}
