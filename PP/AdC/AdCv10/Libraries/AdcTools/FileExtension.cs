using System;
using System.Diagnostics;
using System.IO;

using UnitySC.Shared.Tools;


namespace AdcTools
{
    public static class FileExtension
    {
        ///=================================================================<summary>
        /// Crée un backup du fichier dans TEMP.
        /// Cette fonction ignore silencieusement toutes les erreurs.
        ///</summary>=================================================================
        public static void Backup(PathString filename)
        {
            try
            {
                if (!File.Exists(filename))
                    return;

                // Création du répertoire de backup dans TEMP
                //...........................................
                PathString tmpdir = System.IO.Path.GetTempPath();
                tmpdir /= "Adc";
                if (!Directory.Exists(tmpdir))
                    Directory.CreateDirectory(tmpdir);

                // Sauvegarde du fichier avec sa date
                //...................................
                DateTime datetime = File.GetLastWriteTime(filename);
                string str = datetime.ToString(@"yyyy-MM-dd--HH-mm-ss");
                PathString backup = tmpdir / (filename.Filename + "." + str);
                if (!File.Exists(backup))
                    File.Copy(filename, backup);
            }
            catch (Exception)
            {
                // Tant pis si on ne peut pas faire de backup
            }
        }


        /// <summary>
        /// Copie le contenu d'un repertoire vers un autre emplacement
        /// </summary>
        /// <param name="sourceDirName"> Repertoire source</param>
        /// <param name="destDirName"> Repertoire destination </param>
        /// <param name="copySubDirs"> True pour copier les sous repertoires</param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// Démarre une commande en tâche de fond.
        /// </summary>
        /// On peut accéder au stdout et stderr en utilisant les stream process.StandardOutput et process.StandardError
        public static Process ExecuteBackgroundCommand(string command, string arguments)
        {
            var psi = new ProcessStartInfo(command);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Arguments = arguments;

            Process process = Process.Start(psi);
            return process;
        }

    }
}
