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
using Extranet.Models.Documents;
using Extranet.Models.Guards;
using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO.Compression;

namespace Extranet.Controllers
{
    [MemberGuard]
    public class RegistreController(ILogger<Controller> logger, IOptions<WebSettings> values) : BaseMemberController(logger, values)
    {
        private const string DOWNLOADREGISTER = "DownloadRegister";
        private const string OUTPUTZIPREGISTER = "OutputZipRegister";

        public async Task<IActionResult> RegistreMatiere(string? from, string? to, string? codeai)
        {
            DateHelper.VerifyInputDate(ref from, ref to, _mySettings.NavSettings.NavDateFormat);
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);

            var fromDate = DateTime.Now.AddYears(-1).ToString(_mySettings.NavSettings.NavDateFormat);
            var toDate = DateTime.Now.ToString(_mySettings.NavSettings.NavDateFormat);

            ViewBag.RegisterLineTypes = RegisterLineTypes.Matiere;
            ViewBag.Docs = await dataProvider.GetRegister(from ?? fromDate, to ?? toDate, codeai ?? "", RegisterLineTypes.Matiere);
            ViewBag.minDate = DateTime.Now.AddYears(-2);

            ViewBag.fromDate = DateTime.ParseExact(from ?? fromDate, _mySettings.NavSettings.NavDateFormat, null);
            ViewBag.toDate = DateTime.ParseExact(to ?? toDate, _mySettings.NavSettings.NavDateFormat, null);


            HttpContext.Session.Clear();

            return View();
        }

        public async Task<IActionResult> RegistrePrestation(string? from, string? to, string? codeai)
        {
            DateHelper.VerifyInputDate(ref from, ref to, _mySettings.NavSettings.NavDateFormat);
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            var fromDate = DateTime.Now.AddYears(-1).ToString(_mySettings.NavSettings.NavDateFormat);
            var toDate = DateTime.Now.ToString(_mySettings.NavSettings.NavDateFormat);

            ViewBag.RegisterLineTypes = RegisterLineTypes.Prestation;
            ViewBag.Docs = await dataProvider.GetRegister(from ?? fromDate, to ?? toDate, codeai ?? "", RegisterLineTypes.Prestation);
            ViewBag.minDate = DateTime.Now.AddYears(-2);

            ViewBag.fromDate = DateTime.ParseExact(from ?? fromDate, _mySettings.NavSettings.NavDateFormat, null);
            ViewBag.toDate = DateTime.ParseExact(to ?? toDate, _mySettings.NavSettings.NavDateFormat, null);

            HttpContext.Session.Clear();

            return View();
        }

        public async Task<IActionResult> RegistreChronologique(string? from, string? to, string? codeai)
        {
            DateHelper.VerifyInputDateChrono(ref from, ref to, _mySettings.NavSettings.NavDateFormat);
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            var fromDate = DateTime.Parse("01/01/" + DateTime.Now.Year).ToString(_mySettings.NavSettings.NavDateFormat);
            var toDate = DateTime.Now.ToString(_mySettings.NavSettings.NavDateFormat);

            var docs = dataProvider.GetRegister(from ?? fromDate, to ?? toDate, codeai ?? "", RegisterLineTypes.Matiere);
            var printedRequests = dataProvider.GetRegisterPrintRequests();
            await Task.WhenAll(new[] { docs, printedRequests });

            ViewBag.Docs = docs?.Result;
            ViewBag.PrintedRequests =printedRequests?.Result;

            ViewBag.RegisterLineTypes = RegisterLineTypes.Chronologique;
            ViewBag.minDate = DateTime.Now.AddYears(-2);
            ViewBag.fromDate = DateTime.ParseExact(from ?? fromDate, _mySettings.NavSettings.NavDateFormat, null);
            ViewBag.toDate = DateTime.ParseExact(to ?? toDate, _mySettings.NavSettings.NavDateFormat, null);

            HttpContext.Session.Clear();

            return View();
        }

