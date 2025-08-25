using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Cora.CommIss.Iss.GScan
{
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class ScannedFile
	{
		/// <summary>
		/// Stredisko.
		/// </summary>
		[DataMember]
		public byte[] Content { get; set; }

		/// <summary>
		/// Názov.
		/// </summary>
		[DataMember]
		public string Filename { get; set; }

		/// <summary>
		/// MimeType.
		/// </summary>
		[DataMember]
		public string MimeType { get; set; }

		/// <summary>
		/// Veľkosť súboru.
		/// </summary>
		[DataMember]
		public string Length { get; set; }

		/// <summary>
		/// Príznak, či ide o hlavný súbor.
		/// </summary>
		[DataMember]
		public bool IsMainFile { get; set; }
	}
}