namespace KonataNT.Message.Chains;

public abstract class BaseChain(BaseChain.ChainType type, BaseChain.ChainMode mode)
{
    public enum ChainType
    {
        At,
        Reply,
        Text,
        Image,
        Flash,
        Record,
        Video,
        QFace,
        BFace,
        Xml,
        MultiMsg,
        Json,
        File,
    }

    public enum ChainMode
    {
        Multiple,
        Singleton,
        SingleTag
    }

    public ChainType Type { get; protected set; } = type;

    public ChainMode Mode { get; protected set; } = mode;

    internal abstract string ToPreviewString();
}