using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Token
    {
        protected string value;

        public Token(string value)
        {
            this.value = value;
        }

        public string getValue()
        {
            return this.value;
        }

        public override string ToString()
        {
            return "Token: " + this.value;
        }
    }
}
