using FluentAssertions;

using NUnit.Framework;

using Splat;

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace TableTopCrucible.Core.ValueTypes.Tests
{
    [TestFixture()]
    public class FilePathTests
    {
        [SetUp]
        public void BeforeEach()
        {
            Locator.CurrentMutable.Register<IFileSystem>(() => new MockFileSystem());
        }

        [Test()]
        public void GetFileTypeTest()
        {
            FilePath.From(@"C:\SubDir\file.obj").GetFileType().Should().Be(FileType.Model);
            FilePath.From(@"C:\SubDir\file.sTl").GetFileType().Should().Be(FileType.Model);
            FilePath.From(@"C:\SubDir\file.3mf").GetFileType().Should().Be(FileType.SlicerProject);
            FilePath.From(@"C:\SubDir\file.gcode").GetFileType().Should().Be(FileType.SlicedFile);
        }
    }
}