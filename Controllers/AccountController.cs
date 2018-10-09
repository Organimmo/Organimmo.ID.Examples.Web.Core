// --------------------------------------------------
// Cleanup date: 09/10/2018 10:31
// Cleanup user: Michael Roef
// --------------------------------------------------

#region NAMESPACES

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace Organimmo.ID.Examples.Web.Core.Controllers
{
    [Authorize]
    [Route("[Controller]")]
    public class AccountController : Controller
    {
        #region METHODS

        #region PUBLIC

        [AllowAnonymous]
        [HttpGet("login")]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("OrganimmoID", new AuthenticationProperties {RedirectUri = returnUrl});
        }

        [HttpGet("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("OrganimmoID", new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion

        #endregion
    }
}