using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VSTbot.Parsing.Model;

namespace VSTbot.Parsing.Logic
{
    public interface IParse
    {
        public Task<IEnumerable<Vacancy>> GetVacanciesAsync(string paramString);
        public string InputData(string paramString);
        public string ParamsProcessing(string paramString);
        public Task<string> GetParamStringTemplateAsync();
    }
}
