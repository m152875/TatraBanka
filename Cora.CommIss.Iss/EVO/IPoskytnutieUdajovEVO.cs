using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Xml.Serialization;
using System.Data;

namespace Cora.CommIss.Iss.EVO
{
	/// <summary>
	/// Rozhranie pre poskytnutie udajov z registra vozidiel
	/// </summary>
	[ServiceContract]
	[Cora.CoraCoreObj.RequiresValidConnId]
	public interface IPoskytnutieUdajovEVO
	{
		/// <summary>
		/// Sluzba pre poskytnutie udajov z registra vozidiel
		/// </summary>
		/// <param name="request">Ziadost s udajmi o vozidle</param>
		/// <returns>Udaje o drzitelovi vozidla</returns>
		[OperationContract]
		EVOResponse PoskytnutieUdajovEVO(VozidloRequest request, int zdroj = 2);

		/// <summary>
		/// Sluzba pre poskytnutie udajov z registra vozidiel
		/// </summary>
		/// <param name="request">Ziadost s udajmi o vozidle</param>
		/// <returns>Udaje o drzitelovi vozidla v binarnom tvare</returns>
		[OperationContract]
		byte[] PoskytnutieUdajovEVOCompat(string XmlVozidloRequest);
	}
	[XmlRoot("argv0")]
	public class VozidloRequest
	{

		private string vinField;

		private string evidencneCisloField;

		private string epField;

		private string tdField;

		private string dovodLustracieField;

		private string id_UzivatelaField;

		public string Vin
		{
			get
			{
				return this.vinField;
			}
			set
			{
				this.vinField = value;
			}
		}

		public string EvidencneCislo
		{
			get
			{
				return this.evidencneCisloField;
			}
			set
			{
				this.evidencneCisloField = value;
			}
		}

		public string Ep
		{
			get
			{
				return this.epField;
			}
			set
			{
				this.epField = value;
			}
		}

		public string Td
		{
			get
			{
				return this.tdField;
			}
			set
			{
				this.tdField = value;
			}
		}

		public string DovodLustracie
		{
			get
			{
				return this.dovodLustracieField;
			}
			set
			{
				this.dovodLustracieField = value;
			}
		}

		public string Id_uzivatela
		{
			get
			{
				return this.id_UzivatelaField;
			}
			set
			{
				this.id_UzivatelaField = value;
			}
		}

	}

	public class PobytSidloBuducehoDrzitela
	{

		private string buduciDrzitelPobytTypPobytuField;

		private string buduciDrzitelPobytOkresField;

		private string buduciDrzitelPobytObecField;

		private string buduciDrzitelPobytUlicaField;

		private string buduciDrzitelPobytOrientacneCisloField;

		private string buduciDrzitelPobytSupisneCisloField;

		private string buduciDrzitelAdresaMimoSRField;

		private string buduciDrzitelStatAdresyMimoSRField;

		public string BuduciDrzitelPobytTypPobytu
		{
			get
			{
				return this.buduciDrzitelPobytTypPobytuField;
			}
			set
			{
				this.buduciDrzitelPobytTypPobytuField = value;
			}
		}

		public string BuduciDrzitelPobytOkres
		{
			get
			{
				return this.buduciDrzitelPobytOkresField;
			}
			set
			{
				this.buduciDrzitelPobytOkresField = value;
			}
		}

		public string BuduciDrzitelPobytObec
		{
			get
			{
				return this.buduciDrzitelPobytObecField;
			}
			set
			{
				this.buduciDrzitelPobytObecField = value;
			}
		}

		public string BuduciDrzitelPobytUlica
		{
			get
			{
				return this.buduciDrzitelPobytUlicaField;
			}
			set
			{
				this.buduciDrzitelPobytUlicaField = value;
			}
		}

		public string BuduciDrzitelPobytOrientacneCislo
		{
			get
			{
				return this.buduciDrzitelPobytOrientacneCisloField;
			}
			set
			{
				this.buduciDrzitelPobytOrientacneCisloField = value;
			}
		}

		public string BuduciDrzitelPobytSupisneCislo
		{
			get
			{
				return this.buduciDrzitelPobytSupisneCisloField;
			}
			set
			{
				this.buduciDrzitelPobytSupisneCisloField = value;
			}
		}

