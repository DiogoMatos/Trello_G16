using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Etapa2.Repository;

namespace Etapa2.Controllers
{
    public class SearchController : Controller
    {
        private readonly BoardMemoryRepository _repoBoard = RepositoryLocator.GetBoardRepo();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string searchTerm)
        {
            var board = _repoBoard.GetBoardsThatUserCanUse(User.Identity.Name).FirstOrDefault(t => t.Name.Equals(searchTerm));
            if(board != null)
            {
                return RedirectToAction("Details", "Boards", new {id = board.Id});
            }
            if(!searchTerm.Equals(""))
                ModelState.AddModelError("","No results found");
            return View();
        }

        public ActionResult ShowBoards(string term)
        {
            var boards = from g in GetBoards()
                          where g.ToLower().Contains(term.ToLower())
                          select new
                          {
                              label = g
                          };

            return Json(boards, JsonRequestBehavior.AllowGet);
        }

        public List<string> GetBoards()
        {
            return _repoBoard.GetBoardsThatUserCanUse(User.Identity.Name).Select(t => t.Name).ToList();
        }
    }
}
