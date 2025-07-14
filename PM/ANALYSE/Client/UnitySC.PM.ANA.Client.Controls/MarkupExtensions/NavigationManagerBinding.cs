using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Controls.MarkupExtensions
{
    public class NavigationManagerBinding : Binding
    {
        #region Constructors and Destructors

        public NavigationManagerBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    this.Source = null;
                    return;
                }
                this.Source = ClassLocator.Default.GetInstance<INavigationManager>();
            }
            catch (Exception)
            {
                this.Source = new NavigationManagerBinding();
            }
        }

        public NavigationManagerBinding(string path)
           : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion Constructors and Destructors
    }
}
