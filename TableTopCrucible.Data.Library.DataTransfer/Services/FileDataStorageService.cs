﻿using AutoMapper;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.DataTransfer.Models;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.DataTransfer.Services
{
    /// <summary>
    /// Basic idea: 
    /// (only when subscribed)
    /// FileSystemWatcher => FileLoader => Observable Update
    /// </summary>
    [Singleton(typeof(FileDataStorageService))]
    public interface IFileDataStorageSergvice
    {

    }
    internal class FileDataStorageService : IFileDataStorageSergvice
    {
        private static readonly FileName fileName = FileName.From("MasterFileList.subttc");
        private readonly ISavefileManagementService _savefileManagementService;
        private readonly IMapper _mapper;

        private FilePath workingFile => _savefileManagementService.CurrentWorkingDirectory + fileName;

        public FileDataStorageService(
            ISavefileManagementService savefileManagementService,
            IMapper mapper
            )
        {
            _savefileManagementService = savefileManagementService;
            _mapper = mapper;
        }

        public IEnumerable<FileData> ReadFileMasterList()
        {
            if (!_savefileManagementService.IsFileOpened)
                throw new InvalidOperationException("could not read file master list - no ttcLib is loaded");


            if (!File.Exists(workingFile))
                return new FileData[0];

            return _mapper.Map<IEnumerable<FileData>>(
                JsonSerializer.Deserialize<IEnumerable<FileDataDTO>>(
                    File.ReadAllText(workingFile)));
        }
        public void WriteMasterFileList(IEnumerable<FileData> fileData)
        {
            if (fileData?.Any() != true)
                File.Delete(workingFile);

            File.WriteAllText(
                workingFile,
                JsonSerializer.Serialize(
                    _mapper.Map<IEnumerable<FileDataDTO>>(fileData))
                );

        }

    }
}
