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

namespace Extranet.Models.Helpers
{
    public static class DateHelper
    {
        public static void VerifyInputDate(ref string? dateFrom, ref string? dateTo, string format)
        {
            if (!DateTime.TryParseExact(dateFrom, format, null, System.Globalization.DateTimeStyles.None, out DateTime dtFrom))
                dateFrom = DateTime.Now.AddYears(-1).ToString(format);

            if (!DateTime.TryParseExact(dateTo, format, null, System.Globalization.DateTimeStyles.None, out DateTime dtTo))
                dateTo = DateTime.Now.ToString(format);

            if (dateFrom == null || dateTo == null || (dtFrom > dtTo))
            {
                dateFrom = null;
                dateTo = null;
            }
        }

        public static void VerifyInputDateChrono(ref string? dateFrom, ref string? dateTo, string format)
        {
            if (!DateTime.TryParseExact(dateFrom, format, null, System.Globalization.DateTimeStyles.None, out DateTime dtFrom))
                dateFrom = DateTime.Parse("01/01/" + DateTime.Now.Year).ToString(format);

            if (!DateTime.TryParseExact(dateTo, format, null, System.Globalization.DateTimeStyles.None, out DateTime dtTo))
                dateTo = DateTime.Now.ToString(format);

            if (dateFrom == null || dateTo == null || (dtFrom > dtTo))
            {
                dateFrom = null;
                dateTo = null;
            }
        }

        public static string GetFirstDayOfMonth(DateTime date, string format)
        {
            return _GetFirstDayOfMonth(date).ToString(format);
        }

        private static DateTime _GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static string GetLastDayOfMonth(DateTime date, string format)
        {
            return _GetFirstDayOfMonth(date).AddMonths(1).AddDays(-1).ToString(format);
        }

        public static void GetFirstAndLastDayOfMonth(DateTime date, string format, out string fromDate, out string toDate)
        {
            fromDate = GetFirstDayOfMonth(date, format);
            toDate = GetLastDayOfMonth(date, format);
        }

        public static void GetCompleteYear(DateTime date, string format, out string fromDate, out string toDate)
        {
            fromDate = new DateTime(date.Year, 1, 1).ToString(format);
            toDate = new DateTime(date.Year, 12, 31).ToString(format);
        }

        public static void CheckYear(int? year, string format, out string fromDate, out string toDate)
        {
            //Par défaut on prend l'année en cours sinon au max l'année N-2 (2ans glissant)
            year = year.HasValue && (year.Value >= DateTime.Now.Year - 2) ? year : DateTime.Now.Year;
            GetCompleteYear(year.HasValue ? new DateTime(year.Value, 1, 1) : DateTime.Now, format,
                out fromDate, out toDate);
        }
    }
}
