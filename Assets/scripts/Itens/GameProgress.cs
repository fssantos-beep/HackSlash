// Sistema de progresso do jogo, rastreando eventos importantes como derrotar o chefe
public static class GameProgress
{
    public static bool bossDefeated = false;

    public static void ResetProgress()
    {
        bossDefeated = false;
    }
}