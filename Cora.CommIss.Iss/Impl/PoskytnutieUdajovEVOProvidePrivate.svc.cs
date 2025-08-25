using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cora.CommIss.Iss.EVOClient;
using Cora.CommIss.Iss.EVO;
using System.Collections.Specialized;
using Cora.Convert;
using System.Xml.Serialization;
using System.Xml;
using Cora.Data.DBProvider;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.IO;

namespace Cora.CommIss.Iss.Impl
{
	/// <summary>
	/// Sluzba poskytujuca udaje o vozidle a jeho drzitelovi z EVO
	/// </summary>
	public class PoskytnutieUdajovEVOProvidePrivate : IPoskytnutieUdajovEVO
	{
		/// <summary>
		/// Vrati udaje o vozidle a jeho drzitelovi
		/// </summary>
		/// <param name="request">Request s udajmi vozidla (povinne ECV alebo VIN) - objekt</param>
		/// <param name="zdroj">1 pri ISS, 2 pri ostatnych (MAMP...)</param>
		/// <returns>Udaje o vozidle a jeho drzitelovi</returns>
		public EVOResponse PoskytnutieUdajovEVO(EVO.VozidloRequest request, int zdroj = 2)
		{
			DateTime datePrijatia = DateTime.Now;
			Global.TempDirectoryHelper tempRecorder = null;
			if ( Cora.Utils.Logger.AppLogging.Logger.IsLoggable(Utils.Logger.LogLevel.Debug) )
			{
				tempRecorder = new Cora.Global.TempDirectoryHelper("EVo");
				string filepath = tempRecorder.Gener_FilePath("Request", "xml", datePrijatia);
				XmlElement xmlElem = Cora.Convert.XmlSerializer.Serialize(request);
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml($"<PoskytnutieUdajovEVO_Request/>");
				xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlElem, true));
				XmlElement newChild = xmlDoc.CreateElement("zdroj");
				newChild.InnerText = zdroj.ToString();
				xmlDoc.DocumentElement.AppendChild(newChild);
				Cora.Convert.Xml.WriteToFile(xmlDoc.DocumentElement, filepath, XmlHelper.Create_XmlWriterSettings(true, true, false));
			}

			EVOResponse res = new EVOResponse();
			res.I_fe_reg_log = "-1";
			res.I_voz = "-1";
			res.I_drz = "-1";

