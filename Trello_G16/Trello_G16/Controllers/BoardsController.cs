using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Etapa2.Controllers.Common.Attributes;
using Etapa2.Models;
using Etapa2.Repository;
using Etapa2.Views.Entitys;

namespace Etapa2.Controllers
{
    [Authorization]
    public class BoardsController : Controller
    {
        private BoardMemoryRepository _repo_board = RepositoryLocator.GetBoardRepo();
        private BoardListMemoryRepository _repo_list = RepositoryLocator.GetBoardListRepo();
        private CardMemoryRepository _repo_cards = RepositoryLocator.GetCardRepo();
        private UserMemoryRepository _repo_users = RepositoryLocator.GetUserRepo();
        private int BoardsPerLine = 5;
        
        public ActionResult Index(string errormessage = "")
        {
            string user = User.Identity.Name;
            if (user == null)
                throw new ArgumentNullException("User");

            List<Board> allboards = _repo_board.GetBoardsThatUserCanUse(user).ToList();
            List<IEnumerable<Board>> boardsdivided = new List<IEnumerable<Board>>();
            int count;
            for (int i = 0; i < allboards.Count; i += BoardsPerLine)
            {
                count = (i + BoardsPerLine < allboards.Count) ? BoardsPerLine : allboards.Count - i;
                boardsdivided.Add(allboards.GetRange(i, count));
            }

            ViewBag.error = errormessage;
            ViewBag.BoardsPerLine = BoardsPerLine;
            return View("BoardsView", boardsdivided);
        }

        private int ListsPerLine = 5;
        public ActionResult Details(int id)
        {
            string user = User.Identity.Name;
            BoardDetails boardDetails = new BoardDetails();
            boardDetails.board = _repo_board.GetById(id);

            if (boardDetails.board == null)
            {
                return new HttpNotFoundResult("Board with id=" + id);
            }

            if (user == null || !_repo_board.CanUserViewBoard(id, user))
                return new HttpUnauthorizedResult();

            List<BoardList> alllists = _repo_list.GetListsById(id).OrderBy(list => list.BoardPosition).ToList();
            List<IEnumerable<BoardList>> listsdivided = new List<IEnumerable<BoardList>>();
            int count;
            for (int i = 0; i < alllists.Count; i += ListsPerLine)
            {
                count = (i + ListsPerLine < alllists.Count) ? ListsPerLine : alllists.Count - i;
                listsdivided.Add(alllists.GetRange(i, count));
            }

            boardDetails.canUserWrite = boardDetails.board.CanEdit(user);
            boardDetails.listsPerLine = this.ListsPerLine;
            boardDetails.listsDivided = listsdivided;
            boardDetails.numberOfLists = alllists.Count;

            return View("BoardDetail", boardDetails);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string error = "";
                string user = User.Identity.Name;
                if(user == null)
                    return new HttpUnauthorizedResult();
                string name = collection["name"];
                if (!string.IsNullOrEmpty(name))
                {
                    string description = collection["desc"];
                    if (string.IsNullOrEmpty(description))
                        description = "( no description )";
                    _repo_board.Add(new Board { Description = description, User = user, Name = name, CreateDate = DateTime.Now });
                }
                else
                {
                    error = "Name field can't be blank!";
                }
                return RedirectToAction("Index", new { errormessage=error });
            }
            catch
            {
                return View("Error");
            }
        }
        
        //
        // GET: /Boards/Edit/5

        public ActionResult Edit(int id, string errormessage = "")
        {
            string user = User.Identity.Name;

            if (user == null)
                throw new ArgumentNullException("User");

            Board board = _repo_board.GetById(id);
            if (board == null)
            {
                return new HttpNotFoundResult("Board with id=" + id);
            }
            if (!_repo_board.CanUserWriteOnBoard(id, user))
                return new HttpUnauthorizedResult();

            ViewBag.editerror = errormessage;
            return View("BoardEdit", board);
        }

        //
        // POST: /Boards/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                string user = User.Identity.Name;
                Board board = _repo_board.GetById(id);

                if (!board.CanEdit(user))
                    return new HttpUnauthorizedResult();

                string name = collection["name"];
                string description = collection["desc"];
                if (string.IsNullOrEmpty(name))
                    name = board.Name;
                else if (name != board.Name && _repo_board.BoardNameExists(name, user))
                {
                    RedirectToAction("BoardEdit", new { errormessage = "Board name " + name + " already existes." });
                }

                _repo_board.UpdateBoard(id, name, description);
                return RedirectToAction("Index");
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult PermissionRemove(int boardid, string name, FormCollection collection)
        {
            try
            {
                string user = User.Identity.Name;
                Board board = _repo_board.GetById(boardid);

                if (!board.CanEdit(user))
                    return new HttpUnauthorizedResult();

                _repo_board.RemoveBoardPermissionTo(boardid, name);
                return RedirectToAction("Edit", "Boards", new { id = boardid });
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult PermissionChange(int boardid, string userperm, bool set, FormCollection collection)
        {
            Board board = _repo_board.GetById(boardid);
            string user = User.Identity.Name;

            if (!board.CanEdit(user))
                return new HttpUnauthorizedResult();

            if (board.permissions.Keys.Contains(userperm))
            {
                board.permissions[userperm].CanWrite = set;
            }
            return RedirectToAction("Edit", "Boards", new { id = boardid });
        }

        [HttpPost]
        public ActionResult PermissionAdd(int boardid, FormCollection collection)
        {
            string errormessage = "";
            Board board = _repo_board.GetById(boardid);
            string user = User.Identity.Name;

            if (!board.CanEdit(user))
                return new HttpUnauthorizedResult();

            string name = collection["permissionname"];
            string type = collection["permissiontype"];

            if (string.IsNullOrEmpty(type))
                errormessage = "Insert a type of permission";

            if (string.IsNullOrEmpty(name))
                errormessage += (errormessage==""?"I":" and i") + "nsert a name";

            else if (user==name || !_repo_users.DoesUserExist(name))
                errormessage = "User does not exists";

            if (errormessage == "")
            {
                BoardPermission permission = new BoardPermission { User = name, CanWrite = type == "rw" };
                board = _repo_board.UpdateBoard(boardid, permission);
                _repo_board.GiveBoardPermissionTo(boardid, name);
                ViewBag.permissionerror = "";
                return RedirectToAction("Edit", "Boards", new { id=boardid });
            }
            else
            {
                ViewBag.permissionerror = errormessage;
                return View("BoardEdit", board);
            }
        }

        //
        // GET: /Boards/Delete/5
 
        public ActionResult Delete(int id)
        {
            var list = _repo_list.GetById(id);
            if(!_repo_cards.GetCardsByListId(id).Any())
            {
                _repo_list.Remove(list);
            }
            return RedirectToAction("Details", new { id = list.BoardId });
        }

        [HttpPost]
        public JsonResult GetListsFromBoard(int id)
        {
            var board = _repo_board.GetById(id);
            var json = new List<string>();

            foreach (var item in _repo_list.GetListsById(board))
            {
                json.Add(item.Id+"");
                json.Add(item.Name);
            }
            return Json(new {list = json, len = json.Count}, JsonRequestBehavior.AllowGet);
        }
    }
}
