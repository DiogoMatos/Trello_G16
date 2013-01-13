using System;
using System.Collections.Generic;
using System.Linq;
using Etapa2.Models;

namespace Etapa2.Repository
{
    class BoardListMemoryRepository : IRepository<BoardList, int>
    {
        private readonly IDictionary<int, BoardList> _repo = new Dictionary<int, BoardList>();
        private int _cid = 1;

        public BoardListMemoryRepository()
        {
            this.Add(new BoardList { Name = "A Realizar", BoardPosition = 1, BoardId = 1, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Em Realização", BoardPosition = 2, BoardId = 1, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Pendentes", BoardPosition = 3, BoardId = 1, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Completadas", BoardPosition = 4, BoardId = 1, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "A Realizar", BoardPosition = 1, BoardId = 5, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Em Realização", BoardPosition = 2, BoardId = 5, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Pendentes", BoardPosition = 3, BoardId = 5, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Completadas", BoardPosition = 4, BoardId = 5, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "A Realizar", BoardPosition = 1, BoardId = 7, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Em Realização", BoardPosition = 2, BoardId = 7, CreateDate = DateTime.Now });
            this.Add(new BoardList { Name = "Completadas", BoardPosition = 3, BoardId = 7, CreateDate = DateTime.Now });
        }

        public IEnumerable<BoardList> GetAll()
        {
            return _repo.Values;
        }

        public BoardList GetById(int id)
        {
            BoardList b;
            _repo.TryGetValue(id, out b);
            return b;
        }

        public int Add(BoardList elem)
        {
            elem.Id = _cid;

            int newPos = elem.BoardPosition;
            elem.BoardPosition = 0;
            SetPositionOfList(ref elem, newPos);

            _repo.Add(_cid++,elem);
            return elem.Id;
        }

        public IEnumerable<BoardList> GetListsById(Board board)
        {
            return GetListsById(board.Id);
        }
        
        public IEnumerable<BoardList> GetListsById(int bid)
        {
            return _repo.Values.Select(l => l).Where(l => l.BoardId == bid);
        }

        public IEnumerable<int> GetIntEnumFromBoardId(Board board)
        {
            int count = GetListsById(board).Count();
            var list = new List<int>();
            for (var i = 1; i <= count + 1; i++)
                list.Add(i);
            return list;
        }

        private void SetPositionOfList(ref BoardList list, int newPos)
        {
            IEnumerable<BoardList> lists = GetListsById(list.BoardId);
            if (list.BoardPosition <= 0)
                list.BoardPosition = lists.Count() + 1;

            int len = 0;
            if (newPos > list.BoardPosition)
            {
                foreach (BoardList l in lists)
                {
                    if (l.BoardPosition > list.BoardPosition && l.BoardPosition <= newPos)
                        l.BoardPosition -= 1;
                    ++len;
                }
            }
            else
            {
                foreach (BoardList l in lists)
                {
                    if (l.BoardPosition >= newPos && l.BoardPosition < list.BoardPosition)
                        l.BoardPosition += 1;
                    ++len;
                }
            }

            if (newPos > len)
            {
                newPos = len + 1;
            }
            list.BoardPosition = newPos;
        }

        public bool IsListFromUser(int id, string user)
        {
            return RepositoryLocator.GetBoardRepo().GetById(GetById(id).BoardId).User == user;
        }

        public void Remove(BoardList list)
        {
            SetPositionOfList(ref list, int.MaxValue);
            _repo.Remove(list.Id);
        }

        public bool UserHasListNameInBoard(BoardList list, string newname)
        {
            IEnumerable<BoardList> lists = GetListsById(list.BoardId);
            return lists.Select(l => l.Name).Contains(newname);
        }

        public void UpdateList(int listId, int position, string name)
        {
            BoardList list = _repo[listId];
            SetPositionOfList(ref list, position);
            list.Name = name;
        }

        public void UpdateList(int listId, string name)
        {
            UpdateList(listId, _repo[listId].BoardPosition, name);
        }

        public void UpdateList(int listId, int position)
        {
            UpdateList(listId, position, _repo[listId].Name);
        }
    }
}
