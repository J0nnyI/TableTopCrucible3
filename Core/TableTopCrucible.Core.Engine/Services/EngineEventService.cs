using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using TableTopCrucible.Core.Database.ValueTypes;
using TableTopCrucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Engine.Services
{
    [Singleton]
    public interface IEngineEventService
    {
        Interaction<LibraryFilePath, bool> OnLoad { get; }
    }
    internal class EngineEventService:IEngineEventService
    {
        public Interaction<LibraryFilePath, bool> OnLoad { get; } = new();
    }
}
