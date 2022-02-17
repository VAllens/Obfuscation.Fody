using Mono.Cecil;

namespace Obfuscation.Fody;

internal sealed class AssemblyObfuscationWeaver : ObfuscationWeaverBase
{
    public AssemblyObfuscationWeaver(
        ModuleDefinition moduleDefinition,
        string featureConfigValue,
        bool isAppend,
        TypeReference obfuscationAttributeTypeReference,
        Action<string>? writeInfo = null,
        Action<string>? writeError = null)
        : base(
            moduleDefinition,
            featureConfigValue,
            isAppend,
            obfuscationAttributeTypeReference,
            writeInfo,
            writeError)
    {

    }

    /// <summary>
    /// Executes this assembly weaver.
    /// </summary>
    public override void Execute()
    {
        var targetAssembly = module.Assembly;
        if (!CheckObfuscationAttribute(targetAssembly, out var obfuscationAttribute))
        {
            return;
        }

        ReplaceObfuscationAttribute(targetAssembly, obfuscationAttribute);
    }
}
