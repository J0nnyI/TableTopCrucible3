﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TableTopCrucible.Core.Helper
{
    public static class AssemblyHelper
    {
        // must not use System.IO.Abstractions since the DI builder is not yet done

        public static Lazy<IEnumerable<Assembly>> SolutionAssemblies = new(() =>
        {
            return Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories)
                .Where(file => Path.GetFileName(file).Contains("TableTopCrucible"))
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)))
                .ToArray();
        }, true);

        public static IEnumerable<Type> GetSolutionTypes()
        {
            return SolutionAssemblies.Value.SelectMany(a => a.GetTypes());
        }

        public static IEnumerable<Type> GetSolutionTypes(Type baseType)
        {
            return GetSolutionTypes().Where(t => t.IsAssignableFrom(baseType));
        }

        public static IEnumerable<Type> GetSolutionTypesByAttribute<T>() where T : Attribute
        {
            return GetSolutionTypes().Where(t => t.HasCustomAttribute<T>());
        }

        public static IEnumerable<Type> GetSolutionClasses()
        {
            return GetSolutionTypes()!.Where(t => t.IsClass);
        }

        public static IEnumerable<Type> GetSolutionClassesOfType<T>() => GetSolutionClassesOfType(typeof(T));

        public static IEnumerable<Type> GetSolutionClassesOfType(Type type)
        {
            return GetSolutionTypes()
                !.Where(t => t.IsAssignableTo(type) && t.IsClass);
        }

        public static IEnumerable<Type> GetTypesAssignableTo<T>() => GetTypesAssignableTo(typeof(T));

        public static IEnumerable<Type> GetTypesAssignableTo(Type type)
        {
            return GetSolutionTypes()
                !.Where(t => t.IsAssignableTo(type));
        }
    }
}