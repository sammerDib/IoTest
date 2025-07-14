using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace UnitySC.DataAccess.Service.Interface
{
    /// <summary>
    /// Preserve object reference : Avoid Circular References in WCF
    /// </summary>
    public class PreserveReferencesOperationBehavior : DataContractSerializerOperationBehavior
    {
        public PreserveReferencesOperationBehavior(OperationDescription operation) : base(operation)
        {
        }

        public PreserveReferencesOperationBehavior(
             OperationDescription operation, DataContractFormatAttribute dataContractFormatAttribute)
             : base(operation, dataContractFormatAttribute)
        {
        }

        public override XmlObjectSerializer CreateSerializer(
             Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return new DataContractSerializer(type, name, ns, knownTypes,
                                              0x7FFF /*maxItemsInObjectGraph*/,
                                              false/*ignoreExtensionDataObject*/,
                                              true/*preserveObjectReferences*/,
                                              null/*dataContractSurrogate*/);
        }
    }

    /// <summary>
    /// Preserve object reference : Avoid Circular References in WCF
    /// </summary>
    public class PreserveReferencesAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription description,
                                         BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            IOperationBehavior innerBehavior = new PreserveReferencesOperationBehavior(description);
            innerBehavior.ApplyClientBehavior(description, proxy);
        }

        public void ApplyDispatchBehavior(OperationDescription description,
                                          DispatchOperation dispatch)
        {
            IOperationBehavior innerBehavior = new PreserveReferencesOperationBehavior(description);
            innerBehavior.ApplyDispatchBehavior(description, dispatch);
        }

        public void Validate(OperationDescription description)
        {
        }
    }
}