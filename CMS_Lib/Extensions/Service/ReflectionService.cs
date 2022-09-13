using CMS_Lib.DI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CMS_Lib.Extensions.Service
{
    public interface IReflectionService : IScoped
    {
        List<Type> GetController(Assembly assembly, String namespaces);
        List<MemberInfo> GetActions(Type controller);
    }

    public class ReflectionService : IReflectionService
    {
        public List<Type> GetController(Assembly assembly, string namespaces)
        {
            List<Type> listController = new List<Type>();
            IEnumerable<Type> types = assembly.GetTypes()
                .Where(type => type.Namespace != null && typeof(Controller).IsAssignableFrom(type) && type.Namespace.Contains(namespaces) && type.CustomAttributes.All(x => x.AttributeType.Name != "NonLoad"))
                .OrderBy(x => x.Name);
            return types.ToList();
        }

        public List<MemberInfo> GetActions(Type controller)
        {
            List<MemberInfo> listAction = new List<MemberInfo>();
            IEnumerable<MemberInfo> memberInfo = controller
                .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                // .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                .OrderBy(x => x.Name);
            foreach (MemberInfo method in memberInfo)
            {
                if (method.ReflectedType is not null && method.ReflectedType.IsPublic && !method.IsDefined(typeof(NonActionAttribute)) && method.CustomAttributes.All(x => x.AttributeType.Name != "NonLoad"))
                {
                    listAction.Add(method);
                }
            }
            return listAction;
        }
    }
}
