using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COHTServiceObject
{
    public enum FunctionCode
    {        
        Fct_ConnectADC,
        Fct_DisconnectADC
    }

    public class Command
    {
        public List<Object> ListParams;
        public FunctionCode Fct;

        public Command(FunctionCode fct, Object param1, Object param2, Object param3, Object param4)
        {
            Fct = fct;
            ListParams = new List<object>();
            if (param1 != null)
                ListParams.Add(param1);
            if (param2 != null)
                ListParams.Add(param2);
            if (param3 != null)
                ListParams.Add(param3);
            if (param4 != null)
                ListParams.Add(param4);
        }
    }

	public class CCommandObject
	{
        private List<Command> _commandList = new List<Command>();   // Preallocate for 16 commands
        private System.Object _synchronizationObject = new System.Object();

        public List<Command> CommandList { get => _commandList; set => _commandList = value; }
        public object SynchronizationObject { get => _synchronizationObject; set => _synchronizationObject = value; }
    }
}
