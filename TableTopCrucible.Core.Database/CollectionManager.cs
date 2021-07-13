using AutoMapper;

using LiteDB;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using TableTopCrucible.Core.DataAccess.Exceptions;
using TableTopCrucible.Core.FileManagement.Models;

namespace TableTopCrucible.Core.Database
{
    public interface IDatabaseCollection<Tid, Tentity>
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
    {
    }
    internal class CollectionManager<Tid, Tentity> : IDatabaseCollection<Tid, Tentity>
            where Tid : IEntityId
            where Tentity : IEntity<Tid>
    {
        private readonly ILiteCollection<Tentity> _sourceCollection;
        private readonly IMapper _mapper;

        public CollectionManager(IMapper mapper)
        {
            _mapper = mapper;
        }

        public CollectionManager(ILiteCollection<Tentity> sourceCollection)
        {
            _sourceCollection = sourceCollection;
        }
    }
}