		public string BuduciDrzitelAdresaMimoSR
		{
			get
			{
				return this.buduciDrzitelAdresaMimoSRField;
			}
			set
			{
				this.buduciDrzitelAdresaMimoSRField = value;
			}
		}

		public string BuduciDrzitelStatAdresyMimoSR
		{
			get
			{
				return this.buduciDrzitelStatAdresyMimoSRField;
			}
			set
			{
				this.buduciDrzitelStatAdresyMimoSRField = value;
			}
		}

	}

	public class BuduciDrzitel
	{

		private string buduciDrzitelMenoField;

		private string buduciDrzitelPriezviskoField;

		private System.DateTime buduciDrzitelDatumNarodeniaField;

		private string buduciDrzitelRodneCisloField;

		private string buduciDrzitelNazovField;

		private string buduciDrzitelICOField;

		private PobytSidloBuducehoDrzitela buduciDrzitelPobytField;

		public string BuduciDrzitelMeno
		{
			get
			{
				return this.buduciDrzitelMenoField;
			}
			set
			{
				this.buduciDrzitelMenoField = value;
			}
		}

		public string BuduciDrzitelPriezvisko
		{
			get
			{
				return this.buduciDrzitelPriezviskoField;
			}
			set
			{
				this.buduciDrzitelPriezviskoField = value;
			}
		}

		public System.DateTime BuduciDrzitelDatumNarodenia
		{
			get
			{
				return this.buduciDrzitelDatumNarodeniaField;
			}
			set
			{
				this.buduciDrzitelDatumNarodeniaField = value;
			}
		}

		public string BuduciDrzitelRodneCislo
		{
			get
			{
				return this.buduciDrzitelRodneCisloField;
			}
			set
			{
				this.buduciDrzitelRodneCisloField = value;
			}
		}

		public string BuduciDrzitelNazov
		{
			get
			{
				return this.buduciDrzitelNazovField;
			}
			set
			{
				this.buduciDrzitelNazovField = value;
			}
		}

		public string BuduciDrzitelICO
		{
			get
			{
				return this.buduciDrzitelICOField;
			}
			set
			{
				this.buduciDrzitelICOField = value;
			}
		}

		public PobytSidloBuducehoDrzitela BuduciDrzitelPobyt
		{
			get
			{
				return this.buduciDrzitelPobytField;
			}
			set
			{
				this.buduciDrzitelPobytField = value;
			}
		}

	}

	public class PobytSidloDrzitela
	{

		private string drzitelPobytTypPobytuField;

		private string drzitelPobytOkresField;

		private string drzitelPobytObecField;

		private string drzitelPobytUlicaField;

		private string drzitelPobytOrientacneCisloField;

		private string drzitelPobytSupisneCisloField;

		public string DrzitelPobytTypPobytu
		{
			get
			{
				return this.drzitelPobytTypPobytuField;
			}
			set
			{
				this.drzitelPobytTypPobytuField = value;
			}
		}

		public string DrzitelPobytOkres
		{
			get
			{
				return this.drzitelPobytOkresField;
			}
			set
			{
				this.drzitelPobytOkresField = value;
			}
		}

		public string DrzitelPobytObec
		{
			get
			{
				return this.drzitelPobytObecField;
			}
			set
			{
				this.drzitelPobytObecField = value;
			}
		}

		public string DrzitelPobytUlica
		{
			get
			{
				return this.drzitelPobytUlicaField;
			}
			set
			{
				this.drzitelPobytUlicaField = value;
			}
		}

		public string DrzitelPobytOrientacneCislo
		{
			get
			{
				return this.drzitelPobytOrientacneCisloField;
			}
			set
			{
				this.drzitelPobytOrientacneCisloField = value;
			}
		}

		public string DrzitelPobytSupisneCislo
		{
			get
			{
				return this.drzitelPobytSupisneCisloField;
			}
			set
			{
				this.drzitelPobytSupisneCisloField = value;
			}
		}

	}

	public class Drzitel
	{

		private string drzitelMenoField;

		private string drzitelPriezviskoField;

		private System.DateTime drzitelDatumNarodeniaField;

		private string drzitelRodneCisloField;

		private string drzitelNazovField;

		private string drzitelICOField;

		private PobytSidloDrzitela drzitelPobytField;

		public string DrzitelMeno
		{
			get
			{
				return this.drzitelMenoField;
			}
			set
			{
				this.drzitelMenoField = value;
			}
		}

