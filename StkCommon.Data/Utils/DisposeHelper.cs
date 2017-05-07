using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace StkCommon.Data.Utils
{
	public static class DisposeHelper
	{
		/// <summary>
		/// Вызов Dispose. Внутри проверка на null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="disposable"></param>
		public static void SafeDispose<T>(ref T disposable) where T : IDisposable
		{
		    SafeDispose(ref disposable, false);
		}

	    /// <summary>
	    /// Вызов Dispose. Внутри проверка на null
	    /// </summary>
	    /// <typeparam name="T"></typeparam>
	    /// <param name="disposable"></param>
	    /// <param name="ignoreExceptions">true - гасить все ошибки, false - нет</param>
	    public static void SafeDispose<T>(ref T disposable, bool ignoreExceptions) where T : IDisposable
        {
            try
            {
                // Dispose can safely be called on an object multiple times.
                IDisposable t = disposable;
                disposable = default(T);
                if (null != t)
                {
                    t.Dispose();
                }
            }
            catch
            {
                if (ignoreExceptions)
                    return;
                throw;
            }
        }

        /// <SecurityNote>
        /// Critical - Suppresses unmanaged code security.  Calls Marshal.FinalReleaseComObject which has a LinkDemand.
        /// </SecurityNote>
        [SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static void SafeRelease<T>(ref T comObject) where T : class
		{
			var t = comObject;
			comObject = default(T);
			if (null != t && Marshal.IsComObject(t))
			{
				Debug.Assert(Marshal.IsComObject(t));
				Marshal.FinalReleaseComObject(t);
			}
		}
	}
}
