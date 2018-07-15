using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.Containers;

namespace PhoneDirectoryWPF.UI
{
    public sealed class ExtensionDeletedEventArgs : EventArgs
    {
        public Extension Extension;

        public ExtensionDeletedEventArgs() : base() { }

        public ExtensionDeletedEventArgs(Extension deletedExtension)
        {
            Extension = deletedExtension;
        }
    }
}
