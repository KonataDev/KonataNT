using KonataNT.Message.Chains;

namespace KonataNT.Message;

public class MessageChain : List<BaseChain>
{
    /// <summary>
    /// Convert chain to preview string
    /// </summary>
    /// <returns></returns>
    internal string ToPreviewString()
        => this.Aggregate("", (current, element) => current + element.ToPreviewString());

    /// <summary>
    /// Find chains
    /// </summary>
    /// <typeparam name="TChain"></typeparam>
    /// <returns></returns>
    public List<TChain> FindChain<TChain>()
        => this.Where(i => i is TChain).Cast<TChain>().ToList();

    /// <summary>
    /// Get a chain
    /// </summary>
    /// <typeparam name="TChain"></typeparam>
    /// <returns></returns>
    public TChain? GetChain<TChain>()
        => FindChain<TChain>().FirstOrDefault();

    /// <summary>
    /// Filter the message chain with a chain type
    /// </summary>
    /// <param name="x"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator |(MessageChain x, BaseChain.ChainType type)
        => x.Where(c => c.Type != type);

    /// <summary>
    /// Filter the message chain with a chain type
    /// </summary>
    /// <param name="x"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator &(MessageChain x, BaseChain.ChainType type)
        => x.Where(c => c.Type == type);

    /// <summary>
    /// Filter the message chain with a chain mode
    /// </summary>
    /// <param name="x"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator |(MessageChain x, BaseChain.ChainMode mode)
        => x.Where(c => c.Mode != mode);

    /// <summary>
    /// Filter the message chain with a chain mode
    /// </summary>
    /// <param name="x"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static IEnumerable<BaseChain> operator &(MessageChain x, BaseChain.ChainMode mode)
        => x.Where(c => c.Mode == mode);
    
    public List<BaseChain> this[Range r]
    {
        get
        {
            var (offset, length) = r.GetOffsetAndLength(Count);
            return GetRange(offset, length);
        }
    }

    public List<BaseChain> this[Type type]
        => this.Where(c => c.GetType() == type).ToList();

    public List<BaseChain> this[BaseChain.ChainMode mode]
        => this.Where(c => c.Mode == mode).ToList();

    public List<BaseChain> this[BaseChain.ChainType type]
        => this.Where(c => c.Type == type).ToList();
}