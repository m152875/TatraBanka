using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Cora.CommIss.Iss.ZelenaPosta
{
	/// <summary>
	/// Pre hromadne odosielanie do Zelenej posty
	/// </summary>
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class MailingItem
	{
		[DataMember]
		/// <summary>
		/// Adresat
		/// </summary>
		public Recipient recipient;

		[DataMember]
		/// <summary>
		/// Odosielate;
		/// </summary>
		public Sender sender;

		[DataMember]
		/// <summary>
		/// produkt zel. posty - typ zasielky
		/// </summary>
		public string product;

		[DataMember]
		/// <summary>
		/// Subor, binarny
		/// </summary>
		public byte[][] files;

		[DataMember]
		/// <summary>
		/// IDRegZaznamu
		/// </summary>
		public string IDRegZaznamu;

		[DataMember]
		/// <summary>
		/// CisloKonania
		/// </summary>
		public string CisloKonania;
	}
}