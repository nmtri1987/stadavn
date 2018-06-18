using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Cast the object on top of the stack to the given type, then push it back</summary>
    [Serializable]
    public class OperationTypeOf : IOperation
    {
        readonly string _typeName;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeName">Type name. May contain ? and/or suffix. For example, int[] or int? or int or System.Int32</param>
        public OperationTypeOf(string typeName)
        {
            _typeName = typeName;
        }

        /// Returns an number of entries added to stack by the operation. 0 in this case
        public int StackBalance { get { return 1; } }

        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            Type t = OperationHelper.ResolveType(context, _typeName);
            if (t == null)
                throw new TypeLoadException("Failed to resolve type '" + _typeName + "'");
            stack.Push(t);
        }

        /// Returns a <see cref="T:System.String"/> that represents the current object.
        public override string ToString()
        {
            return "typeof(" + _typeName + ")";
        }
    }
}