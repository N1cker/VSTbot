using VSTbot.Parsing.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace VSTbot.Parsing.Logic
{
    public class ParseSite
    {
        private IParse parse;

        public ParseSite(string siteName)
        {
            switch(siteName)
            {
                case "Dou":
                    parse = new DouParse(); 
                    break;
                case "Djinni":
                    parse = new DjinniParse();
                    break;
                default:
                    break;

            };
        }

        public async Task<string> GetParamTemplateAsync()
        {
            if (parse == null)
                return null;

            return await parse.GetParamStringTemplateAsync();
        }

        public async Task<string> GetResultAsync(string paramString)
        {
            if(parse == null)
                return null;

            List<Vacancy> result = (List<Vacancy>)await parse.GetVacanciesAsync(paramString);

            return await Task.Run(() => ListToString(result));
        }

        public async Task<string> ListToString(List<Vacancy> vacancies)
        {
            return await Task.Run(() =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Found " + vacancies.Count + " vacancies:\n");
                foreach (Vacancy vacancy in vacancies)
                {
                    stringBuilder.Append(vacancy.Name);
                    stringBuilder.Append("\n");
                    stringBuilder.Append(vacancy.Link);
                    stringBuilder.Append("\n");
                }
                return stringBuilder.ToString();
            } 
            );
        }

    }
}
