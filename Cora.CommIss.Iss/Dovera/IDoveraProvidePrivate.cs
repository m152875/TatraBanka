using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Cora.CommIss.Iss.DoveraPoistenecClient;
using Cora.CommIss.Iss.DoveraPlatitelClient;
using Cora.CommIss.Iss.DoveraSystemClient;
using Cora.CommIss.Iss.DoveraHromadneOznamenieClient;
using Cora.CommIss.Iss.DoveraMesacnyVykazClient;

namespace Cora.CommIss.Iss.Dovera
{
	/// <summary>
	/// Rozhranie poskytujuce pristup k webovym sluzbam ZP Dovera - Zamestnavatelia online
	/// </summary>
	[ServiceContract]
	public interface IDoveraProvidePrivate
	{
		/// <summary>
		/// Dajs the stav systemu private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<DajStavSystemuVystup> DajStavSystemuPrivate(DajStavSystemuVstup request, string Login, string Password);
		/// <summary>
		/// Dajs the stav systemu private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] DajStavSystemuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA);
		/// <summary>
		/// Overs the poistenca private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>

		[OperationContract]
		DoveraExtResponse<DajPrihlasovaciTokenVystup> DajPrihlasovaciTokenPrivate(DajPrihlasovaciTokenVstup request, string Login, string Password);

		[OperationContract]
		byte[] DajPrihlasovaciTokenPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA);

		[OperationContract]
		DoveraExtResponse<PrihlasenieTokenomVystup> PrihlasenieTokenomPrivate(PrihlasenieTokenomVstup request, string Login, string Password);

