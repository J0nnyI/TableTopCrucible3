using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text.Json;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.DataTransfer.Enums;
using TableTopCrucible.Data.Library.DataTransfer.Exceptions;
using TableTopCrucible.Data.Library.DataTransfer.Services;
using TableTopCrucible.Data.Library.Models.ValueTypes;

namespace TableTopCrucible.Data.Library.DataTransfer.Master
{
    [Singleton(typeof(MasterFileService))]
    public interface IMasterFileService : INotifyPropertyChanged
    {
        LibraryFilePath FilePath { get; }
        WorkingDirectoryPath WorkDirectoryPath { get; }
        bool IsOpen { get; }

        void Open(LibraryFilePath file, Func<bool> recoverFileResolver = null);
        void Close(LibraryFilePath file);
        void New();
        void RegisterFileManager(ISaveFileManager saveFileManager);
        void Quicksave<Tdto>(DirectoryName directoryname, object data);
    }
    class MasterFileService : ReactiveObject, IMasterFileService
    {
        private readonly ILogger<MasterFileService> _logger;
        private readonly IMapperService _mapperService;
        private readonly ObservableAsPropertyHelper<bool> _isOpen;
        public bool IsOpen => _isOpen.Value;

        [Reactive]
        public LibraryFilePath FilePath { get; private set; }
        [Reactive]
        public WorkingDirectoryPath WorkDirectoryPath { get; private set; }
        [Reactive]
        public FileServiceLoadingState LoadingState { get; private set; }
        private List<ISaveFileManager> _fileManagers;
        private static readonly BareFileName _baseFileName = BareFileName.From("data");
        public MasterFileService(ILoggerFactory loggerFactory, IMapperService mapperService)
        {
            _logger = loggerFactory.CreateLogger<MasterFileService>();
            _mapperService = mapperService;
            _isOpen = this.WhenAnyValue(s => s.FilePath)
                .Select(p => !(p is null))
                .DistinctUntilChanged()
                .ToProperty(this, nameof(IsOpen));
        }

        public void Close(LibraryFilePath file)
        {
            throw new NotImplementedException();
        }

        public void New()
        {
            if (this.WorkDirectoryPath != null)
                throw new FileAlreadyOpenedException();
            this.WorkDirectoryPath = WorkingDirectoryPath.GetTemporaryPath();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="recoverFileResolver">when null, an exception will be thrown when a recovery would be required</param>
        public void Open(LibraryFilePath file, Func<bool> recoverFileResolver = null)
        {
            try
            {
                LoadingState = FileServiceLoadingState.Opening;
                if (IsOpen)
                    throw new FileAlreadyOpenedException();
                FilePath = file;
                try
                {
                    WorkDirectoryPath = FilePath.UnpackLibrary();
                    validateWorkingDirectory();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unpacking failed, starting recovery");
                    handleRecovery(recoverFileResolver);
                }


                LoadingState = FileServiceLoadingState.Open;
            }
            catch (RecoveryCanceledException ex)
            {
                LoadingState = FileServiceLoadingState.Closed;
                throw ex;
            }
            catch (Exception ex)
            {
                LoadingState = FileServiceLoadingState.Closed;
                _logger.LogError(ex, "recovery failed");
                throw ex;
            }
        }
        /// <summary>
        /// takes the directory for the module and 
        /// </summary>
        /// <typeparam name="Tdto"></typeparam>
        /// <param name="directoryname"></param>
        /// <param name="baseFileName"></param>
        /// <param name="data"></param>
        public void Quicksave<Tdto>(DirectoryName directoryname, object data)
        {
            if (LoadingState != FileServiceLoadingState.Open)
                throw new NoMasterfileOpenedException();
            var path = WorkDirectoryPath + directoryname + (_baseFileName + getQuicksaveSuffix() + FileExtension.JSON);

            path.WriteAllText(
                JsonSerializer.Serialize(
                        _mapperService.Map<Tdto>(
                            data
                        )));
        }
        private static BareFileName getQuicksaveSuffix()
            => BareFileName.From(DateTime.Now.ToString("yyyyMMmmssfff"));

        /// <summary>
        /// throws an exception if the recovery was unsuccessfull or canceled
        /// </summary>
        /// <param name="recoverFileResolver"></param>
        /// <returns></returns>
        private void handleRecovery(Func<bool> recoverFileResolver)
        {
            _logger.LogWarning("recovery is not implemented yet");
            if (!WorkDirectoryPath.Exists())
                return;
            _logger.LogWarning("Recovery-Requirement detected");

            if (recoverFileResolver?.Invoke() ?? true)
            {
                _logger.LogError("recovery is not implemented yet");
                throw new NotImplementedException("recovery is not implemented yet");
            }
            else
                throw new RecoveryCanceledException();
        }

        private void validateWorkingDirectory()
        {
            _fileManagers.ForEach(mgr => mgr.Validate());
        }

        public void RegisterFileManager(ISaveFileManager saveFileManager)
        {
            _fileManagers.Add(saveFileManager);
            if (LoadingState == FileServiceLoadingState.Open)
                saveFileManager.Validate();
            saveFileManager.OnFileOpened();
        }
    }
}
