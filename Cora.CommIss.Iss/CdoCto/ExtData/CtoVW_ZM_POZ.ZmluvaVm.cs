using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cora.CommIss.Iss.CdoCto.ExtData
{
	public partial class CtoVW_ZM_POZ
	{
		/// <summary>Zmluva.</summary>
		[Serializable]
		[DataContract(Namespace = "http://www.corageo.sk/")]
		[Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
		public partial class ZmluvaVm
		{
			/// <summary>ID zmluvy.</summary>
			[DataMember(IsRequired = true, Name = "ZmluvaID", Order = 1)]
			public int I_ZM { get; set; }

			/// <summary>Predmety zmluvy.</summary>
			[DataMember(IsRequired = true, Name = "PredmetyZmluvy", Order = 2)]
			public IEnumerable<PredmetVm> Predmety { get; set; }

			/// <summary>Zoznam zmluvných strán (okrem mesta).</summary>
			[DataMember(IsRequired = false, Name = "ZmluvneStrany", Order = 3)]
			public IEnumerable<string> ZML_STRANY { get; set; }

			/// <summary>Dokumenty k zmluve.</summary>
			[DataMember(IsRequired = false, Name = "Dokumenty", Order = 4)]
			public IEnumerable<AttachVm> Dokumenty { get; set; }
		}
	}
}