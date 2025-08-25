using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Cora.CommIss.Iss.TatraBanka
{
    /// <summary>
    /// ITatraBankaService
    /// </summary>
    [ServiceContract]
    public interface ITatraBankaService
    {
        /// <summary>
        /// Získanie URL pre autorizáciu 
        /// </summary>
        /// <param name="AuthName"></param>
        /// <param name="I_UZ"></param>
        /// <returns></returns>
        [OperationContract]
        AuthorizationResultVm GetUrl(string AuthName, int? I_UZ);

        /// <summary>
        /// Získanie účtov na základe consent
        /// </summary>
        /// <param name="consent"></param>
        /// <returns></returns>
        [OperationContract]
        XmlElement GetAccounts(string consent);

		/// <summary>
		/// GetTransactions
		/// </summary>
		/// <param name="consent"></param>
		/// <param name="accountId"></param>
		/// <param name="amountFrom"></param>
		/// <param name="amountTo"></param>
		/// <param name="transactionDirection"></param>
		/// <param name="transactionIdFrom"></param>
		/// <param name="bankTransactionCode"></param>
		/// <param name="variableSymbol"></param>
		/// <param name="constantSymbol"></param>
		/// <param name="pageSize"></param>
		/// <param name="dateTo"></param>
		/// <param name="dateFrom"></param>
		/// <param name="specificSymbol"></param>
		/// <param name="entryReferenceFrom"></param>
		/// <returns>XmlElement - struktura podla Response TB API prelozena do XML</returns>
		[OperationContract]
		XmlElement GetTransactions(string consent, string accountId, int? amountFrom, int? amountTo,
            string transactionDirection, int? transactionIdFrom,string bankTransactionCode,
            string variableSymbol, string constantSymbol, int? pageSize, DateTime? dateTo,
            DateTime? dateFrom, string specificSymbol, string entryReferenceFrom);

		/// <summary>
		/// GetTransactionsCompat
		/// </summary>
		/// <param name="consent"></param>
		/// <param name="accountId"></param>
		/// <param name="amountFrom"></param>
		/// <param name="amountTo"></param>
		/// <param name="transactionDirection"></param>
		/// <param name="transactionIdFrom"></param>
		/// <param name="bankTransactionCode"></param>
		/// <param name="variableSymbol"></param>
		/// <param name="constantSymbol"></param>
		/// <param name="pageSize"></param>
		/// <param name="dateTo"></param>
		/// <param name="dateFrom"></param>
		/// <param name="specificSymbol"></param>
		/// <param name="entryReferenceFrom"></param>
		/// <returns></returns>
		[OperationContract]
        XmlElement GetTransactionsCompat(string consent, string accountId, string amountFrom, string amountTo,
            string transactionDirection, string transactionIdFrom, string bankTransactionCode,
            string variableSymbol, string constantSymbol, string pageSize, string dateTo,
            string dateFrom, string specificSymbol, string entryReferenceFrom);

        /// <summary>
        /// Obnovenie autorizácie
        /// </summary>
        /// <param name="consent"></param>
        /// <returns></returns>
        [OperationContract]
        string RefreshAuth(string consent);

        /// <summary>
        /// Zmazanie autorizácie
        /// </summary>
        /// <param name="consent"></param>
        /// <returns></returns>
        [OperationContract]
        string DeleteAuth(string consent);


        /// <summary>
        /// Získanie detailov účtu na základe consentu a accountId
        /// </summary>
        /// <param name="consent"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [OperationContract]
        XmlElement GetAccountDetails(string consent, string accountId);

        /// <summary>
        /// RequestStatement
        /// </summary>
        /// <param name="consent"></param>
        /// <param name="accountId"></param>
        /// <param name="dateFromStatements"></param>
        /// <param name="dateToStatements"></param>
        /// <param name="exportType"></param>
        /// <param name="requestedSequenceNumber"></param>
        /// <param name="statementsPerDay"></param>
        /// <param name="includeDailyBalances"></param>
        /// <returns></returns>
        [OperationContract]
		XmlElement RequestStatement(string consent, string accountId, string dateFromStatements,
            string dateToStatements, string exportType, int? requestedSequenceNumber,
            bool? statementsPerDay, bool? includeDailyBalances);

		/// <summary>
		/// Získanie výpisu na základe consentu, accountId a statementId
		/// </summary>
		/// <param name="consent"></param>
		/// <param name="accountId"></param>
		/// <param name="taskId"></param>
		/// <returns></returns>
		[OperationContract]
        string GetStatement(string consent, string accountId, string taskId);

        /// <summary>
        /// Získanie JWT tokenu na základe code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string TB_Get_JWT(string code);
    }
}
