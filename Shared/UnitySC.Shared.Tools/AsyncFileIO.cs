using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UnitySC.Shared.Tools
{
    /// <summary>
    /// Async file access.
    /// </summary>
    public static class AsyncFileIO
    {
        /// <summary>
        /// Purges a folder from all its contents.
        /// IOException (includes timeout).
        /// </summary>
        public static async Task FolderPurgeAsync(string folder, CancellationToken cancel = default)
        {
            //>IOException
            await FolderCreateIfNotExisting(folder, cancel).ConfigureAwait(false);

            //>IOException
            foreach (var fichier in await FolderListFilesAsync(folder, cancel).ConfigureAwait(false))
            {
                //>IOException
                await FileOrFolderDeleteAsync(fichier.FullName, cancel).ConfigureAwait(false);
            }

            //>IOException
            foreach (var subfolder in await FolderListSubFoldersAsync(folder, cancel).ConfigureAwait(false))
            {
                //>IOException
                await FileOrFolderDeleteAsync(subfolder.FullName, cancel).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Teste si un élément existe, qu'il soit fichier ou répertoire.
        /// IOException dans le cas où l'info n'est pas disponible (lecteur réseau hs par exemple).
        /// </summary>
        public static async Task<bool> FileOrFolderExistsAsync(string path, CancellationToken cancel = default)
        {
            var file = FileExistsAsync(path, cancel);
            var folder = FolderExistsAsync(path, cancel);

            //>IOException
            await TaskExt.WhenAllAsync(file, folder).ConfigureAwait(false);

            return file.Result || folder.Result;
        }

        /// <summary>
        /// Teste si un fichier existe.
        /// IOException dans le cas où l'info n'est pas disponible (lecteur réseau hs par exemple).
        /// </summary>
        public static Task<bool> FileExistsAsync(string path, CancellationToken cancel = default)
        {
            // throws IOException
            async Task<bool> testAsync()
            {
                await ThreadPoolTools.Post;
                cancel.ThrowIfCancellationRequested();

                try
                {
                    //.Exception
                    return File.Exists(path);
                }
                catch (IOException) { throw; }
                catch (Exception ùEx) { throw new IOException("", ùEx); }
            }

            //>IOException
            return testAsync().cTimeOutAsync<bool, IOException>(FileTimeOut_ms);
        }

        /// <summary>
        /// Teste si un répertoire existe.
        /// IOException dans le cas où l'info n'est pas disponible (lecteur réseau hs par exemple).
        /// </summary>
        public static Task<bool> FolderExistsAsync(string path, CancellationToken cancel = default)
        {
            // throws IOException
            async Task<bool> testAsync()
            {
                await ThreadPoolTools.Post;
                cancel.ThrowIfCancellationRequested();

                try
                {
                    //.Exception
                    return Directory.Exists(path);
                }
                catch (IOException) { throw; }
                catch (Exception ùEx) { throw new IOException("", ùEx); }
            }

            //>IOException
            return testAsync().cTimeOutAsync<bool, IOException>(FileTimeOut_ms);
        }

        /// <summary>
        /// Renomme un fichier ou un répertoire.
        /// IOException en cas d'échec.
        /// </summary>
        public static async Task FileOrFolderMoveAsync(string source, string target, CancellationToken cancel = default)
        {
            //>IOException
            await FileOrFolderDeleteAsync(target, cancel).ConfigureAwait(false);

            // throws IOException
            async Task moveAsync()
            {
                await ThreadPoolTools.Post;
                cancel.ThrowIfCancellationRequested();

                try
                {
                    //.Exception
                    Directory.Move(source, target);
                }
                catch (IOException) { throw; }
                catch (Exception ùEx) { throw new IOException("", ùEx); }
            }

            //>IOException
            await moveAsync().cTimeOutAsync<IOException>(FileTimeOut_ms).ConfigureAwait(false);
        }

        /// <summary>
        /// Supprime l'élément, qu'il soit fichier ou répertoire.
        /// IOException en cas d'échec.
        /// </summary>
        public static async Task FileOrFolderDeleteAsync(string element, CancellationToken cancel = default)
        {
            if (!await FileOrFolderExistsAsync(element, cancel).ConfigureAwait(false))
            {
                return;// Nothing to delete.
            }

            // Suppression de tout attribut qui pourraît empêcher la suppression.
            try
            {
                //.IOException
                await FileOrFolderSetAttributesAsync(element, FileAttributes.Normal, cancel).ConfigureAwait(false);
            }
            catch (IOException) { }// If we fail setting attributes, we can stil try deletion.

            // On tente d'abord de le détruire en tant que répertoire.
            try
            {
                //.IOException
                await FolderPurgeAsync(element, cancel).ConfigureAwait(false);

                cancel.ThrowIfCancellationRequested();

                async Task deleteFolderAsync()
                {
                    await ThreadPoolTools.Post;
                    cancel.ThrowIfCancellationRequested();

                    //.IOException The directory specified by path is read-only or is not empty.
                    // .PathTooLongException The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
                    // .DirectoryNotFoundException The specified path is invalid, such as being on an unmapped drive.
                    //.UnauthorizedAccessException The caller does not have the required permission.
                    //.ArgumentException path is a zero-length string, contains only white space, or contains one or more invalid characters as defined by InvalidPathChars.
                    // .ArgumentNullException path is a null reference (Nothing in Visual Basic).
                    Directory.Delete(element);
                }

                //.Exception
                await deleteFolderAsync().cTimeOutAsync<IOException>(FileTimeOut_ms).ConfigureAwait(false);

                // Répertoire supprimé.
                return;
            }
            catch (Exception) { }
            cancel.ThrowIfCancellationRequested();

            // Si on n'a pas pu supprimer l'élément en tant que répertoire, on tente en tant que fichier.
            async Task deleteFileAsync()
            {
                await ThreadPoolTools.Post;
                cancel.ThrowIfCancellationRequested();

                try
                {
                    //.ArgumentException path is a zero - length string, contains only white space, or contains one or more invalid characters as defined by InvalidPathChars.
                    // .ArgumentNullException path is null.
                    //>IOException The specified file is in use. - or - There is an open handle on the file, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more information, see How to: Enumerate Directories and Files.
                    // >DirectoryNotFoundException The specified path is invalid (for example, it is on an unmapped drive).
                    // >PathTooLongException The specified path, file name, or both exceed the system - defined maximum length. For example, on Windows - based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
                    //.NotSupportedException path is in an invalid format.
                    //.UnauthorizedAccessException The caller does not have the required permission.- or -The file is an executable file that is in use.- or -path is a directory.-or -path specified a read - only file.
                    File.Delete(element);
                }
                catch (IOException) { throw; }
                catch (Exception ùEx) { throw new IOException(string.Empty, ùEx); }
            }

            //>IOException
            await deleteFileAsync().cTimeOutAsync<IOException>(FileTimeOut_ms).ConfigureAwait(false);
        }

        /// <summary>
        /// Makes sure the folder exists: creates it if needed.
        /// throws IOException (includes the case where the path corresponds to a file).
        /// </summary>
        public static async Task FolderCreateIfNotExisting(string folder, CancellationToken cancel = default)
        {
            await ThreadPoolTools.Post;
            cancel.ThrowIfCancellationRequested();

            try
            {
                Directory.CreateDirectory(folder);// No effect if the folder already exists.
            }
            catch (IOException) { throw; }
            catch (Exception ex) { throw new IOException("", ex); }
        }

        /// <summary>
        /// Recursive copy, with overwrite, of all folder contents.
        /// IOException (includes timeout).
        /// </summary>
        public static async Task FolderCopyToAsync(string source, string target, CancellationToken cancel = default)
        {
            Int32 ùThreadOrigine_s32 = Thread.CurrentThread.ManagedThreadId;

            // Make sure the target folder exists.
            //>IOException
            await FolderCreateIfNotExisting(target, cancel).ConfigureAwait(false);

            await ThreadPoolTools.InlineOuPoste(ùThreadOrigine_s32);
            cancel.ThrowIfCancellationRequested();

            try
            {
                Directory.CreateDirectory(target);// No effect if the folder already exists.
            }
            catch (IOException) { throw; }
            catch (Exception ex) { throw new IOException("", ex); }
            cancel.ThrowIfCancellationRequested();

            // Copy files.
            //>IOException
            foreach (var file in await FolderListFilesAsync(source, cancel).ConfigureAwait(false))
            {
                async Task copy()
                {
                    await ThreadPoolTools.InlineOrPost;
                    cancel.ThrowIfCancellationRequested();

                    try
                    {
                        Directory.CreateDirectory(target);// No effect if the folder already exists.
                        cancel.ThrowIfCancellationRequested();
                        file.CopyTo(Path.Combine(target, file.Name), overwrite: true);
                        cancel.ThrowIfCancellationRequested();
                    }
                    catch (IOException) { throw; }
                    catch (Exception ex) { throw new IOException("", ex); }
                }

                //>IOException
                await copy().cTimeOutAsync<IOException>(FileTimeOut_ms);
            }

            // Recurse in subfolders.
            //>IOException
            foreach (var subfolder in await FolderListSubFoldersAsync(source, cancel).ConfigureAwait(false))
            {
                //>IOException
                await FolderCopyToAsync(subfolder.FullName, Path.Combine(target, subfolder.Name), cancel).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Lists files from a folder (non recursive).
        /// IOException (includes timeout).
        /// </summary>
        public static Task<FileInfo[]> FolderListFilesAsync(string folder, CancellationToken cancel = default)
        {
            async Task<FileInfo[]> list()
            {
                await ThreadPoolTools.Post;
                cancel.ThrowIfCancellationRequested();

                try
                {
                    //.Exception
                    return new DirectoryInfo(folder).GetFiles();
                }
                catch (IOException) { throw; }
                catch (Exception ùEx) { throw new IOException(string.Empty, ùEx); }
            }

            //>IOException
            return list().cTimeOutAsync<FileInfo[], IOException>(FileTimeOut_ms);
        }

        /// <summary>
        /// Affecte des attributs à l'élément, sans récursivité.
        /// IOException
        /// Les attributs doivent être compatibles avec l'élément.
        /// </summary>
        public static Task FileOrFolderSetAttributesAsync(string element, FileAttributes attributes, CancellationToken cancel = default)
        {
            async Task setAttributes()
            {
                await ThreadPoolTools.Post;
                cancel.ThrowIfCancellationRequested();

                try
                {
                    //.ArgumentException path is empty, contains only white spaces, contains invalid characters, or the file attribute is invalid.
                    //>IOException
                    // >PathTooLongException The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
                    // >DirectoryNotFoundException The specified path is invalid, such as being on an unmapped drive.
                    // >FileNotFoundException The file cannot be found.
                    //.NotSupportedException path is in an invalid format.
                    //.UnauthorizedAccessException path specified a file that is read - only. - or - This operation is not supported on the current platform. -or - path specified a directory. - or - The caller does not have the required permission.
                    File.SetAttributes(element, attributes);
                }
                catch (IOException) { throw; }
                catch (Exception e) { throw new IOException("", e); }
            }

            //>IOException
            return setAttributes().cTimeOutAsync<IOException>(FileTimeOut_ms);
        }

        /// <summary>
        /// Lists subfolders from a folder (non recursive).
        /// IOException (includes timeout).
        /// </summary>
        public static Task<DirectoryInfo[]> FolderListSubFoldersAsync(string folder, CancellationToken cancel = default)
        {
            async Task<DirectoryInfo[]> list()
            {
                await ThreadPoolTools.Post;
                cancel.ThrowIfCancellationRequested();

                try
                {
                    //.Exception
                    return new DirectoryInfo(folder).GetDirectories();
                }
                catch (IOException) { throw; }
                catch (Exception ùEx) { throw new IOException(string.Empty, ùEx); }
            }

            //>IOException
            return list().cTimeOutAsync<DirectoryInfo[], IOException>(FileTimeOut_ms);
        }

        /// <summary>
        /// Timeout de base pour les accès fichier.
        /// </summary>
        public const Int32 FileTimeOut_ms = 9300;
    }
}