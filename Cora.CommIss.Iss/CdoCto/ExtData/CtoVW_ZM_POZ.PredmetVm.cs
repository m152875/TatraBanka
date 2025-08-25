using System;
using System.Runtime.Serialization;

namespace Cora.CommIss.Iss.CdoCto.ExtData
{
	public partial class CtoVW_ZM_POZ
	{
		/// <summary>Predmet zmluvy.</summary>
		[Serializable]
		[DataContract(Namespace = "http://www.corageo.sk/")]
		[Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
		public partial class PredmetVm
		{
			/// <summary>Primárny klúč - rownum.</summary>
			[DataMember(IsRequired = true, Name = "ID", Order = 1)]
			public long I_ZM_POZ { get; set; }

			/// <summary>ID predmetu zmluvy.</summary>
			[DataMember(IsRequired = true, Name = "PredmetID", Order = 2)]
			public int I_ZM_PREDM { get; set; }

			/// <summary>ID parcely.</summary>
			[DataMember(IsRequired = true, Name = "ParcelaID", Order = 3)]
			public int I_PA { get; set; }

			/// <summary>Identifikátor parcely.</summary>
			[DataMember(IsRequired = true, Name = "IdentifikatorParcely", Order = 4)]
			public string IDENTIF { get; set; }

			/// <summary>Číslo parcely.</summary>
			[DataMember(IsRequired = true, Name = "CisloParcely", Order = 5)]
			public string CP { get; set; }

			/// <summary>Celková výmera parcely.</summary>
			[DataMember(IsRequired = false, Name = "CelkovaVymera", Order = 6)]
			public decimal? VYM { get; set; }

			/// <summary>Prenajatá výmera parcely.</summary>
			[DataMember(IsRequired = false, Name = "PrenajataVymera", Order = 7)]
			public decimal? VYM_SKUT { get; set; }

			/// <summary>Cena za jednotku - z predmetu zmluvy.</summary>
			[DataMember(IsRequired = false, Name = "JenotkovaCena", Order = 8)]
			public decimal? SK_M2 { get; set; }

			/// <summary>Typ nehnuteľného majetku.</summary>
			[DataMember(IsRequired = true, Name = "TypMajetku", Order = 9)]
			public string N_NM_TYP { get; set; }

			/// <summary>Stavba.</summary>
			[DataMember(IsRequired = true, Name = "Stavba", Order = 10)]
			public StavbaVm Stavba { get; set; }
		}
	}
}