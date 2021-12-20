using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.Services
{
    [Singleton]
    public interface IEngineEventService
    {
        Interaction<LibraryFilePath, bool> OnLoad { get; }
    }

    internal class EngineEventService : IEngineEventService
    {
        public Interaction<LibraryFilePath, bool> OnLoad { get; } = new();
    }
}