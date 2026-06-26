using KriegspielTicTacToe.Model.Template;

namespace KriegspielTicTacToe;

/// <summary>
/// Serialization binder ensuring that unexpected complex types are not deserialized.
/// </summary>
public class KnownTypesBinder : Newtonsoft.Json.Serialization.ISerializationBinder {
    public static KnownTypesBinder Instance {get;} = new KnownTypesBinder();
    //TODO: Switch this to a reflection-based thing.
    public Dictionary<string, Type> KnownTypes {get;} = new Type[] {
        typeof(BoardBuilder),
        typeof(BoardRuleset),
        typeof(GameState),
        typeof(MNKTemplate),
        typeof(MNKAction),
        typeof(MNKRuleset),
        typeof(RoundRobinPlayManager),
        typeof(RoundRobinPlayManagerFactory),
        typeof(SynchronizedPlayManager),
        typeof(SynchronizedPlayManagerFactory),
        // the following type(s) are generally serialized without type data but
        // including them here prevents an exception if found unexpectedly.
        typeof(Player),
        typeof(PlayerAction),
        typeof(Board),
        typeof(Space),
    }.ToDictionary(t => t.Name);

    public Type BindToType(string? assemblyName, string? typeName) {
        ArgumentNullException.ThrowIfNull(typeName);
        return KnownTypes[typeName];
    }

    public void BindToName(Type serializedType, out string? assemblyName, out string typeName) {
        assemblyName = null;
        typeName = serializedType.Name;
    }
}
