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

using Realms;

namespace Extranet.Models.Settings
{
    public class SchrollRealmConfig
    {
        private readonly static ulong CurrentSchemaVersion = 3;

        //on doit créer une nouvelle instance, on ne peut pas éxecuter une instance d'un autre thread
        public static Realm GetNewRealmInstance(WebSettings webshopSettings)
        {
            string basePath = webshopSettings.RealmSettings.Path;
            string path = basePath + "/EspaceClientSchroll.realm";
            RealmConfiguration realmConfiguration = new(path)
            {
                SchemaVersion = CurrentSchemaVersion,
                MigrationCallback = (migration, oldSchemaVersion) =>
                {

                }
            };
            return Realm.GetInstance(realmConfiguration);
        }

    }
}
