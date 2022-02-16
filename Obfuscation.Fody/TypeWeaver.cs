using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Obfuscation.Fody;

internal sealed class TypeWeaver
{
    private readonly ModuleDefinition module;
    private readonly string feature;
    private readonly bool isAppend;
    private readonly TypeReference obfuscationAttributeRef;

    /// <summary>
    /// Gets a action that will log an MessageImportance.High message to MSBuild. OPTIONAL.
    /// </summary>
    /// <value>
    /// The log information.
    /// </value>
    private readonly Action<string>? logInfo;

    /// <summary>
    /// Gets a action that will log an error message to MSBuild. OPTIONAL.
    /// </summary>
    /// <value>
    /// The log error.
    /// </value>
    private readonly Action<string>? logError;

    public TypeWeaver(ModuleDefinition moduleDefinition, string featureConfigValue, bool isAppend, TypeReference obfuscationAttributeTypeReference, Action<string>? writeInfo = null, Action<string>? writeError = null)
    {
        module = moduleDefinition;
        feature = featureConfigValue;
        this.isAppend = isAppend;
        obfuscationAttributeRef = obfuscationAttributeTypeReference;
        logInfo = writeInfo;
        logError = writeError;
    }

    /// <summary>
    /// Executes this type weaver.
    /// </summary>
    public void Execute()
    {
        var targetTypes = module.GetAllTypes().Where(x => x.CustomAttributes.Any(t => t.AttributeType.FullName == obfuscationAttributeRef.FullName)).ToArray();
        //TODO: do something...
    }
}