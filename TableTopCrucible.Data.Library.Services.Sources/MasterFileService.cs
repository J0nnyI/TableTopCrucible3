﻿using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Serilog;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.BaseUtils;
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.Services.Sources.Enums;
using TableTopCrucible.Data.Library.Services.Sources.Exceptions;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(MasterFileService))]
    public interface IMasterFileService : INotifyPropertyChanged
    {
        LibraryFilePath FilePath { get; }
        WorkingDirectoryPath WorkDirectoryPath { get; }
        void Open(LibraryFilePath file, Func<bool> recoverFileResolver = null);
        void Close(LibraryFilePath file);
        void New();
        void RegisterFileManager(ISaveFileManager saveFileManager);
    }
    class MasterFileService : DisposableReactiveObjectBase, IMasterFileService
    {
        private readonly ILogger<MasterFileService> _logger;

        [Reactive]
        public LibraryFilePath FilePath { get; private set; }
        [Reactive]
        public WorkingDirectoryPath WorkDirectoryPath { get; private set; }
        [Reactive]
        public FileServiceLoadingState LoadingState { get; private set; }
        private List<ISaveFileManager> _fileManagers;
        public MasterFileService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MasterFileService>();
        }

        public void Close(LibraryFilePath file)
        {
            throw new NotImplementedException();
        }

        public void New()
        {
            throw new NotImplementedException();
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
                this.LoadingState = FileServiceLoadingState.Opening;
                if (this.FilePath != null)
                    throw new FileAlreadyOpenedException();
                this.FilePath = file;
                try
                {
                    this.WorkDirectoryPath = FilePath.UnpackLibrary();
                    validateWorkingDirectory();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Unpacking failed, starting recovery");
                    handleRecovery(recoverFileResolver);
                }


                this.LoadingState = FileServiceLoadingState.Open;
            }
            catch (RecoveryCanceledException ex)
            {
                this.LoadingState = FileServiceLoadingState.Closed;
                throw ex;
            }
            catch (Exception ex)
            {
                this.LoadingState = FileServiceLoadingState.Closed;
                this._logger.LogError(ex, "recovery failed");
                throw ex;
            }
        }

        /// <summary>
        /// throws an exception if the recovery was unsuccessfull or canceled
        /// </summary>
        /// <param name="recoverFileResolver"></param>
        /// <returns></returns>
        private void handleRecovery(Func<bool> recoverFileResolver)
        {
            _logger.LogWarning("recovery is not implemented yet");
            if (!this.WorkDirectoryPath.Exists())
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
            this._fileManagers.ForEach(mgr => mgr.Validate());
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