using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Data.Composer
{
    // Class used to convert a templated string into a fulled-string from a data parameter object
    //    Replace template label data by the data value from object into the model string given to the composer constructor
    //
    // Usage example :
    // ---------------
    // var composer = new TemplateComposer(@"C:\Datafolder\{DirName}_{Id}\{FileName}.txt", DataPrmExample.Empty());
    //
    // var prmExemple1 = new DataPrmExample(){DirName = "MyFolder", FileName = "myFile ", Id = 1234};
    // string ex1 = Composer.ComposeWith(prmExemple1);
    // Assert.areEqual(ex1, "C:\Datafolder\MyFolder_1234\myFile.txt");
    //
    // var prmExemple2 = new DataPrmExample(){DirName = "MyOtherFolder", FileName = "mySecondFile ", Id = 456};
    // string ex2 = Composer.ComposeWith(prmExemple);
    // Assert.areEqual(ex2, "C:\Datafolder\MyOtherFolder_456\mySecondFile.txt");


    public class TemplateComposer
    {
        private readonly string _modelFormat;
        private readonly Type _modelType;

        public TemplateComposer(string templateModel, IParamComposerObject paramObject)
        {
            _modelType = paramObject.GetType();
            _modelFormat = MakeFormat(templateModel, paramObject);
        }

        public string ComposeWith(IParamComposerObject paramObject)
        {
            if (!_modelType.Equals(paramObject.GetType()))
                throw new ArgumentException($"Bad Param Object <{paramObject.GetType().ToString()}> - <{_modelType.ToString()}> is expected");

            return string.Format(_modelFormat, paramObject.ToParamObjects());
        }

        private string MakeFormat(string sModel, IParamComposerObject paramObject)
        {
            string sfmt = sModel ?? string.Empty;
            int index = 0;
            var labels = paramObject.ToParamLabels();
            foreach (var prmLabel in labels)
            {
                string nametpl = "{" + prmLabel + "}";
                string numtpl = "{" + index++ + "}";
                sfmt = sfmt.Replace(nametpl, numtpl);
            }
            return sfmt;
        }

    }
}
