using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ADCConfiguration.Tools
{


    // https://www.codeproject.com/Articles/3111/C-NET-Command-Line-Arguments-Parser

    public class AppArguments
    {
        // Variables
        private SortedDictionary<string, string> Parameters;

        // Constructor

        public AppArguments(string[] Args)
        {
            Parameters = new SortedDictionary<string, string>();
            Regex Spliter = new Regex(@"^-{1,2}|=",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Regex Remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string[] Parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string Txt in Args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                Parts = Spliter.Split(Txt, 3);

                switch (Parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        Parameters.Add("path", Parts[0]);
                        break;

                    // Found just a parameter
                    case 2:
                        if (!Parameters.ContainsKey(Parts[1]))
                            Parameters.Add(Parts[1], "true");
                        break;

                    // Parameter with enclosed value
                    case 3:


                        // Remove possible enclosing characters (",')
                        if (!Parameters.ContainsKey(Parts[1]))
                        {
                            Parts[2] = Remover.Replace(Parts[2], "$1");
                            Parameters.Add(Parts[1], Parts[2]);
                        }

                        break;
                }
            }
        }


        // Retrieve a parameter value if it exists 
        // (overriding C# indexer property)
        public string this[string param]
        {
            get
            {
                string result = null;

                Parameters.TryGetValue(param, out result);

                return result;
            }
        }

        public bool IsDefined(string param)
        {
            return Parameters.ContainsKey(param);
        }




    }

}
