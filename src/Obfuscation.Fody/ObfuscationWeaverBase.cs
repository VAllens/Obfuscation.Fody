using Mono.Cecil;

namespace Obfuscation.Fody;

internal abstract class ObfuscationWeaverBase : IWeaver
{
    private readonly static string[] ExcludedAttributeNames = new[] { nameof(System.Reflection.ObfuscationAttribute.Exclude), nameof(System.Reflection.ObfuscationAttribute.Feature) };
    private readonly static Type ObfuscationAttributeType = typeof(System.Reflection.ObfuscationAttribute);

    protected readonly ModuleDefinition module;
    protected readonly string feature;
    protected readonly bool isAppend;
    protected readonly TypeReference obfuscationAttributeRef;

    /// <summary>
    /// Gets a action that will log an MessageImportance.High message to MSBuild. OPTIONAL.
    /// </summary>
    /// <value>
    /// The log information.
    /// </value>
    protected readonly Action<string>? logInfo;

    /// <summary>
    /// Gets a action that will log an error message to MSBuild. OPTIONAL.
    /// </summary>
    /// <value>
    /// The log error.
    /// </value>
    protected readonly Action<string>? logError;

    protected ObfuscationWeaverBase(
        ModuleDefinition moduleDefinition,
        string featureConfigValue,
        bool isAppend,
        TypeReference obfuscationAttributeTypeReference,
        Action<string>? writeInfo = null,
        Action<string>? writeError = null)
    {
        module = moduleDefinition;
        feature = featureConfigValue;
        this.isAppend = isAppend;
        obfuscationAttributeRef = obfuscationAttributeTypeReference;
        logInfo = writeInfo;
        logError = writeError;
    }

    public abstract void Execute();

    protected virtual bool CheckObfuscationAttribute(ICustomAttributeProvider target, out CustomAttribute obfuscationAttribute)
    {
        if (target == null)
        {
            obfuscationAttribute = null!;
            logInfo?.Invoke($"Custom attribute provider is null");
            return false;
        }

        obfuscationAttribute = target.CustomAttributes.FirstOrDefault(t => t.AttributeType.FullName == obfuscationAttributeRef.FullName);
        if (obfuscationAttribute == null)
        {
            logInfo?.Invoke($"Could not find {nameof(System.Reflection.ObfuscationAttribute)} on {module.Assembly.Name}");
            return false;
        }

        if (!obfuscationAttribute.Properties.Any())
        {
            logInfo?.Invoke("Could not find any property");
            return false;
        }

        CustomAttributeNamedArgument? property = obfuscationAttribute.Properties.FirstOrDefault(t => t.Name == nameof(System.Reflection.ObfuscationAttribute.Exclude));
        if (!property.HasValue)
        {
            logInfo?.Invoke($"Could not find {nameof(System.Reflection.ObfuscationAttribute.Exclude)} property");
            return false;
        }

        if (((bool)property.Value.Argument.Value) == true)
        {
            logInfo?.Invoke($"The {nameof(System.Reflection.ObfuscationAttribute.Exclude)} property value is true, skip this.");
            return false;
        }

        return true;
    }

    protected virtual void ReplaceObfuscationAttribute(ICustomAttributeProvider target, CustomAttribute existObfuscationAttribute)
    {
        int index = target.CustomAttributes.IndexOf(existObfuscationAttribute);
        if (index >= 0)
        {
            CustomAttributeNamedArgument? featureProperty = isAppend == true
                ? existObfuscationAttribute.Properties.FirstOrDefault(t => t.Name == nameof(System.Reflection.ObfuscationAttribute.Feature))
                : null;

            var newObfuscationAttribute = CreateObfuscationAttribute(featureProperty);
            var existProperties = existObfuscationAttribute.Properties.Where(t => t.Name != nameof(System.Reflection.ObfuscationAttribute.Exclude) && t.Name != nameof(System.Reflection.ObfuscationAttribute.Feature)).ToArray();
            if (existProperties.Any())
            {
                CreateOtherProperties(newObfuscationAttribute, existProperties);
            }

            target.CustomAttributes.RemoveAt(index);
            target.CustomAttributes.Insert(index, newObfuscationAttribute);
        }
    }

    private CustomAttribute CreateObfuscationAttribute(CustomAttributeNamedArgument? featureProperty = null)
    {
        var attributeType = typeof(System.Reflection.ObfuscationAttribute);
        MethodReference attributeConstructor = module.ImportReference(attributeType.GetConstructor(Type.EmptyTypes));
        var constructorRef = module.ImportReference(attributeConstructor);
        var customAttribute = new CustomAttribute(constructorRef);

        customAttribute.Properties.Add(CreateExcludeProperty());
        customAttribute.Properties.Add(CreateFeatureProperty(featureProperty));

        return customAttribute;
    }

    private CustomAttributeNamedArgument CreateExcludeProperty()
    {
        var excludePropertyValue = new CustomAttributeArgument(module.ImportReference(typeof(bool)), false);
        return new CustomAttributeNamedArgument(nameof(System.Reflection.ObfuscationAttribute.Exclude), excludePropertyValue);
    }

    private CustomAttributeNamedArgument CreateFeatureProperty(CustomAttributeNamedArgument? featureProperty = null)
    {
        string newFeatureValue = isAppend && featureProperty.HasValue && featureProperty.Value.Argument.Value != null
            ? $"{featureProperty.Value.Argument.Value};{feature}"
            : feature;

        var featurePropertyValue = new CustomAttributeArgument(module.ImportReference(typeof(string)), newFeatureValue);
        return new CustomAttributeNamedArgument(nameof(System.Reflection.ObfuscationAttribute.Feature), featurePropertyValue);
    }

    private static void CreateOtherProperties(CustomAttribute addToObfuscationAttribute, params CustomAttributeNamedArgument[] existProperties)
    {
        if (!existProperties.Any())
        {
            return;
        }

        foreach (CustomAttributeNamedArgument namedArgument in existProperties)
        {
            addToObfuscationAttribute.Properties.Add(
                new CustomAttributeNamedArgument(
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
