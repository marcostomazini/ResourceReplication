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
    public class Translate
    {
        private int StatusTranslate = 200;
        private ResourceReplication.Contexto.Contexto contexto;

        public Translate()
        {
            contexto = new ResourceReplication.Contexto.Contexto();
        }

        public string Execute(string text, string toCulture)
        {
            var culture = toCulture.ToUpper().Contains("ESPANHOL") ? "es" : "en";

            var query = contexto.Textos.Where(x => x.Texto == text && x.Idioma == culture);

            if (query.Any())
            {
                //Console.WriteLine("Tradução existente no BD");
                return query.FirstOrDefault().Traducao;
            }
            else
            {
                //Console.WriteLine("Traduzindo Online...");
                var traducao = TranslateOnTheLine(text, culture);
                
                return traducao;
            }
        }

        private void AdicionarNovaTraducao(string text, string traducao, string culture)
        {
            contexto.Textos.Add(new Textos()
            {
                Texto = text,
                Traducao = traducao,
                Idioma = culture
            });

            contexto.SaveChanges();
        }

        private string TranslateOnTheLine(string text,string culture)
        {
            var url = "http://mymemory.translated.net/api/get?q={0}&langpair=pt-br|{1}";

            //var url = "https://translated-mymemory---translation-memory.p.mashape.com/api/get?key=3dfc770cd06f81f6451b&langpair=pt-br%7C{1}&mt=1&of=json&q={0}";

            if (StatusTranslate == 200)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(url, text, culture));

                request.Method = "GET";

                request.Headers.Add("X-Mashape-Key", "68auyH9m7AmshPkopQT9CBqS45G2p1pRjeujsn0y9YlTF9elT4");
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json";

                // Set credentials to use for this request.
                string MyProxyHostString = "proxy.db1.com.br";
                int MyProxyPort = 8080;
                request.Proxy = new WebProxy(MyProxyHostString, MyProxyPort);
                request.Proxy.Credentials = new NetworkCredential("marcos.tomazini", "t0mazini#");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string result;
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    result = rdr.ReadToEnd();
                }
                response.Close();

                var translate = JsonConvert.DeserializeObject<TranslateDto>(result);

                StatusTranslate = translate.responseStatus;
                if (translate.responseData.match > 0.4)
                {
                    AdicionarNovaTraducao(text, translate.responseData.translatedText, culture);
                    return translate.responseData.translatedText;
                }
            }

            return string.Format("{0} - {1}", culture.ToUpper().Contains("ES") ? "ESPANHOL" : "INGLÊS", text);
        }
    }

    public class TranslateDto
    {
        public ResponseTranslateDto responseData { get; set; }
        public int responseStatus { get; set; }
    }

    public class ResponseTranslateDto
    {
        public string translatedText { get; set; }
        public double match { get; set; }
    }

}
