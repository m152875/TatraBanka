using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using Microsoft.SqlServer.Server;
using System.Web.Http.Results;
using Newtonsoft.Json.Linq;
using System.IO;
using Cora.CommIss.Iss.TatraBanka;
using Cora.Data.DBProvider;
using Cora.Natec.Base;
using System.Data;
using Cora.CommIss.Iss.ZelenaPostaAPI2RequireClient;
using Cora.Global;
using System.Web.Helpers;
using System.Xml;
using System.Xml.Linq;
using System.Web.Services.Description;
using Cora.Utils.Logger;
using System.Security.Policy;
using System.Collections.Specialized;

namespace Cora.CommIss.Iss.Impl
{
    /// <summary>
    /// TatraBankaService
    /// </summary>
    public class TatraBankaService : ITatraBankaService
    {
        static NameValueCollection TatraBankaConfig = (NameValueCollection) System.Configuration.ConfigurationManager.GetSection("TatraBankaConfig");
        
		private string baseUrl = TatraBankaConfig["TB_baseUrl"];
        private string redirectUri = TatraBankaConfig["TB_redirectUrl"];
        
        private string clientId = TatraBankaConfig["TB_clientId"];
        private string clientSecret = Crypt.CryptWithSalt.DecryptString(TatraBankaConfig["TB_clientSecret"]);

        //TODO: tieto hodnoty by mali byť generované dynamicky, nie hardcodované
        private string codeChallenge = "wUBZ2DzHuQCJdJcqbYeoJ_PlcNOFNF7GTtf4shDSPq4";
        private string codeVerifier = "FYxCWiiaYa5aMF0U_Pf6O2P6RFodtjGmH5SN6MMit9k";
        
        /// <summary>
        /// Získanie URL pre autorizáciu
        /// </summary>
        /// <param name="AuthName"></param>
        /// <param name="I_UZ"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public AuthorizationResultVm GetUrl(string AuthName, int? I_UZ)
        {
            AuthorizationResultVm result = new AuthorizationResultVm();
            string token = string.Empty;
            int dB_Add_id;

            try
            {
                #region accessToken

                var formData = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials"},
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"scope", "PREMIUM_AIS"}
                };

                string res = ApiUtils.CallPostApi(baseUrl + "/auth/oauth/v2/token", formData);

                if (string.IsNullOrEmpty(res))
                {
                    result.message = "Nepodarilo sa získať access token!";
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetUrl - Nepodarilo sa získať access token!");
                    return result;
                }

                dynamic jsonResult = JObject.Parse(res);
                token = jsonResult.access_token.ToString();


                #endregion

                #region consent

                var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", Guid.NewGuid().ToString()},
                };

