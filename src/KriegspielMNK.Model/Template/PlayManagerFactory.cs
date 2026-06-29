namespace KriegspielMNK.Model.Template;

public abstract record PlayManagerFactory() {
    public abstract PlayManager Create(IReadOnlyList<Player> players);
}
