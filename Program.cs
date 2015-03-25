using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string domain = "domain";
            string username = "username";
            string password = "password";
            Console.WriteLine(Authenticator.Authenticate(domain, username, password));
        }
    }
}