                res = ApiUtils.CallPostApi(baseUrl + "/v3/consents", null, formHeaderData, token);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetUrl - Nepodarilo sa získať consentId!");
                    result.message = "Nepodarilo sa získať consentId!";
                    return result;
                }

                jsonResult = JObject.Parse(res);
                result.consentId = jsonResult.consentId.ToString();
                #endregion

                #region DB_AUTH_ADD
                using (CoraConnection conn = new CoraConnection(FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>().sConn))
                using (CoraCommand command = (CoraCommand)conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "TBPA_AUTH_ADD";

                    CoraParameter coraParameterRes = new CoraParameter("mI_AUTH", DbType.Int32) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(coraParameterRes);
                    command.Parameters.Add(new CoraParameter("mCONSENT", DbType.String) { Value = result.consentId });
                    command.Parameters.Add(new CoraParameter("mD_REQ", DbType.DateTime) { Value = DateTime.Now });
                    command.Parameters.Add(new CoraParameter("mI_UZ", DbType.Int32) { Value = I_UZ == null ? 0 : I_UZ });
                    command.Parameters.Add(new CoraParameter("mACC_COUNT", DbType.Int32) { Value = 0 });
                    command.Parameters.Add(new CoraParameter("mD_EXP", DbType.DateTime) { Value = DateTime.MinValue });
                    command.Parameters.Add(new CoraParameter("mN_REQ", DbType.String) { Value = string.IsNullOrEmpty(AuthName) ? $"TB_request - {DateTime.Now.ToString()}" : AuthName });
                    command.Parameters.Add(new CoraParameter("mZRUS", DbType.Int32) { Value = 0 });

                    command.ExecuteNonQuery();
                    dB_Add_id = Cora.Convert.Parser.GetInt(coraParameterRes.Value);
                    conn.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.GetUrl - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.GetUrl - Neošetrená chyba: {ex.Message}");
            }

            //authorization url
            result.url = $"{baseUrl}/auth/oauth/v2/authorize?client_id={clientId}" +
                    $"&response_type=code&scope=PREMIUM_AIS:{result.consentId}&state={dB_Add_id}" +
                    $"&code_challenge_method=S256&redirect_uri={redirectUri}" +
                    $"&code_challenge={codeChallenge}";

            result.message = "Úspešne vytvorená URL a získané consentId";

            return result;
        }

        /// <summary>
        /// Získanie účtov na základe consentu
        /// </summary>
        /// <param name="consent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public XmlElement GetAccounts(string consent)
        {
            try
            {
                var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", Guid.NewGuid().ToString()},
                };

                string JWT = DB_Get_JWT(consent);
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetAccounts - Nepodarilo sa získať JWT token!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať JWT token!");
                }

                string res = ApiUtils.CallApi(baseUrl + "/v3/accounts", JWT, formHeaderData);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetAccounts - Nepodarilo sa získať účty!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať účty!");
                }

                // Convert JSON to XML
                XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(res, "TBPA_Result");
				return xmlDoc.DocumentElement;
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.GetAccounts - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.GetAccounts - Neošetrená chyba: {ex.Message}");
            }
        }

        /// <summary>
        /// Získanie JWT tokenu na základe code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string TB_Get_JWT(string code)
        {
            string token = string.Empty;

            if (string.IsNullOrEmpty(code))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.TB_Get_JWT - Chýba povinný parameter code!");
                return "Chýba povinný parameter code!";
            }

            try
            {
                var formData = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"scope", "PREMIUM_AIS"},
                    {"code", code},
                    {"redirect_uri", redirectUri},
                    {"code_verifier", codeVerifier}
                };
                string svcCredentials = System.Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(clientId + ":" + clientSecret));

                var formHeaderData = new Dictionary<string, string>
                {
                    {"Authorization", "Basic " + svcCredentials},
                };
                string res = ApiUtils.CallPostApi(baseUrl + "/auth/oauth/v2/token", formData, formHeaderData);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.TB_Get_JWT - Nepodarilo sa získať JWT token!");
                    return "";
                }
                dynamic jsonResult = JObject.Parse(res);
                token = jsonResult.access_token.ToString();
                return token;
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.TB_Get_JWT - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.TB_Get_JWT - Neošetrená chyba: {ex.Message}");
            }
        }

		/// <summary>
		/// GetTransactions
		/// </summary>
		/// <param name="consent"></param>
		/// <param name="accountId"></param>
		/// <param name="amountFrom"></param>
		/// <param name="amountTo"></param>
		/// <param name="transactionDirection"></param>
		/// <param name="transactionIdFrom"></param>
		/// <param name="bankTransactionCode"></param>
		/// <param name="variableSymbol"></param>
		/// <param name="constantSymbol"></param>
		/// <param name="pageSize"></param>
		/// <param name="dateTo"></param>
		/// <param name="dateFrom"></param>
		/// <param name="specificSymbol"></param>
		/// <param name="entryReferenceFrom"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public XmlElement GetTransactions(string consent, string accountId, int? amountFrom, int? amountTo,
            string transactionDirection, int? transactionIdFrom, string bankTransactionCode,
            string variableSymbol, string constantSymbol, int? pageSize, DateTime? dateTo,
            DateTime? dateFrom, string specificSymbol, string entryReferenceFrom)
		{ 
            try
            {
                if ( string.IsNullOrEmpty(consent) || string.IsNullOrEmpty(accountId))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetTransactions - Chýbajú povinné parametre: consent alebo accountId!");
                    return ApiUtils.CreateErrorXml("Chýbajú povinné parametre: consent alebo accountId!");
                }

                string sGUID = Guid.NewGuid().ToString();
				var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", sGUID}
                };

                var paramData = new Dictionary<string, string>();
				if (amountFrom != null) paramData.Add("amountFrom", amountFrom.ToString());
                if (amountTo != null) paramData.Add("amountTo", amountTo.ToString());
                if (!string.IsNullOrEmpty(transactionDirection)) paramData.Add("transactionDirection", transactionDirection);
                if (transactionIdFrom != null) paramData.Add("transactionIdFrom", transactionIdFrom.ToString());
                if (!string.IsNullOrEmpty(bankTransactionCode)) paramData.Add("bankTransactionCode", bankTransactionCode);
                if (!string.IsNullOrEmpty(variableSymbol)) paramData.Add("variableSymbol", variableSymbol);
				if (!string.IsNullOrEmpty(constantSymbol)) paramData.Add("constantSymbol", constantSymbol);
				if (!string.IsNullOrEmpty(specificSymbol)) paramData.Add("specificSymbol", specificSymbol);
				if ( pageSize != null) paramData.Add("pageSize", pageSize.ToString());
                if (dateTo != null) paramData.Add("dateTo", ((DateTime)dateTo).ToString("yyyy-MM-dd") );
                if (dateFrom != null) paramData.Add("dateFrom", ((DateTime) dateFrom).ToString("yyyy-MM-dd"));
				if ( !string.IsNullOrEmpty(entryReferenceFrom) ) paramData.Add("entryReferenceFrom", entryReferenceFrom);

				string JWT = DB_Get_JWT(consent);
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetTransactions - Nepodarilo sa získať JWT token!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať JWT token!");
                }

                string res = ApiUtils.CallApi(baseUrl + $"/v5/accounts/{accountId}/transactions", JWT, formHeaderData, paramData);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetTransactions - Nepodarilo sa získať transakcie!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať transakcie!");
                }

			

				#region Convert JSON to XML

				XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(res, "TBPA_Result");

				var children = xmlDoc.DocumentElement.ChildNodes;
				for ( int i = children.Count - 1; i >= 0; i-- )
				{
					XmlNode child = children[i];

                    if (child.NodeType == XmlNodeType.Element)
                    {
                        string name = child.Name.ToLower();
                        if ( name != "transactions" && name != "account" )
                        {
                            xmlDoc.DocumentElement.RemoveChild(child);
                        }
                    }
                    else
                    {
                        xmlDoc.DocumentElement.RemoveChild(child);
                    }
				}
				#endregion

				#region  get entryReferenceFrom
				dynamic jsonResult = JObject.Parse(res);
				if ( jsonResult._links.next != null )
				{
					Uri uri = new Uri(jsonResult._links.next.href.ToString());
					var queryParams = HttpUtility.ParseQueryString(uri.Query);
					entryReferenceFrom = queryParams["entryReferenceFrom"];

					XmlNode entryRef = xmlDoc.CreateElement("entryReferenceFrom");
					entryRef.InnerText = entryReferenceFrom;
					xmlDoc.DocumentElement.PrependChild(entryRef);

					XmlNode guidRef = xmlDoc.CreateElement("guid");
					guidRef.InnerText = sGUID;
					xmlDoc.DocumentElement.PrependChild(guidRef);
				}
				#endregion

				return xmlDoc.DocumentElement;
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.GetTransactions - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.GetTransactions - Neošetrená chyba: {ex.Message}");
            }
        }

        /// <summary>
        /// Obnovenie autorizácie
        /// </summary>
        /// <param name="consent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
		public string RefreshAuth(string consent)
        {
            if (string.IsNullOrEmpty(consent))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RefreshAuth - Chýba povinný parameter consent!");
                return "Chýba povinný parameter consent!";
            }
            try
            {
                var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", Guid.NewGuid().ToString()},
                    {"PSU-IP-Address", "198.168.0.1"}
                };

                string JWT = DB_Get_JWT(consent);
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RefreshAuth - Nepodarilo sa získať JWT token!");
                    return "Nepodarilo sa získať JWT token!";
                }
               
                string res = ApiUtils.CallPutApi(baseUrl + $"/v3/refresh", JWT, formHeaderData);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RefreshAuth - Nepodarilo sa obnoviť autorizáciu!");
                    return $"Nepodarilo sa obnoviť autorizáciu";
                }

                dynamic jsonResult = JObject.Parse(res);
                string taskId = jsonResult.taskId.ToString();
                return $"Autorizacia bola uspešne obnovená, taskId: {taskId}";
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.RefreshAuth - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.RefreshAuth - Neošetrená chyba: {ex.Message}");
            }
        }

        /// <summary>
        /// Zmazanie autorizácie
        /// </summary>
        /// <param name="consent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string DeleteAuth(string consent)
        {
            if(string.IsNullOrEmpty(consent))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.DeleteAuth - Chýba povinný parameter consent!");
                return "Chýba povinný parameter consent!";
            }
            try 
            {
                #region DB_GET_I_AUTH
                int I_AUTH = 0;

                using (CoraConnection conn = new CoraConnection(FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>().sConn))
                using (CoraCommand cmd = (CoraCommand)conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = $"SELECT * FROM TBPA_AUTH WHERE CONSENT = '{consent}'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            I_AUTH = int.Parse(reader[0].ToString());
                        }
                        reader.Close();
                    }
                    conn.Close();
                }
                if (I_AUTH == 0)
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.DeleteAuth - Nepodarilo sa získať I_AUTH token!");
                    return "Nepodarilo sa získať I_AUTH token!";
                }
                #endregion

                var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", Guid.NewGuid().ToString()}
                };

                string JWT = DB_Get_JWT(consent);
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.DeleteAuth - Nepodarilo sa získať JWT token!");
                    return "Nepodarilo sa získať JWT token!";
                }

                string res = ApiUtils.CallDeleteApi(baseUrl + $"/v3/consents/{consent}", JWT, formHeaderData);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.DeleteAuth - Nepodarilo sa odstrániť autorizáciu!");
                    return $"Nepodarilo sa odstrániť autorizáciu!";
                }

                #region TBPA_AUTH_DEL
                using (CoraConnection conn = new CoraConnection(FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>().sConn))
                using (CoraCommand command = (CoraCommand)conn.CreateCommand())
                {
                    conn.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "TBPA_AUTH_DEL";

                    command.Parameters.Add(new CoraParameter("p_I_AUTH", DbType.Int32) { Value = I_AUTH });


                    command.ExecuteNonQuery();
                    conn.Close();
                }
                #endregion
                return "Položka bola úspešne odstránená";
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.DeleteAuth - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.DeleteAuth - Neošetrená chyba: {ex.Message}");
            }
        }

        /// <summary>
        /// Získanie JWT tokenu z databázy na základe consent
        /// </summary>
        /// <param name="consent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string DB_Get_JWT(string consent)
        {
            if (string.IsNullOrEmpty(consent))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.DB_Get_JWT - Chýba povinný parameter consent!");
                return "";
            }
            string JWT = "";
            try
            {
                using (CoraConnection conn = new CoraConnection(FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>().sConn))
                using (CoraCommand cmd = (CoraCommand)conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = $"SELECT * FROM TBPA_AUTH WHERE CONSENT = '{consent}'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            JWT = Cora.Crypt.CryptWithSalt.DecryptString(reader[5].ToString());
                        }
                        reader.Close();
                    }
                    conn.Close();
                }
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.DB_Get_JWT - Z DB sa nepodarilo získať JWT token!");
                    return "";
                }
                return JWT;
            }
            catch(Exception ex) 
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.DB_Get_JWT - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.DB_Get_JWT - Neošetrená chyba: {ex.Message}");
            }
        }

        /// <summary>
        /// Získanie detailov účtu na základe consentu a accountId
        /// </summary>
        /// <param name="consent"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public XmlElement GetAccountDetails(string consent, string accountId)
        {
            if (string.IsNullOrEmpty(consent))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetAccountDetails - Chýba povinný parameter consent!");
                return ApiUtils.CreateErrorXml("Chýba povinný parameter consent!");
            }
            if(string.IsNullOrEmpty(accountId))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetAccountDetails - Chýba povinný parameter accountId!");
                return ApiUtils.CreateErrorXml("Chýba povinný parameter accountId!");
            }
            try
            {
                var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", Guid.NewGuid().ToString()},
                };

                string JWT = DB_Get_JWT(consent);
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetAccountDetails - Nepodarilo sa získať JWT token!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať JWT token!");
                }

                string res = ApiUtils.CallApi(baseUrl + $"/v3/accounts/{accountId}", JWT, formHeaderData);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetAccountDetails - Nepodarilo sa získať detaily účtu!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať detaily účtu");
                }

                XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(res, "TBPA_Result");
                return xmlDoc.DocumentElement;
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.GetAccountDetails - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.GetAccountDetails - Neošetrená chyba: {ex.Message}"); ;
            }
        }

        /// <summary>
        /// RequestStatement
        /// </summary>
        /// <param name="consent"></param>
        /// <param name="accountId"></param>
        /// <param name="dateFromStatements"></param>
        /// <param name="dateToStatements"></param>
        /// <param name="exportType"></param>
        /// <param name="requestedSequenceNumber"></param>
        /// <param name="statementsPerDay"></param>
        /// <param name="includeDailyBalances"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public XmlElement RequestStatement(string consent, string accountId, string dateFromStatements,
            string dateToStatements, string exportType, int? requestedSequenceNumber,
            bool? statementsPerDay, bool? includeDailyBalances)
        {
            #region Parameter validation

            if (string.IsNullOrEmpty(consent))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RequestStatement - Chýba povinný parameter consent!");
                return ApiUtils.CreateErrorXml("Chýba povinný parameter consent!");
            }
            if (string.IsNullOrEmpty(accountId))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RequestStatement - Chýba povinný parameter accountId!");
                return ApiUtils.CreateErrorXml("Chýba povinný parameter accountId!");
            }
            if (string.IsNullOrEmpty(dateFromStatements))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RequestStatement - Chýba povinný parameter dateFromStatements!");
                return ApiUtils.CreateErrorXml("Chýba povinný parameter dateFromStatements");
            }
            if (string.IsNullOrEmpty(dateToStatements))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RequestStatement - Chýba povinný parameter dateToStatements!");
                return ApiUtils.CreateErrorXml("Chýba povinný parameter dateToStatements");
            }

            #endregion
            try
            {
                var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", Guid.NewGuid().ToString()},
                };

                string jsonData = JsonConvert.SerializeObject(new
                {
                    exportType = string.IsNullOrEmpty(exportType) ? "XML" : exportType,
                    dateFromStatements = dateFromStatements,
                    dateToStatements = dateToStatements,
                    requestedSequenceNumber = requestedSequenceNumber == null ? 600: requestedSequenceNumber,
                    statementsPerDay = statementsPerDay == null ? false : statementsPerDay,
                    includeDailyBalances = includeDailyBalances == null ? false : includeDailyBalances
                });

                string JWT = DB_Get_JWT(consent);
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RequestStatement - Nepodarilo sa získať JWT token!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať JWT token!");
                }

                string res = ApiUtils.CallPostApi(baseUrl + $"/v1/accounts/{accountId}/statements/tasks", null, formHeaderData, JWT, jsonData);

                if (string.IsNullOrEmpty(res))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.RequestStatement - Nepodarilo sa získať taskId!");
                    return ApiUtils.CreateErrorXml("Nepodarilo sa získať taskId");
                }

                XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(res, "TBPA_Result");
                return xmlDoc.DocumentElement;
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.RequestStatement - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.RequestStatement - Neošetrená chyba: {ex.Message}");
            }
        }

        /// <summary>
        /// Získanie výpisu na základe consentu, accountId a taskId
        /// </summary>
        /// <param name="consent"></param>
        /// <param name="accountId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetStatement(string consent, string accountId, string taskId)
        {
            #region Parameter validation

            if (string.IsNullOrEmpty(consent))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Chýba povinný parameter consent!");
                return "Chýba povinný parameter consent!";
            }
            if (string.IsNullOrEmpty(accountId))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Chýba povinný parameter accountId!");
                return "Chýba povinný parameter accountId!";
            }
            if (string.IsNullOrEmpty(taskId))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Chýba povinný parameter taskId!");
                return "Chýba povinný parameter taskId";
            }
           
            #endregion
            try
            {
                var formHeaderData = new Dictionary<string, string>
                {
                    {"X-Request-ID", Guid.NewGuid().ToString()},
                };

                #region Get_StatementId
                string JWT = DB_Get_JWT(consent);
                if (string.IsNullOrEmpty(JWT))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Nepodarilo sa získať JWT token!");
                    return "Nepodarilo sa získať JWT token!";
                }

				string statemenRes = ApiUtils.CallApi(baseUrl + $"/v1/accounts/{accountId}/statements/tasks/{taskId}", JWT, formHeaderData);

                if (string.IsNullOrEmpty(statemenRes))
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Nepodarilo sa získať statementid!");
                    return $"Nepodarilo sa získať statementid";
                }

				dynamic jsonResult = JObject.Parse(statemenRes);

                if (jsonResult?.state != "SUCCEEDED")
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Výpis ešte nie je pripravený!");
                    return $"Výpis ešte nie je pripravený";
                }

                if (jsonResult?.statements.Count < 1)
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Nepodarilo sa získať vypis!");
                    return $"Nepodarilo sa získať vypis";
                }

                #endregion

                byte[] res = ApiUtils.CallApiAsByteArray(baseUrl + $"/v1/accounts/{accountId}/statements/{jsonResult.statements[0].statementId}", JWT, formHeaderData);

                if (res == null)
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetStatement - Nepodarilo sa získať vypis!");
                    return $"Nepodarilo sa získať vypis";
                }

                return System.Convert.ToBase64String(res);
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.GetStatement - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.GetStatement - Neošetrená chyba: {ex.Message}");
            }
        }

		/// <summary>
		/// GetTransactionsCompat
		/// </summary>
		/// <param name="consent"></param>
		/// <param name="accountId"></param>
		/// <param name="amountFrom"></param>
		/// <param name="amountTo"></param>
		/// <param name="transactionDirection"></param>
		/// <param name="transactionIdFrom"></param>
		/// <param name="bankTransactionCode"></param>
		/// <param name="variableSymbol"></param>
		/// <param name="constantSymbol"></param>
		/// <param name="pageSize"></param>
		/// <param name="dateTo"></param>
		/// <param name="dateFrom"></param>
		/// <param name="specificSymbol"></param>
		/// <param name="entryReferenceFrom"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public XmlElement GetTransactionsCompat(string consent, string accountId, string amountFrom,
            string amountTo, string transactionDirection, string transactionIdFrom, string bankTransactionCode,
            string variableSymbol, string constantSymbol, string pageSize,
            string dateTo, string dateFrom, string specificSymbol, string entryReferenceFrom)
        {
            if (string.IsNullOrEmpty(consent) || string.IsNullOrEmpty(accountId))
            {
                AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.GetTransactionsCompact - Chýbajú povinné parametre: consent alebo accountId!");
                return ApiUtils.CreateErrorXml("Chýbajú povinné parametre: consent alebo accountId!");
            }

            try 
            {
                /*
                int I_amountFrom = 0;

                if ( !string.IsNullOrEmpty(amountFrom))
                {
                    if ( !int.TryParse(amountFrom, out I_amountFrom))
                    {
                        return ApiUtils.CreateErrorXml("Chybný formát parametra amountFrom");
					}
                }
                */

				int? I_amountFrom = null;
				try
				{
					if ( !string.IsNullOrEmpty(amountFrom) ) I_amountFrom = int.Parse(amountFrom);
				}
				catch { }

				int? I_amountTo = null;
                try { 
                    if (!string.IsNullOrEmpty(amountTo)) I_amountTo = int.Parse(amountTo);
				}
				catch { } // ak sa nepodari Parse, tak beriem amountTo ako null

				int? I_transactionIdFrom = null;
                try { 
                    if (!string.IsNullOrEmpty(transactionIdFrom)) I_transactionIdFrom = int.Parse(transactionIdFrom);
				}
				catch { } // ak sa nepodari Parse, tak beriem transactionIdFrom ako null

				int? I_pageSize = null;
                try
                { 
                    if (!string.IsNullOrEmpty(pageSize)) I_pageSize = int.Parse(pageSize);
			    } catch { } // ak sa nepodari Parse, tak beriem pageSize ako null

				DateTime? D_dateTo = null;
                try
                {
                    if ( !string.IsNullOrEmpty(dateTo) ) D_dateTo = DateTime.Parse(dateTo);
                } catch { } // ak sa nepodari Parse, tak beriem D_dateTo ako null

				DateTime? D_dateFrom = null;
                try
                {
                    if ( !string.IsNullOrEmpty(dateFrom) ) D_dateFrom = DateTime.Parse(dateFrom);
                } catch { } // ak sa nepodari Parse, tak beriem D_dateFrom ako null

				return GetTransactions(consent, accountId, I_amountFrom, I_amountTo, transactionDirection, 
                    I_transactionIdFrom, bankTransactionCode, variableSymbol, constantSymbol, 
                    I_pageSize, D_dateTo, D_dateFrom, specificSymbol, entryReferenceFrom);
            }
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaService.GetTransactionsCompact - Neošetrená chyba: {ex.Message}");
                throw new Exception($"TatraBankaService.GetTransactionsCompact - Neošetrená chyba: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Api utility class for Tatra Banka API calls
    /// </summary>
    public class ApiUtils
    {
        public static XmlElement CreateErrorXml(string message)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Error");
            root.InnerText = message;
            return root;
        }

        public static string CallApi(string url, string auth, Dictionary<string, string> header = null,
            Dictionary<string, string> paramDict = null)
        {
            using (HttpClient client = new HttpClient())
            {
                //authentication
                if (!string.IsNullOrEmpty(auth))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");

                //header params
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

				var uriBuilder = new UriBuilder(url);
				if (paramDict != null )
                {
					var query = new FormUrlEncodedContent(paramDict).ReadAsStringAsync().Result;
					uriBuilder.Query = query;
				}   

                try
                {
                    HttpResponseMessage response = client.GetAsync(uriBuilder.Uri).Result;
                    string responseData = "";
                    Task.Run(async () => responseData = await response.Content.ReadAsStringAsync()).Wait();
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallApi - url: {uriBuilder.Uri}");

                    if (response.IsSuccessStatusCode)
                    {  
                        return responseData;
                    }
                    else
                    {
						AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallApi - Nepodarilo sa volanie TB API : {responseData}");
						return "";
                    }

				}
                catch (System.Exception ex)
                {
                    throw new System.Exception("Nepodarilo sa volanie REST API!", ex);
                }
            }
        }

		public static byte[] CallApiAsByteArray(string url, string auth, Dictionary<string, string> header = null,
			Dictionary<string, string> paramDict = null)
		{
			using ( HttpClient client = new HttpClient() )
			{
				//authentication
				if ( !string.IsNullOrEmpty(auth) )
					client.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");

				//header params
				if ( header != null )
				{
					foreach ( var item in header )
					{
						client.DefaultRequestHeaders.Add(item.Key, item.Value);
					}
				}

				var uriBuilder = new UriBuilder(url);
				if ( paramDict != null )
				{
					var query = new FormUrlEncodedContent(paramDict).ReadAsStringAsync().Result;
					uriBuilder.Query = query;
				}

				try
				{
					HttpResponseMessage response = client.GetAsync(uriBuilder.Uri).Result;
					byte[] responseData = null;
					Task.Run(async () => responseData = await response.Content.ReadAsByteArrayAsync()).Wait();
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallApiAsByteArray - url: {uriBuilder.Uri}");

                    if ( response.IsSuccessStatusCode )
                    { 
						return responseData;
					}
					else
					{
						AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallApiAsByteArray - Nepodarilo sa volanie TB API : {responseData}");
						return null;
					}
				}
				catch ( System.Exception ex )
				{
					throw new System.Exception("Nepodarilo sa volanie REST API!", ex);
				}
			}
		}

		public static string CallDeleteApi(string url, string auth, Dictionary<string, string> header = null)
        {
            using (HttpClient client = new HttpClient())
            {
                //authentication
                if (!string.IsNullOrEmpty(auth))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");

                //header params
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                try
                {
                    HttpResponseMessage response = client.DeleteAsync(url).Result;
                    string responseData = "";
                    Task.Run(async () => responseData = await response.Content.ReadAsStringAsync()).Wait();
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallDeleteApi - url: {url}");

                    if (response.IsSuccessStatusCode)
                    { 
                        if(string.IsNullOrEmpty(responseData))
                            responseData = "Položka bola úspešne odstránená";
                        return responseData;
                    }
                    else
                    {
                        AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallDeleteApi - Nepodarilo sa volanie TB API : {responseData}");
                        return "";
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Nepodarilo sa volanie REST API!", ex);
                }
            }
        }

        public static string CallPutApi(string url, string auth, Dictionary<string, string> header = null)
        {
            using (HttpClient client = new HttpClient())
            {
                //authentication
                if (!string.IsNullOrEmpty(auth))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");

                //header params
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                try
                {
                    HttpResponseMessage response = client.PutAsync(url,null).Result;

                    string responseData = "";
                    Task.Run(async () => responseData = await response.Content.ReadAsStringAsync()).Wait();
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallPutApi - url: {url}");

                    if (response.IsSuccessStatusCode)
                    {
                        return responseData;
                    }
                    else
                    {
                        AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallPutApi - Nepodarilo sa volanie TB API : {responseData}");
                        return "";
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception("Nepodarilo sa volanie REST API!", ex);
                }
            }
        }

        public static dynamic CallPostApi(string url, Dictionary<string, string> data = null, Dictionary<string, string> header = null, string auth = null, string jsonBody = null)
        {
            using (HttpClient client = new HttpClient())
            {
                //authentication
                if (!string.IsNullOrEmpty(auth))
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {auth}");

                //header params
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                try
                {
                    
                    if (data == null)
                        data = new Dictionary<string, string>();
                    dynamic content = new FormUrlEncodedContent(data);

                    if (jsonBody != null)
                        content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                    string responseData = "";
                    Task.Run(async () => responseData = await response.Content.ReadAsStringAsync()).Wait();
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallPostApi - url: {url}");

                    if (response.IsSuccessStatusCode)
                    {
                        return responseData;
                    }
                    else
                    {
						AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaService.CallPostApi - Nepodarilo sa volanie TB API : {responseData}");
						return "";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Nepodarilo sa volanie REST API!", ex);
                }
            }
        }
    }
}