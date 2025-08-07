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

using Extranet.Models.Documents;
using Extranet.Models.Helpers;
using Extranet.Models.Members;
using Extranet.Models.Settings;
using Extranet.Models.Stats;
using System.Net;
using System.ServiceModel;

namespace Extranet.Models
{
    public class DataProvider(WebSettings websettings, Member webuser)
    {
        private Random rdn = new Random();
        private readonly WebSettings _websettings = websettings;
        private readonly Member _webuser = webuser;

        private const string hardSalt = "$¨L8@~";
        public const string _defaultCompany = "SCHROLL%20SAS";

        public const string _defaultFilePathName = "/download";
        public const string _defaultSharedFilePathName = "/downloadshared";

        public const string NAV_TRUE = "Oui";
        public const string NAV_FALSE = "Non";
        public const string _defaultContactPicture = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAA7YAAAN2CAYAAADNEhUsAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2hpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoxMjZFRDRBNzc4MjI2ODExODA4M0NCRTRDOUMwMzYyQSIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDoxRDNCOTJFQzJGN0MxMUUzODZGQkEzMTA4RTE1NDFEQiIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDoxRDNCOTJFQjJGN0MxMUUzODZGQkEzMTA4RTE1NDFEQiIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChNYWNpbnRvc2gpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6MTM2RUQ0QTc3ODIyNjgxMTgwODNDQkU0QzlDMDM2MkEiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6MTI2RUQ0QTc3ODIyNjgxMTgwODNDQkU0QzlDMDM2MkEiLz4gPC9yZGY6RGVzY3Jp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";

        /// <summary>
        /// Permet de se loguer et de récupérer
        /// - les infos de l'interlocuteur
        /// - les comptes et adresses d'intervention et sociétés sur lesquels il est paramétré
        /// - Informations du référent commercial.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<NavResponse<ECManagementWS.UserAuthentication_Result>> UserAuthentication(string login, string pwd, string company)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS(company);
            NavResponse<ECManagementWS.UserAuthentication_Result> response = new();
            try
            {
                string digest = SHAHelper.Hash(hardSalt + pwd);

                response.Data = await service.UserAuthenticationAsync(new()
                {
                    pAccountExtranetInfos = new(),
                    pMail = login,
                    pPassword = digest,
                    errorMsg = "",
                });

                response.Success = response.Data?.return_value ?? false;
                response.Message = response.Data?.errorMsg ?? "";
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "UserAuthenticationAsync:@login=" + login + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = "Erreur de communication avec le serveur, veuillez réessayer plus tard.";
            }

            return response;
        }

        public bool CheckAdminLogin(string login, string pwd)
        {
            string digest = SHAHelper.Hash(hardSalt + pwd);
            return login == _websettings.NavSettings.NewsLogin && digest == _websettings.NavSettings.NewsPwd;
        }

        /// <summary>
        /// Permet de réinitialiser le mot de passe. Renvoie un mail à l'utilisateur et regénère un token. 
        /// L'utiliser avec le AskConfirmation = false;
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<NavResponse<ECManagementWS.ResetPassword_Result>> ResetPassword(string login)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS(_defaultCompany);
            NavResponse<ECManagementWS.ResetPassword_Result> response = new();
            try
            {
                response.Data = await service.ResetPasswordAsync(new()
                {
                    pMail = login,
                    errorMsg = ""
                });
                response.Success = response.Data?.return_value ?? false;
                response.Message = response.Data?.errorMsg ?? "";
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "ResetPasswordAsync:@login=" + login + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = "Erreur de communication avec le serveur, veuillez réessayer plus tard.";
            }

