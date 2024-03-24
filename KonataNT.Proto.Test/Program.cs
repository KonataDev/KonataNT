using KonataNT.Proto.Serialization;

namespace KonataNT.Proto.Test;

class Program
{
    static void Main(string[] args)
    {
        var testCase = new TestCase
        {
            Awoo = 1,
            Awoo1 = 123,
            Awoo2 = 114514,
            Array = [1, 2, 5, 6, 9]
        };
        var span = testCase.Serialize();
        Console.WriteLine(BitConverter.ToString(span).Replace("-", ""));
    }
}

[ProtoContract]
public partial class TestCase
{
    [ProtoMember(1)] public uint Awoo { get; set; }
    
    [ProtoMember(2)] public uint Awoo1 { get; set; }
    
    [ProtoMember(3)] public uint Awoo2 { get; set; }

    [ProtoMember(4)] public string Awoo3 { get; set; } = "This is a test string";

    [ProtoMember(5)] public byte[] Awoo4 { get; set; } = new byte[4];

    [ProtoMember(6)] public TestCase1 Nested { get; set; } = new();
    
    [ProtoMember(7)] public int[] Array { get; set; } = [];
}

[ProtoContract]
public partial class TestCase1
{
    [ProtoMember(1)] public uint Awoo { get; set;}
}
