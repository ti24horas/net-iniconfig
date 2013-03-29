using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IniReader
{
    public class ConfigSection
    {
        public string Section { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<AttributeValue> Attributes { get { return _attributes.Select(c => c.Value); } }
        readonly IDictionary<string, AttributeValue> _attributes = new Dictionary<string, AttributeValue>(StringComparer.InvariantCultureIgnoreCase);
        public ConfigSection(string sectionName, string name = null)
        {
            this.Section = sectionName;
            this.Name = name;
        }
        public string this[string index]
        {
            get
            {
                return this._attributes.ContainsKey(index) ? this._attributes[index].Value : null;
            }
            set
            {
                this._attributes[index] = new AttributeValue(index, value);
            }
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

        public void AddAttribute(AttributeValue attr)
        {
            this._attributes[attr.AttributeName] = attr;
        }
    }
}