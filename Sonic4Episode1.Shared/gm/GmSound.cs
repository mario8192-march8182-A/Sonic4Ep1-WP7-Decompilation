using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

public class GmSound
{
    private static GSS_SND_SCB gm_sound_bgm_scb;
    private static GSS_SND_SCB gm_sound_bgm_sub_scb;
    private static GSS_SND_SCB gm_sound_jingle_scb;
    private static GSS_SND_SCB gm_sound_jingle_bgm_scb;
    private static uint gm_sound_flag;
    private static MTS_TASK_TCB gm_sound_1shot_tcb;
    private static MTS_TASK_TCB gm_sound_bgm_fade_tcb;
    private static MTS_TASK_TCB gm_sound_bgm_win_boss_tcb;
    public static readonly string[] gm_sound_bgm_name_list;
    public static readonly string[] gm_sound_speedup_bgm_name_list;
    public static readonly string[] gm_sound_jingle_name_list;
    public static readonly string[] gm_sound_bgm_win_boss_name_list;
    public static readonly int[] gm_sound_bgm_win_boss_wait_frame_list;

    static GmSound()
    {
        gm_sound_bgm_name_list = new string[]
        {
            "snd_sng_z1a1",
            "snd_sng_z1a2",
            "snd_sng_z1a3",
            "snd_sng_boss1",
            "snd_sng_z2a1",
            "snd_sng_z2a2",
            "snd_sng_z2a3",
            "snd_sng_boss1",
            "snd_sng_z3a1",
            "snd_sng_z3a2",
            "snd_sng_z3a3",
            "snd_sng_boss1",
            "snd_sng_z4a1",
            "snd_sng_z4a2",
            "snd_sng_z4a3",
            "snd_sng_boss1",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_jin_clear_final"
        };
        gm_sound_speedup_bgm_name_list = new string[]
        {
            "snd_sng_z1a1_speedup",
            "snd_sng_z1a2_speedup",
            "snd_sng_z1a3_speedup",
            "snd_sng_boss1",
            "snd_sng_z2a1_speedup",
            "snd_sng_z2a2_speedup",
            "snd_sng_z2a3_speedup",
            "snd_sng_boss1",
            "snd_sng_z3a1_speedup",
            "snd_sng_z3a2_speedup",
            "snd_sng_z3a3_speedup",
            "snd_sng_boss1",
            "snd_sng_z4a1_speedup",
            "snd_sng_z4a2_speedup",
            "snd_sng_z4a3_speedup",
            "snd_sng_boss1",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_boss2",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_sng_special",
            "snd_jin_clear_final"
        };
        gm_sound_jingle_name_list = new string[]
        {
            "snd_jin_1up",
            "snd_jin_clear",
            "snd_jin_clear",
            "snd_jin_emerald",
            "snd_jin_invincible",
            "snd_jin_new_record",
            "snd_jin_obore",
            "snd_jin_gameover"
        };
        gm_sound_bgm_win_boss_name_list = new string[]
        {
            "snd_sng_z1a3",
            "snd_sng_z2a1",
            "snd_sng_z3a3",
            "snd_sng_z4a2",
            "snd_sng_boss2",
            "snd_sng_special"
        };
        gm_sound_bgm_win_boss_wait_frame_list = new int[]
        {
            180,
            180,
            180,
            180,
            180,
            180
        };
    }

    public static void Build()
    {
    }

    // Token: 0x0600081E RID: 2078 RVA: 0x0004794D File Offset: 0x00045B4D
    public static bool BuildCheck()
    {
        return true;
    }

    // Token: 0x0600081F RID: 2079 RVA: 0x00047950 File Offset: 0x00045B50
    public static void Flush()
    {
    }

    // Token: 0x06000820 RID: 2080 RVA: 0x00047954 File Offset: 0x00045B54
    public static void Init()
    {
        GsSound.Reset();
        gm_sound_bgm_scb = GsSound.AssignScb(0);
        gm_sound_bgm_sub_scb = GsSound.AssignScb(0);
        gm_sound_jingle_scb = GsSound.AssignScb(0);
        gm_sound_jingle_bgm_scb = GsSound.AssignScb(0);
        GsSound.Begin(3, 32767U, 5);
        gm_sound_flag = 0U;
        gm_sound_1shot_tcb = null;
        gm_sound_bgm_win_boss_tcb = null;
    }

