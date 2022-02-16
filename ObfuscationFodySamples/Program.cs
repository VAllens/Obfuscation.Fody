// See https://aka.ms/new-console-template for more information
using System.Reflection;
using ConsoleAppSamples;

Console.WriteLine("Hello, World!");

IAllenTest test = new AllenTest([Obfuscation(Exclude = true)] (abc) =>
{
    AllenRecord ar = new AllenRecord()
    {
        Num = abc.Num + 12,
        Algo = abc.Algo
    };
    Console.WriteLine(ar);
    return abc.Algo;
});

test.Run(AllenEnum.One);

test[1] = "Hello, Allen!";
Console.WriteLine(test[1]);