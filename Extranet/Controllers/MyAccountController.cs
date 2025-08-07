using Extranet.Extensions;
using Extranet.Models;
using Extranet.Models.Documents;
using Extranet.Models.Guards;
using Extranet.Models.Members;
using Extranet.Models.Question;
using Extranet.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;

namespace Extranet.Controllers
{
    [MemberGuard]
    public class MyAccountController(ILogger<HomeController> logger, IOptions<WebSettings> values) : BaseMemberController(logger, values)
    {
        private static string _sharedfiledirectory;
        private static string _subDirPath;
        private const string OUTPUTZIPSHARED = "OutputZipShared";

        public async Task<IActionResult> Contact()
        {
            var currentUser = (Member)ViewBag.User;
            var dataProvider = new DataProvider(_mySettings, currentUser);
            var currentCompany = currentUser.CurrentAccount?.Companies?.Company?.Where(x => x.CompanyName == currentUser.CurrentCompany).FirstOrDefault();
            var ResponsibilityCenters = await dataProvider.GetResponsibilityCenterList(currentCompany.CompanyName);
            ViewBag.ResponsibilityCenters = ResponsibilityCenters.Data.responsibilityCenterList.Company;

            var questions = new List<Question>
            {
                new Question("Comment changer mon mot de passe ?", "Pour changer votre mot de passe, rendez-vous dans le menu Mon compte > Réglage. Cliquez ensuite sur le bouton « mettre à jour mon mot de passe »."),
                new Question("Des informations sont manquantes ou erronées sur mon espace client, que faire ?", "Si des informations sont manquantes ou vous semblent erronées sur votre espace client,<br/><br/>contactez votre conseiller éco-commercial en vous rendant dans le menu Mon Compte > Mon conseiller."),
                new Question("Comment demander la collecte de mes déchets en ligne ?", "Vous avez la possibilité de demander la collecte de vos déchets en ligne grâce à Direct’Collect. Rendez dans le menu Direct’Collect ou sur <a href=\"http://www.directcollect.fr/\" target=\"_blank\">www.directcollect.fr</a>.<br/><br/>Pour obtenir vos accès, contactez votre conseiller éco-commercial en vous rendant dans le menu Mon Compte > Mon conseiller.<br/><br/>Vous avez également la possibilité de demander la collecte de vos déchets par téléphone ou par mail. Retrouvez les numéros de téléphone et mails en vous rendant dans le menu Mon Compte > Mon conseiller > Contact."),
                new Question("Je produis des déchets dangereux, comment avoir accès à mes BSD (Bordereaux de Suivi Déchets) ?", "Retrouvez vos BSD sur la plateforme Trackdéchets.<br/><br/>Rendez-vous dans le menu Trackdéchets ou sur <a href=\"https://trackdechets.beta.gouv.fr/\" target=\"_blank\">Trackdéchets | La traçabilité des déchets en toute sécurité (beta.gouv.fr)</a>"),
                new Question("Comment obtenir mes Fiches d’Informations Préalables (FIP) ?", "Les Fiches d’Informations Préalables participent à la traçabilité de vos déchets.<br/><br/>Consultez et signez vos FIP sur notre portail dédié. Accessible 7j/7 et 24h/24, notre portail FIP vous permet d’accéder simplement et rapidement à vos Fiches d’Informations Préalables.<br/><br/>Rendez-vous dans le menu Fiches d’Informations Préalables ou sur <a href=\"https://fip.groupeschroll.fr/login\" target=\"_blank\">https://fip.groupeschroll.fr</a>"),
                };

            ViewBag.Questions = questions;
            return View();
        }

        public async Task<IActionResult> FileSharing(string subDirPath = "")
        {
            Member user = ViewBag.User;
            var dataProvider = new DataProvider(_mySettings, user);
            //rechercher les docs dans le dossier
            //\\schroll.local\m2m\Dossiers_Clients\CITRAVAL\CT003579\Espace client\rafik.touzene@capvision.fr
            //sharedfiledirectory = $"{_mySettings.NavSettings.SharedFilePath}\\Dossiers_Clients\\{user.CurrentCompany}\\{user.CurrentAccount.AccountNo}\\ESPACE CLIENT\\{user.Email}\\";
            //DEV/TEST
            //var directory = $"{_mySettings.NavSettings.SharedFilePath}\\Documents\\{user.CurrentAccount.AccountNo}\\";

            _sharedfiledirectory = (await dataProvider.GetInterlocFolder())?.Data?.return_value;
            if (!string.IsNullOrWhiteSpace(subDirPath))
            {
                _subDirPath = subDirPath.StartsWith("\\") ? subDirPath : "\\" + subDirPath;
            }
            else
            {
                _subDirPath = "";
            }

            List<DocumentDownloadLink> fichiers = await GetSharedFiles(_sharedfiledirectory + _subDirPath);

            ViewBag.Docs = new NavResponse() { Data = fichiers, Success = fichiers?.Any() ?? false };

            return View();
        }

