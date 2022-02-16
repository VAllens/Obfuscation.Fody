using Mono.Cecil;

namespace Obfuscation.Fody;

internal static class TypeDefinitionExtensions
{
    public static bool IsStruct(this TypeDefinition obj)
    {
        return !obj.IsPrimitive && obj.IsValueType && !obj.IsEnum;
    }

    public static bool IsEnum(this TypeDefinition obj)
    {
        return !obj.IsPrimitive && obj.IsValueType && obj.IsEnum;
    }

    public static bool IsInterface(this TypeDefinition obj)
    {
        return !obj.IsPrimitive && obj.IsInterface;
    }

    public static bool IsDelegate(this TypeDefinition obj)
    {
        return !obj.IsPrimitive && !obj.IsInterface && obj.BaseType.FullName == "System.MulticastDelegate";
    }

    public static bool IsClass(this TypeDefinition obj)
    {
        return !obj.IsPrimitive && obj.IsClass;
    }
}