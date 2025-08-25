using System;
using System.Runtime.Serialization;

namespace Cora.CommIss.Iss.CdoCto.ExtData
{
	public partial class CtoVW_ZM_POZ
	{
		/// <summary>Dokument k zmluve.</summary>
		[Serializable]
		[DataContract(Namespace = "http://www.corageo.sk/")]
		[Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptOut)]
		public partial class AttachVm
		{
			/// <summary>ID dokumentu.</summary>
			[DataMember(IsRequired = true, Name = "AttachID", Order = 1)]
			public int I_ZAZ_ZAZ { get; set; }

			/// <summary>Názov súboru.</summary>
			[DataMember(IsRequired = true, Name = "Filename", Order = 2)]
			public string Filename { get; set; }

			/// <summary>Typ súboru.</summary>
			[DataMember(IsRequired = true, Name = "Mimetype", Order = 3)]
			public string Mimetype { get; set; }

			/// <summary>Obsah prílohy v Base64.</summary>
			[DataMember(IsRequired = true, Name = "Content", Order = 4)]
			public byte[] Content { get; set; }
		}
	}
}