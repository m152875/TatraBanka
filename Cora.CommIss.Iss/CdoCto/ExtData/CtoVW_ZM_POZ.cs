using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cora.CoraApp.ISS.NUM.AA;
using Cora.Data.DBProvider;
using Cora.Natec.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cora.CommIss.Iss.CdoCto.ExtData
{
	/// <summary>Trieda pre získanie informácií o zmluvách k požadovanej parcele.</summary>
	public static partial class CtoVW_ZM_POZ
	{
		/// <summary>Získanie dát k požadovanej parcele (objekt JSON).</summary>
		/// <param name="conn">Konekcia do DB.</param>
		/// <param name="identifikatorPozadovanejParcely">Identifikátor požadovanej parcely.</param>
		/// <param name="settings">Nastavenia pre serializáciu.</param>
		public static string GetDataJson(CoraConnection conn, string identifikatorPozadovanejParcely, JsonSerializerSettings settings = null)
		{
			MainVm res = GetData(conn, identifikatorPozadovanejParcely);
			return GetDataJSON(res, settings);
		}

		public static string GetDataJSON(MainVm res, JsonSerializerSettings settings = null)
		{
			if ( null == settings )
			{
				settings = new JsonSerializerSettings
				{
					ContractResolver = new CustomResolver(false),
					Formatting = Formatting.Indented
				};
			}
			return JsonConvert.SerializeObject(res, settings);
		}

		/// <summary>Získanie dát k požadovanej parcele (objekt <see cref="MainVm"/>).</summary>
		/// <param name="conn"></param>
		/// <param name="identifikatorPozadovanejParcely"></param>
		public static MainVm GetData(CoraConnection conn, string identifikatorPozadovanejParcely)
		{
			MainVm res = new MainVm();
			res.IDENTIF = identifikatorPozadovanejParcely;

			IEnumerable<int> ids = GetZmluvaIDs(conn, identifikatorPozadovanejParcely);
			if ( ids.Any() )
			{
				var zmluvy = new List<ZmluvaVm>();
				foreach ( int iI_ZM in ids )
				{
					var zml = new ZmluvaVm() { I_ZM = iI_ZM };
					zml.Predmety = GetPredmety(conn, iI_ZM);
					zml.ZML_STRANY = GetZmlStrany(conn, iI_ZM);
					zml.Dokumenty = GetAttachs(conn, iI_ZM);
					zmluvy.Add(zml);
				}
				res.Zmluvy = zmluvy;
			}

			return res;
		}

		private static int[] GetZmluvaIDs(this CoraConnection conn, string identifikatorPozadovanejParcely)
		{
			List<int> ids = new List<int>();
			string sql = string.Format("SELECT DISTINCT i_zm FROM vw_zm_poz WHERE identif = '{0}' ORDER BY i_zm", identifikatorPozadovanejParcely);
			using ( CoraCommand cmd = (CoraCommand) conn.CreateCommand(sql) )
			{
				using ( System.Data.IDataReader dr = cmd.ExecuteReader() )
				{
					while ( dr.Read() )
					{
						ids.Add(Cora.Convert.Parser.GetInt(dr["I_ZM"]));
					}
				}
			}
			return ids.ToArray();
		}

		private static PredmetVm[] GetPredmety(this CoraConnection conn, int iI_ZM)
		{
			List<PredmetVm> res = new List<PredmetVm>();

			string sql = string.Format("SELECT * FROM vw_zm_poz WHERE i_zm = {0} ORDER BY i_zm", iI_ZM);
			using ( CoraCommand cmd = (CoraCommand) conn.CreateCommand(sql) )
			{
				using ( System.Data.IDataReader dr = cmd.ExecuteReader() )
				{
					while ( dr.Read() )
					{
						PredmetVm cto = new PredmetVm();
						cto.I_ZM_POZ = Cora.Convert.Parser.GetLong(dr["I_ZM_POZ"]);
						cto.I_ZM_PREDM = Cora.Convert.Parser.GetInt(dr["I_ZM_PREDM"]);
						cto.I_PA = Cora.Convert.Parser.GetInt(dr["I_PA"]);
						cto.IDENTIF = dr["IDENTIF"].ToString();
						cto.CP = dr["CP"].ToString();
						cto.VYM = Cora.Convert.NullableParser.GetDecimal(dr["VYM"]);
						cto.VYM_SKUT = Cora.Convert.NullableParser.GetDecimal(dr["VYM_SKUT"]);
						cto.SK_M2 = Cora.Convert.NullableParser.GetDecimal(dr["SK_M2"]);
						cto.N_NM_TYP = dr["N_NM_TYP"].ToString();
						if ( Cora.Convert.Parser.TryGetInt(dr["I_ST"], out int iI_ST) && Cora.Convert.Parser.TryGetInt(dr["I_ADR"], out int iI_ADR) )
						{
							cto.Stavba = new StavbaVm() { I_ST = iI_ST, I_ADR = iI_ADR };
							cto.Stavba.N_U = dr["N_U"].ToString();
							cto.Stavba.CS = dr["CS"].ToString();
							cto.Stavba.CO = dr["CO"].ToString();
							cto.Stavba.N_M = dr["N_M"].ToString();
						}
						res.Add(cto);
					}
				}
			}
			return res.ToArray();
		}

		private static string[] GetZmlStrany(this CoraConnection conn, int iI_ZM)
		{
			List<string> res = new List<string>();

			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT ozm.i_zm, ozm.i_zm_o_zm");
			sb.Append(", TRIM(TRIM(o.pf) || CASE WHEN o.i_o_typ = 2 THEN ' IČO: ' || o.ico ELSE ' ' || TRIM(o.meno) || ' ' || TRIM(o.ttp) || ' ' || TRIM(o.ttz) END) AS zml_strany");
			sb.Append(" FROM n_nm_zm_o_zm ozm");
			sb.Append(" LEFT OUTER JOIN n_nm_im_o im ON ozm.i_o_im = im.i_o_im");
			sb.Append(" LEFT OUTER JOIN n_nm_c_o o ON im.i_o = o.i_o");
			sb.Append(" WHERE ozm.zrus = 0");
			sb.Append(" AND im.i_o<>(SELECT NVL(TO_NUMBER(MAX(hodn)), 1) FROM n_nm_sp_def_ob WHERE i_def_ob = 999998008)");
			sb.AppendFormat(" AND ozm.i_zm = {0}", iI_ZM);
			using ( CoraCommand cmd = (CoraCommand) conn.CreateCommand(sb.ToString()) )
			{
				using ( System.Data.IDataReader dr = cmd.ExecuteReader() )
				{
					while ( dr.Read() )
					{
						string text = dr["ZML_STRANY"].ToString().Trim();
						if ( !string.IsNullOrEmpty(text) )
							res.Add(dr["ZML_STRANY"].ToString());
					}
				}
			}
			return res.Any() ? res.ToArray() : null;
		}

		private static AttachVm[] GetAttachs(this CoraConnection conn, int iI_ZM)
		{
			ObjFactory<clsDataObjC_ZAZ_ZAZ> oFact = FactoryFactory.GetSFactory<clsDataObjC_ZAZ_ZAZ>() as ObjFactory<clsDataObjC_ZAZ_ZAZ>;

			StringBuilder sb = new StringBuilder();
			sb.Append("zrus = 0 AND l_zverej = 1");
			sb.Append(" AND EXISTS (SELECT 1 FROM n_nb_c_zaz_zaz_bin WHERE hodn_dat IS NOT NULL AND c_size > 0 AND i_zaz_zaz = c_zaz_zaz.i_zaz_zaz)");
			sb.AppendFormat(" AND n_tab = 'ZM' AND i_ = {0}", iI_ZM);
			IEnumerable<int> ids = oFact.getValuesByCond(conn, "i_zaz_zaz", sb.ToString(), "i_zaz_zaz", false).ToArray().Select(f => Cora.Convert.Parser.GetInt(f));
			List<AttachVm> attachs = new List<AttachVm>();
			if ( ids.Any() )
			{
				foreach ( int iI_ZAZ_ZAZ in ids )
				{
					clsDataObjC_ZAZ_ZAZ cdo = oFact.getByPK(conn, iI_ZAZ_ZAZ);
					if ( null == cdo )
						throw new CoreException($"[i_zaz_zaz={iI_ZAZ_ZAZ}] Záznam neexistuje");
					if ( !cdo.LoadContent(null) )
						throw new CoreException($"[i_zaz_zaz={iI_ZAZ_ZAZ}] Binárny obsah sa nepodarilo načítať");
					AttachVm cto = new AttachVm() { I_ZAZ_ZAZ = iI_ZAZ_ZAZ };
					cto.Filename = cdo.ContentFull.FileName;
					cto.Mimetype = cdo.ContentFull.ContentType;
					cto.Content = cdo.ContentFull.Content;
					attachs.Add(cto);
				}
			}
			return attachs.Any() ? attachs.ToArray() : null;
		}

		public class CustomResolver : DefaultContractResolver
		{
			public CustomResolver(bool addIDs)
			{
				_AddIDs = addIDs;
			}

			protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
			{
				JsonProperty prop = base.CreateProperty(member, memberSerialization);
				if ( prop.PropertyType == typeof(decimal?) )
				{
					prop.Converter = new CustomDecimalConverter(prop.UnderlyingName.ToUpper().Contains("VYM") ? 3 : 2);
				}
				else if ( !_AddIDs )
				{
					if ( (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long))
						&& prop.PropertyName.EndsWith("ID", StringComparison.InvariantCultureIgnoreCase) )
					{
						prop.Ignored = true;
					}
				}

				return prop;
			}

			readonly bool _AddIDs;
		}

		/// <summary>Konvertuje objekt typu <see cref="decimal"/> alebo <see cref="Nullable\<decimal\>"/> na JSON.</summary>
		public class CustomDecimalConverter : CustomCreationConverter<decimal?>
		{
			/// <summary>Inicializuje novú inštanciu triedy <see cref="CustomDecimalConverter" />.</summary>
			/// <param name="decimalDigits">Počet desitinných miest na výstupe.</param>
			public CustomDecimalConverter(int decimalDigits)
			{
				_DecimalDigits = decimalDigits;
			}

			/// <summary>Určuje, či táto inštancia môže previesť určený typ objektu (iba typ <see cref="decimal?"/>).</summary>
			/// <param name = "objectType"> Typ objektu. </param>
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(decimal?);
			}

			/// <summary>Môže <see cref="CustomDecimalConverter" /> načítať JSON (false).</summary>
			public override bool CanRead => false;

			/// <summary>Môže <see cref="CustomDecimalConverter" /> vytvoriť JSON (true).</summary>
			public override bool CanWrite => true;

			/// <summary>Vytvorí objekt, ktorý bude potom vyplnený serializátorom.</summary>
			/// <param name="objectType"> Typ objektu. </param>
			/// <returns> Vytvorený objekt. </returns>
			/// <remarks>Neimplementované. Nie je potrebné.</remarks>
			public override decimal? Create(Type objectType)
			{
				throw new NotImplementedException();
			}

			/// <summary>Writes the JSON representation of the object.</summary>
			/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
			/// <param name="value">The value.</param>
			/// <param name="serializer">The calling serializer.</param>
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				if ( value == null )
				{
					writer.WriteNull();
				}
				else
				{
					if ( !Cora.Convert.Parser.TryGetDecimal(value, out decimal oValue) )
					{
						string message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Položku typu \"{0}\" nie je možné konvertovať na Decimal.", value.GetType());
						throw CreateEx(writer, message);
					}
					System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("sk-SK");
					cultureInfo.NumberFormat.NumberDecimalDigits = _DecimalDigits;
					cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
					System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." };
					nfi.NumberDecimalDigits = _DecimalDigits;
					string text = oValue.ToString("f", nfi);
					writer.WriteValue(text);
				}
			}

			/// <summary>Vydenerovanie výnimky <see cref="JsonSerializationException"/>.</summary>
			/// <param name="writer">Zapisovač.</param>
			/// <param name="message">Zdrojová chybová správa.</param>
			/// <param name="ex">InnerException ak existuje.</param>
			/// <returns></returns>
			public static JsonSerializationException CreateEx(JsonWriter writer, string message, Exception ex = null)
			{
				string path = writer.Path;
				if ( !message.EndsWith(Environment.NewLine, StringComparison.Ordinal) )
				{
					message = message.Trim();
					if ( !message.EndsWith(".") )
						message += ".";
					message += " ";
				}
				message += string.Format(System.Globalization.CultureInfo.InvariantCulture, "Path '{0}'", path);

				bool bb = TryFillLineProperties(writer as IJsonLineInfo, ref message, out int lineNumber, out int linePosition);
				return new Newtonsoft.Json.JsonSerializationException(message, ex);
			}

			/// <summary>Doplnenie chybovej správy o informácie z objektu <see cref="IJsonLineInfo"/> ak je k dispozícii.</summary>
			/// <param name="lineInfo">Objekt <see cref="IJsonLineInfo"/>.</param>
			/// <param name="message">Upravovaná chybová správa.</param>
			/// <param name="lineNumber">Chyba na riadku.</param>
			/// <param name="linePosition">Chyba na pozícii v riadku.</param>
			/// <returns><c>true</c> ak objekt <see cref="IJsonLineInfo"/> je k dispozícii.</returns>
			private static bool TryFillLineProperties(IJsonLineInfo lineInfo, ref string message, out int lineNumber, out int linePosition)
			{
				if ( lineInfo != null && lineInfo.HasLineInfo() )
					message += string.Format(System.Globalization.CultureInfo.InvariantCulture, ", line {0}, position {1}", lineInfo.LineNumber, lineInfo.LinePosition);
				message += ".";
				if ( lineInfo != null && lineInfo.HasLineInfo() )
				{
					lineNumber = lineInfo.LineNumber;
					linePosition = lineInfo.LinePosition;
					return true;
				}
				else
				{
					lineNumber = 0;
					linePosition = 0;
					return false;
				}
			}

			readonly int _DecimalDigits;
		}
	}
}