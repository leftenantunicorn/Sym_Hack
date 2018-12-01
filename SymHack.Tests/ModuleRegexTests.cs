using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymHack;
using SymHack.Controllers;

namespace SymHack.Tests.Controllers
{
    [TestClass]
    public class ModuleRegexTests
    {
        [TestMethod]
        public void Netsh()
        {
            String pattern = "netsh winhttp set proxy (\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}:\\d{1,5})";
            String log = "netsh winhttp set proxy 10.0.0.6:8080";
            foreach (Match m in Regex.Matches(log, pattern))
            {
                String response = "Current WinHTTP proxy settings:\r\n\r\nProxy Server(s) :  {1}\r\nBypass List     :  (none)";

                object[] vars = new object[m.Groups.Count];
                m.Groups.CopyTo(vars, 0);
                response = String.Format(response, vars);

                Assert.AreEqual(
                    "Current WinHTTP proxy settings:\r\n\r\nProxy Server(s) :  10.0.0.6:8080\r\nBypass List     :  (none)",
                    response);
            }
        }

        [TestMethod]
        public void Whois()
        {
            String pattern = "whois (?!exploitme.ca$).*";
            String log = "whois exploitme.ca";
            foreach (Match m in Regex.Matches(log, pattern))
            {
                Assert.Fail();
            }
        }
    }
}