    // Token: 0x06000821 RID: 2081 RVA: 0x000479B0 File Offset: 0x00045BB0
    public static void Exit()
    {
        if (gm_sound_1shot_tcb != null)
        {
            AppMain.mtTaskClearTcb(gm_sound_1shot_tcb);
        }

        if (gm_sound_bgm_fade_tcb != null)
        {
            AppMain.mtTaskClearTcb(gm_sound_bgm_fade_tcb);
        }

        if (gm_sound_bgm_win_boss_tcb != null)
        {
            AppMain.mtTaskClearTcb(gm_sound_bgm_win_boss_tcb);
        }

        GsSound.Halt();
        GsSound.End();
        if (gm_sound_jingle_scb != null)
        {
            GsSound.StopBgm(gm_sound_jingle_scb, 0);
            GsSound.ResignScb(gm_sound_jingle_scb);
            gm_sound_jingle_scb = null;
        }

        if (gm_sound_bgm_scb != null)
        {
            GsSound.StopBgm(gm_sound_bgm_scb, 0);
            GsSound.ResignScb(gm_sound_bgm_scb);
            gm_sound_bgm_scb = null;
        }

        if (gm_sound_bgm_sub_scb != null)
        {
            GsSound.StopBgm(gm_sound_bgm_sub_scb, 0);
            GsSound.ResignScb(gm_sound_bgm_sub_scb);
            gm_sound_bgm_sub_scb = null;
        }

        if (gm_sound_jingle_bgm_scb != null)
        {
            GsSound.StopBgm(gm_sound_jingle_bgm_scb, 0);
            GsSound.ResignScb(gm_sound_jingle_bgm_scb);
            gm_sound_jingle_bgm_scb = null;
        }
    }

    // Token: 0x06000822 RID: 2082 RVA: 0x00047A82 File Offset: 0x00045C82
    public static void PlaySE(string cue_name)
    {
        PlaySE(cue_name, null);
    }

    // Token: 0x06000823 RID: 2083 RVA: 0x00047A8B File Offset: 0x00045C8B
    public static void PlaySE(string cue_name, GSS_SND_SE_HANDLE se_handle)
    {
        GsSound.PlaySe(cue_name, se_handle);
    }

    // Token: 0x06000824 RID: 2084 RVA: 0x00047A94 File Offset: 0x00045C94
    public static void PlaySEForce(string cue_name, GSS_SND_SE_HANDLE se_handle)
    {
        GsSound.PlaySeForce(cue_name, se_handle, 0, false);
    }

    // Token: 0x06000825 RID: 2085 RVA: 0x00047A9F File Offset: 0x00045C9F
    public static void PlaySEForce(string cue_name, GSS_SND_SE_HANDLE se_handle, bool dontplay)
    {
        GsSound.PlaySeForce(cue_name, se_handle, 0, dontplay);
    }

    // Token: 0x06000826 RID: 2086 RVA: 0x00047AAA File Offset: 0x00045CAA
    public static void PlayBGM(string cue_name, int fade_frame)
    {
        PlayStageBGM(fade_frame);
    }

    // Token: 0x06000827 RID: 2087 RVA: 0x00047AB4 File Offset: 0x00045CB4
    public static void PlayStageBGM(int fade_frame)
    {
        GsSound.ScbSetVolume(gm_sound_bgm_scb, 1f);
        GsSound.ScbSetSeqMute(gm_sound_bgm_scb, false);
        GsSound.PlayBgm(gm_sound_bgm_scb,
            gm_sound_bgm_name_list[(int) AppMain.g_gs_main_sys_info.stage_id], fade_frame);
        gm_sound_bgm_scb.flag |= 2147483648U;
    }

    // Token: 0x06000828 RID: 2088 RVA: 0x00047B0C File Offset: 0x00045D0C
    public static void StopBGM(int fade_frame)
    {
        StopStageBGM(fade_frame);
    }

    // Token: 0x06000829 RID: 2089 RVA: 0x00047B14 File Offset: 0x00045D14
    public static void StopStageBGM(int fade_frame)
    {
        GsSound.StopBgm(gm_sound_bgm_scb, fade_frame);
    }

    // Token: 0x0600082A RID: 2090 RVA: 0x00047B21 File Offset: 0x00045D21
    public static void PauseStageBGM(int fade_frame)
    {
        GsSound.PauseBgm(gm_sound_bgm_scb, fade_frame);
    }

    // Token: 0x0600082B RID: 2091 RVA: 0x00047B2E File Offset: 0x00045D2E
    public static void ResumeStageBGM(int fade_frame)
    {
        GsSound.ResumeBgm(gm_sound_bgm_scb, fade_frame);
    }

    // Token: 0x0600082C RID: 2092 RVA: 0x00047B3C File Offset: 0x00045D3C
    public static void ChangeSpeedupBGM()
    {
        bool flag = false;
        bool flag2 = false;
        GsSound.StopBgm(gm_sound_bgm_sub_scb, 0);
        if (GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            flag = true;
        }

        if ((gm_sound_flag & 80U) != 0U)
        {
            flag2 = true;
        }

        if (flag || flag2)
        {
            StopStageBGM(0);
        }
        else
        {
            StopStageBGM(0);
        }

        if (flag)
        {
            PauseStageBGM(0);
        }

        GSS_SND_SCB gss_SND_SCB = gm_sound_bgm_scb;
        gm_sound_bgm_scb = gm_sound_bgm_sub_scb;
        gm_sound_bgm_sub_scb = gss_SND_SCB;
        GsSound.ScbSetVolume(gm_sound_bgm_scb, 1f);
        GsSound.ScbSetSeqMute(gm_sound_bgm_scb, false);
        GsSound.PlayBgm(gm_sound_bgm_scb,
            gm_sound_speedup_bgm_name_list[(int) AppMain.g_gs_main_sys_info.stage_id], 0);
        gm_sound_bgm_scb.flag |= 2147483648U;
        if (flag2)
        {
            gmSoundSetBGMFadeEnd(gm_sound_bgm_scb);
            GsSound.ScbSetVolume(gm_sound_bgm_scb, 0f);
            GsSound.ScbSetSeqMute(gm_sound_bgm_scb, true);
        }
    }

