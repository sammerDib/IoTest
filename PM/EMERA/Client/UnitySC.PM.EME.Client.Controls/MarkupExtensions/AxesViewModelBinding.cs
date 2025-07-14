using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Controls.MarkupExtensions
{
    public class AxesViewModelBinding : Binding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AxesViewModelBinding" /> class.
        /// </summary>
        public AxesViewModelBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Source = null;
                    return;
                }

                Source = ClassLocator.Default.GetInstance<AxesVM>();
            }
            catch (Exception)
            {
                Source = new AxesViewModelBinding();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AxesViewModelBinding" /> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public AxesViewModelBinding(string path)
            : this()
        {
            Path = new PropertyPath(path);
        }
        #endregion
    }
}
