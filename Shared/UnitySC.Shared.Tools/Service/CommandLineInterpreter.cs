using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnitySC.Shared.Tools.Service
{
    /// <summary>
    /// This is a Basic Command Line Interpreter, for Service in Console Mode.
    /// </summary>
    public class CommandLineInterpreter
    {

        public string ExitCommandText { get; set; } = "exit";


        private SortedDictionary<string, Function> _functions = new SortedDictionary<string, Function>();
        public SortedDictionary<string, Function> Functions { get { return _functions; } }


        /// <summary>
        /// Register a Function. the name is not case sensitive.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fonction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Function RegisterCommand(string name, string description, Action<string[]> fonction)
        {

            if (Functions.ContainsKey(name))
            {
                throw new ArgumentException($"The function {name} already exists.");
            }
            var f = new Function(name, description, fonction);


            Functions.Add(name.ToLower(), f);

            return f;
        }

        public Function RegisterCommand(string name, Action<string[]> fonction)
        {
            return RegisterCommand(name, "", fonction);
        }

        private const string Regexstring = "(\"[^\"]+\"|[^\\s\"]+)";
        private readonly Regex _regex = new Regex(Regexstring);


        public ExecuteFunction InterpretCommand(string command)
        {

            if (string.IsNullOrWhiteSpace(command)) return null;

            ExecuteFunction executeFunction = null;

            List<string> s = SplitCommad(command);

            string cmd = s.Any() ? s.First() : null;
            List<string> param = s.Skip(1).ToList();




            if (Functions.TryGetValue(cmd.ToLower(), out var function))
            {
                executeFunction = new ExecuteFunction(function);

                var parmRequested = executeFunction.Parameters.Where(p => !p.Parameter.Optional).ToList();
                var parmOptional = executeFunction.Parameters.Where(p => p.Parameter.Optional).ToList();

                if ((param.Count > function.Parameters.Count) || (parmRequested.Count() > param.Count))
                {

                    throw new Exception("incorrect parameter number.");
                }


                parmRequested.ForEach(p => { p.SetValue(param.First()); param.RemoveAt(0); });

                parmOptional = parmOptional.Take(param.Count).ToList();

                parmOptional.ForEach(p => { p.SetValue(param.First()); param.RemoveAt(0); });

            }
            else
            {
                throw new Exception("Unknown command.");
            }

            return executeFunction;
        }

        public bool IsExitCommand(string command)
        {

            if (string.IsNullOrWhiteSpace(command)) return false;

            List<string> s = SplitCommad(command);

            string cmd = s.Any() ? s.First() : null;

            return (ExitCommandText.ToLower() == cmd.ToLower());
        }

        public bool ExecuteCommand(ExecuteFunction executeFunction)
        {
            string[] param = executeFunction.Parameters.Select(p => p.GetValueString()).ToArray();

            executeFunction.Function.Fonction(param);

            return true;
        }


        private List<string> SplitCommad(string command)
        {
            return _regex.Split(command).Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
        }

        public string ToHelp()
        {
            return string.Join(Environment.NewLine + Environment.NewLine, Functions.Select(f => f.Value.ToHelp()));
        }

        public enum ParameterType
        {
            String = 0,
            Integer = 1,
            Float = 2,
            Bool = 3,
        }

        public class Function
        {
            private string _name;
            public string Name { get => _name; }

            private string _description;
            public string Description { get => _description; }

            private Action<string[]> _fonction;
            public Action<string[]> Fonction { get => _fonction; }

            private List<Parameter> _parameters = new List<Parameter>();
            public List<Parameter> Parameters { get => _parameters; }

            public Function(string name, string description, Action<string[]> fonction)
            {
                _name = name;
                _description = description;
                _fonction = fonction;

            }


            public Function AddParmeter(string name, ParameterType type)
            {
                return AddParmeter(name, type, false, string.Empty);
            }

            public Function AddParmeter(string name, ParameterType type, string description)
            {
                return AddParmeter(name, type, false, description);
            }


            public Function AddParmeter(string name, ParameterType type, bool optional, string description)
            {
                if (_parameters.Any(p => p.Name == name))
                {
                    throw new ArgumentException($"The parameter {name} already exists in the function {this.Name}.");

                }

                if ((_parameters.Count > 0) && (_parameters.Last().Optional))
                {
                    throw new ArgumentException($"a mandatory parameter ({name}) cannot be added after an optional parameter.");
                }


                var param = new Parameter(name, type, optional, description);
                _parameters.Add(param);

                return this;
            }

            /// <summary>
            /// function to generate help
            /// </summary>
            internal string ToHelp()
            {
                return
                    Name.ToUpper() + " - " +
                    Description + Environment.NewLine +

                    Name.ToUpper() + " ( " + string.Join(", ", Parameters.Select(p => p.ToHelpHeader())) + ")"

                    + (Parameters.Any() ? Environment.NewLine : "") +
                    string.Join(Environment.NewLine, Parameters.Select(p => p.ToHelp()))
                    ;

            }

        }

        public class Parameter
        {
            private string _name;
            public string Name { get => _name; }

            private ParameterType _type;
            public ParameterType Type { get => _type; }


            private bool _optional;
            public bool Optional { get => _optional; }

            private string _description;
            public string Description { get => _description; }


            public Parameter(string name, ParameterType type, bool optional, string description)
            {
                _name = name;
                _type = type;
                _optional = optional;
                _description = description;

            }

            /// <summary>
            /// function to generate help
            /// </summary>
            internal string ToHelpHeader()
            {
                string h = Name;

                if (Optional) h = $"[{h}]";

                return h;
            }

            internal string ToHelp()
            {
                string h = Name + " : " + Type.ToString() +
                    (string.IsNullOrWhiteSpace(Description) ? "" : Environment.NewLine + Description);

                return h;
            }
        }


        public class ExecuteFunction
        {
            public Function Function { get; private set; }

            public List<ExecuteParameter> Parameters { get; private set; }


            public ExecuteFunction(Function function)
            {
                Function = function; ;
                Parameters = Function.Parameters.Select(p => new ExecuteParameter(p)).ToList();

            }
        }

        public class ExecuteParameter
        {
            public Parameter Parameter { get; private set; }

            public ExecuteParameter(Parameter parameter)
            {
                Parameter = parameter;
            }


            private string ValueString { get; set; }

            public void SetValue(string value)
            {
                ValueString = value;
            }


            public long GetValueLong()
            {
                return long.Parse(ValueString);
            }

            public double GetValueDouble()
            {
                return double.Parse(ValueString);
            }
            public double GetValueBool()
            {
                return double.Parse(ValueString);
            }

            public string GetValueString()
            {
                return ValueString;
            }
        }



    }






}