    // Token: 0x0600082D RID: 2093 RVA: 0x00047C18 File Offset: 0x00045E18
    public static void ChangeAngryBossBGM()
    {
        bool flag = false;
        bool flag2 = false;
        GsSound.StopBgm(gm_sound_bgm_sub_scb, 0);
        if (GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            flag = true;
        }

        if ((gm_sound_flag & 80U) != 0U)
        {
            flag2 = true;
        }

        if (flag || flag2)
        {
            StopStageBGM(0);
        }
        else
        {
            StopStageBGM(15);
        }

        GSS_SND_SCB gss_SND_SCB = gm_sound_bgm_scb;
        gm_sound_bgm_scb = gm_sound_bgm_sub_scb;
        gm_sound_bgm_sub_scb = gss_SND_SCB;
        GsSound.ScbSetVolume(gm_sound_bgm_scb, 1f);
        GsSound.ScbSetSeqMute(gm_sound_bgm_scb, false);
        GsSound.PlayBgm(gm_sound_bgm_scb, "snd_sng_boss2", 15);
        gm_sound_bgm_scb.flag |= 2147483648U;
        if (flag)
        {
            PauseStageBGM(0);
        }

        if (flag2)
        {
            gmSoundSetBGMFadeEnd(gm_sound_bgm_scb);
            GsSound.ScbSetVolume(gm_sound_bgm_scb, 0f);
            GsSound.ScbSetSeqMute(gm_sound_bgm_scb, true);
        }
    }

    // Token: 0x0600082E RID: 2094 RVA: 0x00047CF0 File Offset: 0x00045EF0
    public static void ChangeWinBossBGM()
    {
        if (AppMain.g_gs_main_sys_info.stage_id >= 16)
        {
            return;
        }

        if (gm_sound_bgm_win_boss_tcb == null)
        {
            gm_sound_bgm_win_boss_tcb = AppMain.MTM_TASK_MAKE_TCB(gmSoundBGMWinBossFunc,
                gmSoundBGMWinBossDest, 0U, 0, 32767U, 5, () => new GMS_SOUND_BGM_WIN_BOSS_MGR_WORK(),
                "GM_SOUND_WB");
            GMS_SOUND_BGM_WIN_BOSS_MGR_WORK gms_SOUND_BGM_WIN_BOSS_MGR_WORK =
                (GMS_SOUND_BGM_WIN_BOSS_MGR_WORK) gm_sound_bgm_win_boss_tcb.work;
            gms_SOUND_BGM_WIN_BOSS_MGR_WORK.Clear();
            gms_SOUND_BGM_WIN_BOSS_MGR_WORK.timer =
                gm_sound_bgm_win_boss_wait_frame_list[AppMain.GMM_MAIN_GET_ZONE_TYPE()];
        }
    }

    // Token: 0x0600082F RID: 2095 RVA: 0x00047D88 File Offset: 0x00045F88
    public static void ChangeFinalBossBGM()
    {
        bool flag = false;
        bool flag2 = false;
        GsSound.StopBgm(gm_sound_bgm_sub_scb, 0);
        if (GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            flag = true;
        }

        if ((gm_sound_flag & 80U) != 0U)
        {
            flag2 = true;
        }

        if (flag || flag2)
        {
            StopStageBGM(0);
        }
        else
        {
            StopStageBGM(15);
        }

        GSS_SND_SCB gss_SND_SCB = gm_sound_bgm_scb;
        gm_sound_bgm_scb = gm_sound_bgm_sub_scb;
        gm_sound_bgm_sub_scb = gss_SND_SCB;
        GsSound.ScbSetVolume(gm_sound_bgm_scb, 1f);
        GsSound.ScbSetSeqMute(gm_sound_bgm_scb, false);
        GsSound.PlayBgm(gm_sound_bgm_scb, "snd_sng_final", 15);
        gm_sound_bgm_scb.flag |= 2147483648U;
        if (flag)
        {
            PauseStageBGM(0);
        }

        if (flag2)
        {
            gmSoundSetBGMFadeEnd(gm_sound_bgm_scb);
            GsSound.ScbSetVolume(gm_sound_bgm_scb, 0f);
            GsSound.ScbSetSeqMute(gm_sound_bgm_scb, true);
        }
    }

    // Token: 0x06000830 RID: 2096 RVA: 0x00047E59 File Offset: 0x00046059
    public static void PlayJingle(uint jngl_idx)
    {
        PlayJingle(jngl_idx, 0);
    }

    // Token: 0x06000831 RID: 2097 RVA: 0x00047E64 File Offset: 0x00046064
    public static void PlayJingle(uint jngl_idx, int fade_frame)
    {
        GsSound.ScbSetVolume(gm_sound_jingle_scb, 1f);
        GsSound.ScbSetSeqMute(gm_sound_jingle_scb, false);
        GsSound.StopBgm(gm_sound_jingle_scb);
        GsSound.PlayBgm(gm_sound_jingle_scb,
            gm_sound_jingle_name_list[(int) jngl_idx], fade_frame);
        gm_sound_jingle_scb.flag |= 2147483648U;
    }

