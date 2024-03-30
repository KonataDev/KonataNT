namespace KonataNT.Message.Chains;

public class ImageChain : BaseChain
{
    /// <summary>
    /// Image Url
    /// </summary>
    public string? ImageUrl { get; protected set; }

    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; protected set; }

    /// <summary>
    /// File hash
    /// </summary>
    public string FileHash { get; protected set; }

    /// <summary>
    /// MD5 byte[]
    /// </summary>
    public byte[] HashData { get; protected set; }

    /// <summary>
    /// Image data
    /// </summary>
    public Stream? FileData { get; protected set; }

    /// <summary>
    /// Image data length
    /// </summary>
    public uint FileLength { get; protected set; }

    /// <summary>
    /// Image width
    /// </summary>
    public uint Width { get; protected set; }

    /// <summary>
    /// Image height
    /// </summary>
    public uint Height { get; protected set; }
    
    private ImageChain(string url, string fileName, string fileHash, uint width, uint height, uint length) : base(ChainType.Image, ChainMode.Multiple)
    {
        ImageUrl = url;
        FileName = fileName;
        FileHash = fileHash;
        FileLength = length;
        Width = width;
        Height = height;
        HashData = Array.Empty<byte>();
    }

    private ImageChain(Stream data, uint width, uint height, byte[] md5, string md5Str) : base(ChainType.Image, ChainMode.Multiple)
    {
        FileData = data;
        FileLength = (uint) data.Length;
        Width = width;
        Height = height;
        HashData = md5;
        FileHash = md5Str;
        FileName = md5Str;
    }
    
    /// <summary>
    /// Create an image chain
    /// </summary>
    /// <param name="url"></param>
    /// <param name="fileName"></param>
    /// <param name="fileHash"></param>
    /// <param name="length"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    internal static ImageChain Create(string url, string fileName, string fileHash, uint width, uint height, uint length) => 
        new(url, fileName, fileHash, width, height, length);


    internal override string ToPreviewString() => "[图片]";
}