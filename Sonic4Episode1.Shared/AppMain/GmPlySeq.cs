using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;

public partial class AppMain
{
    // Token: 0x02000199 RID: 409
    public class GMS_PLY_SEQ_STATE_DATA
    {
        // Token: 0x060021E6 RID: 8678 RVA: 0x00141ED4 File Offset: 0x001400D4
        public GMS_PLY_SEQ_STATE_DATA(uint check_attr, uint accept_attr)
        {
            this.check_attr = check_attr;
            this.accept_attr = accept_attr;
        }

        // Token: 0x04004F21 RID: 20257
        public uint check_attr;

        // Token: 0x04004F22 RID: 20258
        public uint accept_attr;
    }

    // Token: 0x06001716 RID: 5910 RVA: 0x000C94DE File Offset: 0x000C76DE
    public static void GmPlySeqSetSeqState(GMS_PLAYER_WORK ply_work)
    {
        ply_work.seq_init_tbl = AppMain.g_gm_ply_seq_init_tbl_list[(int)ply_work.char_id];
        ply_work.seq_state_data_tbl = AppMain.g_gm_ply_seq_state_data_tbl[(int)ply_work.char_id];
    }

    // Token: 0x06001717 RID: 5911 RVA: 0x000C9504 File Offset: 0x000C7704
    public static void GmPlySeqMain(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        AppMain.GMS_PLY_SEQ_STATE_DATA[] seq_state_data_tbl = ply_work.seq_state_data_tbl;
        if (ply_work.no_spddown_timer != 0)
        {
            ply_work.no_spddown_timer = AppMain.ObjTimeCountDown(ply_work.no_spddown_timer);
        }
        if (ply_work.maxdash_timer != 0)
        {
            ply_work.maxdash_timer = AppMain.ObjTimeCountDown(ply_work.maxdash_timer);
        }
        if ((ply_work.player_flag & 1048576U) != 0U)
        {
            AppMain.gmPlySeqActGoal(ply_work);
        }
        if ((ply_work.player_flag & 2097152U) != 0U)
        {
            AppMain.gmPlySeqBossGoalPre(ply_work);
        }
        if ((ply_work.player_flag & 1073741824U) != 0U)
        {
            AppMain.gmPlySeqBoss5DemoPre(ply_work);
        }
        if (AppMain.GSM_MAIN_STAGE_IS_SPSTAGE_NOT_RETRY())
        {
            AppMain.gmPlySeqSplStgRollCtrl(ply_work);
        }
        AppMain.gmPlySeqCheckChangeSequence(ply_work);
        if (ply_work.seq_func != null)
        {
            ply_work.seq_func(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 8388608U) != 0U)
        {
            GmPlayer.AnimeSpeedSetWalk(ply_work, ply_work.obj_work.spd_m);
        }
        else if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 4194304U) == 0U)
        {
            ply_work.obj_work.obj_3d.speed[0] = 1f;
            ply_work.obj_work.obj_3d.speed[1] = 1f;
        }
        if ((ply_work.player_flag & 16U) != 0U)
        {
            int num = (int)ply_work.pgm_turn_dir;
            if (ply_work.pgm_turn_dir_tbl != null)
            {
                num = (int)ply_work.pgm_turn_dir_tbl[ply_work.pgm_turn_tbl_cnt];
                ply_work.pgm_turn_tbl_cnt++;
                if (ply_work.pgm_turn_tbl_cnt >= ply_work.pgm_turn_tbl_num)
                {
                    ply_work.pgm_turn_tbl_cnt = ply_work.pgm_turn_tbl_num - 1;
                    ply_work.player_flag &= 4294967279U;
                    if ((ply_work.player_flag & 256U) == 0U)
                    {
                        num = 0;
                    }
                }
            }
            else if ((ply_work.obj_work.disp_flag & 1U) != 0U)
            {
                num -= (int)ply_work.pgm_turn_spd;
                if (num <= 0)
                {
                    num = 0;
                    ply_work.player_flag &= 4294967279U;
                }
            }
            else
            {
                num += (int)ply_work.pgm_turn_spd;
                if (num >= 65536)
                {
                    num = 0;
                    ply_work.player_flag &= 4294967279U;
                }
            }
            ply_work.pgm_turn_dir = (ushort)num;
            if ((ply_work.player_flag & 2147483648U) != 0U && (ply_work.player_flag & 16U) == 0U)
            {
                GmPlayer.ActionChange(ply_work, ply_work.fall_act_state);
                ply_work.obj_work.disp_flag |= 4U;
                ply_work.player_flag &= 2147483647U;
            }
        }

        BenchmarkObject.speed = Math.Max(Math.Abs(ply_work.obj_work.spd.y), Math.Abs(ply_work.obj_work.spd_m));
    }

    // Token: 0x06001718 RID: 5912 RVA: 0x000C973B File Offset: 0x000C793B
    public static bool GmPlySeqChangeSequence(GMS_PLAYER_WORK ply_work, int seq_state)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, seq_state);
        if (ply_work.seq_init_tbl[seq_state] != null)
        {
            ply_work.seq_init_tbl[seq_state](ply_work);
            return true;
        }
        return false;
    }

    // Token: 0x06001719 RID: 5913 RVA: 0x000C9760 File Offset: 0x000C7960
    public static void GmPlySeqChangeSequenceState(GMS_PLAYER_WORK ply_work, int seq_state)
    {
        if (ply_work.gmk_obj != null)
        {
            GmPlayer.StateGimmickInit(ply_work);
        }
        ply_work.prev_seq_state = ply_work.seq_state;
        ply_work.seq_state = seq_state;
        ply_work.rect_work[1].flag &= 4294967291U;
        if ((ply_work.player_flag & 256U) != 0U)
        {
            GmPlayer.SetReverseOnlyState(ply_work);
        }
        if ((ply_work.player_flag & 2147483648U) != 0U)
        {
            ply_work.player_flag &= 2147483375U;
            ply_work.pgm_turn_dir_tbl = null;
            ply_work.pgm_turn_dir = 0;
            ply_work.pgm_turn_spd = 0;
        }
    }

    // Token: 0x0600171A RID: 5914 RVA: 0x000C97F0 File Offset: 0x000C79F0
    public static void GmPlySeqSetProgramTurn(GMS_PLAYER_WORK ply_work, ushort turn_spd)
    {
        if ((ply_work.player_flag & 16U) == 0U)
        {
            ply_work.pgm_turn_dir = 0;
        }
        GmPlayer.SetReverse(ply_work);
        ply_work.player_flag |= 16U;
        ply_work.pgm_turn_spd = turn_spd;
        ply_work.pgm_turn_dir += 32768;
        ply_work.pgm_turn_dir_tbl = null;
    }

    // Token: 0x0600171B RID: 5915 RVA: 0x000C9848 File Offset: 0x000C7A48
    public static void GmPlySeqSetProgramTurnTbl(GMS_PLAYER_WORK ply_work, ushort[] turn_tbl, int tbl_num, bool rev_depend_mtn)
    {
        if ((ply_work.player_flag & 16U) == 0U)
        {
            ply_work.pgm_turn_dir = 0;
        }
        if (!rev_depend_mtn)
        {
            GmPlayer.SetReverse(ply_work);
        }
        else
        {
            ply_work.player_flag |= 256U;
        }
        ply_work.player_flag |= 16U;
        ply_work.pgm_turn_dir_tbl = turn_tbl;
        ply_work.pgm_turn_tbl_num = tbl_num;
        ply_work.pgm_turn_tbl_cnt = 0;
    }

    // Token: 0x0600171C RID: 5916 RVA: 0x000C98A8 File Offset: 0x000C7AA8
    public static void GmPlySeqSetProgramTurnFwTurn(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.disp_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqSetProgramTurnTbl(ply_work, AppMain.gm_ply_seq_turn_l_dir_tbl, 10, true);
            return;
        }
        AppMain.GmPlySeqSetProgramTurnTbl(ply_work, AppMain.gm_ply_seq_turn_dir_tbl, 10, true);
    }

    // Token: 0x0600171D RID: 5917 RVA: 0x000C98D8 File Offset: 0x000C7AD8
    public static void GmPlySeqSetFallTurn(GMS_PLAYER_WORK ply_work)
    {
        int num = 0;
        if ((ply_work.player_flag & 2147483648U) != 0U)
        {
            num = ply_work.pgm_turn_tbl_cnt;
        }
        else
        {
            ply_work.fall_act_state = ply_work.act_state;
        }
        if ((ply_work.obj_work.disp_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqSetProgramTurnTbl(ply_work, AppMain.gm_ply_seq_fall_turn_l_dir_tbl, 10, false);
        }
        else
        {
            AppMain.GmPlySeqSetProgramTurnTbl(ply_work, AppMain.gm_ply_seq_fall_turn_dir_tbl, 10, false);
        }
        ply_work.player_flag |= 2147483648U;
        if (ply_work.act_state == 42 || ply_work.act_state == 43)
        {
            GmPlayer.ActionChange(ply_work, 43);
        }
        else
        {
            GmPlayer.ActionChange(ply_work, 41);
        }
        if (num != 0)
        {
            ply_work.pgm_turn_tbl_cnt = 10 - num;
            ply_work.obj_work.obj_3d.frame[0] = (float)ply_work.pgm_turn_tbl_cnt;
        }
    }

    // Token: 0x0600171E RID: 5918 RVA: 0x000C9995 File Offset: 0x000C7B95
    public static void GmPlySeqChangeFw(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 0);
    }

    // Token: 0x0600171F RID: 5919 RVA: 0x000C99A0 File Offset: 0x000C7BA0
    public static void GmPlySeqInitFw(GMS_PLAYER_WORK ply_work)
    {
        if (!AppMain.GSM_MAIN_STAGE_IS_SPSTAGE())
        {
            if (ply_work.obj_work.spd_m != 0)
            {
                if (ply_work.prev_seq_state == 2)
                {
                    GmPlayer.ActionChange(ply_work, 0);
                }
                AppMain.GmPlySeqChangeSequence(ply_work, 1);
                return;
            }
            if ((ply_work.player_flag & 131072U) == 0U)
            {
                GmPlayer.ActionChange(ply_work, 0);
            }
            else
            {
                AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
            }
        }
        else
        {
            AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
        }
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.seq_func = AppMain.gmPlySeqFwMain;
    }

    // Token: 0x06001720 RID: 5920 RVA: 0x000C9A54 File Offset: 0x000C7C54
    public static void gmPlySeqFwMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state == 0)
        {
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                int user_work = (int)(ply_work.obj_work.user_work + 1U);
                ply_work.obj_work.user_work = (uint)user_work;
                if (ply_work.obj_work.user_work >= 8U)
                {
                    if ((ply_work.player_flag & 16384U) != 0U)
                    {
                        GmPlayer.ActionChange(ply_work, 4);
                    }
                    else
                    {
                        GmPlayer.ActionChange(ply_work, 2);
                    }
                    if ((ply_work.obj_work.disp_flag & 1U) != 0U)
                    {
                        AppMain.GmPlySeqSetProgramTurn(ply_work, 4096);
                    }
                    ply_work.obj_work.user_work = 0U;
                    return;
                }
            }
        }
        else if (ply_work.act_state == 2 || ply_work.act_state == 4 || ply_work.act_state == 6)
        {
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, ply_work.act_state + 1);
                ply_work.obj_work.disp_flag |= 4U;
                ply_work.obj_work.user_work = 0U;
                return;
            }
        }
        else if (ply_work.act_state == 3)
        {
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                int user_work2 = (int)(ply_work.obj_work.user_work + 1U);
                ply_work.obj_work.user_work = (uint)user_work2;
                if (ply_work.obj_work.user_work >= 10U)
                {
                    GmPlayer.ActionChange(ply_work, 4);
                    ply_work.obj_work.user_work = 0U;
                    return;
                }
            }
        }
        else if (ply_work.act_state == 5 && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            int user_work3 = (int)(ply_work.obj_work.user_work + 1U);
            ply_work.obj_work.user_work = (uint)user_work3;
            if (ply_work.obj_work.user_work >= 3U && (ply_work.player_flag & 16384U) == 0U)
            {
                GmPlayer.ActionChange(ply_work, 6);
                ply_work.obj_work.user_work = 0U;
            }
        }
    }

    // Token: 0x06001721 RID: 5921 RVA: 0x000C9C04 File Offset: 0x000C7E04
    public static void GmPlySeqInitWalk(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 131072U) == 0U)
        {
            GmPlayer.WalkActionSet(ply_work);
        }
        else
        {
            AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqWalkMain;
        ply_work.obj_work.user_timer = 0;
    }

    // Token: 0x06001722 RID: 5922 RVA: 0x000C9C60 File Offset: 0x000C7E60
    public static void gmPlySeqWalkMain(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.spd_m > 0 && GmPlayer.KeyCheckWalkRight(ply_work) && (ply_work.obj_work.disp_flag & 1U) != 0U) || (ply_work.obj_work.spd_m < 0 && GmPlayer.KeyCheckWalkLeft(ply_work) && (ply_work.obj_work.disp_flag & 1U) == 0U))
        {
            AppMain.GmPlySeqSetProgramTurn(ply_work, 4096);
        }
        GmPlayer.WalkActionCheck(ply_work);
        if ((ply_work.obj_work.user_timer & 63) == 1 && ply_work.obj_work.ride_obj == null)
        {
            AppMain.GmPlyEfctCreateFootSmoke(ply_work);
        }
        ply_work.obj_work.user_timer++;
    }

    // Token: 0x06001723 RID: 5923 RVA: 0x000C9D00 File Offset: 0x000C7F00
    public static void GmPlySeqInitTurn(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.prev_seq_state == 2)
        {
            ply_work.player_flag &= 2147483375U;
            GmPlayer.SetReverse(ply_work);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        if (23 <= ply_work.act_state && ply_work.act_state <= 25)
        {
            GmPlayer.ActionChange(ply_work, 10);
        }
        else if (20 <= ply_work.act_state && ply_work.act_state <= 22)
        {
            GmPlayer.ActionChange(ply_work, 9);
        }
        else
        {
            GmPlayer.ActionChange(ply_work, 8);
            AppMain.GmPlySeqSetProgramTurnFwTurn(ply_work);
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqTurnMain;
    }

    // Token: 0x06001724 RID: 5924 RVA: 0x000C9DA6 File Offset: 0x000C7FA6
    public static void gmPlySeqTurnMain(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            GmPlayer.SetReverseOnlyState(ply_work);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x06001725 RID: 5925 RVA: 0x000C9DC5 File Offset: 0x000C7FC5
    public static void GmPlySeqInitLookupStart(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 11);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqLookupMain;
    }

    // Token: 0x06001726 RID: 5926 RVA: 0x000C9DF8 File Offset: 0x000C7FF8
    public static void GmPlySeqInitLookupMiddle(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 12);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqLookupMain;
    }

    // Token: 0x06001727 RID: 5927 RVA: 0x000C9E46 File Offset: 0x000C8046
    public static void GmPlySeqInitLookupEnd(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 13);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqLookupEndMain;
    }

    // Token: 0x06001728 RID: 5928 RVA: 0x000C9E78 File Offset: 0x000C8078
    public static void gmPlySeqLookupMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd_m != 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 1);
            return;
        }
        int act_state = ply_work.act_state;
        if (act_state != 11)
        {
            return;
        }
        if ((ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 4);
        }
    }

    // Token: 0x06001729 RID: 5929 RVA: 0x000C9EBF File Offset: 0x000C80BF
    public static void gmPlySeqLookupEndMain(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x0600172A RID: 5930 RVA: 0x000C9ED8 File Offset: 0x000C80D8
    public static void GmPlySeqInitSquatStart(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 14);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqSquatMain;
    }

    // Token: 0x0600172B RID: 5931 RVA: 0x000C9F08 File Offset: 0x000C8108
    public static void GmPlySeqInitSquatMiddle(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 15);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 4096)
        {
            ply_work.obj_work.spd_m = 0;
        }
        ply_work.seq_func = AppMain.gmPlySeqSquatMain;
    }

    // Token: 0x0600172C RID: 5932 RVA: 0x000C9F79 File Offset: 0x000C8179
    public static void GmPlySeqInitSquatEnd(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 16);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqSquatEndMain;
    }

    // Token: 0x0600172D RID: 5933 RVA: 0x000C9FAC File Offset: 0x000C81AC
    public static void gmPlySeqSquatMain(GMS_PLAYER_WORK ply_work)
    {
        int act_state = ply_work.act_state;
        if (act_state == 14 && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 7);
        }
        if (ply_work.seq_state == 7 && ply_work.obj_work.spd_m != 0)
        {
            ply_work.obj_work.move_flag |= 16384U;
            AppMain.GmPlySeqChangeSequence(ply_work, 10);
        }
    }

    // Token: 0x0600172E RID: 5934 RVA: 0x000CA012 File Offset: 0x000C8212
    public static void gmPlySeqSquatEndMain(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x0600172F RID: 5935 RVA: 0x000CA02C File Offset: 0x000C822C
    public static void GmPlySeqInitBrake(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 23);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqBrakeMain;
        GmSound.PlaySE("Brake");
        AppMain.GmPlyEfctCreateBrakeImpact(ply_work);
        AppMain.GmPlyEfctCreateBrakeDust(ply_work);
    }

    // Token: 0x06001730 RID: 5936 RVA: 0x000CA080 File Offset: 0x000C8280
    public static void gmPlySeqBrakeMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state != 25 && (((ply_work.obj_work.disp_flag & 1U) != 0U && !GmPlayer.KeyCheckWalkRight(ply_work)) || ((ply_work.obj_work.disp_flag & 1U) == 0U && !GmPlayer.KeyCheckWalkLeft(ply_work))))
        {
            GmPlayer.ActionChange(ply_work, 25);
        }
        switch (ply_work.act_state)
        {
            case 23:
                if ((ply_work.obj_work.disp_flag & 8U) != 0U)
                {
                    GmPlayer.ActionChange(ply_work, 24);
                    ply_work.obj_work.disp_flag |= 4U;
                    return;
                }
                break;
            case 24:
                if ((ply_work.obj_work.spd_m <= 0 || !GmPlayer.KeyCheckWalkLeft(ply_work)) && (ply_work.obj_work.spd_m >= 0 || !GmPlayer.KeyCheckWalkRight(ply_work)))
                {
                    AppMain.GmPlySeqChangeSequence(ply_work, 2);
                    return;
                }
                break;
            case 25:
                if ((ply_work.obj_work.disp_flag & 8U) != 0U)
                {
                    if (ply_work.obj_work.spd_m != 0)
                    {
                        AppMain.GmPlySeqChangeSequence(ply_work, 1);
                        return;
                    }
                    AppMain.GmPlySeqChangeSequence(ply_work, 0);
                }
                break;
            default:
                return;
        }
    }

    // Token: 0x06001731 RID: 5937 RVA: 0x000CA178 File Offset: 0x000C8378
    public static void GmPlySeqInitSpin(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 27);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqSpinMain;
        GmPlayer.SetAtk(ply_work);
        if (ply_work.prev_seq_state != 37 && (ply_work.player_flag & 131072U) == 0U)
        {
            GmSound.PlaySE("Spin");
        }
        AppMain.GmPlyEfctCreateSpinDashDust(ply_work);
        AppMain.GmPlyEfctCreateSuperAuraSpin(ply_work);
        AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 1U);
        AppMain.GmPlyEfctCreateSpinDashCircleBlur(ply_work);
        AppMain.GmPlyEfctCreateTrail(ply_work, 1);
    }

    // Token: 0x06001732 RID: 5938 RVA: 0x000CA210 File Offset: 0x000C8410
    public static void gmPlySeqSpinMain(GMS_PLAYER_WORK ply_work)
    {
    }

    // Token: 0x06001733 RID: 5939 RVA: 0x000CA214 File Offset: 0x000C8414
    public static void GmPlySeqInitSpinDashAcc(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state != 29 && ply_work.act_state != 30 && ply_work.act_state != 28)
        {
            AppMain.GmPlyEfctCreateSpinStartBlur(ply_work);
        }
        if (ply_work.efct_spin_start_blur != null)
        {
            GmPlayer.ActionChange(ply_work, 28);
        }
        else
        {
            GmPlayer.ActionChange(ply_work, 29);
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        if (ply_work.dash_power != 0)
        {
            ply_work.dash_power = AppMain.ObjSpdUpSet(ply_work.dash_power, ply_work.spd_add_spin, ply_work.spd_max_spin);
        }
        else
        {
            ply_work.dash_power = ply_work.spd_spin;
        }
        ply_work.seq_func = AppMain.gmPlySeqSpinDashMain;
        GmPlayer.SetAtk(ply_work);
        if (ply_work.spin_se_timer <= 0)
        {
            GmSound.PlaySE("Dash1");
            ply_work.spin_se_timer = 25;
        }
        if (ply_work.spin_back_se_timer <= 0)
        {
            GmSound.PlaySE("Dash2");
            ply_work.spin_se_timer = 50;
        }
        if (ply_work.prev_seq_state != 11)
        {
            AppMain.GmPlyEfctCreateSpinAddDust(ply_work);
        }
    }

    // Token: 0x06001734 RID: 5940 RVA: 0x000CA308 File Offset: 0x000C8508
    public static void GmPlySeqInitSpinDash(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state != 29 && ply_work.act_state != 30 && ply_work.act_state != 28)
        {
            AppMain.GmPlyEfctCreateSpinStartBlur(ply_work);
        }
        GmPlayer.ActionChange(ply_work, 30);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqSpinDashMain;
        GmPlayer.SetAtk(ply_work);
        AppMain.GmPlyEfctCreateSpinDust(ply_work);
    }

    // Token: 0x06001735 RID: 5941 RVA: 0x000CA388 File Offset: 0x000C8588
    public static void gmPlySeqSpinDashMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state == 28 && ply_work.efct_spin_start_blur == null)
        {
            if (ply_work.seq_state == 11)
            {
                AppMain.GmPlySeqChangeSequence(ply_work, 12);
                return;
            }
            GmPlayer.ActionChange(ply_work, 30);
        }
        if (ply_work.act_state == 29 && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 12);
            return;
        }
        if ((ply_work.key_on & 2) == 0 || ply_work.obj_work.spd_m != 0)
        {
            ply_work.no_spddown_timer = 72;
            ply_work.camera_stop_timer = 32768;
            int num = 48128 + AppMain.FX_Mul(ply_work.dash_power, 512);
            if ((ply_work.obj_work.disp_flag & 1U) != 0U)
            {
                num = -num;
            }
            if (AppMain.MTM_MATH_ABS(num) > AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m))
            {
                ply_work.obj_work.spd_m = num;
            }
            ply_work.dash_power = 0;
            ply_work.obj_work.move_flag |= 16384U;
            AppMain.GmPlySeqChangeSequence(ply_work, 10);
            AppMain.GmPlyEfctCreateSpinDashImpact(ply_work);
            AppMain.GMM_PAD_VIB_SMALL();
            return;
        }
        if ((ply_work.key_on & 1) != 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x06001736 RID: 5942 RVA: 0x000CA4A8 File Offset: 0x000C86A8
    public static void GmPlySeqInitStaggerFront(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 33);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqStaggerMain;
        AppMain.GmPlyEfctCreateSweat(ply_work);
    }

    // Token: 0x06001737 RID: 5943 RVA: 0x000CA4FC File Offset: 0x000C86FC
    public static void GmPlySeqInitStaggerBack(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 34);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqStaggerMain;
        AppMain.GmPlyEfctCreateSweat(ply_work);
    }

    // Token: 0x06001738 RID: 5944 RVA: 0x000CA550 File Offset: 0x000C8750
    public static void GmPlySeqInitStaggerDanger(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 35);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqStaggerMain;
        AppMain.GmPlyEfctCreateSweat(ply_work);
    }

    // Token: 0x06001739 RID: 5945 RVA: 0x000CA5A4 File Offset: 0x000C87A4
    public static void gmPlySeqStaggerMain(GMS_PLAYER_WORK ply_work)
    {
    }

    // Token: 0x0600173A RID: 5946 RVA: 0x000CA5A8 File Offset: 0x000C87A8
    public static void GmPlySeqInitFall(GMS_PLAYER_WORK ply_work)
    {
        if (!AppMain.GSM_MAIN_STAGE_IS_SPSTAGE_NOT_RETRY() && ((ply_work.player_flag & 16384U) == 0U || (ply_work.act_state != 21 && ply_work.act_state != 22)) && (ply_work.player_flag & 131072U) == 0U && ply_work.prev_seq_state != 40)
        {
            if (ply_work.obj_work.dir.z - 8192 <= 49152)
            {
                GmPlayer.ActionChange(ply_work, 42);
            }
            else
            {
                GmPlayer.ActionChange(ply_work, 40);
            }
        }
        AppMain.GmPlySeqInitFallState(ply_work);
    }

    // Token: 0x0600173B RID: 5947 RVA: 0x000CA62C File Offset: 0x000C882C
    public static void GmPlySeqInitFallState(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 32912U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.seq_func = AppMain.gmPlySeqJumpMain;
        ply_work.obj_work.spd.x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)(ply_work.obj_work.dir.z - (AppMain.g_gm_main_system.pseudofall_dir - ply_work.prev_dir_fall2))));
        ply_work.obj_work.spd.y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)(ply_work.obj_work.dir.z - (AppMain.g_gm_main_system.pseudofall_dir - ply_work.prev_dir_fall2))));
        ply_work.obj_work.spd_m = 0;
        ply_work.player_flag &= 4294967280U;
        ply_work.player_flag |= 1U;
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.timer = 0;
    }

    // Token: 0x0600173C RID: 5948 RVA: 0x000CA760 File Offset: 0x000C8960
    public static void GmPlySeqInitJump(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 131072U) == 0U)
        {
            GmPlayer.ActionChange(ply_work, 39);
        }
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 32784U;
        ply_work.obj_work.move_flag &= 4290772990U;
        ushort num = ply_work.obj_work.dir.z;
        if ((num + 256 & 8192) != 0 && (num + 256 & 4095) <= 1024)
        {
            if (ply_work.obj_work.spd_m > 0 && num < 32768)
            {
                num -= 1152;
            }
            else if (ply_work.obj_work.spd_m < 0 && num > 32768)
            {
                num += 1152;
            }
        }
        ply_work.obj_work.spd.x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)num));
        ply_work.obj_work.spd.y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)num));
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.spd.x = obj_work.spd.x + AppMain.FX_Mul(ply_work.spd_jump, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
        OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
        obj_work2.spd.y = obj_work2.spd.y + AppMain.FX_Mul(-ply_work.spd_jump, AppMain.mtMathCos((int)ply_work.obj_work.dir.z));
        if ((ply_work.gmk_flag & 4096U) != 0U)
        {
            ply_work.obj_work.spd.z = ply_work.obj_work.spd.y;
            ply_work.obj_work.spd.y = 0;
            if (ply_work.obj_work.pos.z < 0)
            {
                ply_work.obj_work.spd.z = -ply_work.obj_work.spd.z;
            }
        }
        ply_work.player_flag &= 4294967280U;
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.timer = 0;
        AppMain.GmPlySeqSetJumpState(ply_work, 0, 0U);
        if (ply_work.prev_seq_state == 10 && ply_work.no_spddown_timer >= 20)
        {
            ply_work.no_spddown_timer = 20;
        }
        GmPlayer.SetAtk(ply_work);
        if (AppMain.gm_ply_seq_jump_call_se_jump)
        {
            GmSound.PlaySE("Jump");
        }
        AppMain.GmPlyEfctCreateJumpDust(ply_work);
        AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
    }

    // Token: 0x0600173D RID: 5949 RVA: 0x000CA9DC File Offset: 0x000C8BDC
    public static void GmPlySeqSetJumpState(GMS_PLAYER_WORK ply_work, int nofall_timer, uint flag)
    {
        ply_work.obj_work.user_timer = nofall_timer;
        if (ply_work.no_jump_move_timer == 0)
        {
            ply_work.player_flag &= 4294967263U;
        }
        ply_work.player_flag &= 4294967152U;
        if ((flag & 1U) != 0U)
        {
            ply_work.player_flag |= 1U;
        }
        if ((flag & 2U) != 0U)
        {
            ply_work.player_flag |= 2U;
        }
        if ((flag & 4U) != 0U)
        {
            ply_work.player_flag |= 128U;
        }
        if ((flag & 8U) != 0U)
        {
            ply_work.player_flag |= 32U;
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            ply_work.seq_func = AppMain.gmPlySeqTruckJumpMain;
            return;
        }
        ply_work.seq_func = AppMain.gmPlySeqJumpMain;
    }

    // Token: 0x0600173E RID: 5950 RVA: 0x000CAAA4 File Offset: 0x000C8CA4
    public static void GmPlySeqInitJumpEX(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        AppMain.GmPlySeqInitJump(ply_work);
        ply_work.obj_work.spd.x = spd_x;
        ply_work.obj_work.spd.y = spd_y;
        ply_work.obj_work.spd_m = 0;
        if (ply_work.obj_work.spd.x < 0)
        {
            if (ply_work.obj_work.spd_m > 0)
            {
                ply_work.obj_work.spd_m = 0;
            }
            if ((ply_work.obj_work.disp_flag & 1U) == 0U)
            {
                GmPlayer.SetReverse(ply_work);
                return;
            }
        }
        else
        {
            if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.obj_work.spd_m = 0;
            }
            if ((ply_work.obj_work.disp_flag & 1U) != 0U)
            {
                GmPlayer.SetReverse(ply_work);
            }
        }
    }

    // Token: 0x0600173F RID: 5951 RVA: 0x000CAB58 File Offset: 0x000C8D58
    public static void GmPlySeqAtkReactionInit(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.seq_state == 19)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 20);
            return;
        }
        if ((ply_work.obj_work.move_flag & 16U) != 0U)
        {
            int x = ply_work.obj_work.spd.x;
            int spd_m = ply_work.obj_work.spd_m;
            GmPlayer.StateInit(ply_work);
            AppMain.gm_ply_seq_jump_call_se_jump = false;
            AppMain.GmPlySeqChangeSequence(ply_work, 17);
            AppMain.gm_ply_seq_jump_call_se_jump = true;
            AppMain.GmPlySeqSetJumpState(ply_work, 0, 1U);
            ply_work.obj_work.spd.y = -16384;
            ply_work.obj_work.spd.x = x;
            ply_work.obj_work.spd_m = spd_m;
        }
    }

    // Token: 0x06001740 RID: 5952 RVA: 0x000CABFB File Offset: 0x000C8DFB
    public static void GmPlySeqAtkReactionSpdInit(GMS_PLAYER_WORK ply_work, int spd_x, int no_spddown_timer)
    {
        ply_work.obj_work.spd.x = spd_x;
        ply_work.no_spddown_timer = no_spddown_timer;
        AppMain.GmPlySeqAtkReactionInit(ply_work);
    }

    // Token: 0x06001741 RID: 5953 RVA: 0x000CAC1C File Offset: 0x000C8E1C
    public static void gmPlySeqJumpMain(GMS_PLAYER_WORK ply_work)
    {
        int num = ply_work.obj_work.spd.y;
        if ((ply_work.gmk_flag & 4096U) != 0U)
        {
            num = ply_work.obj_work.spd.z;
            if (ply_work.obj_work.dir.x > 32768)
            {
                num = -num;
            }
        }
        if (ply_work.obj_work.user_timer != 0)
        {
            ply_work.obj_work.user_timer--;
            if (ply_work.obj_work.user_timer == 0)
            {
                ply_work.obj_work.move_flag |= 128U;
            }
        }
        if ((ply_work.player_flag & 5U) == 0U && !GmPlayer.KeyCheckJumpKeyOn(ply_work) && num < -16384)
        {
            ply_work.player_flag |= 4U;
        }
        if ((ply_work.player_flag & 4U) != 0U && ply_work.obj_work.spd.y < 0)
        {
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.spd.y = obj_work.spd.y + ply_work.obj_work.spd_fall;
        }
        int act_state = ply_work.act_state;
        if (act_state != 39)
        {
            switch (act_state)
            {
                case 44:
                    if (num > 1024)
                    {
                        GmPlayer.ActionChange(ply_work, 45);
                    }
                    break;
                case 45:
                    if ((ply_work.obj_work.disp_flag & 8U) != 0U)
                    {
                        GmPlayer.ActionChange(ply_work, 46);
                        ply_work.obj_work.disp_flag |= 4U;
                    }
                    break;
                case 47:
                    if ((ply_work.obj_work.disp_flag & 8U) != 0U)
                    {
                        ply_work.obj_work.disp_flag |= 1024U;
                        GmPlayer.ActionChange(ply_work, 48);
                        ply_work.obj_work.disp_flag |= 4U;
                    }
                    break;
            }
        }
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x06001742 RID: 5954 RVA: 0x000CADE5 File Offset: 0x000C8FE5
    public static void GmPlySeqInitWallPush(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 17);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqWallPushMain;
    }

    // Token: 0x06001743 RID: 5955 RVA: 0x000CAE15 File Offset: 0x000C9015
    public static void gmPlySeqWallPushMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state == 17 && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            GmPlayer.ActionChange(ply_work, 18);
            ply_work.obj_work.disp_flag |= 4U;
        }
    }

    // Token: 0x06001744 RID: 5956 RVA: 0x000CAE4C File Offset: 0x000C904C
    public static void GmPlySeqInitHoming(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.enemy_obj == null)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 21);
            return;
        }
        if ((ply_work.player_flag & 131072U) == 0U)
        {
            GmPlayer.ActionChange(ply_work, 31);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag |= 32784U;
        ply_work.obj_work.move_flag &= 4294967166U;
        ply_work.player_flag |= 128U;
        ply_work.obj_work.dir.z = 0;
        ply_work.gmk_flag &= 4261410812U;
        ply_work.seq_func = AppMain.gmPlySeqHomingMain;
        ply_work.obj_work.user_timer = 131072;
        ply_work.homing_timer = 98304;
        ply_work.homing_boost_timer = 262144;
        GmPlayer.SetAtk(ply_work);
        AppMain.GmPlyEfctCreateHomingImpact(ply_work);
        
        
        GmSound.PlaySE("Homing");
    }

    // Token: 0x06001745 RID: 5957 RVA: 0x000CAF45 File Offset: 0x000C9145
    public static void GmPlySeqSetNoJumpMoveTime(GMS_PLAYER_WORK ply_work, int time)
    {
        ply_work.no_jump_move_timer = time;
        ply_work.player_flag |= 32U;
    }

    // Token: 0x06001746 RID: 5958 RVA: 0x000CAF60 File Offset: 0x000C9160
    public static void gmPlySeqHomingMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.user_timer == 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
            return;
        }
        ply_work.obj_work.user_timer = AppMain.ObjTimeCountDown(ply_work.obj_work.user_timer);
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        if (ply_work.enemy_obj != null)
        {
            AppMain.OBS_RECT_WORK obs_RECT_WORK = ((AppMain.GMS_ENEMY_COM_WORK)ply_work.enemy_obj).rect_work[2];
            int x = ply_work.enemy_obj.pos.x;
            int num;
            if ((ply_work.enemy_obj.disp_flag & 2U) != 0U)
            {
                num = ply_work.enemy_obj.pos.y - ((int)(obs_RECT_WORK.rect.top + obs_RECT_WORK.rect.bottom) << 11);
            }
            else
            {
                num = ply_work.enemy_obj.pos.y + ((int)(obs_RECT_WORK.rect.top + obs_RECT_WORK.rect.bottom) << 11);
            }
            float num2 = AppMain.FXM_FX32_TO_FLOAT(x - ply_work.obj_work.pos.x);
            float num3 = AppMain.FXM_FX32_TO_FLOAT(num - ply_work.obj_work.pos.y);
            double num4 = Math.Atan2((double)num3, (double)num2);
            num4 += (int)ply_work.obj_work.dir_fall;
            ply_work.obj_work.spd.x = (int)(Math.Cos(num4) * 61440f);
            ply_work.obj_work.spd.y = (int)(Math.Sin(num4) * 61440f);
            if (ply_work.obj_work.spd.x < 0)
            {
                ply_work.obj_work.disp_flag |= 1U;
            }
            else if (ply_work.obj_work.spd.x > 0)
            {
                ply_work.obj_work.disp_flag &= 4294967294U;
            }
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) > 256 && (ply_work.obj_work.move_flag & 4U) != 0U)
            {
                AppMain.GmPlySeqLandingSet(ply_work, 0);
                AppMain.GmPlySeqChangeSequence(ply_work, 0);
            }
        }
    }

    // Token: 0x06001747 RID: 5959 RVA: 0x000CB16C File Offset: 0x000C936C
    public static void GmPlySeqInitHomingRef(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 131072U) == 0U)
        {
            GmPlayer.ActionChange(ply_work, 32);
        }
        ply_work.player_flag &= 4294967167U;
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 32912U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.seq_func = AppMain.gmPlySeqHomingRefMain;
        ply_work.obj_work.spd.x = 0;
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            ply_work.obj_work.spd.y = GmPlayer.GMD_PLAYER_WATERJUMP_GET(-20480);
        }
        else
        {
            ply_work.obj_work.spd.y = -20480;
        }
        ply_work.obj_work.spd_add.x = (ply_work.obj_work.spd_add.y = 0);
        ply_work.obj_work.spd_m = 0;
        ply_work.player_flag &= 4294967280U;
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.timer = 0;
        AppMain.GmPlyEfctCreateJumpDust(ply_work);
    }

    // Token: 0x06001748 RID: 5960 RVA: 0x000CB2A5 File Offset: 0x000C94A5
    public static void gmPlySeqHomingRefMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd.y >= 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
            return;
        }
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x06001749 RID: 5961 RVA: 0x000CB2E4 File Offset: 0x000C94E4
    public static void GmPlySeqInitJumpDash(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 131072U) == 0U)
        {
            GmPlayer.ActionChange(ply_work, 39);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag |= 32784U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.player_flag |= 160U;
        ply_work.obj_work.dir.z = 0;
        ply_work.gmk_flag &= 4261410812U;
        int ang;
        if ((ply_work.obj_work.disp_flag & 1U) != 0U)
        {
            ang = -30720;
        }
        else
        {
            ang = 63488;
        }
        if ((ply_work.player_flag & 32768U) != 0U)
        {
            ply_work.obj_work.spd.y = 0;
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.spd.x = obj_work.spd.x + (int)(4096f * AppMain.nnCos(ang));
            OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
            obj_work2.spd.y = obj_work2.spd.y + -(int)(4096f * AppMain.nnSin(ang));
            ply_work.no_spddown_timer = 8;
            ply_work.obj_work.user_timer = 20;
        }
        else
        {
            ply_work.obj_work.spd.y = 0;
            OBS_OBJECT_WORK obj_work3 = ply_work.obj_work;
            obj_work3.spd.x = obj_work3.spd.x + (int)(16384f * AppMain.nnCos(ang));
            OBS_OBJECT_WORK obj_work4 = ply_work.obj_work;
            obj_work4.spd.y = obj_work4.spd.y + -(int)(16384f * AppMain.nnSin(ang));
            ply_work.no_spddown_timer = 8;
            ply_work.obj_work.user_timer = 20;
        }
        GmPlayer.SetAtk(ply_work);
        AppMain.GmPlyEfctCreateJumpDash(ply_work);
        ply_work.seq_func = AppMain.gmPlySeqJumpDashMain;
    }

    // Token: 0x0600174A RID: 5962 RVA: 0x000CB4A4 File Offset: 0x000C96A4
    public static void gmPlySeqJumpDashMain(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            ply_work.player_flag &= 4294967263U;
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        if (ply_work.obj_work.user_timer == 0)
        {
            int x = ply_work.obj_work.spd.x;
            int y = ply_work.obj_work.spd.y;
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
            ply_work.obj_work.spd.x = x;
            ply_work.obj_work.spd.y = y;
            ply_work.player_flag &= 4294967263U;
            return;
        }
        ply_work.obj_work.user_timer--;
    }

    // Token: 0x0600174B RID: 5963 RVA: 0x000CB55C File Offset: 0x000C975C
    public static void GmPlySeqChangeDamage(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 22);
    }

    // Token: 0x0600174C RID: 5964 RVA: 0x000CB568 File Offset: 0x000C9768
    public static void GmPlySeqChangeDamageSetSpd(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 22);
        ply_work.obj_work.spd.x = spd_x;
        ply_work.obj_work.spd.y = spd_y;
        if (spd_x < 0)
        {
            ply_work.obj_work.disp_flag &= 4294967294U;
            return;
        }
        ply_work.obj_work.disp_flag |= 1U;
    }

    // Token: 0x0600174D RID: 5965 RVA: 0x000CB5CC File Offset: 0x000C97CC
    public static void GmPlySeqInitDamage(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.StateInit(ply_work);
        if ((ply_work.player_flag & 32768U) != 0U)
        {
            ply_work.obj_work.spd.x = 24576;
            ply_work.obj_work.spd.y = -12288;
            ply_work.obj_work.spd_m = 0;
            ply_work.obj_work.disp_flag &= 4294967294U;
        }
        else
        {
            ply_work.obj_work.spd.x = -6144;
            ply_work.obj_work.spd.y = -12288;
            ply_work.obj_work.spd_m = 0;
            if ((ply_work.obj_work.disp_flag & 1U) != 0U)
            {
                ply_work.obj_work.spd.x = -ply_work.obj_work.spd.x;
            }
        }
        GmPlayer.ActionChange(ply_work, 36);
        ply_work.obj_work.move_flag |= 32784U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.invincible_timer = ply_work.time_damage;
        GmPlayer.SetDefInvincible(ply_work);
        ply_work.seq_func = AppMain.gmPlySeqDamageMain;
        ply_work.obj_work.disp_flag |= 4U;
        AppMain.GMM_PAD_VIB_LARGE_TIME(60f);
    }

    // Token: 0x0600174E RID: 5966 RVA: 0x000CB713 File Offset: 0x000C9913
    public static void gmPlySeqDamageMain(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            ply_work.rect_work[0].flag &= 4294967039U;
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x0600174F RID: 5967 RVA: 0x000CB74C File Offset: 0x000C994C
    public static void GmPlySeqChangeDeath(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 23);
    }

    // Token: 0x06001750 RID: 5968 RVA: 0x000CB758 File Offset: 0x000C9958
    public static void GmPlySeqInitDeath(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        if ((ply_work.player_flag & 16777216U) != 0U)
        {
            return;
        }
        if ((ply_work.player_flag & 16384U) != 0U)
        {
            GmPlayer.SetEndSuperSonic(ply_work);
        }
        GmPlayer.StateInit(ply_work);
        ply_work.obj_work.disp_flag &= 4294967294U;
        ply_work.obj_work.move_flag |= 768U;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.y = -ply_work.spd_jump;
        ply_work.obj_work.spd_add.x = (ply_work.obj_work.spd_add.y = 0);
        ply_work.obj_work.dir.z = 0;
        ply_work.obj_work.pos.z = 983040;
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            ply_work.jump_pseudofall_dir = AppMain.g_gm_main_system.pseudofall_dir;
            ply_work.gmk_flag |= 16777216U;
            ply_work.obj_work.dir.x = (ply_work.obj_work.dir.y = 0);
            ply_work.obj_work.dir.z = 0;
            ply_work.obj_work.move_flag &= 4294967231U;
        }
        ply_work.player_flag &= 3489660927U;
        ply_work.player_flag |= 1024U;
        ply_work.obj_work.flag |= 2U;
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmSound.PlaySE("Damage3");
        }
        else
        {
            GmSound.PlaySE("Damage1");
        }
        GmPlayer.ActionChange(ply_work, 37);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqDeathMain;
        ply_work.obj_work.user_timer = 0;
        ply_work.water_timer = 0;
        AppMain.GMM_PAD_VIB_LARGE_TIME(90f);
    }

    // Token: 0x06001751 RID: 5969 RVA: 0x000CB968 File Offset: 0x000C9B68
    public static void gmPlySeqDeathMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state == 37 && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            GmPlayer.ActionChange(ply_work, 38);
            ply_work.obj_work.disp_flag |= 4U;
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.dir.z = (ushort)(obj_work.dir.z + 1024);
        }
    }

    // Token: 0x06001752 RID: 5970 RVA: 0x000CB9D4 File Offset: 0x000C9BD4
    public static void GmPlySeqChangeTransformSuper(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 24);
    }

    // Token: 0x06001753 RID: 5971 RVA: 0x000CB9E0 File Offset: 0x000C9BE0
    public static void GmPlySeqInitTransformSuper(GMS_PLAYER_WORK ply_work)
    {
        int num = 0;
        int num2 = 0;
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        if ((ply_work.player_flag & 16777216U) != 0U)
        {
            return;
        }
        ushort z = ply_work.obj_work.dir.z;
        if ((ply_work.obj_work.move_flag & 16U) == 0U)
        {
            num = AppMain.FXM_FLOAT_TO_FX32(AppMain.nnCos(81920 - (int)ply_work.obj_work.dir.z) * 3f);
            num2 = -AppMain.FXM_FLOAT_TO_FX32(AppMain.nnSin(81920 - (int)ply_work.obj_work.dir.z) * 3f);
        }
        GmPlayer.StateInit(ply_work);
        ply_work.obj_work.move_flag &= 4294967167U;
        ply_work.obj_work.flag |= 2U;
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            ply_work.obj_work.move_flag |= 8448U;
        }
        if ((ply_work.obj_work.move_flag & 16U) == 0U)
        {
            ply_work.obj_work.move_flag |= 16U;
            ply_work.obj_work.move_flag &= 4294967280U;
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.pos.x = obj_work.pos.x + num;
            OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
            obj_work2.pos.y = obj_work2.pos.y + num2;
        }
        ply_work.obj_work.spd.x = (ply_work.obj_work.spd.y = 0);
        ply_work.obj_work.spd_add.x = (ply_work.obj_work.spd_add.y = 0);
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.dir.z = 0;
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            ply_work.obj_work.dir.z = z;
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            ply_work.obj_work.pos.z = -32768;
            ply_work.gmk_flag |= 536870912U;
        }
        GmPlayer.ActionChange(ply_work, 50);
        ply_work.seq_func = AppMain.gmPlySeqTransformSuperMain;
        ply_work.obj_work.user_timer = 593920;
        ply_work.obj_work.user_work = 0U;
        AppMain.GmPlyEfctCreateSuperStart(ply_work);
    }

    // Token: 0x06001754 RID: 5972 RVA: 0x000CBC34 File Offset: 0x000C9E34
    public static void gmPlySeqTransformSuperMain(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.user_timer = AppMain.ObjTimeCountDown(ply_work.obj_work.user_timer);
        if (ply_work.act_state == 50)
        {
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, 51);
                ply_work.obj_work.disp_flag |= 4U;
            }
        }
        else if (ply_work.act_state != 52 && ((long)ply_work.obj_work.user_timer & -4096) == 286720L)
        {
            GmPlayer.ActionChange(ply_work, 52);
        }
        if (((long)ply_work.obj_work.user_timer & -4096) == 245760L && (ply_work.player_flag & 16384U) == 0U)
        {
            GMS_PLAYER_RESET_ACT_WORK reset_act_work = new GMS_PLAYER_RESET_ACT_WORK();
            ushort z = ply_work.obj_work.dir.z;
            GmPlayer.ActionChange(ply_work, 53);
            ply_work.obj_work.disp_flag |= 4U;
            GmPlayer.SaveResetAction(ply_work, reset_act_work);
            GmPlayer.SetSuperSonic(ply_work);
            GmPlayer.ResetAction(ply_work, reset_act_work);
            ply_work.obj_work.move_flag &= 4294967167U;
            ply_work.obj_work.flag |= 2U;
            if ((ply_work.player_flag & 262144U) != 0U)
            {
                ply_work.obj_work.move_flag |= 8448U;
            }
            if ((ply_work.player_flag & 262144U) != 0U)
            {
                ply_work.obj_work.dir.z = z;
            }
        }
        if (ply_work.obj_work.user_timer == 0)
        {
            ply_work.obj_work.move_flag |= 128U;
            ply_work.obj_work.flag &= 4294967293U;
            ply_work.obj_work.move_flag &= 4294958847U;
            ply_work.super_sonic_ring_timer = 245760;
            if ((ply_work.player_flag & 262144U) != 0U)
            {
                ply_work.obj_work.pos.z = 0;
                ply_work.gmk_flag &= 3758096383U;
            }
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x06001755 RID: 5973 RVA: 0x000CBE38 File Offset: 0x000CA038
    public static void GmPlySeqChangeActGoal(GMS_PLAYER_WORK ply_work)
    {
        SaveState.deleteSave();
        if ((ply_work.player_flag & 1024U) != 0U || (AppMain.g_gm_main_system.game_flag & 16384U) != 0U)
        {
            return;
        }
        uint move_flag = ply_work.obj_work.move_flag;
        GmPlayer.StateInit(ply_work);
        if (ply_work.seq_state == 11 || ply_work.seq_state == 12)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
        ply_work.obj_work.move_flag |= (move_flag & 1U);
        ply_work.obj_work.move_flag &= 4294441983U;
        ply_work.player_flag |= 22020096U;
        GmPlayer.SetDefInvincible(ply_work);
        ply_work.invincible_timer = 0;
    }

    // Token: 0x06001756 RID: 5974 RVA: 0x000CBEE4 File Offset: 0x000CA0E4
    public static void gmPlySeqActGoal(GMS_PLAYER_WORK ply_work)
    {
        if ((AppMain.g_gm_main_system.game_flag & 16384U) != 0U)
        {
            return;
        }
        ply_work.player_flag |= 4194304U;
        GmPlayer.SetDefInvincible(ply_work);
        ply_work.invincible_timer = 0;
        ply_work.water_timer = 0;
        OBS_CAMERA obs_CAMERA = ObjCamera.Get(AppMain.g_obj.glb_camera_id);
        if (AppMain.FXM_FLOAT_TO_FX32(obs_CAMERA.disp_pos.x) + (AppMain.OBD_LCD_X >> 1) + 128 > ply_work.obj_work.pos.x >> 12)
        {
            ply_work.key_on |= 8;
            ply_work.key_walk_rot_z = 32767;
        }
    }

    // Token: 0x06001757 RID: 5975 RVA: 0x000CBF88 File Offset: 0x000CA188
    public static void GmPlySeqChangeBossGoal(GMS_PLAYER_WORK ply_work, int capsule_pos_x, int capsule_pos_y)
    {
        SaveState.deleteSave();
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        GmPlayer.StateInit(ply_work);
        ply_work.player_flag |= 23068672U;
        ply_work.rect_work[0].def_power = 3;
        ply_work.gmk_work0 = capsule_pos_x;
        ply_work.gmk_work1 = capsule_pos_y;
        if (ply_work.obj_work.pos.x >= capsule_pos_x)
        {
            ply_work.gmk_work2 = 0;
        }
        else
        {
            ply_work.gmk_work2 = 1;
        }
        AppMain.GmPlySeqChangeSequence(ply_work, 0);
    }

    // Token: 0x06001758 RID: 5976 RVA: 0x000CC008 File Offset: 0x000CA208
    public static void gmPlySeqBossGoalPre(GMS_PLAYER_WORK ply_work)
    {
        ply_work.player_flag |= 4194304U;
        ply_work.rect_work[0].def_power = 3;
        ply_work.water_timer = 0;
        if ((ply_work.obj_work.move_flag & 1U) == 0U || ((ply_work.obj_work.move_flag & 1U) != 0U && ply_work.obj_work.pos.y < ply_work.gmk_work1 - 98304))
        {
            if ((ply_work.obj_work.move_flag & 1U) != 0U || AppMain.MTM_MATH_ABS(ply_work.obj_work.pos.x - ply_work.gmk_work0) <= 262144)
            {
                if (ply_work.gmk_work2 != 0)
                {
                    ply_work.key_on |= 4;
                    ply_work.key_walk_rot_z = -32767;
                    return;
                }
                ply_work.key_on |= 8;
                ply_work.key_walk_rot_z = 32767;
                return;
            }
        }
        else
        {
            ply_work.player_flag &= 4292870143U;
            AppMain.GmPlySeqChangeSequence(ply_work, 25);
        }
    }

    // Token: 0x06001759 RID: 5977 RVA: 0x000CC108 File Offset: 0x000CA308
    public static void GmPlySeqInitBossGaol(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        GmPlayer.StateInit(ply_work);
        ply_work.player_flag |= 4194304U;
        ply_work.rect_work[0].def_power = 3;
        ply_work.water_timer = 0;
        GmPlayer.ActionChange(ply_work, 0);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.user_timer = 245760;
        ply_work.seq_func = AppMain.gmPlySeqBossGoalMain;
    }

    // Token: 0x0600175A RID: 5978 RVA: 0x000CC190 File Offset: 0x000CA390
    public static void gmPlySeqBossGoalMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state == 0)
        {
            ply_work.obj_work.user_timer = AppMain.ObjTimeCountDown(ply_work.obj_work.user_timer);
            if (ply_work.obj_work.user_timer == 0)
            {
                if ((ply_work.obj_work.disp_flag & 1U) != 0U)
                {
                    AppMain.GmPlySeqSetProgramTurn(ply_work, 4096);
                }
                GmPlayer.ActionChange(ply_work, 54);
                return;
            }
        }
        else if ((ply_work.obj_work.disp_flag & 8U) != 0U && ply_work.act_state == 54)
        {
            GmPlayer.ActionChange(ply_work, 55);
            ply_work.obj_work.disp_flag |= 4U;
        }
    }

    // Token: 0x0600175B RID: 5979 RVA: 0x000CC224 File Offset: 0x000CA424
    public static void GmPlySeqChangeBoss5Demo(GMS_PLAYER_WORK ply_work, int dest_pos_x, bool is_goal)
    {
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.x = 0;
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        ply_work.player_flag |= 1077936128U;
        if (is_goal)
        {
            SaveState.deleteSave();
            ply_work.player_flag |= 16777216U;
            ply_work.rect_work[0].def_power = 3;
        }
        ply_work.gmk_work0 = dest_pos_x;
        AppMain.GmPlySeqChangeSequence(ply_work, 0);
    }

    // Token: 0x0600175C RID: 5980 RVA: 0x000CC2B0 File Offset: 0x000CA4B0
    public static void gmPlySeqBoss5DemoPre(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.pos.x >= ply_work.gmk_work0)
        {
            if (ply_work.obj_work.spd.x == 0)
            {
                ply_work.player_flag &= 3221225471U;
                AppMain.GmPlySeqChangeSequence(ply_work, 26);
                return;
            }
        }
        else
        {
            ply_work.key_on |= 8;
            ply_work.key_walk_rot_z = 32767;
        }
    }

    // Token: 0x0600175D RID: 5981 RVA: 0x000CC320 File Offset: 0x000CA520
    public static void GmPlySeqInitBoss5Demo(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        GmPlayer.StateInit(ply_work);
        ply_work.player_flag |= 4194304U;
        if (ply_work.act_state != 0)
        {
            GmPlayer.ActionChange(ply_work, 0);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.seq_func = AppMain.gmPlySeqBoss5DemoMain;
    }

    // Token: 0x0600175E RID: 5982 RVA: 0x000CC388 File Offset: 0x000CA588
    public static void gmPlySeqBoss5DemoMain(GMS_PLAYER_WORK ply_work)
    {
    }

    // Token: 0x0600175F RID: 5983 RVA: 0x000CC38A File Offset: 0x000CA58A
    public static void GmPlySeqChangeBoss5DemoEnd(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        ply_work.player_flag &= 3217031167U;
        AppMain.GmPlySeqChangeSequence(ply_work, 0);
    }

    // Token: 0x06001760 RID: 5984 RVA: 0x000CC3B5 File Offset: 0x000CA5B5
    public static void GmPlySeqChangeTRetryFw(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 27);
    }

    // Token: 0x06001761 RID: 5985 RVA: 0x000CC3C0 File Offset: 0x000CA5C0
    public static void GmPlySeqInitTRetryFw(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.player_flag & 131072U) != 0U)
        {
            GmPlayer.SetEndPinballSonic(ply_work);
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            GmPlayer.SetEndTruckRide(ply_work);
        }
        GmPlayer.SpdParameterSet(ply_work);
        ply_work.obj_work.dir.x = 0;
        ply_work.obj_work.dir.y = 0;
        ply_work.obj_work.dir.z = 0;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.spd.z = 0;
        ply_work.obj_work.disp_flag &= 4294967292U;
        GmPlayer.ActionChange(ply_work, 4);
        ply_work.player_flag &= 4293918719U;
        ply_work.obj_work.spd_m = 0;
        ply_work.water_timer = 0;
        ply_work.rect_work[0].def_power = 3;
        ply_work.invincible_timer = 0;
        ply_work.obj_work.move_flag = 16192U;
        ply_work.obj_work.move_flag &= 4294967167U;
        ply_work.obj_work.flag |= 2U;
        ply_work.obj_work.dir_fall = 0;
        ply_work.ply_pseudofall_dir = 0;
        ply_work.jump_pseudofall_dir = 0;
        AppMain.g_gm_main_system.pseudofall_dir = 0;
        ply_work.player_flag |= 4194304U;
        ply_work.player_flag &= 3220176895U;
        ply_work.obj_work.dir_slope = 192;
        ply_work.seq_func = AppMain.gmPlySeqTRetryFw;
    }

    // Token: 0x06001762 RID: 5986 RVA: 0x000CC567 File Offset: 0x000CA767
    public static void gmPlySeqTRetryFw(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            GmPlayer.ActionChange(ply_work, 5);
        }
        ply_work.water_timer = 0;
        ply_work.rect_work[0].def_power = 3;
    }

    // Token: 0x06001763 RID: 5987 RVA: 0x000CC594 File Offset: 0x000CA794
    public static void GmPlySeqChangeTRetryAcc(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 28);
    }

    // Token: 0x06001764 RID: 5988 RVA: 0x000CC59F File Offset: 0x000CA79F
    public static void GmPlySeqInitTRetryAcc(GMS_PLAYER_WORK ply_work)
    {
        ply_work.player_flag |= 512U;
        AppMain.GmPlySeqMoveWalk(ply_work);
        GmPlayer.WalkActionSet(ply_work);
        ply_work.obj_work.user_timer = 0;
        ply_work.seq_func = AppMain.gmPlySeqTRetryAcc;
    }

    // Token: 0x06001765 RID: 5989 RVA: 0x000CC5E0 File Offset: 0x000CA7E0
    public static void gmPlySeqTRetryAcc(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.user_timer++;
        ply_work.obj_work.spd_m += 512;
        if (ply_work.obj_work.user_timer > 100)
        {
            ply_work.seq_func = null;
            ply_work.obj_work.user_timer = 0;
            ply_work.obj_work.move_flag &= 4294959103U;
            ply_work.obj_work.move_flag &= 4294966271U;
            ply_work.obj_work.flag |= 16U;
        }
        if (ply_work.obj_work.spd_m > ply_work.spd4 - 512 && (ply_work.player_flag & 1048576U) == 0U)
        {
            ply_work.obj_work.dir.z = 4097;
            GmPlayer.WalkActionSet(ply_work);
            GmPlayer.WalkActionCheck(ply_work);
            ply_work.obj_work.dir.z = 0;
            AppMain.GmPlySeqChangeTRetryRun(ply_work);
        }
        ply_work.water_timer = 0;
        ply_work.rect_work[0].def_power = 3;
    }

    // Token: 0x06001766 RID: 5990 RVA: 0x000CC6EF File Offset: 0x000CA8EF
    public static void GmPlySeqChangeTRetryRun(GMS_PLAYER_WORK ply_work)
    {
        ply_work.player_flag |= 1048576U;
    }

    // Token: 0x06001767 RID: 5991 RVA: 0x000CC704 File Offset: 0x000CA904
    public static void GmPlySeqInitTruckFw(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd_m != 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 1);
            return;
        }
        if ((ply_work.obj_work.move_flag & 4194304U) == 0U)
        {
            ply_work.gmk_flag &= 4293918719U;
            GmPlayer.ActionChange(ply_work, 73);
        }
        else if (ply_work.obj_3d[(int)GmPlayer.g_gm_player_model_tbl[(int)ply_work.char_id][0]].act_id[0] != (int)GmPlayer.g_gm_player_motion_right_tbl[(int)ply_work.char_id][0] && ply_work.obj_3d[(int)GmPlayer.g_gm_player_model_tbl[(int)ply_work.char_id][73]].act_id[0] != (int)GmPlayer.g_gm_player_motion_right_tbl[(int)ply_work.char_id][73])
        {
            if ((ply_work.gmk_flag & 1048576U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, 70);
            }
            else
            {
                GmPlayer.ActionChange(ply_work, 69);
            }
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.seq_func = AppMain.gmPlySeqTruckFwMain;
    }

    // Token: 0x06001768 RID: 5992 RVA: 0x000CC824 File Offset: 0x000CAA24
    public static void gmPlySeqTruckFwMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_3d[(int)GmPlayer.g_gm_player_model_tbl[(int)ply_work.char_id][73]].act_id[0] == (int)GmPlayer.g_gm_player_motion_right_tbl[(int)ply_work.char_id][73] && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            if ((ply_work.gmk_flag & 1048576U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, 70);
            }
            else
            {
                GmPlayer.ActionChange(ply_work, 69);
            }
            ply_work.obj_work.disp_flag |= 4U;
        }
        if (ply_work.act_state == 69 || ply_work.act_state == 70)
        {
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                uint user_work = ply_work.obj_work.user_work;
                ply_work.obj_work.user_work = ply_work.obj_work.user_work + 1U;
                if (ply_work.obj_work.user_work >= 8U)
                {
                    GmPlayer.ActionChange(ply_work, 2);
                    ply_work.obj_work.user_work = 0U;
                    return;
                }
            }
        }
        else if (ply_work.act_state == 2 || ply_work.act_state == 4 || ply_work.act_state == 6)
        {
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, ply_work.act_state + 1);
                ply_work.obj_work.disp_flag |= 4U;
                ply_work.obj_work.user_work = 0U;
                return;
            }
        }
        else if (ply_work.act_state == 3 && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            int user_work2 = (int)(ply_work.obj_work.user_work + 1U);
            ply_work.obj_work.user_work = (uint)user_work2;
            if (ply_work.obj_work.user_work >= 10U)
            {
                GmPlayer.ActionChange(ply_work, 4);
                ply_work.obj_work.user_work = 0U;
            }
        }
    }

    // Token: 0x06001769 RID: 5993 RVA: 0x000CC9C0 File Offset: 0x000CABC0
    public static void GmPlySeqInitTruckWalk(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 4194304U) == 0U)
        {
            GmPlayer.ActionChange(ply_work, 73);
        }
        else if (ply_work.act_state != 71 && ply_work.act_state != 72)
        {
            if ((ply_work.gmk_flag & 1048576U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, 72);
            }
            else
            {
                GmPlayer.ActionChange(ply_work, 71);
            }
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqTruckWalkMain;
        ply_work.obj_work.user_timer = 0;
        ply_work.truck_left_flip_timer = 245760;
    }

    // Token: 0x0600176A RID: 5994 RVA: 0x000CCA70 File Offset: 0x000CAC70
    public static void gmPlySeqTruckWalkMain(GMS_PLAYER_WORK ply_work)
    {
        bool flag = false;
        if (ply_work.obj_work.spd_m < 0 && ply_work.act_state == 71)
        {
            ply_work.truck_left_flip_timer = AppMain.ObjTimeCountDown(ply_work.truck_left_flip_timer);
            if (ply_work.truck_left_flip_timer == 0)
            {
                ply_work.gmk_flag |= 1048576U;
                flag = true;
            }
        }
        else
        {
            ply_work.truck_left_flip_timer = 245760;
            if (ply_work.obj_work.spd_m >= 0 && ply_work.act_state == 72)
            {
                ply_work.gmk_flag &= 4293918719U;
                flag = true;
            }
        }
        if (flag)
        {
            float num = ply_work.obj_work.obj_3d.frame[0];
            if ((ply_work.gmk_flag & 1048576U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, 72);
            }
            else
            {
                GmPlayer.ActionChange(ply_work, 71);
            }
            ply_work.obj_work.disp_flag |= 4U;
            ply_work.obj_work.obj_3d.frame[0] = (ply_work.obj_work.obj_3d.frame[1] = num);
        }
        if (ply_work.obj_3d[(int)GmPlayer.g_gm_player_model_tbl[(int)ply_work.char_id][73]].act_id[0] == (int)GmPlayer.g_gm_player_motion_right_tbl[(int)ply_work.char_id][73] && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            if (ply_work.obj_work.spd_m >= 0)
            {
                ply_work.gmk_flag &= 4293918719U;
                GmPlayer.ActionChange(ply_work, 71);
            }
            else if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.gmk_flag |= 1048576U;
                GmPlayer.ActionChange(ply_work, 72);
            }
            ply_work.obj_work.disp_flag |= 4U;
        }
    }

    // Token: 0x0600176B RID: 5995 RVA: 0x000CCC10 File Offset: 0x000CAE10
    public static void GmPlySeqInitTruckFall(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_3d[(int)GmPlayer.g_gm_player_model_tbl[(int)ply_work.char_id][40]].act_id[0] != (int)GmPlayer.g_gm_player_motion_right_tbl[(int)ply_work.char_id][40])
        {
            GmPlayer.ActionChange(ply_work, 40);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 49296U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.gmk_flag &= 1073479679U;
        ply_work.seq_func = AppMain.gmPlySeqTruckJumpMain;
        ushort angle = (ushort)(ply_work.obj_work.dir.z + ply_work.obj_work.dir_fall - ply_work.jump_pseudofall_dir);
        ply_work.obj_work.spd.x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)angle));
        ply_work.obj_work.spd.y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)angle));
        ply_work.player_flag &= 4294967280U;
        ply_work.player_flag |= 1U;
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.timer = 0;
    }

    // Token: 0x0600176C RID: 5996 RVA: 0x000CCD78 File Offset: 0x000CAF78
    public static void GmPlySeqInitTruckJump(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_3d[(int)GmPlayer.g_gm_player_model_tbl[(int)ply_work.char_id][40]].act_id[0] != (int)GmPlayer.g_gm_player_motion_right_tbl[(int)ply_work.char_id][40])
        {
            GmPlayer.ActionChange(ply_work, 40);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag |= 49168U;
        ply_work.obj_work.move_flag &= 4290772990U;
        ushort num = ply_work.obj_work.dir.z;
        if ((num + 256 & 8192) != 0 && (num + 256 & 4095) <= 1024)
        {
            if (ply_work.obj_work.spd_m > 0 && num < 32768)
            {
                num -= 1152;
            }
            else if (ply_work.obj_work.spd_m < 0 && num > 32768)
            {
                num += 1152;
            }
        }
        num = (ushort)(num + ply_work.obj_work.dir_fall - ply_work.jump_pseudofall_dir);
        ply_work.obj_work.spd.x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)num));
        ply_work.obj_work.spd.y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)num));
        num = (ushort)(ply_work.obj_work.dir.z + ply_work.obj_work.dir_fall - ply_work.jump_pseudofall_dir);
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.spd.x = obj_work.spd.x + AppMain.FX_Mul(ply_work.spd_jump, AppMain.mtMathSin((int)num));
        OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
        obj_work2.spd.y = obj_work2.spd.y + AppMain.FX_Mul(-ply_work.spd_jump, AppMain.mtMathCos((int)num));
        ply_work.player_flag &= 4294967280U;
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.timer = 0;
        AppMain.GmPlySeqSetJumpState(ply_work, 0, 0U);
        ply_work.seq_func = AppMain.gmPlySeqTruckJumpMain;
        GmPlayer.SetAtk(ply_work);
        GmSound.PlaySE("Lorry3");
    }

    // Token: 0x0600176D RID: 5997 RVA: 0x000CCFA0 File Offset: 0x000CB1A0
    public static void gmPlySeqTruckJumpMain(GMS_PLAYER_WORK ply_work)
    {
        int y = ply_work.obj_work.spd.y;
        if (ply_work.obj_work.user_timer != 0)
        {
            ply_work.obj_work.user_timer--;
            if (ply_work.obj_work.user_timer == 0)
            {
                ply_work.obj_work.move_flag |= 128U;
            }
        }
        if ((ply_work.player_flag & 5U) == 0U && !GmPlayer.KeyCheckJumpKeyOn(ply_work) && y < -16384)
        {
            ply_work.player_flag |= 4U;
        }
        if ((ply_work.player_flag & 4U) != 0U && ply_work.obj_work.spd.y < 0)
        {
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.spd.y = obj_work.spd.y + ply_work.obj_work.spd_fall;
        }
        if ((ply_work.obj_work.move_flag & 6U) == 0U)
        {
            if (MTM_MATH_ABS(ply_work.obj_work.spd.x) < 4096)
            {
                ushort angle = (ushort)(ply_work.obj_work.dir_fall - AppMain.g_gm_main_system.pseudofall_dir);
                OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
                obj_work2.spd.x = obj_work2.spd.x + AppMain.mtMathSin((int)angle);
            }
        }
        if ((ply_work.obj_work.move_flag & 2U) != 0U)
        {
            OBS_OBJECT_WORK obj_work3 = ply_work.obj_work;
            obj_work3.spd.y = obj_work3.spd.y + ply_work.obj_work.spd_fall * 5;
        }
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            GmSound.PlaySE("Lorry4");
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            AppMain.GMM_PAD_VIB_MID();
        }
    }

    // Token: 0x0600176E RID: 5998 RVA: 0x000CD130 File Offset: 0x000CB330
    public static void GmPlySeqInitTruckSquatStart(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 14);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqTruckSquatMain;
    }

    // Token: 0x0600176F RID: 5999 RVA: 0x000CD160 File Offset: 0x000CB360
    public static void GmPlySeqInitTruckSquatMiddle(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 15);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 4096)
        {
            ply_work.obj_work.spd_m = 0;
        }
        ply_work.seq_func = AppMain.gmPlySeqTruckSquatMain;
    }

    // Token: 0x06001770 RID: 6000 RVA: 0x000CD1D1 File Offset: 0x000CB3D1
    public static void GmPlySeqInitTruckSquatEnd(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 16);
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqSquatEndMain;
    }

    // Token: 0x06001771 RID: 6001 RVA: 0x000CD204 File Offset: 0x000CB404
    public static void gmPlySeqTruckSquatMain(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd_m != 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 1);
            return;
        }
        int act_state = ply_work.act_state;
        if (act_state != 14)
        {
            return;
        }
        if ((ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 7);
        }
    }

    // Token: 0x06001772 RID: 6002 RVA: 0x000CD24C File Offset: 0x000CB44C
    public static void GmPlySeqInitTruckStaggerFront(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 33);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqTruckStaggerMain;
        ply_work.obj_work.user_timer = 0;
        AppMain.GmPlyEfctCreateSweat(ply_work);
    }

    // Token: 0x06001773 RID: 6003 RVA: 0x000CD2AC File Offset: 0x000CB4AC
    public static void GmPlySeqInitTruckStaggerBack(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.ActionChange(ply_work, 34);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqTruckStaggerMain;
        ply_work.obj_work.user_timer = 0;
        AppMain.GmPlyEfctCreateSweat(ply_work);
    }

    // Token: 0x06001774 RID: 6004 RVA: 0x000CD30C File Offset: 0x000CB50C
    public static void gmPlySeqTruckStaggerMain(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.gmk_flag & 262144U) == 0U)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        if (ply_work.seq_state == 13)
        {
            ply_work.obj_work.user_timer += 1024;
            if (ply_work.obj_work.user_timer > 8192)
            {
                ply_work.obj_work.user_timer = 8192;
                return;
            }
        }
        else
        {
            ply_work.obj_work.user_timer -= 1024;
            if (ply_work.obj_work.user_timer < -8192)
            {
                ply_work.obj_work.user_timer = -8192;
            }
        }
    }

    // Token: 0x06001775 RID: 6005 RVA: 0x000CD3B0 File Offset: 0x000CB5B0
    public static void GmPlySeqLandingSet(GMS_PLAYER_WORK ply_work, ushort dir_z)
    {
        GmPlayer.SpdParameterSet(ply_work);
        ply_work.obj_work.move_flag &= 4294934511U;
        ply_work.obj_work.move_flag |= 128U;
        ply_work.obj_work.disp_flag &= 4294967263U;
        ply_work.gmk_flag &= 4278190079U;
        ply_work.gmk_flag2 &= 4294967007U;
        ply_work.gmk_flag2 &= 4294966783U;
        ply_work.player_flag &= 4294967135U;
        ply_work.no_jump_move_timer = 0;
        ply_work.score_combo_cnt = 0U;
        if ((ply_work.gmk_flag & 1U) == 0U && (ply_work.obj_work.col_flag & 1U) != 0U)
        {
            ply_work.obj_work.dir.z = 0;
        }
        if (dir_z > 0)
        {
            if ((ply_work.player_flag & 262144U) != 0U)
            {
                if ((ply_work.gmk_flag2 & 1U) != 0U)
                {
                    ply_work.obj_work.spd_m += AppMain.FX_Mul(AppMain.FX_Mul(ply_work.obj_work.move.x, 2048), AppMain.mtMathCos((int)(dir_z - AppMain.g_gm_main_system.pseudofall_dir)));
                    ply_work.obj_work.spd_m += AppMain.FX_Mul(AppMain.FX_Mul(ply_work.obj_work.move.y, 2048), AppMain.mtMathSin((int)(dir_z - AppMain.g_gm_main_system.pseudofall_dir)));
                }
                else
                {
                    ply_work.obj_work.spd_m += AppMain.FX_Mul(ply_work.obj_work.move.x, AppMain.mtMathCos((int)(dir_z - AppMain.g_gm_main_system.pseudofall_dir)));
                    ply_work.obj_work.spd_m += AppMain.FX_Mul(ply_work.obj_work.move.y, AppMain.mtMathSin((int)(dir_z - AppMain.g_gm_main_system.pseudofall_dir)));
                }
            }
            else
            {
                ply_work.obj_work.spd_m += AppMain.FX_Mul(ply_work.obj_work.move.x, AppMain.mtMathCos((int)dir_z));
                ply_work.obj_work.spd_m += AppMain.FX_Mul(ply_work.obj_work.move.y, AppMain.mtMathSin((int)dir_z));
                if (AppMain.ObjObjectDirFallReverseCheck(ply_work.obj_work.dir_fall) != 0U)
                {
                    ply_work.obj_work.spd_m = -ply_work.obj_work.spd_m;
                }
            }
        }
        else if ((ply_work.player_flag & 262144U) != 0U)
        {
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x))
            {
                ply_work.obj_work.spd_m = ply_work.obj_work.spd.x;
            }
            ply_work.obj_work.spd_m += AppMain.FX_Mul(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x), AppMain.mtMathSin((int)(ply_work.obj_work.dir.z + (ply_work.obj_work.dir_fall - AppMain.g_gm_main_system.pseudofall_dir))));
        }
        else
        {
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x))
            {
                ply_work.obj_work.spd_m = ply_work.obj_work.spd.x;
            }
            ply_work.obj_work.spd_m += AppMain.FX_Mul(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x), AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
        }
        ply_work.gmk_flag2 &= 4294967294U;
        ply_work.spd_work_max = AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m);
        if (ply_work.spd_work_max > ply_work.obj_work.spd_slope_max)
        {
            ply_work.spd_work_max = ply_work.obj_work.spd_slope_max;
        }
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        if (dir_z != 0)
        {
            ply_work.obj_work.dir.z = dir_z;
        }
        if ((ply_work.gmk_flag & 4096U) == 0U)
        {
            ply_work.obj_work.dir.x = 0;
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            ply_work.obj_work.move_flag &= 4294950911U;
        }
    }

    // Token: 0x06001776 RID: 6006 RVA: 0x000CD830 File Offset: 0x000CBA30
    public static void GmPlySeqMoveFunc(OBS_OBJECT_WORK obj_work)
    {
        GMS_PLAYER_WORK gms_PLAYER_WORK = (GMS_PLAYER_WORK)obj_work;
        AppMain.GMS_PLY_SEQ_STATE_DATA[] seq_state_data_tbl = gms_PLAYER_WORK.seq_state_data_tbl;
        if ((seq_state_data_tbl[gms_PLAYER_WORK.seq_state].check_attr & 2147483648U) != 0U)
        {
            if ((gms_PLAYER_WORK.player_flag & 32768U) != 0U)
            {
                AppMain.GmPlySeqMoveWalkAutoRun(gms_PLAYER_WORK);
            }
            else if ((gms_PLAYER_WORK.player_flag & 262144U) != 0U)
            {
                AppMain.GmPlySeqMoveWalkTruck(gms_PLAYER_WORK);
            }
            else
            {
                AppMain.GmPlySeqMoveWalk(gms_PLAYER_WORK);
            }
        }
        if ((seq_state_data_tbl[gms_PLAYER_WORK.seq_state].check_attr & 1073741824U) != 0U && (gms_PLAYER_WORK.player_flag & 32U) == 0U)
        {
            if ((gms_PLAYER_WORK.player_flag & 32768U) != 0U)
            {
                AppMain.GmPlySeqMoveJumpAutoRun(gms_PLAYER_WORK);
            }
            else if ((gms_PLAYER_WORK.player_flag & 262144U) != 0U)
            {
                AppMain.GmPlySeqMoveJumpTruck(gms_PLAYER_WORK);
            }
            else
            {
                AppMain.GmPlySeqMoveJump(gms_PLAYER_WORK);
            }
        }
        if (gms_PLAYER_WORK.no_jump_move_timer != 0)
        {
            gms_PLAYER_WORK.no_jump_move_timer = AppMain.ObjTimeCountDown(gms_PLAYER_WORK.no_jump_move_timer);
            if (gms_PLAYER_WORK.no_jump_move_timer == 0)
            {
                gms_PLAYER_WORK.player_flag &= 4294967263U;
            }
        }
        if ((seq_state_data_tbl[gms_PLAYER_WORK.seq_state].check_attr & 536870912U) != 0U)
        {
            AppMain.GmPlySeqMoveSpin(gms_PLAYER_WORK);
        }
        if ((seq_state_data_tbl[gms_PLAYER_WORK.seq_state].check_attr & 134217728U) != 0U)
        {
            AppMain.GmPlySeqMoveSpinNoDec(gms_PLAYER_WORK);
        }
        if ((seq_state_data_tbl[gms_PLAYER_WORK.seq_state].check_attr & 67108864U) != 0U)
        {
            AppMain.GmPlySeqMoveSpinPinball(gms_PLAYER_WORK);
        }
        if ((seq_state_data_tbl[gms_PLAYER_WORK.seq_state].check_attr & 268435456U) != 0U)
        {
            if ((gms_PLAYER_WORK.player_flag & 262144U) != 0U)
            {
                AppMain.GmPlySeqTruckJumpDirec(gms_PLAYER_WORK);
            }
            else if (AppMain.GSM_MAIN_STAGE_IS_SPSTAGE_NOT_RETRY())
            {
                AppMain.gmPlySeqSplJumpDirec(gms_PLAYER_WORK);
            }
            else
            {
                AppMain.GmPlySeqJumpDirec(gms_PLAYER_WORK);
            }
        }
        if ((gms_PLAYER_WORK.player_flag & 32768U) != 0U && 
            (gms_PLAYER_WORK.player_flag & 1024U) == 0U /* && 
            obj_work.pos.x <= AppMain.g_obj.camera[0][0] + 65536 && 
            obj_work.pos.x > AppMain.g_obj.camera[0][0] + 65536 - 4194304*/)
        {
            if ((obj_work.move_flag & 16U) != 0U)
            {
                if (obj_work.spd.x < gms_PLAYER_WORK.scroll_spd_x)
                {
                    obj_work.spd.x = gms_PLAYER_WORK.scroll_spd_x;
                    if ((obj_work.disp_flag & 1U) != 0U)
                    {
                        AppMain.GmPlySeqSetProgramTurn(gms_PLAYER_WORK, 4096);
                    }
                }
            }
            else if (obj_work.spd_m < gms_PLAYER_WORK.scroll_spd_x)
            {
                obj_work.spd_m = gms_PLAYER_WORK.scroll_spd_x;
                if (3 <= gms_PLAYER_WORK.seq_state && gms_PLAYER_WORK.seq_state <= 5)
                {
                    AppMain.GmPlySeqChangeSequence(gms_PLAYER_WORK, 0);
                }
                if ((obj_work.disp_flag & 1U) != 0U)
                {
                    if (gms_PLAYER_WORK.seq_state == 10)
                    {
                        GmPlayer.SetReverse(gms_PLAYER_WORK);
                        AppMain.GmPlySeqChangeSequence(gms_PLAYER_WORK, 0);
                    }
                    else if (6 <= gms_PLAYER_WORK.seq_state && gms_PLAYER_WORK.seq_state <= 8)
                    {
                        AppMain.GmPlySeqSetProgramTurn(gms_PLAYER_WORK, 4096);
                    }
                    else
                    {
                        AppMain.GmPlySeqChangeSequence(gms_PLAYER_WORK, 2);
                    }
                }
            }
        }
        if ((gms_PLAYER_WORK.player_flag & 262144U) != 0U)
        {
            if ((obj_work.move_flag & 1U) == 0U || (gms_PLAYER_WORK.gmk_flag & 262144U) == 0U)
            {
                AppMain.gmPlySeqTruckMove(obj_work);
                return;
            }
        }
        else
        {
            if (AppMain.GSM_MAIN_STAGE_IS_SPSTAGE_NOT_RETRY())
            {
                AppMain.gmPlySeqSplMove(obj_work);
                return;
            }
            AppMain.ObjObjectMove(obj_work);
        }
    }

    // Token: 0x06001777 RID: 6007 RVA: 0x000CDB24 File Offset: 0x000CBD24
    public static void GmPlySeqMoveWalk(GMS_PLAYER_WORK ply_work)
    {
        int speed_add = ply_work.spd_add;
        int speed_dec = ply_work.spd_dec;
        int speed_max = ply_work.spd_max;
        if (GmPlayer.KeyCheckWalkRight(ply_work) || GmPlayer.KeyCheckWalkLeft(ply_work))
        {
            int num4 = AppMain.MTM_MATH_ABS(ply_work.key_walk_rot_z);
            if (num4 > 24576)
            {
                num4 = 24576;
            }
            speed_max = speed_max * num4 / 24576;
        }
        if (speed_max < ply_work.prev_walk_roll_spd_max)
        {
            speed_max = ply_work.prev_walk_roll_spd_max - speed_dec;
            if (speed_max < 0)
            {
                speed_max = 0;
            }
        }
        ply_work.prev_walk_roll_spd_max = speed_max;
        if (ply_work.obj_work.dir.z > 0)
        {
            int num5 = AppMain.FX_Mul(ply_work.spd_max_add_slope, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
            if (num5 > 0)
            {
                speed_max += num5;
            }
        }
        if (ply_work.no_spddown_timer != 0)
        {
            speed_dec = 0;
        }
        else
        {
            int num6 = 0;
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) > ply_work.spd3)
            {
                if (speed_max - ply_work.spd3 != 0)
                {
                    num6 = AppMain.FX_Div(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) - ply_work.spd3, speed_max - ply_work.spd3);
                    if (num6 > 4096)
                    {
                        num6 = 4096;
                    }
                }
                else
                {
                    num6 = 4096;
                }
                num6 = num6 * 3968 >> 12;
            }
            speed_add -= AppMain.FX_Mul(speed_add, num6);
        }
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmPlayer.GMD_PLAYER_WATER_SET(ref speed_add);
            GmPlayer.GMD_PLAYER_WATER_SET(ref speed_dec);
        }
        if (ply_work.spd_work_max >= speed_max && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) >= speed_max)
        {
            if (ply_work.spd_work_max > ply_work.obj_work.spd_m)
            {
                ply_work.spd_work_max = AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m);
            }
            speed_max = ply_work.spd_work_max;
        }
        if ((ply_work.player_flag & 32768U) != 0U && GmPlayer.KeyCheckWalkRight(ply_work) && speed_max > ply_work.scroll_spd_x + 8192)
        {
            speed_max = ply_work.scroll_spd_x + 8192;
        }
        if (!(GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            ply_work.spd_pool = 0;
            //ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP( ply_work.obj_work.spd.x, -speed_max, speed_max );
            //ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP( ply_work.obj_work.spd_m, -speed_max, speed_max );
            if ((ply_work.obj_work.dir.z + 8192 & 65280) <= 16384)
            {
                if ((ply_work.player_flag & 134217728U) != 0U)
                {
                    ply_work.player_flag &= 4160749567U;
                    return;
                }
                if ((ply_work.player_flag & 32768U) != 0U)
                {
                    if ((ply_work.obj_work.disp_flag & 1U) != 0U || ply_work.seq_state != 1)
                    {
                        ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, speed_dec);
                        return;
                    }
                    int num7 = ply_work.scroll_spd_x + -4096;
                    if (num7 < 0)
                    {
                        num7 = 0;
                    }
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, speed_dec);
                    if (ply_work.obj_work.spd_m < num7)
                    {
                        ply_work.obj_work.spd_m = num7;
                        return;
                    }
                }
                else
                {
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, speed_dec);
                }
            }
            return;
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, speed_dec);
            }
            ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, speed_add, speed_max);
            return;
        }
        if (ply_work.obj_work.spd_m > 0)
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, speed_dec);
        }
        ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, -speed_add, speed_max);
    }

    // Token: 0x06001778 RID: 6008 RVA: 0x000CDEEC File Offset: 0x000CC0EC
    public static void GmPlySeqMoveWalkTruck(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.gmk_flag & 262144U) != 0U)
        {
            return;
        }
        int num = ply_work.spd_add;
        int sSpd = ply_work.spd_dec;
        int num2 = ply_work.spd_max;
        ushort num3 = (ushort)(ply_work.obj_work.dir.z + (ply_work.obj_work.dir_fall - AppMain.g_gm_main_system.pseudofall_dir));
        if (num3 != 0)
        {
            int num4 = AppMain.FX_Mul(ply_work.spd_max_add_slope, AppMain.mtMathSin((int)num3));
            if (num4 > 0)
            {
                num2 += num4;
            }
        }
        if (ply_work.no_spddown_timer != 0)
        {
            sSpd = 0;
        }
        else
        {
            int num5 = 0;
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) > ply_work.spd3)
            {
                if (num2 - ply_work.spd3 != 0)
                {
                    num5 = AppMain.FX_Div(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) - ply_work.spd3, num2 - ply_work.spd3);
                    if (num5 > 4096)
                    {
                        num5 = 4096;
                    }
                }
                else
                {
                    num5 = 4096;
                }
                num5 = num5 * 3968 >> 12;
            }
            num -= AppMain.FX_Mul(num, num5);
        }
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmPlayer.GMD_PLAYER_WATER_SET(ref num);
            GmPlayer.GMD_PLAYER_WATER_SET(ref sSpd);
        }
        if (ply_work.spd_work_max >= num2 && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) >= num2)
        {
            if (ply_work.spd_work_max > ply_work.obj_work.spd_m)
            {
                ply_work.spd_work_max = AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m);
            }
            num2 = ply_work.spd_work_max;
        }
        if (((AppMain.g_gm_main_system.game_flag & 1048576U) == 0U && (ply_work.player_flag & 16777216U) == 0U) || !(GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            if ((int)(num3 + ply_work.obj_work.dir_slope & 65535) < (int)ply_work.obj_work.dir_slope << 1)
            {
                ply_work.spd_pool = 0;
                ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -num2, num2);
                ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -num2, num2);
                if ((num3 + 2048 & 65280) <= 4096)
                {
                    if ((ply_work.player_flag & 134217728U) != 0U)
                    {
                        ply_work.player_flag &= 4160749567U;
                        return;
                    }
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, sSpd);
                }
            }
            return;
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, sSpd);
            }
            ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, num, num2);
            return;
        }
        if (ply_work.obj_work.spd_m > 0)
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, sSpd);
        }
        ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, -num, num2);
    }

    // Token: 0x06001779 RID: 6009 RVA: 0x000CE1E8 File Offset: 0x000CC3E8
    public static void GmPlySeqMoveWalkAutoRun(GMS_PLAYER_WORK ply_work)
    {
        int speed_add = ply_work.spd_add;
        int num2 = ply_work.spd_dec;
        int num3 = ply_work.spd_max;
        num3 = AppMain.FX_F32_TO_FX32(9.5f);
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd_m <= ply_work.spd3)
            {
                speed_add >>= 2;
            }
            if (ply_work.obj_work.spd_m < ply_work.scroll_spd_x + AppMain.FX_F32_TO_FX32(0.25f))
            {
                ply_work.obj_work.spd_m = ply_work.scroll_spd_x + AppMain.FX_F32_TO_FX32(0.25f);
            }
            if (ply_work.obj_work.spd_m < AppMain.FX_F32_TO_FX32(8.4f))
            {
                ply_work.obj_work.spd_m = AppMain.FX_F32_TO_FX32(8.4f);
            }
            if (ply_work.obj_work.spd_m > AppMain.FX_F32_TO_FX32(8.7f))
            {
                ply_work.obj_work.spd_m = AppMain.FX_F32_TO_FX32(8.7f);
            }
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work) || GmPlayer.KeyCheckWalkLeft(ply_work))
        {
            int num4 = AppMain.MTM_MATH_ABS(ply_work.key_walk_rot_z);
            if (num4 > 24576)
            {
                num4 = 24576;
            }
            num3 = num3 * num4 / 24576;
        }
        if (num3 < ply_work.prev_walk_roll_spd_max)
        {
            num3 = ply_work.prev_walk_roll_spd_max - num2;
            if (num3 < 0)
            {
                num3 = 0;
            }
        }
        ply_work.prev_walk_roll_spd_max = num3;
        if (ply_work.obj_work.dir.z != 0)
        {
            int num5 = AppMain.FX_Mul(ply_work.spd_max_add_slope, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
            if (num5 > 0)
            {
                num3 += num5;
            }
        }
        if (ply_work.no_spddown_timer != 0)
        {
            num2 = 0;
        }
        else
        {
            int num6 = 0;
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) > ply_work.spd3)
            {
                if (num3 - ply_work.spd3 != 0)
                {
                    num6 = AppMain.FX_Div(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) - ply_work.spd3, num3 - ply_work.spd3);
                    if (num6 > 4096)
                    {
                        num6 = 4096;
                    }
                }
                else
                {
                    num6 = 4096;
                }
                num6 = num6 * 3968 >> 12;
            }
            speed_add -= AppMain.FX_Mul(speed_add, num6);
        }
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmPlayer.GMD_PLAYER_WATER_SET(ref speed_add);
            GmPlayer.GMD_PLAYER_WATER_SET(ref num2);
        }
        if (ply_work.spd_work_max >= num3 && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) >= num3)
        {
            if (ply_work.spd_work_max > ply_work.obj_work.spd_m)
            {
                ply_work.spd_work_max = AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m);
            }
            num3 = ply_work.spd_work_max;
        }
        if ((ply_work.player_flag & 32768U) != 0U)
        {
            ply_work.spd_work_max += 8192;
            num3 = ply_work.spd_work_max + 8192;
        }
        if (!(GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            ply_work.spd_pool = 0;
            ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -num3, num3);
            ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -num3, num3);
            if ((ply_work.obj_work.dir.z + 8192 & 65280) <= 16384)
            {
                if ((ply_work.player_flag & 134217728U) != 0U)
                {
                    ply_work.player_flag &= 4160749567U;
                    return;
                }
                if ((ply_work.player_flag & 32768U) != 0U)
                {
                    if ((ply_work.obj_work.disp_flag & 1U) != 0U || ply_work.seq_state != 1)
                    {
                        ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
                        return;
                    }
                    int num7 = ply_work.scroll_spd_x + -4096;
                    if (num7 < 0)
                    {
                        num7 = 0;
                    }
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
                    if (ply_work.obj_work.spd_m < num7)
                    {
                        ply_work.obj_work.spd_m = num7;
                        return;
                    }
                }
                else
                {
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
                }
            }
            return;
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
            }
            ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, speed_add, num3);
            return;
        }
        if (ply_work.obj_work.spd_m > 0)
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
        }
        ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, -speed_add, num3);
    }

    // Token: 0x0600177A RID: 6010 RVA: 0x000CE668 File Offset: 0x000CC868
    public static void GmPlySeqMoveJump(GMS_PLAYER_WORK ply_work)
    {
        int num = ply_work.spd_jump_add;
        int num2 = ply_work.spd_jump_dec;
        int spd_jump_dec = ply_work.spd_jump_dec;
        int spd_jump_max = ply_work.spd_jump_max;
        ply_work.spd_work_max = 0;
        if ((ply_work.obj_work.dir.z + 8192 & 49152) != 0 || ply_work.obj_work.dir.z == 57344)
        {
            num2 >>= 2;
        }
        if (ply_work.no_spddown_timer != 0)
        {
            num2 = 0;
            num >>= 2;
        }
        else
        {
            int num3 = 0;
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) > ply_work.spd2)
            {
                if (spd_jump_max - ply_work.spd2 != 0)
                {
                    num3 = AppMain.FX_Div(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) - ply_work.spd2, spd_jump_max - ply_work.spd2);
                    if (num3 > 4096)
                    {
                        num3 = 4096;
                    }
                }
                else
                {
                    num3 = 4096;
                }
                num3 = num3 * 3968 >> 12;
            }
            num -= AppMain.FX_Mul(num, num3);
        }
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmPlayer.GMD_PLAYER_WATER_SET(ref num);
            GmPlayer.GMD_PLAYER_WATER_SET(ref num2);
        }
        int sSpd = AppMain.FX_Mul(num2, 4096);
        AppMain.FX_Mul(spd_jump_dec, 4096);
        if (!(GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -spd_jump_max, spd_jump_max);
            ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -spd_jump_max, spd_jump_max);
            ply_work.spd_pool = 0;
            ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, num2);
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
            return;
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd.x < 0)
            {
                ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, sSpd);
            }
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
            ply_work.obj_work.spd.x = AppMain.ObjSpdUpSet(ply_work.obj_work.spd.x, num, spd_jump_max);
            return;
        }
        if (ply_work.obj_work.spd.x > 0)
        {
            ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, sSpd);
        }
        ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
        ply_work.obj_work.spd.x = AppMain.ObjSpdUpSet(ply_work.obj_work.spd.x, -num, spd_jump_max);
    }

    // Token: 0x0600177B RID: 6011 RVA: 0x000CE948 File Offset: 0x000CCB48
    public static void GmPlySeqMoveJumpTruck(GMS_PLAYER_WORK ply_work)
    {
        int num = ply_work.spd_jump_add;
        int num2 = ply_work.spd_jump_dec;
        int spd_jump_dec = ply_work.spd_jump_dec;
        int spd_jump_max = ply_work.spd_jump_max;
        ply_work.spd_work_max = 0;
        if ((ply_work.obj_work.dir.z + 8192 & 49152) != 0 || ply_work.obj_work.dir.z == 57344)
        {
            num2 >>= 2;
        }
        if (ply_work.no_spddown_timer != 0)
        {
            num2 = 0;
            num >>= 2;
        }
        else
        {
            int num3 = 0;
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) > ply_work.spd2)
            {
                if (spd_jump_max - ply_work.spd2 != 0)
                {
                    num3 = AppMain.FX_Div(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) - ply_work.spd2, spd_jump_max - ply_work.spd2);
                    if (num3 > 4096)
                    {
                        num3 = 4096;
                    }
                }
                else
                {
                    num3 = 4096;
                }
                num3 = num3 * 3968 >> 12;
            }
            num -= AppMain.FX_Mul(num, num3);
        }
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmPlayer.GMD_PLAYER_WATER_SET(ref num);
            GmPlayer.GMD_PLAYER_WATER_SET(ref num2);
        }
        int num4 = AppMain.FX_Mul(num2, 4096);
        int num5 = AppMain.FX_Mul(spd_jump_dec, 4096);
        int num6 = 12288;
        if ((ply_work.gmk_flag2 & 512U) != 0U)
        {
            ushort angle = (ushort)(ply_work.obj_work.dir_fall - AppMain.g_gm_main_system.pseudofall_dir);
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.spd.x = obj_work.spd.x + AppMain.mtMathSin((int)angle) / 3;
            if (ply_work.obj_work.spd.x > num6)
            {
                ply_work.obj_work.spd.x = num6;
                return;
            }
            if (ply_work.obj_work.spd.x < -num6)
            {
                ply_work.obj_work.spd.x = -num6;
                return;
            }
        }
        else
        {
            if ((ply_work.gmk_flag2 & 1U) != 0U)
            {
                num4 = AppMain.FX_Mul(num4, 3072);
                num5 = AppMain.FX_Mul(num5, 3072);
                ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -spd_jump_max, spd_jump_max);
                ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -spd_jump_max, spd_jump_max);
                ply_work.spd_pool = 0;
                ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, num4);
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num5);
                return;
            }
            if (GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work))
            {
                if (GmPlayer.KeyCheckWalkRight(ply_work))
                {
                    if (ply_work.obj_work.spd.x < 0)
                    {
                        ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, num4);
                    }
                    if (ply_work.obj_work.spd_m < 0)
                    {
                        ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
                    }
                    ply_work.obj_work.spd.x = AppMain.ObjSpdUpSet(ply_work.obj_work.spd.x, num, spd_jump_max);
                    return;
                }
                if (ply_work.obj_work.spd.x > 0)
                {
                    ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, num4);
                }
                if (ply_work.obj_work.spd_m > 0)
                {
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
                }
                ply_work.obj_work.spd.x = AppMain.ObjSpdUpSet(ply_work.obj_work.spd.x, -num, spd_jump_max);
                return;
            }
            else
            {
                ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -spd_jump_max, spd_jump_max);
                ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -spd_jump_max, spd_jump_max);
                ply_work.spd_pool = 0;
                ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, num2);
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
            }
        }
    }

    // Token: 0x0600177C RID: 6012 RVA: 0x000CEDAC File Offset: 0x000CCFAC
    public static void GmPlySeqMoveJumpAutoRun(GMS_PLAYER_WORK ply_work)
    {
        int num = ply_work.spd_jump_add;
        int num2 = ply_work.spd_jump_dec;
        int spd_jump_dec = ply_work.spd_jump_dec;
        int spd_jump_max = ply_work.spd_jump_max;
        ply_work.spd_work_max = 0;
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            num = 0;
        }
        if ((ply_work.obj_work.dir.z + 8192 & 49152) != 0 || ply_work.obj_work.dir.z == 57344)
        {
            num2 >>= 2;
        }
        if (ply_work.no_spddown_timer != 0)
        {
            num2 = 0;
            num >>= 2;
        }
        else
        {
            int num3 = 0;
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) > ply_work.spd2)
            {
                if (spd_jump_max - ply_work.spd2 != 0)
                {
                    num3 = AppMain.FX_Div(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) - ply_work.spd2, spd_jump_max - ply_work.spd2);
                    if (num3 > 4096)
                    {
                        num3 = 4096;
                    }
                }
                else
                {
                    num3 = 4096;
                }
                num3 = num3 * 3968 >> 12;
            }
            num -= AppMain.FX_Mul(num, num3);
        }
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmPlayer.GMD_PLAYER_WATER_SET(ref num);
            GmPlayer.GMD_PLAYER_WATER_SET(ref num2);
        }
        int sSpd = AppMain.FX_Mul(num2, 4096);
        AppMain.FX_Mul(spd_jump_dec, 4096);
        if (!(GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -spd_jump_max, spd_jump_max);
            ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -spd_jump_max, spd_jump_max);
            ply_work.spd_pool = 0;
            ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, num2);
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
            return;
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd.x < 0)
            {
                ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, sSpd);
            }
            if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
            }
            ply_work.obj_work.spd.x = AppMain.ObjSpdUpSet(ply_work.obj_work.spd.x, num, spd_jump_max);
            return;
        }
        if (ply_work.obj_work.spd.x > 0)
        {
            ply_work.obj_work.spd.x = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.x, sSpd);
        }
        if (ply_work.obj_work.spd_m > 0)
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, spd_jump_dec);
        }
        ply_work.obj_work.spd.x = AppMain.ObjSpdUpSet(ply_work.obj_work.spd.x, -num, spd_jump_max);
    }

    // Token: 0x0600177D RID: 6013 RVA: 0x000CF0B4 File Offset: 0x000CD2B4
    public static void GmPlySeqMoveSpin(GMS_PLAYER_WORK ply_work)
    {
        int num = ply_work.spd_dec_spin;
        if (ply_work.no_spddown_timer != 0)
        {
            ply_work.obj_work.spd_slope = 0;
            num = 0;
        }
        else
        {
            if (ply_work.seq_state != 37)
            {
                ply_work.obj_work.spd_slope = GmPlayer.g_gm_player_parameter[(int)ply_work.char_id].spd_slope_spin;
            }
            else
            {
                ply_work.obj_work.spd_slope = GmPlayer.g_gm_player_parameter[(int)ply_work.char_id].spd_slope_spin_spipe;
            }
            ply_work.obj_work.dir_slope = 640;
        }
        if ((ply_work.obj_work.spd_m > 0 && (ply_work.key_on & 8) != 0) || (ply_work.obj_work.spd_m < 0 && (ply_work.key_on & 4) != 0))
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num >> 1);
            return;
        }
        if ((ply_work.obj_work.spd_m <= 0 || (ply_work.key_on & 8) == 0) && (ply_work.obj_work.spd_m >= 0 || (ply_work.key_on & 4) == 0))
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num << 1);
            return;
        }
        ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num);
    }

    // Token: 0x0600177E RID: 6014 RVA: 0x000CF1F8 File Offset: 0x000CD3F8
    public static void GmPlySeqMoveSpinNoDec(GMS_PLAYER_WORK ply_work)
    {
        int spd_dec_spin = ply_work.spd_dec_spin;
        if (ply_work.no_spddown_timer != 0)
        {
            ply_work.obj_work.spd_slope = 0;
            return;
        }
        if (ply_work.seq_state != 37)
        {
            ply_work.obj_work.spd_slope = GmPlayer.g_gm_player_parameter[(int)ply_work.char_id].spd_slope_spin;
        }
        else
        {
            ply_work.obj_work.spd_slope = GmPlayer.g_gm_player_parameter[(int)ply_work.char_id].spd_slope_spin_spipe;
        }
        ply_work.obj_work.dir_slope = 640;
    }

    // Token: 0x0600177F RID: 6015 RVA: 0x000CF280 File Offset: 0x000CD480
    public static void GmPlySeqMoveSpinPinball(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.spd_slope = GmPlayer.g_gm_player_parameter[(int)ply_work.char_id].spd_slope_spin_pinball;
        ply_work.obj_work.dir_slope = 256;
        int spd_add_spin_pinball = ply_work.spd_add_spin_pinball;
        int num = ply_work.spd_dec_spin_pinball;
        int num2 = ply_work.spd_max_spin_pinball;
        int num3 = AppMain.MTM_MATH_ABS(ply_work.key_walk_rot_z);
        if (num3 > 24576)
        {
            num3 = 24576;
        }
        num2 = num2 * num3 / 24576;
        if (num2 < ply_work.prev_walk_roll_spd_max)
        {
            num2 = ply_work.prev_walk_roll_spd_max - num;
            if (num2 < 0)
            {
                num2 = 0;
            }
        }
        ply_work.prev_walk_roll_spd_max = num2;
        if (ply_work.obj_work.dir.z != 0)
        {
            int num4 = AppMain.FX_Mul(ply_work.spd_max_add_slope_spin_pinball, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
            if (num4 > 0)
            {
                num2 += num4;
            }
        }
        if (ply_work.no_spddown_timer != 0)
        {
            num = 0;
        }
        if (ply_work.spd_work_max >= num2 && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) >= num2)
        {
            if (ply_work.spd_work_max > ply_work.obj_work.spd_m)
            {
                ply_work.spd_work_max = AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m);
            }
            num2 = ply_work.spd_work_max;
        }
        if (!(GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            ply_work.spd_pool = 0;
            ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -num2, num2);
            ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -num2, num2);
            if ((ply_work.obj_work.dir.z + 8192 & 65280) <= 16384)
            {
                if ((ply_work.player_flag & 134217728U) != 0U)
                {
                    ply_work.player_flag &= 4160749567U;
                    return;
                }
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num);
            }
            return;
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num);
            }
            ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, spd_add_spin_pinball, num2);
            return;
        }
        if (ply_work.obj_work.spd_m > 0)
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num);
        }
        ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, -spd_add_spin_pinball, num2);
    }

    // Token: 0x06001780 RID: 6016 RVA: 0x000CF504 File Offset: 0x000CD704
    public static void gmPlySeqTruckMove(OBS_OBJECT_WORK obj_work)
    {
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        GMS_PLAYER_WORK gms_PLAYER_WORK = (GMS_PLAYER_WORK)obj_work;
        ushort num4 = (ushort)(obj_work.dir.z + (obj_work.dir_fall - AppMain.g_gm_main_system.pseudofall_dir));
        obj_work.prev_pos.x = obj_work.pos.x;
        obj_work.prev_pos.y = obj_work.pos.y;
        obj_work.prev_pos.z = obj_work.pos.z;
        if ((obj_work.move_flag & 134217728U) != 0U)
        {
            obj_work.flow.x = 0;
            obj_work.flow.y = 0;
            obj_work.flow.z = 0;
        }
        int x = obj_work.flow.x;
        int y = obj_work.flow.y;
        if ((x != 0 || y != 0) && obj_work.dir_fall != 0)
        {
            AppMain.ObjObjectSpdDirFall(ref x, ref y, obj_work.dir_fall);
        }
        if (obj_work.hitstop_timer != 0)
        {
            obj_work.move.x = AppMain.FX_Mul(x, AppMain.g_obj.speed);
            obj_work.move.y = AppMain.FX_Mul(y, AppMain.g_obj.speed);
            obj_work.move.z = AppMain.FX_Mul(obj_work.flow.z, AppMain.g_obj.speed);
        }
        else
        {
            if ((obj_work.move_flag & 1U) == 0U)
            {
                if ((obj_work.move_flag & 128U) != 0U && (obj_work.move_flag & 1U) == 0U)
                {
                    obj_work.spd.y = obj_work.spd.y + AppMain.FX_Mul(obj_work.spd_fall, AppMain.g_obj.speed);
                }
                if ((obj_work.move_flag & 128U) != 0U && obj_work.spd.y > obj_work.spd_fall_max)
                {
                    obj_work.spd.y = obj_work.spd_fall_max;
                }
            }
            if ((obj_work.move_flag & 64U) != 0U)
            {
                if ((obj_work.move_flag & 131072U) != 0U && (obj_work.spd_m != 0 || (obj_work.move_flag & 262144U) == 0U) && (int)(num4 + obj_work.dir_slope & 65535) >= (int)obj_work.dir_slope << 1)
                {
                    int num5;
                    if (AppMain.MTM_MATH_ABS(obj_work.spd_m) < 16384)
                    {
                        num5 = obj_work.spd_slope;
                    }
                    else
                    {
                        num5 = obj_work.spd_slope << 1;
                    }
                    if ((obj_work.spd_m > 0 && num4 > 32768) || (obj_work.spd_m < 0 && num4 < 32768))
                    {
                        num5 <<= 1;
                    }
                    int num6 = AppMain.FX_Mul(num5, AppMain.mtMathSin((int)num4));
                    if (num6 > 0 || num4 < 32768)
                    {
                        if (num6 < 256)
                        {
                            num6 = 256;
                        }
                    }
                    else if (num6 > -256)
                    {
                        num6 = -256;
                    }
                    obj_work.spd_m = AppMain.ObjSpdUpSet(obj_work.spd_m, num6, obj_work.spd_slope_max);
                }
                if ((obj_work.move_flag & 32768U) == 0U)
                {
                    num = AppMain.FX_Mul(obj_work.spd_m, AppMain.mtMathCos((int)obj_work.dir.z));
                    num2 = AppMain.FX_Mul(obj_work.spd_m, AppMain.mtMathSin((int)obj_work.dir.z));
                }
            }
            if ((obj_work.move_flag & 67108864U) != 0U)
            {
                obj_work.move.x = AppMain.FX_Mul(obj_work.spd.x + num + x, AppMain.g_obj.speed);
                obj_work.move.y = AppMain.FX_Mul(obj_work.spd.y + num2 + y, AppMain.g_obj.speed);
            }
            else
            {
                obj_work.move.x = AppMain.FX_Mul(obj_work.spd.x + num + x + AppMain.g_obj.scroll[0], AppMain.g_obj.speed);
                obj_work.move.y = AppMain.FX_Mul(obj_work.spd.y + num2 + y + AppMain.g_obj.scroll[1], AppMain.g_obj.speed);
            }
            obj_work.move.z = AppMain.FX_Mul(obj_work.spd.z + num3 + obj_work.flow.z, AppMain.g_obj.speed);
            if ((obj_work.move_flag & 1U) != 0U)
            {
                AppMain.ObjObjectSpdDirFall(ref obj_work.move.x, ref obj_work.move.y, obj_work.dir_fall);
            }
            else
            {
                AppMain.ObjObjectSpdDirFall(ref obj_work.move.x, ref obj_work.move.y, gms_PLAYER_WORK.jump_pseudofall_dir);
            }
        }
        obj_work.pos.x = obj_work.pos.x + obj_work.move.x;
        obj_work.pos.y = obj_work.pos.y + obj_work.move.y;
        obj_work.pos.z = obj_work.pos.z + obj_work.move.z;
        obj_work.spd.x = obj_work.spd.x + obj_work.spd_add.x;
        obj_work.spd.y = obj_work.spd.y + obj_work.spd_add.y;
        obj_work.spd.z = obj_work.spd.z + obj_work.spd_add.z;
        obj_work.flow.x = 0;
        obj_work.flow.y = 0;
        obj_work.flow.z = 0;
    }

    // Token: 0x06001781 RID: 6017 RVA: 0x000CFA3C File Offset: 0x000CDC3C
    public static void gmPlySeqSplMove(OBS_OBJECT_WORK obj_work)
    {
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        GMS_PLAYER_WORK gms_PLAYER_WORK = (GMS_PLAYER_WORK)obj_work;
        ushort num4 = (ushort)(obj_work.dir.z + (obj_work.dir_fall - AppMain.g_gm_main_system.pseudofall_dir));
        obj_work.prev_pos.x = obj_work.pos.x;
        obj_work.prev_pos.y = obj_work.pos.y;
        obj_work.prev_pos.z = obj_work.pos.z;
        if ((obj_work.move_flag & 134217728U) != 0U)
        {
            obj_work.flow.x = 0;
            obj_work.flow.y = 0;
            obj_work.flow.z = 0;
        }
        int x = obj_work.flow.x;
        int y = obj_work.flow.y;
        if ((x != 0 || y != 0) && obj_work.dir_fall != 0)
        {
            AppMain.ObjObjectSpdDirFall(ref x, ref y, obj_work.dir_fall);
        }
        if (obj_work.hitstop_timer != 0)
        {
            obj_work.move.x = AppMain.FX_Mul(x, AppMain.g_obj.speed);
            obj_work.move.y = AppMain.FX_Mul(y, AppMain.g_obj.speed);
            obj_work.move.z = AppMain.FX_Mul(obj_work.flow.z, AppMain.g_obj.speed);
        }
        else
        {
            if ((obj_work.move_flag & 1U) == 0U)
            {
                if ((obj_work.move_flag & 128U) != 0U && (obj_work.move_flag & 1U) == 0U)
                {
                    obj_work.spd.y = obj_work.spd.y + AppMain.FX_Mul(obj_work.spd_fall, AppMain.g_obj.speed);
                }
                if ((obj_work.move_flag & 128U) != 0U && obj_work.spd.y > obj_work.spd_fall_max)
                {
                    obj_work.spd.y = obj_work.spd_fall_max;
                }
            }
            if ((obj_work.move_flag & 64U) != 0U)
            {
                if ((obj_work.move_flag & 131072U) != 0U && (obj_work.spd_m != 0 || (obj_work.move_flag & 262144U) == 0U))
                {
                    if ((int)(num4 + obj_work.dir_slope & 65535) >= (int)obj_work.dir_slope << 1 && (obj_work.move_flag & 1U) != 0U)
                    {
                        if (AppMain.MTM_MATH_ABS(obj_work.spd_m) < 8192)
                        {
                            obj_work.spd_m = AppMain.ObjSpdUpSet(obj_work.spd_m, AppMain.FX_Mul(obj_work.spd_slope >> 1, AppMain.mtMathSin((int)num4)), obj_work.spd_slope_max);
                        }
                        else if (AppMain.MTM_MATH_ABS(obj_work.spd_m) < 16384)
                        {
                            obj_work.spd_m = AppMain.ObjSpdUpSet(obj_work.spd_m, AppMain.FX_Mul(obj_work.spd_slope, AppMain.mtMathSin((int)num4)), obj_work.spd_slope_max);
                        }
                        else
                        {
                            obj_work.spd_m = AppMain.ObjSpdUpSet(obj_work.spd_m, AppMain.FX_Mul(obj_work.spd_slope << 1, AppMain.mtMathSin((int)num4)), obj_work.spd_slope_max);
                        }
                    }
                    else
                    {
                        obj_work.spd_m = 0;
                    }
                }
                if ((obj_work.move_flag & 32768U) == 0U)
                {
                    num = AppMain.FX_Mul(obj_work.spd_m, AppMain.mtMathCos((int)obj_work.dir.z));
                    num2 = AppMain.FX_Mul(obj_work.spd_m, AppMain.mtMathSin((int)obj_work.dir.z));
                }
            }
            if ((obj_work.move_flag & 67108864U) != 0U)
            {
                obj_work.move.x = AppMain.FX_Mul(obj_work.spd.x + num + x, AppMain.g_obj.speed);
                obj_work.move.y = AppMain.FX_Mul(obj_work.spd.y + num2 + y, AppMain.g_obj.speed);
            }
            else
            {
                obj_work.move.x = AppMain.FX_Mul(obj_work.spd.x + num + x + AppMain.g_obj.scroll[0], AppMain.g_obj.speed);
                obj_work.move.y = AppMain.FX_Mul(obj_work.spd.y + num2 + y + AppMain.g_obj.scroll[1], AppMain.g_obj.speed);
            }
            obj_work.move.z = AppMain.FX_Mul(obj_work.spd.z + num3 + obj_work.flow.z, AppMain.g_obj.speed);
            if ((obj_work.move_flag & 1U) != 0U)
            {
                AppMain.ObjObjectSpdDirFall(ref obj_work.move.x, ref obj_work.move.y, obj_work.dir_fall);
            }
            else
            {
                AppMain.ObjObjectSpdDirFall(ref obj_work.move.x, ref obj_work.move.y, gms_PLAYER_WORK.jump_pseudofall_dir);
            }
        }
        obj_work.pos.x = obj_work.pos.x + obj_work.move.x;
        obj_work.pos.y = obj_work.pos.y + obj_work.move.y;
        obj_work.pos.z = obj_work.pos.z + obj_work.move.z;
        obj_work.spd.x = obj_work.spd.x + obj_work.spd_add.x;
        obj_work.spd.y = obj_work.spd.y + obj_work.spd_add.y;
        obj_work.spd.z = obj_work.spd.z + obj_work.spd_add.z;
        obj_work.flow.x = 0;
        obj_work.flow.y = 0;
        obj_work.flow.z = 0;
    }

    // Token: 0x06001782 RID: 6018 RVA: 0x000CFF84 File Offset: 0x000CE184
    public static void gmPlySeqSplJumpDirec(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.dir.z = AppMain.ObjRoopMove16(ply_work.obj_work.dir.z, (ushort)(ply_work.jump_pseudofall_dir - ply_work.obj_work.dir_fall), 512);
        if ((ply_work.gmk_flag & 536875264U) == 0U)
        {
            ply_work.obj_work.pos.z = (int)((long)AppMain.ObjSpdDownSet(ply_work.obj_work.pos.z, 16384) & -4096);
            ply_work.obj_work.spd.z = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.z, 512);
            ply_work.obj_work.dir.x = AppMain.ObjRoopMove16(ply_work.obj_work.dir.x, 0, 1024);
        }
    }

    // Token: 0x06001783 RID: 6019 RVA: 0x000D0068 File Offset: 0x000CE268
    public static void GmPlySeqJumpDirec(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.dir.z = AppMain.ObjRoopMove16(ply_work.obj_work.dir.z, 0, 512);
        if ((ply_work.gmk_flag & 536875264U) == 0U)
        {
            ply_work.obj_work.pos.z = (int)((long)AppMain.ObjSpdDownSet(ply_work.obj_work.pos.z, 16384) & -4096);
            ply_work.obj_work.spd.z = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.z, 512);
            ply_work.obj_work.dir.x = AppMain.ObjRoopMove16(ply_work.obj_work.dir.x, 0, 1024);
        }
    }

    // Token: 0x06001784 RID: 6020 RVA: 0x000D013C File Offset: 0x000CE33C
    public static void GmPlySeqTruckJumpDirec(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.dir.z = AppMain.ObjRoopMove16(ply_work.obj_work.dir.z, (ushort)(ply_work.jump_pseudofall_dir - ply_work.obj_work.dir_fall), 512);
        if ((ply_work.gmk_flag & 536875264U) == 0U)
        {
            ply_work.obj_work.pos.z = (int)((long)AppMain.ObjSpdDownSet(ply_work.obj_work.pos.z, 16384) & -4096);
            ply_work.obj_work.spd.z = AppMain.ObjSpdDownSet(ply_work.obj_work.spd.z, 512);
            ply_work.obj_work.dir.x = AppMain.ObjRoopMove16(ply_work.obj_work.dir.x, 0, 1024);
        }
    }

    // Token: 0x06001785 RID: 6021 RVA: 0x000D021F File Offset: 0x000CE41F
    public static bool GmPlySeqCheckAcceptHoming(GMS_PLAYER_WORK ply_work)
    {
        return (ply_work.seq_state_data_tbl[ply_work.seq_state].accept_attr & 16U) != 0U && (ply_work.player_flag & 128U) == 0U;
    }

    // Token: 0x06001786 RID: 6022 RVA: 0x000D0249 File Offset: 0x000CE449
    public static void gmPlySeqCheckChangeSequence(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        if (GmPlayer.IsTransformSuperSonic(ply_work) && GmPlayer.KeyCheckTransformKeyPush(ply_work) && 0 <= ply_work.seq_state && ply_work.seq_state <= 21)
        {
            AppMain.GmPlySeqChangeTransformSuper(ply_work);
        }
        AppMain.gmPlySeqCheckChangeSequenceUserInput(ply_work);
    }

    // Token: 0x06001787 RID: 6023 RVA: 0x000D0284 File Offset: 0x000CE484
    public static bool gmPlySeqCheckChangeSequenceUserInput(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        AppMain.GMS_PLY_SEQ_STATE_DATA[] seq_state_data_tbl = ply_work.seq_state_data_tbl;
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 4U) != 0U)
        {
            if ((ply_work.player_flag & 262144U) != 0U)
            {
                AppMain.gmPlySeqCheckEndTruckWalk(ply_work);
            }
            else
            {
                AppMain.gmPlySeqCheckEndWalk(ply_work);
            }
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 1U) != 0U)
        {
            AppMain.gmPlySeqCheckTurn(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 2U) != 0U)
        {
            AppMain.gmPlySeqCheckDirectTurn(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 8U) != 0U)
        {
            AppMain.gmPlySeqCheckFall(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 32768U) != 0U)
        {
            AppMain.gmPlySeqCheckStagger(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 16U) != 0U)
        {
            AppMain.gmPlySeqCheckEndLookup(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 32U) != 0U)
        {
            AppMain.gmPlySeqCheckEndSquat(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 128U) != 0U)
        {
            AppMain.gmPlySeqCheckEndSpin(ply_work);
        }
        if ((seq_state_data_tbl[ply_work.seq_state].check_attr & 1024U) != 0U)
        {
            AppMain.gmPlySeqCheckEndWallPush(ply_work);
        }
        if (AppMain.GmPlySeqCheckAcceptHoming(ply_work))
        {
            if (ply_work.cursol_enemy_obj == null)
            {
                AppMain.GmPlyEfctCreateHomingCursol(ply_work);
                ply_work.cursol_enemy_obj = ply_work.enemy_obj;
            }
            if (AppMain.gmPlySeqCheckHoming(ply_work))
            {
                return true;
            }
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 4096U) != 0U && AppMain.gmPlySeqCheckSquatSpin(ply_work))
        {
            return true;
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 256U) != 0U && AppMain.gmPlySeqCheckSpin(ply_work))
        {
            return true;
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 512U) != 0U && AppMain.gmPlySeqCheckSpinDashAcc(ply_work))
        {
            return true;
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 2048U) != 0U && AppMain.gmPlySeqCheckPinballSpinDashAcc(ply_work))
        {
            return true;
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 8U) != 0U && AppMain.gmPlySeqCheckJump(ply_work))
        {
            return true;
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 4U) != 0U && AppMain.gmPlySeqCheckBrake(ply_work))
        {
            return true;
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 1024U) != 0U && AppMain.gmPlySeqCheckWallPush(ply_work))
        {
            return true;
        }
        if ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 2U) != 0U)
        {
            if ((ply_work.player_flag & 262144U) != 0U)
            {
                if (AppMain.gmPlySeqCheckTruckWalk(ply_work))
                {
                    return true;
                }
            }
            else if (AppMain.gmPlySeqCheckWalk(ply_work))
            {
                return true;
            }
        }
        return ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 64U) != 0U && AppMain.gmPlySeqCheckLookup(ply_work)) || ((seq_state_data_tbl[ply_work.seq_state].accept_attr & 128U) != 0U && AppMain.gmPlySeqCheckSquat(ply_work));
    }

    // Token: 0x06001788 RID: 6024 RVA: 0x000D0508 File Offset: 0x000CE708
    public static bool gmPlySeqCheckEndWalk(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd_m == 0 && ply_work.obj_work.spd.z == 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return true;
        }
        return false;
    }

    // Token: 0x06001789 RID: 6025 RVA: 0x000D0534 File Offset: 0x000CE734
    public static bool gmPlySeqCheckTurn(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.seq_state == 2)
        {
            if (((ply_work.obj_work.disp_flag & 1U) == 0U && GmPlayer.KeyCheckWalkRight(ply_work)) || ((ply_work.obj_work.disp_flag & 1U) != 0U && GmPlayer.KeyCheckWalkLeft(ply_work)))
            {
                return AppMain.GmPlySeqChangeSequence(ply_work, 2);
            }
        }
        else if ((((ply_work.obj_work.disp_flag & 1U) != 0U && GmPlayer.KeyCheckWalkRight(ply_work)) || ((ply_work.obj_work.disp_flag & 1U) == 0U && GmPlayer.KeyCheckWalkLeft(ply_work))) && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 16384)
        {
            return AppMain.GmPlySeqChangeSequence(ply_work, 2);
        }
        return false;
    }

    // Token: 0x0600178A RID: 6026 RVA: 0x000D05D0 File Offset: 0x000CE7D0
    public static bool gmPlySeqCheckDirectTurn(GMS_PLAYER_WORK ply_work)
    {
        if ((((ply_work.obj_work.disp_flag & 1U) != 0U && GmPlayer.KeyCheckWalkRight(ply_work)) || ((ply_work.obj_work.disp_flag & 1U) == 0U && GmPlayer.KeyCheckWalkLeft(ply_work))) && ((ply_work.obj_work.move_flag & 16U) != 0U || ((ply_work.obj_work.move_flag & 16U) == 0U && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 16384)))
        {
            if (ply_work.act_state == 40 || ply_work.act_state == 48 || ply_work.act_state == 41 || ply_work.act_state == 42 || ply_work.act_state == 43)
            {
                AppMain.GmPlySeqSetFallTurn(ply_work);
            }
            else
            {
                AppMain.GmPlySeqSetProgramTurn(ply_work, 4096);
            }
            return true;
        }
        return false;
    }

    // Token: 0x0600178B RID: 6027 RVA: 0x000D0690 File Offset: 0x000CE890
    public static bool gmPlySeqCheckFall(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) == 0U)
        {
            return AppMain.GmPlySeqChangeSequence(ply_work, 16);
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            if ((ply_work.gmk_flag & 262144U) != 0U)
            {
                if ((ply_work.gmk_flag & 1073741824U) != 0U)
                {
                    if (ply_work.fall_timer != 0)
                    {
                        ply_work.fall_timer = AppMain.ObjTimeCountDown(ply_work.fall_timer);
                        return false;
                    }
                    ply_work.gmk_flag &= 3220963327U;
                    GmPlayer.SpdParameterSet(ply_work);
                    ply_work.jump_pseudofall_dir = AppMain.g_gm_main_system.pseudofall_dir;
                    ply_work.obj_work.pos.x = AppMain.FXM_FLOAT_TO_FX32(ply_work.truck_mtx_ply_mtn_pos.M03);
                    ply_work.obj_work.pos.y = AppMain.FXM_FLOAT_TO_FX32(-ply_work.truck_mtx_ply_mtn_pos.M13);
                    ply_work.obj_work.pos.z = AppMain.FXM_FLOAT_TO_FX32(ply_work.truck_mtx_ply_mtn_pos.M23);
                    AppMain.GmPlySeqChangeDeath(ply_work);
                    ply_work.gmk_flag2 |= 64U;
                    return true;
                }
            }
            else
            {
                ushort num = (ushort)(ply_work.obj_work.dir.z + (ply_work.obj_work.dir_fall - AppMain.g_gm_main_system.pseudofall_dir));
                if (((num + 16384 & 32768) != 0 || num == 49152) && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 8192)
                {
                    if (ply_work.fall_timer != 0)
                    {
                        ply_work.fall_timer = AppMain.ObjTimeCountDown(ply_work.fall_timer);
                        return false;
                    }
                    GmPlayer.SpdParameterSet(ply_work);
                    return AppMain.GmPlySeqChangeSequence(ply_work, 16);
                }
                else
                {
                    GmPlayer.SpdParameterSet(ply_work);
                }
            }
        }
        else if (ply_work.fall_timer != 0)
        {
            ply_work.fall_timer = AppMain.ObjTimeCountDown(ply_work.fall_timer);
        }
        else if (((ply_work.obj_work.dir.z + 16384 & 32768) != 0 || ply_work.obj_work.dir.z == 49152) && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 8192)
        {
            GmPlayer.SpdParameterSet(ply_work);
            return AppMain.GmPlySeqChangeSequence(ply_work, 16);
        }
        return false;
    }

    // Token: 0x0600178C RID: 6028 RVA: 0x000D08A4 File Offset: 0x000CEAA4
    public static bool gmPlySeqCheckStagger(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.dir.z & 32767) != 0 || ply_work.obj_work.ride_obj != null)
        {
            return false;
        }
        AppMain.OBS_COL_CHK_DATA obs_COL_CHK_DATA = GlobalPool<AppMain.OBS_COL_CHK_DATA>.Alloc();
        obs_COL_CHK_DATA.pos_x = ply_work.obj_work.pos.x >> 12;
        obs_COL_CHK_DATA.pos_y = (ply_work.obj_work.pos.y >> 12) + (int)ply_work.obj_work.field_rect[3];
        obs_COL_CHK_DATA.flag = (ushort)(ply_work.obj_work.flag & 1U);
        obs_COL_CHK_DATA.vec = 2;
        obs_COL_CHK_DATA.dir = null;
        obs_COL_CHK_DATA.attr = null;
        int num = AppMain.ObjDiffCollision(obs_COL_CHK_DATA);
        if (num > 0)
        {
            obs_COL_CHK_DATA.pos_x = (ply_work.obj_work.pos.x >> 12) + (int)ply_work.obj_work.field_rect[0] - 2;
            int num2 = AppMain.ObjDiffCollision(obs_COL_CHK_DATA);
            obs_COL_CHK_DATA.pos_x = (ply_work.obj_work.pos.x >> 12) + (int)ply_work.obj_work.field_rect[2] + 2;
            int num3 = AppMain.ObjDiffCollision(obs_COL_CHK_DATA);
            if (num2 <= 0 && num3 >= 16)
            {
                obs_COL_CHK_DATA.pos_x = (ply_work.obj_work.pos.x >> 12) + (int)ply_work.obj_work.field_rect[0] - -4;
                num2 = AppMain.ObjDiffCollision(obs_COL_CHK_DATA);
                if ((ply_work.obj_work.disp_flag & 1U) != 0U)
                {
                    GlobalPool<AppMain.OBS_COL_CHK_DATA>.Release(obs_COL_CHK_DATA);
                    return AppMain.GmPlySeqChangeSequence(ply_work, 14);
                }
                GlobalPool<AppMain.OBS_COL_CHK_DATA>.Release(obs_COL_CHK_DATA);
                if (num2 > 0)
                {
                    return AppMain.GmPlySeqChangeSequence(ply_work, 15);
                }
                return AppMain.GmPlySeqChangeSequence(ply_work, 13);
            }
            else if (num2 >= 16 && num3 <= 0)
            {
                obs_COL_CHK_DATA.pos_x = (ply_work.obj_work.pos.x >> 12) + (int)ply_work.obj_work.field_rect[2] + -4;
                num3 = AppMain.ObjDiffCollision(obs_COL_CHK_DATA);
                if ((ply_work.obj_work.disp_flag & 1U) == 0U)
                {
                    GlobalPool<AppMain.OBS_COL_CHK_DATA>.Release(obs_COL_CHK_DATA);
                    return AppMain.GmPlySeqChangeSequence(ply_work, 14);
                }
                GlobalPool<AppMain.OBS_COL_CHK_DATA>.Release(obs_COL_CHK_DATA);
                if (num3 > 0)
                {
                    return AppMain.GmPlySeqChangeSequence(ply_work, 15);
                }
                return AppMain.GmPlySeqChangeSequence(ply_work, 13);
            }
        }
        GlobalPool<AppMain.OBS_COL_CHK_DATA>.Release(obs_COL_CHK_DATA);
        return false;
    }

    // Token: 0x0600178D RID: 6029 RVA: 0x000D0AA4 File Offset: 0x000CECA4
    public static bool gmPlySeqCheckEndLookup(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) == 0U)
        {
            return AppMain.GmPlySeqChangeSequence(ply_work, 16);
        }
        return (ply_work.key_on & 1) == 0 && AppMain.GmPlySeqChangeSequence(ply_work, 5);
    }

    // Token: 0x0600178E RID: 6030 RVA: 0x000D0AD1 File Offset: 0x000CECD1
    public static bool gmPlySeqCheckEndSquat(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) == 0U)
        {
            return AppMain.GmPlySeqChangeSequence(ply_work, 16);
        }
        return (ply_work.key_on & 2) == 0 && AppMain.GmPlySeqChangeSequence(ply_work, 8);
    }

    // Token: 0x0600178F RID: 6031 RVA: 0x000D0B00 File Offset: 0x000CED00
    public static bool gmPlySeqCheckEndSpin(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd_m < 2048 && ply_work.obj_work.spd_m > -2048)
        {
            ply_work.obj_work.spd_m = 0;
            GmPlayer.SpdParameterSet(ply_work);
            if ((ply_work.player_flag & 131072U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, 39);
                ply_work.obj_work.disp_flag |= 4U;
            }
            return AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
        return false;
    }

    // Token: 0x06001790 RID: 6032 RVA: 0x000D0B78 File Offset: 0x000CED78
    public static bool gmPlySeqCheckEndWallPush(GMS_PLAYER_WORK ply_work)
    {
        return ((ply_work.obj_work.move_flag & 4U) == 0U || ((ply_work.obj_work.disp_flag & 1U) != 0U && !GmPlayer.KeyCheckWalkLeft(ply_work)) || ((ply_work.obj_work.disp_flag & 1U) == 0U && !GmPlayer.KeyCheckWalkRight(ply_work))) && AppMain.GmPlySeqChangeSequence(ply_work, 0);
    }

    // Token: 0x06001791 RID: 6033 RVA: 0x000D0BCC File Offset: 0x000CEDCC
    public static bool gmPlySeqCheckHoming(GMS_PLAYER_WORK ply_work)
    {
        if (!GmPlayer.KeyCheckJumpKeyPush(ply_work) || ply_work.homing_timer != 0 || (ply_work.player_flag & 128U) != 0U || AppMain.GMM_MAIN_STAGE_IS_ENDING())
        {
            return false;
        }
        if (ply_work.enemy_obj != null)
        {
            return AppMain.GmPlySeqChangeSequence(ply_work, 19);
        }
        return AppMain.GmPlySeqChangeSequence(ply_work, 21);
    }

    // Token: 0x06001792 RID: 6034 RVA: 0x000D0C19 File Offset: 0x000CEE19
    public static bool gmPlySeqCheckSquatSpin(GMS_PLAYER_WORK ply_work)
    {
        return (ply_work.key_on & 2) != 0 && (ply_work.obj_work.spd_m > 2048 || ply_work.obj_work.spd_m < -2048) && AppMain.GmPlySeqChangeSequence(ply_work, 10);
    }

    // Token: 0x06001793 RID: 6035 RVA: 0x000D0C53 File Offset: 0x000CEE53
    public static bool gmPlySeqCheckSpin(GMS_PLAYER_WORK ply_work)
    {
        return (ply_work.obj_work.spd_m > 2048 || ply_work.obj_work.spd_m < -2048) && AppMain.GmPlySeqChangeSequence(ply_work, 10);
    }

    // Token: 0x06001794 RID: 6036 RVA: 0x000D0C83 File Offset: 0x000CEE83
    public static bool gmPlySeqCheckSpinDashAcc(GMS_PLAYER_WORK ply_work)
    {
        return GmPlayer.KeyCheckJumpKeyPush(ply_work) && AppMain.GmPlySeqChangeSequence(ply_work, 11);
    }

    // Token: 0x06001795 RID: 6037 RVA: 0x000D0C97 File Offset: 0x000CEE97
    public static bool gmPlySeqCheckPinballSpinDashAcc(GMS_PLAYER_WORK ply_work)
    {
        return (ply_work.key_on & 2) != 0 && GmPlayer.KeyCheckJumpKeyPush(ply_work) && AppMain.GmPlySeqChangeSequence(ply_work, 11);
    }

    // Token: 0x06001796 RID: 6038 RVA: 0x000D0CB5 File Offset: 0x000CEEB5
    public static bool gmPlySeqCheckJump(GMS_PLAYER_WORK ply_work)
    {
        return GmPlayer.KeyCheckJumpKeyPush(ply_work) && ((ply_work.obj_work.move_flag & 1U) != 0U || (ply_work.gmk_obj != null && (ply_work.gmk_flag & 16384U) != 0U)) && AppMain.GmPlySeqChangeSequence(ply_work, 17);
    }

    // Token: 0x06001797 RID: 6039 RVA: 0x000D0CF0 File Offset: 0x000CEEF0
    public static bool gmPlySeqCheckBrake(GMS_PLAYER_WORK ply_work)
    {
        return ply_work.seq_state != 9 && ((GmPlayer.KeyCheckWalkLeft(ply_work) && ply_work.obj_work.spd_m >= 16384) || (GmPlayer.KeyCheckWalkRight(ply_work) && ply_work.obj_work.spd_m <= -16384)) && AppMain.GmPlySeqChangeSequence(ply_work, 9);
    }

    // Token: 0x06001798 RID: 6040 RVA: 0x000D0D48 File Offset: 0x000CEF48
    public static bool gmPlySeqCheckWalk(GMS_PLAYER_WORK ply_work)
    {
        return (!AppMain.GmObjCheckMapLeftLimit(ply_work.obj_work, 14) || !GmPlayer.KeyCheckWalkLeft(ply_work)) && (!AppMain.GmObjCheckMapRightLimit(ply_work.obj_work, 14) || !GmPlayer.KeyCheckWalkRight(ply_work)) && (13 > ply_work.seq_state || ply_work.seq_state > 15 || (ply_work.obj_work.move_flag & 4U) == 0U || (((ply_work.obj_work.disp_flag & 1U) == 0U || !GmPlayer.KeyCheckWalkLeft(ply_work)) && ((ply_work.obj_work.disp_flag & 1U) != 0U || !GmPlayer.KeyCheckWalkRight(ply_work)))) && (ply_work.obj_work.spd_m != 0 || GmPlayer.KeyCheckWalkLeft(ply_work) || GmPlayer.KeyCheckWalkRight(ply_work)) && AppMain.GmPlySeqChangeSequence(ply_work, 1);
    }

    // Token: 0x06001799 RID: 6041 RVA: 0x000D0DFE File Offset: 0x000CEFFE
    public static bool gmPlySeqCheckLookup(GMS_PLAYER_WORK ply_work)
    {
        return ply_work.obj_work.spd_m == 0 && (ply_work.obj_work.move_flag & 1U) != 0U && (ply_work.key_on & 1) != 0 && AppMain.GmPlySeqChangeSequence(ply_work, 3);
    }

    // Token: 0x0600179A RID: 6042 RVA: 0x000D0E31 File Offset: 0x000CF031
    public static bool gmPlySeqCheckSquat(GMS_PLAYER_WORK ply_work)
    {
        return (ply_work.key_on & 2) != 0 && AppMain.GmPlySeqChangeSequence(ply_work, 7);
    }

    // Token: 0x0600179B RID: 6043 RVA: 0x000D0E48 File Offset: 0x000CF048
    public static bool gmPlySeqCheckWallPush(GMS_PLAYER_WORK ply_work)
    {
        return (ply_work.obj_work.move_flag & 4U) != 0U && (ply_work.player_flag & 32768U) == 0U && (((ply_work.obj_work.disp_flag & 1U) != 0U && GmPlayer.KeyCheckWalkLeft(ply_work)) || ((ply_work.obj_work.disp_flag & 1U) == 0U && GmPlayer.KeyCheckWalkRight(ply_work))) && ply_work.obj_work.pos.x >> 12 > AppMain.g_gm_main_system.map_fcol.left + 14 && ply_work.obj_work.pos.x >> 12 < AppMain.g_gm_main_system.map_fcol.right - 14 && AppMain.GmPlySeqChangeSequence(ply_work, 18);
    }

    // Token: 0x0600179C RID: 6044 RVA: 0x000D0F00 File Offset: 0x000CF100
    public static bool gmPlySeqCheckTruckWalk(GMS_PLAYER_WORK ply_work)
    {
        if ((AppMain.GmObjCheckMapLeftLimit(ply_work.obj_work, 14) && GmPlayer.KeyCheckWalkLeft(ply_work)) || (AppMain.GmObjCheckMapRightLimit(ply_work.obj_work, 14) && GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            return false;
        }
        if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) >= 64 || GmPlayer.KeyCheckWalkLeft(ply_work) || GmPlayer.KeyCheckWalkRight(ply_work))
        {
            return AppMain.GmPlySeqChangeSequence(ply_work, 1);
        }
        ply_work.obj_work.spd_m = 0;
        return false;
    }

    // Token: 0x0600179D RID: 6045 RVA: 0x000D0F76 File Offset: 0x000CF176
    public static bool gmPlySeqCheckEndTruckWalk(GMS_PLAYER_WORK ply_work)
    {
        if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 64 && ply_work.obj_work.spd.z == 0)
        {
            ply_work.obj_work.spd_m = 0;
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return true;
        }
        return false;
    }

    // Token: 0x0600179E RID: 6046 RVA: 0x000D0FB8 File Offset: 0x000CF1B8
    public static void gmPlySeqSplStgRollCtrl(GMS_PLAYER_WORK ply_work)
    {
        OBS_CAMERA obs_CAMERA = ObjCamera.Get(0);
        if ((AppMain.g_gm_main_system.game_flag & 17240600U) != 0U)
        {
            return;
        }
        int num = ply_work.key_rot_z;
        if ((AppMain.g_gs_main_sys_info.game_flag & 512U) != 0U)
        {
            num = num * 26 / 10;
            obs_CAMERA.roll += num;
        }
        else
        {
            int num2 = 66;
            ply_work.accel_counter--;
            if (ply_work.accel_counter <= 0)
            {
                ply_work.accel_counter = 0;
                int num3 = num - ply_work.prev_key_rot_z;
                num3 = num3 * 3 / 4;
                if (AppMain.MTM_MATH_ABS(num3) >= 16384)
                {
                    if ((num3 >= 0 || num <= 0) && (num3 <= 0 || num >= 0))
                    {
                        num3 /= 64;
                        ply_work.dir_vec_add = num3;
                        ply_work.accel_counter = 8;
                    }
                }
            }
            num /= num2;
            num += ply_work.dir_vec_add * ply_work.accel_counter;
            obs_CAMERA.roll += num;
        }
        if (ply_work.nudge_di_timer != 0)
        {
            ply_work.nudge_di_timer -= 1;
            return;
        }
        bool flag = false;
        if ((ply_work.key_push & 160) != 0)
        {
            flag = true;
        }
        if (flag)
        {
            AppMain.GMS_SPL_STG_WORK gms_SPL_STG_WORK = AppMain.GmSplStageGetWork();
            ply_work.nudge_di_timer = 30;
            ply_work.nudge_timer = 30;
            gms_SPL_STG_WORK.flag &= 4294967293U;
        }
    }

    // Token: 0x06000851 RID: 2129 RVA: 0x00048CEC File Offset: 0x00046EEC
    public static void GmPlySeqInitSpringJump(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, bool spd_clear, int no_jump_move_time, int fall_dir, bool t_cam_slow)
    {
        bool set_act = true;
        AppMain.GmPlySeqChangeSequenceState(ply_work, 29);
        if (spd_clear)
        {
            ply_work.obj_work.spd.x = (ply_work.obj_work.spd.y = 0);
            ply_work.obj_work.spd_add.x = (ply_work.obj_work.spd_add.y = 0);
            ply_work.obj_work.spd_m = 0;
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            if (fall_dir != -1)
            {
                ply_work.jump_pseudofall_dir = (ushort)fall_dir;
                int num = fall_dir - ply_work.ply_pseudofall_dir;
                if ((ushort)AppMain.MTM_MATH_ABS(num) > 32768)
                {
                    if (num < 0)
                    {
                        ply_work.ply_pseudofall_dir += 65536 + num;
                    }
                    else
                    {
                        ply_work.ply_pseudofall_dir += num - 65536;
                    }
                }
                else
                {
                    ply_work.ply_pseudofall_dir = fall_dir;
                }
                AppMain.g_gm_main_system.pseudofall_dir = (ushort)ply_work.ply_pseudofall_dir;
            }
            GmPlayer.SetAtk(ply_work);
            set_act = false;
            GmPlayer.ActionChange(ply_work, 40);
            ply_work.obj_work.disp_flag |= 4U;
        }
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spd_x, spd_y, set_act);
        if ((ply_work.player_flag & 262144U) != 0U && fall_dir != -1)
        {
            ply_work.gmk_flag2 |= 256U;
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            ply_work.obj_work.disp_flag &= 4294967294U;
            if (t_cam_slow)
            {
                ply_work.gmk_flag2 |= 32U;
            }
        }
        if ((ply_work.player_flag & 262144U) != 0U && fall_dir != -1)
        {
            ply_work.gmk_flag |= 16777216U;
        }
        if (no_jump_move_time > 0)
        {
            AppMain.GmPlySeqSetNoJumpMoveTime(ply_work, no_jump_move_time);
        }
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            GmSound.PlaySE("Lorry5");
        }
        else
        {
            GmSound.PlaySE("Spring");
        }
        AppMain.GMM_PAD_VIB_SMALL();
    }

    // Token: 0x06000852 RID: 2130 RVA: 0x00048EBC File Offset: 0x000470BC
    public static void GmPlySeqInitRockRideStart(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK com_work)
    {
        if (ply_work.gmk_obj == (OBS_OBJECT_WORK)com_work)
        {
            return;
        }
        AppMain.GmPlySeqChangeSequenceState(ply_work, 30);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.gmk_obj = com_work.obj_work;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickRockRidePush;
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        OBS_OBJECT_WORK gmk_obj = ply_work.gmk_obj;
        GmPlayer.ActionChange(ply_work, 17);
        obj_work.spd_m = 0;
        obj_work.spd.x = 0;
        obj_work.spd.y = 0;
        obj_work.spd.z = 0;
        obj_work.spd_add.x = 0;
        obj_work.spd_add.y = 0;
        obj_work.spd_add.z = 0;
        if (obj_work.pos.x < gmk_obj.pos.x)
        {
            obj_work.disp_flag &= 4294967294U;
            return;
        }
        obj_work.disp_flag |= 1U;
    }

    // Token: 0x06000853 RID: 2131 RVA: 0x00048FA0 File Offset: 0x000471A0
    public static void GmPlySeqInitRockRide(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK com_work)
    {
        OBS_OBJECT_WORK gmk_obj = ply_work.gmk_obj;
        if (gmk_obj == (OBS_OBJECT_WORK)com_work)
        {
            return;
        }
        AppMain.GmPlySeqChangeSequenceState(ply_work, 31);
        AppMain.GmPlySeqGmkInitGimmickDependInit(ply_work, com_work.obj_work, 0, 0, 0);
        ply_work.gmk_obj = com_work.obj_work;
        com_work.target_dp_dist = 229376;
        ply_work.player_flag |= 12U;
        ply_work.obj_work.move_flag |= 256U;
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        gmk_obj = ply_work.gmk_obj;
        if (obj_work.pos.y > gmk_obj.pos.y)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
            ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickRockRideStop;
        }
        else
        {
            ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickRockRide;
            GmPlayer.CameraOffsetSet(ply_work, 0, -48);
            GmCamera.AllowSet(10f, 30f, 0f);
        }
        GmPlayer.ActionChange(ply_work, 60);
        obj_work.disp_flag |= 4U;
        ply_work.gmk_flag |= 16384U;
        int v = AppMain.FX_Div(gmk_obj.pos.x - obj_work.pos.x, 229376);
        obj_work.spd_m = AppMain.FX_Mul(v, 15360) + gmk_obj.spd_m;
        if (gmk_obj.spd_m > 0)
        {
            obj_work.disp_flag &= 4294967294U;
            return;
        }
        obj_work.disp_flag |= 1U;
    }

    // Token: 0x06000854 RID: 2132 RVA: 0x00049110 File Offset: 0x00047310
    public static void GmPlySeqInitPulley(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK com_work)
    {
        if (ply_work.gmk_obj == (OBS_OBJECT_WORK)com_work)
        {
            return;
        }
        AppMain.GmPlySeqChangeSequenceState(ply_work, 32);
        com_work.obj_work.spd.x = ply_work.obj_work.spd.x;
        if ((ply_work.obj_work.move_flag & 16U) == 0U)
        {
            com_work.obj_work.spd.x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)ply_work.obj_work.dir.z));
        }
        com_work.obj_work.move_flag &= 4294967231U;
        AppMain.GmPlySeqGmkInitGimmickDependInit(ply_work, com_work.obj_work, 0, 163840, 0);
        com_work.target_dp_pos.x = 0;
        com_work.target_dp_pos.y = 163840;
        com_work.target_dp_pos.z = 0;
        ply_work.player_flag |= 12U;
        com_work.target_dp_dist = -163840;
        ply_work.obj_work.move_flag |= 256U;
        ply_work.obj_work.move_flag &= 4294967278U;
        ply_work.gmk_flag |= 16384U;
        GmPlayer.ActionChange(ply_work, 66);
        ply_work.obj_work.pos.Assign(com_work.obj_work.pos);
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.pos.y = obj_work.pos.y + 163840;
    }

    // Token: 0x06000855 RID: 2133 RVA: 0x00049280 File Offset: 0x00047480
    public static void GmPlySeqInitBreathing(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 33);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickBreathing;
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.spd_m = 0;
        obj_work.spd.x = 0;
        obj_work.spd.y = 0;
        obj_work.spd_add.x = 0;
        obj_work.spd_add.y = 0;
        if ((obj_work.move_flag & 1U) != 0U)
        {
            GmPlayer.ActionChange(ply_work, 62);
        }
        else
        {
            GmPlayer.ActionChange(ply_work, 68);
        }
        GmSound.PlaySE("Breathe");
    }

    // Token: 0x06000856 RID: 2134 RVA: 0x00049314 File Offset: 0x00047514
    public static void GmPlySeqInitDashPanel(GMS_PLAYER_WORK ply_work, uint type)
    {
        int[][] array = new int[4][];
        int[][] array2 = array;
        int num = 0;
        int[] array3 = new int[2];
        array3[0] = 55296;
        array2[num] = array3;
        int[][] array4 = array;
        int num2 = 1;
        int[] array5 = new int[2];
        array5[0] = -55296;
        array4[num2] = array5;
        array[2] = new int[]
        {
            default(int),
            -55296
        };
        array[3] = new int[]
        {
            default(int),
            -55296
        };
        int[][] array6 = array;
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 34);
        if ((ply_work.player_flag & 262144U) == 0U)
        {
            GmPlayer.ActionChange(ply_work, 27);
        }
        else if (type == 1U || type == 3U)
        {
            ply_work.gmk_flag |= 1048576U;
            GmPlayer.ActionChange(ply_work, 72);
        }
        else
        {
            ply_work.gmk_flag &= 1048576U;
            GmPlayer.ActionChange(ply_work, 71);
        }
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.obj_work.user_timer = 60;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainDashPanel;
        if ((ply_work.player_flag & 262144U) == 0U)
        {
            AppMain.GmPlySeqGmkSpdSet(ply_work, array6[(int)((UIntPtr)type)][0], array6[(int)((UIntPtr)type)][1]);
        }
        else
        {
            AppMain.GmPlySeqGmkTruckSpdSet(ply_work, array6[(int)((UIntPtr)type)][0], array6[(int)((UIntPtr)type)][1]);
        }
        ply_work.no_spddown_timer = 49152;
        ply_work.spd_work_max = ply_work.obj_work.spd_m;
        GmPlayer.SetAtk(ply_work);
        //GmSound.PlaySE("Spin");
        if ((ply_work.player_flag & 262144U) == 0U)
        {
            AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 1U);
            AppMain.GmPlyEfctCreateSpinDashCircleBlur(ply_work);
            AppMain.GmPlyEfctCreateTrail(ply_work, 1);
        }
        AppMain.GMM_PAD_VIB_SMALL();
    }

    // Token: 0x06000857 RID: 2135 RVA: 0x000494B0 File Offset: 0x000476B0
    public static void GmPlySeqInitTarzanRope(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK com_work)
    {
        if (ply_work.gmk_obj == (OBS_OBJECT_WORK)com_work)
        {
            return;
        }
        AppMain.GmPlySeqChangeSequenceState(ply_work, 35);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.gmk_obj = com_work.obj_work;
        ply_work.seq_func = null;
        ply_work.obj_work.move_flag &= 4294967102U;
        GmPlayer.ActionChange(ply_work, 63);
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.disp_flag |= 4U;
        ply_work.gmk_flag |= 16384U;
    }

    // Token: 0x06000858 RID: 2136 RVA: 0x00049534 File Offset: 0x00047734
    public static void GmPlySeqInitWaterSlider(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK com_work)
    {
        if (ply_work.gmk_obj == (OBS_OBJECT_WORK)com_work)
        {
            return;
        }
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 36);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.gmk_obj = com_work.obj_work;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainWaterSlider;
        ply_work.obj_work.move_flag &= 4294967279U;
        GmPlayer.ActionChange(ply_work, 65);
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.disp_flag |= 4U;
        AppMain.GmGmkWaterSliderCreateEffect();
    }

    // Token: 0x06000859 RID: 2137 RVA: 0x000495BC File Offset: 0x000477BC
    public static void GmPlySeqInitSpipe(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 37);
        if (ply_work.act_state != 26 && ply_work.act_state != 27)
        {
            GmSound.PlaySE("Spin");
        }
        if (ply_work.act_state != 27)
        {
            GmPlayer.ActionChange(ply_work, 27);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainSpipe;
        AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 1U);
        AppMain.GmPlyEfctCreateSpinDashCircleBlur(ply_work);
    }

    // Token: 0x0600085A RID: 2138 RVA: 0x00049649 File Offset: 0x00047849
    public static ushort GmPlySeqScrewCheck(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.seq_func == AppMain.gmPlySeqGmkScrewMain)
        {
            return 1;
        }
        return 0;
    }

    // Token: 0x0600085B RID: 2139 RVA: 0x00049668 File Offset: 0x00047868
    public static void GmPlySeqInitScrew(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK gmk_work, int pos_x, int pos_y, ushort flag)
    {
        if (AppMain.GmPlySeqScrewCheck(ply_work) != 0)
        {
            return;
        }
        AppMain.GmPlySeqChangeSequenceState(ply_work, 38);
        GmPlayer.WalkActionSet(ply_work);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 8208U;
        ply_work.gmk_flag |= 147456U;
        ply_work.gmk_obj = gmk_work.obj_work;
        ply_work.gmk_work0 = pos_x;
        ply_work.gmk_work1 = pos_y;
        ply_work.obj_work.user_work = (uint)flag;
        ply_work.obj_work.user_timer = 0;
        if ((ply_work.obj_work.user_work & (uint)AppMain.GMD_GMK_SCREW_EVE_FLAG_LEFT) != 0U)
        {
            if (ply_work.gmk_work0 > ply_work.obj_work.pos.x)
            {
                ply_work.obj_work.user_timer = ply_work.gmk_work0 - ply_work.obj_work.pos.x;
            }
        }
        else if (ply_work.gmk_work0 < ply_work.obj_work.pos.x)
        {
            ply_work.obj_work.user_timer = ply_work.obj_work.pos.x - ply_work.gmk_work0;
        }
        ply_work.gmk_work1 -= (int)ply_work.obj_work.field_rect[3] << 12;
        ply_work.seq_func = AppMain.gmPlySeqGmkScrewMain;
        ply_work.timer = 16;
    }

    // Token: 0x0600085C RID: 2140 RVA: 0x000497BC File Offset: 0x000479BC
    public static void GmPlySeqInitDemoFw(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 39);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.seq_func = null;
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            GmPlayer.ActionChange(ply_work, 69);
            ply_work.obj_work.disp_flag |= 4U;
            return;
        }
        GmPlayer.ActionChange(ply_work, 0);
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.disp_flag |= 4U;
    }

    // Token: 0x0600085D RID: 2141 RVA: 0x00049824 File Offset: 0x00047A24
    public static void GmPlySeqInitCannon(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK gmk_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 41);
        GmPlayer.ActionChange(ply_work, 26);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.obj_work.move_flag |= 512U;
        ply_work.obj_work.pos.x = gmk_work.obj_work.pos.x;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.x = 0;
        if (ply_work.obj_work.spd_add.y <= 0)
        {
            ply_work.obj_work.spd_add.y = 672;
        }
        ply_work.seq_func = AppMain.gmPlySeqGmkCannonWait;
        ply_work.gmk_obj = gmk_work.obj_work;
        ply_work.gmk_flag2 |= 134U;
        GmPlayer.SetDefInvincible(ply_work);
        ply_work.invincible_timer = 0;
    }

    // Token: 0x0600085E RID: 2142 RVA: 0x00049932 File Offset: 0x00047B32
    public static void GmPlySeqInitCannonShoot(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 42);
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spd_x, spd_y);
        GmPlayer.ActionChange(ply_work, 67);
        ply_work.obj_work.disp_flag |= 4U;
        GmPlayer.SetDefNormal(ply_work);
        GmPlayer.SetAtk(ply_work);
        AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
    }

    // Token: 0x0600085F RID: 2143 RVA: 0x00049974 File Offset: 0x00047B74
    public static void GmPlySeqInitStopper(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK gmk_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 40);
        if (ply_work.act_state != 26)
        {
            GmPlayer.ActionChange(ply_work, 26);
        }
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 16U;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.move_flag &= 4294967167U;
        ply_work.obj_work.flag |= 2U;
        ply_work.seq_func = AppMain.gmPlySeqGmkStopperMove;
        ply_work.gmk_obj = gmk_work.obj_work;
    }

    // Token: 0x06000860 RID: 2144 RVA: 0x00049A38 File Offset: 0x00047C38
    public static void GmPlySeqInitStopperEnd(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.move_flag |= 144U;
        ply_work.seq_func = AppMain.gmPlySeqGmkStopperEnd;
    }

    // Token: 0x06000861 RID: 2145 RVA: 0x00049A63 File Offset: 0x00047C63
    public static void GmPlySeqGmkInitUpBumper(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 43);
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spd_x, spd_y);
        GmSound.PlaySE("Spring");
    }

    // Token: 0x06000862 RID: 2146 RVA: 0x00049A80 File Offset: 0x00047C80
    public static void GmPlySeqGmkInitSeesaw(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK gmk_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 44);
        if (ply_work.act_state != 27)
        {
            GmPlayer.ActionChange(ply_work, 27);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.obj_work.move_flag &= 4294967167U;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.dir.z = 0;
        ply_work.seq_func = AppMain.gmPlySeqGmkSeesaw;
        ply_work.gmk_obj = gmk_work.obj_work;
        AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 1U);
        AppMain.GmPlyEfctCreateSpinDashCircleBlur(ply_work);
    }

    // Token: 0x06000863 RID: 2147 RVA: 0x00049B54 File Offset: 0x00047D54
    public static void GmPlySeqGmkInitSeesawEnd(GMS_PLAYER_WORK ply_work, int spdx, int spdy)
    {
        AppMain.GmPlySeqChangeSequence(ply_work, 16);
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spdx, spdy, false);
        ply_work.no_spddown_timer = 0;
        ply_work.gmk_obj = null;
        GmPlayer.ActionChange(ply_work, 26);
        ply_work.obj_work.disp_flag |= 4U;
        GmPlayer.SetAtk(ply_work);
    }

    // Token: 0x06000864 RID: 2148 RVA: 0x00049BA4 File Offset: 0x00047DA4
    public static void GmPlySeqGmkInitSpinFall(GMS_PLAYER_WORK ply_work, int spdx, int spdy)
    {
        ply_work.gmk_obj = null;
        AppMain.GmPlySeqChangeSequenceState(ply_work, 66);
        AppMain.GmPlySeqInitFallState(ply_work);
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spdx, spdy, false);
        ply_work.no_spddown_timer = 0;
        if (ply_work.act_state != 26)
        {
            GmPlayer.ActionChange(ply_work, 26);
            ply_work.obj_work.disp_flag |= 4U;
        }
        GmPlayer.SetAtk(ply_work);
        AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
    }

    // Token: 0x06000865 RID: 2149 RVA: 0x00049C08 File Offset: 0x00047E08
    public static void GmPlySeqInitPinball(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, int no_spddown_timer)
    {
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 45);
        GmPlayer.StateGimmickInit(ply_work);
        if (ply_work.act_state != 39)
        {
            GmPlayer.ActionChange(ply_work, 39);
            ply_work.obj_work.disp_flag |= 4U;
            AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        AppMain.GmPlySeqGmkSpdSet(ply_work, spd_x, spd_y);
        ply_work.obj_work.spd_add.x = 0;
        ply_work.obj_work.spd_add.y = 0;
        ply_work.obj_work.spd_add.z = 0;
        ply_work.obj_work.user_timer = 60;
        ply_work.no_spddown_timer = no_spddown_timer * 4096;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainPinball;
        GmPlayer.SetAtk(ply_work);
        GmSound.PlaySE("Spin");
    }

    // Token: 0x06000866 RID: 2150 RVA: 0x00049CE1 File Offset: 0x00047EE1
    public static void GmPlySeqInitPinballAir(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        AppMain.GmPlySeqInitPinballAir(ply_work, spd_x, spd_y, 5, false, 0);
    }

    // Token: 0x06000867 RID: 2151 RVA: 0x00049CEE File Offset: 0x00047EEE
    public static void GmPlySeqInitPinballAir(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, int no_move_time)
    {
        AppMain.GmPlySeqInitPinballAir(ply_work, spd_x, spd_y, no_move_time, false, 0);
    }

    // Token: 0x06000868 RID: 2152 RVA: 0x00049CFB File Offset: 0x00047EFB
    public static void GmPlySeqInitPinballAir(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, int no_move_time, bool flag_no_recover_homing)
    {
        AppMain.GmPlySeqInitPinballAir(ply_work, spd_x, spd_y, no_move_time, flag_no_recover_homing ? 1 : 0, 0);
    }

    // Token: 0x06000869 RID: 2153 RVA: 0x00049D0F File Offset: 0x00047F0F
    public static void GmPlySeqInitPinballAir(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, int no_move_time, bool flag_no_recover_homing, int no_spddown_timer)
    {
        AppMain.GmPlySeqInitPinballAir(ply_work, spd_x, spd_y, no_move_time, flag_no_recover_homing ? 1 : 0, no_spddown_timer);
    }

    // Token: 0x0600086A RID: 2154 RVA: 0x00049D24 File Offset: 0x00047F24
    public static void GmPlySeqInitPinballAir(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, int no_move_time, int flag_no_recover_homing, int no_spddown_timer)
    {
        uint num = 0U;
        if ((ply_work.rect_work[1].flag & 4U) != 0U)
        {
            num = 1U;
        }
        uint num2 = 0U;
        if ((ply_work.player_flag & 128U) != 0U)
        {
            num2 = 1U;
        }
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 46);
        GmPlayer.StateGimmickInit(ply_work);
        if (((long)flag_no_recover_homing & (long)((ulong)num2)) != 0L)
        {
            ply_work.player_flag |= 128U;
        }
        ply_work.obj_work.move_flag |= 32784U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.obj_work.move_flag |= 128U;
        ply_work.obj_work.flag &= 4294967293U;
        ply_work.player_flag |= 32U;
        ply_work.obj_work.spd_fall = AppMain.FX_Mul(ply_work.obj_work.spd_fall, AppMain.FX_F32_TO_FX32(1.1f));
        ply_work.obj_work.dir.y = 0;
        AppMain.GmPlySeqGmkSpdSet(ply_work, spd_x, spd_y);
        ply_work.obj_work.spd_add.x = 0;
        ply_work.obj_work.spd_add.y = 0;
        ply_work.obj_work.spd_add.z = 0;
        ply_work.obj_work.spd_m = 0;
        bool flag = false;
        if ((ply_work.obj_work.disp_flag & 4U) != 0U)
        {
            flag = true;
        }
        int num3 = ply_work.act_state;
        int num4 = num3;
        switch (num4)
        {
            case 0:
            case 1:
                break;
            default:
                switch (num4)
                {
                    case 8:
                    case 9:
                    case 10:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        goto IL_1E2;
                    default:
                        switch (num4)
                        {
                            case 41:
                                num3 = 40;
                                flag = true;
                                goto IL_1E2;
                            case 42:
                                goto IL_1E2;
                            case 43:
                                num3 = 42;
                                flag = true;
                                goto IL_1E2;
                            default:
                                goto IL_1E2;
                        }
                        break;
                }
                break;
        }
        num3 = 40;
        flag = true;
    IL_1E2:
        GmPlayer.ActionChange(ply_work, num3);
        if (flag)
        {
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.no_spddown_timer = no_spddown_timer * 4096;
        ply_work.obj_work.user_timer = no_move_time;
        ply_work.obj_work.user_flag = 1U;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainPinballAir;
        if (num != 0U)
        {
            GmPlayer.SetAtk(ply_work);
        }
        if ((ply_work.gmk_flag & 4096U) != 0U)
        {
            ply_work.obj_work.spd.z = ply_work.obj_work.spd.y;
            ply_work.obj_work.spd.y = 0;
            if (ply_work.obj_work.pos.z < 0)
            {
                ply_work.obj_work.spd.z = -ply_work.obj_work.spd.z;
            }
        }
    }

    // Token: 0x0600086B RID: 2155 RVA: 0x00049FE4 File Offset: 0x000481E4
    public static void GmPlySeqInitFlipper(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, AppMain.GMS_ENEMY_COM_WORK com_work)
    {
        if (ply_work.gmk_obj == (OBS_OBJECT_WORK)com_work)
        {
            return;
        }
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 47);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.gmk_obj = com_work.obj_work;
        if (ply_work.act_state != 39)
        {
            GmPlayer.ActionChange(ply_work, 39);
            ply_work.obj_work.disp_flag |= 4U;
            AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
        }
        ply_work.obj_work.spd.x = spd_x;
        ply_work.obj_work.spd.y = spd_y;
        ply_work.obj_work.spd.z = 0;
        ply_work.obj_work.spd_add.x = 0;
        ply_work.obj_work.spd_add.y = 0;
        ply_work.obj_work.spd_add.z = 0;
        ply_work.obj_work.move_flag &= 4294967166U;
        ply_work.obj_work.move_flag |= 33040U;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.dir.z = 0;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainFlipper;
        GmSound.PlaySE("Spin");
        if (ply_work.obj_work.spd.x > 0)
        {
            ply_work.obj_work.disp_flag &= 4294967294U;
            return;
        }
        if (ply_work.obj_work.spd.x < 0)
        {
            ply_work.obj_work.disp_flag |= 1U;
        }
    }

    // Token: 0x0600086C RID: 2156 RVA: 0x0004A168 File Offset: 0x00048368
    public static void GmPlySeqGmkInitForceSpin(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 51);
        GmPlayer.StateGimmickInit(ply_work);
        if (ply_work.act_state != 26 && ply_work.act_state != 27)
        {
            GmSound.PlaySE("Spin");
        }
        if (ply_work.act_state != 26)
        {
            GmPlayer.ActionChange(ply_work, 26);
            ply_work.obj_work.disp_flag |= 4U;
            AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 0U);
        }
        ply_work.seq_func = AppMain.gmPlySeqGmkMainForceSpin;
        ply_work.obj_work.user_timer = ply_work.obj_work.spd_m;
        ply_work.obj_work.user_flag = 0U;
        GmPlayer.SetAtk(ply_work);
        ply_work.obj_work.move_flag |= 193U;
        ply_work.gmk_obj = null;
    }

    // Token: 0x0600086D RID: 2157 RVA: 0x0004A230 File Offset: 0x00048430
    public static void GmPlySeqGmkInitForceSpinDec(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 52);
        GmPlayer.StateGimmickInit(ply_work);
        if (ply_work.act_state != 26 && ply_work.act_state != 27)
        {
            GmSound.PlaySE("Spin");
        }
        if (ply_work.act_state != 26)
        {
            GmPlayer.ActionChange(ply_work, 26);
            ply_work.obj_work.disp_flag |= 4U;
            AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 0U);
        }
        ply_work.seq_func = AppMain.gmPlySeqGmkMainForceSpinDec;
        ply_work.obj_work.user_timer = ply_work.obj_work.spd_m;
        ply_work.obj_work.user_flag = 1U;
        GmPlayer.SetAtk(ply_work);
        ply_work.obj_work.move_flag |= 193U;
        ply_work.gmk_obj = null;
    }

    // Token: 0x0600086E RID: 2158 RVA: 0x0004A2F8 File Offset: 0x000484F8
    public static void GmPlySeqGmkInitForceSpinFall(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 53);
        ply_work.obj_work.move_flag |= 32912U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainForceSpinFall;
        ply_work.obj_work.spd.x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)ply_work.obj_work.dir.z));
        ply_work.obj_work.spd.y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
        if ((ply_work.obj_work.user_flag & 1U) != 0U && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) > AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.y))
        {
            ply_work.obj_work.spd.y = ply_work.obj_work.spd.x >> 1;
            if (ply_work.obj_work.spd.y < 0)
            {
                ply_work.obj_work.spd.y = -ply_work.obj_work.spd.y;
            }
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.spd.x = obj_work.spd.x >> 1;
        }
    }

    // Token: 0x0600086F RID: 2159 RVA: 0x0004A460 File Offset: 0x00048660
    public static void GmPlySeqInitPinballCtpltHold(GMS_PLAYER_WORK ply_work, AppMain.GMS_ENEMY_COM_WORK gmk_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 48);
        if (ply_work.prev_seq_state != 51 && ply_work.prev_seq_state != 52)
        {
            GmPlayer.ActionChange(ply_work, 26);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag &= 4294967279U;
        ply_work.obj_work.move_flag &= 4294967167U;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.dir.z = 0;
        AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 0U);
        ply_work.seq_func = AppMain.gmPlySeqGmkMainPinballCtpltHold;
        ply_work.gmk_obj = gmk_work.obj_work;
    }

    // Token: 0x06000870 RID: 2160 RVA: 0x0004A534 File Offset: 0x00048734
    public static void GmPlySeqInitPinballCtplt(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, (spd_x == 0) ? 49 : 50);
        GmPlayer.ActionChange(ply_work, 26);
        ply_work.obj_work.disp_flag |= 4U;
        if (spd_x != 0)
        {
            ply_work.obj_work.move_flag &= 4294967279U;
            ply_work.seq_func = AppMain.gmPlySeqGmkMainPinballCtplt;
            if (spd_x > 0)
            {
                ply_work.obj_work.disp_flag &= 4294967294U;
            }
            else
            {
                ply_work.obj_work.disp_flag |= 1U;
            }
        }
        else
        {
            ply_work.obj_work.move_flag |= 144U;
            AppMain.GmPlySeqGmkSpdSet(ply_work, spd_x, spd_y);
            ply_work.obj_work.spd_m = 0;
            ply_work.obj_work.dir.z = 0;
            ply_work.seq_func = AppMain.gmPlySeqGmkMainPinballAir;
        }
        ply_work.obj_work.flag &= 4294967293U;
        GmPlayer.SetAtk(ply_work);
        ply_work.no_spddown_timer = 2457600;
        GmSound.PlaySE("Catapult");
        AppMain.GmPlyEfctCreateSpinJumpBlur(ply_work);
    }

    // Token: 0x06000871 RID: 2161 RVA: 0x0004A650 File Offset: 0x00048850
    public static void GmPlySeqInitMoveGear(GMS_PLAYER_WORK ply_work, OBS_OBJECT_WORK gmk_obj, bool cam_adjust)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 54);
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        ply_work.obj_work.dir.z = 0;
        AppMain.GmPlySeqGmkInitGimmickDependInit(ply_work, gmk_obj, 0, 0, 0);
        ply_work.player_flag |= 514U;
        ply_work.obj_work.user_flag = 1U;
        ply_work.obj_work.move_flag |= 257U;
        if (ply_work.obj_work.spd_m != 0)
        {
            GmPlayer.WalkActionSet(ply_work);
        }
        else if (ply_work.act_state != 0)
        {
            GmPlayer.ActionChange(ply_work, 0);
            ply_work.obj_work.disp_flag |= 4U;
        }
        if (cam_adjust)
        {
            GmPlayer.CameraOffsetSet(ply_work, 0, -48);
            GmCamera.AllowSet(0f, 0f, 0f);
        }
        ply_work.seq_func = AppMain.gmPlySeqGmkMainMoveGear;
    }

    // Token: 0x06000872 RID: 2162 RVA: 0x0004A728 File Offset: 0x00048928
    public static void GmPlySeqInitSteamPipeIn(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 57);
        GmPlayer.ActionChange(ply_work, 26);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 256U;
        ply_work.obj_work.move_flag &= 4294967167U;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.spd_m = 0;
        GmPlayer.SetDefInvincible(ply_work);
        ply_work.invincible_timer = 0;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainSteamPipe;
        ply_work.gmk_obj = null;
        ply_work.obj_work.user_timer = 0;
        AppMain.GmPlyEfctCreateSteamPipe(ply_work);
        AppMain.GmPlyEfctCreateSpinDashBlur(ply_work, 0U);
    }

    // Token: 0x06000873 RID: 2163 RVA: 0x0004A7FC File Offset: 0x000489FC
    public static void GmPlySeqInitSteamPipeOut(GMS_PLAYER_WORK ply_work, int spd_x)
    {
        ply_work.obj_work.move_flag &= 4294967039U;
        ply_work.obj_work.move_flag |= 128U;
        GmPlayer.SetDefNormal(ply_work);
        ply_work.obj_work.user_timer = 60;
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spd_x, 0);
        GmPlayer.ActionChange(ply_work, 26);
        GmPlayer.SetAtk(ply_work);
    }

    // Token: 0x06000874 RID: 2164 RVA: 0x0004A860 File Offset: 0x00048A60
    public static void GmPlySeqGmkInitPopSteamJump(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, int no_jump_move_time)
    {
        if (ply_work.seq_state != 58)
        {
            AppMain.GmPlySeqChangeSequenceState(ply_work, 58);
        }
        ply_work.obj_work.spd.x = (ply_work.obj_work.spd.y = 0);
        ply_work.obj_work.spd_add.x = (ply_work.obj_work.spd_add.y = 0);
        ply_work.obj_work.spd_m = 0;
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spd_x, spd_y);
        if (no_jump_move_time > 0)
        {
            AppMain.GmPlySeqSetNoJumpMoveTime(ply_work, no_jump_move_time);
        }
    }

    // Token: 0x06000875 RID: 2165 RVA: 0x0004A8E8 File Offset: 0x00048AE8
    public static void GmPlySeqInitDrainTank(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 55);
        GmPlayer.StateGimmickInit(ply_work);
        GmPlayer.ActionChange(ply_work, 26);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 32784U;
        ply_work.obj_work.move_flag &= 4294967166U;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.spd.z = 0;
        ply_work.obj_work.spd_add.x = 0;
        ply_work.obj_work.spd_add.y = 0;
        ply_work.obj_work.spd_add.z = 0;
        ply_work.seq_func = null;
    }

    // Token: 0x06000876 RID: 2166 RVA: 0x0004A9BC File Offset: 0x00048BBC
    public static void GmPlySeqInitDrainTankFall(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 56);
        GmPlayer.StateGimmickInit(ply_work);
        GmPlayer.ActionChange(ply_work, 26);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.obj_work.move_flag |= 32912U;
        ply_work.obj_work.move_flag &= 4294967294U;
        ply_work.obj_work.spd_add.x = 0;
        ply_work.obj_work.spd_add.y = 0;
        ply_work.obj_work.spd_add.z = 0;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainDrainTank;
    }

    // Token: 0x06000877 RID: 2167 RVA: 0x0004AA64 File Offset: 0x00048C64
    public static void GmPlySeqInitSplIn(GMS_PLAYER_WORK ply_work, AppMain.VecFx32 pos)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 59);
        GmPlayer.StateGimmickInit(ply_work);
        if (ply_work.act_state != 26)
        {
            GmPlayer.ActionChange(ply_work, 39);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag |= 41232U;
        ply_work.obj_work.move_flag &= 4294967166U;
        ply_work.obj_work.pos.x = pos.x;
        ply_work.obj_work.pos.y = pos.y;
        ply_work.seq_func = null;
        AppMain.g_gm_main_system.game_flag |= 16384U;
    }

    // Token: 0x06000878 RID: 2168 RVA: 0x0004AB20 File Offset: 0x00048D20
    public static void GmPlySeqGmkInitBoss2Catch(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 60);
        if (ply_work.act_state != 39)
        {
            GmPlayer.ActionChange(ply_work, 39);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.move_flag |= 41232U;
        ply_work.obj_work.move_flag &= 4294967166U;
        ply_work.seq_func = null;
    }

    // Token: 0x06000879 RID: 2169 RVA: 0x0004AB90 File Offset: 0x00048D90
    public static void GmPlySeqGmkInitBoss5Quake(GMS_PLAYER_WORK ply_work, int no_move_time)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 61);
        if (ply_work.act_state != 34)
        {
            GmPlayer.ActionChange(ply_work, 34);
            ply_work.obj_work.disp_flag |= 4U;
        }
        ply_work.obj_work.spd.x = (ply_work.obj_work.spd.y = 0);
        ply_work.obj_work.spd_add.x = (ply_work.obj_work.spd_add.y = 0);
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.move_flag |= 40960U;
        ply_work.obj_work.user_timer = no_move_time;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainBoss5Quake;
    }

    // Token: 0x0600087A RID: 2170 RVA: 0x0004AC54 File Offset: 0x00048E54
    public static void GmPlySeqGmkInitEndingDemo1(GMS_PLAYER_WORK ply_work)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 62);
        GmPlayer.ActionChange(ply_work, 77);
        ply_work.seq_func = AppMain.gmPlySeqGmkMainEndingFrontSide;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.spd_add.x = 0;
        ply_work.obj_work.spd_add.y = 0;
    }

    // Token: 0x0600087B RID: 2171 RVA: 0x0004ACD4 File Offset: 0x00048ED4
    public static void GmPlySeqGmkInitEndingDemo2(GMS_PLAYER_WORK ply_work, bool type2)
    {
        AppMain.GmPlySeqChangeSequenceState(ply_work, 63);
        GmPlayer.ActionChange(ply_work, 39);
        ply_work.obj_work.disp_flag |= 4U;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainEndingFinish;
        ply_work.obj_work.spd.y = -10240;
        ply_work.obj_work.spd_add.y = 168;
        ply_work.obj_work.dir.y = 16384;
        ply_work.obj_work.user_work = 0U;
        ply_work.obj_work.user_flag = 0U;
        if (type2)
        {
            ply_work.obj_work.user_flag = 1U;
        }
        ply_work.obj_work.move_flag |= 33040U;
    }

    // Token: 0x0600087C RID: 2172 RVA: 0x0004AD94 File Offset: 0x00048F94
    public static void GmPlySeqGmkInitTruckDanger(GMS_PLAYER_WORK ply_work, OBS_OBJECT_WORK gmk_obj)
    {
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        AppMain.GmPlySeqChangeSequenceState(ply_work, 64);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.gmk_obj = gmk_obj;
        GmPlayer.SetDefInvincible(ply_work);
        ply_work.invincible_timer = 0;
        ply_work.player_flag &= 4294967280U;
        ply_work.gmk_flag &= 4293918719U;
        ply_work.gmk_flag |= 32768U;
        AppMain.nnMakeUnitMatrix(ply_work.ex_obj_mtx_r);
        int num = (int)(32768 - ply_work.obj_work.dir.z + (ushort)((short)(AppMain.g_gm_main_system.pseudofall_dir - ply_work.obj_work.dir_fall)));
        ply_work.gmk_work1 = 0;
        ply_work.gmk_work2 = 69632;
        ply_work.gmk_work3 = 0;
        ply_work.obj_work.user_work = 0U;
        uint num2 = GmPlayer.gmPlayerCheckTruckAirFoot(ply_work);
        if (ply_work.obj_work.dir.z <= 32768)
        {
            if ((num2 & 1U) != 0U)
            {
                num -= 6144;
                ply_work.obj_work.user_timer = 1024;
            }
            else
            {
                ply_work.player_flag |= 2U;
                GmPlayer.ActionChange(ply_work, 74);
            }
        }
        else if ((num2 & 2U) != 0U)
        {
            num += 6144;
            ply_work.player_flag |= 4U;
            ply_work.obj_work.user_timer = -1024;
        }
        else
        {
            ply_work.player_flag |= 2U;
            GmPlayer.ActionChange(ply_work, 74);
        }
        ply_work.gmk_work0 = (int)((ushort)(num / 17));
        ply_work.seq_func = AppMain.gmPlySeqGmkMainTruckDanger;
        GmSound.PlaySE("Lorry2");
    }

    // Token: 0x0600087D RID: 2173 RVA: 0x0004AF1C File Offset: 0x0004911C
    public static void GmPlySeqGmkInitTruckDangerRet(GMS_PLAYER_WORK ply_work, OBS_OBJECT_WORK gmk_obj)
    {
        int gmk_work = ply_work.gmk_work3;
        uint num = ply_work.player_flag & 13U;
        AppMain.GmPlySeqChangeSequenceState(ply_work, 64);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.player_flag |= num;
        ply_work.gmk_obj = gmk_obj;
        GmPlayer.ActionChange(ply_work, 76);
        ply_work.gmk_flag |= 32768U;
        int num2 = (int)(32768 - ply_work.obj_work.dir.z) - gmk_work + (int)((short)(AppMain.g_gm_main_system.pseudofall_dir - ply_work.obj_work.dir_fall));
        if (num2 > 0)
        {
            num2 = (int)((ushort)num2);
        }
        ply_work.gmk_work0 = -num2 / 14;
        ply_work.gmk_work1 = num2;
        ply_work.gmk_work2 = 0;
        ply_work.gmk_work3 = gmk_work;
        ply_work.obj_work.vib_timer = 0;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainTruckDangerRet;
    }

    // Token: 0x0600087E RID: 2174 RVA: 0x0004AFED File Offset: 0x000491ED
    public static void GmPlySeqGmkInitGmkJump(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, spd_x, spd_y, true);
    }

    // Token: 0x0600087F RID: 2175 RVA: 0x0004AFF8 File Offset: 0x000491F8
    public static void GmPlySeqGmkInitGmkJump(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y, bool set_act)
    {
        if ((ply_work.player_flag & 1024U) != 0U)
        {
            return;
        }
        GmPlayer.StateGimmickInit(ply_work);
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            ply_work.obj_work.spd.x = ply_work.obj_work.spd_m;
        }
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        if ((ply_work.player_flag & 262144U) != 0U)
        {
            AppMain.ObjObjectSpdDirFall(ref spd_x, ref spd_y, (ushort)-ply_work.jump_pseudofall_dir);
        }
        else
        {
            AppMain.ObjObjectSpdDirFall(ref spd_x, ref spd_y, ply_work.obj_work.dir_fall);
        }
        if ((ply_work.obj_work.move_flag & 16U) == 0U)
        {
            ply_work.camera_jump_pos_y = ply_work.obj_work.pos.y;
        }
        ply_work.obj_work.move_flag |= 32784U;
        ply_work.obj_work.move_flag &= 4294967294U;
        if (spd_x != 0)
        {
            if (set_act)
            {
                GmPlayer.ActionChange(ply_work, 47);
            }
            ply_work.obj_work.spd.x = spd_x;
            if (ply_work.obj_work.spd.x < 0)
            {
                if (ply_work.obj_work.spd_m > 0)
                {
                    ply_work.obj_work.spd_m = 0;
                }
                ply_work.obj_work.disp_flag |= 1U;
            }
            else
            {
                if (ply_work.obj_work.spd_m < 0)
                {
                    ply_work.obj_work.spd_m = 0;
                }
                ply_work.obj_work.disp_flag &= 4294967294U;
            }
            ply_work.no_spddown_timer = 262144;
        }
        else
        {
            if (set_act)
            {
                GmPlayer.ActionChange(ply_work, 44);
                ply_work.obj_work.disp_flag |= 4U;
            }
            ply_work.obj_work.spd.x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)ply_work.obj_work.dir.z));
        }
        if (spd_y != 0)
        {
            ply_work.obj_work.spd.y = spd_y;
        }
        else
        {
            ply_work.obj_work.spd.y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
        }
        ply_work.obj_work.user_timer = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.timer = 0;
        AppMain.GmPlySeqSetJumpState(ply_work, 0, 3U);
        if ((ply_work.player_flag & 67108864U) != 0U)
        {
            GmPlayer.GMD_PLAYER_WATERJUMP_SET(ref ply_work.obj_work.spd.x);
            GmPlayer.GMD_PLAYER_WATERJUMP_SET(ref ply_work.obj_work.spd.y);
        }
    }

    // Token: 0x06000880 RID: 2176 RVA: 0x0004B270 File Offset: 0x00049470
    public static void GmPlySeqGmkInitGimmickDependInit(GMS_PLAYER_WORK ply_work, OBS_OBJECT_WORK gmk_obj, int ofst_x, int ofst_y, int ofst_z)
    {
        if (ply_work.gmk_obj == gmk_obj)
        {
            return;
        }
        GmPlayer.SpdParameterSet(ply_work);
        GmPlayer.StateGimmickInit(ply_work);
        ply_work.gmk_obj = gmk_obj;
        ply_work.obj_work.move_flag |= 40976U;
        ply_work.obj_work.move_flag &= 4294967103U;
        ply_work.obj_work.user_flag = 0U;
        ply_work.player_flag &= 4294967280U;
        ply_work.obj_work.spd.x = 0;
        ply_work.obj_work.spd.y = 0;
        ply_work.obj_work.spd_m = 0;
        ply_work.obj_work.user_work = 0U;
        ply_work.obj_work.user_timer = 0;
        ply_work.gmk_work0 = ofst_x;
        ply_work.gmk_work1 = ofst_y;
        ply_work.gmk_work2 = ofst_z;
        ply_work.seq_func = AppMain.GmPlySeqGmkMainGimmickDepend;
    }

    // Token: 0x06000881 RID: 2177 RVA: 0x0004B354 File Offset: 0x00049554
    public static void GmPlySeqGmkMainGimmickDepend(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obs_OBJECT_WORK = (OBS_OBJECT_WORK)ply_work;
        OBS_OBJECT_WORK gmk_obj = ply_work.gmk_obj;
        if (gmk_obj != null)
        {
            AppMain.GMS_ENEMY_COM_WORK gms_ENEMY_COM_WORK = (AppMain.GMS_ENEMY_COM_WORK)ply_work.gmk_obj;
            if ((gms_ENEMY_COM_WORK.enemy_flag & 15U) != 0U)
            {
                ply_work.gmk_obj = null;
                if ((gms_ENEMY_COM_WORK.enemy_flag & 2U) != 0U)
                {
                    obs_OBJECT_WORK.spd.x = gmk_obj.spd.x;
                    obs_OBJECT_WORK.spd.y = gmk_obj.spd.y;
                }
                else if ((gms_ENEMY_COM_WORK.enemy_flag & 4U) != 0U)
                {
                    obs_OBJECT_WORK.spd.x = gmk_obj.spd_m;
                }
                else if ((gms_ENEMY_COM_WORK.enemy_flag & 8U) != 0U)
                {
                    obs_OBJECT_WORK.spd.x = obs_OBJECT_WORK.move.x;
                    obs_OBJECT_WORK.spd.y = obs_OBJECT_WORK.move.y;
                }
            }
            else
            {
                obs_OBJECT_WORK.prev_pos.x = obs_OBJECT_WORK.pos.x;
                obs_OBJECT_WORK.prev_pos.y = obs_OBJECT_WORK.pos.y;
                obs_OBJECT_WORK.prev_pos.z = obs_OBJECT_WORK.pos.z;
                if ((ply_work.player_flag & 1U) != 0U)
                {
                    obs_OBJECT_WORK.pos.Assign(gmk_obj.pos);
                }
                else if ((ply_work.player_flag & 2U) != 0U)
                {
                    obs_OBJECT_WORK.pos.x = gmk_obj.pos.x + gms_ENEMY_COM_WORK.target_dp_pos.x;
                    obs_OBJECT_WORK.pos.y = gmk_obj.pos.y + gms_ENEMY_COM_WORK.target_dp_pos.y;
                    obs_OBJECT_WORK.pos.z = gmk_obj.pos.z + gms_ENEMY_COM_WORK.target_dp_pos.z;
                }
                else if ((ply_work.player_flag & 4U) != 0U)
                {
                    NNS_MATRIX nns_MATRIX = GlobalPool<NNS_MATRIX>.Alloc();
                    NNS_VECTOR nns_VECTOR = new NNS_VECTOR(0f, -1f, 0f);
                    AppMain.nnMakeUnitMatrix(nns_MATRIX);
                    AppMain.nnRotateXYZMatrix(nns_MATRIX, nns_MATRIX, (int)(-(int)gms_ENEMY_COM_WORK.target_dp_dir.x), (int)gms_ENEMY_COM_WORK.target_dp_dir.y, (int)gms_ENEMY_COM_WORK.target_dp_dir.z);
                    AppMain.nnTransformVector(nns_VECTOR, nns_MATRIX, nns_VECTOR);
                    AppMain.nnScaleVector(nns_VECTOR, nns_VECTOR, AppMain.FXM_FX32_TO_FLOAT(gms_ENEMY_COM_WORK.target_dp_dist));
                    obs_OBJECT_WORK.pos.x = gmk_obj.pos.x + AppMain.FXM_FLOAT_TO_FX32(nns_VECTOR.x);
                    obs_OBJECT_WORK.pos.y = gmk_obj.pos.y + AppMain.FXM_FLOAT_TO_FX32(nns_VECTOR.y);
                    obs_OBJECT_WORK.pos.z = gmk_obj.pos.z + AppMain.FXM_FLOAT_TO_FX32(nns_VECTOR.z);
                    GlobalPool<NNS_MATRIX>.Release(nns_MATRIX);
                }
                if ((ply_work.player_flag & 8U) != 0U)
                {
                    obs_OBJECT_WORK.dir.Assign(gms_ENEMY_COM_WORK.target_dp_dir);
                }
                obs_OBJECT_WORK.move.x = obs_OBJECT_WORK.pos.x - obs_OBJECT_WORK.prev_pos.x;
                obs_OBJECT_WORK.move.y = obs_OBJECT_WORK.pos.y - obs_OBJECT_WORK.prev_pos.y;
                obs_OBJECT_WORK.move.z = obs_OBJECT_WORK.pos.z - obs_OBJECT_WORK.prev_pos.z;
                if ((obs_OBJECT_WORK.user_flag & 1U) != 0U && gmk_obj.vib_timer != 0)
                {
                    obs_OBJECT_WORK.vib_timer = gmk_obj.vib_timer + 4096;
                }
                if ((obs_OBJECT_WORK.move_flag & 8192U) != 0U)
                {
                    obs_OBJECT_WORK.flow.x = (obs_OBJECT_WORK.flow.y = (obs_OBJECT_WORK.flow.z = 0));
                }
            }
        }
        if (ply_work.gmk_obj == null)
        {
            GmPlayer.StateGimmickInit(ply_work);
        }
    }

    // Token: 0x06000882 RID: 2178 RVA: 0x0004B6D8 File Offset: 0x000498D8
    public static void GmPlySeqGmkSpdSet(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        if (spd_x < 0)
        {
            ply_work.obj_work.disp_flag |= 1U;
        }
        else if (spd_x > 0)
        {
            ply_work.obj_work.disp_flag &= 4294967294U;
        }
        if ((ply_work.obj_work.move_flag & 16U) != 0U)
        {
            if (((ply_work.obj_work.disp_flag & 1U) != 0U && ply_work.obj_work.spd.x > spd_x) || ((ply_work.obj_work.disp_flag & 1U) == 0U && ply_work.obj_work.spd.x < spd_x))
            {
                ply_work.obj_work.spd.x = spd_x;
            }
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.y) < AppMain.MTM_MATH_ABS(spd_y))
            {
                ply_work.obj_work.spd.y = spd_y;
                return;
            }
        }
        else
        {
            switch ((ply_work.obj_work.dir.z + 8192 & 49152) >> 14)
            {
                case 0:
                case 2:
                    if (((ply_work.obj_work.disp_flag & 1U) != 0U && ply_work.obj_work.spd_m > spd_x) || ((ply_work.obj_work.disp_flag & 1U) == 0U && ply_work.obj_work.spd_m < spd_x))
                    {
                        ply_work.obj_work.spd_m = spd_x;
                    }
                    if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.y) < AppMain.MTM_MATH_ABS(spd_y))
                    {
                        ply_work.obj_work.spd.y = spd_y;
                        if (ply_work.obj_work.spd.y < 0)
                        {
                            ply_work.obj_work.move_flag |= 16U;
                            return;
                        }
                    }
                    break;
                case 1:
                    if ((spd_y > 0 && ply_work.obj_work.spd_m < spd_y) || (spd_y < 0 && ply_work.obj_work.spd_m > spd_y))
                    {
                        ply_work.obj_work.spd_m = spd_y;
                    }
                    if (ply_work.obj_work.spd_m > 0)
                    {
                        ply_work.obj_work.disp_flag &= 4294967294U;
                    }
                    else
                    {
                        ply_work.obj_work.disp_flag |= 1U;
                    }
                    if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) < AppMain.MTM_MATH_ABS(spd_x))
                    {
                        ply_work.obj_work.spd.x = spd_x;
                        return;
                    }
                    break;
                case 3:
                    if ((spd_y > 0 && ply_work.obj_work.spd_m > -spd_y) || (spd_y < 0 && ply_work.obj_work.spd_m < -spd_y))
                    {
                        ply_work.obj_work.spd_m = -spd_y;
                    }
                    if (ply_work.obj_work.spd_m > 0)
                    {
                        ply_work.obj_work.disp_flag &= 4294967294U;
                    }
                    else
                    {
                        ply_work.obj_work.disp_flag |= 1U;
                    }
                    if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) < AppMain.MTM_MATH_ABS(spd_x))
                    {
                        ply_work.obj_work.spd.x = spd_x;
                    }
                    break;
                default:
                    return;
            }
        }
    }

    // Token: 0x06000883 RID: 2179 RVA: 0x0004B9BC File Offset: 0x00049BBC
    public static void GmPlySeqGmkTruckSpdSet(GMS_PLAYER_WORK ply_work, int spd_x, int spd_y)
    {
        if (spd_x < 0)
        {
            ply_work.gmk_flag |= 1048576U;
        }
        else if (spd_x > 0)
        {
            ply_work.gmk_flag &= 4293918719U;
        }
        if ((ply_work.obj_work.move_flag & 16U) != 0U)
        {
            if (((ply_work.obj_work.disp_flag & 1U) != 0U && ply_work.obj_work.spd.x > spd_x) || ((ply_work.obj_work.disp_flag & 1U) == 0U && ply_work.obj_work.spd.x < spd_x))
            {
                ply_work.obj_work.spd.x = spd_x;
            }
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.y) < AppMain.MTM_MATH_ABS(spd_y))
            {
                ply_work.obj_work.spd.y = spd_y;
                return;
            }
        }
        else
        {
            ushort num = (ushort)(ply_work.obj_work.dir.z + ply_work.obj_work.dir_fall);
            switch ((num + 8192 & 49152) >> 14)
            {
                case 0:
                case 2:
                    if (((ply_work.gmk_flag & 1048576U) != 0U && ply_work.obj_work.spd_m > spd_x) || ((ply_work.gmk_flag & 1048576U) == 0U && ply_work.obj_work.spd_m < spd_x))
                    {
                        ply_work.obj_work.spd_m = spd_x;
                    }
                    if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.y) < AppMain.MTM_MATH_ABS(spd_y))
                    {
                        ply_work.obj_work.spd.y = spd_y;
                        if (ply_work.obj_work.spd.y < 0)
                        {
                            ply_work.obj_work.move_flag |= 16U;
                            return;
                        }
                    }
                    break;
                case 1:
                    if ((spd_y > 0 && ply_work.obj_work.spd_m < spd_y) || (spd_y < 0 && ply_work.obj_work.spd_m > spd_y))
                    {
                        ply_work.obj_work.spd_m = spd_y;
                    }
                    if (ply_work.obj_work.spd_m > 0)
                    {
                        ply_work.gmk_flag &= 4293918719U;
                    }
                    else
                    {
                        ply_work.gmk_flag |= 1048576U;
                    }
                    if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) < AppMain.MTM_MATH_ABS(spd_x))
                    {
                        ply_work.obj_work.spd.x = spd_x;
                        return;
                    }
                    break;
                case 3:
                    if ((spd_y > 0 && ply_work.obj_work.spd_m > -spd_y) || (spd_y < 0 && ply_work.obj_work.spd_m < -spd_y))
                    {
                        ply_work.obj_work.spd_m = -spd_y;
                    }
                    if (ply_work.obj_work.spd_m > 0)
                    {
                        ply_work.gmk_flag &= 4293918719U;
                    }
                    else
                    {
                        ply_work.gmk_flag |= 1048576U;
                    }
                    if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd.x) < AppMain.MTM_MATH_ABS(spd_x))
                    {
                        ply_work.obj_work.spd.x = spd_x;
                    }
                    break;
                default:
                    return;
            }
        }
    }

    // Token: 0x06000884 RID: 2180 RVA: 0x0004BCA4 File Offset: 0x00049EA4
    public static void gmPlySeqGmkMainGimmickRockRidePush(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.obj_3d.speed[0] -= 0.02f;
        if (obj_work.obj_3d.speed[0] <= 0.5f)
        {
            obj_work.obj_3d.speed[0] = 0.5f;
        }
        if ((obj_work.disp_flag & 8U) != 0U)
        {
            obj_work.user_timer = 5;
            ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickRockRideStartWait;
            obj_work.obj_3d.speed[0] = 1f;
        }
    }

    // Token: 0x06000885 RID: 2181 RVA: 0x0004BD38 File Offset: 0x00049F38
    public static void gmPlySeqGmkMainGimmickRockRideStartWait(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.user_timer--;
        if (obj_work.user_timer > 0)
        {
            return;
        }
        obj_work.user_timer = 0;
        ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickRockRideStartJump;
    }

    // Token: 0x06000886 RID: 2182 RVA: 0x0004BD80 File Offset: 0x00049F80
    public static void gmPlySeqGmkMainGimmickRockRideStartJump(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        if ((obj_work.disp_flag & 8U) != 0U)
        {
            OBS_OBJECT_WORK gmk_obj = ply_work.gmk_obj;
            int num = 13824;
            if (gmk_obj.pos.x < obj_work.pos.x)
            {
                num = -num;
            }
            AppMain.GmPlySeqGmkInitGmkJump(ply_work, num, -24576);
            ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickRockRideStartFall;
        }
    }

    // Token: 0x06000887 RID: 2183 RVA: 0x0004BDE4 File Offset: 0x00049FE4
    public static void gmPlySeqGmkMainGimmickRockRideStartFall(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x06000888 RID: 2184 RVA: 0x0004BE04 File Offset: 0x0004A004
    public static void gmPlySeqGmkMainGimmickRockRide(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        OBS_OBJECT_WORK gmk_obj = ply_work.gmk_obj;
        if (gmk_obj.spd_m == 0)
        {
            ply_work.seq_func = AppMain.gmPlySeqGmkMainGimmickRockRideStop;
            GmPlayer.CameraOffsetSet(ply_work, 0, 0);
            GmCamera.AllowReset();
            return;
        }
        int num = GmPlayer.KeyGetGimmickRotZ(ply_work);
        int num2 = -num;
        float x = (float)num2 / AppMain.GMD_GMK_ROCK_RIDE_KEY_ANGLE_LIMIT;
        int num3 = AppMain.FX_Mul(61440, AppMain.FX_F32_TO_FX32(x));
        if (gmk_obj.spd_m > 0)
        {
            num3 += -32768;
        }
        else
        {
            num3 -= -32768;
        }
        int num4 = num3 - obj_work.spd_m;
        if (num4 > 0)
        {
            obj_work.spd_m += 384;
        }
        else if (num4 < 0)
        {
            obj_work.spd_m -= 384;
        }
        int num5 = gmk_obj.spd_m - obj_work.spd_m;
        int num6 = AppMain.MTM_MATH_ABS(num5);
        if (num6 >= 15360)
        {
            int num7 = 16384;
            if (num5 < 0)
            {
                num7 = -num7;
            }
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
            AppMain.GmPlySeqGmkInitGmkJump(ply_work, num7, 12288);
            GmPlayer.CameraOffsetSet(ply_work, 0, 0);
            GmCamera.AllowReset();
            return;
        }
        if (num6 >= 2816)
        {
            if (ply_work.act_state != 61)
            {
                GmPlayer.ActionChange(ply_work, 61);
                obj_work.disp_flag |= 4U;
            }
        }
        else if (ply_work.act_state != 60)
        {
            GmPlayer.ActionChange(ply_work, 60);
            obj_work.disp_flag |= 4U;
        }
        AppMain.GMS_ENEMY_COM_WORK gms_ENEMY_COM_WORK = (AppMain.GMS_ENEMY_COM_WORK)gmk_obj;
        gms_ENEMY_COM_WORK.target_dp_dir.z = (ushort)(num5 * 4 / 5);
        AppMain.GmPlySeqGmkMainGimmickDepend(ply_work);
    }

    // Token: 0x06000889 RID: 2185 RVA: 0x0004BF8C File Offset: 0x0004A18C
    public static void gmPlySeqGmkMainGimmickRockRideStop(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        AppMain.GmPlySeqGmkInitGmkJump(ply_work, obj_work.spd.x, obj_work.spd.y);
    }

    // Token: 0x0600088A RID: 2186 RVA: 0x0004BFBC File Offset: 0x0004A1BC
    public static void gmPlySeqGmkMainGimmickBreathing(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        if ((obj_work.disp_flag & 8U) != 0U)
        {
            if ((obj_work.move_flag & 1U) != 0U)
            {
                AppMain.GmPlySeqChangeSequence(ply_work, 0);
                return;
            }
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
        }
    }

    // Token: 0x0600088B RID: 2187 RVA: 0x0004BFFD File Offset: 0x0004A1FD
    public static void gmPlySeqGmkMainDashPanel(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.user_timer--;
        if (ply_work.obj_work.user_timer <= 0 || ply_work.obj_work.spd_m == 0)
        {
            GmPlayer.SpdParameterSet(ply_work);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x0600088C RID: 2188 RVA: 0x0004C03C File Offset: 0x0004A23C
    public static void gmPlySeqGmkMainWaterSlider(GMS_PLAYER_WORK ply_work)
    {
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        if ((obj_work.move_flag & 1U) == 0U)
        {
            AppMain.nnMakeUnitMatrix(ply_work.ex_obj_mtx_r);
            ply_work.gmk_flag &= 4294934527U;
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        if (GmPlayer.KeyCheckJumpKeyPush(ply_work))
        {
            AppMain.nnMakeUnitMatrix(ply_work.ex_obj_mtx_r);
            ply_work.gmk_flag &= 4294934527U;
            obj_work.spd_m /= 2;
            AppMain.GmPlySeqChangeSequence(ply_work, 17);
        }
    }

    // Token: 0x0600088D RID: 2189 RVA: 0x0004C0BC File Offset: 0x0004A2BC
    public static void gmPlySeqGmkMainSpipe(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.gmk_flag & 65536U) != 0U)
        {
            if ((ply_work.obj_work.move_flag & 1U) == 0U)
            {
                int num = AppMain.FX_Mul(40960, AppMain.mtMathCos((int)(ply_work.obj_work.dir.z - 16384)));
                OBS_OBJECT_WORK obj_work = ply_work.obj_work;
                obj_work.pos.x = obj_work.pos.x - num;
                num = AppMain.FX_Mul(40960, AppMain.mtMathSin((int)(ply_work.obj_work.dir.z - 16384)));
                OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
                obj_work2.pos.y = obj_work2.pos.y - num;
                ply_work.obj_work.spd.x = 0;
                ply_work.obj_work.spd.y = 0;
            }
            ply_work.obj_work.move_flag &= 4294934511U;
            if (ply_work.obj_work.spd_m == 0)
            {
                ply_work.obj_work.spd_m = 8192;
                GmSound.PlaySE("Spin");
            }
        }
        else
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 10);
        }
        ply_work.gmk_flag &= 4294901759U;
    }

    // Token: 0x0600088E RID: 2190 RVA: 0x0004C1E8 File Offset: 0x0004A3E8
    public static void gmPlySeqGmkScrewMain(GMS_PLAYER_WORK ply_work)
    {
        GmPlayer.WalkActionCheck(ply_work);
        AppMain.GMS_PLY_SEQ_STATE_DATA[] seq_state_data_tbl = ply_work.seq_state_data_tbl;
        if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < ply_work.spd2 && (ply_work.obj_work.move_flag & 1U) == 0U)
        {
            ply_work.obj_work.dir.x = (ply_work.obj_work.dir.y = (ply_work.obj_work.dir.z = 0));
            ply_work.obj_work.move_flag &= 4294959103U;
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
            return;
        }
        if (ply_work.timer != 0)
        {
            ply_work.timer--;
        }
        else if ((ply_work.obj_work.move_flag & 13U) != 0U)
        {
            ply_work.obj_work.dir.x = (ply_work.obj_work.dir.y = (ply_work.obj_work.dir.z = 0));
            ply_work.obj_work.move_flag &= 4294959103U;
            ply_work.obj_work.spd.x = ply_work.obj_work.spd_m;
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        AppMain.GmPlySeqMoveWalk(ply_work);
        AppMain.gmPlySeqGmkMoveScrew(ply_work, 1530320, 288, 38);
    }

    // Token: 0x0600088F RID: 2191 RVA: 0x0004C340 File Offset: 0x0004A540
    public static void gmPlySeqGmkMoveScrew(GMS_PLAYER_WORK ply_work, int screw_length, short screw_width, short screw_height)
    {
        ply_work.obj_work.user_timer += AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m);
        int num = ply_work.obj_work.user_timer;
        sbyte b = (sbyte)(num / screw_length);
        num -= screw_length * (int)b;
        int num2 = (num << 8) / screw_length << 4;
        ushort num3 = (ushort)(num2 << 4 & 65535);
        ply_work.obj_work.dir.x = num3;
        if ((ply_work.obj_work.user_work & (uint)AppMain.GMD_GMK_SCREW_EVE_FLAG_LEFT) != 0U)
        {
            ply_work.obj_work.dir.x = (ushort)-ply_work.obj_work.dir.x;
        }
        if (ply_work.obj_work.dir.x < 16384)
        {
            ply_work.obj_work.dir.z = ply_work.obj_work.dir.x;
        }
        else if (ply_work.obj_work.dir.x < 32768)
        {
            ply_work.obj_work.dir.z = (ushort)(16384 - (ply_work.obj_work.dir.x - 16384));
        }
        else if (ply_work.obj_work.dir.x < 49152)
        {
            ply_work.obj_work.dir.z = (ushort)(ply_work.obj_work.dir.x - 32768);
        }
        else
        {
            ply_work.obj_work.dir.z = (ushort)(65536 - (int)ply_work.obj_work.dir.x);
        }
        OBS_OBJECT_WORK obj_work = ply_work.obj_work;
        obj_work.dir.z = (ushort)(obj_work.dir.z >> 1);
        if (ply_work.obj_work.dir.x < 32768)
        {
            ply_work.obj_work.dir.z = (ushort)-ply_work.obj_work.dir.z;
        }
        ply_work.obj_work.dir.x = (ushort)-ply_work.obj_work.dir.x;
        int num4 = AppMain.mtMathCos((int)num3);
        screw_height -= ply_work.obj_work.field_rect[3];
        screw_height = (short)-screw_height;
        ply_work.obj_work.prev_pos.x = ply_work.obj_work.pos.x;
        ply_work.obj_work.prev_pos.y = ply_work.obj_work.pos.y;
        if ((ply_work.obj_work.user_work & (uint)AppMain.GMD_GMK_SCREW_EVE_FLAG_LEFT) != 0U)
        {
            ply_work.obj_work.pos.x = ply_work.gmk_work0 - ((int)((short)b * screw_width) << 12) - (int)screw_width * num2;
        }
        else
        {
            ply_work.obj_work.pos.x = ply_work.gmk_work0 + ((int)((short)b * screw_width) << 12) + (int)screw_width * num2;
        }
        ply_work.obj_work.pos.y = ply_work.gmk_work1 + ((int)screw_height << 12) - num4 * (int)screw_height;
        ply_work.obj_work.move.x = ply_work.obj_work.pos.x - ply_work.obj_work.prev_pos.x;
        ply_work.obj_work.move.y = ply_work.obj_work.pos.y - ply_work.obj_work.prev_pos.y;
    }

    // Token: 0x06000890 RID: 2192 RVA: 0x0004C678 File Offset: 0x0004A878
    public static void gmPlySeqGmkCannonWait(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.pos.y >= ply_work.gmk_obj.pos.y)
        {
            ply_work.obj_work.pos.y = ply_work.gmk_obj.pos.y;
            ply_work.obj_work.spd.y = 0;
            ply_work.obj_work.spd_add.y = 0;
            ply_work.obj_work.spd_add.y = 0;
            ply_work.obj_work.spd_fall = 0;
            ply_work.seq_func = null;
            ply_work.obj_work.move_flag &= 4294967167U;
        }
    }

    // Token: 0x06000891 RID: 2193 RVA: 0x0004C724 File Offset: 0x0004A924
    public static void gmPlySeqGmkStopperMove(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.pos.x = (ply_work.obj_work.pos.x + ply_work.gmk_obj.pos.x) / 2;
        int num = AppMain.MTM_MATH_ABS(ply_work.obj_work.pos.x - ply_work.gmk_obj.pos.x);
        if (num < 1024)
        {
            ply_work.obj_work.pos.x = ply_work.gmk_obj.pos.x;
        }
        if (ply_work.obj_work.pos.y > ply_work.gmk_obj.pos.y)
        {
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.pos.y = obj_work.pos.y - 32768;
            if (ply_work.obj_work.pos.y < ply_work.gmk_obj.pos.y)
            {
                ply_work.obj_work.pos.y = ply_work.gmk_obj.pos.y;
            }
        }
        else
        {
            OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
            obj_work2.pos.y = obj_work2.pos.y + 32768;
            if (ply_work.obj_work.pos.y > ply_work.gmk_obj.pos.y)
            {
                ply_work.obj_work.pos.y = ply_work.gmk_obj.pos.y;
            }
        }
        if (ply_work.obj_work.pos.x == ply_work.gmk_obj.pos.x && ply_work.obj_work.pos.y == ply_work.gmk_obj.pos.y)
        {
            ply_work.seq_func = AppMain.gmPlySeqGmkStopperWait;
        }
    }

    // Token: 0x06000892 RID: 2194 RVA: 0x0004C8EC File Offset: 0x0004AAEC
    public static void gmPlySeqGmkStopperWait(GMS_PLAYER_WORK ply_work)
    {
    }

    // Token: 0x06000893 RID: 2195 RVA: 0x0004C8F0 File Offset: 0x0004AAF0
    public static void gmPlySeqGmkStopperEnd(GMS_PLAYER_WORK ply_work)
    {
        bool flag = false;
        if (ply_work.gmk_obj == null)
        {
            flag = true;
        }
        else if (ply_work.gmk_obj.user_timer < ply_work.obj_work.pos.y >> 12)
        {
            flag = true;
        }
        if (flag)
        {
            int y = ply_work.obj_work.spd.y;
            AppMain.GmPlySeqChangeSequence(ply_work, 16);
            AppMain.GmPlySeqGmkInitGmkJump(ply_work, 0, y, false);
            ply_work.gmk_obj = null;
            if (ply_work.act_state != 26)
            {
                GmPlayer.ActionChange(ply_work, 26);
                ply_work.obj_work.disp_flag |= 4U;
            }
            ply_work.obj_work.flag &= 4294967293U;
        }
    }

    // Token: 0x06000894 RID: 2196 RVA: 0x0004C993 File Offset: 0x0004AB93
    public static void gmPlySeqGmkSeesaw(GMS_PLAYER_WORK ply_work)
    {
    }

    // Token: 0x06000895 RID: 2197 RVA: 0x0004C998 File Offset: 0x0004AB98
    public static void gmPlySeqGmkMainPinball(GMS_PLAYER_WORK ply_work)
    {
        ply_work.obj_work.user_timer--;
        if (ply_work.obj_work.user_timer <= 0 || ply_work.obj_work.spd_m == 0)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        if ((ply_work.obj_work.move_flag & 1U) == 0U)
        {
            int spd_x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)ply_work.obj_work.dir.z));
            int spd_y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
            AppMain.GmPlySeqInitPinballAir(ply_work, spd_x, spd_y);
        }
    }

    // Token: 0x06000896 RID: 2198 RVA: 0x0004CA40 File Offset: 0x0004AC40
    public static void gmPlySeqGmkMainPinballAir(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.user_timer > 0)
        {
            ply_work.obj_work.user_timer--;
        }
        if (ply_work.obj_work.user_timer <= 0 && ply_work.obj_work.user_flag != 0U)
        {
            ply_work.player_flag &= 4294967263U;
        }
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            ply_work.no_spddown_timer = 0;
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
        }
    }

    // Token: 0x06000897 RID: 2199 RVA: 0x0004CABD File Offset: 0x0004ACBD
    public static void gmPlySeqGmkMainPinballCtpltHold(GMS_PLAYER_WORK ply_work)
    {
    }

    // Token: 0x06000898 RID: 2200 RVA: 0x0004CAC0 File Offset: 0x0004ACC0
    public static void gmPlySeqGmkMainPinballCtplt(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) == 0U)
        {
            int spd_x = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathCos((int)ply_work.obj_work.dir.z));
            int spd_y = AppMain.FX_Mul(ply_work.obj_work.spd_m, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
            AppMain.GmPlySeqInitPinballAir(ply_work, spd_x, spd_y);
        }
    }

    // Token: 0x06000899 RID: 2201 RVA: 0x0004CB30 File Offset: 0x0004AD30
    public static void gmPlySeqGmkMainFlipper(GMS_PLAYER_WORK ply_work)
    {
        if (AppMain.MTM_MATH_ABS(ply_work.gmk_obj.pos.x - ply_work.obj_work.pos.x) > 221184 || AppMain.MTM_MATH_ABS(ply_work.gmk_obj.pos.y - ply_work.obj_work.pos.y) > 131072)
        {
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            ply_work.obj_work.move_flag |= 128U;
            ply_work.obj_work.move_flag &= 4294934271U;
        }
    }

    // Token: 0x0600089A RID: 2202 RVA: 0x0004CBD0 File Offset: 0x0004ADD0
    public static void gmPlySeqGmkMainForceSpin(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd_m == 0)
        {
            if ((ply_work.obj_work.disp_flag & 1U) != 0U)
            {
                ply_work.obj_work.spd_m = -8192;
            }
            else
            {
                ply_work.obj_work.spd_m = 8192;
            }
            GmSound.PlaySE("Spin");
        }
        if ((ply_work.obj_work.move_flag & 1U) == 0U)
        {
            AppMain.GmPlySeqGmkInitForceSpinFall(ply_work);
        }
    }

    // Token: 0x0600089B RID: 2203 RVA: 0x0004CC3C File Offset: 0x0004AE3C
    public static void gmPlySeqGmkMainForceSpinDec(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.spd_m == 0)
        {
            if ((ply_work.obj_work.disp_flag & 1U) != 0U)
            {
                ply_work.obj_work.spd_m = -8192;
            }
            else
            {
                ply_work.obj_work.spd_m = 8192;
            }
            GmSound.PlaySE("Spin");
        }
        if ((ply_work.obj_work.disp_flag & 1U) != 0U)
        {
            if (ply_work.obj_work.spd_m < -8192)
            {
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, -2048);
            }
        }
        else if (ply_work.obj_work.spd_m > 8192)
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, 2048);
        }
        if ((ply_work.obj_work.move_flag & 1U) == 0U)
        {
            AppMain.GmPlySeqGmkInitForceSpinFall(ply_work);
            ply_work.obj_work.spd_m = 0;
        }
    }

    // Token: 0x0600089C RID: 2204 RVA: 0x0004CD27 File Offset: 0x0004AF27
    public static void gmPlySeqGmkMainForceSpinFall(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            if ((ply_work.obj_work.user_flag & 1U) != 0U)
            {
                AppMain.GmPlySeqGmkInitForceSpinDec(ply_work);
                return;
            }
            AppMain.GmPlySeqGmkInitForceSpin(ply_work);
        }
    }

    // Token: 0x0600089D RID: 2205 RVA: 0x0004CD54 File Offset: 0x0004AF54
    public static void gmPlySeqGmkMainMoveGear(GMS_PLAYER_WORK ply_work)
    {
        bool flag = true;
        OBS_OBJECT_WORK gmk_obj = ply_work.gmk_obj;
        if (gmk_obj == null)
        {
            AppMain.GmPlySeqChangeFw(ply_work);
            return;
        }
        if ((gmk_obj.user_flag & 1U) == 0U && GmPlayer.KeyCheckJumpKeyPush(ply_work))
        {
            ply_work.obj_work.spd_m = 0;
            ply_work.obj_work.spd.x = (ply_work.obj_work.spd.y = 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 17);
            return;
        }
        ply_work.obj_work.move_flag |= 1U;
        if ((gmk_obj.user_flag & 2U) != 0U)
        {
            ply_work.obj_work.spd_m = 0;
            ply_work.obj_work.spd.x = (ply_work.obj_work.spd.y = 0);
        }
        if (ply_work.act_state != 8 && (((gmk_obj.user_flag & 2U) == 0U && ((GmPlayer.KeyCheckWalkLeft(ply_work) && (ply_work.obj_work.disp_flag & 1U) == 0U && ply_work.obj_work.spd_m <= 0) || (GmPlayer.KeyCheckWalkRight(ply_work) && (ply_work.obj_work.disp_flag & 1U) != 0U && ply_work.obj_work.spd_m >= 0))) || ((gmk_obj.user_flag & 1U) != 0U && (((ply_work.obj_work.disp_flag & 1U) == 0U && ply_work.obj_work.spd_m <= 0) || ((ply_work.obj_work.disp_flag & 1U) != 0U && ply_work.obj_work.spd_m >= 0))) || ((gmk_obj.user_flag & 2U) != 0U && gmk_obj.user_work == 7U && ((GmPlayer.KeyCheckWalkLeft(ply_work) && (ply_work.obj_work.disp_flag & 1U) == 0U && gmk_obj.user_timer <= 0) || (GmPlayer.KeyCheckWalkRight(ply_work) && (ply_work.obj_work.disp_flag & 1U) != 0U && gmk_obj.user_timer >= 0)))))
        {
            GmPlayer.ActionChange(ply_work, 8);
            AppMain.GmPlySeqSetProgramTurnFwTurn(ply_work);
        }
        else if (ply_work.act_state == 8)
        {
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                GmPlayer.SetReverseOnlyState(ply_work);
                GmPlayer.ActionChange(ply_work, 0);
                ply_work.obj_work.disp_flag |= 4U;
            }
        }
        else if (((gmk_obj.user_flag & 2U) != 0U && ((GmPlayer.KeyCheckWalkLeft(ply_work) && (ply_work.obj_work.disp_flag & 1U) != 0U) || (GmPlayer.KeyCheckWalkRight(ply_work) && (ply_work.obj_work.disp_flag & 1U) == 0U))) || gmk_obj.user_timer != 0)
        {
            if (ply_work.ring_num == 0 && (gmk_obj.user_work == 0U || gmk_obj.user_work == 4U))
            {
                if (ply_work.act_state != 33)
                {
                    GmPlayer.ActionChange(ply_work, 33);
                    ply_work.obj_work.obj_3d.blend_spd = 0.0625f;
                    ply_work.obj_work.disp_flag |= 4U;
                }
                ply_work.obj_work.obj_3d.speed[0] = 0.5f;
                ply_work.obj_work.obj_3d.speed[1] = 0.5f;
                flag = false;
            }
            else
            {
                if ((((gmk_obj.user_flag & 8U) == 0U && GmPlayer.KeyCheckWalkRight(ply_work)) || ((gmk_obj.user_flag & 8U) != 0U && GmPlayer.KeyCheckWalkLeft(ply_work))) && gmk_obj.user_work == 7U && gmk_obj.user_timer == 0)
                {
                    AppMain.GmPlySeqChangeFw(ply_work);
                    return;
                }
                if (gmk_obj.user_work == 1U || gmk_obj.user_work == 2U)
                {
                    if (ply_work.act_state != 60)
                    {
                        GmPlayer.ActionChange(ply_work, 60);
                        ply_work.obj_work.obj_3d.blend_spd = 0.0625f;
                        ply_work.obj_work.disp_flag |= 4U;
                    }
                }
                else if (ply_work.act_state != 20)
                {
                    GmPlayer.ActionChange(ply_work, 20);
                    ply_work.obj_work.disp_flag |= 4U;
                }
            }
            if (ply_work.act_state != 33)
            {
                flag = false;
                int num = gmk_obj.user_timer * 3;
                int num2 = AppMain.MTM_MATH_ABS((num >> 3) + (num >> 2));
                if (num2 <= 1024)
                {
                    num2 = 1024;
                }
                if (num2 >= 32768)
                {
                    num2 = 32768;
                }
                ply_work.obj_work.obj_3d.speed[0] = AppMain.FXM_FX32_TO_FLOAT(num2);
                ply_work.obj_work.obj_3d.speed[1] = AppMain.FXM_FX32_TO_FLOAT(num2);
            }
        }
        else if (ply_work.obj_work.spd_m != 0)
        {
            GmPlayer.WalkActionCheck(ply_work);
        }
        else if (ply_work.act_state != 0)
        {
            GmPlayer.ActionChange(ply_work, 0);
            ply_work.obj_work.disp_flag |= 4U;
        }
        if ((gmk_obj.user_flag & 3U) == 0U)
        {
            if ((gmk_obj.user_flag & 4U) != 0U)
            {
                AppMain.gmPlySeqGmkMoveGearMove(ply_work, true);
            }
            else
            {
                AppMain.gmPlySeqGmkMoveGearMove(ply_work, false);
            }
        }
        if (flag)
        {
            AppMain.gmPlySeqGmkMoveGearAnimeSpeedSetWalk(ply_work, ply_work.obj_work.spd_m);
        }
        AppMain.GmPlySeqGmkMainGimmickDepend(ply_work);
    }

    // Token: 0x0600089E RID: 2206 RVA: 0x0004D1C8 File Offset: 0x0004B3C8
    public static void gmPlySeqGmkMoveGearMove(GMS_PLAYER_WORK ply_work, bool spd_up_type)
    {
        int num;
        int num2;
        if (!spd_up_type)
        {
            if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 1024)
            {
                num = ply_work.spd_add >> 3;
                num2 = ply_work.spd_dec;
            }
            else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 2048)
            {
                num = ply_work.spd_add >> 2;
                num2 = (ply_work.spd_dec >> 1) + (ply_work.spd_dec >> 2);
            }
            else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 3072)
            {
                num = ply_work.spd_add >> 1;
                num2 = ply_work.spd_dec >> 1;
            }
            else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 4096)
            {
                num = (ply_work.spd_add >> 1) + (ply_work.spd_add >> 2);
                num2 = ply_work.spd_dec >> 2;
            }
            else
            {
                num = ply_work.spd_add;
                num2 = ply_work.spd_dec >> 3;
            }
        }
        else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 3072)
        {
            num = ply_work.spd_add >> 1;
            num2 = ply_work.spd_dec;
        }
        else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) < 4096)
        {
            num = (ply_work.spd_add >> 1) + (ply_work.spd_add >> 2);
            num2 = (ply_work.spd_dec >> 1) + (ply_work.spd_dec >> 2);
        }
        else
        {
            num = ply_work.spd_add;
            num2 = ply_work.spd_dec >> 1;
        }
        int num3 = (ply_work.spd_max >> 1) + (ply_work.spd_max >> 2);
        if (GmPlayer.KeyCheckWalkRight(ply_work) || GmPlayer.KeyCheckWalkLeft(ply_work))
        {
            int num4 = AppMain.MTM_MATH_ABS(ply_work.key_walk_rot_z);
            if (num4 > 24576)
            {
                num4 = 24576;
            }
            num3 = num3 * num4 / 24576;
        }
        if (num3 < ply_work.prev_walk_roll_spd_max)
        {
            num3 = ply_work.prev_walk_roll_spd_max - num2;
            if (num3 < 0)
            {
                num3 = 0;
            }
        }
        ply_work.prev_walk_roll_spd_max = num3;
        if (ply_work.obj_work.dir.z != 0)
        {
            int num5 = AppMain.FX_Mul(ply_work.spd_max_add_slope, AppMain.mtMathSin((int)ply_work.obj_work.dir.z));
            if (num5 > 0)
            {
                num3 += num5;
            }
        }
        if (ply_work.no_spddown_timer != 0)
        {
            num2 = 0;
        }
        else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) <= ply_work.spd1)
        {
            num = num * 5 / 8;
        }
        else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) <= ply_work.spd2)
        {
            num >>= 1;
        }
        else if (AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) > ply_work.spd3)
        {
            int num6;
            if (num3 - ply_work.spd3 != 0)
            {
                num6 = AppMain.FX_Div(AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) - ply_work.spd3, num3 - ply_work.spd3);
                if (num6 > 4096)
                {
                    num6 = 4096;
                }
            }
            else
            {
                num6 = 4096;
            }
            num6 = num6 * 3968 >> 12;
            num -= AppMain.FX_Mul(num, num6);
        }
        if (ply_work.spd_work_max >= num3 && AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m) >= num3)
        {
            if (ply_work.spd_work_max > ply_work.obj_work.spd_m)
            {
                ply_work.spd_work_max = AppMain.MTM_MATH_ABS(ply_work.obj_work.spd_m);
            }
            num3 = ply_work.spd_work_max;
        }
        if ((ply_work.player_flag & 32768U) != 0U && GmPlayer.KeyCheckWalkRight(ply_work) && num3 > ply_work.scroll_spd_x + 8192)
        {
            num3 = ply_work.scroll_spd_x + 8192;
        }
        if (!(GmPlayer.KeyCheckWalkLeft(ply_work) | GmPlayer.KeyCheckWalkRight(ply_work)))
        {
            ply_work.spd_pool = 0;
            ply_work.obj_work.spd.x = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd.x, -num3, num3);
            ply_work.obj_work.spd_m = AppMain.MTM_MATH_CLIP(ply_work.obj_work.spd_m, -num3, num3);
            if ((ply_work.obj_work.dir.z + 8192 & 65280) <= 16384)
            {
                if ((ply_work.player_flag & 134217728U) != 0U)
                {
                    ply_work.player_flag &= 4160749567U;
                    return;
                }
                if ((ply_work.player_flag & 32768U) != 0U)
                {
                    if ((ply_work.obj_work.disp_flag & 1U) != 0U || ply_work.seq_state != 1)
                    {
                        ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
                        return;
                    }
                    int num7 = ply_work.scroll_spd_x + -4096;
                    if (num7 < 0)
                    {
                        num7 = 0;
                    }
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
                    if (ply_work.obj_work.spd_m < num7)
                    {
                        ply_work.obj_work.spd_m = num7;
                        return;
                    }
                }
                else
                {
                    ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
                }
            }
            return;
        }
        if (GmPlayer.KeyCheckWalkRight(ply_work))
        {
            if (ply_work.obj_work.spd_m < 0)
            {
                ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
            }
            ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, num, num3);
            return;
        }
        if (ply_work.obj_work.spd_m > 0)
        {
            ply_work.obj_work.spd_m = AppMain.ObjSpdDownSet(ply_work.obj_work.spd_m, num2);
        }
        ply_work.obj_work.spd_m = AppMain.ObjSpdUpSet(ply_work.obj_work.spd_m, -num, num3);
    }

    // Token: 0x0600089F RID: 2207 RVA: 0x0004D708 File Offset: 0x0004B908
    public static void gmPlySeqGmkMoveGearAnimeSpeedSetWalk(GMS_PLAYER_WORK ply_work, int spd_set)
    {
        int num;
        if (19 <= ply_work.act_state && ply_work.act_state <= 21)
        {
            num = AppMain.MTM_MATH_ABS((spd_set >> 3) + (spd_set >> 2));
            if (num <= 2048)
            {
                num = 2048;
            }
            if (num >= 32768)
            {
                num = 32768;
            }
        }
        else
        {
            num = 4096;
        }
        if (ply_work.obj_work.obj_3d != null)
        {
            ply_work.obj_work.obj_3d.speed[0] = AppMain.FXM_FX32_TO_FLOAT(num);
            ply_work.obj_work.obj_3d.speed[1] = AppMain.FXM_FX32_TO_FLOAT(num);
        }
    }

    // Token: 0x060008A0 RID: 2208 RVA: 0x0004D798 File Offset: 0x0004B998
    public static void gmPlySeqGmkMainSteamPipe(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.user_timer < 245760)
        {
            ply_work.obj_work.user_timer = AppMain.ObjTimeCountUp(ply_work.obj_work.user_timer);
            float num = AppMain.FXM_FX32_TO_FLOAT(ply_work.obj_work.user_timer) / 60f * 2f;
            ply_work.obj_work.obj_3d.speed[0] = 1f + num;
            ply_work.obj_work.obj_3d.speed[1] = 1f + num;
        }
    }

    // Token: 0x060008A1 RID: 2209 RVA: 0x0004D824 File Offset: 0x0004BA24
    public static void gmPlySeqGmkMainDrainTank(GMS_PLAYER_WORK ply_work)
    {
        if ((ply_work.obj_work.move_flag & 1U) != 0U)
        {
            AppMain.GmPlySeqLandingSet(ply_work, 0);
            AppMain.GmPlySeqChangeSequence(ply_work, 0);
            return;
        }
        if (ply_work.obj_work.spd_add.x > 0)
        {
            if (ply_work.obj_work.spd.x >= 0)
            {
                ply_work.obj_work.spd_add.x = 0;
                ply_work.obj_work.spd.x = 0;
                return;
            }
        }
        else if (ply_work.obj_work.spd_add.x < 0 && ply_work.obj_work.spd.x <= 0)
        {
            ply_work.obj_work.spd_add.x = 0;
            ply_work.obj_work.spd.x = 0;
        }
    }

    // Token: 0x060008A2 RID: 2210 RVA: 0x0004D8E4 File Offset: 0x0004BAE4
    public static void gmPlySeqGmkMainBoss5Quake(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.obj_work.user_timer > 0)
        {
            ply_work.obj_work.user_timer--;
            return;
        }
        AppMain.GmPlySeqLandingSet(ply_work, 0);
        ply_work.obj_work.move_flag &= 4294959103U;
        AppMain.GmPlySeqChangeSequence(ply_work, 0);
    }

    // Token: 0x060008A3 RID: 2211 RVA: 0x0004D939 File Offset: 0x0004BB39
    public static void gmPlySeqGmkMainEndingFrontSide(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state == 77 && (ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            GmPlayer.ActionChange(ply_work, 78);
            ply_work.obj_work.disp_flag |= 4U;
        }
    }

    // Token: 0x060008A4 RID: 2212 RVA: 0x0004D970 File Offset: 0x0004BB70
    public static void gmPlySeqGmkMainEndingFinish(GMS_PLAYER_WORK ply_work)
    {
        if (ply_work.act_state != 80 && ply_work.act_state != 82 && ply_work.act_state != 84)
        {
            int user_work = (int)(ply_work.obj_work.user_work + 4U);
            ply_work.obj_work.user_work = (uint)user_work;
            OBS_OBJECT_WORK obj_work = ply_work.obj_work;
            obj_work.scale.x = obj_work.scale.x + (int)ply_work.obj_work.user_work;
            OBS_OBJECT_WORK obj_work2 = ply_work.obj_work;
            obj_work2.scale.y = obj_work2.scale.y + (int)ply_work.obj_work.user_work;
            OBS_OBJECT_WORK obj_work3 = ply_work.obj_work;
            obj_work3.scale.z = obj_work3.scale.z + (int)ply_work.obj_work.user_work;
            OBS_OBJECT_WORK obj_work4 = ply_work.obj_work;
            obj_work4.pos.z = obj_work4.pos.z + 1024;
        }
        if (ply_work.act_state != 39 || ply_work.obj_work.spd.y <= -4096)
        {
            if ((ply_work.act_state == 79 || ply_work.act_state == 81 || ply_work.act_state == 83) && (ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                if ((ply_work.player_flag & 16384U) != 0U)
                {
                    GmPlayer.ActionChange(ply_work, 84);
                }
                else if (ply_work.obj_work.user_flag != 0U)
                {
                    GmPlayer.ActionChange(ply_work, 82);
                }
                else
                {
                    GmPlayer.ActionChange(ply_work, 80);
                }
                ply_work.obj_work.move_flag |= 8192U;
                ply_work.obj_work.disp_flag |= 32U;
                AppMain.GmEndingTrophySet();
            }
            return;
        }
        if ((ply_work.player_flag & 16384U) != 0U)
        {
            GmPlayer.ActionChange(ply_work, 83);
            return;
        }
        if (ply_work.obj_work.user_flag != 0U)
        {
            GmPlayer.ActionChange(ply_work, 81);
            return;
        }
        GmPlayer.ActionChange(ply_work, 79);
    }

    // Token: 0x060008A6 RID: 2214 RVA: 0x0004DFE8 File Offset: 0x0004C1E8
    public static void gmPlySeqGmkMainTruckDanger(GMS_PLAYER_WORK ply_work)
    {
        NNS_MATRIX nns_MATRIX = GlobalPool<NNS_MATRIX>.Alloc();
        if ((ply_work.player_flag & 2U) == 0U)
        {
            if (AppMain.MTM_MATH_ABS(ply_work.gmk_work3) < 6144)
            {
                ply_work.gmk_work3 += ply_work.obj_work.user_timer;
                if ((ply_work.player_flag & 4U) == 0U)
                {
                    if (ply_work.gmk_work3 > 6144)
                    {
                        ply_work.gmk_work3 = 6144;
                    }
                    ply_work.obj_work.user_timer = AppMain.ObjSpdUpSet(ply_work.obj_work.user_timer, 32, 1024);
                }
                else
                {
                    if (ply_work.gmk_work3 < -6144)
                    {
                        ply_work.gmk_work3 = -6144;
                    }
                    ply_work.obj_work.user_timer = AppMain.ObjSpdUpSet(ply_work.obj_work.user_timer, -32, 1024);
                }
            }
            else
            {
                if (ply_work.act_state != 74 && ply_work.act_state != 75)
                {
                    GmPlayer.ActionChange(ply_work, 74);
                }
                if (ply_work.obj_work.user_work < 3U)
                {
                    int user_work = (int)(ply_work.obj_work.user_work + 1U);
                    ply_work.obj_work.user_work = (uint)user_work;
                    ply_work.obj_work.user_timer = -ply_work.obj_work.user_timer >> 1;
                    if (ply_work.gmk_work3 < 0)
                    {
                        ply_work.gmk_work3++;
                    }
                    else
                    {
                        ply_work.gmk_work3--;
                    }
                }
                else
                {
                    ply_work.player_flag |= 2U;
                }
            }
        }
        if (ply_work.act_state == 74)
        {
            ply_work.gmk_work2 = AppMain.ObjTimeCountDown(ply_work.gmk_work2);
            if (ply_work.gmk_work2 != 0)
            {
                ply_work.gmk_work1 = (int)((ushort)(ply_work.gmk_work1 + ply_work.gmk_work0));
            }
            if ((ply_work.obj_work.disp_flag & 8U) != 0U)
            {
                GmPlayer.ActionChange(ply_work, 75);
                ply_work.obj_work.disp_flag |= 4U;
                ply_work.gmk_flag |= 1073741824U;
                ply_work.obj_work.vib_timer = ply_work.fall_timer;
            }
        }
        else if ((ply_work.gmk_flag & 1073741824U) != 0U)
        {
            ply_work.gmk_work1 = (int)((ushort)((int)(32768 - ply_work.obj_work.dir.z) - ply_work.gmk_work3 + (int)((short)(AppMain.g_gm_main_system.pseudofall_dir - ply_work.obj_work.dir_fall))));
            if ((ply_work.player_flag & 1U) != 0U)
            {
                AppMain.GmPlySeqGmkInitTruckDangerRet(ply_work, ply_work.truck_obj);
            }
        }
        if ((ply_work.gmk_flag & 1073741824U) == 0U && ply_work.act_state == 75 && (ply_work.player_flag & 2U) != 0U)
        {
            ply_work.gmk_flag |= 1073741824U;
        }
        AppMain.nnMakeUnitMatrix(ply_work.ex_obj_mtx_r);
        AppMain.nnTranslateMatrix(ply_work.ex_obj_mtx_r, ply_work.ex_obj_mtx_r, 0f, 5f, 9f);
        AppMain.nnRotateXMatrix(ply_work.ex_obj_mtx_r, ply_work.ex_obj_mtx_r, ply_work.gmk_work1);
        AppMain.nnTranslateMatrix(ply_work.ex_obj_mtx_r, ply_work.ex_obj_mtx_r, 0f, -5f, -9f);
        float num;
        float num2;
        float num3;
        if ((ply_work.player_flag & 4U) == 0U)
        {
            num = 0f;
            num2 = 8f;
            num3 = -5f;
        }
        else
        {
            num = 0f;
            num2 = 8f;
            num3 = 5f;
        }
        AppMain.nnMakeUnitMatrix(nns_MATRIX);
        AppMain.nnTranslateMatrix(nns_MATRIX, nns_MATRIX, -num, -num2, -num3);
        AppMain.nnRotateXMatrix(nns_MATRIX, nns_MATRIX, ply_work.gmk_work3);
        AppMain.nnRotateYMatrix(nns_MATRIX, nns_MATRIX, AppMain.MTM_MATH_ABS(ply_work.gmk_work3) >> 2);
        AppMain.nnRotateZMatrix(nns_MATRIX, nns_MATRIX, AppMain.MTM_MATH_ABS(ply_work.gmk_work3) >> 2);
        AppMain.nnTranslateMatrix(nns_MATRIX, nns_MATRIX, num, num2, num3);
        AppMain.nnMultiplyMatrix(ply_work.ex_obj_mtx_r, nns_MATRIX, ply_work.ex_obj_mtx_r);
    }

    // Token: 0x060008A7 RID: 2215 RVA: 0x0004E368 File Offset: 0x0004C568
    public static void gmPlySeqGmkMainTruckDangerRet(GMS_PLAYER_WORK ply_work)
    {
        NNS_MATRIX nns_MATRIX = GlobalPool<NNS_MATRIX>.Alloc();
        ply_work.gmk_work2 = AppMain.ObjTimeCountUp(ply_work.gmk_work2);
        if (73728 <= ply_work.gmk_work2 && ply_work.gmk_work2 <= 131072)
        {
            ply_work.gmk_work1 += ply_work.gmk_work0;
            if (ply_work.gmk_work0 < 0)
            {
                if (ply_work.gmk_work1 < 0)
                {
                    ply_work.gmk_work1 = 0;
                }
            }
            else if (ply_work.gmk_work1 > 0)
            {
                ply_work.gmk_work1 = 0;
            }
        }
        if ((ply_work.player_flag & 2U) != 0U)
        {
            ply_work.gmk_work3 = AppMain.ObjSpdDownSet(ply_work.gmk_work3, 1024);
            if (AppMain.MTM_MATH_ABS(ply_work.gmk_work3) == 0)
            {
                ply_work.gmk_work3 = 0;
                ply_work.gmk_flag |= 2147483648U;
                AppMain.GmPlySeqChangeFw(ply_work);
                return;
            }
        }
        else if ((ply_work.obj_work.disp_flag & 8U) != 0U)
        {
            ply_work.gmk_work1 = 0;
            if (ply_work.gmk_work3 == 0)
            {
                ply_work.gmk_flag |= 2147483648U;
                AppMain.GmPlySeqChangeFw(ply_work);
                return;
            }
            ply_work.player_flag |= 2U;
            ply_work.gmk_flag &= 4293918719U;
            GmPlayer.ActionChange(ply_work, 69);
            ply_work.obj_work.disp_flag |= 4U;
        }
        AppMain.nnMakeUnitMatrix(ply_work.ex_obj_mtx_r);
        AppMain.nnTranslateMatrix(ply_work.ex_obj_mtx_r, ply_work.ex_obj_mtx_r, 0f, 5f, 9f);
        AppMain.nnRotateXMatrix(ply_work.ex_obj_mtx_r, ply_work.ex_obj_mtx_r, (int)(ply_work.gmk_work1));
        AppMain.nnTranslateMatrix(ply_work.ex_obj_mtx_r, ply_work.ex_obj_mtx_r, 0f, -5f, -9f);
        float num;
        float num2;
        float num3;
        if ((ply_work.player_flag & 4U) == 0U)
        {
            num = 0f;
            num2 = 8f;
            num3 = -5f;
        }
        else
        {
            num = 0f;
            num2 = 8f;
            num3 = 5f;
        }
        AppMain.nnMakeUnitMatrix(nns_MATRIX);
        AppMain.nnTranslateMatrix(nns_MATRIX, nns_MATRIX, -num, -num2, -num3);
        AppMain.nnRotateXMatrix(nns_MATRIX, nns_MATRIX, ply_work.gmk_work3);
        AppMain.nnRotateZMatrix(nns_MATRIX, nns_MATRIX, AppMain.MTM_MATH_ABS(ply_work.gmk_work3) >> 2);
        AppMain.nnTranslateMatrix(nns_MATRIX, nns_MATRIX, num, num2, num3);
        AppMain.nnMultiplyMatrix(ply_work.ex_obj_mtx_r, nns_MATRIX, ply_work.ex_obj_mtx_r);
    }
}