    // Token: 0x06000832 RID: 2098 RVA: 0x00047EBE File Offset: 0x000460BE
    public static void StopJingle(int fade_frame)
    {
        GsSound.StopBgm(gm_sound_jingle_scb, fade_frame);
    }

    // Token: 0x06000833 RID: 2099 RVA: 0x00047ECC File Offset: 0x000460CC
    public static void PlayBGMJingle(uint jngl_idx, int fade_frame)
    {
        GsSound.ScbSetVolume(gm_sound_jingle_bgm_scb, 1f);
        GsSound.ScbSetSeqMute(gm_sound_jingle_bgm_scb, false);
        GsSound.StopBgm(gm_sound_jingle_bgm_scb);
        GsSound.PlayBgm(gm_sound_jingle_bgm_scb,
            gm_sound_jingle_name_list[(int) jngl_idx], fade_frame);
        gm_sound_jingle_bgm_scb.flag |= 2147483648U;
    }

    // Token: 0x06000834 RID: 2100 RVA: 0x00047F26 File Offset: 0x00046126
    public static void StopBGMJingle(int fade_frame)
    {
        GsSound.StopBgm(gm_sound_jingle_bgm_scb, fade_frame);
    }

    // Token: 0x06000835 RID: 2101 RVA: 0x00047F33 File Offset: 0x00046133
    public static void PauseBGMJingle(int fade_frame)
    {
        GsSound.PauseBgm(gm_sound_jingle_bgm_scb, fade_frame);
    }

    // Token: 0x06000836 RID: 2102 RVA: 0x00047F40 File Offset: 0x00046140
    public static void ResumeBGMJingle(int fade_frame)
    {
        GsSound.ResumeBgm(gm_sound_jingle_bgm_scb, fade_frame);
    }

    // Token: 0x06000837 RID: 2103 RVA: 0x00047F4D File Offset: 0x0004614D
    public static void SetVolumeSE(float volume)
    {
        GsSound.SetVolume(1, volume);
    }

    // Token: 0x06000838 RID: 2104 RVA: 0x00047F56 File Offset: 0x00046156
    public static void SetVolumeBGM(float volume)
    {
        GsSound.SetVolume(0, volume);
    }

    // Token: 0x06000839 RID: 2105 RVA: 0x00047F60 File Offset: 0x00046160
    public static void PlayJingleObore()
    {
        if (!GsSound.IsBgmStop(gm_sound_jingle_bgm_scb))
        {
            return;
        }

        if ((gm_sound_flag & 1U) != 0U)
        {
            return;
        }

        if (!GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            PauseStageBGM(0);
        }

        PlayBGMJingle(6U, 0);
        if ((gm_sound_flag & 32U) != 0U)
        {
            gmSoundSetBGMFadeEnd(gm_sound_jingle_bgm_scb);
            GsSound.ScbSetVolume(gm_sound_jingle_bgm_scb, 0f);
            GsSound.ScbSetSeqMute(gm_sound_jingle_bgm_scb, true);
        }

        gm_sound_flag |= 1U;
    }

    // Token: 0x0600083A RID: 2106 RVA: 0x00047FD8 File Offset: 0x000461D8
    public static void StopJingleObore()
    {
        if ((gm_sound_flag & 1U) == 0U)
        {
            return;
        }

        if (!GsSound.IsBgmPause(gm_sound_jingle_bgm_scb) && (gm_sound_flag & 32U) == 0U)
        {
            StopBGMJingle(15);
        }
        else
        {
            StopBGMJingle(0);
        }

        if ((gm_sound_flag & 2147483648U) == 0U)
        {
            ResumeStageBGM(0);
        }

        gm_sound_flag &= 4294967294U;
    }

    // Token: 0x0600083B RID: 2107 RVA: 0x00048034 File Offset: 0x00046234
    public static void PlayJingleInvincible()
    {
        if ((gm_sound_flag & 4U) != 0U)
        {
            return;
        }

        if (!GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            PauseStageBGM(0);
        }

        PlayBGMJingle(4U, 0);
        if ((gm_sound_flag & 32U) != 0U)
        {
            gmSoundSetBGMFadeEnd(gm_sound_jingle_bgm_scb);
            GsSound.ScbSetVolume(gm_sound_jingle_bgm_scb, 0f);
            GsSound.ScbSetSeqMute(gm_sound_jingle_bgm_scb, true);
        }

        gm_sound_flag |= 4U;
        gm_sound_flag &= 4294967294U;
    }

    // Token: 0x0600083C RID: 2108 RVA: 0x000480AC File Offset: 0x000462AC
    public static void StopJingleInvincible()
    {
        if ((gm_sound_flag & 4U) == 0U)
        {
            return;
        }

        if (!GsSound.IsBgmPause(gm_sound_jingle_bgm_scb) && (gm_sound_flag & 32U) == 0U)
        {
            StopBGMJingle(0);
        }
        else
        {
            StopBGMJingle(0);
        }

        if ((gm_sound_flag & 2147483648U) == 0U)
        {
            ResumeStageBGM(0);
        }

        gm_sound_flag &= 4294967291U;
    }

