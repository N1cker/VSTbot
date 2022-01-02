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
                case "LinkedIn":
                    parse = new LinkedInParse();
                    break;
                case "Djinni":
                    parse = new DjinniParse();
                    break;
                default:
                    break;

            };
        }
        public string GetParamTemplate()
        {
            if (parse == null)
                return null;

            return parse.GetParamStringTemplate();
        }

        public string GetResult(string paramString)
        {
            if(parse == null)
                return null;

            List<Vacancy> result = (List<Vacancy>)parse.GetVacancies(paramString);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Found " + result.Count + " vacancies:\n");
            foreach (Vacancy vacancy in result)
            {
                stringBuilder.Append(vacancy.Name);
                stringBuilder.Append("\n");
                stringBuilder.Append(vacancy.Link);
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

    }
}
