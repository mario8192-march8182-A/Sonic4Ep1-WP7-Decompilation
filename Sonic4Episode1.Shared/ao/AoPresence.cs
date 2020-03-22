
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


#if DESKTOP || (WINDOWS_UAP && !ARM)
using Discord;
#endif

public enum AoPresenceLocation
{
    TitleScreen,
    StageSelect,
    Stage
}

public static class AoPresence
{
#if DESKTOP || (WINDOWS_UAP && !ARM)
    private static Discord.Discord discord;
    private static Discord.ActivityManager activityManager;
#endif

    // Token: 0x060002BB RID: 699 RVA: 0x00016F33 File Offset: 0x00015133
    public static void Init()
    {
#if DESKTOP || (WINDOWS_UAP && !ARM)
        try
        {
            discord = new Discord.Discord(686320985669894185, (ulong)CreateFlags.NoRequireDiscord);
            discord.SetLogHook(LogLevel.Debug, (level, message) => Debug.WriteLine($"{level}: {message}", "Discord"));
            activityManager = discord.GetActivityManager();

            AoPresenceSet(AoPresenceLocation.TitleScreen);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
#endif
    }

    public static void AoPresenceUpdate()
    {
#if DESKTOP || (WINDOWS_UAP && !ARM)
        if (discord != null)
        {
            try
            {
                discord.RunCallbacks();
            }
            catch (Exception)
            {
                discord = null; // this shouldnt fucking throw discord you monkey fucks
            }
        }
#endif
    }

    // Token: 0x060002BC RID: 700 RVA: 0x00016F35 File Offset: 0x00015135
    public static bool AoPresenceInitialized()
    {
        return true;
    }

    // Token: 0x060002BD RID: 701 RVA: 0x00016F38 File Offset: 0x00015138
    public static void AoPresenceExit()
    {
#if DESKTOP || (WINDOWS_UAP && !ARM)
        if (discord == null)
            return;

        discord.Dispose();
#endif
    }

    // Token: 0x060002BE RID: 702 RVA: 0x00016F3A File Offset: 0x0001513A
    public static void AoPresenceSet(AoPresenceLocation location)
    {
#if DESKTOP || (WINDOWS_UAP && !ARM)
        if (discord == null)
            return;

        var discordPresence = new Activity() { ApplicationId = 686320985669894185 };
        discordPresence.Details = AoPresenceGetTitle(location);
        discordPresence.State = AoPresenceGetSubtitle(location);
        discordPresence.Assets = new ActivityAssets()
        {
            LargeImage = AoPresenceGetImageKey(location)
        };

        if (AppMain.GmMainCheckExeTimerCount())
        {
            var epoch = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var now = DateTimeOffset.UtcNow;
            var timeSpan = (long)(now - epoch).TotalMilliseconds;

            discordPresence.Timestamps = new ActivityTimestamps()
            {
                Start = timeSpan
            };
        }

        activityManager.UpdateActivity(discordPresence, null);
#endif
    }

    private static string AoPresenceGetTitle(AoPresenceLocation location)
    {
        if (location == AoPresenceLocation.StageSelect)
        {
            return "Stage Select";
        }

        if (location == AoPresenceLocation.TitleScreen)
        {
            return "Title Screen";
        }

        switch (AppMain.g_gs_main_sys_info.stage_id)
        {
            case 0:
                return "Splash Hill Zone - Act 1";
            case 1:
                return "Splash Hill Zone - Act 2";
            case 2:
                return "Splash Hill Zone - Act 3";
            case 3:
                return "Splash Hill Zone - Boss";

            case 4:
                return "Casino Street Zone - Act 1";
            case 5:
                return "Casino Street Zone - Act 2";
            case 6:
                return "Casino Street Zone - Act 3";
            case 7:
                return "Casino Street Zone - Boss";

            case 8:
                return "Lost Labyrinth Zone - Act 1";
            case 9:
                return "Lost Labyrinth Zone - Act 2";
            case 10:
                return "Lost Labyrinth Zone - Act 3";
            case 11:
                return "Lost Labyrinth Zone - Boss";

            case 12:
                return "Mad Gear Zone - Act 1";
            case 13:
                return "Mad Gear Zone - Act 2";
            case 14:
                return "Mad Gear Zone - Act 3";
            case 15:
                return "Mad Gear Zone - Boss";

            case 16:
                return "E.G.G. Station Zone";

                //case AOE_PRESENCE.AOD_PRESENCE_SS1T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS2T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS3T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS4T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS5T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS6T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS7T:
                //    break;

                //case AOE_PRESENCE.AOD_PRESENCE_NUM:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_NONE:
                //    break;
                //default:
                //    break;
        }

        Debug.WriteLine($"Unknown stage: {AppMain.g_gs_main_sys_info.stage_id}");

        return "Menuing";
    }

    private static string AoPresenceGetSubtitle(AoPresenceLocation location)
    {
        if (location == AoPresenceLocation.StageSelect)
        {
            return "";
        }

        if (location == AoPresenceLocation.TitleScreen)
        {
            return "";
        }

        switch (AppMain.g_gs_main_sys_info.stage_id)
        {
            case 0:
                return "The Adventure Begins";
            case 1:
                return "High-Speed Athletics";
            case 2:
                return "Sunset Dash";
            case 3:
                return "Showdown with Dr. Eggman";

            // Casino Street
            case 4:
                return "Neon City Adrift in the Night";
            case 5:
                return "100,000 Point Challenge";
            case 6:
                return "Casino Climax";
            case 7:
                return "Dr. Eggman's Party";

            // Lost Labyrinth 
            case 8:
                return "Ancient Maze of Mystery";
            case 9:
                return "Strange Mine Cart";
            case 10:
                return "Underwater Maze Escape";
            case 11:
                return "Trap-Filed Ruins";

            // Mad Gear 
            case 12:
                return "Dr. Eggman's Secret Base";
            case 13:
                return "Escape the Cog Trap";
            case 14:
                return "Impending Doom";
            case 15:
                return "Defeat the Real Dr. Eggman";

            // E.G.G. Station
            case 16:
                return "Final Showdown in Space";

            // special stages
            //case AOE_PRESENCE.AOD_PRESENCE_SS1T:
            //    break;
            //case AOE_PRESENCE.AOD_PRESENCE_SS2T:
            //    break;
            //case AOE_PRESENCE.AOD_PRESENCE_SS3T:
            //    break;
            //case AOE_PRESENCE.AOD_PRESENCE_SS4T:
            //    break;
            //case AOE_PRESENCE.AOD_PRESENCE_SS5T:
            //    break;
            //case AOE_PRESENCE.AOD_PRESENCE_SS6T:
            //    break;
            //case AOE_PRESENCE.AOD_PRESENCE_SS7T:
            //    break;

            //case AOE_PRESENCE.AOD_PRESENCE_NUM:
            //case AOE_PRESENCE.AOD_PRESENCE_NONE:
            //    break;
            default:
                break;
        }

        Debug.WriteLine($"Unknown stage: {AppMain.g_gs_main_sys_info.stage_id}");

        return "NYI";
    }

    private static string AoPresenceGetImageKey(AoPresenceLocation location)
    {
        if (location == AoPresenceLocation.StageSelect)
        {
            return "stage-select";
        }

        if (location == AoPresenceLocation.TitleScreen)
        {
            return "title-screen";
        }

        switch (AppMain.g_gs_main_sys_info.stage_id)
        {
            case 0:
                return "stage-z1s1";
            case 1:
                return "stage-z1s2";
            case 2:
                return "stage-z1s3";
            case 3:
                return "stage-z1sb";

            // Casino Street
            case 4:
                return "stage-z2s1";
            case 5:
                return "stage-z2s2";
            case 6:
                return "stage-z2s3";
            case 7:
                return "stage-z2sb";

            // Lost Labyrinth 
            case 8:
                return "stage-z3s1";
            case 9:
                return "stage-z3s2";
            case 10:
                return "stage-z3s3";
            case 11:
                return "stage-z3sb";

            // Mad Gear 
            case 12:
                return "stage-z4s1";
            case 13:
                return "stage-z4s2";
            case 14:
                return "stage-z4s3";
            case 15:
                return "stage-z4sb";

            // E.G.G. Station
            case 16:
                return "stage-zfsb";

                // special stages
                //case AOE_PRESENCE.AOD_PRESENCE_SS1T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS2T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS3T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS4T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS5T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS6T:
                //    break;
                //case AOE_PRESENCE.AOD_PRESENCE_SS7T:
                //    break;

                //case AOE_PRESENCE.AOD_PRESENCE_NUM:
                //case AOE_PRESENCE.AOD_PRESENCE_NONE:
                //    break;
                //default:
                //    break;
        }

        return "NYI";
    }
}