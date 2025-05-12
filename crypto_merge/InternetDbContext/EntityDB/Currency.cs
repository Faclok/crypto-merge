using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetDatabase.EntityDB
{
    public class Currency: BaseEntity
    {

        public string Title { get; private set; } = string.Empty;

        public decimal RubCurrency { get; private set; } = 95m;

        public string CharCurrency { get; private set; } = string.Empty;

        public Currency(string title, decimal rubCurrency, string charCurrency)
        {
            Title = title;
            RubCurrency = rubCurrency;
            CharCurrency = charCurrency;
        }

        private Currency() { }
    }
}
