using VSTbot.Parsing.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VSTbot.Parsing.Logic
{
    public static class ParseSite
    {
        public static IEnumerable<Vacancy> GetVacancies(string siteName, string paramString)
        {
            HtmlWeb web = new HtmlWeb();
            string link = InputData(siteName, paramString);
            List<Vacancy> vacancies = new List<Vacancy>();

            var htmlDoc = web.Load(link);

            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//*//*[@id=\"vacancyListId\"]/ul/li/div/div/a");

            if (htmlNodes != null)
            {
                foreach (var item in htmlNodes)
                {
                    vacancies.Add(new Vacancy { Name = item.InnerText, Link = item.Attributes["href"].Value });
                }
            }
            return vacancies;
        }

        private static string InputData(string siteName, string paramString)
        {
            StringBuilder resultLink = new StringBuilder();

            if (siteName == null || paramString == null)
                return null;

            string siteLink = siteName switch
            {
                "Dou" => "https://jobs.dou.ua/vacancies/?",
                _ => null

            };
            //Null parameter here!!!!
            resultLink.Append(siteLink);
            string paramStringProcessed = ParamsProcessing(paramString);
            resultLink.Append(paramStringProcessed);

            return resultLink.ToString();
        }

        private static string ParamsProcessing(string paramString)
        {
            string[] paramArr = paramString.Split(new char[] { ' ' });

            switch (paramArr.Length)
            {
                case 1:
                    paramArr[0] = paramArr[0].Insert(0, "category=");
                    break;
                case 2:
                    paramArr[0] = paramArr[0].Insert(0, "category=");
                    paramArr[1] = paramArr[1].IdentifyCityOrExp();
                    break;
                case 3:
                    paramArr[0] = paramArr[0].Insert(0, "category=");
                    paramArr[1] = paramArr[1].IdentifyCityOrExp();
                    paramArr[2] = paramArr[2].IdentifyCityOrExp();
                    break;
                default:
                    break;
            }

            return String.Concat(paramArr);
        }

        private static string IdentifyCityOrExp(this string param)
        {
            if (param.Any(char.IsDigit))
                return param.Insert(0, "&exp=");
            else
                return param.Insert(0, "&city=");
        }
    }
}
