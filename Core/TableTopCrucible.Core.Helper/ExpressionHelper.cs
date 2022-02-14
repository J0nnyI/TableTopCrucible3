using System;
using System.Linq.Expressions;
using ReactiveUI;

namespace TableTopCrucible.Core.Helper;

public static class ExpressionHelper
{
    public static string GetPropertyName<TProp, TObj>(this Expression<Func<TObj, TProp>> propertyReader)
    {
        // copied from private static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>
        if (propertyReader is null)
            throw new ArgumentNullException(nameof(propertyReader));

        var expression = Reflection.Rewrite(propertyReader.Body);

        var parent = expression.GetParent();

        if (parent is null)
            throw new ArgumentException("The property expression does not have a valid parent.",
                nameof(propertyReader));
        if (parent.NodeType != ExpressionType.Parameter)
            throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");

        var memberInfo = expression.GetMemberInfo();
        if (memberInfo is null)
            throw new ArgumentException("The property expression does not point towards a valid member.",
                nameof(propertyReader));

        var name = memberInfo.Name;
        if (expression is IndexExpression)
            name += "[]";

        return name;
    }
}