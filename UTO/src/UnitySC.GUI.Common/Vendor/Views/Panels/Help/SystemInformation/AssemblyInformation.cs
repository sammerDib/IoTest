using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.SystemInformation
{
    /// <summary>
    /// Aims to store specific information associated to a module or an assembly
    /// </summary>
    public class AssemblyInformation
    {
        #region Constructor

        public AssemblyInformation(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            Name = assembly.GetName().Name;
            OriginalFilename = Path.GetFileName(assembly.Location);
            CompanyName = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            ProductName = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            ProductVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            FileName = assembly.Location;
            FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            FileDescription = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        }

        #endregion Constructor

        #region Properties

        public string OriginalFilename { get; }

        public string CompanyName { get; }

        public string ProductName { get; }

        public string ProductVersion { get; }

        public string FileName { get; }

        public string FileVersion { get; }

        public string FileDescription { get; }

        public string Name { get; }

        #endregion Properties

        public override string ToString()
        {
            const int width = 30;

            var build = new StringBuilder();
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_ORIGINAL_FILE_NAME.PadRight(width) + OriginalFilename);
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_SELECTED_ASSEMBLY_NAME.PadRight(width) + Name);
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_COMPANY.PadRight(width) + CompanyName);
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_PRODUCT_NAME.PadRight(width) + ProductName);
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_PRODUCTVERSION.PadRight(width) + ProductVersion);
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_FILE_NAME.PadRight(width) + FileName);
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_FILEVERSION.PadRight(width) + FileVersion);
            build.AppendLine(Agileo.GUI.Properties.Resources.S_HELP_SYSTEMINFO_FILE_DESCRIPTION.PadRight(width) + FileDescription);
            build.AppendLine();
            return build.ToString();
        }

        public void ToXML(XmlTextWriter writer)
        {
            writer.WriteStartElement("Module");
            writer.WriteElementString("Name", Name);
            writer.WriteStartElement("Details");
            writer.WriteElementString("Company", CompanyName);
            writer.WriteElementString("ProductName", ProductName);
            writer.WriteElementString("ProductVersion", ProductVersion);
            writer.WriteElementString("OriginalFileName", OriginalFilename);
            writer.WriteElementString("FileName", FileName);
            writer.WriteElementString("FileVersion", FileVersion);
            writer.WriteElementString("FileDescription", FileDescription);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
