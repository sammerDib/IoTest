namespace UnitySC.Beckhoff.Emulator
{
    public class Tag
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string TypeToString { get; set; }

        public Tag()
        {

        }

        public Tag(string name, string address)
        {
            Name = name;
            Address = address;
        }
    }
}
