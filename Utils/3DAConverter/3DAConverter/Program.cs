using Serilog;

using UnitySC.Shared.Data.FormatFile;


internal class Program
{
    private static void Main(string[] args)
    {
        string directoryPath = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
            ? args[0]
            : AppDomain.CurrentDomain.BaseDirectory;


        //Création du logger
        string logFilePath = Path.Combine(directoryPath, "conversion_log.txt");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(logFilePath, shared:true, outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}")
            .WriteTo.Console()
            .CreateLogger();
        Log.Information($"********************************");
        Log.Information($"Conversion running in directory: {directoryPath}");



        var files = Directory.GetFiles(directoryPath, "*.3da");
        if (files.Length == 0)
        {
            Log.Information("No files to convert.");
        }
        else
        {
            foreach (var file in files)
            {
                try
                {
                    string filenameOut = $"{Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file))}.bcrf";
                    Converter3DA.ToBCRF(file, filenameOut);
                    string successMessage = $"File {file} successfully converted.";
                    Log.Information(successMessage);
                }
                catch (Exception ex)
                {
                    string errorMessage = $"Conversion error on file {file}: {ex.Message}";
                    Log.Error(errorMessage);
                }
            }
        }

        Log.Information($"Conversion complete. See log file: {logFilePath}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

}

