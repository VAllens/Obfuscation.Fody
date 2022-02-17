using Fody;
using Mono.Cecil;

namespace Obfuscation.Fody;

/// <summary>
/// Obfuscation module weaver.
/// </summary>
/// <seealso cref="BaseModuleWeaver" />
public sealed class ModuleWeaver : BaseModuleWeaver, IWeaver
{
    public override void Execute()
    {
        //if (!System.Diagnostics.Debugger.IsAttached)
        //{
        //    System.Diagnostics.Debugger.Launch();
        //}

        if (Config.Name != "Obfuscation")
        {
            WriteInfo("Could not find any Obfuscation element config on FodyWeavers.xml");
            return;
        }

        if (!Config.HasAttributes)
        {
            WriteInfo("Could not find any attribute on Obfuscation element");
            return;
        }

        var featureAttribute = Config.Attribute(nameof(System.Reflection.ObfuscationAttribute.Feature));
        if (featureAttribute == null || string.IsNullOrWhiteSpace(featureAttribute.Value))
        {
            WriteInfo("Could not find Feature attribute on Obfuscation element, or the value is empty.");
            return;
        }

        var appendAttribute = Config.Attribute("Append");
        bool isAppend = appendAttribute != null && appendAttribute.Value.Equals("true", StringComparison.OrdinalIgnoreCase);

        if (ModuleDefinition is null)
        {
            WriteInfo("The module definition has not been defined.");
            return;
        }

        var runtimes = ModuleDefinition.AssemblyReferences.Where(x => x.Name == "System.Runtime").OrderByDescending(x => x.Version).FirstOrDefault();
        if (runtimes is null)
        {
            WriteInfo($"Could not find assembly: System.Runtime ({string.Join(", ", ModuleDefinition.AssemblyReferences.Select(x => x.Name))})");
            return;
        }

        var obfuscationAttributeRef = new TypeReference("System.Reflection", nameof(System.Reflection.ObfuscationAttribute), ModuleDefinition, runtimes);

        IWeaver assemblyWeaver = new AssemblyObfuscationWeaver(ModuleDefinition, featureAttribute.Value, isAppend, obfuscationAttributeRef, WriteInfo, WriteError);
        assemblyWeaver.Execute();

        IWeaver propertyWeaver = new TypeObfuscationWeaver(ModuleDefinition, featureAttribute.Value, isAppend, obfuscationAttributeRef, WriteInfo, WriteError);
        propertyWeaver.Execute();
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "netstandard";
        yield return "System";
        yield return "System.Runtime"; ;
    }
}