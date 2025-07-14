using System;

using Agileo.Semi.Communication.Abstractions.E5;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SequenceLog
{
    public class SecsMessage
    {
        public ulong Txid { get; set; }

        public string Name { get; set; }

        public Direction Direction { get; set; }

        public string StreamAndFunction { get; set; }

        public string Smn { get; set; }

        public byte Stream { get; set; }

        public byte Function { get; set; }

        public DateTime Time { get; set; }
    }
}
