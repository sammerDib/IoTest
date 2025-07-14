using System;
using System.Collections.Generic;
using System.Linq;

using DeepLearningSoft48.Modules.Parameters;

using UnitySC.Shared.LibMIL;

namespace DeepLearningSoft48.Modules
{
    /// <summary>
    /// Module base class
    /// </summary>
    public abstract class ModuleBase
    {
        //====================================================================
        // Constructor
        //====================================================================
        public ModuleBase(IModuleFactory factory)
        {
            Factory = factory;
        }

        //====================================================================
        // Properties
        //====================================================================
        public IModuleFactory Factory { get; private set; }
        public virtual string DisplayName { get { return Factory.Label; } }

        protected List<ParameterBase> _parameterList = null;
        public List<ParameterBase> ParameterList
        {
            get
            {
                if (_parameterList == null)
                {
                    Type moduleType = this.GetType();
                    _parameterList = new List<ParameterBase>(
                        from f in moduleType.GetFields()
                        where f.FieldType.IsSubclassOf(typeof(ParameterBase))
                        select f.GetValue(this) as ParameterBase
                        );
                }
                return _parameterList;
            }
        }

        //====================================================================
        // Process Method
        //====================================================================
        public abstract MilImage Process(MilImage imgToProcess);


        //=================================================================
        // Validate Method
        //=================================================================
        /// <summary>
        /// Tests if the module is correctly configured.
        /// </summary>
        /// <returns> Null or error message. </returns>
        public virtual string Validate()
        {
            foreach (ParameterBase param in ParameterList)
            {
                string error = param.Validate();
                if (error != null)
                    return error;
            }
            return null;
        }

        //=================================================================
        // ToString Method
        //=================================================================
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
