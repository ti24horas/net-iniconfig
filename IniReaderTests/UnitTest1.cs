using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using IniReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IniReaderTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestUnamedSectionCanBeHandled()
        {
            var str = "[section]";
            var cfg = IniFileReader.Load(str);
            var section = cfg.GetSection("section");
            Assert.IsNotNull(section);
            Assert.AreEqual("section", section.Section);

        }

        [TestMethod]
        public void TestNamedSectionCanBeHandled()
        {
            var str = "[section \"name\"]";
            var cfg = IniFileReader.Load(str);
            var section = cfg.GetSection("section", "name");
            Assert.IsNotNull(section);
            Assert.AreEqual("section", section.Section);
            Assert.AreEqual("name", section.Name);
        }

        [TestMethod]
        public void TestUnamedSectionWithAttributesCanBeHandled()
        {
            var sb = new StringBuilder("[section]");
            sb.AppendLine();
            sb.AppendLine("host = google.com.br");
            sb.AppendLine("port = 81");
            var cfg = IniFileReader.Load(sb.ToString());
            var section = cfg.GetSection("section");
            Assert.IsNotNull(section);
            Assert.AreEqual("section", section.Section);
            Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "host" && c.Value == "google.com.br"));
            Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "port" && c.Value == "81"));
        }

        [TestMethod]
        public void TestUnamedSectionWithAttributeWithoutValueCanBeHandled()
        {
            var sb = new StringBuilder("[section]");
            sb.AppendLine();
            sb.AppendLine("host = google.com.br");
            sb.AppendLine("port = 81");
            sb.AppendLine("useAlternatePort");
            var cfg = IniFileReader.Load(sb.ToString());
            var section = cfg.GetSection("section");
            Assert.IsNotNull(section);
            Assert.AreEqual("section", section.Section);
            Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "host" && c.Value == "google.com.br"));
            Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "port" && c.Value == "81"));
            Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "useAlternatePort"));
        }
        
        [TestMethod]
        public void TestDuplicateAttributeValuesNotIgnoringDuplicatesThrowsException()
        {
            var sb = new StringBuilder("[section]");
            sb.AppendLine();
            sb.AppendLine("host = google.com.br");
            sb.AppendLine("port = 81");
            sb.AppendLine("port = 80");
            try
            {
                IniFileReader.Load(sb.ToString());
            }
            catch (DuplicateNameException)
            {
                return;
            }
            Assert.Fail("should throw DuplicateNameException");
        }
    }
}
