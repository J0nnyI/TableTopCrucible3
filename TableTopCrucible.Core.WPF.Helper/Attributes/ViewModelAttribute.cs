
using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

using TableTopCrucible.Core.Helper;

namespace TableTopCrucible.Core.WPF.Helper.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ViewModelAttribute : Attribute
    {
        public Type viewType { get; }
        public ViewModelAttribute(Type view)
        {
            viewType = view;
        }

        public static ResourceDictionary GetTemplateDictionary()
        {
            var res = new ResourceDictionary();

            var types = AssemblyHelper
                .GetSolutionAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Select(viewModelType => new
                {
                    viewModelType,
                    (viewModelType.GetCustomAttributes(typeof(ViewModelAttribute), false).FirstOrDefault() as ViewModelAttribute)?.viewType
                })
                .Where(typeEx => typeEx.viewType != null);

            types.Select(typeEx => createTemplate(typeEx.viewModelType, typeEx.viewType))
                .ToList()
                .ForEach(template =>
                {
                    res.Add(template.DataTemplateKey, template);
                });

            return res;
        }

        // https://www.codeproject.com/Articles/444371/Creating-WPF-Data-Templates-in-Code-The-Right-Way
        static DataTemplate createTemplate(Type viewModelType, Type viewType)
        {
            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            var context = new ParserContext();

            context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("vm", "vm");
            context.XmlnsDictionary.Add("v", "v");

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            return template;
        }
    }
}
