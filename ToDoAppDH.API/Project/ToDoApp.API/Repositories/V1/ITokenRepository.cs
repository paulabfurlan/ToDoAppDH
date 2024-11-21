using Microsoft.AspNetCore.Identity;

namespace ToDoApp.API.Repositories.V1
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
