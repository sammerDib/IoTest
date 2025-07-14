using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Light;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class LightsSupervisorBinding: Binding
    {
        #region Constructors and Destructors

         public LightsSupervisorBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    this.Source = null;
                    return;
                }
                this.Source = ServiceLocator.LightsSupervisor;
            }
            catch (Exception)
            {

                this.Source = new LightsSupervisorBinding();
            }


        }

        public LightsSupervisorBinding(string path)
            : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion
    }
}
