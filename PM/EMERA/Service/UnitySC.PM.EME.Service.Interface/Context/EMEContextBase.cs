using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface.Context;

namespace UnitySC.PM.EME.Service.Interface.Context
{
    /// <summary>
    /// Context dedicated to EMERA flows
    /// Note: this is not an interface because of WCF which does not accept interfaces as DataContract.
    /// </summary>
    [DataContract]
    [KnownType(typeof(PositionContext))]
    [KnownType(typeof(XYPositionContext))]
    [KnownType(typeof(XYZPositionContext))]
    [KnownType(typeof(ChamberContext))]
    [KnownType(typeof(LightContext))]
    [KnownType(typeof(LightsContext))]
    [KnownType(typeof(PMContext))]
    [KnownType(typeof(ContextsList))]
    [XmlInclude(typeof(PositionContext))]
    [XmlInclude(typeof(XYPositionContext))]
    [XmlInclude(typeof(XYZPositionContext))]
    [XmlInclude(typeof(ChamberContext))]
    [XmlInclude(typeof(LightContext))]
    [XmlInclude(typeof(LightsContext))]
    [XmlInclude(typeof(PMContext))]
    [XmlInclude(typeof(ContextsList))]
    public abstract class EMEContextBase : IContext
    {
    }
}
