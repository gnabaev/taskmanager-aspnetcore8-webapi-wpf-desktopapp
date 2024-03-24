using System.Security.Claims;
using System.Text;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services
{
    public class UsersService : ICommonService<UserModel>
    {
        private readonly ApplicationContext _db;

        public UsersService(ApplicationContext db)
        {
            _db = db;
        }

        public Tuple<string, string> GetUserLoginPassFromBasicAuth(HttpRequest request)
        {
            string userName = "";
            string userPass = "";
            string authHeader = request.Headers["Authorization"].ToString();

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUserNamePass = authHeader.Replace("Basic ", "");
                var encoding = Encoding.GetEncoding("iso-8859-1");

                string[] namePassArray = encoding.GetString(Convert.FromBase64String(encodedUserNamePass)).Split(':');
                userName = namePassArray[0];
                userPass = namePassArray[1];
            }

            return new Tuple<string, string>(userName, userPass);
        }

        public ClaimsIdentity GetIdentity(string username, string password)
        {
            User currentUser = Get(username, password);
            if (currentUser != null)
            {
                currentUser.LastLoginDate = DateTime.Now;
                _db.Users.Update(currentUser);
                _db.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, currentUser.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, currentUser.Status.ToString())
                };

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }

        public User Get(string login, string password)
        {
            return _db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
        }

        public User Get(string login)
        {
            return _db.Users.FirstOrDefault(u => u.Email == login);
        }

        public bool Create(UserModel model)
        {
            try
            {
                var newUser = new User(model.FirstName, model.LastName, model.Email, model.Password,
                    model.Status, model.Phone, model.Photo);

                _db.Users.Add(newUser);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(int id, UserModel model)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Id == id);

            if (existingUser != null)
            {
                try
                {
                    existingUser.FirstName = model.FirstName;
                    existingUser.LastName = model.LastName;
                    existingUser.Email = model.Email;
                    existingUser.Password = model.Password;
                    existingUser.Phone = model.Phone;
                    existingUser.Photo = model.Photo;
                    existingUser.Status = model.Status;

                    _db.Users.Update(existingUser);
                    _db.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public bool Delete(int id)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Id == id);

            if (existingUser != null)
            {
                try
                {
                    _db.Users.Remove(existingUser);
                    _db.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public UserModel Get(int id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id);
            return user?.ToDto();
        }

        public IEnumerable<UserModel> GetAllByIds(List<int> userIds)
        {
            foreach (int id in userIds)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == id).ToDto();
                yield return user;
            }
        }
    }
}
