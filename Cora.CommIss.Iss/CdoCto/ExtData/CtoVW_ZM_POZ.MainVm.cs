using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cora.CommIss.Iss.CdoCto.ExtData
{
	public partial class CtoVW_ZM_POZ
	{
		[Serializable]
		[DataContract(Namespace = "http://www.corageo.sk/")]
		[Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
		public partial class MainVm
		{
			/// <summary>Identifikátor požadovanej parcely.</summary>
			[DataMember(IsRequired = true, Name = "IdentifikatorPozadovanejParcely", Order = 1)]
			public string IDENTIF { get; set; }

			/// <summary>Zmluvy.</summary>
			[DataMember(IsRequired = true, Name = "Zmluvy", Order = 2)]
			public IEnumerable<ZmluvaVm> Zmluvy { get; set; }
		}
	}
}