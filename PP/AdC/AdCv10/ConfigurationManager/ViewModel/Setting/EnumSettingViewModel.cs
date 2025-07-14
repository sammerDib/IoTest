using System;
using System.Collections.Generic;
using System.Linq;

using ConfigurationManager.Configuration;

namespace ConfigurationManager.ViewModel.Setting
{
    public class EnumSettingViewModel<T> : EnumSettingBase where T : struct
    {
        public EnumSettingViewModel(Configuration.Setting setting) : base(setting)
        {
        }

        public T EnumValue
        {
            get
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), Value, true);
                }
                catch (Exception ex)
                {
                    State = SettingState.Error;
                    Error = ex.Message;
                    return (T)Enum.Parse(typeof(T), "0");
                }
            }
            set { Value = value.ToString(); }
        }

        public IEnumerable<KeyValuePair<Enum, string>> EnumList
        {
            get
            {
                var list = from e in Enum.GetValues(typeof(T)).Cast<Enum>()
                           select new KeyValuePair<Enum, string>(e, e.ToString());

                return list;
            }
        }

        public override void Validate()
        {
            T res;
            State = Enum.TryParse<T>(Value, out res) ? SettingState.Valid : SettingState.Error;
        }
    }

    public abstract class EnumSettingBase : SettingBaseViewModel
    {
        public EnumSettingBase(Configuration.Setting setting) : base(setting)
        {
        }

        public override void Validate()
        {

        }
    }
}
