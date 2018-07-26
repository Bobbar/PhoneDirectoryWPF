using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Containers
{
    public sealed class Setting
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public Setting() { }

        public Setting(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
