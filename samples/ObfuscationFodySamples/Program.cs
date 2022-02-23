// See https://aka.ms/new-console-template for more information

using System.Reflection;
using ObfuscationFodySamples;

Type type = typeof(IAllenInterface);
Type obfuscationAttributeType = typeof(ObfuscationAttribute);

CustomAttributeData obfuscationAttributeInstance = type.CustomAttributes.First(x => x.AttributeType == obfuscationAttributeType);
CustomAttributeNamedArgument feature = obfuscationAttributeInstance.NamedArguments.First(x => x.MemberName == nameof(ObfuscationAttribute.Feature));
Console.WriteLine("Obfuscation Feature: " + feature.TypedValue.Value);

Console.ReadKey();