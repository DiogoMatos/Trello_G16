using System;
using System.Collections.Generic;
using Etapa2.Models;
using System.Linq;

namespace Etapa2.Repository
{
    class BoardMemoryRepository : IRepository<Board, int>
    {
        private readonly IDictionary<int, Board> _repo_id = new Dictionary<int, Board>();
        private readonly IDictionary<string, List<Board>> _repo_user = new Dictionary<string, List<Board>>();
        private int _cid = 1;
        
        public BoardMemoryRepository()
        {
            Add(new Board { Name = "PI", Description = "Trabalho de Programação na Internet", User = "Pedro", CreateDate = new DateTime(2012, 9, 27) });
            Add(new Board { Name = "PC S2", Description = "Série de execícios 2", User = "Pedro", CreateDate = new DateTime(2012, 9, 15) });
            Add(new Board { Name = "PC S3", Description = "Série de execícios 3", User = "Pedro", CreateDate = new DateTime(2012, 11, 27) });
            Add(new Board { Name = "Ing", Description = "Apresentação", User = "Pedro", CreateDate = new DateTime(2012, 11, 5) });

            Add(new Board { Name = "PS", Description = "Mobile Series", User = "Diogo", CreateDate = new DateTime(2013, 1, 03) });
            Add(new Board { Name = "My Home Project", Description = "", User = "Diogo", CreateDate = new DateTime(2011, 6, 17) });

            Add(new Board { Name = "CarQoute", Description = "Gestão da Frota", User = "Susana", CreateDate = new DateTime(2009, 2, 28) });
        }

        public IEnumerable<Board> GetAll()
        {
            return _repo_id.Values;
        }

        public IEnumerable<Board> GetBoardsThatUserCanUse(string user)
        {
            if (_repo_user.ContainsKey(user))
                return _repo_user[user];
            return new Board [0];
        }

        public Board GetById(int id)
        {
            Board b;
            _repo_id.TryGetValue(id, out b);
            return b;
        }

        public bool IsBoardIdFromUser(int id, string user)
        {
            Board b = GetById(id);
            if(b!=null)
                return b.User == user;
            return true;
        }

        public int Add(Board elem)
        {
            elem.Id = _cid;
            _repo_id.Add(_cid++, elem);
            if(!_repo_user.ContainsKey(elem.User))
                _repo_user.Add(elem.User, new List<Board>());
            _repo_user[elem.User].Add(elem);
            return elem.Id;
        }

        public bool IsBordFromUser(int id, string user)
        {
            return GetById(id).User == user;
        }

        public bool BoardNameExists(string name, string user)
        {
            List<Board> boards = _repo_user[user];
            foreach (Board board in boards)
            {
                if (board.User == user && board.Name == name)
                    return true;
            }
            return false;
        }

        public Board UpdateBoard(int id, string name, string description)
        {
            Board b = _repo_id[id];
            b.Name = name;
            b.Description = description;
            return b;
        }

        public Board UpdateBoard(int id, BoardPermission permission)
        {
            Board b = _repo_id[id];
            b.permissions[permission.User] = permission;
            return b;
        }

        public bool CanUserWriteOnBoard(int id, string user)
        {
            Board board = _repo_id[id];
            return (user == board.User || (board.permissions.Keys.Contains(user) && board.permissions[user].CanWrite));
        }

        public void GiveBoardPermissionTo(int boardid, string user)
        {
            Board board = _repo_id[boardid];
            if(!_repo_user[user].Contains(board))
                _repo_user[user].Add(board);
        }

        public void RemoveBoardPermissionTo(int boardid, string user)
        {
            Board board = _repo_id[boardid];
            if (_repo_user[user].Contains(board))
                _repo_user[user].Remove(board);
            if (board.permissions.Keys.Contains(user))
                board.permissions.Remove(user);
            
        }

        public bool CanUserViewBoard(int id, string user)
        {
            return (_repo_user[user].Select(board => board.Id).Contains(id));
        }
    }
}
