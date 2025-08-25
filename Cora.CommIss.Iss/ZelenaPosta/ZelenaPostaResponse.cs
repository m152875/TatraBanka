using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Cora.CommIss.LEForm;
using Cora.CommIss.Podania;


namespace Cora.CommIss.Iss.ZelenaPosta
{
	/// <summary>
	/// Objekt odpovede Zelenej pošty.
	/// </summary>
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class ZelenaPostaResponse
	{
		/// <summary>
		/// ID zásielky.
		/// </summary>
		[DataMember]
		public string SlotId { get; set; }

		/// <summary>
		/// Status kod.
		/// </summary>
		[DataMember]
		public int Status { get; set; }
	}
}
