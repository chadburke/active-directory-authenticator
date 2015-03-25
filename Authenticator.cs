using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices; // DllImport
using System.Security; // SecurityCritical
using Microsoft.Win32.SafeHandles;

using System.DirectoryServices; // Requires Reference
using System.DirectoryServices.AccountManagement; // Requires Reference

namespace ActiveDirectoryDemo
{
    class Authenticator
    {

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );

        [DllImport("kernel32.dll", SetLastError = false)]
        static extern bool CloseHandle(IntPtr hObject);

        public static string Authenticate(string domain, string username, string password)
        {
            IntPtr h;

            // this should be checked before this point but ..
            if (password == "")
            {
                return "invalid_password";
            }

            // LogonUser essentially attempts a logon as the given credentials to the machine that
            // this code runs on.  In a domain setup this check will rarely pass as user's are typically
            // assigned to specific terminals.
            if (LogonUser(username, domain, password, 3, 0, out h))
            {
                CloseHandle(h);
                return "valid_login";
            }

            // if LogonUser fails then there is an error code that we need to fetch
            // Marshal.GetLastWin32Error is how we get our hands on it
            int error = Marshal.GetLastWin32Error();

            // 1329 means 'not authorized' -- this means: the user and password were correct
            // but they don't have authority to logon to this machine.  That's okay for this purpose.
            // Note: It appears that either some versions of Windows return this code differently [ 64 bit? ]
            // Windows XP   : Works
            // Server 2003  : Works
            // Server 2008  : Mostly works?  Sometimes 1329 is thrown when it should not be.
            if (error == 1329)
            {
                return "valid_login";
            }

            // 1326 means 'Unknown user name or bad password' -- that's not specific enough for us.
            // So we dig.
            if (error == 1326)
            {

                PrincipalContext principalContext = null;
                UserPrincipal foundUser = null;

                try
                {
                    principalContext = new PrincipalContext(ContextType.Domain, domain);
                    foundUser = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, username);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // user doesn't exist, invalid_username
                if (foundUser == null)
                {
                    return "invalid_username";
                }

                // user exists but since there was an error we know the password is wrong
                return "invalid_password";

            }

            // this does can occur and could be assumed to be an invalid_password
            return "unknown_error";

        }
    }
}
