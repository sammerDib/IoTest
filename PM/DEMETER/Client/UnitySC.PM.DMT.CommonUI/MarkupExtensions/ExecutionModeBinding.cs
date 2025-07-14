using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.CommonUI.MarkupExtensions
{
    public class ExecutionModeBinding:Binding
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IconServiceBinding" /> class.
        /// </summary>
        public ExecutionModeBinding()
        {
            try
            {
                this.Source = ClassLocator.Default.GetInstance<IExecutionMode>();
            }
            catch (Exception)
            {

                this.Source = new ExecutionMode();
            }

           
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="IconServiceBinding" /> class.
        /// </summary>
        /// <param name="path"> The path. </param>
        public ExecutionModeBinding(string path)
            : this()
        {
            this.Path = new PropertyPath(path);
        }

        #endregion


    }
}
