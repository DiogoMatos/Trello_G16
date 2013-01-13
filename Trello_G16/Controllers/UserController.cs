using Etapa2.Models;
using Etapa2.Repository;
using System;
using System.Web;
using System.Web.Mvc;

namespace Etapa2.Controllers
{
    public class UserController : Controller
    {
        private UserMemoryRepository _repoUsers = RepositoryLocator.GetUserRepo();

        public ActionResult Index()
        {
            
            var user = _repoUsers.GetById(User.Identity.Name);
            var list = _repoUsers.GetAllUsersExceptAdmins();
            var tuple = Tuple.Create(user,list);
            return View(tuple);
        }

        public ActionResult Edit() 
        {
            var user = _repoUsers.GetById(User.Identity.Name);
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User model, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    model.ImageMimeType = imageFile.ContentType;
                    model.Image = new byte[imageFile.ContentLength];
                    imageFile.InputStream.Read(model.Image, 0, imageFile.ContentLength);
                }
                var user = _repoUsers.GetById(User.Identity.Name);
                _repoUsers.Remove(user);
                _repoUsers.Add(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult RemoveUser()
        {
            var user = _repoUsers.GetById(User.Identity.Name);
            _repoUsers.Remove(user);
            if (ControllerContext.HttpContext.Request.Cookies["Authentication"] != null)
            {
                var cookie = ControllerContext.HttpContext.Request.Cookies["Authentication"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RemoveById(string id)
        {
            var user = _repoUsers.GetById(id);
            _repoUsers.Remove(user);
            return RedirectToAction("Index");
        }

        public ActionResult SetAdmin(string id) 
        {
            var user = _repoUsers.GetById(id);
            user.SetAdmin();
            return RedirectToAction("Index");
        }

        public FileContentResult GetImage(string Nickname)
        {
            var user = _repoUsers.GetById(Nickname);
            return (user != null && user.Image != null) ? new FileContentResult(user.Image, user.ImageMimeType) : null;
        }

        public bool CheckName(string name)
        {
            return _repoUsers.VerifyNameAvailability(name);
        }
    }
}