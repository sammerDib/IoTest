using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.ANA.Client.Proxy;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class ProbesSupervisorBinding : Binding
    {
        #region Constructors and Destructors

         public ProbesSupervisorBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    this.Source = null;
                    return;
                }

                this.Source = ServiceLocator.ProbesSupervisor;
            }
            catch (Exception)
            {
                this.Source = new ProbesSupervisorBinding();
            }
        }

         public ProbesSupervisorBinding(string path)
            : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion Constructors and Destructors
    }
}
