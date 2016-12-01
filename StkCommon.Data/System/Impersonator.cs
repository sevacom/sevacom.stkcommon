using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using StkCommon.Data.Utils;

namespace StkCommon.Data.System
{
	public interface IImpersonator
	{
		/// <summary>
		/// Impersonates the specified user account.
		/// </summary>
		///<param name="userName">Name of the user.</param>
		///<param name="domainName">Name of the domain.</param>
		///<param name="password">The password. <see cref="string"/></param>
		///<param name="logonType">Type of the logon.</param>
		///<param name="logonProvider">The logLogonProviderry.Network.LogonProvider"/></param>
		void Impersonate(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider);
	}

	/// <summary>
	/// http://platinumdogs.me/2008/10/30/net-c-impersonation-with-network-credentials/
	/// </summary>
	public class Impersonator : IImpersonator, IDisposable
	{
		private WindowsImpersonationContext _wic;

		/// <summary>
		/// Begins impersonation with the given credentials, Logon type and Logon provider.
		/// </summary>
		///<param name="userName">Name of the user.</param>
		///<param name="domainName">Name of the domain.</param>
		///<param name="password">The password. <see cref="string"/></param>
		///<param name="logonType">Type of the logon.</param>
		///<param name="logonProvider">The logLogonProviderry.Network.LogonProvider"/></param>
		public Impersonator(string userName, string domainName, string password, LogonType logonType,
			LogonProvider logonProvider)
		{
			Impersonate(userName, domainName, password, logonType, logonProvider);
		}

		/// <summary>
		/// Begins impersonation with the given credentials.
		/// </summary>
		///<param name="userName">Name of the user.</param>
		///<param name="domainName">Name of the domain.</param>
		///<param name="password">The password. <see cref="string"/></param>
		public Impersonator(string userName, string domainName, string password)
		{
			Impersonate(userName, domainName, password, LogonType.Interactive,
				LogonProvider.Default);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Impersonator"/> class.
		/// </summary>
		public Impersonator()
		{
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			UndoImpersonation();
			DisposeHelper.SafeDispose(ref _wic);
		}

		/// <summary>
		/// Impersonates the specified user account.
		/// </summary>
		///<param name="userName">Name of the user.</param>
		///<param name="domainName">Name of the domain.</param>
		///<param name="password">The password. <see cref="string"/></param>
		public void Impersonate(string userName, string domainName, string password)
		{
			Impersonate(userName, domainName, password, LogonType.Interactive,
				LogonProvider.Default);
		}

		/// <summary>
		/// Impersonates the specified user account.
		/// </summary>
		///<param name="userName">Name of the user.</param>
		///<param name="domainName">Name of the domain.</param>
		///<param name="password">The password. <see cref="string"/></param>
		///<param name="logonType">Type of the logon.</param>
		///<param name="logonProvider">The logLogonProviderry.Network.LogonProvider"/></param>
		public void Impersonate(string userName, string domainName, string password, LogonType logonType,
			LogonProvider logonProvider)
		{
			UndoImpersonation();

			IntPtr logonToken = IntPtr.Zero;
			IntPtr logonTokenDuplicate = IntPtr.Zero;
			try
			{
				// revert to the application pool identity, saving the identity of the current requestor
				_wic = WindowsIdentity.Impersonate(IntPtr.Zero);

				// do logon & impersonate
				if (Win32NativeMethods.LogonUser(userName,
					domainName,
					password,
					(int)logonType,
					(int)logonProvider,
					ref logonToken) != 0)
				{
					if (
						Win32NativeMethods.DuplicateToken(logonToken, (int)ImpersonationLevel.SecurityImpersonation,
							ref logonTokenDuplicate) != 0)
					{
						using(var wi = new WindowsIdentity(logonTokenDuplicate))
							wi.Impersonate(); // discard the returned identity context (which is the context of the application pool)
					}
					else
						throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				else
					throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			finally
			{
				if (logonToken != IntPtr.Zero)
					Win32NativeMethods.CloseHandle(logonToken);

				if (logonTokenDuplicate != IntPtr.Zero)
					Win32NativeMethods.CloseHandle(logonTokenDuplicate);
			}
		}

		/// <summary>
		/// Stops impersonation.
		/// </summary>
		private void UndoImpersonation()
		{
			// restore saved requestor identity
			if (_wic != null)
				_wic.Undo();
			_wic = null;
		}
	}

	/// <summary>
	/// Specifies the type of login used.
	/// http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx
	/// </summary>
	public enum LogonType
	{
		/// <summary>
		/// This logon type is intended for users who will be interactively using the computer, such as a user being logged
		/// on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching
		/// logon information for disconnected operations; therefore, it is inappropriate for some client/server applications,
		/// such as a mail server.
		/// </summary>
		Interactive = 2,

		/// <summary>
		/// This logon type is intended for high performance servers to authenticate plaintext passwords.
		/// The LogonUser function does not cache credentials for this logon type.
		/// </summary>
		Network = 3,

		/// <summary>
		/// This logon type is intended for batch servers, where processes may be executing on behalf of a user
		/// without their direct intervention. This type is also for higher performance servers that process many
		/// plaintext authentication attempts at a time, such as mail or web servers.
		/// </summary>
		Batch = 4,

		/// <summary>
		/// Indicates a service-type logon. The account provided must have the service privilege enabled. 
		/// </summary>
		Service = 5,

		/// <summary>
		/// GINAs are no longer supported.
		/// Windows Server 2003 and Windows XP:  This logon type is for GINA DLLs that log on users who will be
		/// interactively using the computer. This logon type can generate a unique audit record that shows when
		/// the workstation was unlocked.
		/// </summary>
		Unlock = 7,

		/// <summary>
		/// This logon type preserves the name and password in the authentication package, which allows the server
		/// to make connections to other network servers while impersonating the client. A server can accept plaintext
		/// credentials from a client, call LogonUser, verify that the user can access the system across the network,
		/// and still communicate with other servers.
		/// </summary>
		NetworkCleartext = 8,

		/// <summary>
		/// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
		/// The new logon session has the same local identifier but uses different credentials for other network connections.
		/// This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
		/// </summary>
		NewCredentials = 9,
	}
	public enum LogonProvider
	{
		Default = 0,
		Winnt35 = 1,
		Winnt40 = 2,
		Winnt50 = 3
	};

	public enum ImpersonationLevel
	{
		Anonymous = 0,
		Identification = 1,
		SecurityImpersonation = 2,
		Delegation = 3
	}

	internal static class Win32NativeMethods
	{
		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern int LogonUser(string lpszUserName,
			string lpszDomain,
			string lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			ref IntPtr phToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DuplicateToken(IntPtr hToken,
			int impersonationLevel,
			ref IntPtr hNewToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool RevertToSelf();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern bool CloseHandle(IntPtr handle);
	}
}
