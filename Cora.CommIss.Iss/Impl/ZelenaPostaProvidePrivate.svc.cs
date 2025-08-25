using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using System.Xml;
using Cora.CommIss.Iss.ZelenaPosta;
using Cora.Convert;
using Cora.Data.DBProvider;
using Cora.Natec.Base;
using Cora.CommIss.Iss.ZelenaPostaAPI2RequireClient;
using Cora.CommIss.Iss.ZelenaPostaRequireClient;

namespace Cora.CommIss.Iss.Impl
{
	public class ZelenaPostaProvidePrivate : IZelenaPostaProvidePrivate
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="title"></param>
		/// <param name="product"></param>
		/// <param name="file"></param>
		/// <param name="recipient"></param>
		/// <param name="sender"></param>
		/// <returns></returns>
		public ZelenaPostaResponse Send(string title, string product, byte[] file, ZelenaPosta.Recipient recipient, ZelenaPosta.Sender sender)
		{
			ZelenaPostaResponse res = new ZelenaPostaResponse();
			ZelenaPostaRequireClient.addressType sndr = null;
			CdoZPO_MAILING cdoMailing = new CdoZPO_MAILING();

			if ( null == recipient || null == file || file.Count() == 0 )
			{
				res.Status = ZelenaPostaStatus.MISSING_PARAM;

				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("ZelenaPostaProvidePrivate.Send: Odosielateľ alebo príloha nemôže byť prázdne!"));

				return res;
			}

			if ( null != sender )
			{
				sndr = new ZelenaPostaRequireClient.addressType()
				{
					name = sender.Name,
					street = sender.Street,
					city = sender.City,
					zip = sender.Zip,
					country = sender.Country
				};
			}

			try
			{
				using ( ZelenaPostaRequireClient.MailingClient client = createAndconfigureEndpoint<ZelenaPostaRequireClient.MailingClient>() )
				{
					//Basic Autentifikacia http (bez SSL), nutne pridat rucne Authorization header do http request contextu
					var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
					httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " +
						System.Convert.ToBase64String(Encoding.ASCII.GetBytes(client.ClientCredentials.UserName.UserName + ":" + client.ClientCredentials.UserName.Password));

					var context = new System.ServiceModel.OperationContext(client.InnerChannel);
					using ( new System.ServiceModel.OperationContextScope(context) )
					{
						context.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;

						//client
						ZelenaPostaRequireClient.sendResponse sendRes = client.send(new ZelenaPostaRequireClient.sendRequest()
						{
							title = title,
							mailings = new ZelenaPostaRequireClient.sendRequestMailing[1]
						{
							new ZelenaPostaRequireClient.sendRequestMailing()
							{
								product = product,
								recipient = new ZelenaPostaRequireClient.addressType()
								{
									name = recipient.Name,
									street = recipient.Street,
									city = recipient.City,
									zip = recipient.Zip,
									country = recipient.Country
								},
								sender = sndr,
								documents = new ZelenaPostaRequireClient.sendRequestMailingDocument[1]
								{
									new ZelenaPostaRequireClient.sendRequestMailingDocument()
									{
										file = file
									}
								}
							}
						}
						});

						if ( null == sendRes )
						{
							res.Status = ZelenaPostaStatus.EMPTY_RESPONSE;
						}
						else
						{
							res.SlotId = sendRes.slotId;
							res.Status = ZelenaPostaStatus.OK;

							cdoMailing.col_SLOT_ID = sendRes.slotId;
						}

						cdoMailing.Insert(); //trigger zabezpeci insert s iniciacnym stavom 99 do ZPO_MAILING_STATE

						//aktualizacia CORRESPONDENCE pre vlozeny mailing
						GetMailingsStatus(res.SlotId);
					}
				}
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("ZelenaPostaProvidePrivate.Send: Chyba odosielania pošty: {0}", ex.Message));

