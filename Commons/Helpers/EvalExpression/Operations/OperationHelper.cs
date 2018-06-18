using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    /// Helpful utilities used by different operations
    public static class OperationHelper
    {
        /// <summary>
        /// Get top objects from stack in reverse order (i.e. the topmost object on stack is the last array item)
        /// </summary>
        /// <param name="stack">Stack</param>
        /// <param name="count">Number of objects to pop</param>
        /// <returns>Retrieved objects</returns>
        public static object[] PopArray(Stack<object> stack, int count)
        {
            object[] p = new object[count];
            for (int i = 0; i < count; ++i)
                p[p.Length - 1 - i] = stack.Pop();
            return p;
        }

        /// <summary>
        /// Try to resolve typename to type. This is a helper function on top of <see cref="IEvaluationContext.FindType"/> that parses [] and ? suffix
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type ResolveType(IEvaluationContext context, string name)
        {
            bool nullable = false;
            bool array = false;
            if (name.EndsWith("[]", StringComparison.Ordinal))
            {
                array = true;
                name = name.Substring(0, name.Length - 2);
            }
            if (name.EndsWith("?", StringComparison.Ordinal))
            {
                nullable = true;
                name = name.Substring(0, name.Length - 1);
            }
            Type t = context.FindType(name);
            if (t != null)
            {
                if (nullable)
                    t = typeof(Nullable<>).MakeGenericType(t);
                if (array)
                    t = t.MakeArrayType();
            }
            return t;
        }
    }
}