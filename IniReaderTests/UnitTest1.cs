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
        public void TestFileWithStrangeSectionShouldThrowException()
        {
            var str = "[]";
            try
            {
                var cfg = IniFileReader.Load(str);
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Assert.Fail("should throw InvalidOperationException");
            
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
        public void AttributeWithoutSectionShouldThrowException()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("host = google.com.br");
            sb.AppendLine("port = 81");
            try
            {
                var cfg = IniFileReader.Load(sb.ToString());

            }
            catch (InvalidOperationException)
            {

                return;
            }
            Assert.Fail("Should throw InvalidOperationException");
            
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
            sb.AppendLine("#comment line");
            sb.AppendLine("");//empty line
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

        [TestMethod]
        public void TestUnamedSectionWithStartingtabsCanBeHandled()
        {
            var sb = new StringBuilder("            [section]");
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
        public void TestUnamedSectionWithTabbedAttributesCanBeHandled()
        {
            var sb = new StringBuilder("[section]");
            sb.AppendLine();
            sb.AppendLine("\thost = google.com.br");
            sb.AppendLine("\t\tport = 81");
            var cfg = IniFileReader.Load(sb.ToString());
            var section = cfg.GetSection("section");
            Assert.IsNotNull(section);
            Assert.AreEqual("section", section.Section);
            Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "host" && c.Value == "google.com.br"));
            Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "port" && c.Value == "81"));
        }
        [TestMethod]
        public void TestMultipleSections()
        {
            var sb = new StringBuilder("[section]");
            sb.AppendLine();
            sb.AppendLine("\thost = google.com.br");
            sb.AppendLine("\t\tport = 81");
            sb.AppendLine("[section2]");
            sb.AppendLine("\thost = google.com.br");
            sb.AppendLine("\t\tport = 81");

            var cfg = IniFileReader.Load(sb.ToString());
            foreach (var x in new[] {"section", "section2"})
            {
                var section = cfg.GetSection(x);
                Assert.IsNotNull(section);
                Assert.AreEqual(x, section.Section);
                Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "host" && c.Value == "google.com.br"));
                Assert.IsTrue(section.Attributes.Any(c => c.AttributeName == "port" && c.Value == "81"));

            }
        }

    }
}
