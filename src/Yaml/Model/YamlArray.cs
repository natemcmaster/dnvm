using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Yaml.Model
{
    public class YamlArray : YamlObject
    {
        public YamlObject[] Items { get; }

        public int Count => Items.Length;
        public YamlObject this[int index] => Items[index];
    }
}
