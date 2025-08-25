using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cora.CommIss.Iss.TatraBanka
{
    /// <summary>
    /// AuthorizationResultVm
    /// </summary>
    public class AuthorizationResultVm
    {
        /// <summary>
        /// url
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// consentId
        /// </summary>
        public string consentId { get; set; }

        /// <summary>
        /// message
        /// </summary>
        public string message { get; set; }

    }
}