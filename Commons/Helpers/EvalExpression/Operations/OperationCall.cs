using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression.Operations
{
    ///<summary>Call a method or property</summary>
    [Serializable]
    public class OperationCall : IOperation
    {
        readonly string _id;
        readonly bool _thisCall;
        readonly int _paramCount;
        readonly bool _isProperty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Method or property name to call. This may contain multiple dots, for example: "Process.Module.Name"</param>
        /// <param name="thisCall">true if object must be popped from stack and its method/property invoked. false=invoke external function or property</param>
        /// <param name="isProperty">true=property, false=method</param>
        /// <param name="paramCount">number of method or property parameters on stack</param>
        public OperationCall(string id, bool thisCall, bool isProperty, int paramCount)
        {
            _id = id;
            _thisCall = thisCall;
            _isProperty = isProperty;
            _paramCount = paramCount;
        }

        /// Returns an number of entries added to stack by the operation. 
        public int StackBalance { get{return 1-((_thisCall?1:0)+_paramCount);} }

        /// Evaluate the operation against stack
        public void Eval(IEvaluationContext context, Stack<object> stack)
        {
            object[] p = OperationHelper.PopArray(stack, _paramCount);
            
            if (!_thisCall && !_id.Contains(".") && !_isProperty)
            {
                stack.Push(context.CallExternal(_id, p));
                return;
            }

            // ID is smth like x.y.z where we have no idea whether x is namespace,type, or object name. So we split it
            // and ask
            var parts = (!_id.Contains(".") && !string.IsNullOrEmpty(_id) && _paramCount != 0 && _isProperty)?((_id+".").Split('.')):_id.Split('.');
            
            IEnumerable<TypeObjectPair> typeAndObjects = null;
            if (_thisCall)
            {
                var o=stack.Pop();
                typeAndObjects = new TypeObjectPair[] { new TypeObjectPair(o.GetType(), o) };
            }
            
            object sub;
            bool success = false;
            for (int i=0;i<parts.Length;++i)
            {
                string currentPart = parts[i];
                if (typeAndObjects == null)
                {
                    string tn = string.Join(".", parts, 0, i + 1);
                    if (string.IsNullOrEmpty(tn))
                        typeAndObjects = context.GetNonameObjects();
                    else
                    {
                        if (context.TryGetExternal(tn, out sub))
                        {
                            success = true;
                            typeAndObjects = (sub == null) ? null : new[] {new TypeObjectPair(sub.GetType(), sub)};
                        }
                        else
                        {
                            var t = context.FindType(tn);
                            if (t != null)
                            {
                                typeAndObjects = new[] { new TypeObjectPair(t, null) };
                                success = true;
                            }
                        }
                    }
                }
                else
                {
                    success = false;
                    foreach (var to in typeAndObjects)
                    {
                        if (_isProperty || i != parts.Length - 1)
                            success = Utils.TryGetProperty(to.Object, to.Type, currentPart, (i == parts.Length - 1) ? p : null, context.AccessPrivate,out sub);
                        else
                            success = Utils.TryCallMethod(to.Object, to.Type, currentPart, (i == parts.Length - 1) ? p : null, context.AccessPrivate, out sub);
                        if (success)
                        {
                            typeAndObjects = null;
                            if (!object.ReferenceEquals(sub, null))
                                typeAndObjects = new[] {new TypeObjectPair(sub.GetType(), sub)};
                            break;
                        }
                    }
                    if (!success)
                    {
                        // Last possible chance for COM objects where reflection does not work, but calling might! 
                        if (i==parts.Length-1)
                        {
                            foreach (var to in typeAndObjects)
                            {
                                if (to.Object==null)
                                    continue;
                               
                                if (_isProperty)
                                {
                                    if (p.Length == 0)
                                    {
                                        stack.Push(Utils.GetProperty(to.Object, currentPart));
                                        return;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        stack.Push(Utils.CallMethod(to.Object, currentPart, p));
                                    }
                                    catch(Exception e)
                                    {
                                        var comex = e as System.Runtime.InteropServices.COMException;
                                        var miex = e as MissingMethodException;
                                        if (miex != null || (comex != null && ((uint)comex.ErrorCode == 0x80020006u || (uint)comex.ErrorCode == 0x80020005u)))
                                        {
                                            if (currentPart.StartsWith("set_", StringComparison.OrdinalIgnoreCase) && p.Length == 1)
                                            {
                                                Utils.SetPropertySimple(to.Object, currentPart.Substring(4), p[0]);
                                                stack.Push(null);
                                                return;
                                            }
                                            if (currentPart.StartsWith("get_", StringComparison.OrdinalIgnoreCase) && p.Length == 0)
                                            {
                                                stack.Push(Utils.GetProperty(to.Object, currentPart.Substring(4)));
                                                return;
                                            }
                                        }
                                        if (e is System.Reflection.TargetInvocationException && e.InnerException!=null)
                                            Utils.Rethrow(e.InnerException);
                                        Utils.Rethrow(e);
                                    }
                                    return;
                                }
                            }
                        }
                        throw new MissingMemberException("Failed to resolve '" + currentPart + "' with "+p.Length+" arguments");
                    }
                }
            }
            if (!success)
                throw new MissingMemberException("Failed to resolve '" + _id + "' with " + p.Length + " arguments");
            if (typeAndObjects==null)
                stack.Push(null);
            else
                foreach (var o in typeAndObjects)
                {
                    stack.Push(o.Object);
                    break;
                }
        }

        /// Returns a <see cref="T:System.String"/> that represents the current object.
        public override string ToString()
        {
            if (_isProperty)
            {
                string s = _thisCall ? "getProperty" : "getValue";
                if (_paramCount == 0)
                    return s+"(" + _id + ")";
                return s+"(" + _id + '[' + _paramCount + "])";
            }
            if (_thisCall)
                return "method." + _id + "(" + _paramCount + " args...)";
            return _id + "(" + _paramCount + " args...)";
        }
    }
}