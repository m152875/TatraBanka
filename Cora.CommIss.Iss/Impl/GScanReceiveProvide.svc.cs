using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Cora.CommIss.Iss.GScan;
using Cora.CommIss.SKTalk;
using Cora.CoraApp.ISS.NUM.AA;
using Cora.Data.DBProvider;

namespace Cora.CommIss.Iss.Impl
{
	public class GScanReceiveProvide : IGScanReceiveProvide
	{
		private const int GSCAN_EL_PODANIE_ZDROJ = 5;

		public int ReceiveDocuments(ScannedMessage message)
		{
			if ( null == message )
				return SKTalkReceiveCode.NULL_MESSAGE;

			try
			{
				int iZaz = SKTalkReceiveCode.NO_RESPONSE;

				Natec.Base.ObjFactory<clsDataObjC_ZAZ> factory = (Natec.Base.ObjFactory<clsDataObjC_ZAZ>) Natec.Base.FactoryFactory.GetSFactory<clsDataObjC_ZAZ>();
				using ( CoraConnection conn = factory.CreateAndOpenConnection() )
				{
					using ( IDbCommand cmd = conn.CreateCommand("N_DS_PI_ZAZ_GSCAN_ISS_INS") )
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new CoraParameter("mI_UZ_EVID", IntegrationSettings.OpisDbSaverUserId, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("mDELIVERYDATE", message.DeliveryDate, DbType.Date));
						cmd.Parameters.Add(new CoraParameter("mDELIVERYMETHOD", message.DeliveryMethod, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("mDESCRIPTION", message.Description, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mMSGTYPE", message.MsgType, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mPROCESSTODATE", message.ProcessToDate, DbType.Date));
						cmd.Parameters.Add(new CoraParameter("mSENDERSIGN", message.SenderSign, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mSUBJECT", message.Subject, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mTYPEANDKIND", message.TypeAndKind, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("mUSERCENTER", message.UserCenter, DbType.String));
						//sender part
						cmd.Parameters.Add(new CoraParameter("mCITY", message.SenderWithAddress.City, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mCOMPANYNAME", message.SenderWithAddress.CompanyName, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mCOMPANYNUMBER", message.SenderWithAddress.CompanyNumber, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("mDISTRICT", message.SenderWithAddress.District, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mFIRSTNAME", message.SenderWithAddress.FirstName, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mLASTNAME", message.SenderWithAddress.LastName, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mSTREETINVENTORYNM", message.SenderWithAddress.StreetInventoryNm, DbType.Int32));
						cmd.Parameters.Add(new CoraParameter("mSTREETNAME", message.SenderWithAddress.StreetName, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mSTREETREFERENCENM", message.SenderWithAddress.StreetReferenceNm, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mTITLEAFTER", message.SenderWithAddress.TitleAfter, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mTITLEFRONT", message.SenderWithAddress.TitleFront, DbType.String));
						cmd.Parameters.Add(new CoraParameter("mZIP", message.SenderWithAddress.Zip, DbType.String));

						cmd.Parameters.Add(new CoraParameter("mI_ZAZ", DbType.Int32) { Direction = ParameterDirection.Output });

						cmd.ExecuteNonQuery();
						CoraParameter ret = (CoraParameter) cmd.Parameters["mI_ZAZ"];
						iZaz = int.Parse(ret.Value.ToString());
					}

					if ( iZaz < 0 )
					{
						Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
							string.Format("Nepodarilo sa uložiť GScan document: {0}", iZaz.ToString()));

						conn.Close();

						return iZaz;
					}

					//spracovanie suborov
					if ( !InsertFiles(conn, iZaz, message.Files.ToList<ScannedFile>(), message.DeliveryDate) )
					{
						Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
							string.Format("GScanReceiveProvide: Spracovaie príloh nebolo kompletné."));

						conn.Close();

						return SKTalkReceiveCode.DB_WRITE_FAILED;
					}

					conn.Close();
				}
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("Nepodarilo sa spracovať GScan document: {0}", ex.ToString()));
			}

			return SKTalkReceiveCode.Ok;
		}

		private bool InsertFiles(CoraConnection conn, int iZaz, List<ScannedFile> files, DateTime dateDelivery)
		{
			int[] resFiles = new int[files.Count()];

			files.ForEach(file =>
			{
				resFiles[files.IndexOf(file)] = SKTalkProvide.SaveZaznamBin(conn, iZaz, dateDelivery, null, file.Content, System.IO.Path.GetExtension(file.Filename),
					file.Filename, null, false, false, int.Parse(file.Length), file.MimeType, null, false, false, file.IsMainFile, GSCAN_EL_PODANIE_ZDROJ);
			});

			//overenie uspesnosti vlozenia vsetkych suborov do C_FILE_DIS
			if ( resFiles.Count(i => i < 0) > 0 )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("Nepodarilo sa spracovať všetky GScan súbory: ({0}) s výsledkami spracovania: ({1})",
						string.Join(",", files.Select(f => f.Filename)), 
						string.Join(",", resFiles))
				);

				return false;
			}

			return true;
		}
	}
}