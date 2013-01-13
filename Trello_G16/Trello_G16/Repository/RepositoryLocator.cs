using Etapa2.Models;

namespace Etapa2.Repository
{
    class RepositoryLocator
    {
        private readonly static IRepository<BoardList, int> BoardListRepo = new BoardListMemoryRepository();
        private readonly static IRepository<Board, int> BoardRepo = new BoardMemoryRepository();
        private readonly static IRepository<Card, int> CardRepo = new CardMemoryRepository();
        private readonly static IRepository<User, string> UserRepo = new UserMemoryRepository();
        private static readonly IRepository<User, int> CifraRepo = new CifraMemoryRepository(); 

        public static BoardListMemoryRepository GetBoardListRepo()
        {
            return (BoardListMemoryRepository)BoardListRepo;
        }

        public static BoardMemoryRepository GetBoardRepo()
        {
            return (BoardMemoryRepository)BoardRepo;
        }

        public static CardMemoryRepository GetCardRepo()
        {
            return (CardMemoryRepository)CardRepo;
        }

        public static UserMemoryRepository GetUserRepo()
        {
            return (UserMemoryRepository)UserRepo;
        }

        public static CifraMemoryRepository GetCifraRepo()
        {
            return (CifraMemoryRepository)CifraRepo;
        }
    }
}
