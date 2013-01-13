using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Etapa2.Models;
using Etapa2.Repository;
using Etapa2.Views.Entitys;

namespace Etapa2.Controllers
{
    public class CardsController : Controller
    {
        private CardMemoryRepository _repo_card = RepositoryLocator.GetCardRepo();
        private BoardMemoryRepository _repo_board = RepositoryLocator.GetBoardRepo();

        //
        // GET: /Cards/Details/5

        public ActionResult Details(int id)
        {
            string user = User.Identity.Name;

            if (user == null)
                return new HttpUnauthorizedResult();

            Card card = _repo_card.GetById(id);

            if (card == null)
            {
                card = _repo_card.GetArchivedCard(id, user);
                if (card == null)
                {
                    return new HttpNotFoundResult("Board with id=" + id);
                }
            }

            if (!_repo_board.CanUserViewBoard(card.BoardId, user))
                return new HttpUnauthorizedResult();


            return View("CardDetail", card);
        }

        //
        // POST: /Cards/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string user = User.Identity.Name;
                if (user == null)
                    return new HttpUnauthorizedResult();

                int boardId = int.Parse(collection["boardId"]);
                int boardListId = int.Parse(collection["boardListId"]);
                int position = int.Parse(collection["position"]);
                string description = collection["description"];
                if (string.IsNullOrEmpty(description))
                    description = "( no description )";
                string eDate = collection["end_date"];
                string name = collection["name"];
                string [] auxEndDate = eDate.Split('-');
                DateTime endDate = new DateTime(int.Parse(auxEndDate[2]),int.Parse(auxEndDate[1]),int.Parse(auxEndDate[0]));
                int cardId =_repo_card.Add(new Card
                    {
                        BoardId = boardId,
                        BoardListId = boardListId,
                        BoardListPosition = position,
                        DateConclusion = endDate,
                        Description = description,
                        Name = name,
                        CreateDate = DateTime.Now
                    });

                return RedirectToAction("Details", new { id = cardId });
            }
            catch
            {
                return View("Error");
            }
        }


        //
        // POST: /Cards/Archive

        [HttpPost]
        public ActionResult Archive(int cardId, FormCollection collection)
        {
            try
            {
                string user = User.Identity.Name;
                int listId = _repo_card.ArchiveCard(cardId, user).BoardListId;
                return RedirectToAction("Details", "Lists", new { id = listId });
            }
            catch
            {
                return View("Error");
            }
        }


        //
        // GET: /Cards/Archived

        private int ArchivedCardsPerLine= 7;
        public ActionResult Archived()
        {
            string user = User.Identity.Name;
            if (user == null)
                return new HttpUnauthorizedResult();

            List<Card> allcards = _repo_card.GetArchivedCardsByUser(user).ToList();
            List<IEnumerable<Card>> cardsdivided = new List<IEnumerable<Card>>();
            int count;
            for (int i = 0; i < allcards.Count; i += ArchivedCardsPerLine)
            {
                count = (i + ArchivedCardsPerLine < allcards.Count) ? ArchivedCardsPerLine : allcards.Count - i;
                cardsdivided.Add(allcards.GetRange(i, count));
            }

            ViewBag.ArchivedCardsPerLine = ArchivedCardsPerLine;
            return View("ArchivedCards", cardsdivided);
        }

        //
        // GET: /Cards/Edit/5
 
        public ActionResult Edit(int id, string okmessage = "", string errormessage = "")
        {
            string user = User.Identity.Name;
            Card card = _repo_card.GetById(id);
            if (card == null)
                return new HttpNotFoundResult("Card with ID=" + id + " doen't exist!");

            if (user == null || !_repo_board.CanUserWriteOnBoard(card.BoardId, user))
                return new HttpUnauthorizedResult();

            string owner = _repo_board.GetById(card.BoardId).User;
            int cardcount = _repo_card.GetCardsByListId(card.BoardListId).Count();
            CardEdit cardedit = new CardEdit { card = card, user = owner, numberofcardsinlist = cardcount };

            ViewBag.okmessage = okmessage;
            ViewBag.errormessage = errormessage;

            return View("CardEdit", cardedit);
        }

        //
        // POST: /Cards/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            string error = "";
            try
            {
                string user = User.Identity.Name;
                Card card = _repo_card.GetById(id);
                if (card == null)
                    return new HttpNotFoundResult("Card with ID=" + id + " doen't exist!");

                if (user == null || !_repo_board.CanUserWriteOnBoard(card.BoardId, user))
                    return new HttpUnauthorizedResult();

                string name = collection["name"];
                string s_dateEnd = collection["end_date"];

                if (string.IsNullOrEmpty(name))
                    error = "Insert a name";

                DateTime endDate = new DateTime();

                try{
                    string[] auxEndDate = s_dateEnd.Split('-');
                    endDate = new DateTime(int.Parse(auxEndDate[2]), int.Parse(auxEndDate[1]), int.Parse(auxEndDate[0]));
                }
                catch{
                    if (error == "")
                        error = "Insert a valid date";
                    else
                        error += " and a valid date";
                }
                if(error != ""){
                    error+='!';
                    return RedirectToAction("Edit", new { id = id, errormessage = error });
                }

                string description = collection["desc"];
                int position = int.Parse(collection["pos"]);

                _repo_card.UpdateCard(id, name, endDate, description, position);
                return RedirectToAction("Edit", new { id = id, okmessage = "Card updated!" });
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
