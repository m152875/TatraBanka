using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cora.CommIss.Iss.Dovera
{
	public class DoveraExtResponse<T>
	{
		public T Response { get; set; }
		public string Message { get; set; }
		public Int64 Result { get; set; }

	}
}