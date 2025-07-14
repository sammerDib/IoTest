using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Data.Composer
{
    // interface for TemplateComposer 's paramaters object 
    //
    //  labels and object are linked by index, maning that array should have the same size and have the same order
    //
    public interface IParamComposerObject
    {
        string[] ToParamLabels();
        object[] ToParamObjects();
    }
}
