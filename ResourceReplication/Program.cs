using System;
using System.Collections;
using System.ComponentModel.Design;
using System.IO;
using System.Resources;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Text;
using ResourceReplication.Functions;

namespace ResourceCreate
{
    class Program
    {
        private static IBase _base = new Base();

        private static string folder = string.Empty;

        static void Main(string[] args)
        {
            TextWriterTraceListener writer = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(writer);

            _base.Hello();

            foreach (var item in args)
            {
                folder = item;
                DirectoryInfo d = new DirectoryInfo(folder);
                FileInfo[] Files = d.GetFiles("*.resx").Where(x => !x.Name.Contains(".en.") && !x.Name.Contains(".es.")).ToArray();
                foreach (FileInfo file in Files)
                {
                    _base.Execute(Path.GetFileNameWithoutExtension(file.Name), folder, "Espanhol");
                    _base.Execute(Path.GetFileNameWithoutExtension(file.Name), folder, "Inglês");
                }
            }

            _base.Hello();
        }
    }
}
