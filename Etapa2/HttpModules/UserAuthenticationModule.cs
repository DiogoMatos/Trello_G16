using Etapa2.Repository;
using System;
using System.Security.Principal;
using System.Web;

namespace Etapa2.HttpModules
{
    public class UserAuthenticationModule : IHttpModule
    {
        private HttpApplication _httpApplication;
        private UserMemoryRepository _repoUsers;

        public void Init(HttpApplication httpApplication)
        {
            _httpApplication = httpApplication;
            _httpApplication.AuthenticateRequest += AuthenticateUser;
            _repoUsers = RepositoryLocator.GetUserRepo();
        }

        public virtual void OnStart(HttpApplication context)
        {
            _httpApplication.AuthenticateRequest += AuthenticateUser;
        }

        public virtual void OnInit(HttpApplication context)
        {
            return;
        }

        void AuthenticateUser(object sender, EventArgs a)
        {
            var application = (HttpApplication) sender;
            string[] roles = null;
            string nick = "";
            try
            {
                string cookie = _httpApplication.Request.Cookies["Authentication"].Value;
                nick = (cookie != null) ? cookie : "";
                if (nick != "")
                {
                    //Localizar user no repo
                    var usr = _repoUsers.GetById(nick);

                    switch (usr.Type)
                    {
                        case "user":
                            roles = new string[] { "user" };
                            break;
                        case "admin":
                            roles = new string[] { "admin", "user" };
                            break;
                        default:
                            roles = new string[0];
                            break;
                    }
                }
            }catch(Exception)
            {
                roles = new string[0];
            }
            application.Context.User = new GenericPrincipal(new GenericIdentity(nick), roles);
        }

        public void Dispose(){ }
    }
}


   