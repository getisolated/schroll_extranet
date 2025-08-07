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
using System.IO;

namespace Extranet.Models.Documents
{
    public struct IsDirectory { }
    public struct AIFilter { }
    public struct InterventionDateFilter { }
    public struct PostingDateFilter { }
    public struct DueStatusFilter { }
    public struct WeighingTicketNoFilter { }
    public struct WorkCodeFilter { }
    public struct MatterCodeFilter { }
    public struct StatusFilter { }
    public struct CodeDechetFilter { }

    public class PropDetails
    {
        public Type Type { get; set; }
        public string Value { get; set; }
        public string Header { get; set; }

        /// <summary>
        /// Pour les registres en tant que valeur de substitution pour les filtres (exemple:concaténation de 2 valeurs à afficher dans la sélection)
        /// </summary>
        public string HiddenValue { get; set; }

        /// <summary>
        /// Identifiant document pour le téléchargement
        /// </summary>
        public string DownloadId { get; set; }
        public string Symbol { get; set; }
    }

    public class Document
    {
        protected static readonly string DisplayDateFormat = "yyyy-MM-dd";
        protected static readonly string DisplayCurrencyFormat = "C2";//ou N2
        public static readonly string DisplayDecimalFormat = "F";

        public PropDetails Prop1 { get; set; }

        public PropDetails Prop2 { get; set; }

        public PropDetails Prop3 { get; set; }

        public PropDetails Prop4 { get; set; }

        public PropDetails Prop5 { get; set; }

        public PropDetails Prop6 { get; set; }
        public PropDetails Prop7 { get; set; }

        public static List<Document> GetDocuments(object NavData, RegisterLineTypes? RegisterLineType = null)
        {
            if (NavData == null)
                return null;

            List<Document> documents = new List<Document>();

            switch (NavData)
            {
                case Dop[] docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(InterventionDateFilter), Value = doc.InterventionDate, Header = "Date d'intervention", DownloadId = doc.No },
                            Prop2 = new PropDetails { Type = typeof(string), Value = doc.No, Header = "N° document" },
                            Prop3 = new PropDetails { Type = typeof(MatterCodeFilter), Value = $"{doc.MatterCode} - {doc.MatterDesc}", Header = "Matière" },
                            Prop4 = new PropDetails { Type = typeof(WorkCodeFilter), Value = $"{doc.WorkCode} - {doc.WorkDesc}", Header = "Travail" },
                            Prop5 = new PropDetails { Type = typeof(AIFilter), Value = doc.IA, Header = "Adresse d'intervention" },
                            Prop6 = new PropDetails { Type = typeof(WeighingTicketNoFilter), Value = doc.WeighingTicketNo, Header = "N° ticket de pesée" },
                        };

