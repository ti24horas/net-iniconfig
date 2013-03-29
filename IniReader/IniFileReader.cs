using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace IniReader
{
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
                return _currentToken.Trim(new[] {' ', '\t'}).StartsWith("#");

            }

            private bool IsEmpty()
            {
                return _currentToken.Trim(new[] {' ', '\t'}).Length == 0;
            }
            public bool IsSection()
            {
                var trimmed = _currentToken.Trim(new[] {' ', '\t'});
                return (trimmed.StartsWith("[") && trimmed.EndsWith("]"));
            }
            public bool IsAttribute()
            {
                var isSection = IsSection();
                if(isSection) return false;
                var isComment = IsComment();
                if(isComment)return false;
                var isEmpty = IsEmpty();
                if (isEmpty)
                    return false;
                return true;
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
            using (var reader = new StringReader(str))
            return Load(reader);
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
                            if (values.ContainsKey(attr.AttributeName))
                                throw new DuplicateNameException("attributename");
                            values[attr.AttributeName] = attr;
                        }
                        currentSection.AddAttribute(attr);
                    }
                }
                if (currentSection != null)
                    sections.Add(currentSection);

            }
            return new Config(sections);
        }

        public static Config LoadFile(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                return Load(sr);
            }
        }
        
    }
}
