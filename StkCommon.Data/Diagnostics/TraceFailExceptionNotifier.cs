using System;
using System.Diagnostics;

namespace StkCommon.Data.Diagnostics
{
	public class TraceFailExceptionNotifier : IExceptionNotifier
	{
		public void Notify(Exception exception, string errorMessage, string errorId = null)
		{
			Trace.Fail(errorMessage, exception.ToString());
		}

		public void Notify(Exception exception)
		{
			Trace.Fail(exception.ToString());
		}

		public void Notify(string errorMessage, string errorId = null)
		{
			Trace.Fail(errorMessage);
		}
	}
}