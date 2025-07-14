using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    /// <summary>
    /// Context dedicated to ANALYSE flows
    /// Note: this is not an interface because of WCF which does not accept interfaces as DataContract.
    /// </summary>
    [DataContract]
    [KnownType(typeof(PositionContext))]
    [KnownType(typeof(XYPositionContext))]
    [KnownType(typeof(XYZTopZBottomPositionContext))]
    [KnownType(typeof(AnaPositionContext))]
    [KnownType(typeof(ChamberContext))]
    [KnownType(typeof(LightContext))]
    [KnownType(typeof(LightsContext))]
    [KnownType(typeof(PMContext))]
    [KnownType(typeof(ObjectivesContext))]
    [KnownType(typeof(ObjectiveContext))]
    [KnownType(typeof(TopObjectiveContext))]
    [KnownType(typeof(BottomObjectiveContext))]
    [KnownType(typeof(TopImageAcquisitionContext))]
    [KnownType(typeof(BottomImageAcquisitionContext))]
    [KnownType(typeof(DualImageAcquisitionContext))]
    [KnownType(typeof(ContextsList))]
    [XmlInclude(typeof(PositionContext))]
    [XmlInclude(typeof(XYPositionContext))]
    [XmlInclude(typeof(XYZTopZBottomPositionContext))]
    [XmlInclude(typeof(AnaPositionContext))]
    [XmlInclude(typeof(ChamberContext))]
    [XmlInclude(typeof(LightContext))]
    [XmlInclude(typeof(LightsContext))]
    [XmlInclude(typeof(PMContext))]
    [XmlInclude(typeof(ObjectivesContext))]
    [XmlInclude(typeof(ObjectiveContext))]
    [XmlInclude(typeof(TopObjectiveContext))]
    [XmlInclude(typeof(BottomObjectiveContext))]
    [XmlInclude(typeof(TopImageAcquisitionContext))]
    [XmlInclude(typeof(BottomImageAcquisitionContext))]
    [XmlInclude(typeof(DualImageAcquisitionContext))]
    [XmlInclude(typeof(ContextsList))]
    public abstract class ANAContextBase : IContext
    {
    }
}
