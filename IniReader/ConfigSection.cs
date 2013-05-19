namespace IniReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ConfigSection
    {
        private readonly IDictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>(StringComparer.InvariantCultureIgnoreCase);

        public ConfigSection(string sectionName, string name = null)
        {
            this.Section = sectionName;
            this.Name = name;
        }

        public string Section { get; private set; }

        public string Name { get; private set; }

        public IEnumerable<AttributeValue> Attributes
        {
            get
            {
                return this.attributes.Select(c => c.Value);
            }
        }

        public string this[string index]
        {
            get
            {
                return this.attributes.ContainsKey(index) ? this.attributes[index].Value : null;
            }
        }

        public void AddAttribute(AttributeValue attr)
        {
            this.attributes[attr.AttributeName] = attr;
        }

        internal static ConfigSection FromName(string namedSection)
        {
            var regex = new Regex(@"\t*\[(?<section>.+[^\""])(\s+\""(?<name>.+)\""){0,1}]", RegexOptions.Singleline);
            var values = regex.Match(namedSection);
            var sectionName = values.Groups["section"];
            var name = values.Groups["name"];
            if (!sectionName.Success)
            {
                throw new InvalidOperationException("section name not found: " + namedSection);
            }

            return new ConfigSection(sectionName.Value, name.Success ? name.Value : null);
        }
    }
}