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
using Extranet.Extensions;
using Extranet.Models;
using Extranet.Models.Documents;
using Extranet.Models.Guards;
using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO.Compression;

namespace Extranet.Controllers
{
    [MemberGuard]
    public class DocumentsController(ILogger<Controller> logger, IOptions<WebSettings> values) : BaseMemberController(logger, values)
    {
        private const string DOWNLOADLINKS = "DownloadLinks";
        private const string OUTPUTZIP = "OutputZip";

        public async Task<IActionResult> AttestationsValorisation(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetValuationCertificateList(fromDate, toDate);

            return View();
        }

        public async Task<IActionResult> BulletinsDeTravail(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetWorkReportList(fromDate, toDate);

            return View();
        }

        public async Task<IActionResult> TicketsDeReception(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetReceiptTicketList(fromDate, toDate);

            return View();
        }

        public async Task<IActionResult> CertificatsDeDestruction(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetDestructionCertificateList(fromDate, toDate);

            return View();
        }

        public async Task<IActionResult> DocumentsBiodechets(int? year)
        {
            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);
            DateHelper.CheckYear(year, _mySettings.NavSettings.NavDateFormat, out string fromDate, out string toDate);

            ViewBag.Docs = await dataProvider.GetBioWasteList(fromDate, toDate);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetResponsibilityCenterList()
        {
            var user = (Member)ViewBag.User;
            var dataProvider = new DataProvider(_mySettings, user);

            List<Task> tasks = new List<Task>();
            if (!user?.CurrentAccount?.Companies?.Company?.Any() ?? false) return new EmptyResult();

            foreach (var company in user?.CurrentAccount?.Companies?.Company)
            {
                tasks.Add(dataProvider.GetResponsibilityCenterList(company.CompanyName));
            }

            await Task.WhenAll(tasks);

            List<ResponsibilityCenters> responsibilityCenters = new List<ResponsibilityCenters>();
            foreach (Task<NavResponse<GetResponsibilityCenterList_Result>> task in tasks)
            {
                if ((!task?.Result?.Success ?? false) ||
                    (!task?.Result?.Data?.responsibilityCenterList?.Company?.Any() ?? false)) continue;

                responsibilityCenters.AddRange(task?.Result?.Data?.responsibilityCenterList?.Company);
            }

            return Json(responsibilityCenters);
        }

        [HttpPost]
        public async Task<List<DocumentDownloadLink>> DownloadDocs([FromBody] DownloadDocs downloadDocs)
        {
            if (!downloadDocs?.docNos?.Any() ?? false)
                throw new Exception("array is null or empty");

            var links = HttpContext.Session.Get<List<DocumentDownloadLink>>(DOWNLOADLINKS);
            bool notfound = false;
            if (links?.Any() ?? false)
            {
                foreach (var docNo in downloadDocs.docNos)
                {
                    if (!links.Where(x => x.DocNo == docNo)?.Any() ?? false)
                    {
                        notfound = true;
                        HttpContext.Session.Clear();
                        break;
                    }
                }

                if (!notfound)
                    return await CheckFileExistsOnServer(links);
            }

            return await DownloadDocFromNAV(downloadDocs, false);
        }

        private async Task<List<DocumentDownloadLink>> DownloadDocFromNAV(DownloadDocs downloadDocs, bool isForZip)
        {
            if (!downloadDocs?.docNos?.Any() ?? false)
                throw new Exception("array is null or empty");

            var dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);

            List<Task> tasks = new List<Task>();
            foreach (string docNo in downloadDocs.docNos)
            {
                tasks.Add(dataProvider.GetDocuments(downloadDocs.docType, docNo));
            }

            await Task.WhenAll(tasks);

            int i = -1;
            List<DocumentDownloadLink> linksToCheck = new List<DocumentDownloadLink>();
            List<DocumentDownloadLink> links = new List<DocumentDownloadLink>();
            foreach (Task<NavResponse<ECManagementWS.GetDocument_Result>> task in tasks)
            {
                i++;
                if (!task?.Result?.Success ?? false)
                    throw new Exception("Erreur durant le téléchargement du fichier");

                FileInfo fileInfo = null;
                try { fileInfo = new FileInfo(task!.Result.Data?.return_value); } catch { }

                string FileName = !string.IsNullOrWhiteSpace(fileInfo?.Extension) && (fileInfo?.Exists ?? false) ? task!.Result.Data?.return_value : "";
                if (string.IsNullOrEmpty(FileName))
                    linksToCheck.Add(new DocumentDownloadLink()
                    {
                        DocNo = downloadDocs.docNos[i],
                        FileName = task!.Result.Data?.return_value
                    });

                if (!isForZip)
                    FileName = FileName?.ToLower()?.Replace(_mySettings.NavSettings.FilePath.ToLower(), DataProvider._defaultFilePathName + "/").Replace("\\", "/");

                links.Add(new DocumentDownloadLink()
                {
                    DocNo = downloadDocs.docNos[i],
                    FileName = FileName
                });
            }

            if (linksToCheck?.Any() ?? false)
                HttpContext.Session.Set(DOWNLOADLINKS, linksToCheck);

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
                HttpContext.Session.Set(DOWNLOADLINKS, linksToCheck);

            return links;
        }

        [HttpPost]
        public async Task<IActionResult> Zip([FromBody] DownloadDocs downloadDocs)
        {
            var files = await DownloadDocFromNAV(downloadDocs, true);

            if (!files?.Any() ?? false)
                return StatusCode(404, "No files found");

            try
            {
                // Create a temporary memory stream to hold the zip archive
                using (var memoryStream = new MemoryStream())
                {
                    // Create a new zip archive
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            var fileInfo = new FileInfo(file.FileName);

                            // Create a new entry in the zip archive for each file
                            var entry = zipArchive.CreateEntry(fileInfo.Name);

                            // Write the file contents into the entry
                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(file.FileName, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    //the zip file as a byte array
                    var data = memoryStream.ToArray();
                    HttpContext.Session.Set<byte[]>(OUTPUTZIP, data);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

            return Ok();
        }

        public async Task<IActionResult> DownloadZip()
        {
            var array = HttpContext.Session.Get<byte[]>(OUTPUTZIP);
            if (array != null)
            {
                return File(array, "application/zip", "documents.zip");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }

    public class DownloadDocs
    {
        public string type { get; set; }
        public List<string> docNos { get; set; }

        public DocTypes docType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(type))
                    return Enum.Parse<DocTypes>(type);
                else
                    return DocTypes.None;
            }
        }
    }
}