    // Token: 0x0600083D RID: 2109 RVA: 0x00048108 File Offset: 0x00046308
    public static bool GmBGMIsAlreadyPlaying()
    {
        AMS_CRIAUDIO_INTERFACE criaudioInterface = AmCri.AudioGetGlobal();
        return criaudioInterface.players[gm_sound_bgm_scb.auply_no] != null &&
               criaudioInterface.players[gm_sound_bgm_scb.auply_no].se_name ==
               gm_sound_bgm_name_list[(int) AppMain.g_gs_main_sys_info.stage_id] &&
               !criaudioInterface.players[gm_sound_bgm_scb.auply_no].IsPaused();
    }

    // Token: 0x0600083E RID: 2110 RVA: 0x00048176 File Offset: 0x00046376
    public static void PlayJingle1UP(bool ret_last_sound)
    {
#if NICE_PHYSICS
        PlaySE("OneUp");
#else
        if (ret_last_sound)
        {
            gmSoundPlay1ShotJingle(0U, 0, 0, 0);
            return;
        }

        PlayJingle(0U, 0);
#endif
    }

    // Token: 0x0600083F RID: 211 RVA: 0x0004818C File Offset: 0x0004638C
    public static void PlayGameOver()
    {
        StopStageBGM(15);
        StopBGMJingle(15);
        if (gm_sound_1shot_tcb != null)
        {
            AppMain.mtTaskClearTcb(gm_sound_1shot_tcb);
        }

        PlayJingle(7U, 0);
    }

    // Token: 0x06000840 RID: 2112 RVA: 0x000481B4 File Offset: 0x000463B4
    public static void PlayClear()
    {
        StopStageBGM(15);
        StopBGMJingle(15);
        if (gm_sound_1shot_tcb != null)
        {
            AppMain.mtTaskClearTcb(gm_sound_1shot_tcb);
        }

        PlayJingle(1U, 0);
    }

    // Token: 0x06000841 RID: 2113 RVA: 0x000481DC File Offset: 0x000463DC
    public static void PlayClearFinal()
    {
        StopStageBGM(15);
        StopBGMJingle(15);
        if (gm_sound_1shot_tcb != null)
        {
            AppMain.mtTaskClearTcb(gm_sound_1shot_tcb);
        }

        PlayJingle(2U, 0);
    }

    // Token: 0x06000842 RID: 2114 RVA: 0x00048204 File Offset: 0x00046404
    public static void AllPause()
    {
        gm_sound_flag &= 4043309055U;
        if (!GsSound.IsBgmStop(gm_sound_jingle_scb) &&
            !GsSound.IsBgmPause(gm_sound_jingle_scb))
        {
            GsSound.PauseBgm(gm_sound_jingle_scb, 0);
            gm_sound_flag |= 67108864U;
        }

        if (!GsSound.IsBgmStop(gm_sound_jingle_bgm_scb) &&
            !GsSound.IsBgmPause(gm_sound_jingle_bgm_scb))
        {
            GsSound.PauseBgm(gm_sound_jingle_bgm_scb, 0);
            gm_sound_flag |= 33554432U;
        }

        if (!GsSound.IsBgmStop(gm_sound_bgm_scb) && !GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            GsSound.PauseBgm(gm_sound_bgm_scb, 0);
            gm_sound_flag |= 16777216U;
        }

        GsSound.StopBgm(gm_sound_bgm_sub_scb, 0);
        GsSound.PauseSe(128U);
        gm_sound_flag |= 134217728U;
    }

    // Token: 0x06000843 RID: 2115 RVA: 0x000482E0 File Offset: 0x000464E0
    public static void AllResume()
    {
        if ((gm_sound_flag & 67108864U) != 0U)
        {
            GsSound.ResumeBgm(gm_sound_jingle_scb, 0);
        }
        else if ((gm_sound_flag & 33554432U) != 0U)
        {
            GsSound.ResumeBgm(gm_sound_jingle_bgm_scb, 0);
        }
        else if ((gm_sound_flag & 16777216U) != 0U)
        {
            GsSound.ResumeBgm(gm_sound_bgm_scb, 0);
        }

        GsSound.ResumeSe(128U);
        gm_sound_flag &= 4043309055U;
    }

