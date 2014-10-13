using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ResourceReplication.Functions
{
    public class Base : IBase
    {
        private StringBuilder sb = new StringBuilder();
        private const string traco = "--------------------------------------------------------------------------------";
        private const string copyright = "--------------------------- marcos.tomazini@gmail.com --------------------------";
        private const string generate = "-------------------------- gerado por Marcos Tomazini --------------------------";
        private const string type = "------------------------ script de migração de resources -----------------------";
        private string culture = string.Empty;

        public void Execute(string item, string folder, string culture)
        {
            this.culture = culture;
            Console.WriteLine(string.Format("Gerando Padrão {0} Para Item {1}...", culture, string.Format("{0}.resx", item)));

            ResXResourceWriter rsxw = new ResXResourceWriter(string.Format(@"{0}{1}.es.resx", folder, item));
            ResXResourceReader rr = new ResXResourceReader(string.Format(@"{0}{1}.resx", folder, item));
            Generate(rsxw, rr);
            rsxw.Close();

            ShowInformation(item, folder);
        }

        public virtual void Hello()
        {
            Copyright();
            Console.WriteLine(sb.ToString());
        }

        private void Generate(ResXResourceWriter rsxw, ResXResourceReader rr)
        {
            rr.UseResXDataNodes = true;
            IDictionaryEnumerator dict = rr.GetEnumerator();
            while (dict.MoveNext())
            {
                ResXDataNode node = (ResXDataNode)dict.Value;

                ResXDataNode newNode = new ResXDataNode(node.Name, string.Format("{0} - {1}", culture, node.GetValue((ITypeResolutionService)null)));
                newNode.Comment = node.Comment;

                rsxw.AddResource(newNode);
            }
        }

        private void ShowInformation(string item, string folder)
        {
            Console.WriteLine(string.Format("Gerado Padrão {0}", culture));
            Console.Write("Caminho");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(string.Format(" {0}", string.Format(@"{0}{1}.resx", folder, item)));
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine();
        }
        
        private string Copyright()
        {
            sb.Clear();
            sb.AppendLine(traco);
            sb.AppendLine(type);
            sb.AppendLine(generate);
            sb.AppendLine(copyright);
            sb.AppendLine(traco);

            return sb.ToString();
        }
    }
}
