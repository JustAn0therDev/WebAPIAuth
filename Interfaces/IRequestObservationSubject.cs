using System.Threading.Tasks;

namespace WebAPIAuth.Interfaces {
    public interface IRequestObservationSubject {
        ValueTask NotifyObserverAsync(int userSessionId);
    }
}