using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Cora.CommIss.Iss.Impl
{
	public class TLZProvidePrivate : ITLZProvidePrivate
	{
		public TLZFile GetGenPDF(int pnI_tl_dat, int pnI_uz)
		{
			Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Debug,
				string.Format("TLZProvidePrivate.GetGenPDF: pnI_tl_dat: {0}, pnI_uz", pnI_tl_dat, pnI_uz));

			TLZFile res = new TLZFile();
			res.Success = false;

			try
			{
				//volanie zaregistrovanej COM kniznice VISUAL FOX PRO (publikovanej JDr)
				/*foxtlarchiv.foxtlarchiv vfpWorker = new foxtlarchiv.foxtlarchiv();
				string filepath = vfpWorker.FoxGenPDF(pnI_tl_dat, pnI_uz);

				// uvolni objekt
				vfpWorker = null;

				if ( string.IsNullOrEmpty(filepath?.Trim()) )
				{
					string msg = "Cesta k súboru nebola vygenerovaná.";

					Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
						string.Format("TLZProvidePrivate.GetGenPDF: {0}",
						msg));

					res.ErrorMsg = msg;
					return res;
				}

				byte[] fileAsBytes = File.ReadAllBytes(filepath);

				if ( null == fileAsBytes )
				{
					string msg = "Súbor sa nepodarilo načítať.";

					Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
						string.Format("TLZProvidePrivate.GetGenPDF: {0}",
						msg));

					res.ErrorMsg = msg;
					return res;
				}

				res.Content = System.Convert.ToBase64String(fileAsBytes);

				FileInfo fInfo = new FileInfo(filepath);

				res.Name = fInfo.Name;
				res.Length = fInfo.Length;
				res.Extension = fInfo.Extension;
				res.Success = true;
				*/
			}
			catch ( Exception e )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("TLZProvidePrivate.GetGenPDF: Chyba pri generovani alebo spracovani suboru: {0}",
					e.Message));

				res.Success = false;
				res.ErrorMsg = e.Message;
			}
			return res;

		}
	}
}