				res.Status = ZelenaPostaStatus.SEND_ERROR;
			}

			return res;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="title"></param>
		/// <param name="product"></param>
		/// <param name="file"></param>
		/// <param name="recipient"></param>
		/// <param name="sender"></param>
		/// <returns></returns>
		public ZelenaPostaResponse Send2(string title, string product, byte[][] files, 
			ZelenaPosta.Recipient recipient, ZelenaPosta.Sender sender, string IDRegZaznamu, string CisloKonania)
		{
			ZelenaPostaResponse res = new ZelenaPostaResponse();
			ZelenaPostaAPI2RequireClient.address sndr = null;
			CdoZPO_MAILING cdoMailing = new CdoZPO_MAILING();

			// overim, ci adresat je vyplneny a ci je prilozeny aspon jeden subor
			if ( null == recipient || null == files || files.Count() < 0 )
			{
				res.Status = ZelenaPostaStatus.MISSING_PARAM;

				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("ZelenaPostaProvidePrivate.Send2: Odosielateľ alebo príloha nemôže byť prázdne!"));

				return res;
			}

			// kedze je aspon 1 subor v poli, tak overim, ci nie je prazdny
			if ( files[0] == null || files[0].Count() == 0 )
			{
				res.Status = ZelenaPostaStatus.MISSING_PARAM;

				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("ZelenaPostaProvidePrivate.Send2: Odosielateľ alebo príloha nemôže byť prázdne!"));

				return res;
			}

			if ( null != sender )
			{
				sndr = new ZelenaPostaAPI2RequireClient.address()
				{
					name = sender.Name,
					street = sender.Street,
					city = sender.City,
					zip = sender.Zip,
					country = sender.Country
				};
			}

			try
			{
				using ( ZelenaPostaAPI2RequireClient.SentServiceClient client = createAndconfigureEndpoint<ZelenaPostaAPI2RequireClient.SentServiceClient>() )
				{
					//Basic Autentifikacia http (bez SSL), nutne pridat rucne Authorization header do http request contextu
					var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
					httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " +
						System.Convert.ToBase64String(Encoding.ASCII.GetBytes(client.ClientCredentials.UserName.UserName + ":" + client.ClientCredentials.UserName.Password));

					var context = new System.ServiceModel.OperationContext(client.InnerChannel);
					using ( new System.ServiceModel.OperationContextScope(context) )
					{
						context.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;

						List<documentToSend> aDocuments = new List<documentToSend>();
						foreach (byte[] f in files )
						{
							aDocuments.Add(new documentToSend()
							{
								file = f,
								fieldValues = new fieldValue[1]
								{
									new fieldValue()
									{
										value = CisloKonania,
										name = "CG_Cislo_Konania"
									}
								}

							});
						}

						//client
						ZelenaPostaAPI2RequireClient.sendMailingsResponse sendRes = client.sendMailings(new ZelenaPostaAPI2RequireClient.sendMailingsRequest()
						{
							title = title,
							mailings = new ZelenaPostaAPI2RequireClient.mailingToSend[1]
						{
							new ZelenaPostaAPI2RequireClient.mailingToSend()
							{
								product = product,
								recipient = new ZelenaPostaAPI2RequireClient.address()
								{
									name = recipient.Name,
									street = recipient.Street,
									city = recipient.City,
									zip = recipient.Zip,
									country = recipient.Country
								},
								sender = sndr,
								documents = aDocuments.ToArray(),
								deliveryConfirmationText = IDRegZaznamu
							}
						}
						});

						if ( null == sendRes )
						{
							res.Status = ZelenaPostaStatus.EMPTY_RESPONSE;
						}
						else
						{
							res.SlotId = sendRes.slotId;
							res.Status = ZelenaPostaStatus.OK;

							cdoMailing.col_SLOT_ID = sendRes.slotId;
						}

						cdoMailing.Insert(); //trigger zabezpeci insert s iniciacnym stavom 99 do ZPO_MAILING_STATE

						//aktualizacia CORRESPONDENCE pre vlozeny mailing
						GetMailingsStatus(res.SlotId);
					}
				}
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("ZelenaPostaProvidePrivate.Send2: Chyba odosielania pošty: {0}", ex.Message));

				res.Status = ZelenaPostaStatus.SEND_ERROR;
			}

			return res;
		}

		public ZelenaPostaResponse SendHromadne(string title, MailingItem[] items)
		{
			ZelenaPostaResponse res = new ZelenaPostaResponse();
			ZelenaPostaAPI2RequireClient.address sndr = null;
			CdoZPO_MAILING cdoMailing = new CdoZPO_MAILING();

			if ( null == items || items?.Length < 1 )
			{
				res.Status = ZelenaPostaStatus.MISSING_PARAM;

				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("ZelenaPostaProvidePrivate.SendHromadne: pole položiek na odoslanie nemože byť prázdne!"));

				return res;
			}

			try
			{
				using ( ZelenaPostaAPI2RequireClient.SentServiceClient client = createAndconfigureEndpoint<ZelenaPostaAPI2RequireClient.SentServiceClient>() )
				{
					//Basic Autentifikacia http (bez SSL), nutne pridat rucne Authorization header do http request contextu
					var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
					httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " +
						System.Convert.ToBase64String(Encoding.ASCII.GetBytes(client.ClientCredentials.UserName.UserName + ":" + client.ClientCredentials.UserName.Password));

					var context = new System.ServiceModel.OperationContext(client.InnerChannel);
					using ( new System.ServiceModel.OperationContextScope(context) )
					{
						context.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;
						//client.Endpoint.EndpointBehaviors.Add(new WsSecurityEndpointBehavior("test.corageo@zelenaposta.sk", "OOMeiFtldzAxYrE"));

						List<mailingToSend> mailings = new List<mailingToSend>();
						foreach ( var item in items )
						{
							if ( item.recipient == null )
								continue;

							if ( item.files == null )
								continue;

							if ( null != item.sender )
							{
								sndr = new ZelenaPostaAPI2RequireClient.address()
								{
									name = item.sender.Name,
									street = item.sender.Street,
									city = item.sender.City,
									zip = item.sender.Zip,
									country = item.sender.Country
								};
							}

							List <documentToSend> aDocs = new List<documentToSend>();
							foreach ( byte[] f in item.files)
							{
								aDocs.Add(new documentToSend(){ 
									file = f,
									fieldValues = new fieldValue[1]
									{
										new fieldValue()
										{
											value = item.CisloKonania,
											name = "CG_Cislo_Konania"
										}
									}
								});
							}

							var mailing = new ZelenaPostaAPI2RequireClient.mailingToSend()
							{
								product = item.product,
								recipient = new ZelenaPostaAPI2RequireClient.address()
								{
									name = item.recipient.Name,
									street = item.recipient.Street,
									city = item.recipient.City,
									zip = item.recipient.Zip,
									country = item.recipient.Country
								},
								sender = sndr,
								documents = aDocs.ToArray(),
								deliveryConfirmationText = item.IDRegZaznamu
							};

							mailings.Add(mailing);
						}

						//client
						ZelenaPostaAPI2RequireClient.sendMailingsResponse sendRes = client.sendMailings(new ZelenaPostaAPI2RequireClient.sendMailingsRequest()
						{
							title = title,
							mailings = mailings.ToArray()
						});

						if ( null == sendRes )
						{
							res.Status = ZelenaPostaStatus.EMPTY_RESPONSE;
						}
						else
						{
							res.SlotId = sendRes.slotId;
							res.Status = ZelenaPostaStatus.OK;

							cdoMailing.col_SLOT_ID = sendRes.slotId;
						}

						cdoMailing.Insert(); //trigger zabezpeci insert s iniciacnym stavom 99 do ZPO_MAILING_STATE

						//aktualizacia CORRESPONDENCE pre vlozeny mailing
						GetMailingsStatus(res.SlotId);
					}
				}
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("ZelenaPostaProvidePrivate.Send2: Chyba odosielania pošty: {0}", ex.Message));

				res.Status = ZelenaPostaStatus.SEND_ERROR;
			}

			return res;
		}

		/// <summary>
		/// Služba na aktualizáciu status zásielok cez API 2 Zelenej pošty.
		/// </summary>
		public void GetMailingsStatus(string slotId = null)
		{
			string errorText = "ZelenaPostaProvidePrivate.GetMailingStatus: Chyba zistenia stavu odoslanej pošty: ";
			List<int> mailings = new List<int>();
			List<string> bundles = new List<string>();

			//overia sa vsetky zasielky, ktore nie su vo finalnych stavoch 11 a 13
			if ( string.IsNullOrEmpty(slotId) )
			{
				string[] finalState11 = new string[] { "'2ndClass'", "'2ndClassD4'", "'postcard'", "'electronic'" };
				string[] finalState13 = new string[] { "'registeredWithAdvice'", "'registered'", "'officialLetter'",
					"'registeredWithAdviceAndRestrictedDelivery'", "'cashOnDelivery'" };

				using ( CoraConnection conn = new CoraConnection(FactoryFactory.GetSFactory<CdoZPO_MAILING_STATE>().sConn) )
				using ( CoraCommand cmd = (CoraCommand) conn.CreateCommand() )
				{
					conn.Open();
					cmd.CommandText = string.Format(@"select m.SLOT_ID
						from ZPO_MAILING_STATE z1
						join ZPO_MAILING m on Z1.I_MAILING = m.I_MAILING
						where 
							(
								(m.CORRESPONDENCE in ({0}) AND z1.I_STATE not in (11)) 
								OR (m.CORRESPONDENCE in ({1}) AND z1.I_STATE not in (13))
							)
							AND Z1.DATE_UPDATED in (select MAX(z2.DATE_UPDATED) from ZPO_MAILING_STATE z2 where Z2.I_MAILING = Z1.I_MAILING)
						order by z1.I_MAILING", string.Join(",", finalState11), string.Join(",", finalState13));

					using ( var reader = cmd.ExecuteReader() )
					{
						while ( reader.Read() )
						{
							bundles.Add(reader[0].ToString());
						}
						reader.Close();
					}
					conn.Close();
				}
			}
			//overi sa zasielka podla SLOT_ID
			else
			{
				bundles.Add(slotId);
			}

			if ( bundles.Count() == 0 )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Warning,
					string.Format(errorText + "Nenašli sa záznamy na overenie statusu."));

				return;
			}

			try
			{
				using ( ZelenaPostaAPI2RequireClient.SentServiceClient client = createAndconfigureEndpoint<ZelenaPostaAPI2RequireClient.SentServiceClient>() )
				{
					//Basic Autentifikacia http (bez SSL), nutne pridat rucne Authorization header do http request contextu
					var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
					httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " +
						System.Convert.ToBase64String(Encoding.ASCII.GetBytes(client.ClientCredentials.UserName.UserName + ":" + client.ClientCredentials.UserName.Password));

					var context = new System.ServiceModel.OperationContext(client.InnerChannel);
					using ( new System.ServiceModel.OperationContextScope(context) )
					{
						context.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;

						//API2 getBundles - na zaklade davky sa zisti ID zasielky
						ZelenaPostaAPI2RequireClient.getBundlesResponse bundlesRes = client.getBundles(new ZelenaPostaAPI2RequireClient.getBundlesRequest()
						{
							slots = bundles.ToArray<string>(),
							limit = 1000
						});

						if ( null == bundlesRes || bundlesRes.bundles.Count() == 0 )
						{
							Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
								string.Format(errorText + "Odpoveď ZelenaPostaAPI2RequireClient.getBundlesRequest je prázdna"));

							return;
						}

						bundlesRes.bundles.ToList().ForEach(b => mailings.Add(b.id));

						//API2 getMailings - zisti sa stav zasielky
						ZelenaPostaAPI2RequireClient.getMailingsResponse mailingsRes = client.getMailings(new ZelenaPostaAPI2RequireClient.getMailingsRequest()
						{
							bundles = mailings.ToArray<int>(),
							limitSpecified = false
						});

						if ( null == mailingsRes )
						{
							Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
								string.Format(errorText + "Odpoveď ZelenaPostaAPI2RequireClient.getMailingResponse je prázdna"));

							return;
						}

						//cyklus vez vsetky overene zasielky
						foreach ( ZelenaPostaAPI2RequireClient.simpleSentMailing mailing in mailingsRes.mailings )
						{
							CdoZPO_MAILING cdoMailing = new CdoZPO_MAILING();//kvoli vytvoreniu konekcie

							cdoMailing = FactoryFactory.GetSFactory<CdoZPO_MAILING>()
								.GetByCondParam("SLOT_ID = <paramBind>P0</paramBind>", new object[] { mailing.id }, null, null).FirstOrDefault();

							if ( null == cdoMailing )
							{
								Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
									string.Format(errorText + "Nenájdený rodičovský ZPO_MAILING podľa SLOT_ID:{0}", mailing.id));

								continue;
							}

							//zapis stavu (alebo akutalneho datumu) do DB
							ZelenaPostaStatus.MailingStates actualState = (ZelenaPostaStatus.MailingStates) mailing.status,
							lastState = selectLastMailingState(cdoMailing);

							if ( actualState == lastState )
							{
								updateLastMailingState(cdoMailing);
							}
							else
							{
								insertLastMailingState(cdoMailing, actualState);
							}

							//ak sa iniciovalo overenie jedneho SLOT_ID zo vstupenho parametra, aktualizuje sa jeho CORRESPONDENCE
							if ( !string.IsNullOrEmpty(slotId) && mailingsRes.mailings.Count() == 1 )
							{
								var mailingRes = client.getMailing(new ZelenaPostaAPI2RequireClient.getMailingRequest()
								{
									mailingId = mailing.id
								});

								if ( mailingRes != null )
								{
									cdoMailing.col_CORRESPONDENCE = mailingRes?.mailing?.productCorrespondence;
									cdoMailing.Update();
								}
							}
						}

						return;
					}
				}
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format(errorText, ex.Message));

				return;
			}
			finally
			{

			}
		}

		/// <summary>
		/// Získa aktuálny status zásielky.
		/// </summary>
		/// <param name="slotId">The slot identifier.</param>
		/// <returns>
		/// Posledný aktuálny status zásielky.
		/// </returns>
		public ZelenaPostaStatus.MailingStates GetStatus(string slotId)
		{
			int mailingIdent;
			string errorText = "ZelenaPostaProvidePrivate.GetStatus: Chyba zistenia stavu odoslanej pošty: ";

			if ( !Cora.Convert.Parser.TryGetInt(slotId, out mailingIdent) )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format(errorText + "Nesprávny formát vstupu - SLOT_ID"));

				return ZelenaPostaStatus.MailingStates.CgError;
			}

			CdoZPO_MAILING cdoMailing = FactoryFactory.GetSFactory<CdoZPO_MAILING>()
				.GetByCondParam("SLOT_ID = <paramBind>P0</paramBind>", new object[] { slotId }, null, null).FirstOrDefault();

			if ( null == cdoMailing )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format(errorText + "Nenájdený rodičovský ZPO_MAILING podľa SLOT_ID:{0}", slotId));

				return ZelenaPostaStatus.MailingStates.CgError;
			}

			return selectLastMailingState(cdoMailing);
		}

		/// <summary>
		/// Ziska zoznam produktov Zelenej posty
		/// </summary>
		/// <returns></returns>
		public ZelenaPostaAPI2ProductRequireClient.productInfo[] GetProducts()
		{
			string errorText = "ZelenaPostaProvidePrivate.GetProducts: Chyba zistenia produktov Zelenej posty: {0} ";

			try
			{
				using ( ZelenaPostaAPI2ProductRequireClient.ProductServiceClient client = createAndconfigureEndpoint<ZelenaPostaAPI2ProductRequireClient.ProductServiceClient>() )
				{
					if ( client == null )
					{
						Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
							string.Format("ZelenaPostaAPI2ProductRequireClient.getProducts - nepodarilo sa vytvorit a nakonfigurovat endpoint klienta"));
						return null;
					}
					//Basic Autentifikacia http (bez SSL), nutne pridat rucne Authorization header do http request contextu
					var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
					httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " +
						System.Convert.ToBase64String(Encoding.ASCII.GetBytes(client.ClientCredentials.UserName.UserName + ":" + client.ClientCredentials.UserName.Password));

					var context = new System.ServiceModel.OperationContext(client.InnerChannel);
					using ( new System.ServiceModel.OperationContextScope(context) )
					{
						context.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;
						ZelenaPostaAPI2ProductRequireClient.getProductsResponse bundlesRes = client.getProducts(new ZelenaPostaAPI2ProductRequireClient.getProductsRequest());

						if ( null == bundlesRes)
						{
							Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
								string.Format(errorText + "Odpoveď ZelenaPostaAPI2ProductRequireClient.getProducts je prázdna"));
							return null;
						}

						return bundlesRes.products;
					}
				}
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,	string.Format(errorText, ex.Message));
				return null;
			}
			finally
			{

			}
		}

		/// <summary>
		/// Služba na aktualizáciu status zásielok cez API 2 Zelenej pošty.
		/// </summary>
		public void CancelMailings(string slotId = null)
		{
			string errorText = "ZelenaPostaProvidePrivate.CancelMailings: Chyba zrusenia zasielky odoslanej pošty: ";
			List<int> mailings = new List<int>();
			List<string> bundles = new List<string>();
			bundles.Add(slotId);

			if ( slotId == null )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
								string.Format(errorText + "ZelenaPostaAPI2RequireClient.cancelMailings - parameter slotId nemôže byť prázdny"));
				return;
			}

			try
			{
				using ( ZelenaPostaAPI2RequireClient.SentServiceClient client = createAndconfigureEndpoint<ZelenaPostaAPI2RequireClient.SentServiceClient>() )
				{
					//Basic Autentifikacia http (bez SSL), nutne pridat rucne Authorization header do http request contextu
					var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
					httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " +
						System.Convert.ToBase64String(Encoding.ASCII.GetBytes(client.ClientCredentials.UserName.UserName + ":" + client.ClientCredentials.UserName.Password));

					var context = new System.ServiceModel.OperationContext(client.InnerChannel);
					using ( new System.ServiceModel.OperationContextScope(context) )
					{
						context.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;

						//client.getMailings
						ZelenaPostaAPI2RequireClient.getBundlesResponse bundlesRes = client.getBundles(new ZelenaPostaAPI2RequireClient.getBundlesRequest()
						{
							slots = bundles.ToArray<string>(),
							limitSpecified = false	
						});

						if ( null == bundlesRes || bundlesRes.bundles.Count() == 0 )
						{
							Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
								string.Format(errorText + "Odpoveď ZelenaPostaAPI2RequireClient.getBundlesRequest je prázdna"));

							return;
						}

						bundlesRes.bundles.ToList().ForEach(b => mailings.Add(b.id));

						//cancel mailings
						cancelMailingsResponse res = client.cancelMailings(new cancelMailingsRequest()
						{
							mailingIds = mailings.ToArray<int>(),
						});

						if ( null == res)
						{
							Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
								string.Format(errorText + "Odpoveď ZelenaPostaAPI2RequireClient.cancelMailings je prázdna"));

							return;
						}
					}
				}
			}
			catch ( Exception ex )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format(errorText, ex.Message));

				return;
			}
			finally
			{

			}
		}

		/// <summary>
		/// Získa posledný aktuálny status zásielky z DB štruktúr.
		/// </summary>
		/// <param name="cdoMailing">The CDO mailing.</param>
		/// <returns></returns>
		private ZelenaPostaStatus.MailingStates selectLastMailingState(CdoZPO_MAILING cdoMailing)
		{
			var cdoState = cdoMailing.GetAllCdoZPO_MAILING_STATE_col_I_MAILING(null, "DATE_UPDATED DESC").FirstOrDefault();

			if ( null == cdoState )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("Nenájdený záznam ZPO_MAILING_STATE podľa SLOT_ID:{0}", cdoMailing.col_SLOT_ID));

				return ZelenaPostaStatus.MailingStates.CgError;
			}

			return (ZelenaPostaStatus.MailingStates) cdoState.col_I_STATE;
		}

		private void insertLastMailingState(CdoZPO_MAILING cdoMailing, ZelenaPostaStatus.MailingStates state)
		{
			CdoZPO_MAILING_STATE cdoState = new CdoZPO_MAILING_STATE()
			{
				col_I_MAILING = cdoMailing.col_I_MAILING,
				col_I_STATE = (int) state,
				col_DATE_UPDATED = DateTime.Now
			};

			cdoState.Insert();
		}

		private void updateLastMailingState(CdoZPO_MAILING cdoMailing)
		{
			var cdoState = cdoMailing.GetAllCdoZPO_MAILING_STATE_col_I_MAILING(null, "DATE_UPDATED DESC").FirstOrDefault();

			if ( null == cdoState )
			{
				Utils.Logger.AppLogging.Logger.Log(Utils.Logger.LogLevel.Error,
					string.Format("Nenájdený záznam ZPO_MAILING_STATE podľa SLOT_ID:{0}", cdoMailing.col_SLOT_ID));

				return;
			}

			cdoState.col_DATE_UPDATED = DateTime.Now;
			cdoState.Update();
		}

		#region compat metody
		public byte[] SendCompat(string title, string product, string sFile,
			string recName, string recStreet, string recCity, string recZip, string recCountry,
			string sndName, string sndStreet, string sndCity, string sndZip, string sndCountry)
		{

			byte[] file = System.Convert.FromBase64String(sFile);

			ZelenaPostaResponse res = this.Send(title, product, file,
				new ZelenaPosta.Recipient()
				{
					Name = recName,
					Street = recStreet,
					City = recCity,
					Zip = recZip,
					Country = recCountry
				},
				new ZelenaPosta.Sender()
				{
					Name = sndName,
					Street = sndStreet,
					City = sndCity,
					Zip = sndZip,
					Country = sndCountry
				});

			return Cora.Convert.Xml.GetBytes(res.Serialize());
		}

		public byte[] Send2Compat(string title, string product, string sFile,
					string recName, string recStreet, string recCity, string recZip, string recCountry,
					string sndName, string sndStreet, string sndCity, string sndZip, string sndCountry, 
					string IDRegZaznamu, string CisloKonania)
		{

			byte[][] file = new byte[1][];
			file[0] = System.Convert.FromBase64String(sFile);

			ZelenaPostaResponse res = this.Send2(title, product, file,
				new ZelenaPosta.Recipient()
				{
					Name = recName,
					Street = recStreet,
					City = recCity,
					Zip = recZip,
					Country = recCountry
				},
				new ZelenaPosta.Sender()
				{
					Name = sndName,
					Street = sndStreet,
					City = sndCity,
					Zip = sndZip,
					Country = sndCountry
				}, IDRegZaznamu, CisloKonania);

			return Cora.Convert.Xml.GetBytes(res.Serialize());
		}
		#endregion

		private static T createAndconfigureEndpoint<T>() where T : class
		{
			object client = null;
			Type t = typeof(T);

			if ( t == typeof(ZelenaPostaRequireClient.MailingClient) ) //API 1
			{
				client = new ZelenaPostaRequireClient.MailingClient();
				Cora.Web.WebServiceClientConfig.Create("WebServiceClientConfig_ZelenaPosta").Configure<ZelenaPostaRequireClient.Mailing>(
					(ZelenaPostaRequireClient.MailingClient) client);
			}
			else if ( t == typeof(ZelenaPostaAPI2RequireClient.SentServiceClient) ) //API 2 Sent
			{
				client = new ZelenaPostaAPI2RequireClient.SentServiceClient();
				Cora.Web.WebServiceClientConfig.Create("WebServiceClientConfig_ZelenaPosta").Configure<ZelenaPostaAPI2RequireClient.SentService>(
					(ZelenaPostaAPI2RequireClient.SentServiceClient) client);
			}
			else if ( t == typeof(ZelenaPostaAPI2ProductRequireClient.ProductServiceClient) ) //API 2 Products
			{
				client = new ZelenaPostaAPI2ProductRequireClient.ProductServiceClient();
				Cora.Web.WebServiceClientConfig.Create("WebServiceClientConfig_ZelenaPosta").Configure<ZelenaPostaAPI2ProductRequireClient.ProductService>(
					(ZelenaPostaAPI2ProductRequireClient.ProductServiceClient) client);
			}

			return (T) client;
		}
	}

	public class WsSecurityEndpointBehavior : IEndpointBehavior
	{
		private readonly string _username;
		private readonly string _password;

		public WsSecurityEndpointBehavior(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public void AddBindingParameters(ServiceEndpoint endpoint,
				BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.ClientMessageInspectors.Add(new WsSecurityMessageInspector(_username, _password));
		}
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}

	public class WsSecurityMessageInspector : IClientMessageInspector
	{
		private readonly string _username;
		private readonly string _password;

		public WsSecurityMessageInspector(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var header = new Security
			{
				UsernameToken =
					{
						Password = new Password
						{
							Value = _password,
							Type =  "wsse:PasswordText"
						},
						Username = _username
					}
			};

			request.Headers.Add(header);

			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}
	}

	public class Password
	{
		[XmlAttribute] public string Type { get; set; }

		[XmlText] public string Value { get; set; }
	}

	[XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
	public class UsernameToken
	{
		[XmlElement] public string Username { get; set; }

		[XmlElement] public Password Password { get; set; }
	}

	public class Security : MessageHeader
	{
		public Security()
		{
			UsernameToken = new UsernameToken();
		}

		public UsernameToken UsernameToken { get; set; }

		public override string Name => GetType().Name;

		public override string Namespace =>
			"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

		public override bool MustUnderstand => true;

		protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
		{
			var serializer = new System.Xml.Serialization.XmlSerializer(typeof(UsernameToken));
			serializer.Serialize(writer, UsernameToken);
		}
	}
}