using System.Threading.Tasks;

namespace WebAPIAuth.Interfaces {
    public interface IRequestObserver {
        ValueTask OnNotifiedAsync(int userSessionId);
    }
}