        private async Task<List<DocumentDownloadLink>> GetSharedFiles(string directory, DownloadDocs downloadDocs = null, bool filesOnly = false)
        {
            List<DocumentDownloadLink> files = [];
            try
            {
                string[] fileEntries = Directory.GetFiles(directory);
                string[] directoryEntries = Directory.GetDirectories(directory);
                foreach (string fileName in fileEntries)
                {
                    var file = new FileInfo(fileName);
                    if (!string.IsNullOrWhiteSpace(downloadDocs?.docNos?.Find(x => x == file.Name)) || downloadDocs == null)
                    {
                        files.Add(new DocumentDownloadLink()
                        {
                            DocNo = file.Name,
                            FileName = fileName,
                            Date = file.LastWriteTime.ToString(_mySettings.NavSettings.NavDateFormat)
                        });
                    }
                }

                if (!filesOnly)
                    foreach (string dir in directoryEntries)
                    {
                        files.Add(new DocumentDownloadLink()
                        {
                            DirName = dir.Replace(_sharedfiledirectory, ""),
                            IsDirectory = true,
                        });
                    }
            }
            catch (Exception ex)
            {
                await DataProvider.ReportError(_mySettings, ex.Message);
            }

            return files;
        }

        [HttpPost]
        public async Task<IActionResult> ZipShared([FromBody] DownloadDocs downloadDocs)
        {
            Member user = ViewBag.User;
            var dataProvider = new DataProvider(_mySettings, user);
            var directory = _sharedfiledirectory + _subDirPath;
            var files = await GetSharedFiles(directory, downloadDocs, filesOnly: true);

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

                            await dataProvider.SetNewDocAsRead(file.FileName);

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
                    HttpContext.Session.Set<byte[]>(OUTPUTZIPSHARED, data);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

            return Ok();
        }

        public async Task<IActionResult> DownloadZipShared()
        {
            var array = HttpContext.Session.Get<byte[]>(OUTPUTZIPSHARED);
            if (array != null)
            {
                return File(array, "application/zip", "fichiers_partagés.zip");
            }
            else
            {
                return new EmptyResult();
            }
        }

        public IActionResult Settings()
        {
            return View();
        }

        public async Task<IActionResult> RecepisseTransportCourtage(string subDirPath = "")
        {
            Member user = ViewBag.User;
            var dataProvider = new DataProvider(_mySettings, user);

            _sharedfiledirectory = @"\\schroll.local\m2m\Espace_client\Récépissés transport\" + user.CurrentCompany;

            if (!string.IsNullOrWhiteSpace(subDirPath))
            {
                _subDirPath = subDirPath.StartsWith("\\") ? subDirPath : "\\" + subDirPath;
            }
            else
            {
                _subDirPath = "";
            }

            List<DocumentDownloadLink> fichiers = await GetSharedFiles(_sharedfiledirectory + _subDirPath);

            ViewBag.Docs = new NavResponse() { Data = fichiers, Success = fichiers?.Any() ?? false };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] JsonElement data)
        {
            try
            {
                string currentPassword = data.GetProperty("currentPassword").GetString() ?? "";
                string newPassword = data.GetProperty("newPassword").GetString() ?? "";
                string name = data.GetProperty("name").GetString() ?? "";
                string lastName = data.GetProperty("lastName").GetString() ?? "";
                string mail = data.GetProperty("mail").GetString() ?? "";

                DataProvider dataProvider = new DataProvider(_mySettings, (Member)ViewBag.User);

                var response = await dataProvider.UpdatePassword(mail, currentPassword, newPassword, name, lastName);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred : " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ContactMessage([FromBody] JsonElement data)
        {
            try
            {
                string subject = data.GetProperty("subject").GetString() ?? "";
                string message = data.GetProperty("message").GetString() ?? "";

                // Envoie du message

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred : " + ex.Message);
            }
        }

        [MemberGuard]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
