using AutoMapper;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Splat;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text.Json;

using TableTopCrucible.App.Shared;
using TableTopCrucible.Core.BaseUtils;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.DataTransfer.Services;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Core.FileManagement.Tests
{
    [TestFixture]
    public class DataWarehouseFactoryTests
    {
        private IServiceProvider di;
        private IDataWarehouseFactory warehouseFactory;

        [SetUp]
        public void BeforeEach()
        {
            di = DependencyBuilder.GetTestProvider(srv => srv.AddSingleton<IFileSystem, MockFileSystem>());
            warehouseFactory = di.GetRequiredService<IDataWarehouseFactory>();
        }
        [Test]
        public void CreateNewWarehouseTest()
        {
            warehouseFactory.Should().NotBeNull();
            var warehouse = warehouseFactory.CreateNewWarehouse();
            warehouse.Should().NotBeNull();
        }
    }
}