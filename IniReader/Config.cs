namespace IniReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Config
    {
        private readonly IDictionary<string, ConfigSection> sections;

        public Config(IEnumerable<ConfigSection> sections)
        {
            this.sections = sections.ToDictionary(c => string.Format("{0}.{1}", c.Section, c.Name ?? string.Empty), StringComparer.InvariantCultureIgnoreCase);
        }

        public ConfigSection this[string section]
        {
            get { return this.GetSection(section); }
        }

        public ConfigSection this[string section, string name]
        {
            get { return this.GetSection(section, name); }
        }

        public ConfigSection GetSection(string sectionName, string name = null)
        {
            var nameToSearch = string.Format("{0}.{1}", sectionName, name ?? string.Empty);
            ConfigSection section;
            this.sections.TryGetValue(nameToSearch, out section);
            return section;
        }
    }
}