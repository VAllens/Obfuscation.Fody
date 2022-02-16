using System.Reflection;

namespace ObfuscationFodySamples
{
    [Obfuscation(Exclude = false)]
    internal delegate int AllenDelegate([Obfuscation(Exclude = false)] AllenStruct as1);

    [Obfuscation(Exclude = false)]
    internal record AllenRecord
    {
        [Obfuscation(Exclude = false)]
        public int Property2 { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] init; }

        [Obfuscation(Exclude = false)]
        public AllenEnum Property1;

        [Obfuscation(Exclude = false)]
        public static explicit operator AllenRecord([Obfuscation(Exclude = false)] AllenStruct obj)
        {
            return new AllenRecord() { Property1 = obj.Property1, Property2 = obj.Property2 };
        }

        [Obfuscation(Exclude = false)]
        public static implicit operator AllenStruct([Obfuscation(Exclude = false)] AllenRecord obj)
        {
            return new AllenStruct(obj.Property2) { Property1 = obj.Property1 };
        }
    }

    [Obfuscation(Exclude = false)]
    internal interface IAllenTest
    {
        [Obfuscation(Exclude = false)]
        string this[int index] { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] set; }

        [Obfuscation(Exclude = false)]
        void Run([Obfuscation(Exclude = false)] AllenEnum num);
    }

    [Obfuscation(Exclude = false, StripAfterObfuscation = false)]
    internal sealed class AllenTest : IAllenTest
    {
        [Obfuscation(Exclude = false)]
        private readonly Dictionary<int, string> dict = new();

        [Obfuscation(Exclude = false)]
        public string this[int index] { [Obfuscation(Exclude = false)] get => dict[index]; [Obfuscation(Exclude = false)] set => dict[index] = value; }

        [Obfuscation(Exclude = false)]
        private event AllenDelegate AllenDelegate;

        public AllenTest([Obfuscation(Exclude = false)] AllenDelegate allenDelegate)
        {

            AllenDelegate = allenDelegate;
        }

        [Obfuscation(Exclude = false)]
        public void Run([Obfuscation(Exclude = false)] AllenEnum num)
        {
            AllenStruct as1 = new(1)
            {
                Property1 = num
            };
            int property2 = AllenDelegate(as1);
            Console.WriteLine("property2: " + property2);
        }

        [Obfuscation(Exclude = false)]
        ~AllenTest()
        {
            Console.WriteLine("Destructor");
        }
    }

    [Obfuscation(Exclude = false)]
    internal enum AllenEnum
    {
        [Obfuscation(Exclude = false)]
        One,
        [Obfuscation(Exclude = false)]
        Two
    }

    [Obfuscation(Exclude = false)]
    internal readonly struct AllenStruct
    {
        public AllenStruct([Obfuscation(Exclude = false)] int property2)
        {
            Property2 = property2;
            Property1 = AllenEnum.One;
        }

        [Obfuscation(Exclude = false)]
        public AllenEnum Property1 { [Obfuscation(Exclude = false)] get; [Obfuscation(Exclude = false)] init; }

        [Obfuscation(Exclude = false)]
        public readonly int Property2;

        [Obfuscation(Exclude = false)]
        public static explicit operator AllenStruct([Obfuscation(Exclude = false)] AllenRecord obj)
        {
            return new AllenStruct(obj.Property2) { Property1 = obj.Property1 };
        }

        [Obfuscation(Exclude = false)]
        public static implicit operator AllenRecord([Obfuscation(Exclude = false)] AllenStruct obj)
        {
            return new AllenRecord() { Property1 = obj.Property1, Property2 = obj.Property2 };
        }
    }
}
