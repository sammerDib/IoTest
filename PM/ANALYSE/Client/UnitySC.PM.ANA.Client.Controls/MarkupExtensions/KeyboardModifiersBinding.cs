using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class KeyboardModifiersBinding:Binding
    {
        #region Constructors and Destructors

         public KeyboardModifiersBinding()
        {
            try
            {
                this.Source = Keyboard.Modifiers;
            }
            catch (Exception)
            {

                this.Source = new AxesSupervisorBinding();
            }


        }

        public KeyboardModifiersBinding(string path)
            : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion
    }
}
