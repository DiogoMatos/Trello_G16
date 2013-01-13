using Etapa2.Models;

namespace Etapa2.Authentication
{
    public interface IAuthProvider
    {
        User Authenticate(string nick, string password);
    }
}