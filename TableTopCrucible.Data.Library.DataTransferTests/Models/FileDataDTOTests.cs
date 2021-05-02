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

namespace TableTopCrucible.Data.Library.DataTransfer.Models.Tests
{

    [AutoMap(typeof(TestData), ReverseMap = true)]
    public class TestDTO
    {
        public ValueOfDTO<string> ContentValue { get; set; }
    }

    public class TestData
    {
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
            _mapperConfig = new Lazy<MapperConfiguration>(
                () => new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps("TableTopCrucible.Data.Library.DataTransfer");
                    cfg.AddMaps("TableTopCrucible.Data.Library.DataTransferTests");
                }));
            _mapper = new Lazy<IMapper>(() => mapperConfig.CreateMapper());
        }
        private readonly Lazy<MapperConfiguration> _mapperConfig;
        private MapperConfiguration mapperConfig => _mapperConfig.Value;
        private readonly Lazy<IMapper> _mapper;
        private IMapper mapper => _mapper.Value;


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
                                    JsonSerializer.Serialize(mapper.Map<Tdto>(obj.Item2))
                                )
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
            var dtoIn = mapper.Map<Tdto>(data);
            var file = JsonSerializer.Serialize(dtoIn);
            var dtoOut = JsonSerializer.Deserialize<Tdto>(file);
            var dataOut = mapper.Map<Tdata>(dtoOut);
            Assert.AreEqual(data, dataOut, "testmap failed" + Environment.NewLine +
                getJson<Tdata, Tdto>(("dataIn", data), ("dataOut", dataOut))
                );
        }

        [TestMethod]
        public void IsConfigValid()
        {
            mapperConfig.AssertConfigurationIsValid();

        }

        [TestMethod]
        public void TestDto()
        {
            var data = new TestData(FilePath.From("tester"));
            TestMap<TestData, TestDTO>(data);
        }

        [TestMethod]
        public void TestHashKey()
        {

        }

        [TestMethod]
        public void TestValueTypeMap()
        {
            var a = buildDemoFileData();
            var b = buildDemoFileData();
            var errorText = new Lazy<string>(() => getJson<FileData, FileDataDTO>(("a", a), ("b", b)));
            Assert.AreEqual(a, b, errorText.Value);
            Assert.IsTrue(a.Equals(b), errorText.Value);
            TestMap<FilePath, ValueOfDTO<string>>(FilePath.From("path"));
        }

        private FileData buildDemoFileData()
        {
            return new FileData(
                    FilePath.From("test"),
                    FileHashKey.From(FileHashKey.From((
                        buildHash(),
                        FileSize.From(12)
                    ))),
                    DateTime.Parse("2019.01.01")
                );
        }
        private FileHash buildHash()
            => FileHash.From(Enumerable.Range(1, 64).Select(i => Convert.ToByte(i)).ToArray());
        [TestMethod]
        public void TestFileDataDTO()
        {
            TestMap<FileData, FileDataDTO>(
                buildDemoFileData()
            );
        }
    }
}