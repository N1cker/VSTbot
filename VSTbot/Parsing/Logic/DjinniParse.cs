using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSTbot.Parsing.Model;

namespace VSTbot.Parsing.Logic
{
    public class DjinniParse : IParse
    {
        public async Task<string> GetParamStringTemplateAsync()
        {
            return await Task.Run(() => "Write a couple of parameters in the given format:\n" +
                    "\"specialization\" \"work experience\"\n" +
                    "Example:\nJava 3 \\ .Net 6");
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync(string paramString)
        {
            return await Task.Run(() => GetVacancies(paramString));
        }
        public IEnumerable<Vacancy> GetVacancies(string paramString)
        {
            HtmlWeb web = new HtmlWeb();
            string link = InputData(paramString);
            List<Vacancy> vacancies = new List<Vacancy>();

            var htmlDoc = web.Load(link);

            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'list-jobs__title')]/a");

            if (htmlNodes != null)
            {
                foreach (var item in htmlNodes)
                {
                    vacancies.Add(new Vacancy { Name = item.InnerText.Trim(), Link = item.Attributes["href"].Value.Insert(0, "https://djinni.co/") });
                }
            }
            return vacancies;
        }

        public string InputData(string paramString)
        {
            StringBuilder resultLink = new StringBuilder();

            if (paramString == null)
                return null;

            string siteLink = "https://djinni.co/jobs/keyword-";
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
                    paramArr[0] = SpecNameToParam(paramArr[0]);
                    break;
                case 2:
                    paramArr[0] = SpecNameToParam(paramArr[0]);
                    paramArr[1] = ExpNameToParam(paramArr[1]);
                    break;
                default:
                    break;
            }

            return String.Concat(paramArr);
        }
        private string SpecNameToParam(string param)
        {
            string result;
            Dictionary<string, string> dict =
            new Dictionary<string, string>();

            dict.Add("C#", "dotnet");
            dict.Add(".Net", "dotnet");
            dict.Add("C++", "cplusplus");

            if (dict.ContainsKey(param))
                result = dict[param];
            else
                result = param.ToLower();
            result = result.Insert(result.Length, "/");
            return result;
        }
        private string ExpNameToParam(string param)
        {
            if (param.Any(char.IsDigit))
            {
                param = YearsOfExpirienceToParam(param);
                return param.Insert(0, "?exp_level=");
            }
            else
                return null;
        }

        private string YearsOfExpirienceToParam(string param)
        {
            if (param == null)
                return null;

            int numbParam;
            bool success = Int32.TryParse(param, out numbParam);
            if (!success)
                return null;

            string resultParam = numbParam switch
            {
                int n when n >= 0 && n < 1 => "no_exp",
                int n when n >= 1 && n < 2 => "1y",
                int n when n >= 2 && n < 3 => "2y",
                int n when n >= 3 && n < 5 => "3y",
                int n when n >= 5 => "5y",
                _ => null
            };

            return resultParam;
        }
    }
}