			if ( request == null )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error, string.Format("PoskytnutieUdajovEVOProviderPrivate: request nemoze byt prazdny."));
				res.Msg = new EVO.Message()
				{
					Code = "cora003",
					Text = "Požiadavka nesmie byť prázdna"
				};

				if ( null != tempRecorder )
				{
					string filepath = tempRecorder.Gener_FilePath("Response", "xml", datePrijatia);
					Cora.Convert.Xml.WriteToFile(Cora.Convert.XmlSerializer.Serialize(res), filepath, XmlHelper.Create_XmlWriterSettings(true, true, false));
				}
				return res;
			}

			try
			{
				poskytnutieUdajovOvozidleAdrziteloviResponse resClient = GetData(request);
				res = ResponseMapper.ToServiceResponse(resClient);
				DateTime dateOdoslania = DateTime.Now;
				res.I_fe_reg_log = LogEvo(request, resClient, zdroj, datePrijatia, dateOdoslania);

				if ( Int32.Parse(res.I_fe_reg_log) > 0 )
					res.I_voz = AddMpEvVoz(res);

				if ( Int32.Parse(res.I_voz) > 0 )
					res.I_drz = AddMpEvDrz(res);

			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error, string.Format("Chyba pri pokuse o spracovanie poziadavky/odpovede z centralneho registra: {0}", ex.ToString()));
				res.Msg = new Message
				{
					Code = "cora004",
					Text = "Chyba pri pokuse o spracovanie požiadavky/odpovede z centrálneho registra"
				};
			}
			if ( null != tempRecorder )
			{
				string filepath = tempRecorder.Gener_FilePath("Response", "xml", datePrijatia);
				Cora.Convert.Xml.WriteToFile(Cora.Convert.XmlSerializer.Serialize(res), filepath, XmlHelper.Create_XmlWriterSettings(true, true, false));
			}
			return res;
		}

		/// <summary>
		/// Vrati udaje o vozidle a jeho drzitelovi v binarnom tvare
		/// </summary>
		/// <param name="XmlVozidloRequest">Request s udajmi vozidla (povinne ECV alebo VIN) - xml string</param>
		/// <returns>Udaje o vozidle a jeho drzitelovi</returns>
		public byte[] PoskytnutieUdajovEVOCompat(string XmlVozidloRequest)
		{
			StringReader sr = new StringReader(XmlVozidloRequest);
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(EVO.VozidloRequest));
			EVO.VozidloRequest req = (EVO.VozidloRequest) ser.Deserialize(sr);

			EVOResponse res = PoskytnutieUdajovEVO(req, 1);
			XmlElement xmlRet = res.Serialize();
			return Cora.Convert.Xml.GetBytes(xmlRet);
		}


		/// <summary>
		/// Získa dáta o vozidle z centrálneho registra
		/// </summary>
		/// <param name="request">Request s udajmi vozidla (povinne ECV alebo VIN)</param>
		/// <returns>Udaje o vozidle a jeho drzitelovi</returns>
		private poskytnutieUdajovOvozidleAdrziteloviResponse GetData(EVO.VozidloRequest request)
		{
			EVOClient.vozidloRequest vozidloReq = RequestMapper.ToClientRequest(request);
			poskytnutieUdajovOvozidleAdrziteloviResponse resClient = new poskytnutieUdajovOvozidleAdrziteloviResponse();

			using ( PoskytnutieUdajovOvozidleAdrziteloviClient client = new PoskytnutieUdajovOvozidleAdrziteloviClient() )
			{
				NameValueCollection EvoClientConfig = (NameValueCollection) System.Configuration.ConfigurationManager.GetSection("EVOClientConfig");
				client.ClientCredentials.UserName.UserName = EvoClientConfig["UserName"];
				client.ClientCredentials.UserName.Password = Cora.Crypt.CryptWithSalt.DecryptString(EvoClientConfig["Password"]);
				vozidloReq.ep = EvoClientConfig["ExternalUser"];
				try
				{
					resClient = client.wsPoskytnutieUdajovOvozidleAdrzitelovi(vozidloReq);
				}
				catch ( Exception ex )
				{
					Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
						string.Format("Nepodarilo sa ziskat data z centralneho registra vozidiel: {0}", ex.ToString()));
					resClient.msg = new message
					{
						code = "cora001",
						text = "Nepodarilo sa získať dáta z centrálneho registra vozidiel"
					};
				}
			}



			if ( resClient == null )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
				string.Format("Response z centralneho registra je prazdny!!"));
				resClient = new poskytnutieUdajovOvozidleAdrziteloviResponse
				{
					msg = new message
					{
						code = "cora002",
						text = "Odpoveď z centrálneho registra je prázdna"
					}
				};
			}


			//DEMO - len pre ucely vyvoja !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//poskytnutieUdajovOvozidleAdrziteloviResponse res1 = new poskytnutieUdajovOvozidleAdrziteloviResponse()
			//{
			//	msg = new EVOClient.message()
			//	{
			//		code = "eGOV100",
			//		text = "Úspešné vykonanie"
			//	},
			//	vozidlo = new EVOClient.vozidlo()
			//	{
			//		BuduciDrzitel = new EVOClient.buduciDrzitel()
			//		{
			//			BuduciDrzitelDatumNarodenia = new DateTime(1993, 6, 8),
			//			BuduciDrzitelICO = "123456789",
			//			BuduciDrzitelMeno = "Ján",
			//			BuduciDrzitelNazov = "Firma SRO",
			//			BuduciDrzitelPriezvisko = "Veselý",
			//			BuduciDrzitelRodneCislo = "123456/4569",
			//			BuduciDrzitelPobyt = new EVOClient.pobytSidloBuducehoDrzitela()
			//			{
			//				BuduciDrzitelAdresaMimoSR = "Česká republika",
			//				BuduciDrzitelPobytObec = "Praha",
			//				BuduciDrzitelPobytOkres = "Praha 1",
			//				BuduciDrzitelPobytOrientacneCislo = "25",
			//				BuduciDrzitelPobytSupisneCislo = "1236",
			//				BuduciDrzitelPobytTypPobytu = "Trvalý",
			//				BuduciDrzitelPobytUlica = "Zlínska",
			//				BuduciDrzitelStatAdresyMimoSR = "Česká republika"
			//			}
			//		},
			//		Drzitel = new EVOClient.drzitel()
			//		{
			//			DrzitelDatumNarodenia = new DateTime(1994, 2, 6),
			//			DrzitelICO = "1236547879",
			//			DrzitelMeno = "Tomáš",
			//			DrzitelNazov = "Firma Conn a.s.",
			//			DrzitelPriezvisko = "Spišák",
			//			DrzitelRodneCislo = "987654/1236",
			//			DrzitelPobyt = new EVOClient.pobytSidloDrzitela()
			//			{
			//				DrzitelPobytObec = "Levoča",
			//				DrzitelPobytOkres = "Levoča",
			//				DrzitelPobytOrientacneCislo = "32",
			//				DrzitelPobytSupisneCislo = "1111",
			//				DrzitelPobytTypPobytu = "trvalý",
			//				DrzitelPobytUlica = "Potočná"
			//			}
			//		},
			//		DatumZmeny = new DateTime(2018, 3, 11),
			//		DruhVozidla = "osobné",
			//		EvidencneCislo = "LE992AD",
			//		Farba = "zelená",
			//		KategoriaVozidla = "M1",
			//		StavVozidla = "V evidencii",
			//		VIN = "TMBJB6Y1232456",
			//		Znacka = "škoda"
			//	}
			//};
			//poskytnutieUdajovOvozidleAdrziteloviResponse res2 = new poskytnutieUdajovOvozidleAdrziteloviResponse()
			//{
			//	msg = new EVOClient.message()
			//	{
			//		code = "eGOV100",
			//		text = "Úspešné vykonanie"
			//	},
			//	vozidlo = new EVOClient.vozidlo()
			//	{
			//		BuduciDrzitel = new EVOClient.buduciDrzitel()
			//		{
			//			BuduciDrzitelDatumNarodenia = new DateTime(1993, 6, 8),
			//			BuduciDrzitelICO = "123456789",
			//			BuduciDrzitelMeno = "Alojz",
			//			BuduciDrzitelNazov = "Firma S.R.O",
			//			BuduciDrzitelPriezvisko = "Kráľ",
			//			BuduciDrzitelRodneCislo = "123456/4569",
			//			BuduciDrzitelPobyt = new EVOClient.pobytSidloBuducehoDrzitela()
			//			{
			//				BuduciDrzitelAdresaMimoSR = "Nemecko",
			//				BuduciDrzitelPobytObec = "Bonn",
			//				BuduciDrzitelPobytOkres = "Nonn",
			//				BuduciDrzitelPobytOrientacneCislo = "25",
			//				BuduciDrzitelPobytSupisneCislo = "1236",
			//				BuduciDrzitelPobytTypPobytu = "Trvalý",
			//				BuduciDrzitelPobytUlica = "Hauptstrasse",
			//				BuduciDrzitelStatAdresyMimoSR = "Nemecko"
			//			}
			//		},
			//		Drzitel = new EVOClient.drzitel()
			//		{
			//			DrzitelDatumNarodenia = new DateTime(1994, 2, 6),
			//			DrzitelICO = "1236547879",
			//			DrzitelMeno = "Peter",
			//			DrzitelNazov = "Firma a.s.",
			//			DrzitelPriezvisko = "Petrovič",
			//			DrzitelRodneCislo = "987654/1236",
			//			DrzitelPobyt = new EVOClient.pobytSidloDrzitela()
			//			{
			//				DrzitelPobytObec = "Košice",
			//				DrzitelPobytOkres = "Košice",
			//				DrzitelPobytOrientacneCislo = "32",
			//				DrzitelPobytSupisneCislo = "1111",
			//				DrzitelPobytTypPobytu = "trvalý",
			//				DrzitelPobytUlica = "Kováčska"
			//			}
			//		},
			//		DatumZmeny = new DateTime(2018, 3, 11),
			//		DruhVozidla = "osobné",
			//		EvidencneCislo = "LE993AD",
			//		Farba = "strieborná",
			//		KategoriaVozidla = "N1",
			//		StavVozidla = "V evidencii",
			//		VIN = "GYDF8G1D232456",
			//		Znacka = "seat"
			//	}
			//};
			//poskytnutieUdajovOvozidleAdrziteloviResponse res3 = new poskytnutieUdajovOvozidleAdrziteloviResponse()
			//{
			//	msg = new EVOClient.message()
			//	{
			//		code = "eGOV100",
			//		text = "Úspešné vykonanie"
			//	},
			//	vozidlo = new EVOClient.vozidlo()
			//	{
			//		BuduciDrzitel = new EVOClient.buduciDrzitel(),
			//		Drzitel = new EVOClient.drzitel()
			//		{
			//			DrzitelDatumNarodenia = new DateTime(1994, 2, 6),
			//			DrzitelICO = "1236547879",
			//			DrzitelMeno = "Michal",
			//			DrzitelNazov = "Firma DFG",
			//			DrzitelPriezvisko = "Michalovič",
			//			DrzitelRodneCislo = "987654/1236",
			//			DrzitelPobyt = new EVOClient.pobytSidloDrzitela()
			//			{
			//				DrzitelPobytObec = "Bratislava",
			//				DrzitelPobytOkres = "Bratislava",
			//				DrzitelPobytOrientacneCislo = "32",
			//				DrzitelPobytSupisneCislo = "1111",
			//				DrzitelPobytTypPobytu = "trvalý",
			//				DrzitelPobytUlica = "Potočná"
			//			}
			//		},
			//		DatumZmeny = new DateTime(2018, 3, 11),
			//		DruhVozidla = "nákladné",
			//		EvidencneCislo = "LE994AD",
			//		Farba = "červená",
			//		KategoriaVozidla = "N1",
			//		StavVozidla = "V evidencii",
			//		VIN = "TMSD123A2456",
			//		Znacka = "iveco"
			//	}
			//};
			//if ( request.EvidencneCislo == "LE992AD" )
			//{
			//	resClient = res1;
			//}
			//if ( request.EvidencneCislo == "LE993AD" )
			//{
			//	resClient = res2;
			//}
			//if ( request.EvidencneCislo == "LE994AD" )
			//{
			//	resClient = res3;
			//}

			//if ( resClient == null )
			//{
			//	Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
			//	string.Format("Response z centralneho registra je prazdny!!"));
			//	resClient = new poskytnutieUdajovOvozidleAdrziteloviResponse
			//	{
			//		msg = new message
			//		{
			//			code = "cora002",
			//			text = "Odpoveď z centrálneho registra je prázdna"
			//		}
			//	};
			//}

			return resClient;
		}

		/// <summary>
		/// Zapise lustraciu vozidla do fe_reg_log
		/// </summary>
		/// <param name="request">request s udajmi vozidla</param>
		/// <param name="resClient">udaje o vozidle a jeho drzitelovi</param>
		/// <param name="zdroj">1 pri ISS, 2 pri ostatnych (MAMP...)</param>
		/// <param name="datePrijatia"></param>
		/// <param name="dateOdoslania"></param>
		/// <returns>ID zapisu v tabulke fe_reg_log</returns>
		private string LogEvo(EVO.VozidloRequest request, poskytnutieUdajovOvozidleAdrziteloviResponse resClient, int zdroj, DateTime datePrijatia, DateTime dateOdoslania)
		{
			try
			{
				string tnRet = "";
				XmlElement requestXml = request.Serialize();
				XmlElement resClientXml = resClient.Serialize();

				Natec.Base.ObjFactory<CdoZPO_MAILING_STATE> factory = (Natec.Base.ObjFactory<CdoZPO_MAILING_STATE>) Natec.Base.FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>();
				using ( CoraConnection conn = factory.CreateAndOpenConnection() )
				{
					using ( IDbCommand cmd = conn.CreateCommand("N_NM_PCK_COMM_FE_REG_LOG.pr_comm_fe_reg_log__add") )
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new CoraParameter("pnI_C_REG_TYP", 4, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("pnI_UZ", System.Convert.ToInt32(request.Id_uzivatela), DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("pnI_METHOD", 1, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("pcVSTUP_XML", requestXml.OuterXml, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pdD_ODOS", dateOdoslania, DbType.Date));
						cmd.Parameters.Add(new CoraParameter("pnI_ZDROJ", zdroj, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("pnL_ZAPIS_BIN", 1, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("tnRet", DbType.Int32) { Direction = ParameterDirection.Output });
						cmd.Parameters.Add(new CoraParameter("pcVYSTUP_XML", resClientXml.OuterXml, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcID_ERR", resClient?.msg?.code ?? "", DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcTXT_ERR", resClient?.msg?.text ?? "", DbType.String));
						cmd.Parameters.Add(new CoraParameter("pdD_PRIJ", datePrijatia, DbType.Date));
						cmd.Parameters.Add(new CoraParameter("pcDOV_LUSTR", request.DovodLustracie, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcHODN_DAT", Xml.GetBytes(resClientXml), DbType.Binary));
						cmd.Parameters.Add(new CoraParameter("pnL_PACK", 0, DbType.Int32));

						cmd.ExecuteNonQuery();
						CoraParameter ret = (CoraParameter) cmd.Parameters["tnRet"];
						tnRet = ret.Value.ToString();
					}
					conn.Close();
				}

				if ( string.IsNullOrEmpty(tnRet) )
					tnRet = "-1";

				return tnRet;
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
				string.Format("Nepodarilo sa zalogovat lustraciu EVO: {0}", ex.ToString()));
			}
			return "-1";
		}

		/// <summary>
		/// Zapise lustraciu vozidla do mp_ev_voz
		/// </summary>
		/// <param name="responseEVO">udaje z EVO</param>
		/// <returns>ID zapisu v tabulke mp_ev_voz</returns>
		private string AddMpEvVoz(EVOResponse responseEVO)
		{
			if ( responseEVO == null )
				return "-1";

			try
			{
				string tnRet = "";

				Natec.Base.ObjFactory<CdoZPO_MAILING_STATE> factory = (Natec.Base.ObjFactory<CdoZPO_MAILING_STATE>) Natec.Base.FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>();
				using ( CoraConnection conn = factory.CreateAndOpenConnection() )
				{
					using ( IDbCommand cmd = conn.CreateCommand("N_NM_PCK_MP.pr_mp__mp_ev_voz_add") )
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new CoraParameter("tnRet", DbType.Int32) { Direction = ParameterDirection.Output });
						cmd.Parameters.Add(new CoraParameter("pnI_FE_REG_LOG", Int32.Parse(responseEVO.I_fe_reg_log), DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("pcVIN", responseEVO.Vozidlo?.VIN ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcEC", responseEVO.Vozidlo?.EvidencneCislo ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcKATEG", responseEVO.Vozidlo?.KategoriaVozidla ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcDRUH", responseEVO.Vozidlo?.DruhVozidla ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcZNACKA", responseEVO.Vozidlo?.Znacka ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcFARBA", responseEVO.Vozidlo?.Farba ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcSTAV", responseEVO.Vozidlo?.StavVozidla ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pdD_ZMENA", responseEVO.Vozidlo?.DatumZmeny, DbType.Date));
						cmd.Parameters.Add(new CoraParameter("pcPOZN", null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pnZRUS", 0, DbType.Int32));

						cmd.ExecuteNonQuery();
						CoraParameter ret = (CoraParameter) cmd.Parameters["tnRet"];
						tnRet = ret.Value.ToString();
					}
					conn.Close();
				}

				if ( string.IsNullOrEmpty(tnRet) )
					tnRet = "-1";

				return tnRet;
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error, $"Nepodarilo sa vytvorit zaznam v mp_ev_voz. Chyba: {ex.ToString()}");
				return "-1";
			}
		}


		/// <summary>
		/// Zapise lustraciu vozidla do mp_ev_drz
		/// </summary>
		/// <param name="responseEVO">udaje z EVO</param>
		/// <returns>ID zapisu v tabulke mp_ev_drz</returns>
		private string AddMpEvDrz(EVOResponse responseEVO)
		{
			if ( responseEVO == null )
				return "-1";

			try
			{
				string tnRet = "";

				Natec.Base.ObjFactory<CdoZPO_MAILING_STATE> factory = (Natec.Base.ObjFactory<CdoZPO_MAILING_STATE>) Natec.Base.FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>();
				using ( CoraConnection conn = factory.CreateAndOpenConnection() )
				{
					using ( IDbCommand cmd = conn.CreateCommand("N_NM_PCK_MP.pr_mp__mp_ev_drz_add") )
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new CoraParameter("tnRet", DbType.Int32) { Direction = ParameterDirection.Output });
						cmd.Parameters.Add(new CoraParameter("pnI_VOZ", Int32.Parse(responseEVO.I_voz), DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("pcMENO", responseEVO.Vozidlo?.Drzitel?.DrzitelMeno ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcPRIEZ", responseEVO.Vozidlo?.Drzitel?.DrzitelPriezvisko ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pdD_NAR", responseEVO.Vozidlo?.Drzitel?.DrzitelDatumNarodenia, DbType.Date));
						cmd.Parameters.Add(new CoraParameter("pcRC", responseEVO.Vozidlo?.Drzitel?.DrzitelRodneCislo ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_P", responseEVO.Vozidlo?.Drzitel?.DrzitelNazov ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcICO", responseEVO.Vozidlo?.Drzitel?.DrzitelICO ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcPOBYT_TYP", responseEVO.Vozidlo?.Drzitel?.DrzitelPobyt?.DrzitelPobytTypPobytu ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_OKRES", responseEVO.Vozidlo?.Drzitel?.DrzitelPobyt?.DrzitelPobytOkres ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_M", responseEVO.Vozidlo?.Drzitel?.DrzitelPobyt?.DrzitelPobytObec ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_U", responseEVO.Vozidlo?.Drzitel?.DrzitelPobyt?.DrzitelPobytUlica ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcCO", responseEVO.Vozidlo?.Drzitel?.DrzitelPobyt?.DrzitelPobytOrientacneCislo ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcCS", responseEVO.Vozidlo?.Drzitel?.DrzitelPobyt?.DrzitelPobytSupisneCislo ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcMENO_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelMeno ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcPRIEZ_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPriezvisko ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pdD_NAR_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelDatumNarodenia, DbType.Date));
						cmd.Parameters.Add(new CoraParameter("pcRC_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelRodneCislo ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_P_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelNazov ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcICO_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelICO ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcPOBYT_TYP_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelPobytTypPobytu ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_OKRES_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelPobytOkres ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_M_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelPobytObec ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcN_U_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelPobytUlica ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcCO_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelPobytOrientacneCislo ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcCS_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelPobytSupisneCislo ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcADR_ZAHR_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelAdresaMimoSR ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcSTAT_ZAHR_B", responseEVO.Vozidlo?.BuduciDrzitel?.BuduciDrzitelPobyt?.BuduciDrzitelStatAdresyMimoSR ?? null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pcPOZN", null, DbType.String));
						cmd.Parameters.Add(new CoraParameter("pnZRUS", 0, DbType.Int32));
						cmd.ExecuteNonQuery();
						CoraParameter ret = (CoraParameter) cmd.Parameters["tnRet"];
						tnRet = ret.Value.ToString();
					}
					conn.Close();
				}

				if ( string.IsNullOrEmpty(tnRet) )
					tnRet = "-1";

				return tnRet;
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error, $"Nepodarilo sa vytvorit zaznam v mp_ev_drz. Chyba: {ex.ToString()}");
				return "-1";
			}
		}
	}
}