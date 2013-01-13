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
    public class ListsController : Controller
    {
        private BoardListMemoryRepository _repo_list = RepositoryLocator.GetBoardListRepo();
        private CardMemoryRepository _repo_card = RepositoryLocator.GetCardRepo();
        private BoardMemoryRepository _repo_board = RepositoryLocator.GetBoardRepo();

        //
        // GET: /Lists/Details/5

        private int CardsPerLine = 7;
        public ActionResult Details(int id)
        {
            string user = User.Identity.Name;

            if (user == null)
                throw new ArgumentNullException("User");

            ListDetails listDetails = new ListDetails();
            BoardList list = listDetails.list = _repo_list.GetById(id);

            if (list == null)
            {
                return new HttpNotFoundResult("Board with id=" + id);
            }

            if (!_repo_board.CanUserViewBoard(list.BoardId, user))
                return new HttpUnauthorizedResult();

            List<Card> allcards = _repo_card.GetCardsByListId(id).OrderBy(card => card.BoardListPosition).ToList();
            List<IEnumerable<Card>> listsdivided = new List<IEnumerable<Card>>();
            int count;
            for (int i = 0; i < allcards.Count; i += CardsPerLine)
            {
                count = (i + CardsPerLine < allcards.Count) ? CardsPerLine : allcards.Count - i;
                listsdivided.Add(allcards.GetRange(i, count));
            }

            listDetails.cardsPerLine = this.CardsPerLine;
            listDetails.cardsDivided = listsdivided;
            listDetails.numberOfCards = allcards.Count;
            listDetails.canUserWrite = _repo_board.CanUserWriteOnBoard(list.BoardId, user);

            return View("ListDetail", listDetails);
        }


        //
        // POST: /Lists/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string user = User.Identity.Name;
                if (user == null)
                    return new HttpUnauthorizedResult();
                string name = collection["name"];
                int position = int.Parse(collection["position"]);
                int boardId = int.Parse(collection["boardId"]);
                BoardList newbl = new BoardList();
                newbl.CreateDate = DateTime.Now;
                newbl.Name = name;
                newbl.BoardPosition = position;
                newbl.BoardId = boardId;

                int lid = _repo_list.Add(newbl);
                return RedirectToAction("Details", new {id = lid});
            }
            catch
            {
                return View("Error");
            }
        }
        
        //
        // GET: /Lists/Edit/5
 
        public ActionResult Edit(int id)
        {
            string user = User.Identity.Name;
            BoardList list = _repo_list.GetById(id);
            if (list == null)
                return new HttpNotFoundResult("List with ID=" + id + " doen't exist!");

            if (user == null || !_repo_board.CanUserWriteOnBoard(list.BoardId, user))
                return new HttpUnauthorizedResult();

            int countcards = _repo_card.GetCardsByListId(id).Count();
            string owner = _repo_board.GetById(list.BoardId).User;
            int listcount = _repo_list.GetListsById(list.BoardId).Count();
            ListEdit listedit = new ListEdit { list = list, user = owner, numberofcardslistcontains = countcards, numberoflistsinboard = listcount };

            return View("ListEdit", listedit);
        }

        //
        // POST: /Lists/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                string user = User.Identity.Name;
                BoardList list = _repo_list.GetById(id);
                if (list == null)
                    return new HttpNotFoundResult("List with ID=" + id + " doen't exist!");

                if (user == null || !_repo_board.CanUserWriteOnBoard(list.BoardId, user))
                    return new HttpUnauthorizedResult();
                
                string nameerror="", newname = collection["name"];
                int pos;
                if(collection["pos"] == null || !int.TryParse(collection["pos"], out pos)){
                    pos = list.BoardPosition;
                }
                 
                if(string.IsNullOrEmpty(newname)){
                    nameerror = "Insert a name...";
                }
                else if (newname == list.Name)
                {
                    nameerror = "Name is the same...";
                }
                else if (_repo_list.UserHasListNameInBoard(list, newname))
                {
                    nameerror = "This board already has a list with the name '" + newname + "'.";
                }

                if (nameerror == "")
                {
                    _repo_list.UpdateList(id, newname);
                    ViewBag.messagename = "Name changed!";
                    ViewBag.errorname = "";
                }
                else
                {
                    ViewBag.messagename = "";
                    ViewBag.errorname = nameerror;
                }
                if (pos != list.BoardPosition)
                {
                    _repo_list.UpdateList(id, pos);
                    ViewBag.messagepos = "Position changed!";
                }
                else
                    ViewBag.messagepos = "";

                return View("ListEdit", new ListEdit { list = list, user = _repo_board.GetById(list.BoardId).User,
                                                       numberofcardslistcontains = _repo_card.GetCardsByListId(id).Count(),
                                                       numberoflistsinboard = _repo_list.GetListsById(list.BoardId).Count()
                });
            }
            catch
            {
                return View("Error");
            }
        }

        //
        // POST: /Lists/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                string user = User.Identity.Name;
                BoardList list = _repo_list.GetById(id);
                if (list == null)
                    return new HttpNotFoundResult("List with ID=" + id + " doen't exist!");

                if (user == null || !_repo_board.CanUserWriteOnBoard(list.BoardId, user))
                    return new HttpUnauthorizedResult();

                _repo_list.Remove(list);

                return RedirectToAction("Details", "Boards", new { id = list.BoardId });
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        public JsonResult ChangeCardsOrder(int[] list, int id)
        {
            try
            {
                var i = 0;
                foreach (var item in list.Where(item => item != 0))
                {
                    if (_repo_card != null && _repo_card.GetById(item).BoardListPosition != (i + 1))
                        _repo_card.GetById(item).BoardListPosition = i + 1;
                    i++;
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MoveCard(int listId, int cardId)
        {
            var card =_repo_card.GetById(cardId);
            card.BoardListId = listId;
            var pos = _repo_card.GetCardsByListId(listId).Count();
            _repo_card.SetPositionOfCard(ref card, pos);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