		[OperationContract]
		byte[] PrihlasenieTokenomPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string PrihlasovaciToken, string StrankaPoPrihlaseni, string FFilter);

		[OperationContract]
		DoveraExtResponse<DajOznamyDZPVystup> DajOznamyDZPPrivate(DajOznamyDZPVstup request, string Login, string Password);
		/// <summary>
		/// Dajs the oznamy DZP compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="Forma">The forma.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] DajOznamyDZPPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int Forma);

		[OperationContract]
		DoveraExtResponse<OverPoistencaVystup> OverPoistencaPrivate(OverPoistencaVstup request, string Login, string Password);
		/// <summary>
		/// Overs the poistenca private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="RC">The rc.</param>
		/// <param name="ICP">The icp.</param>
		/// <param name="Datum">The datum.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] OverPoistencaPrivateCompat(string Login, string Password, string RC, string ICP, string Datum);
		/// <summary>
		/// Overs the oop private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<OverOOPVystup> OverOOPPrivate(OverOOPVstup request, string Login, string Password);		
		[OperationContract]
		/// <summary>
		/// Overs the oop private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="RC">The rc.</param>
		/// <param name="ICP">The icp.</param>
		/// <param name="RozhodnyDen">The rozhodny den.</param>
		/// <returns></returns>
		/// byte[] OverOOPPrivateCompat(string request);
		byte[] OverOOPPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string RC, string ICP, string RozhodnyDen);

		[OperationContract]
		DoveraExtResponse<DajStavUctuVystup> DajStavUctuPrivate(DajStavUctuVstup request, string Login, string Password);

		[OperationContract]
		byte[] DajStavUctuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA);

		/// <summary>
		/// Otestujs the hromadne oznamenie private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<OtestujHromadneOznamenieVystup> OtestujHromadneOznameniePrivate(OtestujHromadneOznamenieVstup request, string Login, string Password);
		/// <summary>
		/// Otestujs the hromadne oznamenie private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetRiadkov">The pocet riadkov.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] OtestujHromadneOznameniePrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string Typ, int Obdobie, int PocetRiadkov, int IdExterne, string Nazov, string Obsah);
		/// <summary>
		/// Dajs the stav otestovania hromadneho oznamenia private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<DajStavOtestovaniaHromadnehoOznameniaVystup> DajStavOtestovaniaHromadnehoOznameniaPrivate(DajStavOtestovaniaHromadnehoOznameniaVstup request, string Login, string Password);
		/// <summary>
		/// Dajs the stav otestovania hromadneho oznamenia private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] DajStavOtestovaniaHromadnehoOznameniaPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania);
		/// <summary>
		/// Dajs the stav odoslania hromadneho oznamenia private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<DajStavOdoslaniaHromadnehoOznameniaVystup> DajStavOdoslaniaHromadnehoOznameniaPrivate(DajStavOdoslaniaHromadnehoOznameniaVstup request, string Login, string Password);
		/// <summary>
		/// Dajs the stav odoslania hromadneho oznamenia private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] DajStavOdoslaniaHromadnehoOznameniaPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania);
		/// <summary>
		/// Odoslis the hromadne oznamenie private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<OdosliHromadneOznamenieVystup> OdosliHromadneOznameniePrivate(OdosliHromadneOznamenieVstup request, string Login, string Password);
		/// <summary>
		/// Odoslis the hromadne oznamenie private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetRiadkov">The pocet riadkov.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] OdosliHromadneOznameniePrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string Typ, int Obdobie, int PocetRiadkov, int IdExterne, string Nazov, string Obsah);
		/// <summary>
		/// Odoslis the mesacny vykaz private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<OdosliMesacnyVykazVystup> OdosliMesacnyVykazPrivate(OdosliMesacnyVykazVstup request, string Login, string Password);
		/// <summary>
		/// Odoslis the mesacny vykaz private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="SumaPreddavkov">The suma preddavkov.</param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetViet">The pocet viet.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] OdosliMesacnyVykazPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, decimal SumaPreddavkov, string Typ, int Obdobie, int PocetViet, int IdExterne, string Nazov, string Obsah);
		/// <summary>
		/// Dajs the stav odoslania mesacneho vykazu private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<DajStavOdoslaniaMesacnehoVykazuVystup> DajStavOdoslaniaMesacnehoVykazuPrivate(DajStavOdoslaniaMesacnehoVykazuVstup request, string Login, string Password);
		/// <summary>
		/// Dajs the stav odoslania mesacneho vykazu private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] DajStavOdoslaniaMesacnehoVykazuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania);
		/// <summary>
		/// Dajs the stav otestovania mesacneho vykazu private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]
		DoveraExtResponse<DajStavOtestovaniaMesacnehoVykazuVystup> DajStavOtestovaniaMesacnehoVykazuPrivate(DajStavOtestovaniaMesacnehoVykazuVstup request, string Login, string Password);
		/// <summary>
		/// Dajs the stav otestovania mesacneho vykazu private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="IDVolania">The identifier volania.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] DajStavOtestovaniaMesacnehoVykazuPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, int IDVolania);
		/// <summary>
		/// Otestujs the mesacny vykaz private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		[OperationContract]

		DoveraExtResponse<OtestujMesacnyVykazVystup> OtestujMesacnyVykazPrivate(OtestujMesacnyVykazVstup request, string Login, string Password);
		/// <summary>
		/// Otestujs the mesacny vykaz private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="SumaPreddavkov">The suma preddavkov.</param>
		/// <param name="Typ">The typ.</param>
		/// <param name="Obdobie">The obdobie.</param>
		/// <param name="PocetViet">The pocet viet.</param>
		/// <param name="IdExterne">The identifier externe.</param>
		/// <param name="Nazov">The nazov.</param>
		/// <param name="Obsah">The obsah.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] OtestujMesacnyVykazPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, decimal SumaPreddavkov, string Typ, int Obdobie, int PocetViet, int IdExterne, string Nazov, string Obsah);

		/// <summary>
		/// Dajs the zoznam zamestnancov private.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <returns></returns>
		DoveraExtResponse<DajZoznamZamestnancovVystup> DajZoznamZamestnancovPrivate(DajZoznamZamestnancovVstup request, string Login, string Password);
		/// <summary>
		/// Dajs the zoznam zamestnancov private compat.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
		/// <param name="VS">The vs.</param>
		/// <param name="UnikatnyKodPLA">The unikatny kod pla.</param>
		/// <param name="DatumOd">The datum od.</param>
		/// <param name="DatumDo">The datum do.</param>
		/// <param name="IdKategorieOd">The identifier kategorie od.</param>
		/// <param name="RodneCisla">The rodne cisla.</param>
		/// <returns></returns>
		[OperationContract]
		byte[] DajZoznamZamestnancovPrivateCompat(string Login, string Password, string VS, string UnikatnyKodPLA, string DatumOd, string DatumDo, int IdKategorieOd, string RodneCisla);
	}
}