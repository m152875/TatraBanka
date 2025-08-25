using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Cora.CommIss.Iss.GScan
{
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class ScannedMessage
	{
		/// <summary>
		/// Spôsob doručenia.
		/// </summary>
		[DataMember]
		public int DeliveryMethod { get; set; }

		/// <summary>
		/// Typ a druh.
		/// </summary>
		[DataMember]
		public int TypeAndKind { get; set; }

		/// <summary>
		/// Vec.
		/// </summary>
		[DataMember]
		public string Subject { get; set; }

		/// <summary>
		/// Vybaviť do dátumu.
		/// </summary>
		[DataMember]
		public DateTime ProcessToDate { get; set; }

		/// <summary>
		/// Odosielateľ aj s adresou.
		/// </summary>
		[DataMember]
		public PersonWithAddress SenderWithAddress { get; set; }

		/// <summary>
		/// Adresát  aj s adresou.
		/// </summary>
		[DataMember]
		public PersonWithAddress RecipientWithAddress { get; set; }

		/// <summary>
		/// Stredisko.
		/// </summary>
		[DataMember]
		public string UserCenter { get; set; }

		/// <summary>
		/// Zoznam priložených súborov.
		/// </summary>
		[DataMember]
		public IEnumerable<ScannedFile> Files { get; set; }

		/// <summary>
		/// Pošta prijatá alebo odosielaná. Default bude 1=prijatá / 0=odosielaná.
		/// </summary>
		[DataMember]
		public MessageType MsgType { get; set; }

		/// <summary>
		/// Dátum doručenia písomnosti do úradu.
		/// </summary>
		[DataMember]
		public DateTime DeliveryDate { get; set; }

		/// <summary>
		/// Značka odosielateľa.
		/// </summary>
		[DataMember]
		public string SenderSign { get; set; }

		/// <summary>
		/// Poznámka.
		/// </summary>
		[DataMember]
		public string Description { get; set; }
	}

	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public enum MessageType
	{
		[EnumMember(Value = "1")] //prehlasujeme za Default, preto 1
		Received,
		[EnumMember(Value = "0")]
		Sent
	}
}
