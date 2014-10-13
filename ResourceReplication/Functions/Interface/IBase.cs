using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ResourceReplication.Functions
{
    public interface IBase
    {
        void Execute(string item, string folder, string culture);

        void Hello();
    }
}
