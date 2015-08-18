using Newtonsoft.Json;
using ResourceReplication.Class;
using ResourceReplication.Functions.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ResourceReplication.Functions
{
    /// <summary>
    /// http://mymemory.translated.net/api/get?q=oi%20mundo!&langpair=pt-br|en
    /// </summary>
    public class Base : IBase
    {
        private StringBuilder sb = new StringBuilder();
        private const string traco = "--------------------------------------------------------------------------------";
        private const string copyright = "--------------------------- marcos.tomazini@gmail.com --------------------------";
        private const string generate = "-------------------------- gerado por Marcos Tomazini --------------------------";
        private const string type = "------------------------ script de migração de resources -----------------------";
        private const string duplicate = "------------------------------- itens duplicados -------------------------------";
        private Translate translate;

        public void Execute(string item, string folder, string culture)
        {
            translate = new Translate();

            Console.WriteLine(string.Format("Gerando Padrão {0} Para Item {1}...", culture, string.Format("{0}.resx", item)));

            ResXResourceWriter rsxw = new ResXResourceWriter(string.Format(@"{0}{1}.{2}.resx", folder, item, culture.ToUpper().Contains("ESPANHOL") ? "es" : "en"));
            ResXResourceReader rr = new ResXResourceReader(string.Format(@"{0}{1}.resx", folder, item));
            Generate(rsxw, rr, culture);
            rsxw.Close();

            ShowInformation(item, folder, culture);
        }

        public virtual void Hello()
        {
            Copyright();
            Console.WriteLine(sb.ToString());
        }

        private void Generate(ResXResourceWriter rsxw, ResXResourceReader rr, string culture)
        {
            rr.UseResXDataNodes = true;
            IDictionaryEnumerator dict = rr.GetEnumerator();
            SortedDictionary<string, ResXDataNode> sortedRSXR = new SortedDictionary<string, ResXDataNode>();

            while (dict.MoveNext())
            {
                ResXDataNode node = (ResXDataNode)dict.Value;
                sortedRSXR.Add(node.Name, node);
            }

            var resources = new List<Resource>();
            foreach (var item in sortedRSXR)
            {
                resources.Add(new Resource()
                {
                    Name = (item.Value).Name,
                    Value = Convert.ToString((item.Value).GetValue((ITypeResolutionService)null))
                });
            }

            var duplicates = resources.GroupBy(x => x.Value)
              .Where(g => g.Count() > 1)
              .Select(y => y)
              .ToList();

            if (duplicates.Any())
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(duplicate);
                foreach (var items in duplicates)
                {
                    foreach (var item in items)
                    {
                        ShowDuplicates(item);
                    }
                    Console.WriteLine();
                }
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(traco);
                Console.ResetColor();
            }

            foreach (var item in sortedRSXR)
            {
                ResXDataNode newNode = new ResXDataNode((item.Value).Name, Translate(string.Format("{0}",(item.Value).GetValue((ITypeResolutionService)null)), culture));
                //ResXDataNode newNode = new ResXDataNode((item.Value).Name, string.Format("{0} - {1}", culture, (item.Value).GetValue((ITypeResolutionService)null)));
                //ResXDataNode newNode = new ResXDataNode((item.Value).Name, string.Format("{0} - {1}", culture, (item.Value).GetValue((ITypeResolutionService)null)));
                newNode.Comment = (item.Value).Comment;
                rsxw.AddResource(newNode);
            }
        }

        private void ShowDuplicates(Resource resource)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(string.Format("Nome: {0} - Valor: {1}", resource.Name, resource.Value));
        }

        private string Translate(string text, string toCulture)
        {
            //var traducao = string.Format("{0} - {1}", toCulture, text);
            var traducao = translate.Execute(text, toCulture);

            return traducao;
        }

        private void ShowInformation(string item, string folder, string culture)
        {
            Console.WriteLine(string.Format("Gerado Padrão {0}", culture));
            Console.Write("Caminho");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(string.Format(" {0}", string.Format(@"{0}{1}.{2}.resx", folder, item, culture.ToUpper().Contains("ESPANHOL") ? "es" : "en"), folder, item));
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