        [HttpPost]
        public async Task<List<DocumentDownloadLink>> DownloadDocsChrono(string entryNo)
        {
            if(string.IsNullOrWhiteSpace(entryNo)) return null;

            var user = (Member)ViewBag.User;
            var dataProvider = new DataProvider(_mySettings, user);
            var printedRequests = await dataProvider.GetRegisterPrintRequests();

            List<ECManagementWS.PrintRequest> lines = ((ECManagementWS.PrintRequest[])printedRequests.Data)?.ToList();
            var mydoc = lines?.Find(x=>x.EntryNo == int.Parse(entryNo));

            //var filename = mydoc.Filename?.ToLower()?.Replace(_mySettings.NavSettings.FilePath.ToLower(), DataProvider._defaultFilePathName + "/").Replace("\\", "/");

            var links = mydoc != null ? new List<DocumentDownloadLink> { new DocumentDownloadLink(){ DocNo = "Registre", FileName = mydoc.Filename } } : null;

            return (links?.Any() ?? false) ? await CheckFileExistsOnServer(links) : null;
        }

        [HttpPost]
        public async Task<List<DocumentDownloadLink>> DownloadDocs(string? from, string? to, string? ai)
        {
            DateHelper.VerifyInputDate(ref from, ref to, _mySettings.NavSettings.NavDateFormat);
            DateHelper.CheckYear(DateTime.Now.Year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            var fromDateStr = DateTime.ParseExact(from ?? fromDate, _mySettings.NavSettings.NavDateFormat, null);
            var toDateStr = DateTime.ParseExact(to ?? toDate, _mySettings.NavSettings.NavDateFormat, null);

            var links = HttpContext.Session.Get<List<DocumentDownloadLink>>(DOWNLOADREGISTER);
            bool notfound = false;
            if (links?.Any() ?? false)
            {
                return await CheckFileExistsOnServer(links);
            }

            return await DownloadRegister(fromDateStr.ToString(_mySettings.NavSettings.NavDateFormat), toDateStr.ToString(_mySettings.NavSettings.NavDateFormat), ai, false);
        }

        private async Task<List<DocumentDownloadLink>> DownloadRegister(string? from, string? to, string? ai, bool isForZip)
        {
            if (from == null || to == null)
                throw new ArgumentNullException();

            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);

            NavResponse path = await dataProvider.GetCustRegister(from, to, ai ?? "");
            if(!path?.Success ?? false)
                throw new Exception("Erreur durant le téléchargement du fichier");

            var registerDownloadPath = (string)path?.Data;


            int i = -1;
            List<DocumentDownloadLink> linksToCheck = new List<DocumentDownloadLink>();
            List<DocumentDownloadLink> links = new List<DocumentDownloadLink>();

            FileInfo fileInfo = null;
            try { fileInfo = new FileInfo(registerDownloadPath); } catch { }

            string FileName = !string.IsNullOrWhiteSpace(fileInfo?.Extension) && (fileInfo?.Exists ?? false) ? registerDownloadPath : "";
            if (string.IsNullOrEmpty(FileName))
                linksToCheck.Add(new DocumentDownloadLink()
                {
                    DocNo = "Registre",
                    FileName = registerDownloadPath
                });

            if (!isForZip)
                FileName = FileName?.ToLower()?.Replace(_mySettings.NavSettings.FilePath.ToLower(), DataProvider._defaultFilePathName + "/").Replace("\\", "/");

            links.Add(new DocumentDownloadLink()
            {
                DocNo = "Registre",
                FileName = FileName
            });


            if (linksToCheck?.Any() ?? false)
                HttpContext.Session.Set(DOWNLOADREGISTER, linksToCheck);

            return links;
        }

        private async Task<List<DocumentDownloadLink>> CheckFileExistsOnServer(List<DocumentDownloadLink> links)
        {
            List<DocumentDownloadLink> linksToCheck = new List<DocumentDownloadLink>();
            foreach (var link in links)
            {
                FileInfo fileInfo = null;
                try { fileInfo = new FileInfo(link.FileName); } catch { }

                string FileName = !string.IsNullOrWhiteSpace(fileInfo?.Extension) && (fileInfo?.Exists ?? false) ? link.FileName : "";
                if (string.IsNullOrEmpty(FileName))
                {
                    linksToCheck.Add(new DocumentDownloadLink()
                    {
                        DocNo = link.DocNo,
                        FileName = link.FileName,
                    });
                    link.FileName = "";
                }
                else
                {
                    link.FileName = FileName?.ToLower()?.Replace(_mySettings.NavSettings.FilePath.ToLower(), DataProvider._defaultFilePathName + "/").Replace("\\", "/");
                }
            }

            if (linksToCheck?.Any() ?? false)
                HttpContext.Session.Set(DOWNLOADREGISTER, linksToCheck);

            return links;
        }

    }
}
