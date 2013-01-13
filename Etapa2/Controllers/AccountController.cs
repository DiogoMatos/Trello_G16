using System.Globalization;
using System.Security.Policy;
using Etapa2.Authentication;
using Etapa2.Models;
using Etapa2.Repository;
using System;
using System.Web.Mvc;


namespace Etapa2.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthProvider _authProvider;
        private readonly UserMemoryRepository _repoUsers;
        private readonly CifraMemoryRepository _repoCifra;

        public AccountController()
        {
            _authProvider = new FormsAuthProvider();
            _repoUsers = RepositoryLocator.GetUserRepo();
            _repoCifra = RepositoryLocator.GetCifraRepo();
        }

        // GET: /Account/LogOn
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(User model)
        {
            //Tentar localizar user no repo
            var user = _repoUsers.GetById(model.Nickname);

            ModelState.Remove("Name");
            ModelState.Remove("Email");

            if (ModelState.IsValid && user != null)
            {
                var u = _authProvider.Authenticate(user.Nickname, model.Password);
                if (u != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Password invalid!");
                return View();
            }
            ModelState.AddModelError("", "Nickname invalid!");
            return View();
        }

        public ActionResult LogOff()
        {
            if (ControllerContext.HttpContext.Request.Cookies["Authentication"] != null)
            {
                var cookie = ControllerContext.HttpContext.Request.Cookies["Authentication"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                //Adicionar cifra e enviar email
                model.Type = "user";
                int rand = _repoCifra.Add(model);
                //Criar chave para desafio do email
                string key = rand.ToString(CultureInfo.InvariantCulture);

                //Enconding....
                var str = Encryptor.Encrypt(key);

                //Enviar Email
                var emailSender = new EmailSender();
                emailSender.ProcessEmail(model.Email, new Url("http://localhost:50027/Account/Validate/" + str));
                return RedirectToAction("LogOn");
            }
            return View();
        }

        public ActionResult Validate(string id)
        {
            //Decoding....
            var str = Encryptor.Decrypt(id);
            var rand = Int32.Parse(str);

            var model = _repoCifra.GetById(rand);
            if (model != null)
            {
                _repoUsers.Add(model);
                _repoCifra.Remove(rand);
                return RedirectToAction("LogOn");
            }
            return View("Error");
        }
    }
}