    // Token: 0x06000844 RID: 2116 RVA: 0x0004835C File Offset: 0x0004655C
    public static void gmSoundPlay1ShotJingle(uint jngl_idx, int jingle_fade_in_frame, int bgm_fade_out_frame,
        int bgm_fade_in_frame)
    {
        gm_sound_flag |= 2147483648U;
        if (!GsSound.IsBgmStop(gm_sound_bgm_scb) && !GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            PauseStageBGM(bgm_fade_out_frame);
        }

        if (!GsSound.IsBgmStop(gm_sound_jingle_bgm_scb) &&
            !GsSound.IsBgmPause(gm_sound_jingle_bgm_scb))
        {
            PauseBGMJingle(bgm_fade_out_frame);
        }

        if (gm_sound_1shot_tcb != null)
        {
            StopJingle(0);
        }
        else
        {
            gm_sound_1shot_tcb = AppMain.MTM_TASK_MAKE_TCB(gmSound1ShotJingleFunc,
                gmSound1ShotJingleDest, 0U, 0, 32767U, 5, () => new GMS_SOUND_1SHOT_JINGLE_WORK(),
                "GM_SOUND_1SH");
        }

        GMS_SOUND_1SHOT_JINGLE_WORK gms_SOUND_1SHOT_JINGLE_WORK =
            (GMS_SOUND_1SHOT_JINGLE_WORK) gm_sound_1shot_tcb.work;
        gms_SOUND_1SHOT_JINGLE_WORK.Clear();
        PlayJingle(jngl_idx, jingle_fade_in_frame);
        gms_SOUND_1SHOT_JINGLE_WORK.bgm_fade_in_frame = bgm_fade_in_frame;
    }

    // Token: 0x06000845 RID: 2117 RVA: 0x00048434 File Offset: 0x00046634
    public static void gmSound1ShotJingleFunc(MTS_TASK_TCB tcb)
    {
        if ((gm_sound_flag & 134217728U) != 0U)
        {
            return;
        }

        GMS_SOUND_1SHOT_JINGLE_WORK gms_SOUND_1SHOT_JINGLE_WORK =
            (GMS_SOUND_1SHOT_JINGLE_WORK) gm_sound_1shot_tcb.work;
        if (GsSound.IsBgmStop(gm_sound_jingle_scb))
        {
            StopJingle(0);
            gm_sound_flag &= 2147483647U;
            if (GsSound.IsBgmStop(gm_sound_jingle_bgm_scb) ||
                GsSound.IsBgmPause(gm_sound_jingle_bgm_scb))
            {
                ResumeBGMJingle(gms_SOUND_1SHOT_JINGLE_WORK.bgm_fade_in_frame);
            }
            else if (!GsSound.IsBgmStop(gm_sound_bgm_scb) &&
                     GsSound.IsBgmPause(gm_sound_bgm_scb))
            {
                ResumeStageBGM(gms_SOUND_1SHOT_JINGLE_WORK.bgm_fade_in_frame);
            }

            AppMain.mtTaskClearTcb(tcb);
        }
    }

    // Token: 0x06000846 RID: 2118 RVA: 0x000484CF File Offset: 0x000466CF
    public static void gmSound1ShotJingleDest(MTS_TASK_TCB tcb)
    {
        gm_sound_1shot_tcb = null;
    }

    // Token: 0x06000847 RID: 2119 RVA: 0x000484E0 File Offset: 0x000466E0
    public static void gmSoundSetBGMFade(GSS_SND_SCB snd_scb, float start_vol, float end_vol, int frame)
    {
        if (GsSound.IsBgmStop(gm_sound_bgm_scb))
        {
            return;
        }

        if (GsSound.IsBgmPause(gm_sound_bgm_scb))
        {
            return;
        }

        gmSoundSetBGMFadeEnd(snd_scb);
        if (frame <= 0)
        {
            frame = 1;
        }

        GMS_SOUND_BGM_FADE_MGR_WORK gms_SOUND_BGM_FADE_MGR_WORK;
        if (gm_sound_bgm_fade_tcb == null)
        {
            gm_sound_bgm_fade_tcb = AppMain.MTM_TASK_MAKE_TCB(gmSoundBGMFadeFunc,
                gmSoundBGMFadeDest, 0U, 0, 32767U, 5, () => new GMS_SOUND_BGM_FADE_MGR_WORK(),
                "GM_SOUND_BFADE");
            gms_SOUND_BGM_FADE_MGR_WORK = (GMS_SOUND_BGM_FADE_MGR_WORK) gm_sound_bgm_fade_tcb.work;
            gms_SOUND_BGM_FADE_MGR_WORK.Clear();
        }

        gms_SOUND_BGM_FADE_MGR_WORK = (GMS_SOUND_BGM_FADE_MGR_WORK) gm_sound_bgm_fade_tcb.work;
        gmSoundBGMFadeAttachList(gms_SOUND_BGM_FADE_MGR_WORK, new GMS_SOUND_BGM_FADE_WORK
        {
            snd_scb = snd_scb,
            start_vol = start_vol,
            end_vol = end_vol,
            frame = frame,
            fade_spd = (end_vol - start_vol) / (float) frame,
            now_vol = start_vol
        });
    }

