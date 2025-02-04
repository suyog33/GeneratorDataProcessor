using GeneratorDataProcessor.Core.Interfaces;

namespace GeneratorDataProcessor.Core.Services
{
    public class FileMonitorService : IFileMonitorService
    {
        private readonly IDataProcessor dataProcessor;
        private readonly string inputFolder;
        private FileSystemWatcher fileWatcher;

        public FileMonitorService(IDataProcessor dataProcessor, string inputFolder)
        {
            this.dataProcessor = dataProcessor;
            this.inputFolder = inputFolder;
        }

        public void StartMonitoring()
        {
            try
            {
                fileWatcher = new FileSystemWatcher(inputFolder, "*.xml")
                {
                    NotifyFilter = NotifyFilters.FileName
                };

                fileWatcher.Created += FileWatcher_Created;
                fileWatcher.EnableRaisingEvents = true;

                Console.WriteLine($"Monitoring started on {inputFolder}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                // Delay to allow file to be fully written. (small file size)
                // For large file more complex logic can be written
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine($"New file detected: {e.FullPath}");
                dataProcessor.ProcessFile(e.FullPath, e.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {e.FullPath}: {ex.Message}");
            }
        }
    }
}
