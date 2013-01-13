using Etapa2.Models;
using Etapa2.Repository;
using System;
using System.Linq;
using System.Web;

namespace Etapa2.Authentication
{
    public class FormsAuthProvider : IAuthProvider
    {
        private readonly UserMemoryRepository _repoUsers = RepositoryLocator.GetUserRepo();

        public User Authenticate(string nick, string password)
        {
            var user = _repoUsers.GetById(nick);
            if (user != null && user.VerifyUserCredentials(nick,password))
            {
                var httpCookie = new HttpCookie("Authentication");

                httpCookie.Value = nick;
                httpCookie.Expires = DateTime.Now.AddDays(1);
                httpCookie.Path = "/";

                HttpContext.Current.Response.Cookies.Add(httpCookie);
                return user;
            }
            return null;
        }
    }
}