    // Token: 0x06000848 RID: 2120 RVA: 0x000485CC File Offset: 0x000467CC
    public static void gmSoundSetBGMFadeEnd(GSS_SND_SCB snd_scb)
    {
        if (gm_sound_bgm_fade_tcb != null)
        {
            GMS_SOUND_BGM_FADE_MGR_WORK gms_SOUND_BGM_FADE_MGR_WORK =
                (GMS_SOUND_BGM_FADE_MGR_WORK) gm_sound_bgm_fade_tcb.work;
            GMS_SOUND_BGM_FADE_WORK next;
            for (GMS_SOUND_BGM_FADE_WORK gms_SOUND_BGM_FADE_WORK = gms_SOUND_BGM_FADE_MGR_WORK.head;
                gms_SOUND_BGM_FADE_WORK != null;
                gms_SOUND_BGM_FADE_WORK = next)
            {
                next = gms_SOUND_BGM_FADE_WORK.next;
                if (gms_SOUND_BGM_FADE_WORK.snd_scb == snd_scb)
                {
                    gmSoundBGMFadeDetachList(gms_SOUND_BGM_FADE_MGR_WORK, gms_SOUND_BGM_FADE_WORK);
                }
            }

            if (gms_SOUND_BGM_FADE_MGR_WORK.num <= 0)
            {
                AppMain.mtTaskClearTcb(gm_sound_bgm_fade_tcb);
            }
        }
    }

    // Token: 0x06000849 RID: 2121 RVA: 0x0004862C File Offset: 0x0004682C
    public static void gmSoundBGMFadeFunc(MTS_TASK_TCB tcb)
    {
        GMS_SOUND_BGM_FADE_MGR_WORK
            gms_SOUND_BGM_FADE_MGR_WORK = (GMS_SOUND_BGM_FADE_MGR_WORK) tcb.work;
        GMS_SOUND_BGM_FADE_WORK next;
        for (GMS_SOUND_BGM_FADE_WORK gms_SOUND_BGM_FADE_WORK = gms_SOUND_BGM_FADE_MGR_WORK.head;
            gms_SOUND_BGM_FADE_WORK != null;
            gms_SOUND_BGM_FADE_WORK = next)
        {
            next = gms_SOUND_BGM_FADE_WORK.next;
            gms_SOUND_BGM_FADE_WORK.now_vol += gms_SOUND_BGM_FADE_WORK.fade_spd;
            gms_SOUND_BGM_FADE_WORK.frame--;
            if (gms_SOUND_BGM_FADE_WORK.frame <= 0)
            {
                gms_SOUND_BGM_FADE_WORK.now_vol = gms_SOUND_BGM_FADE_WORK.end_vol;
            }

            GsSound.ScbSetVolume(gms_SOUND_BGM_FADE_WORK.snd_scb, gms_SOUND_BGM_FADE_WORK.now_vol);
            if (gms_SOUND_BGM_FADE_WORK.frame <= 0 || GsSound.IsBgmStop(gms_SOUND_BGM_FADE_WORK.snd_scb))
            {
                if (gms_SOUND_BGM_FADE_WORK.now_vol > 0f)
                {
                    GsSound.ScbSetSeqMute(gms_SOUND_BGM_FADE_WORK.snd_scb, false);
                }
                else
                {
                    GsSound.ScbSetSeqMute(gms_SOUND_BGM_FADE_WORK.snd_scb, true);
                }

                gmSoundBGMFadeDetachList(gms_SOUND_BGM_FADE_MGR_WORK, gms_SOUND_BGM_FADE_WORK);
            }
        }

        if (gms_SOUND_BGM_FADE_MGR_WORK.num <= 0)
        {
            AppMain.mtTaskClearTcb(tcb);
        }
    }

    // Token: 0x0600084A RID: 2122 RVA: 0x000486FC File Offset: 0x000468FC
    public static void gmSoundBGMFadeDest(MTS_TASK_TCB tcb)
    {
        GMS_SOUND_BGM_FADE_MGR_WORK
            gms_SOUND_BGM_FADE_MGR_WORK = (GMS_SOUND_BGM_FADE_MGR_WORK) tcb.work;
        GMS_SOUND_BGM_FADE_WORK next;
        for (GMS_SOUND_BGM_FADE_WORK gms_SOUND_BGM_FADE_WORK = gms_SOUND_BGM_FADE_MGR_WORK.head;
            gms_SOUND_BGM_FADE_WORK != null;
            gms_SOUND_BGM_FADE_WORK = next)
        {
            next = gms_SOUND_BGM_FADE_WORK.next;
            gmSoundBGMFadeDetachList(gms_SOUND_BGM_FADE_MGR_WORK, gms_SOUND_BGM_FADE_WORK);
        }

        if (gm_sound_bgm_fade_tcb == tcb)
        {
            gm_sound_bgm_fade_tcb = null;
        }
    }

    // Token: 0x0600084B RID: 2123 RVA: 0x00048744 File Offset: 0x00046944
    public static void gmSoundBGMFadeAttachList(GMS_SOUND_BGM_FADE_MGR_WORK mgr_work,
        GMS_SOUND_BGM_FADE_WORK fade_work)
    {
        if (mgr_work.tail != null)
        {
            fade_work.prev = mgr_work.tail;
            mgr_work.tail.next = fade_work;
            mgr_work.tail = fade_work;
        }
        else
        {
            mgr_work.head = fade_work;
            mgr_work.tail = fade_work;
        }

        mgr_work.num++;
    }

