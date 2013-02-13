using System;
using System.Collections.Generic;
using System.Linq;

namespace IniReader
{
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
}