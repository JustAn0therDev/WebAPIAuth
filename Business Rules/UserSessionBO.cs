using System;
using System.Linq;
using WebAPIAuth.Utils;
using WebAPIAuth.Models;
using WebAPIAuth.Contexts;
using WebAPIAuth.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAuth.BusinessRules
{
    public class UserSessionBO : IRequestObserver {

        ///<summary>
        ///<description>Method that should be called if the caller has an IRequestObservationSubject interface.</description>
        ///</summary>
        public async ValueTask OnNotifiedAsync(int userSessionId) {
            using (var connection = new DatabaseContext()) {
                UserSession session = await connection.UserSession
                .Where(userSession => userSession.ID == userSessionId)
                .FirstOrDefaultAsync();

                if (session == null || session.EndDate != null)
                    throw new UnauthorizedAccessException("Session not found");

                if (session.StartDate.AddMinutes(30) <= DateTime.Now)
                    if (await EndSessionAsync(userSessionId)) {
                        throw new UnauthorizedAccessException("Invalid session");
                    }
            }
        }

        ///<summary>
        ///<description>Starts user session by checking name and password.</description>
        ///</summary>
        public async ValueTask<int> StartSessionAsync(User user) {
            int userSessionID = 0;
            using (var connection = new DatabaseContext()) {
                user = await connection
                .User
                .Where(w => w.Username == user.Username && w.Password == user.Password.GetEncryptedString())
                .FirstOrDefaultAsync();

                if (user == null)
                    throw new ArgumentNullException("Invalid user for authentication");
                else if (await connection.UserSession.Where(w => w.UserID == user.ID && w.EndDate == null).CountAsync() > 0) 
                    throw new InvalidOperationException("User cannot authenticate with existing session");

                var entity = await connection.UserSession.AddAsync(new UserSession { UserID = user.ID, StartDate = DateTime.Now });
                await connection.SaveChangesAsync();
                if (entity != null) 
                    int.TryParse(entity.Property("ID").CurrentValue.ToString(), out userSessionID);
                 else 
                    throw new Exception("Something went wrong while creating a session. Please try again.");
            }
            return userSessionID;
        }

        ///<summary>
        ///<description>Ends the session found via the provided</description>
        ///<paramref name="userSessionId"/>
        ///</summary>
        public static async ValueTask<bool> EndSessionAsync(int userSessionId) {
            bool endedSession = false;
            using (var connection = new DatabaseContext()) {
                UserSession session = await connection.UserSession
                .Where(userSession => userSession.ID == userSessionId)
                .FirstOrDefaultAsync();

                if (session == null || session.EndDate != null)
                    throw new UnauthorizedAccessException("Session not found");
                
                session.EndDate = DateTime.Now;
                await connection.SaveChangesAsync();
                endedSession = true;
            }
            return endedSession;
        }
    }
}
