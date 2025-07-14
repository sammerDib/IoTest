using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Conditions;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader
{
    [Device(IsAbstract = true)]
    public interface ISubstrateIdReader : IUnityCommunicatingDevice
    {
        #region Statuses

        [Status]
        string SubstrateId { get; }

        #endregion Statuses

        #region Commands

        [Command(Documentation = "Request to get all recipes.")]
        void RequestRecipes();

        [Command(Documentation = "Read substrate Id.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsRecipeNameValid))]
        void Read(string recipeName = null);

        [Command(Documentation = "Get image.")]
        [Pre(Type = typeof(IsCommunicating))]
        void GetImage(string imagePath);

        #endregion Commands
    }
}
