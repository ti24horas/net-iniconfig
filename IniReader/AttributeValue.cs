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
            var regexAttributeValue = new Regex(@"\t*(?<name>[^\s]+)\s{0,1}=[\s\t]{0,1}(?<value>.+)", RegexOptions.Compiled);
            var match = regexAttributeValue.Match(value);
            if (!match.Success)
            {
                //if not found a match with kvp, then this attribute does not 
                return new AttributeValue(value.Trim(new[]{' ', '\t'}));
            }
            return new AttributeValue(match.Groups["name"].Value, match.Groups["value"].Value);
        }
    }
}