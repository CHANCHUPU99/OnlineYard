/// <summary>
/// Define un contrato para un "Sujeto" u "Observado" en el patrón de diseño Observador.
/// Cualquier clase que implemente esta interfaz es capaz de mantener una lista de observadores y
/// notificarles sobre eventos específicos.
/// </summary>
public interface INotifications
{
    public void AddObserver(IObserver obs);
    void SuscribeNotification(IObserver observer);
    void UnSuscribeNotification(IObserver observer);
    void Notify(int idEvent);

}