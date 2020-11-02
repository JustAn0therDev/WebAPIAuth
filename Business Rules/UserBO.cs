using System;
using System.Linq;
using WebAPIAuth.Utils;
using WebAPIAuth.Models;
using WebAPIAuth.Contexts;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WebAPIAuth.BusinessRules
{
    public static class UserBO {

        ///<summary>
        ///<description>Returns a complete list of all users</description>
        ///</summary>
        public static async ValueTask<IReadOnlyList<User>> GetAllUsersAsync() {
            IReadOnlyList<User> users = null;
            using (var connection = new DatabaseContext()) {
                var set = connection.Set<User>();
                users = await connection.User.ToListAsync();
            }
            return users;
        }

        ///<summary>
        ///<description>Returns the ID of the newly created</description>
        ///<paramref name="user"/>
        ///</summary>
        public static async ValueTask<int> CreateUserAsync(User user) {
            int newlySavedId;
            user.Password = user.Password.GetEncryptedString();

            if (user == null)
                throw new ArgumentNullException("A user must be provided.");
            else if (user.ID != 0)
                throw new InvalidOperationException("A new user must not have an ID.");

            using (var connection = new DatabaseContext()) {
                
                if (await connection.User.Where(w => user.Username == w.Username).CountAsync() > 0)
                    throw new InvalidOperationException("The provided username is already taken");

                var entity = await connection.User.AddAsync(user);
                await connection.SaveChangesAsync();
                newlySavedId = (int)entity.Property("ID").CurrentValue;
            }
            return newlySavedId;
        }

        ///<summary>
        ///<description>Returns a boolean value representing if the deleting operation was successful. Receives an</description>
        ///<paramref name="ID"/>
        ///</summary>
        public static async ValueTask<bool> DeleteUserAsync(int ID) {
            bool deleted = false;

            using (var connection = new DatabaseContext()) {
                List<User> users = await connection.User.Where(w => w.ID == ID).ToListAsync();

                if (users == null || users.Count == 0)
                    throw new InvalidOperationException("User not found");

                EntityEntry<User> entityEntry = null;
                await Task.Run(() => entityEntry = connection.Remove<User>(users.FirstOrDefault()));
                deleted = entityEntry.State == EntityState.Deleted ? true : false;

                await connection.SaveChangesAsync();
            }

            return deleted;
        }
    }
}