            return response;
        }

        /// <summary>
        /// Permet de sauvegarder le mot de passe
        /// </summary>
        /// <param name="login"></param>
        /// <param name="token"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<NavResponse<ECManagementWS.SavePassword_Result>> SavePassword(string login, string token, string pwd)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS(_defaultCompany);
            NavResponse<ECManagementWS.SavePassword_Result> response = new();
            try
            {
                string digest = SHAHelper.Hash(hardSalt + pwd);

                response.Data = await service.SavePasswordAsync(new()
                {
                    errorMsg = "",
                    pMail = login,
                    pPassword = digest,
                    pToken = token
                });
                response.Success = response.Data?.return_value ?? false;
                response.Message = response.Data?.errorMsg ?? "";
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "SavePasswordAsync:Error:" + e.Message, e);
                response.Success = false;
                response.Message = "Erreur de communication avec le serveur, veuillez réessayer plus tard.";
            }

            return response;
        }

        public async Task<NavResponse<ECManagementWS.UpdatePassword_Result>> UpdatePassword(string mail, string oldPassword, string newPassword, string name, string lastName)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS(_defaultCompany);
            NavResponse<ECManagementWS.UpdatePassword_Result> response = new();
            try
            {
                string digestOldPwd = SHAHelper.Hash(hardSalt + oldPassword);
                string digestNewPwd = SHAHelper.Hash(hardSalt + newPassword);

                response.Data = await service.UpdatePasswordAsync(new()
                {
                    errorMsg = "",
                    pMail = mail,
                    pLastName = lastName,
                    pName = name,
                    pNewPassword = digestNewPwd,
                    pOldPassword = digestOldPwd,
                });
                response.Success = response.Data?.return_value ?? false;
                response.Message = response.Data?.errorMsg ?? "";
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "UpdatePasswordAsync:Error:" + e.Message, e);
                response.Success = false;
                response.Message = "Erreur de communication avec le serveur, veuillez réessayer plus tard.";
            }

            return response;
        }

        /// <summary>
        /// Dossier pour partage de fichier
        /// </summary>
        /// <returns></returns>
        public async Task<NavResponse<ECManagementWS.GetInterlocFolder_Result>> GetInterlocFolder()
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS(_webuser?.CurrentCompany);
            NavResponse<ECManagementWS.GetInterlocFolder_Result> response = new();
            try
            {
                response.Data = await service.GetInterlocFolderAsync(_webuser?.CurrentCompany, _webuser?.CurrentAccount?.AccountNo, _webuser?.Email);

                response.Success = true;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetInterlocFolderAsync:Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        #region Stats

        public async Task<NavResponse<ECManagementWS.GetStatistics_Result>> GetStatistics(string fromDate, string toDate, StatTypes statType)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse<ECManagementWS.GetStatistics_Result> response = new();
            try
            {
                response.Data = await service.GetStatisticsAsync(new ECManagementWS.GetStatistics()
                {
                    accountNo = _webuser?.CurrentAccount?.AccountNo,
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser?.Email,
                    statType = statType.ToString(),
                    statisticList = new()
                });
                response.Success = true;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetStatisticsAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@statType=" + statType +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task<NavResponse<ECManagementWS.GetNbPassage_Result>> GetNbPassage(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse<ECManagementWS.GetNbPassage_Result> response = new();
            try
            {
                response.Data = await service.GetNbPassageAsync(_webuser?.Email, _webuser?.CurrentAccount?.AccountNo, fromDate, toDate);
                response.Success = true;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetNbPassageAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task<NavResponse<ECManagementWS.GetLastAccountingPeriod_Result>> GetLastAccountingPeriod()
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS(_webuser?.CurrentCompany);
            NavResponse<ECManagementWS.GetLastAccountingPeriod_Result> response = new();
            try
            {
                response.Data = await service.GetLastAccountingPeriodAsync(new ECManagementWS.GetLastAccountingPeriod()
                {
                    endingDate = "",
                    startingDate = ""
                });

                response.Success = true;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetLastAccountingPeriodAsync:Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task<NavResponse<ECManagementWS.GetDetailsOfServiceCosts_Result>> GetDetailsOfServiceCosts(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse<ECManagementWS.GetDetailsOfServiceCosts_Result> response = new();
            try
            {
                response.Data = await service.GetDetailsOfServiceCostsAsync(new ECManagementWS.GetDetailsOfServiceCosts()
                {
                    userMail = _webuser.Email,
                    accountNo = _webuser.CurrentAccount.AccountNo,
                    detailsServiceCosts = new ECManagementWS.InvoiceLines(),
                    fromDate = fromDate,
                    toDate = toDate
                });
                response.Success = true;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetDetailsOfServiceCostsAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        #endregion

        #region Lists

        /// <summary>
        /// Documents Biodéchets
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetBioWasteList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetBioWasteListAsync(new()
                {
                    accountNo = webuser.CurrentAccount.AccountNo,
                    bioDocumentList = new(),
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser.Email
                });
                response.Success = true;

                response.Data = data?.bioDocumentList?.Dop;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetBioWasteListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Bulletins de travail
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetWorkReportList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetWorkReportListAsync(new()
                {
                    accountNo = webuser.CurrentAccount.AccountNo,
                    dopList = new(),
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser.Email
                });
                response.Success = true;

                response.Data = data?.dopList?.Dop;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetWorkReportListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Certificats de destruction
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetDestructionCertificateList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetDestructionCertificateListAsync(new()
                {
                    accountNo = webuser.CurrentAccount.AccountNo,
                    certificateList = new(),
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser.Email
                });
                response.Success = true;

                response.Data = data?.certificateList?.Certificate;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetDestructionCertificateListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Décomptes d'achat
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetPurchaseStatementList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetPurchaseStatementListAsync(new()
                {
                    accountNo = webuser.CurrentAccount.AccountNo,
                    purchaseStatementList = new(),
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser.Email
                });
                response.Success = true;

                response.Data = data?.purchaseStatementList?.PurchaseStatement;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetPurchaseStatementListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Tickets de réception
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetReceiptTicketList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetReceiptTicketListAsync(new()
                {
                    accountNo = webuser.CurrentAccount.AccountNo,
                    dopList = new(),
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser.Email
                });
                response.Success = true;

                response.Data = data?.dopList?.Dop;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetReceiptTicketListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Avoirs
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetSalesCreditMemoList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetSalesCreditMemoListAsync(new()
                {
                    accountNo = webuser.CurrentAccount.AccountNo,
                    salesCrMemoList = new(),
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser.Email
                });
                response.Success = true;

                response.Data = data?.salesCrMemoList?.SalesCrMemoHeader;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetSalesCreditMemoListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Factures
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetSalesInvoiceList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetSalesInvoiceListAsync(new()
                {
                    accountNo = webuser.CurrentAccount.AccountNo,
                    salesInvoiceList = new(),
                    fromDate = fromDate,
                    toDate = toDate,
                    userMail = _webuser.Email
                });
                response.Success = true;

                response.Data = data?.salesInvoiceList?.SalesInvoiceHeader;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetSalesInvoiceListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Attestations de valorisation
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetValuationCertificateList(string fromDate, string toDate)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                //var data = await service.GetValuationCertificateListAsync(new()
                //{
                //    accountNo = webuser.CurrentAccount.AccountNo,
                //    salesInvoiceList = new(),
                //    fromDate = fromDate,
                //    toDate = toDate,
                //    userMail = _webuser.Email
                //});
                response.Success = true;

                //response.Data = data?.salesInvoiceList?.SalesInvoiceHeader;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetValuationCertificateListAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        #endregion

        #region Registre

        /// <summary>
        /// Registre
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="registerLineTypes">RegisterLineTypes</param>
        /// <returns></returns>
        public async Task<NavResponse> GetRegister(string fromDate, string toDate, string codeAi, RegisterLineTypes registerLineTypes)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetRegisterAsync(new ECManagementWS.GetRegister()
                {
                    accountNo = _webuser?.CurrentAccount?.AccountNo,
                    fromDate = fromDate,
                    toDate = toDate,
                    aICode = codeAi ?? "",
                    userMail = _webuser.Email,
                    registerList = new(),
                    lineType = registerLineTypes.ToString()
                });
                response.Success = true;

                response.Data = data?.registerList?.TI;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetRegisterAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Liste des exports registre chronologique réalisés
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="codeAi"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetRegisterPrintRequests(string fromDate = "", string toDate = "", string codeAi = "")
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetRegisterPrintRequestsAsync(new ECManagementWS.GetRegisterPrintRequests()
                {
                    accountNo = _webuser?.CurrentAccount?.AccountNo,
                    fromDate = fromDate,
                    toDate = toDate,
                    aICode = codeAi ?? "",
                    userMail = _webuser.Email,
                    registerPrintRequests = new()
                });
                response.Success = true;

                response.Data = data?.registerPrintRequests?.PrintRequest;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetRegisterPrintRequestsAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        /// <summary>
        /// Renvoie le chemin d'accès du Registre des déchets sortant du client stocké sur le serveur.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="registerLineTypes"></param>
        /// <returns></returns>
        public async Task<NavResponse> GetCustRegister(string fromDate, string toDate, string codeAI)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse response = new();

            try
            {
                var data = await service.GetCustRegisterAsync(_webuser?.Email, _webuser?.CurrentAccount?.AccountNo, fromDate, toDate, codeAI ?? "");
                response.Success = true;

                response.Data = data?.return_value;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetCustRegisterAsync:@accountNo=" + webuser?.CurrentAccount?.AccountNo +
                    ";@fromDate=" + fromDate +
                    ";@toDate=" + toDate + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        #endregion

        #region Responsibility Center

        public async Task<NavResponse<ECManagementWS.GetResponsibilityCenterList_Result>> GetResponsibilityCenterList(string company)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse<ECManagementWS.GetResponsibilityCenterList_Result> response = new();
            try
            {
                response.Data = await service.GetResponsibilityCenterListAsync(new ECManagementWS.GetResponsibilityCenterList()
                {
                    companyNameTxt = company,
                    responsibilityCenterList = new(),
                    userMail = _webuser.Email
                });

                response.Success = true;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetResponsibilityCenterListAsync:@company=" + company + ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        #endregion

        public async Task<ECSystemService.Companies_Result> GetCompanies()
        {
            ECSystemService.SystemService_PortClient service = InitECSystemService();
            ECSystemService.Companies_Result res = null;

            try
            {
                res = await service.CompaniesAsync();
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "CompaniesAsync:Error:" + e.Message, e);
            }

            return res;
        }

        public async Task<bool> GetMaintenanceInProgress()
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            ECManagementWS.GetMaintenanceInProgress_Result res = null;

            try
            {
                res = await service.GetMaintenanceInProgressAsync();
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetMaintenanceInProgressAsync:Error:" + e.Message, e);
            }

            return res?.return_value ?? false;
        }

        #region Notifs documents

        /// <summary>
        /// Récupération des notification du nombre de doc non lus
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetNewDocsToRead()
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            ECManagementWS.GetNewDocsToRead_Result res = null;

            try
            {
                res = await service.GetNewDocsToReadAsync(new ECManagementWS.GetNewDocsToRead()
                {
                    pMail = _webuser.Email,
                    pAccountNo = _webuser.CurrentAccount.AccountNo,
                    newDocsToPopup = new ECManagementWS.NewDocs()
                });
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetNewDocsToReadAsync:Error:" + e.Message, e);
            }

            return res?.return_value ?? 0;
        }

        /// <summary>
        /// Envoie une confirmation de lecture du document
        /// </summary>
        /// <param name="pPath"></param>
        /// <returns></returns>
        public async Task<bool> SetNewDocAsRead(string pPath)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            ECManagementWS.SetNewDocAsRead_Result res = null;

            try
            {
                res = await service.SetNewDocAsReadAsync(_webuser.Email, pPath, true);
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "SetNewDocAsReadAsync:Error:" + e.Message, e);
            }

            return res?.return_value ?? false;
        }

        #endregion

        #region PDF

        public async Task<NavResponse<ECManagementWS.GetDocument_Result>> GetDocuments(DocTypes docType, string docNo)
        {
            ECManagementWS.CustomerExtranet_PortClient service = InitECManagementWS();
            NavResponse<ECManagementWS.GetDocument_Result> response = new();

            try
            {
                response.Data = await service.GetDocumentAsync(docType.ToString(), webuser.Email, docNo);
                response.Success = true;
            }
            catch (Exception e)
            {
                await ReportError(_websettings, "GetDocumentAsync:@DocTypes=" + docType + ", @docNo=" + docNo + ", " +
                    "@webuser.Email=" + webuser.Email +
                    ";Error:" + e.Message, e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        #endregion

        #region WebServices

        /// <summary>
        /// Initialisation du service NAV
        /// </summary>
        /// <returns></returns>
        private ECSystemService.SystemService_PortClient InitECSystemService()
        {
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);//Error : Authentication failed because the connection could not be reused.

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            ECSystemService.SystemService_PortClient service =
            new ECSystemService.SystemService_PortClient(CreateHttpBinding(),
                    new EndpointAddress(new Uri(_websettings.NavSettings.ECSystemService)));

            service.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = _websettings.NavSettings.Domain,
                UserName = _websettings.NavSettings.User,
                Password = _websettings.NavSettings.Pwd
            };
            service.ClientCredentials.Windows.AllowedImpersonationLevel =
                System.Security.Principal.TokenImpersonationLevel.Delegation;

            return service;
        }

        /// <summary>
        /// Initialisation du service NAV
        /// </summary>
        /// <returns></returns>
        private ECManagementWS.CustomerExtranet_PortClient InitECManagementWS(string company = null)
        {
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);//Error : Authentication failed because the connection could not be reused.

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            ECManagementWS.CustomerExtranet_PortClient service =
            new ECManagementWS.CustomerExtranet_PortClient(CreateHttpBinding(),
                    new EndpointAddress(new Uri(string.Format(_websettings.NavSettings.ECManagementWS, _webuser?.CurrentCompany ?? company))));

            service.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential
            {
                Domain = _websettings.NavSettings.Domain,
                UserName = _websettings.NavSettings.User,
                Password = _websettings.NavSettings.Pwd
            };
            service.ClientCredentials.Windows.AllowedImpersonationLevel =
                System.Security.Principal.TokenImpersonationLevel.Delegation;

            return service;
        }

        /// <summary>
        /// Création de la liaison Ntlm
        /// </summary>
        /// <returns></returns>
        private BasicHttpBinding CreateHttpBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding()
            {
                Name = "basichttpbinding",
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
            };
            TimeSpan timeout = new TimeSpan(0, 1, 0);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            binding.CloseTimeout = timeout;

            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;//!!!
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;//!!!

            return binding;
        }

        #endregion

        #region Erreur Log

        private static SemaphoreSlim lock_ = new SemaphoreSlim(1, 1);
        private static string ExtLogError = ".caplog";
        /// <summary>
        /// Fonction de rapport d'erreur journalier
        /// </summary>
        /// <param name="error"></param>
        public static async Task<bool> ReportError(WebSettings _websettings, string error, Exception? e = null)
        {

            try
            {
                await lock_.WaitAsync();

                if (e != null)
                {
                    var ex = e;
                    while (ex.InnerException != null)
                    {
                        error += "\nex.InnerException.Message::" + ex.InnerException.Message;

                        ex = ex.InnerException;
                    }
                }

                DateTime dtNow = DateTime.Now;
                string fichierError = dtNow.ToShortDateString().Replace("/", "");

                string log_file = _websettings.NavSettings.ErrorLogPath + fichierError + ExtLogError;
                if (File.Exists(log_file))
                {
                    await File.AppendAllTextAsync(log_file, "\r\n" + dtNow.ToString() + " :: " + error);
                }
                else
                {
                    StreamWriter responseWriter = new StreamWriter(log_file);
                    await responseWriter.WriteLineAsync(dtNow.ToString() + " :: " + error);
                    await responseWriter.FlushAsync();
                    responseWriter.Dispose();
                    responseWriter.Close();
                }
            }
            finally
            {
                lock_.Release();
            }

            return true;
        }

        #endregion
    }
}