		public string DrzitelPriezvisko
		{
			get
			{
				return this.drzitelPriezviskoField;
			}
			set
			{
				this.drzitelPriezviskoField = value;
			}
		}

		public System.DateTime DrzitelDatumNarodenia
		{
			get
			{
				return this.drzitelDatumNarodeniaField;
			}
			set
			{
				this.drzitelDatumNarodeniaField = value;
			}
		}

		public string DrzitelRodneCislo
		{
			get
			{
				return this.drzitelRodneCisloField;
			}
			set
			{
				this.drzitelRodneCisloField = value;
			}
		}

		public string DrzitelNazov
		{
			get
			{
				return this.drzitelNazovField;
			}
			set
			{
				this.drzitelNazovField = value;
			}
		}

		public string DrzitelICO
		{
			get
			{
				return this.drzitelICOField;
			}
			set
			{
				this.drzitelICOField = value;
			}
		}

		public PobytSidloDrzitela DrzitelPobyt
		{
			get
			{
				return this.drzitelPobytField;
			}
			set
			{
				this.drzitelPobytField = value;
			}
		}

	}

	public class Vozidlo
	{

		private string vINField;

		private string evidencneCisloField;

		private string kategoriaVozidlaField;

		private string druhVozidlaField;

		private string znackaField;

		private string farbaField;

		private string stavVozidlaField;

		private System.DateTime datumZmenyField;

		private Drzitel drzitelField;

		private BuduciDrzitel buduciDrzitelField;

		public string VIN
		{
			get
			{
				return this.vINField;
			}
			set
			{
				this.vINField = value;
			}
		}

		public string EvidencneCislo
		{
			get
			{
				return this.evidencneCisloField;
			}
			set
			{
				this.evidencneCisloField = value;
			}
		}

		public string KategoriaVozidla
		{
			get
			{
				return this.kategoriaVozidlaField;
			}
			set
			{
				this.kategoriaVozidlaField = value;
			}
		}

		public string DruhVozidla
		{
			get
			{
				return this.druhVozidlaField;
			}
			set
			{
				this.druhVozidlaField = value;
			}
		}

		public string Znacka
		{
			get
			{
				return this.znackaField;
			}
			set
			{
				this.znackaField = value;
			}
		}

		public string Farba
		{
			get
			{
				return this.farbaField;
			}
			set
			{
				this.farbaField = value;
			}
		}

		public string StavVozidla
		{
			get
			{
				return this.stavVozidlaField;
			}
			set
			{
				this.stavVozidlaField = value;
			}
		}

		public System.DateTime DatumZmeny
		{
			get
			{
				return this.datumZmenyField;
			}
			set
			{
				this.datumZmenyField = value;
			}
		}

		public Drzitel Drzitel
		{
			get
			{
				return this.drzitelField;
			}
			set
			{
				this.drzitelField = value;
			}
		}

		public BuduciDrzitel BuduciDrzitel
		{
			get
			{
				return this.buduciDrzitelField;
			}
			set
			{
				this.buduciDrzitelField = value;
			}
		}

	}

	public class Message
	{

		private string codeField;

		private string textField;

		public string Code
		{
			get
			{
				return this.codeField;
			}
			set
			{
				this.codeField = value;
			}
		}

		public string Text
		{
			get
			{
				return this.textField;
			}
			set
			{
				this.textField = value;
			}
		}

	}

	public class EVOResponse
	{

		private Message msgField;

		private Vozidlo vozidloField;

		private string i_fe_reg_logField;
		private string i_vozField;
		private string i_drzField;

		public Message Msg
		{
			get
			{
				return this.msgField;
			}
			set
			{
				this.msgField = value;
			}
		}

		public Vozidlo Vozidlo
		{
			get
			{
				return this.vozidloField;
			}
			set
			{
				this.vozidloField = value;
			}
		}

		public string I_fe_reg_log
		{
			get
			{
				return this.i_fe_reg_logField;
			}
			set
			{
				this.i_fe_reg_logField = value;
			}
		}
		public string I_voz
		{
			get
			{
				return this.i_vozField;
			}
			set
			{
				this.i_vozField = value;
			}
		}
		public string I_drz
		{
			get
			{
				return this.i_drzField;
			}
			set
			{
				this.i_drzField = value;
			}
		}
	}
}