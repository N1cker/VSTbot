using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSTbot.Parsing.Model;

namespace VSTbot.Parsing.Logic
{
    public class DouParse : IParse
    {
        public string GetParamStringTemplate()
        {
            return "Write a couple of parameters in the given format:\n" +
                    "\"job title\" \"city\" \"work experience\"\n" +
                    "Example:\nJava Lviv 1-3\\.Net 5plus Kiev";
        }

        public IEnumerable<Vacancy> GetVacancies(string paramString)
        {
            HtmlWeb web = new HtmlWeb();
            string link = InputData(paramString);
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

        public string InputData(string paramString)
        {
            StringBuilder resultLink = new StringBuilder();

            if (paramString == null)
                return null;

            string siteLink = "https://jobs.dou.ua/vacancies/?";
            //Null parameter here!!!!
            resultLink.Append(siteLink);
            string paramStringProcessed = ParamsProcessing(paramString);
            resultLink.Append(paramStringProcessed);

            return resultLink.ToString();
        }

        public string ParamsProcessing(string paramString)
        {
            string[] paramArr = paramString.Split(new char[] { ' ' });

            switch (paramArr.Length)
            {
                case 1:
                    paramArr[0] = paramArr[0].Insert(0, "category=");
                    break;
                case 2:
                    paramArr[0] = paramArr[0].Insert(0, "category=");
                    paramArr[1] = IdentifyCityOrExp(paramArr[1]);
                    break;
                case 3:
                    paramArr[0] = paramArr[0].Insert(0, "category=");
                    paramArr[1] = IdentifyCityOrExp(paramArr[1]);
                    paramArr[2] = IdentifyCityOrExp(paramArr[2]);
                    break;
                default:
                    break;
            }

            return String.Concat(paramArr);
        }

        private string IdentifyCityOrExp(string param)
        {
            if (param.Any(char.IsDigit))
                return param.Insert(0, "&exp=");
            else
                return param.Insert(0, "&city=");
        }

    }
}
