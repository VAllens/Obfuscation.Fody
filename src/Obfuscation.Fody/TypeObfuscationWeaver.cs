using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Obfuscation.Fody;

internal sealed class TypeObfuscationWeaver : ObfuscationWeaverBase
{
    public TypeObfuscationWeaver(
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
    /// Executes this type weaver.
    /// </summary>
    public override void Execute()
    {
        var allTargetTypes = module.GetAllTypes().Where(x => x.CustomAttributes.Any(t => t.AttributeType.FullName == obfuscationAttributeRef.FullName)).ToList();

        var enumTypeDefinitions = allTargetTypes.Where(x => x.IsEnum).ToArray();
        ModifyEnumTypeDefinitions(enumTypeDefinitions);
        RemovePartTypeDefinitions(allTargetTypes, enumTypeDefinitions);

        var interfaceTypeDefinitions = allTargetTypes.Where(x => x.IsInterface).ToArray();
        ModifyInterfaceTypeDefinitions(interfaceTypeDefinitions);
        RemovePartTypeDefinitions(allTargetTypes, interfaceTypeDefinitions);

        var structTypeDefinitions = allTargetTypes.Where(x => x.IsStruct()).ToArray();
        ModifyStructTypeDefinitions(structTypeDefinitions);
        RemovePartTypeDefinitions(allTargetTypes, structTypeDefinitions);

        var delegateTypeDefinitions = allTargetTypes.Where(x => x.IsDelegate()).ToArray();
        ModifyDelegateTypeDefinitions(delegateTypeDefinitions);
        RemovePartTypeDefinitions(allTargetTypes, delegateTypeDefinitions);

        var classTypeDefinitions = allTargetTypes.ToArray();
        ModifyClassTypeDefinitions(classTypeDefinitions);
        //RemovePartTypeDefinitions(allTargetTypes, classTypeDefinitions);
    }

    private void ModifyClassTypeDefinitions(params TypeDefinition[] typeDefinitions)
    {
        foreach (TypeDefinition typeDefinition in typeDefinitions)
        {
            //modify class type attribute
            ModifyTypeDefinition(typeDefinition);

            //modify class fields attribute
            foreach (FieldDefinition fieldDefinition in typeDefinition.Fields)
            {
                ModifyFieldDefinition(fieldDefinition);
            }

            //modify class properties attribute
            //Indexer is a special property
            foreach (PropertyDefinition propertyDefinition in typeDefinition.Properties)
            {
                ModifyPropertyDefinition(propertyDefinition);
            }

            //modify class methods attribute
            //Constructor and finalizer, operator, getter, setter is a special method
            foreach (MethodDefinition methodDefinition in typeDefinition.Methods.Where(x => (!x.IsGetter && !x.IsSetter) || !x.IsHideBySig))
            {
                ModifyMethodDefinition(methodDefinition);
            }

            //modify class events attribute
            foreach (EventDefinition eventDefinition in typeDefinition.Events)
            {
                ModifyEventDefinition(eventDefinition);
            }
        }
    }

    private void ModifyDelegateTypeDefinitions(params TypeDefinition[] typeDefinitions)
    {
        foreach (TypeDefinition typeDefinition in typeDefinitions)
        {
            //modify delegate type attribute
            ModifyTypeDefinition(typeDefinition);

            //modify delegate method's paramters attribute
            foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
            {
                foreach (ParameterDefinition parameterDefinition in methodDefinition.Parameters)
                {
                    ModifyParameterDefinition(parameterDefinition);
                }
            }
        }
    }

    private void ModifyStructTypeDefinitions(params TypeDefinition[] typeDefinitions)
    {
        foreach (TypeDefinition typeDefinition in typeDefinitions)
        {
            //modify struct type attribute
            ModifyTypeDefinition(typeDefinition);

            //modify struct fields attribute
            foreach (FieldDefinition fieldDefinition in typeDefinition.Fields)
            {
                ModifyFieldDefinition(fieldDefinition);
            }

            //modify struct properties attribute
            //Indexer is a special property
            foreach (PropertyDefinition propertyDefinition in typeDefinition.Properties)
            {
                ModifyPropertyDefinition(propertyDefinition);
            }

            //modify struct methods attribute
            //Constructor and operator, getter, setter is a special method
            foreach (MethodDefinition methodDefinition in typeDefinition.Methods.Where(x => (!x.IsGetter && !x.IsSetter) || !x.IsHideBySig))
            {
                ModifyMethodDefinition(methodDefinition);
            }

            //modify struct events attribute
            foreach (EventDefinition eventDefinition in typeDefinition.Events)
            {
                ModifyEventDefinition(eventDefinition);
            }
        }
    }

    private void ModifyEnumTypeDefinitions(params TypeDefinition[] typeDefinitions)
    {
        foreach (TypeDefinition typeDefinition in typeDefinitions)
        {
            //modify enum type attribute
            ModifyTypeDefinition(typeDefinition);

            //modify enum fields attribute
            foreach (FieldDefinition fieldDefinition in typeDefinition.Fields)
            {
                ModifyFieldDefinition(fieldDefinition);
            }
        }
    }

    private void ModifyInterfaceTypeDefinitions(params TypeDefinition[] typeDefinitions)
    {
        foreach (TypeDefinition typeDefinition in typeDefinitions)
        {
            //modify interface type attribute
            ModifyTypeDefinition(typeDefinition);

            //modify interface properties attribute
            //Indexer is a special property
            foreach (PropertyDefinition propertyDefinition in typeDefinition.Properties)
            {
                ModifyPropertyDefinition(propertyDefinition);
            }

            //modify interface methods attribute
            //Getter and setter is a special method
            foreach (MethodDefinition methodDefinition in typeDefinition.Methods.Where(x => (!x.IsGetter && !x.IsSetter) || !x.IsHideBySig))
            {
                ModifyMethodDefinition(methodDefinition);
            }

            //modify interface events attribute
            foreach (EventDefinition eventDefinition in typeDefinition.Events)
            {
                ModifyEventDefinition(eventDefinition);
            }
        }
    }

    private void ModifyMethodDefinition(MethodDefinition methodDefinition)
    {
        if (CheckObfuscationAttribute(methodDefinition, out var obfuscationAttribute2))
        {
            ReplaceObfuscationAttribute(methodDefinition, obfuscationAttribute2);
        }

        foreach (ParameterDefinition parameterDefinition in methodDefinition.Parameters)
        {
            ModifyParameterDefinition(parameterDefinition);
        }
    }

    private void ModifyEventDefinition(EventDefinition eventDefinition)
    {
        if (CheckObfuscationAttribute(eventDefinition, out var obfuscationAttribute2))
        {
            ReplaceObfuscationAttribute(eventDefinition, obfuscationAttribute2);
        }

        if (CheckObfuscationAttribute(eventDefinition.AddMethod, out var obfuscationAttribute3))
        {
            ReplaceObfuscationAttribute(eventDefinition.AddMethod, obfuscationAttribute3);
        }

        if (CheckObfuscationAttribute(eventDefinition.RemoveMethod, out var obfuscationAttribute4))
        {
            ReplaceObfuscationAttribute(eventDefinition.RemoveMethod, obfuscationAttribute4);
        }

        if (CheckObfuscationAttribute(eventDefinition.InvokeMethod, out var obfuscationAttribute5))
        {
            ReplaceObfuscationAttribute(eventDefinition.InvokeMethod, obfuscationAttribute5);
        }
    }

    private void ModifyPropertyDefinition(PropertyDefinition propertyDefinition)
    {
        if (CheckObfuscationAttribute(propertyDefinition, out var obfuscationAttribute2))
        {
            ReplaceObfuscationAttribute(propertyDefinition, obfuscationAttribute2);
        }

        if (CheckObfuscationAttribute(propertyDefinition.GetMethod, out var obfuscationAttribute3))
        {
            ReplaceObfuscationAttribute(propertyDefinition.GetMethod, obfuscationAttribute3);
        }

        if (CheckObfuscationAttribute(propertyDefinition.SetMethod, out var obfuscationAttribute4))
        {
            ReplaceObfuscationAttribute(propertyDefinition.SetMethod, obfuscationAttribute4);
        }
    }

    private void ModifyParameterDefinition(ParameterDefinition parameterDefinition)
    {
        if (CheckObfuscationAttribute(parameterDefinition, out var obfuscationAttribute2))
        {
            ReplaceObfuscationAttribute(parameterDefinition, obfuscationAttribute2);
        }
    }

    private void ModifyFieldDefinition(FieldDefinition fieldDefinition)
    {
        if (CheckObfuscationAttribute(fieldDefinition, out var obfuscationAttribute2))
        {
            ReplaceObfuscationAttribute(fieldDefinition, obfuscationAttribute2);
        }
    }

    private void ModifyTypeDefinition(TypeDefinition typeDefinition)
    {
        if (CheckObfuscationAttribute(typeDefinition, out var obfuscationAttribute2))
        {
            ReplaceObfuscationAttribute(typeDefinition, obfuscationAttribute2);
        }
    }

    private static void RemovePartTypeDefinitions(ICollection<TypeDefinition> target, IEnumerable<TypeDefinition> typeDefinitions)
    {
        foreach (var item in typeDefinitions)
        {
            target.Remove(item);
        }
    }
}
