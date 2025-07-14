using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.Tools;

using UnitySC.PM.Shared.UI.Proxy;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class ControllersSupervisorBinding : Binding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ControllersSupervisorBinding" /> class.
        /// </summary>
        public ControllersSupervisorBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Source = null;
                    return;
                }

                Source = ClassLocator.Default.GetInstance<ControllersSupervisor>();
            }
            catch (Exception)
            {
                Source = new ControllersSupervisorBinding();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ControllersSupervisorBinding" /> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public ControllersSupervisorBinding(string path)
            : this()
        {
            Path = new PropertyPath(path);
        }

        #endregion Constructors and Destructors
    }
}
