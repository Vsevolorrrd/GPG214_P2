
public class UnlockablesManager : Singleton<UnlockablesManager>
{
    public void CheckUnlockConditions(float totalPlaytime, int mapsGenerated, int tilesClaimed)
    {
        foreach (var player in PlayerManager.Instance.nations)
        {
            if (player.isUnlocked) continue;

            if (player.nationName == "Player 3" && totalPlaytime >= 60f)
            {
                PlayerManager.Instance.UnlockPlayer(player);
            }

            if (player.nationName == "Player 4" && mapsGenerated >= 25)
            {
                PlayerManager.Instance.UnlockPlayer(player);
            }

            if (player.nationName == "Player 5" && tilesClaimed >= 100)
            {
                PlayerManager.Instance.UnlockPlayer(player);
            }
        }
    }
    public void LockPlayers()
    {
        foreach (var player in PlayerManager.Instance.nations)
        {
            if (player.nationName == "Player 3")
            {
                PlayerManager.Instance.LockPlayer(player);
            }

            if (player.nationName == "Player 4")
            {
                PlayerManager.Instance.LockPlayer(player);
            }

            if (player.nationName == "Player 5")
            {
                PlayerManager.Instance.LockPlayer(player);
            }
        }
    }
}
