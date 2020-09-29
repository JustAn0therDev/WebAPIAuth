using System.Threading.Tasks;

namespace WebAPIAuth.Interfaces {
    public interface IRequestObserver {
        ValueTask OnNotified(int userSessionId);
    }
}