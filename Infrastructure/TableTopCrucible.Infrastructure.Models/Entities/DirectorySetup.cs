using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Newtonsoft.Json;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DirectorySetup : DataEntity<DirectorySetupId>
    {
        public DirectorySetup()
        {
        }
        public DirectorySetup(DirectoryPath path)
        {
            Path = path;
            Name = path.GetDirectoryName().ToName();
        }

        public DirectoryPath ThumbnailDirectory => Path + (DirectoryName)"Thumbnails";

        private Name _name;
        [JsonProperty]
        public Name Name
        {
            get => _name;
            set => SetRequiredValue(ref _name, value);
        }

        private DirectoryPath _path;
        [JsonProperty]
        public DirectoryPath Path
        {
            get => _path;
            set => SetRequiredValue(ref _path, value);
        }
        
        public override string ToString() => $"Name:'{Name}' | Path:'{Path}' | Id:'{Id}'";
    }
    
}