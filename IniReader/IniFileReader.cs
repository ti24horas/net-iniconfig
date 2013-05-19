namespace IniReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Class used to read ini file
    /// </summary>
    public class IniFileReader
    {
        /// <summary>
        /// chars used as comments at line start
        /// </summary>
        private const string CommentChars = "#;";

        /// <summary>
        /// Loads a ini file from string
        /// </summary>
        /// <param name="str">The content representing ini file</param>
        /// <returns>The configuration reader</returns>
        public static Config Load(string str)
        {
            using (var reader = new StringReader(str))
            {   
                return Load(reader);
            }
        }
        
        /// <summary>
        /// Loads a ini file from TextReader
        /// </summary>
        /// <param name="reader">The text reader containing ini file contents</param>
        /// <param name="ignoreDuplicates">If true, ignores duplicate attributes in section</param>
        /// <returns>The configuration reader</returns>
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
                        {
                            throw new InvalidOperationException("attribute value without section");
                        }

                        var attr = AttributeValue.LoadFromString(tokenReader.GetValue());
                        if (!ignoreDuplicates)
                        {
                            if (values.ContainsKey(attr.AttributeName))
                            {
                                throw new DuplicateNameException("attributename");
                            }

                            values[attr.AttributeName] = attr;
                        }

                        currentSection.AddAttribute(attr);
                    }
                }

                if (currentSection != null)
                {
                    sections.Add(currentSection);
                }
            }

            return new Config(sections);
        }

        /// <summary>
        /// Returns a configuration containing ini file contents
        /// </summary>
        /// <param name="filename">The filename containing ini file contents</param>
        /// <returns>The configuration reader</returns>
        public static Config LoadFile(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                return Load(sr);
            }
        }

        private class TokenReader
        {
            private readonly TextReader reader;

            private string currentToken;

            public TokenReader(TextReader reader)
            {
                this.reader = reader;
            }

            public bool IsSection()
            {
                var trimmed = this.currentToken.Trim(new[] { ' ', '\t' });
                return trimmed.StartsWith("[") && trimmed.EndsWith("]");
            }

            public bool IsAttribute()
            {
                if (this.IsSection())
                {
                    return false;
                }

                if (this.IsComment())
                {
                    return false;
                }

                return !this.IsEmpty();
            }

            public bool ReadNext()
            {
                this.currentToken = this.reader.ReadLine();
                return this.currentToken != null;
            }

            public string GetValue()
            {
                return this.currentToken;
            }

            private bool IsComment()
            {
                return CommentChars.Any(a => this.currentToken.Trim(new[] { ' ', '\t' }).StartsWith(a.ToString(CultureInfo.InvariantCulture)));
            }

            private bool IsEmpty()
            {
                return this.currentToken.Trim(new[] { ' ', '\t' }).Length == 0;
            }
        }
    }
}
