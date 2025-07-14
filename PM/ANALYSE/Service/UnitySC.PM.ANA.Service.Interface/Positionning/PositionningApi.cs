using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Service.Interface.Positionning
{

    /// <summary>
    /// A location for a tool must lie into a referential, which are tied together by transformation relationship.
    /// <br />
    /// NOTE: The lower a referential index is, the closer to the hardware.
    /// </summary>
    public enum Referential
    {
        Motor = 0,
        Stage = 1,
        Wafer = 2,
        Die = 3,
        Image = 4,

        // Dummy entry used to know the number of Referential
        Count = 5,

    }

    /// <summary>
    /// An atomic operation made to a BasePosition setPoint. 
    /// <br />
    /// Implementation is responsible for Axis detection and behaviour accordance if needed.
    /// </summary>
    [DataContract]
    public class BaseTransformation
    {
        public virtual BasePosition Transform(BasePosition from, Referential referentialDest)
        {
            return new BasePosition(referentialDest, from.Axis, from.SetPoint);
        }
    }

    /// <summary>
    /// Utility class which insert a BaseTransformation in a IReferentialManager
    ///<br />
    /// NOTE: This class is not intended to be used as is, but through the referential API.
    /// <br />
    /// Reference: IReferentialManager.Add(BaseTransformation).To(Referential)
    /// </summary>
    public class TransformationAdder
    {
        private IReferentialManager _referentialManager;
        private BaseTransformation _transformation;

        public TransformationAdder(IReferentialManager referentialManager, Positionning.BaseTransformation transformation)
        {
            _referentialManager = referentialManager;
            _transformation = transformation;
        }

        /// <summary>
        /// Perform add of transformation in referentialManager, at right index of transformation structure
        /// </summary>
        /// <param name="referential"></param>
        public void For(Positionning.Referential referential)
        {
            var transformations = _referentialManager.Transformations;
            int referentialIndex = (int)referential;
            transformations[referentialIndex].Add(_transformation);
            _referentialManager.Transformations = transformations;
        }
    }

    /// <summary>
    /// Utility class which apply a BaseTransform to a CompositePosition
    ///<br />
    /// NOTE: This class is not intended to be used as is, but through the referential API.
    /// <br />
    /// Reference: IReferentialManager.Transform(CompositePosition).To(Referential)
    /// </summary>
    /// <exception>Throw when trying to transform a CompositePosition to a more precise Referential</exception>
    public class CompositePositionTransformer
    {
        private Positionning.CompositePosition _from;
        private List<List<BaseTransformation>> _transformations;
        public CompositePositionTransformer(Positionning.CompositePosition from, List<List<BaseTransformation>> transformations)
        {
            _from = from;
            _transformations = transformations;
        }

        public Positionning.CompositePosition To(Positionning.Referential referential)
        {
            var positionBuilder = new Positionning.CompositePosition.Builder(referential);
            foreach (var position in _from.Positions)
            {
                if (position.Referential == referential)
                {
                    positionBuilder.Add(position);
                }
                else
                {
                    TargetReferentialIsCoarserOrThrow(_from.Referential, referential);
                    var transformationsToPerform = SelectTransformsToApply(_from.Referential, referential);
                    var tmpPosition = position;
                    foreach (var aTransform in transformationsToPerform)
                    {
                        tmpPosition = aTransform.Transform(tmpPosition, referential);
                    }
                    positionBuilder.Add(tmpPosition);
                }
            }
            return positionBuilder.Build();
        }

        private void TargetReferentialIsCoarserOrThrow(Positionning.Referential from, Positionning.Referential to)
        {
            if (from < to)
            {
                throw new ArgumentException($"Cannot convert to a more precise referential");
            }
        }

        /// <summary>
        /// Returns a collection of BaseTransformation which apply when converting from 'from' to 'to' referentials.
        /// Basically, internal structure has a list of transformation list, one for each referential.
        /// At index 0 is the "Motor" referential, and all other referentials increments below. When converting one
        /// level up, all transformations of "from" referential are applied. When converting N levels up, transformations of 
        /// N-1 levels are collected, from "from" referential. "to" referential is used as boundary and its transformations
        /// are not executed. It means 'Motor' (index 0) referential transformations will never be executed and are useless.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private IEnumerable<Positionning.BaseTransformation> SelectTransformsToApply(Positionning.Referential from, Positionning.Referential to)
        {
            var result = new List<Positionning.BaseTransformation>();
            for (int index = (int)from; index > (int)to; --index)
            {
                result.AddRange(_transformations[index]);
            }
            return result;

        }
    }

    [DataContract]
    public static class Axes
    {
        [DataMember]
        public static BaseAxis X = new BaseAxis("X");

        [DataMember]
        public static BaseAxis Y = new BaseAxis("Y");

        [DataMember]
        public static BaseAxis ZTop = new BaseAxis("ZTop");

        [DataMember]
        public static BaseAxis ZBottom = new BaseAxis("ZBottom");

        [DataContractAttribute]
        public class BaseAxis
        {
            public BaseAxis(string identifier)
            {
                Identifier = identifier;
            }

            [DataMemberAttribute]
            public readonly string Identifier;

        }
    }

    /// <summary>
    /// A given hardware should implement zero or more
    /// Axis, to allow that hardware to move.
    /// </summary>


    /// <summary>
    /// An AxisPosition carry information for a hardware move across on a given axis.
    /// NOTE: A position is always linked to a Referential.
    /// NOTE: coordinates are expressed using *millimeters*.
    /// </summary>
    [DataContract]
    public class BasePosition
    {

        public BasePosition(Positionning.Referential referential, Positionning.Axes.BaseAxis axis, double setpoint)
        {
            SetPoint = setpoint;
            Referential = referential;
            Axis = axis;
        }
        [DataMember]
        public readonly Positionning.Axes.BaseAxis Axis;

        [DataMember]
        public readonly double SetPoint;

        [DataMember]
        public readonly Positionning.Referential Referential;

    }

    /// <summary>
    /// Allows to group multiple BasePosition to define multi-axis position
    /// <br />
    /// // NOTE: Implementation must ensure all composite  BasePosition are in the same referential.
    /// </summary>
    [DataContract]
    public class CompositePosition
    {
        [DataMember]
        public readonly List<Positionning.BasePosition> Positions;

        [DataMember]
        public readonly Referential Referential;

        private CompositePosition(Referential referential, List<Positionning.BasePosition> positions)
        {
            Referential = referential;
            Positions = positions;
        }

        /// <summary>
        /// The builder allow to easily create CompositePosition
        /// </summary>
        public class Builder
        {
            private List<Positionning.BasePosition> _positions;
            private Referential _referential;
            public Builder(Positionning.Referential referential)
            {
                _positions = new List<Positionning.BasePosition>();
                _referential = referential;
            }

            public Builder Add(Positionning.Axes.BaseAxis axis, double setPoint)
            {
                return Add(new Positionning.BasePosition(_referential, axis, setPoint));
            }

            public Builder Add(Positionning.BasePosition p)
            {

                if (p.Referential != _referential)
                {
                    throw new ArgumentException($"Can only compose positions in the same referential");
                }
                _positions.Add(p);
                return this;
            }
            public CompositePosition Build()
            {
                return new CompositePosition(_referential, _positions);
            }
        }
    }
}
