using System;
using System.Collections.Generic;
using Etapa2.Models;

namespace Etapa2.Repository
{
    public class CifraMemoryRepository : IRepository<User, int>
    {
        private readonly IDictionary<int, User> _repo = new Dictionary<int, User>();
        private readonly Random _rand = new Random();

        public IEnumerable<User> GetAll() 
        {
            return _repo.Values;
        }

        public User GetById(int num) 
        {
            User user;
            _repo.TryGetValue(num, out user);
            return user;
        }

        public void Remove(int key) 
        {
            _repo.Remove(key);
        }

        public int Add(User elem)
        {
            int num = _rand.Next();
            _repo.Add(num, elem);
            return num;
        }
    }
}