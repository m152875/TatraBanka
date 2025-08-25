using System;
using Cora.CommIss.Iss.CdoCto.ExtData;
using Cora.CoraApp.ISS.EXT.MAT;
using Cora.Data.DBProvider;
using Cora.Utils.Logger;
using static Cora.CommIss.PublikovanieInformacii.PublikovanieInformaciiProvide;

namespace Cora.CommIss.Iss.Impl
{
	public class ExtDataProvide : IExtDataProvide
	{
		#region json (navratovy typ)
		public string GetMET_C_CM_json(CtoMET_C_CM metCcm)
		{
			return SerializeToJson<CtoMET_C_CM[]>(GetMET_C_CM(metCcm));
		}

		public string GetMET_C_U_json(CtoMET_C_U metCu)
		{
			return SerializeToJson<CtoMET_C_U[]>(GetMET_C_U(metCu));
		}

		public string GetMET_C_PA_MM_json(CtoMET_C_PA_MM metCpamm)
		{
			return SerializeToJson<CtoMET_C_PA_MM[]>(GetMET_C_PA_MM(metCpamm));
		}

		public string GetMET_PO_PES_json(CtoMET_PO_PES metPopes)
		{
			return SerializeToJson<CtoMET_PO_PES[]>(GetMET_PO_PES(metPopes));
		}

		public string GetMET_DN_POZ_json(CtoMET_DN_POZ metDnpoz)
		{
			return SerializeToJson<CtoMET_DN_POZ[]>(GetMET_DN_POZ(metDnpoz));
		}
		#endregion

		#region originalne WCF datatype metody
		public CtoMET_C_CM[] GetMET_C_CM(CtoMET_C_CM metCcm)
		{
			ObjectGridMapper<CtoMET_C_CM> mapper = ObjectGridMapper<CtoMET_C_CM>.GetMapper(293093, null);

			return mapper.GetInformacie(metCcm);
		}

		public CtoMET_C_U[] GetMET_C_U(CtoMET_C_U metCu)
		{
			ObjectGridMapper<CtoMET_C_U> mapper = ObjectGridMapper<CtoMET_C_U>.GetMapper(293094, null);

			return mapper.GetInformacie(metCu);
		}

		public CtoMET_C_PA_MM[] GetMET_C_PA_MM(CtoMET_C_PA_MM metCpamm)
		{
			ObjectGridMapper<CtoMET_C_PA_MM> mapper = ObjectGridMapper<CtoMET_C_PA_MM>.GetMapper(293096, null);

			return mapper.GetInformacie(metCpamm);
		}

		public CtoMET_PO_PES[] GetMET_PO_PES(CtoMET_PO_PES metPopes)
		{
			ObjectGridMapper<CtoMET_PO_PES> mapper = ObjectGridMapper<CtoMET_PO_PES>.GetMapper(293097, null);

			return mapper.GetInformacie(metPopes);
		}

		public CtoMET_DN_POZ[] GetMET_DN_POZ(CtoMET_DN_POZ metDnpoz)
		{
			ObjectGridMapper<CtoMET_DN_POZ> mapper = ObjectGridMapper<CtoMET_DN_POZ>.GetMapper(293095, null);

			return mapper.GetInformacie(metDnpoz);
		}

		/// <summary>Získanie informácií o zmluvách k požadovanej parcele vo formáte JSON.</summary>
		/// <param name="identifikatorPozadovanejParcely">Identifikátor požadovanej parcely.</param>
		/// <returns>Informácie o zmluvách k požadovanej parcele (objekt JSON).</returns>
		public string GetZmluvyKParcele_json(string identifikatorPozadovanejParcely)
		{
			var res = GetZmluvyKParcele(identifikatorPozadovanejParcely);
			if ( null == res ) return null;

			return CtoVW_ZM_POZ.GetDataJSON(res, new Newtonsoft.Json.JsonSerializerSettings
			{
				ContractResolver = new CtoVW_ZM_POZ.CustomResolver(_ZM_AddIDs),
				Formatting = Newtonsoft.Json.Formatting.Indented
			});
		}

		/// <summary>Získanie informácií o zmluvách k požadovanej parcele vo formáte <see cref="CtoVW_ZM_POZ.MainVm"/>.</summary>
		/// <param name="identifikatorPozadovanejParcely">Identifikátor požadovanej parcely.</param>
		/// <returns>Informácie o zmluvách k požadovanej parcele (objekt <see cref="CtoVW_ZM_POZ.MainVm"/>).</returns>
		public CtoVW_ZM_POZ.MainVm GetZmluvyKParcele(string identifikatorPozadovanejParcely)
		{
			CoraConnection conn = null;
			try
			{
				conn = new CoraConnection(_ConnStr.Value);
				conn.Open();
				AppLogging.Logger.Log(LogLevel.Info, "[IdentifikatorPozadovanejParcely={0}] GetZmluvy", identifikatorPozadovanejParcely);
				CtoVW_ZM_POZ.MainVm res = CtoVW_ZM_POZ.GetData(conn, identifikatorPozadovanejParcely);

				if ( AppLogging.Logger.IsLoggable(LogLevel.Finest) )
				{
					string jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(res, new Newtonsoft.Json.JsonSerializerSettings
					{
						ContractResolver = new CtoVW_ZM_POZ.CustomResolver(true),
						Formatting = Newtonsoft.Json.Formatting.Indented
					});
					Cora.Global.TempDirectoryHelper _TempDirectory = new Global.TempDirectoryHelper("ExtDataProvide");
					string filepath = _TempDirectory.Gener_FilePath($"ZM_IP{identifikatorPozadovanejParcely}", "json");
					System.IO.File.WriteAllText(filepath, jsonText);
				}
				return res;
			}
			catch ( Exception ex )
			{
				AppLogging.Logger.Log(LogLevel.Error, "[IdentifikatorPozadovanejParcely={0}] GetZmluvy_json. Chyba: {1}", identifikatorPozadovanejParcely, ex);
				return null;
			}
			finally
			{
				if ( null != conn )
				{
					if ( conn.State == System.Data.ConnectionState.Open )
						conn.Close();
					conn.Dispose();
				}
			}
		}

		#endregion

		public string SerializeToJson<T>(T obj) where T : class
		{
			System.IO.MemoryStream mStream = new System.IO.MemoryStream();
			System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
			ser.WriteObject(mStream, obj);

			return System.Text.Encoding.UTF8.GetString(mStream.ToArray());
		}

		static Lazy<string> _ConnStr = new Lazy<string>(() => Cora.Natec.AA.CdoAA_DBZDR.GetConnectionStringForDbZdr(1010));

		static ExtDataProvide()
		{
			string sValue = System.Configuration.ConfigurationManager.AppSettings["ExtData_ZM_AddIDs"];
			_ZM_AddIDs = Cora.Convert.Parser.TryGetBool(sValue, out bool bValue) && bValue;
		}

		readonly static bool _ZM_AddIDs;
	}
}