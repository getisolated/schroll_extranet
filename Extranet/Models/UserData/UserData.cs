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
using Extranet.Models.Settings;
using Extranet.Services;
using Newtonsoft.Json;
using Realms;

namespace Extranet.Models.UserData
{
    public class UserData : Realms.RealmObject
    {
        /// <summary>
        /// Correspond à l'identifiant utilisateur
        /// </summary>
        [PrimaryKey]
        public string id { get; set; }

        /// <summary>
        /// Encrypted data List<Account>
        /// </summary>
        public string data { get; set; }

        public static List<Account> GetAccounts(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;


            var realmPath = WebSettingsService.GetRealmPath();
            var realm = SchrollRealmConfig.GetNewRealmInstance(new WebSettings() { RealmSettings = new RealmSettings() { Path = realmPath } });
            var ud = realm.Find<UserData>(id);
            if(ud ==  null || ud?.data == null)
                return null;

            var accounts = JsonConvert.DeserializeObject<List<Account>>(ud?.data);

            return accounts;
        }

        public static void SetUserData(Interlocutor interlocutor)
        {
            if (interlocutor == null)
                return;

            var realmPath = WebSettingsService.GetRealmPath();
            var realm = SchrollRealmConfig.GetNewRealmInstance(new WebSettings() { RealmSettings = new RealmSettings() { Path = realmPath } });
            realm.Write(() =>
            {
                var ud = new UserData()
                {
                    id = interlocutor.Email,
                    data = JsonConvert.SerializeObject(interlocutor.Accounts?.Account)
                };
                realm.Add(ud, true);
            });
        }
    }
}