                        documents.Add(document);
                    }
                    break;
                case Dop1[] docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(InterventionDateFilter), Value = doc.InterventionDate, Header = "Date d'intervention", DownloadId = doc.No },
                            Prop2 = new PropDetails { Type = typeof(string), Value = doc.No, Header = "N° document" },
                            Prop3 = new PropDetails { Type = typeof(MatterCodeFilter), Value = $"{doc.MatterCode} - {doc.MatterDesc}", Header = "Matière" },
                            Prop4 = new PropDetails { Type = typeof(WorkCodeFilter), Value = $"{doc.WorkCode} - {doc.WorkDesc}", Header = "Travail" },
                            Prop5 = new PropDetails { Type = typeof(AIFilter), Value = doc.IA, Header = "Adresse d'intervention" },
                            Prop6 = new PropDetails { Type = typeof(WeighingTicketNoFilter), Value = doc.WeighingTicketNo, Header = "N° ticket de pesée" },
                        };

                        documents.Add(document);
                    }
                    break;
                case Certificate[] docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(string), Value = doc.No, Header = "N° document", DownloadId = doc.No },
                            Prop2 = new PropDetails { Type = typeof(AIFilter), Value = doc.IA, Header = "Adresse d'intervention" },
                            Prop3 = new PropDetails { Type = typeof(DateTime), Value = doc.ReleaseDate, Header = "Date de validité" },
                            Prop4 = new PropDetails { Type = typeof(string), Value = doc.State, Header = "État" }
                        };

                        documents.Add(document);
                    }
                    break;
                case PurchaseStatement[] docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(PostingDateFilter), Value = doc.PostingDate, Header = "Date comptabilisation", DownloadId = doc.No },
                            Prop2 = new PropDetails { Type = typeof(string), Value = doc.No, Header = "N° document" },
                            Prop3 = new PropDetails { Type = typeof(string), Value = doc.AccountNo, Header = "N° compte" },
                            Prop4 = new PropDetails { Type = typeof(AIFilter), Value = doc.IA, Header = "Adresse d'intervention" },
                            Prop5 = new PropDetails { Type = typeof(decimal), Value = doc.AmountExcludingVAT.ToString(DisplayDecimalFormat), Header = "Montant H.T.", Symbol = "€" }
                        };

                        documents.Add(document);
                    }
                    break;
                case SalesCrMemoHeader[] docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(PostingDateFilter), Value = doc.PostingDate, Header = "Date comptabilisation", DownloadId = doc.No },
                            Prop2 = new PropDetails { Type = typeof(string), Value = doc.No, Header = "N° document" },
                            Prop3 = new PropDetails { Type = typeof(AIFilter), Value = doc.IA, Header = "Adresse d'intervention" },
                            Prop4 = new PropDetails { Type = typeof(decimal), Value = doc.Amount.ToString(DisplayDecimalFormat), Header = "Montant H.T.", Symbol = "€" },
                            Prop5 = new PropDetails { Type = typeof(decimal), Value = doc.AmountIncludingVAT.ToString(DisplayDecimalFormat), Header = "Montant T.T.C.", Symbol = "€" }
                        };

                        documents.Add(document);
                    }
                    break;
                case SalesInvoiceHeader[] docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(PostingDateFilter), Value = doc.PostingDate, Header = "Date comptabilisation", DownloadId = doc.No },
                            Prop2 = new PropDetails { Type = typeof(string), Value = doc.No, Header = "N° document" },
                            Prop3 = new PropDetails { Type = typeof(AIFilter), Value = doc.IA, Header = "Adresse d'intervention" },
                            Prop4 = new PropDetails { Type = typeof(DateTime), Value = doc.DueDate.ToString("dd/MM/yy"), Header = "Date d'échéance" },
                            Prop5 = new PropDetails { Type = typeof(decimal), Value = doc.Amount.ToString(DisplayDecimalFormat), Header = "Montant H.T.", Symbol = "€" },
                            Prop6 = new PropDetails { Type = typeof(decimal), Value = doc.AmountIncludingVAT.ToString(DisplayDecimalFormat), Header = "Montant T.T.C.", Symbol = "€" },
                            Prop7 = new PropDetails { Type = typeof(DueStatusFilter), Value = doc.DueStatus, Header = "Statut" },
                        };

                        documents.Add(document);
                    }
                    break;
                case TI[] docs:
                    if (RegisterLineType == RegisterLineTypes.Matiere)
                    {
                        foreach (var doc in docs)
                        {
                            var document = new DocRegistre
                            {
                                Prop1 = new PropDetails { Type = typeof(string), Value = doc.DateReception.ToString(), Header = "Date de réception" },
                                Prop2 = new PropDetails { Type = typeof(string), Value = doc.NoDossier, Header = "N° dossier" },
                                Prop3 = new PropDetails { Type = typeof(string), Value = doc.CodeClassification, Header = "Code NED" },
                                Prop4 = new PropDetails { Type = typeof(string), Value = doc.NatureDechet, Header = "Nature du déchet" },
                                Prop5 = new PropDetails { Type = typeof(CodeDechetFilter), Value = doc.QualiteDechet, Header = "Code déchet", HiddenValue = $"{doc.QualiteDechet} - {doc.LibelleDechet}" },
                                Prop6 = new PropDetails { Type = typeof(string), Value = doc.LibelleDechet, Header = "Libellé du déchet" },
                                Prop7 = new PropDetails { Type = typeof(decimal), Value = doc.Quantite.ToString(DisplayDecimalFormat), Header = "Quantité" },
                                Prop8 = new PropDetails { Type = typeof(string), Value = doc.Unite, Header = "Unité" },
                                Prop9 = new PropDetails { Type = typeof(AIFilter), Value = doc.AdresseIntervention, Header = "Adresse d'intervention", HiddenValue = $"{doc.AdresseIntervention} - {doc.RaisonSocialePD}" },
                                Prop10 = new PropDetails { Type = typeof(string), Value = doc.RaisonSocialePD, Header = "Raison sociale Producteur déchet" },
                                Prop11 = new PropDetails { Type = typeof(string), Value = doc.Nom2PD, Header = "Nom 2  Producteur déchet" },
                                Prop12 = new PropDetails { Type = typeof(string), Value = doc.ContactPD, Header = "Contact Producteur déchet" },
                                Prop13 = new PropDetails { Type = typeof(string), Value = doc.AdresseVillePD, Header = "Adresse - Ville Producteur déchet" },
                                Prop14 = new PropDetails { Type = typeof(string), Value = doc.SiretPD, Header = "SIRET Producteur déchet" },
                                Prop15 = new PropDetails { Type = typeof(string), Value = doc.Materiel, Header = "Matériel" },
                                Prop16 = new PropDetails { Type = typeof(string), Value = doc.MaterielDesc, Header = "Désignation matériel" },
                                Prop17 = new PropDetails { Type = typeof(string), Value = doc.EmplacementMateriel, Header = "Emplacement Matériel" },
                                Prop18 = new PropDetails { Type = typeof(string), Value = doc.DesEmplacementMateriel, Header = "Désignation Emplacement Matériel" },
                                Prop19 = new PropDetails { Type = typeof(string), Value = doc.NomTD, Header = "Nom Transporteur du déchet" },
                                Prop20 = new PropDetails { Type = typeof(string), Value = doc.AdresseTD, Header = "Adresse Transporteur du déchet" },
                                Prop21 = new PropDetails { Type = typeof(string), Value = doc.SiretTD, Header = "SIRET Transporteur du déchet" },
                                Prop22 = new PropDetails { Type = typeof(string), Value = doc.NoRecepisseTransporteur, Header = "N° Récépissé Transporteur" },
                                Prop23 = new PropDetails { Type = typeof(string), Value = doc.ImmatriculationTransporteur, Header = "Immatriculation Transporteur" },
                                Prop24 = new PropDetails { Type = typeof(string), Value = doc.NoBSD, Header = "N° BSD" },
                                Prop25 = new PropDetails { Type = typeof(string), Value = doc.NoTicketPesee, Header = "N° Ticket de pesée" },
                                Prop26 = new PropDetails { Type = typeof(string), Value = doc.NomCourtier, Header = "Nom Courtier" },
                                Prop27 = new PropDetails { Type = typeof(string), Value = doc.AdresseCourtier, Header = "Adresse Courtier" },
                                Prop28 = new PropDetails { Type = typeof(string), Value = doc.SiretCourtier, Header = "SIRET Courtier" },
                                Prop29 = new PropDetails { Type = typeof(string), Value = doc.NoRecepisseCourtier, Header = "N° Récépissé Courtier" },
                                Prop30 = new PropDetails { Type = typeof(string), Value = doc.NomEcoOrganisme, Header = "Nom Eco-organisme" },
                                Prop31 = new PropDetails { Type = typeof(string), Value = doc.SirenEcoOrganisme, Header = "SIREN Eco-organisme" },
                                Prop32 = new PropDetails { Type = typeof(string), Value = doc.RaisonSocialeCT, Header = "Raison sociale Centre de traitement" },
                                Prop33 = new PropDetails { Type = typeof(string), Value = doc.SiretCT, Header = "SIRET Centre de traitement" },
                                Prop34 = new PropDetails { Type = typeof(string), Value = doc.AdresseCT, Header = "Adresse Centre de traitement" },
                                Prop35 = new PropDetails { Type = typeof(string), Value = doc.CodeTraitementCT, Header = "Code de traitement du Centre de traitement" },
                                Prop36 = new PropDetails { Type = typeof(string), Value = doc.CodeTraitementFinal, Header = "Code de traitement final" },
                                Prop37 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoMatiere, Header = "Taux valorisation matière", Symbol = "%" },
                                Prop38 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoEnergetique, Header = "Taux valorisation énergétique", Symbol = "%" },
                                Prop39 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoOrganique, Header = "Taux valorisation organique", Symbol = "%" },
                                Prop40 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoReemploi, Header = "Taux valorisation réemploi", Symbol = "%" },
                                Prop41 = new PropDetails { Type = typeof(decimal), Value = doc.TauxEnfouissement, Header = "Taux d'enfouissement", Symbol = "%" },
                                Prop42 = new PropDetails { Type = typeof(decimal), Value = doc.PrixUnitaire.ToString(DisplayDecimalFormat), Header = "Prix unitaire H.T.", Symbol = "€" },
                                Prop43 = new PropDetails { Type = typeof(decimal), Value = doc.Montant.ToString(DisplayDecimalFormat), Header = "Montant H.T.", Symbol = "€" },
                            };

                            documents.Add(document);
                        }
                    }
                    else if (RegisterLineType == RegisterLineTypes.Prestation)
                    {
                        foreach (var doc in docs)
                        {
                            var document = new DocRegistre
                            {
                                Prop1 = new PropDetails { Type = typeof(string), Value = doc.DateReception.ToString(), Header = "Date de réception" },
                                Prop2 = new PropDetails { Type = typeof(string), Value = doc.NoDossier, Header = "N° dossier" },
                                Prop5 = new PropDetails { Type = typeof(CodeDechetFilter), Value = doc.QualiteDechet, Header = "Code de prestation", HiddenValue = $"{doc.QualiteDechet} - {doc.LibelleDechet}" },
                                Prop6 = new PropDetails { Type = typeof(string), Value = doc.LibelleDechet, Header = "Type de prestation" },
                                Prop7 = new PropDetails { Type = typeof(decimal), Value = doc.Quantite.ToString(DisplayDecimalFormat), Header = "Quantité" },
                                Prop8 = new PropDetails { Type = typeof(string), Value = doc.Unite, Header = "Unité" },
                                Prop9 = new PropDetails { Type = typeof(AIFilter), Value = doc.AdresseIntervention, Header = "Adresse d'intervention", HiddenValue = $"{doc.AdresseIntervention} - {doc.RaisonSocialePD}" },
                                Prop10 = new PropDetails { Type = typeof(string), Value = doc.RaisonSocialePD, Header = "Raison sociale Producteur déchet" },
                                Prop11 = new PropDetails { Type = typeof(string), Value = doc.Nom2PD, Header = "Nom 2  Producteur déchet" },
                                Prop12 = new PropDetails { Type = typeof(string), Value = doc.ContactPD, Header = "Contact Producteur déchet" },
                                Prop13 = new PropDetails { Type = typeof(string), Value = doc.AdresseVillePD, Header = "Adresse - Ville Producteur déchet" },
                                Prop14 = new PropDetails { Type = typeof(string), Value = doc.SiretPD, Header = "SIRET Producteur déchet" },
                                Prop15 = new PropDetails { Type = typeof(string), Value = doc.Materiel, Header = "Matériel" },
                                Prop16 = new PropDetails { Type = typeof(string), Value = doc.MaterielDesc, Header = "Désignation matériel" },
                                Prop17 = new PropDetails { Type = typeof(string), Value = doc.EmplacementMateriel, Header = "Emplacement Matériel" },
                                Prop18 = new PropDetails { Type = typeof(string), Value = doc.DesEmplacementMateriel, Header = "Désignation Emplacement Matériel" },
                                Prop19 = new PropDetails { Type = typeof(string), Value = doc.NomTD, Header = "Nom Transporteur du déchet" },
                                Prop20 = new PropDetails { Type = typeof(string), Value = doc.AdresseTD, Header = "Adresse Transporteur du déchet" },
                                Prop21 = new PropDetails { Type = typeof(string), Value = doc.SiretTD, Header = "SIRET Transporteur du déchet" },
                                Prop22 = new PropDetails { Type = typeof(string), Value = doc.NoRecepisseTransporteur, Header = "N° Récépissé Transporteur" },
                                Prop23 = new PropDetails { Type = typeof(string), Value = doc.ImmatriculationTransporteur, Header = "Immatriculation Transporteur" },
                                Prop41 = new PropDetails { Type = typeof(decimal), Value = doc.TauxEnfouissement, Header = "Taux d'enfouissement", Symbol = "%" },
                                Prop42 = new PropDetails { Type = typeof(decimal), Value = doc.PrixUnitaire.ToString(DisplayDecimalFormat), Header = "Prix unitaire H.T.", Symbol = "€" },
                                Prop43 = new PropDetails { Type = typeof(decimal), Value = doc.Montant.ToString(DisplayDecimalFormat), Header = "Montant H.T.", Symbol = "€" },
                            };

                            documents.Add(document);
                        }
                    }
                    else if (RegisterLineType == RegisterLineTypes.Chronologique)
                    {
                        foreach (var doc in docs)
                        {
                            var document = new DocRegistre
                            {
                                Prop1 = new PropDetails { Type = typeof(string), Value = doc.DateReception.ToString(), Header = "Date de réception" },
                                Prop2 = new PropDetails { Type = typeof(string), Value = doc.NoDossier, Header = "N° dossier" },
                                Prop3 = new PropDetails { Type = typeof(string), Value = doc.CodeClassification, Header = "Code NED" },
                                Prop4 = new PropDetails { Type = typeof(string), Value = doc.NatureDechet, Header = "Nature du déchet" },
                                Prop5 = new PropDetails { Type = typeof(string), Value = doc.QualiteDechet, Header = "Code déchet" },
                                Prop6 = new PropDetails { Type = typeof(string), Value = doc.LibelleDechet, Header = "Libellé du déchet" },
                                Prop7 = new PropDetails { Type = typeof(decimal), Value = doc.Quantite.ToString(DisplayDecimalFormat), Header = "Quantité" },
                                Prop8 = new PropDetails { Type = typeof(string), Value = doc.Unite, Header = "Unité" },
                                Prop9 = new PropDetails { Type = typeof(AIFilter), Value = doc.AdresseIntervention, Header = "Adresse d'intervention", HiddenValue = $"{doc.AdresseIntervention} - {doc.RaisonSocialePD}" },
                                Prop10 = new PropDetails { Type = typeof(string), Value = doc.RaisonSocialePD, Header = "Raison sociale Producteur déchet" },
                                Prop11 = new PropDetails { Type = typeof(string), Value = doc.Nom2PD, Header = "Nom 2  Producteur déchet" },
                                Prop12 = new PropDetails { Type = typeof(string), Value = doc.ContactPD, Header = "Contact Producteur déchet" },
                                Prop13 = new PropDetails { Type = typeof(string), Value = doc.AdresseVillePD, Header = "Adresse - Ville Producteur déchet" },
                                Prop14 = new PropDetails { Type = typeof(string), Value = doc.SiretPD, Header = "SIRET Producteur déchet" },
                                Prop15 = new PropDetails { Type = typeof(string), Value = doc.Materiel, Header = "Matériel" },
                                Prop16 = new PropDetails { Type = typeof(string), Value = doc.MaterielDesc, Header = "Désignation matériel" },
                                Prop17 = new PropDetails { Type = typeof(string), Value = doc.EmplacementMateriel, Header = "Emplacement Matériel" },
                                Prop18 = new PropDetails { Type = typeof(string), Value = doc.DesEmplacementMateriel, Header = "Désignation Emplacement Matériel" },
                                Prop19 = new PropDetails { Type = typeof(string), Value = doc.NomTD, Header = "Nom Transporteur du déchet" },
                                Prop20 = new PropDetails { Type = typeof(string), Value = doc.AdresseTD, Header = "Adresse Transporteur du déchet" },
                                Prop21 = new PropDetails { Type = typeof(string), Value = doc.SiretTD, Header = "SIRET Transporteur du déchet" },
                                Prop22 = new PropDetails { Type = typeof(string), Value = doc.NoRecepisseTransporteur, Header = "N° Récépissé Transporteur" },
                                Prop23 = new PropDetails { Type = typeof(string), Value = doc.ImmatriculationTransporteur, Header = "Immatriculation Transporteur" },
                                Prop24 = new PropDetails { Type = typeof(string), Value = doc.NoBSD, Header = "N° BSD" },
                                Prop25 = new PropDetails { Type = typeof(string), Value = doc.NoTicketPesee, Header = "N° Ticket de pesée" },
                                Prop26 = new PropDetails { Type = typeof(string), Value = doc.NomCourtier, Header = "Nom Courtier" },
                                Prop27 = new PropDetails { Type = typeof(string), Value = doc.AdresseCourtier, Header = "Adresse Courtier" },
                                Prop28 = new PropDetails { Type = typeof(string), Value = doc.SiretCourtier, Header = "SIRET Courtier" },
                                Prop29 = new PropDetails { Type = typeof(string), Value = doc.NoRecepisseCourtier, Header = "N° Récépissé Courtier" },
                                Prop30 = new PropDetails { Type = typeof(string), Value = doc.NomEcoOrganisme, Header = "Nom Eco-organisme" },
                                Prop31 = new PropDetails { Type = typeof(string), Value = doc.SirenEcoOrganisme, Header = "SIREN Eco-organisme" },
                                Prop32 = new PropDetails { Type = typeof(string), Value = doc.RaisonSocialeCT, Header = "Raison sociale Centre de traitement" },
                                Prop33 = new PropDetails { Type = typeof(string), Value = doc.SiretCT, Header = "SIRET Centre de traitement" },
                                Prop34 = new PropDetails { Type = typeof(string), Value = doc.AdresseCT, Header = "Adresse Centre de traitement" },
                                Prop35 = new PropDetails { Type = typeof(string), Value = doc.CodeTraitementCT, Header = "Code de traitement du Centre de traitement" },
                                Prop36 = new PropDetails { Type = typeof(string), Value = doc.CodeTraitementFinal, Header = "Code de traitement final" },
                                Prop37 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoMatiere, Header = "Taux valorisation matière", Symbol = "%" },
                                Prop38 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoEnergetique, Header = "Taux valorisation énergétique", Symbol = "%" },
                                Prop39 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoOrganique, Header = "Taux valorisation organique", Symbol = "%" },
                                Prop40 = new PropDetails { Type = typeof(decimal), Value = doc.TauxValoReemploi, Header = "Taux valorisation réemploi", Symbol = "%" },
                                Prop41 = new PropDetails { Type = typeof(decimal), Value = doc.TauxEnfouissement, Header = "Taux d'enfouissement", Symbol = "%" },
                                Prop42 = new PropDetails { Type = typeof(decimal), Value = doc.PrixUnitaire.ToString(DisplayDecimalFormat), Header = "Prix unitaire H.T.", Symbol = "€" },
                                Prop43 = new PropDetails { Type = typeof(decimal), Value = doc.Montant.ToString(DisplayDecimalFormat), Header = "Montant H.T.", Symbol = "€" },
                            };

                            documents.Add(document);
                        }
                    }
                    break;
                case List<DocumentDownloadLink> docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(string), Value = doc.DocNo, Header = "Nom fichier" },
                            Prop2 = new PropDetails { Type = typeof(string), Value = doc.Date, Header = "Date" },
                            Prop3 = new PropDetails { Type = typeof(IsDirectory), Value = doc.IsDirectory.ToString() },
                            Prop4 = new PropDetails { Type = typeof(string), Value = doc.DirName }
                        };

                        documents.Add(document);
                    }
                    documents = documents?.OrderBy(x => x.Prop3.Value).ToList();
                    break;
                case PrintRequest[] docs:
                    foreach (var doc in docs)
                    {
                        var document = new Document
                        {
                            Prop1 = new PropDetails { Type = typeof(string), Value = doc.CreatedDate, Header = "Date d'export", DownloadId = doc.EntryNo.ToString() },
                            Prop2 = new PropDetails { Type = typeof(string), Value = doc.IACode, Header = "Filtre adresse d'intervention" },
                            Prop3 = new PropDetails { Type = typeof(string), Value = doc.Param1, Header = "Filtre date de début" },
                            Prop4 = new PropDetails { Type = typeof(string), Value = doc.Param2, Header = "Filtre date de fin" },
                        };

                        documents.Add(document);
                    }
                    break;
                default:
                    return null;
            }

            return documents;
        }


    }

    public class DocRegistre : Document
    {
        public PropDetails Prop1 { get; set; }
        public PropDetails Prop2 { get; set; }
        public PropDetails Prop3 { get; set; }
        public PropDetails Prop4 { get; set; }
        public PropDetails Prop5 { get; set; }
        public PropDetails Prop6 { get; set; }
        public PropDetails Prop7 { get; set; }
        public PropDetails Prop8 { get; set; }
        public PropDetails Prop9 { get; set; }
        public PropDetails Prop10 { get; set; }
        public PropDetails Prop11 { get; set; }
        public PropDetails Prop12 { get; set; }
        public PropDetails Prop13 { get; set; }
        public PropDetails Prop14 { get; set; }
        public PropDetails Prop15 { get; set; }
        public PropDetails Prop16 { get; set; }
        public PropDetails Prop17 { get; set; }
        public PropDetails Prop18 { get; set; }
        public PropDetails Prop19 { get; set; }
        public PropDetails Prop20 { get; set; }
        public PropDetails Prop21 { get; set; }
        public PropDetails Prop22 { get; set; }
        public PropDetails Prop23 { get; set; }
        public PropDetails Prop24 { get; set; }
        public PropDetails Prop25 { get; set; }
        public PropDetails Prop26 { get; set; }
        public PropDetails Prop27 { get; set; }
        public PropDetails Prop28 { get; set; }
        public PropDetails Prop29 { get; set; }
        public PropDetails Prop30 { get; set; }
        public PropDetails Prop31 { get; set; }
        public PropDetails Prop32 { get; set; }
        public PropDetails Prop33 { get; set; }
        public PropDetails Prop34 { get; set; }
        public PropDetails Prop35 { get; set; }
        public PropDetails Prop36 { get; set; }
        public PropDetails Prop37 { get; set; }
        public PropDetails Prop38 { get; set; }
        public PropDetails Prop39 { get; set; }
        public PropDetails Prop40 { get; set; }
        public PropDetails Prop41 { get; set; }
        public PropDetails Prop42 { get; set; }
        public PropDetails Prop43 { get; set; }
    }

}
