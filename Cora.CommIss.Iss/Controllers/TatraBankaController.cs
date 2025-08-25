using System;
using System.Web.Http;
using System.Web.Http.Description;
using Cora.Utils.Logger;
using Cora.CommIss.Iss.TatraBanka;
using Cora.CommIss.Iss.Impl;
using System.Web.Http.Results;
using Cora.Natec.Base;
using Cora.Data.DBProvider;
using System.Data;
using System.Collections.Generic;
using Cora.CommIss.VerejneRegistre.Mvsr;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;


namespace Cora.CommIss.Iss.Controllers
{
    /// <summary>
	/// TatraBanka controller
	/// </summary>
	[RoutePrefix("api/tatraBanka")]
    //[ApiActivation]
    public class TatraBankaController : ApiController
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ResponseType(null)]
        public IHttpActionResult redirectUrl([FromUri] string code = null, [FromUri] string state = null,
                                             [FromUri] string error = null, [FromUri] string error_description = null)
        {
            try
            {
				string html = $@"<html>
    <head>
        <title>CG TB Premium API - autorizácia</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                margin: 40px;
                background-color: #f4f4f4;
            }}
            .box {{
                background: white;
                padding: 30px;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0,0,0,0.1);
                max-width: 600px;
                margin: auto;
                text-align: center;
            }}
            h2 {{
                color: #333;
            }}
            .row {{
                display: flex;
                padding: 10px 0;
                text-align: left;
            }}
            .label {{
                width: 100px;
                font-weight: bold;
                color: #000;
                flex-shrink: 0;
            }}
            .value {{
                color: #007ACC;
                word-break: break-word;
                flex-grow: 1;
            }}
            .logo {{
                display: block;
                margin: 0 auto 20px auto;
                max-width: 200px;
            }}
        </style>
    </head>
    <body>
        <div class='box'>
            <img src=""https://www.corageo.sk/wp-content/uploads/2017/01/logo.png"" alt=""Corageo Logo"" class=""logo"">
            <h2>Autorizácia bola úspešná</h2>
        </div>
    </body>
</html>";

				if ( (code != null && state != null))
                {
                    ITatraBankaService tatraBankaService = new TatraBankaService();

                    string JWT = tatraBankaService.TB_Get_JWT(code);
                    if ( string.IsNullOrEmpty(JWT) )
                    {
                        AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaController.redirectUrl - Nepodarilo sa získať JWT token!");
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Nepodarilo sa získať JWT token!"));
                    }

                    int mACC_COUNT = 0;

                    #region mACC_COUNT
                    var formHeaderData = new Dictionary<string, string>
                        {
                            {"X-Request-ID", Guid.NewGuid().ToString()},
                        };

					NameValueCollection TatraBankaConfig = (NameValueCollection) System.Configuration.ConfigurationManager.GetSection("TatraBankaConfig");

                    if(TatraBankaConfig == null) 
                    {
						AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaController.redirectUrl - Nepodarilo sa  získať údaje z TatraBankaConfig!");
						return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Nepodarilo sa  získať údaje z TatraBankaConfig!"));
					}

					string res = ApiUtils.CallApi(TatraBankaConfig["TB_baseUrl"] + "/v3/accounts", JWT, formHeaderData);

                    if ( string.IsNullOrEmpty(res) )
                    {
                        AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaController.redirectUrl - Nepodarilo sa získať počet účtov!");
						return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Nepodarilo sa získať počet účtov!"));

					}
					else
                    {
                        dynamic acc = JObject.Parse(res);
                        mACC_COUNT = acc.accounts.Count;
                    }
                    #endregion

                    #region DB_AUTH_UPD
                    using ( CoraConnection conn = new CoraConnection(FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>().sConn) )
                    using ( CoraCommand command = (CoraCommand) conn.CreateCommand() )
                    {
                        conn.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "TBPA_AUTH_UPDJWT";

                        command.Parameters.Add(new CoraParameter("mI_AUTH", DbType.String) { Value = state });
                        command.Parameters.Add(new CoraParameter("mJWT", DbType.String) { Value = Crypt.CryptWithSalt.EncryptString(JWT) });
                        command.Parameters.Add(new CoraParameter("mD_EXP", DbType.DateTime) { Value = DateTime.Now.AddDays( TatraBankaConfig["D_EXPIRACIE"] == null ? 180 : double.Parse(TatraBankaConfig["D_EXPIRACIE"])) });
                        command.Parameters.Add(new CoraParameter("mACC_COUNT", DbType.Int32) { Value = mACC_COUNT });

                        command.ExecuteNonQuery();
                        conn.Close();
                    }
                    #endregion

                    return new HtmlActionResult(html);
                }
                else if(error != null && error_description != null)
				{
					AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaController.redirectUrl - Chyba: {error}, popis: {error_description}");
					html = $@"<html>
    <head>
        <title>CG TB Premium API - autorizácia</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                margin: 40px;
                background-color: #f4f4f4;
            }}
            .box {{
                background: white;
                padding: 30px;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0,0,0,0.1);
                max-width: 600px;
                margin: auto;
                text-align: center;
            }}
            h2 {{
                color: #333;
            }}
            .row {{
                display: flex;
                padding: 10px 0;
                text-align: left;
            }}
            .label {{
                width: 100px;
                font-weight: bold;
                color: #000;
                flex-shrink: 0;
            }}
            .value {{
                color: #007ACC;
                word-break: break-word;
                flex-grow: 1;
            }}
            .logo {{
                display: block;
                margin: 0 auto 20px auto;
                max-width: 200px;
            }}
        </style>
    </head>
    <body>
        <div class='box'>
            <img src=""https://www.corageo.sk/wp-content/uploads/2017/01/logo.png"" alt=""Corageo Logo"" class=""logo"">
            <h2>Autorizácia zlyhala: {error}</h2>
        </div>
    </body>
</html>";
					return new HtmlActionResult(html);
				}
				else
                {
                    AppLogging.Logger.Log(LogLevel.Debug, $"TatraBankaController.redirectUrl - Zadané nesprávne parametre");
					return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Zadané nesprávne parametre"));
				}
			}
            catch (Exception ex)
            {
                AppLogging.Logger.Log(LogLevel.Error, $"TatraBankaController.redirectUrl - Neošetrená chyba: {ex}");
				return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, $"Neošetrená chyba: {ex}"));
			}
		}

    }
}