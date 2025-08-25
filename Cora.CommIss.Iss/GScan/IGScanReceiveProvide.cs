using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Cora.CommIss.Iss.GScan
{
	/// <summary>
	/// Rozhranie pre prijem dokumentov zo skenovacieho modulu GScan (spoločnosť Gradient).
	/// </summary>
	[ServiceContract]
	public interface IGScanReceiveProvide
	{
		[OperationContract]
		int ReceiveDocuments(ScannedMessage message);
	}
}
