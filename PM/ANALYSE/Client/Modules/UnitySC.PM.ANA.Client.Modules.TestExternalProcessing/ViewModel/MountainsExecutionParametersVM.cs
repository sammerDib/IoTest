using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.EP.Mountains.Interface;

namespace UnitySC.PM.ANA.Client.Modules.TestExternalProcessing.ViewModel
{
    public class MountainsExecutionParametersVM : ObservableObject
    {
        private MountainsExecutionParameters _parameters;

        public MountainsExecutionParameters Data => _parameters;

        public MountainsExecutionParametersVM()
        {
            _parameters = new MountainsExecutionParameters();
            _parameters.PointData = new PointData();
            InitWithTestValue();
        }


        private void InitWithTestValue()
        {
            StatisticsDocumentFilePath = null;
            OpenStatistics = false;
            UseStatistics = false;
            PrintPDF = true;
            SaveCSV = true;
            SaveResultFile = true;
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            TemplateFile = Path.Combine(baseDirectory, @"TestMountains\TestTemplate.mnt");
            StudiableFile = Path.Combine(baseDirectory, @"TestMountains\TestStudiable.bcrf");
            ResultFolderPath = Path.Combine(baseDirectory, @"TestMountains\Result");
            PointNumber = 1;
            XCoordinate = 10;
            YCoordinate = 5;
        }

        public string StatisticsDocumentFilePath
        {
            get => _parameters.StatisticsDocumentFilePath; set { if (_parameters.StatisticsDocumentFilePath != value) { _parameters.StatisticsDocumentFilePath = value; OnPropertyChanged(); } }
        }

        public bool OpenStatistics
        {
            get => _parameters.OpenStatistics; set { if (_parameters.OpenStatistics != value) { _parameters.OpenStatistics = value; OnPropertyChanged(); } }
        }

        public bool UseStatistics
        {
            get => _parameters.UseStatistics; set { if (_parameters.UseStatistics != value) { _parameters.UseStatistics = value; OnPropertyChanged(); } }
        }

        public bool PrintPDF
        {
            get => _parameters.PrintPDF; set { if (_parameters.PrintPDF != value) { _parameters.PrintPDF = value; OnPropertyChanged(); } }
        }

        public bool SaveCSV
        {
            get => _parameters.SaveCSV; set { if (_parameters.SaveCSV != value) { _parameters.SaveCSV = value; OnPropertyChanged(); } }
        }

        public bool SaveResultFile
        {
            get => _parameters.SaveResultFile; set { if (_parameters.SaveResultFile != value) { _parameters.SaveResultFile = value; OnPropertyChanged(); } }
        }

        public string TemplateFile
        {
            get => _parameters.TemplateFile; set { if (_parameters.TemplateFile != value) { _parameters.TemplateFile = value; OnPropertyChanged(); } }
        }

        public string ResultFolderPath
        {
            get => _parameters.ResultFolderPath; set { if (_parameters.ResultFolderPath != value) { _parameters.ResultFolderPath = value; OnPropertyChanged(); } }
        }


        public string StudiableFile
        {
            get => _parameters.PointData.StudiableFile; set { if (_parameters.PointData.StudiableFile != value) { _parameters.PointData.StudiableFile = value; OnPropertyChanged(); } }
        }

        public int PointNumber
        {
            get => _parameters.PointData.PointNumber; set { if (_parameters.PointData.PointNumber != value) { _parameters.PointData.PointNumber = value; OnPropertyChanged(); } }
        }

        public double XCoordinate
        {
            get => _parameters.PointData.XCoordinate; set { if (_parameters.PointData.XCoordinate != value) { _parameters.PointData.XCoordinate = value; OnPropertyChanged(); } }
        }

        public double YCoordinate
        {
            get => _parameters.PointData.YCoordinate; set { if (_parameters.PointData.YCoordinate != value) { _parameters.PointData.YCoordinate = value; OnPropertyChanged(); } }
        }

    }
}
