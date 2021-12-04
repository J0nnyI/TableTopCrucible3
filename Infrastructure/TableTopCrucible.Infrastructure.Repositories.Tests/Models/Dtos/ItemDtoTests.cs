using NUnit.Framework;
using TableTopCrucible.Infrastructure.Repositories.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using FluentAssertions;
using Splat;
using TableTopCrucible.Core.TestHelper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;

namespace TableTopCrucible.Infrastructure.Repositories.Models.Dtos.Tests
{
    [TestFixture()]
    public class ItemDtoTests
    {
        [Test()]
        public void BidirectionalConversion()
        {
            var hash = Enumerable.Range(1,64).Select(i=>(byte)i).ToArray();
            var size = 100;
            var itemIn = new Item((Name) "tagTest", FileHashKey.From(FileHash.From(hash), FileSize.From(size)));
            
            var itemDtoIn = new ItemEntity();
            itemDtoIn.Initialize(itemIn);

            itemDtoIn.Name.Should().Be(itemIn.Name.Value);
            itemDtoIn.Id.Should().Be(itemIn.Id.Value);
            itemDtoIn.ModelFileHash.Should().BeEquivalentTo(itemIn.ModelFileKey.Hash.Value);
            itemDtoIn.ModelFileSize.Should().Be(itemIn.ModelFileKey.FileSize.Value);

            var serialized = JsonSerializer.Serialize(itemDtoIn);

            var itemDtoOut = (ItemEntity)JsonSerializer.Deserialize(serialized, typeof(ItemEntity));
            
            itemDtoOut.Name.Should().Be(itemIn.Name.Value);
            itemDtoOut.Id.Should().Be(itemIn.Id.Value);
            itemDtoOut.ModelFileHash.Should().BeEquivalentTo(itemIn.ModelFileKey.Hash.Value);
            itemDtoOut.ModelFileSize.Should().Be(itemIn.ModelFileKey.FileSize.Value);

            var itemOut = itemDtoOut.ToEntity();
            itemOut.Name.Should().Be(itemIn.Name);
            itemOut.Id.Value.Should().Be(itemIn.Id.Value);
            itemOut.ModelFileKey.Hash.Value.Should().BeEquivalentTo(itemIn.ModelFileKey.Hash.Value);
            itemOut.ModelFileKey.FileSize.Value.Should().Be(itemIn.ModelFileKey.FileSize.Value);
            //itemIn.ModelFileKey.Should().Be(itemOut.ModelFileKey);

        }
    }
}