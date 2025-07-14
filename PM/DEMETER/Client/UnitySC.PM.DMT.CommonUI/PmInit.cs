using System;
using System.ComponentModel.Composition;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.CommonUI
{
    /// <summary>
    /// Logique d'interaction pour RecipeEditor.xaml
    /// </summary>
    [Export(typeof(IPmInit))]
    [UCMetadata(ActorType = ActorType.DEMETER)]
    class PmInit : IPmInit
    {
 
        public ActorType ActorType => ActorType.DEMETER;

        public void BootStrap()
        {
            Bootstrapper.Register();
        }

        public void Init(bool isStandalone)
        {
            throw new NotImplementedException();
        }
    }
}
