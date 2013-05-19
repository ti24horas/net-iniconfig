namespace IniReaderTests.Bindings
{
    using System;
    using System.Linq;
    using System.Text;

    using IniReader;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding, Scope(Feature = "IniReaderReadStrings")]
    public class IniReaderReadStringsBinding
    {
        private readonly Context ctx;

        public IniReaderReadStringsBinding(Context ctx)
        {
            this.ctx = ctx;
            ctx.FileContents = new StringBuilder(string.Empty);
        }

        [When("I get unnamed section (.*)")]
        public void WhenIGetUnamedSection(string name)
        {
            var reader = IniFileReader.Load(this.ctx.FileContents.ToString());
            this.ctx.ReadSection = reader.GetSection(name);
        }

        [Then("The section id read is (.*)")]
        public void ThenSectionIdIs(string name)
        {
            Assert.AreEqual(name, this.ctx.ReadSection.Section);
        }

        [When(@"I get named section (.*) with name=(.*)")]
        public void WhenIGetNamedSectionSectionWithNameName(string id, string name)
        {
            var reader = IniFileReader.Load(this.ctx.FileContents.ToString());
            this.ctx.ReadSection = reader[id, name];
        }

        [When(@"I load string")]
        public void WhenILoadString()
        {
            try
            {
                this.ctx.Config = IniFileReader.Load(this.ctx.FileContents.ToString());
            }
            catch (Exception ex)
            {
                this.ctx.Error = ex;
            }
        }

        [Then(@"The section name is (.*)")]
        public void ThenTheSectionNameIsName(string name)
        {
            Assert.AreEqual(name, this.ctx.ReadSection.Name);
        }

        [Given(@"file with lines$")]
        public void GivenFileWithLines(string lines)
        {
            this.ctx.FileContents = new StringBuilder(lines);
        }

        [Then(@"The error type should be (.*)")]
        public void ThenTheErrorTypeShouldBeSystemInvalidOperationException(string exceptionName)
        {
            var errorType = this.ctx.Error.GetType();
            Assert.AreEqual(exceptionName, errorType.FullName);
        }

        [Then(@"the (.*) attribute is (.*)")]
        public void ThenTheAttributeIs(string attr, string value)
        {
            Assert.IsTrue(this.ctx.ReadSection.Attributes.Any(c => c.AttributeName == attr));
            var val = this.ctx.ReadSection[attr];
            Assert.AreEqual(value, val);
        }

        [Then(@"the attribute (.*) from section (.*) is (.*)")]
        public void ThenTheAttributeFromSectionIs(string attributeName, string section, string attributeValue)
        {
            Assert.AreEqual(attributeValue, this.ctx.Config[section][attributeName]);
        }

        [Then(@"the attribute (.*) from section (.*) contains null value")]
        public void ThenTheAttributeInexistentAttributeFromSectionSectionContainsNullValue(string attributeName, string section)
        {
            Assert.IsNull(this.ctx.Config[section][attributeName]);
        }

        public class Context
        {
            public ConfigSection ReadSection { get; set; }

            public StringBuilder FileContents { get; set; }

            public Exception Error { get; set; }

            public Config Config { get; set; }
        }
    }
}
