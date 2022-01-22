using Newtonsoft.Json;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DirectorySetup : DataEntity<DirectorySetupId>
    {
        private Name _name;

        private DirectoryPath _path;

        public DirectorySetup()
        {
        }

        public DirectorySetup(DirectoryPath path)
        {
            Path = path;
            Name = path.GetDirectoryName().ToName();
        }

        public DirectoryPath ThumbnailDirectory => Path + (DirectoryName)"Thumbnails";

        [JsonProperty]
        public Name Name
        {
            get => _name;
            set => SetRequiredValue(ref _name, value);
        }

        [JsonProperty]
        public DirectoryPath Path
        {
            get => _path;
            set => SetRequiredValue(ref _path, value);
        }

        public override string ToString() => $"Name:'{Name}' | Path:'{Path}' | Id:'{Id}'";
    }
}