using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Controls.MarkupExtensions
{
    public class ChuckViewModelBinding : Binding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChuckViewModelBinding" /> class.
        /// </summary>
        public ChuckViewModelBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Source = null;
                    return;
                }
                Source = ClassLocator.Default.GetInstance<ChuckVM>();
            }
            catch (Exception)
            {
                Source = new ChuckViewModelBinding();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChuckViewModelBinding" /> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public ChuckViewModelBinding(string path)
            : this()
        {
            Path = new PropertyPath(path);
        }

        #endregion Constructors and Destructors
    }
}
