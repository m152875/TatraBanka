using System;
using System.Configuration;
using Cora.Crypt;
using Cora.Utils.Logger;
using Cora.CommIss.Iss.Dovera;
using Cora.CommIss.Iss.DoveraSystemClient;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using Cora.Convert;
using Cora.CommIss.Iss.DoveraPoistenecClient;
using Cora.CommIss.Iss.DoveraPlatitelClient;
using Cora.CommIss.Iss.DoveraHromadneOznamenieClient;
using Cora.CommIss.Iss.DoveraMesacnyVykazClient;
using System.Net;

namespace Cora.CommIss.Iss.Impl
{
	/// <summary>
	/// Sluzba poskytujuca pristup k webovym sluzbam ZP Dovera - Zamestnavatelia online
	/// </summary>
	public class DoveraProvidePrivate : IDoveraProvidePrivate
	{
		/// <summary>
		/// Ziskanie stavu sluzieb
		/// </summary>
		/// <param name="request"></param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<DajStavSystemuVystup> DajStavSystemuPrivate(DajStavSystemuVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new SystemServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					//client.ClientCredentials.UserName.UserName = DoveraClientConfig["UserName"];
					client.ClientCredentials.UserName.UserName = Login;
					//client.ClientCredentials.UserName.Password = CryptWithSalt.DecryptString(DoveraClientConfig["Password"]);
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					var response = client.DajStavSystemu(request);
					return new DoveraExtResponse<DajStavSystemuVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajStavSystemu zlyhalo. Chyba: {0}", ex.ToString()));
				return new DoveraExtResponse<DajStavSystemuVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		//public byte[] DajStavSystemuPrivateCompat(string request)
		//{
		//    StringReader sr = new StringReader(request);
		//    System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(DajStavSystemuVstup));
		//    DajStavSystemuVstup req = (DajStavSystemuVstup)ser.Deserialize(sr);

		//    DajStavSystemuVystup res = DajStavSystemuPrivate(req);
		//    XmlElement xmlRet = res.Serialize();
		//    return Cora.Convert.Xml.GetBytes(xmlRet);
		//}
		/// <summary>
		/// Dajs the stav systemu private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <returns></returns>
		public byte[] DajStavSystemuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA)
		{
			DajStavSystemuVstup request = new DajStavSystemuVstup
			{
				TokenPouzivatela = new DoveraSystemClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				}
			};
			var res = DajStavSystemuPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		public DoveraExtResponse<DajPrihlasovaciTokenVystup> DajPrihlasovaciTokenPrivate(DajPrihlasovaciTokenVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new SystemServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
//					return client.DajPrihlasovaciToken(request);
					var response = client.DajPrihlasovaciToken(request);
					return new DoveraExtResponse<DajPrihlasovaciTokenVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajPrihlasovaciToken zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajPrihlasovaciTokenVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}

		public byte[] DajPrihlasovaciTokenPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA)
		{
			DajPrihlasovaciTokenVstup request = new DajPrihlasovaciTokenVstup
			{
				TokenPouzivatela = new DoveraSystemClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				}
			};
			var res = DajPrihlasovaciTokenPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		public DoveraExtResponse<PrihlasenieTokenomVystup> PrihlasenieTokenomPrivate(PrihlasenieTokenomVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new SystemServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.PrihlasenieTokenom(request);
					var response = client.PrihlasenieTokenom(request);
					return new DoveraExtResponse<PrihlasenieTokenomVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby PrihlasenieTokenom zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<PrihlasenieTokenomVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}

		public byte[] PrihlasenieTokenomPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string PrihlasovaciToken, string StrankaPoPrihlaseni, string FFilter)
		{
			PrihlasenieTokenomVstup request = new PrihlasenieTokenomVstup
			{
				TokenPouzivatela = new DoveraSystemClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				PrihlasovaciToken = PrihlasovaciToken,
				StrankaPoPrihlaseni = StrankaPoPrihlaseni,
//				Filter = FFilter
			};

			var res = PrihlasenieTokenomPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		public DoveraExtResponse<DajOznamyDZPVystup> DajOznamyDZPPrivate(DajOznamyDZPVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new SystemServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.DajOznamyDZP(request);
					var response = client.DajOznamyDZP(request);
					return new DoveraExtResponse<DajOznamyDZPVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajOznamyDZP zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajOznamyDZPVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}

		public byte[] DajOznamyDZPPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int Forma)
		{
			DajOznamyDZPVstup request = new DajOznamyDZPVstup
			{
				TokenPouzivatela = new DoveraSystemClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				}
			};

			switch ( Forma )
			{
				case 0:
					request.FormaVystup = APIFormaVystup.API;
					break;
				case 1:
					request.FormaVystup = APIFormaVystup.PDF;
					break;
				default:
					break;
			}

			var res = DajOznamyDZPPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}


		public DoveraExtResponse<OverPoistencaVystup> OverPoistencaPrivate(OverPoistencaVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new PoistenecServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.OverPoistenca(request);
					var response = client.OverPoistenca(request);
					return new DoveraExtResponse<OverPoistencaVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby OverPoistenca zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<OverPoistencaVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Overs the poistenca private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="RC">The rc.</param>
		/// <param name="ICP">The icp.</param>
		/// <param name="Datum">The datum.</param>
		/// <returns></returns>
		public byte[] OverPoistencaPrivateCompat(string Login, string Password, string RC, string ICP, string Datum)
		{
			OverPoistencaVstup request = new OverPoistencaVstup
			{
				TokenPouzivatela = new DoveraPoistenecClient.APITokenPouzivatela
				{
					Login = Login
				},
				RC = RC,
				ICP = ICP,
				Datum = Datum
			};
			var res = OverPoistencaPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Overs the oop private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<OverOOPVystup> OverOOPPrivate(OverOOPVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new PlatitelSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.OverOOP(request);
					var response = client.OverOOP(request);
					return new DoveraExtResponse<OverOOPVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby OverOOP zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<OverOOPVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="RC"></param>
		/// <param name="ICP"></param>
		/// <param name="RozhodnyDen"></param>
		/// <returns></returns>
		//public byte[] OverOOPPrivateCompat(string request)
		//      {
		//          StringReader sr = new StringReader(request);
		//          System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(OverOOPVstup));
		//          OverOOPVstup req = (OverOOPVstup)ser.Deserialize(sr);

		//          OverOOPVystup res = OverOOPPrivate(req);
		//          XmlElement xmlRet = res.Serialize();
		//          return Cora.Convert.Xml.GetBytes(xmlRet);
		//      }
		public byte[] OverOOPPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string RC, string ICP, string RozhodnyDen)
		{

			OverOOPVstup request = new OverOOPVstup
			{

				TokenPouzivatela = new DoveraPlatitelClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				RC = RC,
				ICP = ICP,
				RozhodnyDen = DateTime.ParseExact(RozhodnyDen, "yyyyMMdd", null),
				RozhodnyDenSpecified = !string.IsNullOrWhiteSpace(RozhodnyDen)
			};

			var res = OverOOPPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Dajs the stav uctu private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		public DoveraExtResponse<DajStavUctuVystup> DajStavUctuPrivate(DajStavUctuVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new PlatitelSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.DajStavUctu(request);
					var response = client.DajStavUctu(request);
					return new DoveraExtResponse<DajStavUctuVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajStavUctu zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajStavUctuVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Dajs the stav uctu private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <returns></returns>
		public byte[] DajStavUctuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA)
		{
			DajStavUctuVstup request = new DajStavUctuVstup
			{
				TokenPouzivatela = new DoveraPlatitelClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				}
			};
			var res = DajStavUctuPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Otestujs the hromadne oznamenie private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<OtestujHromadneOznamenieVystup> OtestujHromadneOznameniePrivate(OtestujHromadneOznamenieVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new HromadneOznamenieSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.OtestujHromadneOznamenie(request);
					var response = client.OtestujHromadneOznamenie(request);
					return new DoveraExtResponse<OtestujHromadneOznamenieVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby OtestujHromadneOznamenie zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<OtestujHromadneOznamenieVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		//public byte[] OtestujHromadneOznameniePrivateCompat(string request)
		//{
		//    StringReader sr = new StringReader(request);
		//    System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(OtestujHromadneOznamenieVstup));
		//    OtestujHromadneOznamenieVstup req = (OtestujHromadneOznamenieVstup)ser.Deserialize(sr);

		//    OtestujHromadneOznamenieVystup res = OtestujHromadneOznameniePrivate(req);
		//    XmlElement xmlRet = res.Serialize();
		//    return Cora.Convert.Xml.GetBytes(xmlRet);
		//}				
		/// <summary>
		/// Otestujs the hromadne oznamenie private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetRiadkov">The pocet riadkov.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		public byte[] OtestujHromadneOznameniePrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string Typ, int Obdobie, int PocetRiadkov, int IdExterne, string Nazov, string Obsah)
		{
			OtestujHromadneOznamenieVstup request = new OtestujHromadneOznamenieVstup
			{
				TokenPouzivatela = new DoveraHromadneOznamenieClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA,
				},
				Typ = Typ,
				Obdobie = Obdobie,
				PocetRiadkov = PocetRiadkov,
				HromadneOznamenie = new DoveraHromadneOznamenieClient.APIDavka
				{
					IdExterne = IdExterne,
					Nazov = Nazov,
					Obsah = System.Convert.FromBase64String(Obsah)
				},
			};

			var res = OtestujHromadneOznameniePrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Dajs the stav otestovania hromadneho oznamenia private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<DajStavOtestovaniaHromadnehoOznameniaVystup> DajStavOtestovaniaHromadnehoOznameniaPrivate(DajStavOtestovaniaHromadnehoOznameniaVstup request, string Login, string Password)

		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new HromadneOznamenieSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.DajStavOtestovaniaHromadnehoOznamenia(request);
					var response = client.DajStavOtestovaniaHromadnehoOznamenia(request);
					return new DoveraExtResponse<DajStavOtestovaniaHromadnehoOznameniaVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajStavOtestovaniaHromadnehoOznamenia zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajStavOtestovaniaHromadnehoOznameniaVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Dajs the stav otestovania hromadneho oznamenia private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		public byte[] DajStavOtestovaniaHromadnehoOznameniaPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania)
		{
			DajStavOtestovaniaHromadnehoOznameniaVstup request = new DajStavOtestovaniaHromadnehoOznameniaVstup
			{
				TokenPouzivatela = new DoveraHromadneOznamenieClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				IdVolania = IDVolania
			};
			var res = DajStavOtestovaniaHromadnehoOznameniaPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Dajs the stav odoslania hromadneho oznamenia private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<DajStavOdoslaniaHromadnehoOznameniaVystup> DajStavOdoslaniaHromadnehoOznameniaPrivate(DajStavOdoslaniaHromadnehoOznameniaVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new HromadneOznamenieSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.DajStavOdoslaniaHromadnehoOznamenia(request);
					var response = client.DajStavOdoslaniaHromadnehoOznamenia(request);
					return new DoveraExtResponse<DajStavOdoslaniaHromadnehoOznameniaVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajStavOdoslaniaHromadnehoOznamenia zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajStavOdoslaniaHromadnehoOznameniaVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Dajs the stav odoslania hromadneho oznamenia private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		public byte[] DajStavOdoslaniaHromadnehoOznameniaPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania)
		{
			DajStavOdoslaniaHromadnehoOznameniaVstup request = new DajStavOdoslaniaHromadnehoOznameniaVstup
			{
				TokenPouzivatela = new DoveraHromadneOznamenieClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				IdVolania = IDVolania
			};

			var res = DajStavOdoslaniaHromadnehoOznameniaPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Odoslis the hromadne oznamenie private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<OdosliHromadneOznamenieVystup> OdosliHromadneOznameniePrivate(OdosliHromadneOznamenieVstup request, string Login, string Password)

		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new HromadneOznamenieSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.OdosliHromadneOznamenie(request);
					var response = client.OdosliHromadneOznamenie(request);
					return new DoveraExtResponse<OdosliHromadneOznamenieVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby OdosliHromadneOznamenie zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<OdosliHromadneOznamenieVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Odoslis the hromadne oznamenie private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetRiadkov">The pocet riadkov.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		public byte[] OdosliHromadneOznameniePrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string Typ, int Obdobie, int PocetRiadkov, int IdExterne, string Nazov, string Obsah)
		{
			OdosliHromadneOznamenieVstup request = new OdosliHromadneOznamenieVstup
			{
				TokenPouzivatela = new DoveraHromadneOznamenieClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				Typ = Typ,
				Obdobie = Obdobie,
				PocetRiadkov = PocetRiadkov,
				HromadneOznamenie = new DoveraHromadneOznamenieClient.APIDavka
				{
					IdExterne = IdExterne,
					Nazov = Nazov,
					Obsah = System.Convert.FromBase64String(Obsah)
				},
			};

			var res = OdosliHromadneOznameniePrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Odoslis the mesacny vykaz private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<OdosliMesacnyVykazVystup> OdosliMesacnyVykazPrivate(OdosliMesacnyVykazVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new MesacnyVykazSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.OdosliMesacnyVykaz(request);
					var response = client.OdosliMesacnyVykaz(request);
					return new DoveraExtResponse<OdosliMesacnyVykazVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby OdosliMesacnyVykaz zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<OdosliMesacnyVykazVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		//public byte[] OdosliMesacnyVykazPrivateCompat(string request)
		//{
		//    StringReader sr = new StringReader(request);
		//    System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(OdosliMesacnyVykazVstup));
		//    OdosliMesacnyVykazVstup req = (OdosliMesacnyVykazVstup)ser.Deserialize(sr);

		//    OdosliMesacnyVykazVystup res = OdosliMesacnyVykazPrivate(req);
		//    XmlElement xmlRet = res.Serialize();
		//    return Cora.Convert.Xml.GetBytes(xmlRet);
		//}				
		/// <summary>
		/// Odoslis the mesacny vykaz private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="SumaPreddavkov">The suma preddavkov.</param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetViet">The pocet viet.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		public byte[] OdosliMesacnyVykazPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, decimal SumaPreddavkov, string Typ, int Obdobie, int PocetViet, int IdExterne, string Nazov, string Obsah)
		{
			OdosliMesacnyVykazVstup request = new OdosliMesacnyVykazVstup
			{
				TokenPouzivatela = new DoveraMesacnyVykazClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				SumaPreddavkov = SumaPreddavkov,
				Typ = Typ,
				Obdobie = Obdobie,
				PocetViet = PocetViet,
				MesacnyVykaz = new DoveraMesacnyVykazClient.APIDavka
				{
					IdExterne = IdExterne,
					Nazov = Nazov,
					Obsah = System.Convert.FromBase64String(Obsah)
				},

			};

			var res = OdosliMesacnyVykazPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Dajs the stav odoslania mesacneho vykazu private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<DajStavOdoslaniaMesacnehoVykazuVystup> DajStavOdoslaniaMesacnehoVykazuPrivate(DajStavOdoslaniaMesacnehoVykazuVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new MesacnyVykazSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.DajStavOdoslaniaMesacnehoVykazu(request);
					var response = client.DajStavOdoslaniaMesacnehoVykazu(request);
					return new DoveraExtResponse<DajStavOdoslaniaMesacnehoVykazuVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajStavOdoslaniaMesacnehoVykazu zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajStavOdoslaniaMesacnehoVykazuVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Dajs the stav odoslania mesacneho vykazu private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		public byte[] DajStavOdoslaniaMesacnehoVykazuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania)
		{ 
			DajStavOdoslaniaMesacnehoVykazuVstup request = new DajStavOdoslaniaMesacnehoVykazuVstup
			{
				TokenPouzivatela = new DoveraMesacnyVykazClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				IdVolania = IDVolania
			};

			var res = DajStavOdoslaniaMesacnehoVykazuPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Dajs the stav otestovania mesacneho vykazu private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<DajStavOtestovaniaMesacnehoVykazuVystup> DajStavOtestovaniaMesacnehoVykazuPrivate(DajStavOtestovaniaMesacnehoVykazuVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new MesacnyVykazSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.DajStavOtestovaniaMesacnehoVykazu(request);
					var response = client.DajStavOtestovaniaMesacnehoVykazu(request);
					return new DoveraExtResponse<DajStavOtestovaniaMesacnehoVykazuVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajStavOtestovaniaMesacnehoVykazu zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajStavOtestovaniaMesacnehoVykazuVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Dajs the stav otestovania mesacneho vykazu private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		public byte[] DajStavOtestovaniaMesacnehoVykazuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania)
		{
			DajStavOtestovaniaMesacnehoVykazuVstup request = new DajStavOtestovaniaMesacnehoVykazuVstup
			{
				TokenPouzivatela = new DoveraMesacnyVykazClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				IdVolania = IDVolania
			};

			var res = DajStavOtestovaniaMesacnehoVykazuPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Otestujs the mesacny vykaz private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password"></param>
		/// <returns></returns>
		public DoveraExtResponse<OtestujMesacnyVykazVystup> OtestujMesacnyVykazPrivate(OtestujMesacnyVykazVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new MesacnyVykazSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					client.ClientCredentials.UserName.Password = Password;
					//					return client.OtestujMesacnyVykaz(request);
					var response = client.OtestujMesacnyVykaz(request);
					return new DoveraExtResponse<OtestujMesacnyVykazVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby OtestujMesacnyVykaz zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<OtestujMesacnyVykazVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}

		//    OtestujMesacnyVykazVystup res = OtestujMesacnyVykazPrivate(req);
		//    XmlElement xmlRet = res.Serialize();
		//    return Cora.Convert.Xml.GetBytes(xmlRet);
		//}				
		/// <summary>
		/// Otestujs the mesacny vykaz private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password"></param>
		/// <param name="VS"></param>
		/// <param name="UnikatnyKodPLA"></param>
		/// <param name="SumaPreddavkov">The suma preddavkov.</param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetViet">The pocet viet.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		public byte[] OtestujMesacnyVykazPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, decimal SumaPreddavkov, string Typ, int Obdobie, int PocetViet, int IdExterne, string Nazov, string Obsah)
		{
			OtestujMesacnyVykazVstup request = new OtestujMesacnyVykazVstup
			{
				TokenPouzivatela = new DoveraMesacnyVykazClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				SumaPreddavkov = SumaPreddavkov,
				Typ = Typ,
				Obdobie = Obdobie,
				PocetViet = PocetViet,
				MesacnyVykaz = new DoveraMesacnyVykazClient.APIDavka
				{
					IdExterne = IdExterne,
					Nazov = Nazov,
					Obsah = System.Convert.FromBase64String(Obsah)
				},
			};
			var res = OtestujMesacnyVykazPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}

		/// <summary>
		/// Dajs the zoznam zamestnancov private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login"></param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		public DoveraExtResponse<DajZoznamZamestnancovVystup> DajZoznamZamestnancovPrivate(DajZoznamZamestnancovVstup request, string Login, string Password)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			try
			{
				using ( var client = new PlatitelSoapServiceSoapClient() )
				{
					NameValueCollection DoveraClientConfig = (NameValueCollection) ConfigurationManager.GetSection("DoveraClientConfig");
					client.ClientCredentials.UserName.UserName = Login;
					client.ClientCredentials.UserName.Password = Password;
					request.TokenPouzivatela.DodavatelSW = DoveraClientConfig["DodavatelSW"];
					//					return client.DajZoznamZamestnancov(request);
					var response = client.DajZoznamZamestnancov(request);
					return new DoveraExtResponse<DajZoznamZamestnancovVystup>
					{
						Response = response,
						Message = "OK"
					};
				}
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, string.Format("Volanie sluzby DajZoznamZamestnancov zlyhalo. Chyba: {0}", ex.ToString()));
				//				return null;
				return new DoveraExtResponse<DajZoznamZamestnancovVystup>
				{
					Message = ex.Message,
					Result = ex.HResult
				};
			}
		}
		/// <summary>
		/// Dajs the zoznam zamestnancov private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="DatumOd">The datum od.</param>
		/// <param name="DatumDo">The datum do.</param>
		/// <param name="IdKategorieOd">The identifier kategorie od.</param>
		/// <param name="RodneCisla">The rodne cisla.</param>
		/// <returns></returns>
		public byte[] DajZoznamZamestnancovPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string DatumOd, string DatumDo, int IdKategorieOd, string RodneCisla)
		{
			DajZoznamZamestnancovVstup request = new DajZoznamZamestnancovVstup
			{
				TokenPouzivatela = new DoveraPlatitelClient.APITokenPouzivatela
				{
					Login = Login,
					VS = VS,
					UnikatnyKodPLA = UnikatnyKodPLA
				},
				DatumOd = DateTime.ParseExact(DatumOd, "yyyyMMdd", null),
				DatumDo = DateTime.ParseExact(DatumDo, "yyyyMMdd", null),
				IdKategorieOd = IdKategorieOd,
				RodneCisla = RodneCisla.Split(',')
			};

			if ( request.RodneCisla.Length == 1 )
			{
				if (string.IsNullOrWhiteSpace(request.RodneCisla[0]))
				{
					request.RodneCisla = null;
				}
			}
			var res = DajZoznamZamestnancovPrivate(request, Login, Password);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}
	}
}
