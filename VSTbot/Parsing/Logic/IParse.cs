using System;
using System.Collections.Generic;
using System.Text;
using VSTbot.Parsing.Model;

namespace VSTbot.Parsing.Logic
{
    public interface IParse
    {
        public IEnumerable<Vacancy> GetVacancies(string paramString);
        public string InputData(string paramString);
        public string ParamsProcessing(string paramString);
        public string GetParamStringTemplate();
    }
}
