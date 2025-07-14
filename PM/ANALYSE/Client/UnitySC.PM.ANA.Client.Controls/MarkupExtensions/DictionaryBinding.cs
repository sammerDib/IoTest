using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class DictionaryBinding:Binding
    {

        public object Key { get; set; }

        public IDictionary Dictionary { get; set; }

        #region Constructors and Destructors

        public DictionaryBinding()
        {
            try
            {
                if ((Dictionary!=null) && Key!=null)
                    this.Source = Dictionary[Key];
            }
            catch (Exception)
            {

                this.Source = new DictionaryBinding();
            }
        }

        public DictionaryBinding(string path)
            : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion
    }
}
