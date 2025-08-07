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

using ECManagementWS;
using Extranet.Controllers;
using Extranet.Models.Helpers;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Web;

namespace Extranet.Models.Members
{
    public class Member
    {
        public const string meInTheVoid = "ff264f66-aedd-461f-ad18-596f8642b833";

        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Lastname { get; set; } = "";
        public string Title { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsAdmin { get; set; }

        public string CurrentCompany { get; set; } = DataProvider._defaultCompany;
        public Account CurrentAccount { get; set; }

        public List<Account> GetAccounts()
        {
            return UserData.UserData.GetAccounts(Email);
        }

        public Member() { }

        public Member(bool isadmin)
        {
            IsAdmin = isadmin;
        }

        public static Member GetMemberFromLogin(UserAuthentication_Result userAuthentication_Result)
        {
            var interlocutor = userAuthentication_Result?.pAccountExtranetInfos?.Interlocutor;

            if (interlocutor == null)
                return null;

            //insérer les données dans Realm
            UserData.UserData.SetUserData(interlocutor);

            return new()
            {
                //Code = interlocutor.InterlocutorCode,
                Name = interlocutor.Name,
                Lastname = interlocutor.Lastname,
                Title = interlocutor.Title,
                //Accounts = interlocutor.Accounts?.Account?.ToList(),//transféré dans realm
                CurrentCompany = interlocutor.Accounts?.Account?.First()?.Companies?.Company?.First()?.CompanyName ?? DataProvider._defaultCompany,
                CurrentAccount = interlocutor.Accounts?.Account?.First(),
                Email = interlocutor.Email
            };
        }

        public static async Task SetCurrentCompany(HttpContext context, string company)
        {
            var user = context.Items["User"] as Member;
            if (user == null)
                return;

            user.CurrentCompany = company;

            await SetCurrentUser(context, user);
        }

        public static async Task SetCurrentAccount(HttpContext context, string accountNo)
        {
            var user = context.Items["User"] as Member;
            if (user == null)
                return;

            // Get in the user.GetAccounts()?.Account? and get the one that has AccountNo equals to the accountNo in parameter
            var account = user.GetAccounts()?.FirstOrDefault(acc => acc.AccountNo == accountNo);
            user.CurrentAccount = account;
            var defaultCompany = user.CurrentAccount?.Companies?.Company?.FirstOrDefault(c => c.CompanyName == HttpUtility.UrlDecode(DataProvider._defaultCompany))?.CompanyName;
            if (defaultCompany != null)
            {
                user.CurrentCompany = defaultCompany;
            }
            else
            {
                user.CurrentCompany = account.Companies?.Company?.First()?.CompanyName ?? DataProvider._defaultCompany;
            }
            await SetCurrentUser(context, user);
        }

        public static async Task SetCurrentUser(HttpContext context, Member user)
        {
            if (user == null)
                return;

            var userjson = JsonConvert.SerializeObject(user);
            string myuserstr = DESHelper.EncryptToBase64String(userjson);

            // create claims
            List<Claim> claims =
            [
                new(meInTheVoid, myuserstr)
            ];

            // create identity
            ClaimsIdentity identity = new(claims, "schcookie");

            // create principal
            ClaimsPrincipal principal = new(identity);

            AuthenticationProperties props = new()
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            // sign-in
            await context.SignInAsync(AuthController.securityscheme, principal, props);
        }

        public static async Task DeleteCurrentUser(HttpContext context)
        {
            await context.SignOutAsync(AuthController.securityscheme);
        }
    }
}
