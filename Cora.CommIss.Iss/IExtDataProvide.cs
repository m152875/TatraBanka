using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Cora.CoraApp.ISS.EXT.MAT;
using Cora.CoraCoreObj;
using Cora.Web.Wcf;

namespace Cora.CommIss.Iss
{
	/// <summary>
	/// Vonkajšie rozhranie pre poskytovanie dát.
	/// </summary>
	[ServiceContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	[RequiresValidConnId]
	[BulkOperationWsdl]
	public interface IExtDataProvide
	{
		#region json (navratovy typ)

		[OperationContract]
		[BulkOperation]
		string GetMET_C_CM_json(CtoMET_C_CM metCcm);

		[OperationContract]
		[BulkOperation]
		string GetMET_C_U_json(CtoMET_C_U metCu);

		[OperationContract]
		[BulkOperation]
		string GetMET_C_PA_MM_json(CtoMET_C_PA_MM metCpamm);

		[OperationContract]
		[BulkOperation]
		string GetMET_PO_PES_json(CtoMET_PO_PES metPopes);

		[OperationContract]
		[BulkOperation]
		string GetMET_DN_POZ_json(CtoMET_DN_POZ metDnpoz);

		/// <summary>Získanie informácií o zmluvách k požadovanej parcele vo formáte JSON.</summary>
		/// <param name="identifikatorPozadovanejParcely">Identifikátor požadovanej parcely.</param>
		/// <returns>Informácie o zmluvách k požadovanej parcele (objekt JSON).</returns>
		[OperationContract]
		string GetZmluvyKParcele_json(string identifikatorPozadovanejParcely);

		//[OperationContract]
		CdoCto.ExtData.CtoVW_ZM_POZ.MainVm GetZmluvyKParcele(string identifikatorPozadovanejParcely);

		#endregion

		#region originalne WCF datatype metody
		/*
		[OperationContract]
		[BulkOperation]
		CtoMET_C_CM[] GetMET_C_CM(CtoMET_C_CM metCcm);

		[OperationContract]
		[BulkOperation]
		CtoMET_C_U[] GetMET_C_U(CtoMET_C_U metCu);

		[OperationContract]
		[BulkOperation]
		CtoMET_C_PA_MM[] GetMET_C_PA_MM(CtoMET_C_PA_MM metCpamm);

		[OperationContract]
		[BulkOperation]
		CtoMET_PO_PES[] GetMET_PO_PES(CtoMET_PO_PES metPopes);
		*/
		#endregion
	}
}