using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Cora.CommIss.Iss.GScan
{
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class PersonWithAddress
	{
		/// <summary>
		/// Titul pred menom.
		/// </summary>
		[DataMember]
		public string TitleFront { get; set; }

		/// <summary>
		/// Meno.
		/// </summary>
		[DataMember]
		public string FirstName { get; set; }

		/// <summary>
		/// Priezvisko.
		/// </summary>
		[DataMember]
		public string LastName { get; set; }

		/// <summary>
		/// Titul za menom.
		/// </summary>
		[DataMember]
		public string TitleAfter { get; set; }

		/// <summary>
		/// Ulica.
		/// </summary>
		[DataMember]
		public string StreetName { get; set; }

		/// <summary>
		/// Číslo súpisné.
		/// </summary>
		[DataMember]
		public int StreetInventoryNm { get; set; }

		/// <summary>
		/// Číslo orientačné.
		/// </summary>
		[DataMember]
		public string StreetReferenceNm { get; set; }

		/// <summary>
		/// Mesto/obec.
		/// </summary>
		[DataMember]
		public string City { get; set; }

		/// <summary>
		/// PSČ obce.
		/// </summary>
		[DataMember]
		public string Zip { get; set; }

		/// <summary>
		/// Mestska časť obce.
		/// </summary>
		[DataMember]
		public string District { get; set; }

		/// <summary>
		/// Názov firmy.
		/// </summary>
		[DataMember]
		public string CompanyName { get; set; }

		/// <summary>
		/// IČO firmy.
		/// </summary>
		[DataMember]
		public int CompanyNumber { get; set; }
	}
}