    // Token: 0x0600084C RID: 2124 RVA: 0x00048798 File Offset: 0x00046998
    public static void gmSoundBGMFadeDetachList(GMS_SOUND_BGM_FADE_MGR_WORK mgr_work,
        GMS_SOUND_BGM_FADE_WORK fade_work)
    {
        if (fade_work.prev != null)
        {
            fade_work.prev.next = fade_work.next;
        }
        else
        {
            mgr_work.head = fade_work.next;
        }

        if (fade_work.next != null)
        {
            fade_work.next.prev = fade_work.prev;
        }
        else
        {
            mgr_work.tail = fade_work.prev;
        }

        mgr_work.num--;
    }

    // Token: 0x0600084D RID: 2125 RVA: 0x00048804 File Offset: 0x00046A04
    public static void gmSoundBGMWinBossFunc(MTS_TASK_TCB tcb)
    {
        GMS_SOUND_BGM_WIN_BOSS_MGR_WORK gms_SOUND_BGM_WIN_BOSS_MGR_WORK =
            (GMS_SOUND_BGM_WIN_BOSS_MGR_WORK) tcb.work;
        if ((gm_sound_flag & 134217728U) != 0U)
        {
            return;
        }

        gms_SOUND_BGM_WIN_BOSS_MGR_WORK.timer--;
        if (gms_SOUND_BGM_WIN_BOSS_MGR_WORK.timer <= 0)
        {
            bool flag = false;
            bool flag2 = false;
            GsSound.StopBgm(gm_sound_bgm_sub_scb, 0);
            if (GsSound.IsBgmPause(gm_sound_bgm_scb))
            {
                flag = true;
            }

            if ((gm_sound_flag & 80U) != 0U)
            {
                flag2 = true;
            }

            if (flag || flag2)
            {
                StopStageBGM(0);
            }
            else
            {
                StopStageBGM(30);
            }

            GSS_SND_SCB gss_SND_SCB = gm_sound_bgm_scb;
            gm_sound_bgm_scb = gm_sound_bgm_sub_scb;
            gm_sound_bgm_sub_scb = gss_SND_SCB;
            GsSound.ScbSetVolume(gm_sound_bgm_scb, 1f);
            GsSound.ScbSetSeqMute(gm_sound_bgm_scb, false);
            GsSound.PlayBgm(gm_sound_bgm_scb,
                gm_sound_bgm_win_boss_name_list[AppMain.GMM_MAIN_GET_ZONE_TYPE()], 30);
            gm_sound_bgm_scb.flag |= 2147483648U;
            if (flag)
            {
                PauseStageBGM(0);
            }

            if (flag2)
            {
                gmSoundSetBGMFadeEnd(gm_sound_bgm_scb);
                GsSound.ScbSetVolume(gm_sound_bgm_scb, 0f);
                GsSound.ScbSetSeqMute(gm_sound_bgm_scb, true);
            }

            AppMain.mtTaskClearTcb(tcb);
        }
    }

    // Token: 0x0600084E RID: 2126 RVA: 0x00048915 File Offset: 0x00046B15
    public static void gmSoundBGMWinBossDest(MTS_TASK_TCB tcb)
    {
        if (tcb == gm_sound_bgm_win_boss_tcb)
        {
            gm_sound_bgm_win_boss_tcb = null;
        }
    }
}

public class GMS_SOUND_BGM_WIN_BOSS_MGR_WORK
{
    // Token: 0x06002209 RID: 8713 RVA: 0x00142274 File Offset: 0x00140474
    internal void Clear()
    {
        this.timer = 0;
    }

    // Token: 0x04004FAA RID: 20394
    public int timer;
}

public class GMS_SOUND_BGM_FADE_MGR_WORK
{
    // Token: 0x06002207 RID: 8711 RVA: 0x00142255 File Offset: 0x00140455
    internal void Clear()
    {
        this.num = 0;
        this.head = null;
        this.tail = null;
    }

    // Token: 0x04004FA7 RID: 20391
    public int num;

    // Token: 0x04004FA8 RID: 20392
    public GMS_SOUND_BGM_FADE_WORK head;

    // Token: 0x04004FA9 RID: 20393
    public GMS_SOUND_BGM_FADE_WORK tail;
}

public class GMS_SOUND_BGM_FADE_WORK
{
    // Token: 0x04004F9F RID: 20383
    public float start_vol;

    // Token: 0x04004FA0 RID: 20384
    public float end_vol;

    // Token: 0x04004FA1 RID: 20385
    public float fade_spd;

    // Token: 0x04004FA2 RID: 20386
    public float now_vol;

    // Token: 0x04004FA3 RID: 20387
    public int frame;

    // Token: 0x04004FA4 RID: 20388
    public GSS_SND_SCB snd_scb;

    // Token: 0x04004FA5 RID: 20389
    public GMS_SOUND_BGM_FADE_WORK next;

    // Token: 0x04004FA6 RID: 20390
    public GMS_SOUND_BGM_FADE_WORK prev;
}

public class GMS_SOUND_1SHOT_JINGLE_WORK
{
    // Token: 0x06002204 RID: 8708 RVA: 0x0014223C File Offset: 0x0014043C
    internal void Clear()
    {
        this.bgm_fade_in_frame = 0;
    }

    // Token: 0x04004F9E RID: 20382
    public int bgm_fade_in_frame;
}