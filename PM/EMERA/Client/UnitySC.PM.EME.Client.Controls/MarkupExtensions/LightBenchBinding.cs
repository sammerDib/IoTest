using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Controls.MarkupExtensions
{
    public class LightBenchBinding : Binding
    {
        #region Constructors and Destructors

        public LightBenchBinding()
        {
            try
            {
                if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                {
                    this.Source = null;
                    return;
                }
                this.Source = ClassLocator.Default.GetInstance<LightBench>();
            }
            catch (Exception)
            {

                this.Source = new LightBenchBinding();
            }


        }

        public LightBenchBinding(string path)
            : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion
    }
}
