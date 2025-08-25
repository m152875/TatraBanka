using System;
using System.Runtime.Serialization;

namespace Cora.CommIss.Iss.CdoCto.ExtData
{
	public partial class CtoVW_ZM_POZ
	{
		/// <summary>Informácie o stavbe.</summary>
		[Serializable]
		[DataContract(Namespace = "http://www.corageo.sk/")]
		[Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
		public partial class StavbaVm
		{
			/// <summary>ID stavby.</summary>
			[DataMember(IsRequired = true, Name = "StavbaID", Order = 1)]
			public int I_ST { get; set; }

			/// <summary>ID adresy stavby.</summary>
			[DataMember(IsRequired = true, Name = "AdresaID", Order = 2)]
			public int I_ADR { get; set; }

			/// <summary>Názov mesta.</summary>
			[DataMember(IsRequired = false, Name = "Obec", Order = 3)]
			public string N_M { get; set; }

			/// <summary>Súpisné číslo.</summary>
			[DataMember(IsRequired = false, Name = "SupisneCislo", Order = 4)]
			public string CS { get; set; }

			/// <summary>Názov ulice.</summary>
			[DataMember(IsRequired = false, Name = "Ulica", Order = 5)]
			public string N_U { get; set; }

			/// <summary>Orientačné číslo.</summary>
			[DataMember(IsRequired = false, Name = "OrientacneCislo", Order = 6)]
			public string CO { get; set; }
		}
	}
}