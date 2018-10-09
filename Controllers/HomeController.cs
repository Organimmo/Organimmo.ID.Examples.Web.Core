// --------------------------------------------------
// Cleanup date: 09/10/2018 10:53
// Cleanup user: Michael Roef
// --------------------------------------------------

#region NAMESPACES

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace Organimmo.ID.Examples.Web.Core.Controllers
{
    [Authorize]
    [Route("/")]
    public class HomeController : Controller
    {
        #region METHODS

        #region PUBLIC

        public async Task<IActionResult> Index()
        {
            // The access token for the current authenticated user.
            // This token enables you to communicate with the audience API that you defined in Startup.cs
            // Don't share this token with the end user and don't store it in your database!
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            // This is a unique and readonly ID, this ID will always be the same for the current authenticated user.
            // Use this ID to bind records to the current user.
            ViewBag.UserID = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // Current emailaddress
            ViewBag.Email = User.FindFirst("name")?.Value;
            // Current picture (Gravatar)
            ViewBag.Picture = User.FindFirst("picture")?.Value;

            return View();
        }

        #endregion

        #endregion
    }
}