using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cora.CommIss.Iss.DMS;
using Cora.DMS;
using Cora.DMS.Base;

namespace Cora.CommIss.Iss.Impl
{
	public class DMSProvide : IDMSProvide
	{
		public int DeleteFile(int iFile)
		{
			DMSProvider dms = new DMSProvider();
			return dms.DeleteFile(iFile, 0);
		}

		public int InsertFile(string fileName, string extension, string mimeType, byte[] content)
		{
			DMSProvider dms = new DMSProvider();
			return dms.InsertFile(fileName, extension, mimeType, content, 0);
		}

		public int UpdateFile(int iFile, string fileName, string extension, string mimeType, byte[] content)
		{
			DMSProvider dms = new DMSProvider();
			return dms.UpdateFile(iFile, fileName, extension, mimeType, content, 0);
		}

		public GetFileByFileIdRes GetFileByFileId(int iFile)
		{
			CtoC_FILE_DMS retCto = null;

			try
			{
				DMSProvider dms = new DMSProvider();
				var cdo = dms.GetByFileId(iFile);

				retCto = cdo.ToDto();

				return new GetFileByFileIdRes(0, retCto);
			}
			catch ( Exception e )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("DMSProvide.GetFileByFileIdResponse chyba ziskavania objektu podla id {0}: {1}",
					iFile, e.Message));

				return new GetFileByFileIdRes(-1, null);
			}
			finally
			{
				if ( null != retCto )
					Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Debug,
						string.Format("DMSProvide.GetFileByFileIdResponse ziskavania objektu podla id {0}: {1}",
						iFile, retCto.N_FILE));
				else
					Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Warning,
					string.Format("DMSProvide.GetFileByFileIdResponse ziskavania objektu podla id {0} je prazdne.",
					iFile));
			}
		}
	}
}