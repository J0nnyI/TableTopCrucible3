using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using AutoMapper;
using TableTopCrucible.Core.ValueTypes;
using System.Text.Json;
using TableTopCrucible.Data.Models.Sources;
using System.Linq;
using TableTopCrucible.Data.Library.DataTransfer.Services;
using TableTopCrucible.App.Shared;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using FluentAssertions;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Splat;
using TableTopCrucible.Core.BaseUtils;

namespace TableTopCrucible.Data.Library.DataTransfer.Models.Tests
{
    public class TestData
    {
        public TestData()
        {

        }
        public TestData(FilePath content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public FilePath Content { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is TestData data &&
                   EqualityComparer<FilePath>.Default.Equals(Content, data.Content);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Content);
        }
    }



    [TestFixture]
    public class FileDataDTOTests
    {
        public FileDataDTOTests()
        {
        }
        [SetUp]
        public void BeforeEach()
            => di = DependencyBuilder.GetTestProvider(srv => srv.AddSingleton<IFileSystem, MockFileSystem>());
        private IServiceProvider di;

        private IMapperService mapperService;


        private string getJson<T>(params (string, T)[] objs) =>
            string.Join(
                Environment.NewLine,
                objs
                    ?.Select(obj =>
                        obj.Item1 + " : " +
                        obj.Item2 == null
                            ? "null"
                            : JsonSerializer.Serialize(obj.Item2)));
        private string getJson<Tdata, Tdto>(params (string, Tdata)[] objs) =>
            string.Join(
                Environment.NewLine,

                objs
                    ?.Select(obj =>
                        "\"" + obj.Item1 + "\" : {" + Environment.NewLine +
                        (obj.Item2 == null
                            ? "null"
                            : (
                                string.Join(Environment.NewLine,
                                    "\"data\": ",
                                    JsonSerializer.Serialize(obj.Item2),
                                    "\"dto\": ",
                                    JsonSerializer.Serialize(mapperService.Map<Tdto>(obj.Item2))
                                ) + ","
                            )) +
                    Environment.NewLine + "},"
                )
            );

        /// <summary>
        /// converts data => dto => json-string => dto => data and checks the result using <see cref="Assert.AreEqual()"/>
        /// </summary>
        /// <typeparam name="Tdata"></typeparam>
        /// <typeparam name="Tdto"></typeparam>
        /// <param name="data"></param>
        /// <param name="mapper"></param>
        public void TestMap<Tdata, Tdto>(Tdata data)
        {
            var dtoIn = mapperService.Map<Tdto>(data);
            var file = JsonSerializer.Serialize(dtoIn);
            var dtoOut = JsonSerializer.Deserialize<Tdto>(file);
            var dataOut = mapperService.Map<Tdata>(dtoOut);
            data.Should().Be(dataOut, "testmap failed" + Environment.NewLine +
                getJson<Tdata, Tdto>(("dataIn", data), ("dataOut", dataOut))
                );
        }
        [Test]
        public void test_FS_Provider()
        {
            Locator.Current.GetService<IFileSystem>()
                .Should().BeAssignableTo<MockFileSystem>("a mock file system should be used by splat")
                .And.BeSameAs(FileSystemHelper.FileSystem, "it should be the helper instance");
            di.GetRequiredService<IFileSystem>()
                .Should().BeAssignableTo<MockFileSystem>("a mock file system should be used by MS DI");

        }
        [Test]
        public void IsConfigValid()
        {
            new MapperConfiguration(cfg =>
            {
                IMapperService.RegisteredAssemblies
                    .ToList()
                    .ForEach(lib => cfg.AddMaps(lib));
            }).AssertConfigurationIsValid();

        }

        [Test]
        public void TestHashKey()
        {

        }

        private FileData buildDemoFileData()
        {
            return new FileData(
                    FilePath.From("test"),
                    FileHashKey.From((
                        buildHash(),
                        FileSize.From(12)
                    )),
                    DateTime.Parse("2019.01.01")
                );
        }
        private FileHash buildHash()
            => FileHash.From(Enumerable.Range(1, 64).Select(i => Convert.ToByte(i)).ToArray());
        [Test]
        public void TestFileDataEquals()
        {
            var a = buildDemoFileData();
            var b = buildDemoFileData();
            a.Should().Be(a);
            a.Should().Be(b);
        }
        [Test]
        public void TestFileDataDTO()
        {
            TestMap<FileData, FileDataDTO>(
                buildDemoFileData()
            );
        }
    }
}