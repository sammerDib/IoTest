using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ADCCalibDMT
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            // -image <_imageFile}> -xml <_calibWaferXmlFile> -ouput <_calibFile>"

            var optionsDictionary = new Dictionary<string, string>();

            if (args.Count() >1)
                optionsDictionary.Add(args[0], args[1]);
            if (args.Count() > 3)
                optionsDictionary.Add(args[2], args[3]);
            if (args.Count() > 5)
                optionsDictionary.Add(args[4], args[5]);

            if (optionsDictionary.ContainsKey("-image") && optionsDictionary.ContainsKey("-xml") && optionsDictionary.ContainsKey("-ouput"))
                Application.Run(new MainForm(optionsDictionary["-image"], optionsDictionary["-xml"], optionsDictionary["-ouput"]));
            else
                Application.Run(new MainForm());
        }
    }
}
