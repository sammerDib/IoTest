using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class AxesSupervisorBinding:Binding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AxesSupervisorBinding" /> class.
        /// </summary>
        public AxesSupervisorBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Source = null;
                    return;
                }

                    Source = ClassLocator.Default.GetInstance<AxesSupervisor>();
            }
            catch (Exception)
            {

                Source = new AxesSupervisorBinding();
            }


        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AxesSupervisorBinding" /> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public AxesSupervisorBinding(string path)
            : this()
        {
            Path = new PropertyPath(path);
        }

        #endregion
    }
}
