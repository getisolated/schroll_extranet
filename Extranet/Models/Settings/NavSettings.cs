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

namespace Extranet.Models.Settings
{
    public class NavSettings
    {
        public string? ECSystemService { get; set; }
        public string? ECManagementWS { get; set; }
        public string? User { get; set; }
        public string? Pwd { get; set; }
        public string? Domain { get; set; }
        public string? ErrorLogPath { get; set; }
        public string? NavDateFormat { get; set; }
        public string? NewsLogin { get; set; }
        public string? NewsPwd { get; set; }
        public string? FilePath { get; set; }
        public string? SharedFilePath { get; set; }
    }
}
