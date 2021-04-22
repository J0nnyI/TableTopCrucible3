using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Reactive.Subjects;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Data.Library.DataTransfer.Services
{
    /// <summary>
    /// 
    /// </summary>
    [Singleton(typeof(SavefileManagementService))]
    public interface ISavefileManagementService
    {
        DirectoryPath CurrentWorkingDirectory { get; }
        bool IsFileOpened { get; }

        void CreateMasterfile();
    }
    internal class SavefileManagementService : ISavefileManagementService
    {
        /// <summary>
        /// the directory in which all the sub-jsons are unzipped
        /// </summary>
        private readonly BehaviorSubject<DirectoryPath> currentWorkingDirectoryChanges = new BehaviorSubject<DirectoryPath>(null);
        private readonly ILogger<SavefileManagementService> logger;

        public DirectoryPath CurrentWorkingDirectory => currentWorkingDirectoryChanges.Value;
        public bool IsFileOpened => currentWorkingDirectoryChanges.Value != null;


        public SavefileManagementService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<SavefileManagementService>();
        }


        public void CreateMasterfile()
        {
            if (IsFileOpened)
                throw new InvalidOperationException("I can't open a file when one is already opened (not implemented yet)");

            currentWorkingDirectoryChanges.OnNext(generateTemporaryDirectory());
        }
        private DirectoryPath generateTemporaryDirectory()
        {
            var path = DirectoryPath.From(Path.Combine(Path.GetTempPath(), @"TableTopCrucible\newMasters\", "~" + DateTime.Now.ToString("yyyyMMddHHmmss")));
            this.logger.LogInformation("creating temporary working directory at {0}", path);
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
