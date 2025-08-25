using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Cora.CoraCoreObj;
using Cora.DMS;

namespace Cora.CommIss.Iss.DMS
{
	/// <summary>
	/// Vonkajšie rozhranie pre prácu s DMS.
	/// </summary>
	[ServiceContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	[RequiresValidConnId]
	public interface IDMSProvide
	{
		[OperationContract]
		int InsertFile(string fileName, string extension, string mimeType, byte[] content);

		[OperationContract]
		int UpdateFile(int iFile, string fileName, string extension, string mimeType, byte[] content);

		[OperationContract]
		int DeleteFile(int iFile);

		[OperationContract]
		GetFileByFileIdRes GetFileByFileId(int iFile);
	}
}
