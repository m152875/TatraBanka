using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Cora.DMS;

namespace Cora.CommIss.Iss.DMS
{
	[DataContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public class GetFileByFileIdRes
	{
		public GetFileByFileIdRes()
		{
		}

		public GetFileByFileIdRes(int code, CtoC_FILE_DMS file)
		{
			Code = code;
			File = file;
		}

		[DataMember]
		public int Code { get; set; }

		[DataMember]
		public CtoC_FILE_DMS File { get; set; }
	}
}