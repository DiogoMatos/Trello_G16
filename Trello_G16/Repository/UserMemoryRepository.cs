using Etapa2.Models;
using System.Collections.Generic;
using System.Linq;

namespace Etapa2.Repository
{
    public class UserMemoryRepository : IRepository<User,string>
    {
        private readonly IDictionary<string, User> _repo = new Dictionary<string, User>();

        public UserMemoryRepository() 
        {
            Add(new User { Name = "Diogo", Nickname = "Diogo", Email = "diogo@diogo.pt", Password = "xxxxx", Type = "admin", Id = 0 });
            Add(new User { Name = "Pedro", Nickname = "Pedro", Email = "pedro@pedro.pt", Password = "xxxxx", Type = "user", Id = 0 });
            Add(new User { Name = "Susana", Nickname = "Susana", Email = "susana@susana.pt", Password = "xxxxx", Type = "user", Id = 0 });
        }

        public IEnumerable<User> GetAll()
        {
            return _repo.Values;
        }

        public User GetById(string nickname)
        {
            return (nickname !=null && _repo.ContainsKey(nickname)) ? _repo[nickname] : null;
        }

        public string Add(User user)
        {
            if(_repo.ContainsKey(user.Nickname)) 
                return null;
            _repo.Add(user.Nickname, user);
            return user.Nickname;
        }

        public void Remove(User user) 
        {
            _repo.Remove(user.Nickname);
        }

        public IEnumerable<User> GetAllUsersExceptAdmins()
        {
            return _repo.Values.Where(u => u.Type.Equals("user"));
        }

        public bool VerifyNameAvailability(string name)
        {
            return !_repo.ContainsKey(name);

        }

        public bool DoesUserExist(string name)
        {
            return _repo.Values.Select(user => user.Name).Contains(name);

        }
    }
}