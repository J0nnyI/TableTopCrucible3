using FluentAssertions;

using NUnit.Framework;

namespace TableTopCrucible.Core.ValueTypes.Tests
{
    [TestFixture]
    public class FileExtensionTests
    {
        [Test]
        public void GetFileTypeTest()
        {
            FileExtension.From(".stl").GetFileType().Should().Be(FileType.Model);
            FileExtension.From(".obj").GetFileType().Should().Be(FileType.Model);
            FileExtension.From(".3mf").GetFileType().Should().Be(FileType.SlicerProject);
            FileExtension.From(".gcode").GetFileType().Should().Be(FileType.SlicedFile);
            FileExtension.From(".photon").GetFileType().Should().Be(FileType.SlicedFile);
        }
    }
}