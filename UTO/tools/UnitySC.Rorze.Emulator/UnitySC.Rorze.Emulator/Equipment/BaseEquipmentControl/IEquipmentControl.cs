namespace UnitySC.Rorze.Emulator.Equipment.BaseEquipmentControl
{
    internal interface IEquipmentControl
    {
        bool AutoResponseEnabled { get; set; }

        bool AutoResponse(string toRespondeTo);

        void Clean();
    }
}
