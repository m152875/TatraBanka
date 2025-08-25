using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cora.CommIss.Iss.ZelenaPosta
{
	public static class ZelenaPostaStatus
	{
		/// <summary>OK.</summary>
		public const int OK = 0;

		/// <summary>Chybajuci parameter.</summary>
		public const int MISSING_PARAM = -1;

		/// <summary>Chyba pri odosielaní pošty.</summary>
		public const int SEND_ERROR = -2;

		/// <summary>Odpoved volania je prazdna.</summary>
		public const int EMPTY_RESPONSE = -3;

		public enum MailingStates
		{
			Created = 1, //Zásielka bola vytvorená.
			Failed = 4, //Spracovanie zásielky zlyhalo.
			Canceled = 5, //Zásielka bola zrušená.
			Paused = 6, //Spracovanie zásielky bolo manuálne pozdržané.
			Ignored = 7, //Zásielka je ignorovaná.
			ProviderRequested = 8, //Zásielka bola odoslaná na tlač.
			ProviderReceived = 9, //Zásielka bola prijatá u tlačového poskytovateľa
			ProviderPrinted = 10, //Zásielka bola vytlačená.
			ProviderSent = 11, //Zásielka bola podaná na pošte.
			ProviderResent = 12, //Zásielka bola opätovne podaná na pošte.
			ProviderDelivered = 13, //Zásielka bola doručená.

			CgSent = 99, //Zásielka bola odoslaná na rozhranie API Zelenej Pošty.
			CgError = -1
		}

	}
}
