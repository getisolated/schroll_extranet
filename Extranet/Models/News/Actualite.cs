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

using Extranet.Models.Settings;
using Realms;

namespace Extranet.Models.News
{
    public class Actualite : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString("d");
        public string Description { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public string LinkTitle { get; set; }

        public DateTimeOffset Date { get; set; }

        public static List<Actu> GetActualites(WebSettings webSettings)
        {
            var realm = SchrollRealmConfig.GetNewRealmInstance(webSettings);
            var actualites = realm.All<Actualite>()?.OrderByDescending(x => x.Date)?.ToList();

            List<Actu> actualitesList = new List<Actu>();
            if (actualites != null)
                foreach (Actualite realmactu in actualites)
                {
                    actualitesList.Add(new Actu(realmactu));
                }

            return actualitesList;
        }
    }

    public class Actu
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public string LinkTitle { get; set; }

        public DateTimeOffset Date { get; set; }

        public Actu() { }
        public Actu(Actualite actualite)
        {
            this.Link = actualite.Link;
            this.LinkTitle = actualite.LinkTitle;
            this.Date = actualite.Date;
            this.Description = actualite.Description;
            this.Image = actualite.Image;
            this.Id = actualite.Id;
        }
    }

    public class AjaxActualite
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string LinkTitle { get; set; }
        public string myfile { get; set; }
        public string ext { get; set; }
    }
}
