using System.ComponentModel.Composition;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PP.ADC.Client.CommonUI
{
    /// <summary>
    /// Logique d'interaction pour RecipeEditor.xaml
    /// </summary>
    [Export(typeof(IPmInit))]
    [UCMetadata(ActorType = ActorType.ADC)]
    public class PmInit : IPmInit
    {

        public ActorType ActorType => ActorType.ADC;


        public void BootStrap()
        {
            Bootstrapper.Register();

        }


        public void Init(bool isStandalone)
        {

        }



    }
}
