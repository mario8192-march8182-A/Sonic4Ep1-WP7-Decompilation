using System;

namespace Discord
{
    static class Constants
    {
#if x64
        public const string DllName = "x64/discord_game_sdk";
#else
        public const string DllName = "x86/discord_game_sdk";
#endif
    }
}
