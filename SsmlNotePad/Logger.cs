#if DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad
{
    public static class Logger
    {
        [DebuggerStepThrough]
        public static void WriteLine(string text)
        {
            if (Task.CurrentId.HasValue)
                Debug.WriteLine((String.IsNullOrWhiteSpace(text)) ? String.Format("Thread {0}, Task {1}:", Thread.CurrentThread.ManagedThreadId, Task.CurrentId.Value) :
                    String.Format("Thread {0}, Task {1}: {2}", Thread.CurrentThread.ManagedThreadId, Task.CurrentId.Value, text.Trim()));
            else
                Debug.WriteLine((String.IsNullOrWhiteSpace(text)) ? String.Format("Thread {0):", Thread.CurrentThread.ManagedThreadId) :
                    String.Format("Thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, text.Trim()));
        }

        [DebuggerStepThrough]
        public static void WriteLine(string format, params object[] args)
        {
            WriteLine((String.IsNullOrWhiteSpace(format)) ? format : String.Format(format, args));
        }

        [DebuggerStepThrough]
        public static void WriteLine(object value)
        {
            if (value == null || value is string)
                WriteLine(value as string);
            else
                Debug.WriteLine(value);
        }

        [ThreadStatic]
        public static List<StackFrame> _lastExecutionStack = new List<StackFrame>();

        //[DebuggerStepThrough]
        //public static void AssertCurrentMethod(int skipFrames, bool fNeedFileInfo = false) 
        //{
        //    StackTrace stackTrace = new StackTrace(skipFrames + 1, fNeedFileInfo);
        //    StackFrame[] allFrames = stackTrace.GetFrames();
        //    StackFrame currentThreadFrame = (allFrames.Length > 0) ? allFrames[0] : null;
        //    IEnumerable<StackFrame> stackFrames = allFrames.Skip(1);
        //    lock (_lastExecutionStack)
        //    {
        //        Thread.BeginCriticalRegion();
        //        try
        //        {
        //            int commonCount = _lastExecutionStack.AsEnumerable().Reverse().Zip(allFrames.Reverse(), (last, current) => new { Last = last, Current = current })
        //                .SkipWhile(a =>
        //                {
        //                    string ls = a.Last.GetFileName();
        //                    if (!String.IsNullOrEmpty(ls))
        //                    {
        //                        string cs = a.Current.GetFileName();
        //                        if (!String.IsNullOrEmpty(cs))
        //                        {
        //                            if (cs != ls)
        //                                return false;
        //                            int li = a.Last.GetFileLineNumber();
        //                            if (li > 0)
        //                            {
        //                                int ci = a.Current.GetFileLineNumber();
        //                                if (ci > 0)
        //                                    return li == ci;
        //                            }
        //                        }
        //                    }
        //                    MethodBase lm = a.Last.GetMethod();
        //                    MethodBase cm = a.Current.GetMethod();
        //                    return lm.ReflectedType.Equals(cm.ReflectedType) && lm.Equals(cm);
        //                }).Count();

        //            if (commonCount == _lastExecutionStack.Count && commonCount == allFrames.Length)
        //                return;

        //            foreach (StackFrame f in _lastExecutionStack.Skip(commonCount).Reverse())
        //            {
        //                Debug.Unindent();
        //                Debug.WriteLine("Exit {0}", f.AsString());
        //            }
        //            foreach (StackFrame f in stackFrames.Reverse().Skip(commonCount))
        //            {
        //                Debug.WriteLine("Enter {0}", f.AsString());
        //                Debug.Indent();
        //            }
        //            _lastExecutionStack.Clear();
        //            _lastExecutionStack.AddRange(stackFrames);
        //            if (currentThreadFrame != null && commonCount < allFrames.Length)
        //            {
        //                Debug.Indent();
        //                if (Task.CurrentId.HasValue)
        //                    Debug.WriteLine("Thread {0}, Task {1}: Enter {1}", Thread.CurrentThread.ManagedThreadId, Task.CurrentId.Value, currentThreadFrame.AsString());
        //                else
        //                    Debug.WriteLine("Thread {0}: Enter {1}", Thread.CurrentThread.ManagedThreadId, currentThreadFrame.AsString());
        //            }
        //        }
        //        finally { Thread.EndCriticalRegion(); }
        //    }
        //}

        //[DebuggerStepThrough]
        //public static void AssertCurrentMethod(bool fNeedFileInfo = false) { AssertCurrentMethod(1, fNeedFileInfo); }

        [DebuggerStepThrough]
        internal static void Indent() { Debug.Indent(); }

        [DebuggerStepThrough]
        public static string AsString(this StackFrame stackFrame)
        {
            MethodBase method = stackFrame.GetMethod();
            StringBuilder sb = new StringBuilder(method.ReflectedType.FullName);
            if (method is MethodInfo)
            {
                sb.Append((method as MethodInfo).ReturnType.FullName);
                sb.Append(" ");
                sb.Append(method.ReflectedType.FullName);
                sb.Append(".");
                sb.Append(method.Name);
                if (method.ContainsGenericParameters)
                    sb.AppendFormat("<{0}>", String.Join(", ", method.GetGenericArguments().Select(a => a.ToString()).ToArray()));
            } else if (method is ConstructorInfo)
            {
                sb.Append(method.ReflectedType.FullName);
                sb.Append(".");
                sb.Append(method.Name);
                if (method.ContainsGenericParameters)
                    sb.AppendFormat("<{0}>", String.Join(", ", method.GetGenericArguments().Select(a => a.ToString()).ToArray()));
                sb.Append(" new");
            }
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0)
                sb.Append("()");
            else if (parameters.Length == 1)
                sb.AppendFormat("({0})", parameters[0].ToString());
            else
                sb.AppendFormat("({0})", String.Join(", ", parameters.Select(a => a.ToString()).ToArray()));
            string s = stackFrame.GetFileName();
            if (!String.IsNullOrWhiteSpace(s))
            {
                sb.AppendFormat("; {0}", s);
                int lineNumber = stackFrame.GetFileLineNumber();
                if (lineNumber > -1)
                {
                    sb.AppendFormat(", Line {0}", lineNumber);
                    int colNumber = stackFrame.GetFileColumnNumber();
                    if (colNumber > -1)
                        sb.AppendFormat(", Column {0}", colNumber);
                }
            }
            return sb.ToString();
        }

        [DebuggerStepThrough]
        public static void Unindent() { Debug.Unindent(); }
    }
}

#endif