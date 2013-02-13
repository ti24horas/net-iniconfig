using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IniReader
{
    public class AttributeValue
    {
        public AttributeValue(string attributeName, string value = null)
        {
            this.AttributeName = attributeName;
            this.Value = value;
        }

        public string Value { get; private set; }

        public string AttributeName { get; private set; }

        public static AttributeValue LoadFromString(string value)
        {
            var regexAttributeValue = new Regex(@"(?<name>[^\s]+)\s{0,1}=[\s]{0,1}(?<value>.+)", RegexOptions.Compiled);
            var match = regexAttributeValue.Match(value);
            if (!match.Success)
            {
                //if not found a match with kvp, then this attribute does not 
                return new AttributeValue(value);
            }
            return new AttributeValue(match.Groups["name"].Value, match.Groups["value"].Value);
        }
    }
    public class ConfigSection
    {
        public string Section { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<AttributeValue> Attributes { get { return _attributes.Select(c => c.Value); } }
        readonly IDictionary<string, AttributeValue> _attributes = new Dictionary<string, AttributeValue>();
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
            var regex = new Regex(@"\[(?<section>.+[^\""])(\s+\""(?<name>.+)\""){0,1}]", RegexOptions.Singleline);
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
    public class Config
    {
        private readonly IDictionary<string, ConfigSection> _sections;

        public Config(IEnumerable<ConfigSection> sections)
        {
            this._sections = sections.ToDictionary(c => string.Format("{0}.{1}", c.Section, c.Name ?? string.Empty));
        }
        public ConfigSection GetSection(string sectionName, string name = null)
        {
            var nameToSearch = string.Format("{0}.{1}", sectionName, name ?? String.Empty);
            ConfigSection section;
            _sections.TryGetValue(nameToSearch, out section);
            return section;
        }
    }
    public class IniFileReader
    {
        class TokenReader
        {
            private string _currentToken;
            private readonly TextReader _reader;

            public TokenReader(TextReader reader)
            {
                this._reader = reader;

            }

            private bool IsComment()
            {
                return _currentToken.Trim().StartsWith("#");

            }

            private bool IsEmpty()
            {
                return _currentToken.Trim().Length == 0;
            }
            public bool IsSection()
            {
                var trimmed = _currentToken.Trim();
                return (trimmed.StartsWith("[") && trimmed.EndsWith("]"));
            }
            public bool IsAttribute()
            {
                return !IsSection() && !IsComment() && !IsEmpty();

            }
            public bool ReadNext()
            {
                _currentToken = _reader.ReadLine();
                return _currentToken != null;
            }
            public string GetValue()
            {
                return _currentToken;
            }
        }
        public static Config Load(string str)
        {
            return Load(new StringReader(str));

        }
        public static Config Load(TextReader reader, bool ignoreDuplicates = false)
        {
            var sections = new List<ConfigSection>();
            ConfigSection currentSection = null;
            IDictionary<string, AttributeValue> values = null;
            using (reader)
            {
                var tokenReader = new TokenReader(reader);
                while (tokenReader.ReadNext())
                {
                    if (tokenReader.IsSection())
                    {
                        if (currentSection != null)
                        {
                            sections.Add(currentSection);
                        }

                        currentSection = ConfigSection.FromName(tokenReader.GetValue());
                        values = new Dictionary<string, AttributeValue>();
                        
                    }
                    else if (tokenReader.IsAttribute())
                    {
                        if (currentSection == null)
                            throw new InvalidOperationException("attribute value without section");
                        var attr = AttributeValue.LoadFromString(tokenReader.GetValue());
                        if (!ignoreDuplicates)
                        {
                            if (values != null)
                            {
                                if (values.ContainsKey(attr.AttributeName))
                                    throw new DuplicateNameException("attributename");
                                values[attr.AttributeName] = attr;
                            }
                        }
                        currentSection.AddAttribute(attr);
                    }
                }
                if (currentSection != null)
                    sections.Add(currentSection);

            }
            return new Config(sections);
        }
        public static Config Load(Stream stream)
        {
            return Load(new StreamReader(stream));
        }
    }
}
