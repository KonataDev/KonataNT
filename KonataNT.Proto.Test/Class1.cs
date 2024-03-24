namespace KonataNT.Proto.Test;

internal class Program
{
    public static void Main(string[] args)
    {
        var awoo = new Awoo
        {
            Wolf1 = new Awoo1 { Wolf1 = "Wolf1" },
            Wolf2 = new Awoo1 { Wolf1 = "Wolf2" },
            Type = 1,
            Type1 = "Type1"
        };
        Console.WriteLine(awoo);

        var info = awoo.Serialize();
    }
    
}

[ProtoContract]
internal partial class Awoo
{
    [ProtoMember(1)] public Awoo1 Wolf1 { get; set; }

    [ProtoMember(2)] public Awoo1 Wolf2 { get; set; }

    [ProtoMember(3)] public uint Type { get; set; }

    [ProtoMember(4)] public string Type1 { get; set; }
    
    [ProtoMember(5)] public string Type2 { get; set; }
}

[ProtoContract]
internal partial class Awoo1
{
    [ProtoMember(1)] public string Wolf1 { get; set; }
}