using System.Reflection;
using Mono.Cecil;

namespace Obfuscation.Fody;

internal sealed class AssemblyWeaver
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

    public AssemblyWeaver(ModuleDefinition moduleDefinition, string featureConfigValue, bool isAppend, TypeReference obfuscationAttributeTypeReference, Action<string>? writeInfo = null, Action<string>? writeError = null)
    {
        module = moduleDefinition;
        feature = featureConfigValue;
        this.isAppend = isAppend;
        obfuscationAttributeRef = obfuscationAttributeTypeReference;
        logInfo = writeInfo;
        logError = writeError;
    }
    /// <summary>
    /// Executes this assembly weaver.
    /// </summary>
    public void Execute()
    {
        var targetAssembly = module.Assembly;
        var obfuscationAttribute = targetAssembly.CustomAttributes.FirstOrDefault(t => t.AttributeType.FullName == obfuscationAttributeRef.FullName);
        if (obfuscationAttribute == null)
        {
            logInfo?.Invoke($"Could not find {nameof(ObfuscationAttribute)} on {module.Assembly.Name}");
            return;
        }

        if (!obfuscationAttribute.Properties.Any())
        {
            logInfo?.Invoke("Could not find any property");
            return;
        }

        Mono.Cecil.CustomAttributeNamedArgument? property = obfuscationAttribute.Properties.FirstOrDefault(t => t.Name == nameof(ObfuscationAttribute.Exclude));
        if (!property.HasValue)
        {
            logInfo?.Invoke($"Could not find {nameof(ObfuscationAttribute.Exclude)} property");
            return;
        }

        if (((bool)property.Value.Argument.Value) == true)
        {
            logInfo?.Invoke($"The {nameof(ObfuscationAttribute.Exclude)} property value is true, skip this.");
            return;
        }

        ReplaceObfuscationAttribute(targetAssembly, obfuscationAttribute);
    }

    private void ReplaceObfuscationAttribute(Mono.Cecil.ICustomAttributeProvider attributeProvider, CustomAttribute existObfuscationAttribute)
    {
        int index = attributeProvider.CustomAttributes.IndexOf(existObfuscationAttribute);
        if (index >= 0)
        {
            Mono.Cecil.CustomAttributeNamedArgument? featureProperty = isAppend == true
                ? existObfuscationAttribute.Properties.FirstOrDefault(t => t.Name == nameof(ObfuscationAttribute.Feature))
                : null;

            var newObfuscationAttribute = CreateObfuscationAttribute(featureProperty);
            var existProperties = existObfuscationAttribute.Properties.Where(t => t.Name != nameof(ObfuscationAttribute.Exclude) && t.Name != nameof(ObfuscationAttribute.Feature)).ToArray();
            if (existProperties.Any())
            {
                CreateOtherProperties(newObfuscationAttribute, existProperties);
            }

            attributeProvider.CustomAttributes.RemoveAt(index);
            attributeProvider.CustomAttributes.Insert(index, newObfuscationAttribute);
        }
    }

    private CustomAttribute CreateObfuscationAttribute(Mono.Cecil.CustomAttributeNamedArgument? featureProperty = null)
    {
        var attributeType = typeof(ObfuscationAttribute);
        MethodReference attributeConstructor = module.ImportReference(attributeType.GetConstructor(Type.EmptyTypes));
        var constructorRef = module.ImportReference(attributeConstructor);
        var customAttribute = new CustomAttribute(constructorRef);

        customAttribute.Properties.Add(CreateExcludeProperty());
        customAttribute.Properties.Add(CreateFeatureProperty(featureProperty));

        return customAttribute;
    }

    private Mono.Cecil.CustomAttributeNamedArgument CreateExcludeProperty()
    {
        var excludePropertyValue = new CustomAttributeArgument(module.ImportReference(typeof(bool)), false);
        return new Mono.Cecil.CustomAttributeNamedArgument(nameof(ObfuscationAttribute.Exclude), excludePropertyValue);
    }

    private Mono.Cecil.CustomAttributeNamedArgument CreateFeatureProperty(Mono.Cecil.CustomAttributeNamedArgument? featureProperty = null)
    {
        string newFeatureValue = isAppend && featureProperty.HasValue && featureProperty.Value.Argument.Value != null
            ? $"{featureProperty.Value.Argument.Value};{feature}"
            : feature;

        var featurePropertyValue = new CustomAttributeArgument(module.ImportReference(typeof(string)), newFeatureValue);
        return new Mono.Cecil.CustomAttributeNamedArgument(nameof(ObfuscationAttribute.Feature), featurePropertyValue);
    }

    private static void CreateOtherProperties(CustomAttribute addToObfuscationAttribute, params Mono.Cecil.CustomAttributeNamedArgument[] existProperties)
    {
        if (!existProperties.Any())
        {
            return;
        }

        foreach (Mono.Cecil.CustomAttributeNamedArgument namedArgument in existProperties)
        {
            addToObfuscationAttribute.Properties.Add(
                new Mono.Cecil.CustomAttributeNamedArgument(
                    namedArgument.Name,
                    new CustomAttributeArgument(
                        namedArgument.Argument.Type,
                        namedArgument.Argument.Value
                    )
                )
            );
        }
    }
}
