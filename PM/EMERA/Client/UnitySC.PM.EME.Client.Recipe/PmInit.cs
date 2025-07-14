using System;
using System.ComponentModel.Composition;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.EME.Client.Recipe
{
    /// <summary>
    /// Logique d'interaction pour RecipeEditor.xaml
    /// </summary>
    [Export(typeof(IPmInit))]
    [UCMetadata(ActorType = ActorType.EMERA)]
    public class PmInit : IPmInit
    {
 
        public ActorType ActorType => ActorType.ANALYSE;

        private static IntPtr _hookID = IntPtr.Zero;

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
