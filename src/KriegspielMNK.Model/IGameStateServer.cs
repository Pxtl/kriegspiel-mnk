namespace KriegspielMNK.Model;
public interface IGameStateServer {
    IPlayActionResult Attempt(PlayerAction action);
    void ResignPlayer(Player player);
}