using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CMS_Lib.Helpers;

public static class CollectionHelper
{
    public static bool IsList(object o)
    {
        try
        {
            return o.GetType().ImplementsGenericInterface(typeof(IList<>)) ||
                   o.GetType().ImplementsGenericInterface(typeof(IEnumerable<>)) ||
                   o.GetType().ImplementsGenericInterface(typeof(IReadOnlyList<>));
        }
        catch (Exception)
        {
            // ignored
        }

        return false;
    }
    public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
    {
        return type
            .GetTypeInfo()
            .ImplementedInterfaces
            .Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == interfaceType);
    }
}