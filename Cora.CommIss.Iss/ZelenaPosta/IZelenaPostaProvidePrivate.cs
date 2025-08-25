using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Cora.CommIss.Iss.ZelenaPosta
{
	/// <summary>
	/// Vnútorné rozhranie pre prácu s ISS TLZ Archívom.
	/// </summary>
	[ServiceContract(Namespace = "http://www.corageo.sk/schemas/CommIss")]
	public interface IZelenaPostaProvidePrivate
	{
		/// <summary>
		/// Služba na odoslanie cez API 1 Zelenej pošty.
		/// </summary>
		[OperationContract]
		ZelenaPostaResponse Send(string title, string product, byte[] file, Recipient recipient, Sender sender);

		/// <summary>
		/// Služba na odoslanie cez API 2 Zelenej pošty.
		/// </summary>
		[OperationContract]
		ZelenaPostaResponse Send2(string title, string product, byte[][] files, Recipient recipient, Sender sender, string IDRegZaznamu, string CisloKonania);

		/// <summary>
		/// Služba na odoslanie cez API 2 Zelenej pošty.
		/// </summary>
		[OperationContract]
		ZelenaPostaResponse SendHromadne(string title, MailingItem[] items);

		/// <summary>
		/// Služba na aktualizáciu status zásielok cez API 2 Zelenej pošty.
		/// </summary>
		[OperationContract]
		void GetMailingsStatus(string slotId = null);

		/// <summary>
		/// Služba Zruší odoslané zásielky. Zásielky je možné zrušiť, len ak ešte neboli odoslané na tlač a len ak existujú.
		/// </summary>
		[OperationContract]
		void CancelMailings(string slotId = null);

		/// <summary>
		/// Získa aktuálny status zásielky.
		/// </summary>
		/// <param name="slotId">The slot identifier.</param>
		/// <returns>Posledný aktuálny status zásielky.</returns>
		[OperationContract]
		ZelenaPostaStatus.MailingStates GetStatus(string slotId);

		/// <summary>
		/// Získa aktuálny Zoznam produktov zelenej posty
		/// </summary>
		[OperationContract]
		ZelenaPostaAPI2ProductRequireClient.productInfo[] GetProducts();

		/// <summary>
		/// Služba na odoslanie cez API 1 Zelenej pošty (Compat).
		/// </summary>
		/// <param name="title"></param>
		/// <param name="product"></param>
		/// <param name="file"></param>
		/// <param name="recName"></param>
		/// <param name="recStreet"></param>
		/// <param name="recCity"></param>
		/// <param name="recZip"></param>
		/// <param name="recCountry"></param>
		/// <param name="sndName"></param>
		/// <param name="sndStreet"></param>
		/// <param name="sndCity"></param>
		/// <param name="sndZip"></param>
		/// <param name="sndCountry"></param>
		/// <returns></returns>
		[OperationContract]
		byte[] SendCompat(string title, string product, string file,
			string recName, string recStreet, string recCity, string recZip, string recCountry,
			string sndName, string sndStreet, string sndCity, string sndZip, string sndCountry);

		/// <summary>
		/// Služba na odoslanie cez API 2 Zelenej pošty (Compat).
		/// </summary>
		/// <param name="title"></param>
		/// <param name="product"></param>
		/// <param name="file"></param>
		/// <param name="recName"></param>
		/// <param name="recStreet"></param>
		/// <param name="recCity"></param>
		/// <param name="recZip"></param>
		/// <param name="recCountry"></param>
		/// <param name="sndName"></param>
		/// <param name="sndStreet"></param>
		/// <param name="sndCity"></param>
		/// <param name="sndZip"></param>
		/// <param name="sndCountry"></param>
		/// <param name="IDRegZaznamu"></param>
		/// <param name="CisloKonania"></param> 
		/// <returns></returns>
		[OperationContract]
		byte[] Send2Compat(string title, string product, string file,
			string recName, string recStreet, string recCity, string recZip, string recCountry,
			string sndName, string sndStreet, string sndCity, string sndZip, string sndCountry, string IDRegZaznamu, string CisloKonania);
	}
}
