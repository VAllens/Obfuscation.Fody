using System.Reflection;

namespace ObfuscationFodySamples
{
    [Obfuscation(Exclude = false)]
    internal delegate int AllenDelegate([Obfuscation(Exclude = false)] AllenStruct as1);

    [Obfuscation(Exclude = false)]
    internal enum AllenEnum
    {
        [Obfuscation(Exclude = false)]
        One,
        [Obfuscation(Exclude = false)]
        Two
    }

    [Obfuscation(Exclude = false)]
    internal struct AllenStruct
    {
        public AllenStruct([Obfuscation(Exclude = false)] int property2)
        {
            Property2 = property2;
            Property1 = AllenEnum.One;
            AllenDelegate = new AllenDelegate((a) => { return a.Property2; });
        }

        [Obfuscation(Exclude = false)]
        private readonly Dictionary<int, string> dict = new();

        [Obfuscation(Exclude = false)]
        public string this[int index] { [Obfuscation(Exclude = false)] get => dict[index]; [Obfuscation(Exclude = false)] set => dict[index] = value; }

        [Obfuscation(Exclude = false)]
        public event AllenDelegate? AllenDelegate;

        [Obfuscation(Exclude = false)]
        public AllenEnum Property1 { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] init; }

        [Obfuscation(Exclude = false)]
        public readonly int Property2;

        [Obfuscation(Exclude = false)]
        public void Method1([Obfuscation(Exclude = false)] AllenEnum parameter1)
        {
            Console.WriteLine(parameter1);
        }

        [Obfuscation(Exclude = false)]
        public static explicit operator AllenStruct([Obfuscation(Exclude = false)] AllenRecord obj)
        {
            return new AllenStruct(obj.Field2) { Property1 = obj.Field1 };
        }

        [Obfuscation(Exclude = false)]
        public static implicit operator AllenRecord([Obfuscation(Exclude = false)] AllenStruct obj)
        {
            return new AllenRecord(obj.Property1) { Field2 = obj.Property2 };
        }
    }

    [Obfuscation(Exclude = false)]
    internal record AllenRecord
    {
        [Obfuscation(Exclude = false)]
        public int Field2 { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] init; }

        [Obfuscation(Exclude = false)]
        public readonly AllenEnum Field1;

        [Obfuscation(Exclude = false)]
        public event AllenDelegate? AllenDelegate;

        public AllenRecord([Obfuscation(Exclude = false)] AllenEnum field1)
        {
            Field1 = field1;
        }

        [Obfuscation(Exclude = false)]
        public void Method1([Obfuscation(Exclude = false)] AllenEnum parameter1)
        {
            Console.WriteLine(parameter1);
        }

        [Obfuscation(Exclude = false)]
        public static explicit operator AllenRecord([Obfuscation(Exclude = false)] AllenStruct obj)
        {
            return new AllenRecord(obj.Property1) { Field2 = obj.Property2 };
        }

        [Obfuscation(Exclude = false)]
        public static implicit operator AllenStruct([Obfuscation(Exclude = false)] AllenRecord obj)
        {
            return new AllenStruct(obj.Field2) { Property1 = obj.Field1 };
        }

        [Obfuscation(Exclude = false)]
        ~AllenRecord()
        {
            Console.WriteLine("Destructor");
        }
    }

    [Obfuscation(Exclude = false)]
    internal interface IAllenInterface
    {
        [Obfuscation(Exclude = false)]
        event AllenDelegate AllenDelegate;

        [Obfuscation(Exclude = false)]
        int Property1 { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] init; }

        [Obfuscation(Exclude = false)]
        string this[int index] { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] set; }

        [Obfuscation(Exclude = false)]
        void Run([Obfuscation(Exclude = false)] AllenEnum num);
    }

    [Obfuscation(Exclude = false, Feature = "I'm a feature", StripAfterObfuscation = false)]
    internal sealed class AllenClass : IAllenInterface
    {
        [Obfuscation(Exclude = false)]
        int IAllenInterface.Property1 { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] init; }

        [Obfuscation(Exclude = false)]
        private readonly Dictionary<int, string> dict = new();

        [Obfuscation(Exclude = false)]
        public string this[int index] { [Obfuscation(Exclude = false)] get => dict[index]; [Obfuscation(Exclude = false)] set => dict[index] = value; }

        [Obfuscation(Exclude = false)]
        public event AllenDelegate AllenDelegate;

        public AllenClass([Obfuscation(Exclude = false)] AllenDelegate allenDelegate)
        {
            AllenDelegate = allenDelegate;
        }

        [Obfuscation(Exclude = false)]
        public void Run([Obfuscation(Exclude = false)] AllenEnum parameter1)
        {
            AllenStruct as1 = new(1)
            {
                Property1 = parameter1
            };

            int property2 = AllenDelegate(as1);
            Console.WriteLine("property2: " + property2);
        }

        [Obfuscation(Exclude = false)]
        ~AllenClass()
        {
            Console.WriteLine("Destructor");
        }
    }
}


