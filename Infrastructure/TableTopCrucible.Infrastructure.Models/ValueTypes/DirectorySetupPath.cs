
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Infrastructure.Repositories.Models.ValueTypes
{
    public class DirectorySetupPath : DirectoryPath<DirectorySetupPath>
    {
        public static explicit operator DirectorySetupPath(string value)
            => From(value);
    }
}
