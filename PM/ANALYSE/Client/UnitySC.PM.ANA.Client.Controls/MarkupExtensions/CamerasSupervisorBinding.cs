using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class CamerasSupervisorBinding:Binding
    {
        #region Constructors and Destructors

       public CamerasSupervisorBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    this.Source = null;
                    return;
                }
                this.Source = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            }
            catch (Exception)
            {

                this.Source = new CamerasSupervisorBinding();
            }


        }

       public CamerasSupervisorBinding(string path)
            : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion
    }
}
