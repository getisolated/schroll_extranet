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

namespace Extranet.Models
{
    public class NavResponse<T> where T : class
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

    public class NavResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
