using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Positionning;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Implementation
{
    /// <summary>
    /// Default implementation for IReferentialManager
    /// <br />
    /// It manages the internal structure and delegates actual work to utility classes
    /// </summary>
    public class ReferentialManagerImpl : IReferentialManager
    {

        public List<List<BaseTransformation>> Transformations { get; set; }
        private ILogger<ReferentialManagerImpl> _logger;

        public ReferentialManagerImpl()
        {
            RegisterLogger();
            CreateEmptyTransformationList();
        }

        private void RegisterLogger()
        {
            _logger = new SerilogLogger<ReferentialManagerImpl>();
        }

        public CompositePositionTransformer Convert(CompositePosition from)
        {
            var transformer = new CompositePositionTransformer(from, Transformations);
            return transformer;
        }

        public TransformationAdder Add(BaseTransformation transformation)
        {
            return new TransformationAdder(this, transformation);
        }

        private void CreateEmptyTransformationList()
        {
            int referentialCount = (int)Referential.Count;
            Transformations = new List<List<BaseTransformation>>(referentialCount);
            for (int index = 0; index < (int)Referential.Count; ++index)
            {
                Transformations.Add(new List<BaseTransformation>());
            }
        }
    }
}
