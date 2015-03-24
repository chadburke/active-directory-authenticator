# active-directory-authenticator
In an Active Directory domain sometimes standard LDAP packages are not able to communicate effectively with AD.  

This relatively short bit of code is something we've used successfully for about 4 years on a Windows Server 2003 box to allow Windows username / password combinations to be authenticated for all custom applications.

This was originally developed and tested on a Windows XP machine and deployed to a Windows Server 2003 machine.  There appears to have been some change between Server 2003 and Server 2008 that causes a `LogonUser` failure to return the error code `1329` for a different reason than before.  This is a problem as that code basically says whether the login was successful or not.

I've not yet solved the 2008 issue but I'm comfortable using the code on a Server 2003 box.

Also, if you end up getting a security alert along the lines of `x is security transparent, but is a member of a security critical type.` when trying to run the code adding the following to AssemblyInfo.cs appears to fix it :

To the top :

`using System.Security; // Requires reference to System.Security`

To the bottom :

`[assembly: SecurityRules(SecurityRuleSet.Level1)]`
