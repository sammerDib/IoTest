using System;

namespace UnitySC.PP.Shared.Configuration
{
    public class AppParameter
    {

        public static AppParameter Instance { get; } = new AppParameter();


        private AppParameter()
        { }


        private Func<string, string> _getValue = null;

        public void Init(Func<string, string> getValue)
        {
            _getValue = getValue;
        }

        public string Get(string param)
        {
            if (_getValue != null)
            {
                return _getValue(param);
            }
            return null;
        }

    }
}

