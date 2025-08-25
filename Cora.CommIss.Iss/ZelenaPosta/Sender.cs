using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Cora.CommIss.Iss.ZelenaPosta
{
	/// <summary>
	/// Odosielateľ pošty.
	/// </summary>
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class Sender
	{
		/// <summary>
		/// Meno/Názov.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Ulica.
		/// </summary>
		[DataMember]
		public string Street { get; set; }

		/// <summary>
		/// Mesto.
		/// </summary>
		[DataMember]
		public string City { get; set; }

		/// <summary>
		/// PSC.
		/// </summary>
		[DataMember]
		public string Zip { get; set; }

		/// <summary>
		/// Krajina.
		/// </summary>
		[DataMember]
		public string Country { get; set; }
	}
}