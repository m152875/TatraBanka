using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cora.CommIss.Iss.EVO
{
	/// <summary>
	/// Trieda mapujuca requesty privatej sluzby a klienta sluzby EVO
	/// </summary>
	public static class RequestMapper
	{
		/// <summary>
		/// Premapuje request privatnej sluzby na request klienta sluzby EVO
		/// </summary>
		/// <param name="req">Request privatnej sluzby</param>
		/// <returns>Request klienta EVO</returns>
		public static EVOClient.vozidloRequest ToClientRequest(VozidloRequest req)
		{
			EVOClient.vozidloRequest ret = new EVOClient.vozidloRequest();
			if ( req.EvidencneCislo != null || req.Vin != null )
			{
				ret.dovodLustracie = req.DovodLustracie;
				ret.ep = req.Ep;
				ret.evidencneCislo = req.EvidencneCislo;
				ret.td = req.Td;
				ret.vin = req.Vin;
			}
			else
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("EVO.RequestMapper.ToClientRequest: Aspon jeden z parametrov VIN alebo evidencne cislo je povinny!"));
			}
			return ret;
		}

		public static VozidloRequest ToServiceRequest(EVOClient.vozidloRequest req)
		{
			VozidloRequest ret = new VozidloRequest();
			if ( req.evidencneCislo != null || req.vin != null )
			{
				ret.DovodLustracie = req.dovodLustracie;
				ret.Ep = req.ep;
				ret.EvidencneCislo = req.evidencneCislo;
				ret.Td = req.td;
				ret.Vin = req.vin;
			}
			return ret;
		}
	}
}