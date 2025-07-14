using System;

using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.Shared.Data.Composer;

namespace UnitySC.DataAccess.ResultScanner.Implementation
{
    public class PathComposer
    {
        private readonly TemplateComposer _cmpFile;
        private readonly TemplateComposer _cmpDir;

        public PathComposer(string fileNameModel, string directoryModel)
        {
            _cmpFile = new TemplateComposer(fileNameModel, ResultPathParams.Empty);
            _cmpDir = new TemplateComposer(directoryModel, ResultPathParams.Empty);
        }

        public string GetDirPath(ResultPathParams prm)
        {
            return _cmpDir.ComposeWith(prm);
        }

        public string GetFileName(ResultPathParams prm)
        {
            return _cmpFile.ComposeWith(prm);
        }
    }
}
