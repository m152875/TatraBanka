using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace Cora.CommIss.Iss
{
	/// <summary>
	/// Vnútorné rozhranie pre prácu s ISS TLZ Archívom.
	/// </summary>
	[ServiceContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public interface ITLZProvidePrivate
	{
		[OperationContract]
		TLZFile GetGenPDF(int pnI_tl_dat, int pnI_uz);
	}

	/// <summary>
	/// Súbor vo formáte Base64 s doplnkovými informáciami.
	/// </summary>
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class TLZFile
	{
		/// <summary>
		/// Názov súboru.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Obsah súboru vo formáte Base64.
		/// </summary>
		[DataMember]
		public string Content { get; set; }

		/// <summary>
		/// Veľkosť súboru.
		/// </summary>
		[DataMember]
		public long Length { get; set; }

		/// <summary>
		/// Prípona súboru.
		/// </summary>
		[DataMember]
		public string Extension { get; set; }

		/// <summary>
		/// Veľkosť súboru.
		/// </summary>
		[DataMember]
		public bool Success { get; set; }

		/// <summary>
		/// Prípona súboru.
		/// </summary>
		[DataMember]
		public string ErrorMsg { get; set; }
	}
}