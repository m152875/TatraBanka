using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cora.CommIss.Iss.EVOClient;

namespace Cora.CommIss.Iss.EVO
{
	/// <summary>
	/// Trieda mapuje response privatnej sluzby a klienta EVO
	/// </summary>
	public static class ResponseMapper
	{
		/// <summary>
		/// Premapuje response z EVO na response privatnej sluzby
		/// </summary>
		/// <param name="response">Response klienta EVO</param>
		/// <returns>Response privatnej sluzby</returns>
		public static EVOResponse ToServiceResponse(poskytnutieUdajovOvozidleAdrziteloviResponse response)
		{
			EVOResponse ret = new EVOResponse();

			if ( response != null )
			{

				//prepis udajov vozidla
				if ( response.vozidlo != null )
				{
					ret.Vozidlo = new Vozidlo
					{
						DatumZmeny = new DateTime(response.vozidlo.DatumZmeny.Year, response.vozidlo.DatumZmeny.Month, response.vozidlo.DatumZmeny.Day),
						DruhVozidla = response.vozidlo.DruhVozidla,
						EvidencneCislo = response.vozidlo.EvidencneCislo,
						Farba = response.vozidlo.Farba,
						KategoriaVozidla = response.vozidlo.KategoriaVozidla,
						StavVozidla = response.vozidlo.StavVozidla,
						VIN = response.vozidlo.VIN,
						Znacka = response.vozidlo.Znacka
					};

					//prepis udajov drzitela
					if ( response.vozidlo.Drzitel != null )
					{
						ret.Vozidlo.Drzitel = new Drzitel
						{
							DrzitelDatumNarodenia = new DateTime(response.vozidlo.Drzitel.DrzitelDatumNarodenia.Year, response.vozidlo.Drzitel.DrzitelDatumNarodenia.Month, response.vozidlo.Drzitel.DrzitelDatumNarodenia.Day),
							DrzitelICO = response.vozidlo.Drzitel.DrzitelICO,
							DrzitelMeno = response.vozidlo.Drzitel.DrzitelMeno,
							DrzitelNazov = response.vozidlo.Drzitel.DrzitelNazov,
							DrzitelPriezvisko = response.vozidlo.Drzitel.DrzitelPriezvisko,
							DrzitelRodneCislo = response.vozidlo.Drzitel.DrzitelRodneCislo
						};

						if ( response.vozidlo.Drzitel.DrzitelPobyt != null )
						{
							ret.Vozidlo.Drzitel.DrzitelPobyt = new PobytSidloDrzitela
							{
								DrzitelPobytObec = response.vozidlo.Drzitel.DrzitelPobyt.DrzitelPobytObec,
								DrzitelPobytOkres = response.vozidlo.Drzitel.DrzitelPobyt.DrzitelPobytOkres,
								DrzitelPobytOrientacneCislo = response.vozidlo.Drzitel.DrzitelPobyt.DrzitelPobytOrientacneCislo,
								DrzitelPobytSupisneCislo = response.vozidlo.Drzitel.DrzitelPobyt.DrzitelPobytSupisneCislo,
								DrzitelPobytTypPobytu = response.vozidlo.Drzitel.DrzitelPobyt.DrzitelPobytTypPobytu,
								DrzitelPobytUlica = response.vozidlo.Drzitel.DrzitelPobyt.DrzitelPobytUlica
							};
						}
					}

					//prepis udajov buduceho drzitela
					if ( response.vozidlo.BuduciDrzitel != null )
					{
						ret.Vozidlo.BuduciDrzitel = new BuduciDrzitel
						{
							BuduciDrzitelDatumNarodenia = new DateTime(response.vozidlo.BuduciDrzitel.BuduciDrzitelDatumNarodenia.Year, response.vozidlo.BuduciDrzitel.BuduciDrzitelDatumNarodenia.Month, response.vozidlo.BuduciDrzitel.BuduciDrzitelDatumNarodenia.Day),
							BuduciDrzitelICO = response.vozidlo.BuduciDrzitel.BuduciDrzitelICO,
							BuduciDrzitelMeno = response.vozidlo.BuduciDrzitel.BuduciDrzitelMeno,
							BuduciDrzitelNazov = response.vozidlo.BuduciDrzitel.BuduciDrzitelNazov,
							BuduciDrzitelPriezvisko = response.vozidlo.BuduciDrzitel.BuduciDrzitelPriezvisko,
							BuduciDrzitelRodneCislo = response.vozidlo.BuduciDrzitel.BuduciDrzitelRodneCislo
						};

						if ( response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt != null )
						{
							ret.Vozidlo.BuduciDrzitel.BuduciDrzitelPobyt = new PobytSidloBuducehoDrzitela
							{
								BuduciDrzitelAdresaMimoSR = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelAdresaMimoSR,
								BuduciDrzitelPobytObec = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelPobytObec,
								BuduciDrzitelPobytOkres = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelPobytOkres,
								BuduciDrzitelPobytOrientacneCislo = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelPobytOrientacneCislo,
								BuduciDrzitelPobytSupisneCislo = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelPobytSupisneCislo,
								BuduciDrzitelPobytTypPobytu = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelPobytTypPobytu,
								BuduciDrzitelPobytUlica = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelPobytUlica,
								BuduciDrzitelStatAdresyMimoSR = response.vozidlo.BuduciDrzitel.BuduciDrzitelPobyt.BuduciDrzitelStatAdresyMimoSR
							};
						}
					}
				}

				//prepis spravy
				if ( response.msg != null )
				{
					ret.Msg = new Message
					{
						Code = response.msg.code,
						Text = response.msg.text
					};
				}

			}

			return ret;
		}
	}
}