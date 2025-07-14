using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using ADCConfiguration.ViewModel.Recipe;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.Services
{
    public class FileService
    {
        public static string Md5_copy(string source, string destination)
        {
            FileInfo my_file = new FileInfo(source);
            int bytes_read = 0;
            int old_bytes_read = 0;
            byte[] buffer = new byte[4096];
            byte[] old_buffer;

            FileStream output = new FileStream(destination, FileMode.Create);

            using (FileStream input = new FileStream(source, FileMode.Open))
            {
                using (System.Security.Cryptography.HashAlgorithm ha = System.Security.Cryptography.MD5.Create())
                {
                    bytes_read = input.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, bytes_read);

                    do
                    {
                        old_bytes_read = bytes_read;
                        old_buffer = buffer;
                        buffer = new byte[4096];
                        bytes_read = input.Read(buffer, 0, buffer.Length);
                        output.Write(buffer, 0, bytes_read);

                        if (bytes_read == 0)
                        {
                            ha.TransformFinalBlock(old_buffer, 0, old_bytes_read);
                        }
                        else
                        {
                            ha.TransformBlock(old_buffer, 0, old_bytes_read, old_buffer, 0);
                        }

                    } while (bytes_read != 0);

                    output.Close();

                    return BitConverter.ToString(ha.Hash).ToString().Replace("-", "");
                }
            }
        }

        public static string CalculateMD5(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// Create directory and copy file in sql recipe file directory
        /// </summary>
        /// <param name="filesStatus"></param>
        /// <param name="recipeName"></param>
        /// <param name="inputFileFolder"></param>
        /// <returns>DtoRecipeFiles</returns>
        public static List<Dto.RecipeFile> UpdateRecipeFile(List<FileStatusViewModel> filesStatus, string recipeName, string inputFileFolder)
        {
            List<Dto.RecipeFile> dtoRecipeFiles = new List<Dto.RecipeFile>();
            // Update recipe directory
            string sqlRecipeFileDirectory = ConfigurationManager.AppSettings["DatabaseConfig.AdditionnalRecipeFiles.ServerDirectory"];

            if (!Directory.Exists(sqlRecipeFileDirectory))
                Directory.CreateDirectory(sqlRecipeFileDirectory);

            string currentRecipeFileDirectory = Path.Combine(sqlRecipeFileDirectory, Path.GetFileNameWithoutExtension(recipeName));

            if (!Directory.Exists(currentRecipeFileDirectory))
                Directory.CreateDirectory(currentRecipeFileDirectory);

            // Update file directory
            foreach (FileStatusViewModel fileStatus in filesStatus)
            {
                string currentFileDirectory = Path.Combine(currentRecipeFileDirectory, fileStatus.FileName.Replace('.', '_'));

                if (!Directory.Exists(currentFileDirectory))
                    Directory.CreateDirectory(currentFileDirectory);

                // Files path
                string externalfileFormat = "{0}_V{1}{2}";// 0 => FileName without extension, 1 => File Version  2 => Extension
                string oldDestFile = Path.Combine(currentFileDirectory, string.Format(externalfileFormat, Path.GetFileNameWithoutExtension(fileStatus.FileName), fileStatus.OldVersion, Path.GetExtension(fileStatus.FileName)));
                string newDestFile = Path.Combine(currentFileDirectory, string.Format(externalfileFormat, Path.GetFileNameWithoutExtension(fileStatus.FileName), fileStatus.OldVersion + 1, Path.GetExtension(fileStatus.FileName)));
                string inputFile = Path.Combine(inputFileFolder, fileStatus.FileName);


                switch (fileStatus.State)
                {
                    case FileState.New:
                        fileStatus.MD5 = Md5_copy(inputFile, newDestFile);
                        fileStatus.State = FileState.NewVersionCreated;
                        fileStatus.NewVersion = 1;
                        fileStatus.UserId = Services.Instance.AuthentificationService.CurrentUser.Id;
                        break;
                    case FileState.ToUpdate:
                        if (CalculateMD5(oldDestFile) != CalculateMD5(inputFile))
                        {
                            fileStatus.MD5 = Md5_copy(inputFile, newDestFile);
                            fileStatus.NewVersion = fileStatus.OldVersion + 1;
                            fileStatus.State = FileState.NewVersionCreated;
                            fileStatus.UserId = Services.Instance.AuthentificationService.CurrentUser.Id;
                        }
                        else
                        {
                            fileStatus.State = FileState.IdenticalFile;
                            fileStatus.NewVersion = fileStatus.OldVersion;
                        }
                        break;
                    case FileState.Missing:
                        fileStatus.NewVersion = fileStatus.OldVersion;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid FileState");

                }

                // File exist              
                if (fileStatus.NewVersion != 0)
                {
                    Dto.RecipeFile recipeFile = new Dto.RecipeFile();
                    recipeFile.FileName = fileStatus.FileName;
                    recipeFile.MD5 = fileStatus.MD5;
                    recipeFile.Version = fileStatus.NewVersion;
                    recipeFile.CreatorUserId = fileStatus.UserId;
                    dtoRecipeFiles.Add(recipeFile);
                }
            }

            return dtoRecipeFiles;
        }

    }
}
