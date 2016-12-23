using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Yaml.Model
{
    public class YamlDictionary : YamlObject, IEnumerable<KeyValuePair<string, YamlObject>>
    {
        private Dictionary<string, YamlObject> _items;

        public IEnumerable<string> Keys
            => _items.Keys;

        public IEnumerable<YamlObject> Values
            => _items.Values;

        public IEnumerator<KeyValuePair<string, YamlObject>> GetEnumerator()
            => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
