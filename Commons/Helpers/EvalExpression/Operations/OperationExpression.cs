using System;
using System.Collections.Generic;
using System.Text;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Evaluate expression, as a sequence of operations</summary>
    [Serializable]
    public class OperationExpression : IOperation
    {
        private readonly IOperation[] _data;

        /// Returns number of entries added to stack by the operation. 
        public int StackBalance
        {
            get
            {
                int r = 0;
                foreach (var element in _data)
                {
                    r += element.StackBalance;
                }
                return r;
            }
        }

        /// Constructor
        public OperationExpression(params IOperation[] e)
        {
            _data = e;
        }


        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            foreach (var element in _data)
                element.Eval(context, stack);
        }

        /// Return string representation of the expression
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("expr {");
            for (int i = 0; i < _data.Length; ++i)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append(_data[i].ToString());
            }
            sb.Append("}");
            return sb.ToString();

        }
    }
}