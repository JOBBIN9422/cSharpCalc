using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Operator : Token
    {
        private int priority;

        public Operator(string value) : base(value)
        {
            this.setPriority();
        }

        public void setPriority()
        {
            if (this.value.Equals("(") || this.value.Equals(")"))
            {
                this.priority = 0;
            }

            if (this.value.Equals("^"))
            {
                this.priority = 3;
            }
            else if (this.value.Equals("*") || this.value.Equals("/"))
            {
                this.priority = 2;
            }
            else if (this.value.Equals("+") || this.value.Equals("-"))
            {
                this.priority = 1;
            }

        }

        public int getPriority()
        {
            return this.priority;
        }

        public override string ToString()
        {
            return base.ToString() + ", priority = " + this.priority;
        }
    }
}
