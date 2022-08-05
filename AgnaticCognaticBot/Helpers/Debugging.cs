namespace AgnaticCognaticBot.Helpers;

public abstract class Debugging
{
    public static bool IsDebugMode()
    {
        #if DEBUG
            return true;
        #else
            return false;
        #endif
    }
}