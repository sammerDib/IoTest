using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Configuration
{
    public class ClientConfigurationBinding : Binding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientConfigurationBinding" /> class.
        /// </summary>
        public ClientConfigurationBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    Source = null;
                    return;
                }

                Source = ClassLocator.Default.GetInstance<IClientConfigurationManager>();
            }
            catch (Exception)
            {
                Source = new ClientConfigurationBinding();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientConfigurationBinding" /> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public ClientConfigurationBinding(string path)
            : this()
        {
            Path = new PropertyPath(path);
        }

        #endregion Constructors and Destructors
    }
}
