using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class ChuckSupervisorBinding : Binding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChuckSupervisorBinding" /> class.
        /// </summary>
        public ChuckSupervisorBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Source = null;
                    return;
                }

                Source = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            }
            catch (Exception)
            {
                Source = new ChuckSupervisorBinding();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChuckSupervisorBinding" /> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public ChuckSupervisorBinding(string path)
            : this()
        {
            Path = new PropertyPath(path);
        }

        #endregion Constructors and Destructors
    }
}
