using Microsoft.VisualStudio.TestTools.UnitTesting;
using TableTopCrucible.Data.Library.DataTransfer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using TableTopCrucible.Core.ValueTypes;
using System.Text.Json;
using ValueOf;
using TableTopCrucible.Data.Models.Sources;
using System.Linq;
using AutoMapper.Configuration.Annotations;
using TableTopCrucible.Data.Library.DataTransfer.Services;
using TableTopCrucible.App.Shared;
using Microsoft.Extensions.DependencyInjection;

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



    [TestClass()]
    public class FileDataDTOTests
    {
        public FileDataDTOTests()
        {
            this.mapperService = 
                DependencyBuilder
                    .GetServices()
                    .BuildServiceProvider()
                    .GetRequiredService<IMapperService>();
        }
        private IMapperService mapperService { get; }


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
            Assert.AreEqual(data, dataOut, "testmap failed" + Environment.NewLine +
                getJson<Tdata, Tdto>(("dataIn", data), ("dataOut", dataOut))
                );
        }

        [TestMethod]
        public void IsConfigValid()
        {
            new MapperConfiguration(cfg =>
            {
                IMapperService.RegisteredAssemblies
                    .ToList()
                    .ForEach(lib => cfg.AddMaps(lib));
            }).AssertConfigurationIsValid();

        }

        [TestMethod]
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
        [TestMethod]
        public void TestFileDataEquals()
        {
            var a = buildDemoFileData();
            var b = buildDemoFileData();
            Assert.AreEqual(a, a,"same instance");
            Assert.AreEqual(a, b, "same value");
        }
        [TestMethod]
        public void TestFileDataDTO()
        {
            TestMap<FileData, FileDataDTO>(
                buildDemoFileData()
            );
        }
    }
}