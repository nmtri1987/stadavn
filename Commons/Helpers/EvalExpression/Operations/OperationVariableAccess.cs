using System;
using System.Collections.Generic;
using System.Text;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Return a first defined expression in the |-separated list. List may contain variable names, values or expressions.
    /// For example ${a|b|0} = if variable a is defined, return a. Else, if b is defined, return b. Otherwise return 0</summary>
    [Serializable]
    public class OperationVariableAccess : IOperation
    {
        class VarAccessOption
        {
            public string Name;
            public object Value;
            public IOperation Expression;
            public override string ToString()
            {
                if (Name != null) return "name=" + Name + "";
                if (Expression != null) return "expr=" + Expression;
                return "value=" + Value + "";
            }
        }


        
        private readonly List<VarAccessOption> _options=new List<VarAccessOption>();

        /// Add value to the list
        public void AddValue(object value)  {_options.Add(new VarAccessOption {Value = value}); }

        /// Add expression to the list
        public void AddExpression(IOperation expr)  {   _options.Add(new VarAccessOption {Expression = expr});}

        /// Add variable name to the list
        public void AddName(string name)    {   _options.Add(new VarAccessOption { Name = name});}

        /// Returns number of entries added to stack by the operation. 
        public int StackBalance
        {
            get
            {
                int z = 0;
                foreach (var option in _options)
                    if (option.Expression != null)
                    {
                        z = option.Expression.StackBalance;
                        break;
                    }
                    else
                        z = 1;
                return z;

            }
        }
        
        /// Returns a string representation of the current object
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("getvar(");
            for (int i = 0; i < _options.Count; ++i)
            {
                if (i != 0)
                    sb.Append(" | ");
                sb.Append(_options[i].ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }


        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            string name = null;
            foreach (var option in _options)
            {
                object z;
                if (option.Name != null)
                {
                    name = option.Name;
                    if (context.TryGetValue(option.Name, out z))
                    {
                        stack.Push(z);
                        return;
                    }
                }
                else if (option.Expression != null)
                {
                    option.Expression.Eval(context, stack);
                    return;
                }
                else
                {
                    stack.Push(option.Value);
                    return;
                }
            }
            throw new KeyNotFoundException("Variable '"+name+"' is undefined");
        }
    }
}