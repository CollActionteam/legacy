using System;

namespace CollAction.Helpers
{
    public static class TypeExtensions
    {
        public static Type GetGenericBaseClass(this Type t, Type baseGeneric)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == baseGeneric)
            {
                return t;
            }
            else
            {
                return GetGenericBaseClass(t.BaseType, baseGeneric);
            }
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            Type[] interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
