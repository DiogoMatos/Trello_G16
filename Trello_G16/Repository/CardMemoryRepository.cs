using System;
using System.Collections.Generic;
using System.Linq;
using Etapa2.Models;

namespace Etapa2.Repository
{
    class CardMemoryRepository : IRepository<Card, int>
    {
        private readonly IDictionary<int, Card> _repo = new Dictionary<int, Card>();
        private readonly IDictionary<string, List<Card>> _repo_archived = new Dictionary<string, List<Card>>();
        private int _cid = 1;

        public CardMemoryRepository()
        {
            this.Add(new Card { Name = "Login", Description = "Controlo de Login", BoardListPosition = 1, BoardId = 1, BoardListId = 1, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe1", Description = "c2", BoardListPosition = 2, BoardId = 1, BoardListId = 1, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe2", Description = "c3", BoardListPosition = 3, BoardId = 1, BoardListId = 1, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe3", Description = "c4", BoardListPosition = 4, BoardId = 1, BoardListId = 1, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe4", Description = "c5", BoardListPosition = 5, BoardId = 1, BoardListId = 1, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe5", Description = "c6", BoardListPosition = 6, BoardId = 1, BoardListId = 1, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe6", Description = "c7", BoardListPosition = 7, BoardId = 1, BoardListId = 1, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe7", Description = "c8", BoardListPosition = 1, BoardId = 1, BoardListId = 2, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe8", Description = "c9", BoardListPosition = 2, BoardId = 1, BoardListId = 2, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe9", Description = "c10", BoardListPosition = 1, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe10", Description = "c11", BoardListPosition = 2, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe11", Description = "c12", BoardListPosition = 3, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe12", Description = "c13", BoardListPosition = 4, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe13", Description = "c14", BoardListPosition = 5, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe14", Description = "c15", BoardListPosition = 6, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe15", Description = "c16", BoardListPosition = 7, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
            this.Add(new Card { Name = "Exe16", Description = "c17", BoardListPosition = 8, BoardId = 1, BoardListId = 3, DateConclusion = RandomDay(), IsArchived = false, CreateDate = RandomDay() });
        }

        public IEnumerable<Card> GetAll()
        {
            return _repo.Values;
        }

        public Card GetById(int id)
        {
            Card card;
            _repo.TryGetValue(id, out card);
            return card;
        }

        public IEnumerable<Card> GetArchivedCardsByUser(string user)
        {
            if(_repo_archived.ContainsKey(user))
            {
                return _repo_archived[user];
            }
            else
            {
                return new Card [0];
            }
        }
        
        public Card ArchiveCard(int cardId, string user)
        {
            if(_repo.ContainsKey(cardId))
            {
                if(!_repo_archived.ContainsKey(user))
                    _repo_archived.Add(user, new List<Card>());
                Card cardArch = Pop(cardId);
                cardArch.IsArchived = true;
                cardArch.BoardListPosition = _repo_archived[user].Count + 1;
                _repo_archived[user].Add(cardArch);
                return cardArch;
            }
            else
            {
                return null;
            }
        }

        public int Add(Card elem)
        {
            elem.Id = _cid;

            int newPos = elem.BoardListPosition;
            elem.BoardListPosition = 0;
            SetPositionOfCard(ref elem, newPos);

            _repo.Add(_cid++,elem);
            return elem.Id;
        }

        public Card Pop(int cid)
        {
            Card card;
            bool b = _repo.TryGetValue(cid, out card);
            if (b)
            {
                _repo.Remove(cid);
                int pos = card.BoardListPosition;
                foreach (Card c in _repo.Values)
                {
                    if (c.BoardListPosition > pos)
                    {
                        c.BoardListPosition -= 1;
                    }
                }
            }
            return card;
        }

        public IEnumerable<Card> GetCardsByListId(BoardList boardList)
        {
            return GetCardsByListId(boardList.Id);
        }

        public IEnumerable<Card> GetCardsByListId(int bid)
        {
            return _repo.Values.Select(l => l).Where(l => l.BoardListId == bid);
        }

        public IEnumerable<Card> GetArchivedCardsByBoardId(Board board)
        {
            return _repo.Values.Select(l => l).Where(l => l.IsArchived && l.BoardId == board.Id);
        } 

        public IEnumerable<int> GetIntEnumFromListId(BoardList bList)
        {
            int count = GetCardsByListId(bList).Count();
            var list = new List<int>();
            for (var i = 1; i <= count + 1; i++)
                list.Add(i);
            return list;
        }

        public void SetPositionOfCard(ref Card card, int newPos)
        {
            IEnumerable<Card> cards = GetCardsByListId(card.BoardListId);
            if (card.BoardListPosition <= 0)
                card.BoardListPosition = cards.Count() + 1;

            int len = 0;
            if (newPos > card.BoardListPosition)
            {
                foreach (Card c in cards)
                {
                    if (c.BoardListPosition > card.BoardListPosition && c.BoardListPosition <= newPos)
                        c.BoardListPosition -= 1;
                    ++len;
                }
            }
            else
            {
                foreach (Card c in cards)
                {
                    if (c.BoardListPosition >= newPos && c.BoardListPosition < card.BoardListPosition)
                        c.BoardListPosition += 1;
                    ++len;
                }
            }

            if (newPos > len)
            {
                newPos = len + 1;
            }
            card.BoardListPosition = newPos;
        }

        public bool IsCardFromUser(int id, string user)
        {
            Card card = GetById(id);
            if (card == null)
            {
                return true;
            }
            return RepositoryLocator.GetBoardRepo().GetById(card.BoardId).User == user;
        }

        public Card GetArchivedCard(int id, string user)
        {
            return _repo_archived[user].Find(card => card.Id == id);
        }

        public void UpdateCard(int id, string name, DateTime enddate, string description, int position)
        {
            Card card = _repo[id];
            card.Name = name;
            card.DateConclusion = enddate;
            card.Description = description;
            if (position != card.BoardListPosition)
                SetPositionOfCard(ref card, position);
        }

        private DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            Random gen = new Random();

            int range = ((TimeSpan)(DateTime.Today - start)).Days;
            return start.AddDays(gen.Next(range));
        }
    }
}
