// See https://aka.ms/new-console-template for more information

using System.Reflection;
using ObfuscationFodySamples;

Console.WriteLine("Hello, World!");

IAllenInterface test = new AllenClass([Obfuscation(Exclude = true)] (abc) =>
{
    AllenRecord ar = new(abc.Property1)
    {
        Field2 = abc.Property2 + 12
    };
    Console.WriteLine(ar);

    return abc.Property2;
});

test.Run(AllenEnum.One);

test[1] = "Hello, Allen!";
Console.WriteLine(test[1]);

Console.ReadKey();