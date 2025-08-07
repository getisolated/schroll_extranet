// <copyrights>
// Ce programme est la propriété de la société Cap Vision (capvision.fr).
// Tous droits réservés.
// Ce programme est protégé par les lois sur les droits d''auteur en vigueur en France
// et dans d''autres pays. Toute reproduction, modification, distribution ou utilisation
// sans autorisation préalable est strictement interdite.
//
// This program is the property of Cap Vision company (capvision.fr).
// All rights reserved.
// This program is protected by copyright laws in force in France
// and other countries. Any reproduction, modification, distribution or use
// without prior authorization is strictly prohibited.
// </copyrights>

using Extranet.Extensions;
using Extranet.Models;
using Extranet.Models.Guards;
using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Web;

namespace Extranet.Controllers
{
    public class AuthController(ILogger<HomeController> logger, IOptions<WebSettings> values) : Controller
    {
        internal static readonly string securityscheme = "2803D1F7-3109-4480-96FD-2C257EF6D91F";

        private readonly ILogger<HomeController> _logger = logger;
        private readonly WebSettings _values = values.Value;

        [GuestGuard, AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            DataProvider dataProvider = new DataProvider(_values, null);

            //Retour fonction : SavePassword
            ViewBag.NavResponse = TempData.Get<NavResponse<string>>("NavResponseMessage");

            return View();
        }

        [HttpPost, GuestGuard, AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel login)
        {
            DataProvider dataProvider = new DataProvider(_values, null);

            if (dataProvider.CheckAdminLogin(login.email, login.password))
            {
                await Member.SetCurrentUser(HttpContext, new Member(true) { Code = login.email, Name = "Administrateur actus" });
                return RedirectToAction("News", "Admin");
            }

            var res = await dataProvider.UserAuthentication(login.email, login.password, login.society);

            if ((res?.Success ?? false) && (res?.Data?.pAccountExtranetInfos?.Interlocutor?.Accounts?.Account?.Any() ?? false))
            {
                var user = Member.GetMemberFromLogin(res.Data);

                var defaultCompany = user.GetAccounts()?.First()?.Companies?.Company?.FirstOrDefault(c => c.CompanyName == HttpUtility.UrlDecode(DataProvider._defaultCompany))?.CompanyName;
                if (defaultCompany != null)
                {
                    user.CurrentCompany = defaultCompany;
                }
                else
                {
                    user.CurrentCompany = user.GetAccounts()?.First()?.Companies?.Company?.First()?.CompanyName ?? DataProvider._defaultCompany;
                }
                await Member.SetCurrentUser(HttpContext, user);

                return RedirectToAction("Accueil", "Home");
            }
            else
            {
                bool success = false;
                string error = res?.Message;
                if (string.IsNullOrWhiteSpace(error))
                {
                    error = "Aucun compte associé n'a été trouvé avec ces identifiants. Veuillez contacter nos services pour plus d'informations.";
                }

                ViewBag.NavResponse = new NavResponse<string>() { Success = success, Message = error ?? "Erreur" };
                return View();
            }
        }

        [HttpPost]
        public async Task<NavResponse<ECManagementWS.ResetPassword_Result>> SubmitForgottenPwdRequest([FromBody] ForgottenPwdRequest forgottenPwdRequest)
        {
            DataProvider dataProvider = new DataProvider(_values, null);

            var response = await dataProvider.ResetPassword(forgottenPwdRequest.email);

            response.Message = (response?.Success ?? false) ? "Un email de réinitialisation de mot de passe vous a été envoyé." : "Une erreur s'est produite, veuillez réessayer plus tard.";

            return response;
        }

        [GuestGuard, AllowAnonymous]
        public async Task<IActionResult> ForgottenPassword()
        {
            return View();
        }

        [HttpPost, GuestGuard, AllowAnonymous]
        public async Task<IActionResult> SavePassword(string token, LoginModel login)
        {
            DataProvider dataProvider = new DataProvider(_values, null);

            var response = await dataProvider.SavePassword(login.email, token, login.password);

            TempData.Put("NavResponseMessage", new NavResponse<string>()
            {
                Message = (response?.Success ?? false) ? "Votre mot de passe a été réinitialisé avec succès.<br>Vous pouvez vous connecter." : (response?.Message ?? "Erreur"),
                Success = response?.Success ?? false
            });

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await DeleteCurrentUser();
            return RedirectToAction("Login", "Auth");
        }

        #region User auth storage

        private async Task SetCurrentUser(Member user)
        {
            string myuserstr = DESHelper.EncryptToBase64String(JsonConvert.SerializeObject(user));

            // create claims
            List<Claim> claims =
            [
                new(Member.meInTheVoid, myuserstr)
            ];

            // create identity
            ClaimsIdentity identity = new(claims, "schrollcookie");

            // create principal
            ClaimsPrincipal principal = new(identity);

            AuthenticationProperties props = new()
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            // sign-in
            await HttpContext.SignInAsync(securityscheme, principal, props);
        }

        private async Task DeleteCurrentUser()
        {
            await HttpContext.SignOutAsync(securityscheme);
        }

        #endregion
    }
}
