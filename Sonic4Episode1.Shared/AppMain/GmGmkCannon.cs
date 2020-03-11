using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class AppMain
{
    // Token: 0x0200029F RID: 671
    public class GmkCannonDat
    {
        // Token: 0x04005ACA RID: 23242
        private const int GME_GMK_RECT_DATA_LEFT = 0;

        // Token: 0x04005ACB RID: 23243
        private const int GME_GMK_RECT_DATA_TOP = 1;

        // Token: 0x04005ACC RID: 23244
        private const int GME_GMK_RECT_DATA_RIGHT = 2;

        // Token: 0x04005ACD RID: 23245
        private const int GME_GMK_RECT_DATA_BOTTOM = 3;

        // Token: 0x04005ACE RID: 23246
        private const int GME_GMK_RECT_DATA_MAX = 4;
    }

    // Token: 0x020002A0 RID: 672
    public class GMS_GMK_CANNON_WORK : IOBS_OBJECT_WORK
    {
        // Token: 0x0600244F RID: 9295 RVA: 0x0014A8A3 File Offset: 0x00148AA3
        public GMS_GMK_CANNON_WORK()
        {
            this.gmk_work = new AppMain.GMS_ENEMY_3D_WORK(this);
        }

        // Token: 0x06002450 RID: 9296 RVA: 0x0014A8B7 File Offset: 0x00148AB7
        public static explicit operator AppMain.GMS_ENEMY_3D_WORK(AppMain.GMS_GMK_CANNON_WORK p)
        {
            return p.gmk_work;
        }

        // Token: 0x06002451 RID: 9297 RVA: 0x0014A8BF File Offset: 0x00148ABF
        public OBS_OBJECT_WORK Cast()
        {
            return this.gmk_work.ene_com.obj_work;
        }

        // Token: 0x06002452 RID: 9298 RVA: 0x0014A8D1 File Offset: 0x00148AD1
        public static explicit operator OBS_OBJECT_WORK(AppMain.GMS_GMK_CANNON_WORK work)
        {
            return work.gmk_work.ene_com.obj_work;
        }

        // Token: 0x04005ACF RID: 23247
        public readonly AppMain.GMS_ENEMY_3D_WORK gmk_work;

        // Token: 0x04005AD0 RID: 23248
        public GMS_PLAYER_WORK ply_work;

        // Token: 0x04005AD1 RID: 23249
        public bool hitpass;

        // Token: 0x04005AD2 RID: 23250
        public short shoot_after;

        // Token: 0x04005AD3 RID: 23251
        public short angle_set;

        // Token: 0x04005AD4 RID: 23252
        public short angle_now;

        // Token: 0x04005AD5 RID: 23253
        public int cannon_power;
    }

    // Token: 0x020002A1 RID: 673
    public class GMS_GMK_CANNONPARTS_WORK : IOBS_OBJECT_WORK
    {
        // Token: 0x06002453 RID: 9299 RVA: 0x0014A8E3 File Offset: 0x00148AE3
        public GMS_GMK_CANNONPARTS_WORK()
        {
            this.eff_work = new AppMain.GMS_EFFECT_3DNN_WORK(this);
        }

        // Token: 0x06002454 RID: 9300 RVA: 0x0014A8F7 File Offset: 0x00148AF7
        public OBS_OBJECT_WORK Cast()
        {
            return this.eff_work.efct_com.obj_work;
        }

        // Token: 0x04005AD6 RID: 23254
        public readonly AppMain.GMS_EFFECT_3DNN_WORK eff_work;
    }

    // Token: 0x06001248 RID: 4680 RVA: 0x0009FB38 File Offset: 0x0009DD38
    private static OBS_OBJECT_WORK GmGmkCannonInit(AppMain.GMS_EVE_RECORD_EVENT eve_rec, int pos_x, int pos_y,
        byte type)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK =
            (AppMain.GMS_GMK_CANNON_WORK) AppMain.GMM_ENEMY_CREATE_WORK(eve_rec, pos_x, pos_y,
                () => new AppMain.GMS_GMK_CANNON_WORK(), "Gmk_Cannon");
        OBS_OBJECT_WORK obs_OBJECT_WORK = (OBS_OBJECT_WORK) gms_GMK_CANNON_WORK;
        AppMain.GMS_ENEMY_3D_WORK gms_ENEMY_3D_WORK = (AppMain.GMS_ENEMY_3D_WORK) gms_GMK_CANNON_WORK;
        AppMain.ObjObjectCopyAction3dNNModel(obs_OBJECT_WORK, AppMain.gm_gmk_cannon_obj_3d_list[0],
            gms_ENEMY_3D_WORK.obj_3d);
        obs_OBJECT_WORK.pos.z = 131072;
        OBS_OBJECT_WORK obs_OBJECT_WORK2 = obs_OBJECT_WORK;
        obs_OBJECT_WORK2.pos.y = obs_OBJECT_WORK2.pos.y - 18432;
        obs_OBJECT_WORK.dir.y = 32768;
        obs_OBJECT_WORK.move_flag |= 256U;
        if (eve_rec.width != 0)
        {
            gms_GMK_CANNON_WORK.cannon_power = (int) eve_rec.width;
        }
        else
        {
            gms_GMK_CANNON_WORK.cannon_power = 61440;
        }

        AppMain.gmGmkCannon_CreateParts(gms_GMK_CANNON_WORK);
        AppMain.gmGmkCannonStart(obs_OBJECT_WORK);
        return obs_OBJECT_WORK;
    }

    // Token: 0x06001249 RID: 4681 RVA: 0x0009FC10 File Offset: 0x0009DE10
    private static void gmGmkCannonFieldColOn(OBS_OBJECT_WORK obj_work)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK = (AppMain.GMS_GMK_CANNON_WORK) obj_work;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.obj = obj_work;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.diff_data = AppMain.g_gm_default_col;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.width = 24;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.height = 56;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.ofst_x = -12;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.ofst_y = -30;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.flag |= 134217824U;
    }

    // Token: 0x0600124A RID: 4682 RVA: 0x0009FCF4 File Offset: 0x0009DEF4
    private static void gmGmkCannonFieldColOff(OBS_OBJECT_WORK obj_work)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK = (AppMain.GMS_GMK_CANNON_WORK) obj_work;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.obj = null;
    }

    // Token: 0x0600124B RID: 4683 RVA: 0x0009FD24 File Offset: 0x0009DF24
    private static void gmGmkCannon_CannonTurn(AppMain.GMS_GMK_CANNON_WORK pwork)
    {
        if (pwork.angle_set > pwork.angle_now)
        {
            pwork.angle_now += 342;
            if (pwork.angle_now > pwork.angle_set)
            {
                pwork.angle_now = pwork.angle_set;
                return;
            }
        }
        else
        {
            pwork.angle_now -= 342;
            if (pwork.angle_now < pwork.angle_set)
            {
                pwork.angle_now = pwork.angle_set;
            }
        }
    }

    // Token: 0x0600124C RID: 4684 RVA: 0x0009FD9C File Offset: 0x0009DF9C
    private static void gmGmkCannonStart(OBS_OBJECT_WORK obj_work)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK = (AppMain.GMS_GMK_CANNON_WORK) obj_work;
        AppMain.gmGmkCannonFieldColOn(obj_work);
        gms_GMK_CANNON_WORK.gmk_work.ene_com.rect_work[0].flag &= 4294967291U;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.rect_work[1].flag &= 4294967291U;
        AppMain.OBS_RECT_WORK obs_RECT_WORK = gms_GMK_CANNON_WORK.gmk_work.ene_com.rect_work[2];
        obs_RECT_WORK.ppDef = AppMain.gmGmkCannonHit;
        obs_RECT_WORK.ppHit = null;
        AppMain.ObjRectAtkSet(obs_RECT_WORK, 0, 0);
        AppMain.ObjRectDefSet(obs_RECT_WORK, 65534, 0);
        AppMain.ObjRectWorkSet(obs_RECT_WORK, -12, -38, 12, -6);
        obj_work.flag &= 4294967293U;
        gms_GMK_CANNON_WORK.ply_work = null;
        gms_GMK_CANNON_WORK.angle_set = 0;
        gms_GMK_CANNON_WORK.angle_now = 0;
        gms_GMK_CANNON_WORK.gmk_work.ene_com.enemy_flag &= 4294934527U;
        obj_work.ppFunc = AppMain.gmGmkCannonStay;
    }

    // Token: 0x0600124D RID: 4685 RVA: 0x0009FE98 File Offset: 0x0009E098
    private static void gmGmkCannonStay(OBS_OBJECT_WORK obj_work)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK = (AppMain.GMS_GMK_CANNON_WORK) obj_work;
        GMS_PLAYER_WORK gms_PLAYER_WORK = AppMain.g_gm_main_system.ply_work[0];
        if (gms_GMK_CANNON_WORK.gmk_work.ene_com.col_work.obj_col.obj != null)
        {
            if (gms_PLAYER_WORK.act_state == 31)
            {
                AppMain.gmGmkCannonFieldColOff(obj_work);
            }
        }
        else if (gms_PLAYER_WORK.act_state != 31)
        {
            AppMain.gmGmkCannonFieldColOn(obj_work);
        }

        if (gms_GMK_CANNON_WORK.ply_work != null)
        {
            AppMain.OBS_RECT_WORK obs_RECT_WORK = gms_GMK_CANNON_WORK.gmk_work.ene_com.rect_work[2];
            obs_RECT_WORK.flag &= 4294967291U;
            if (gms_PLAYER_WORK.seq_state != 41)
            {
                AppMain.gmGmkCannonStart(obj_work);
                return;
            }

            if (obj_work.pos.y <= gms_GMK_CANNON_WORK.ply_work.obj_work.pos.y)
            {
                obj_work.ppFunc = AppMain.gmGmkCannonReady;
            }
        }
    }

    // Token: 0x0600124E RID: 4686 RVA: 0x0009FF67 File Offset: 0x0009E167
    private static short gmGmkCannon_GetAngle(ushort key)
    {
        if ((key & 8) != 0)
        {
            return 2730;
        }

        if ((key & 4) != 0)
        {
            return -2730;
        }

        return 0;
    }

    // Token: 0x0600124F RID: 4687 RVA: 0x0009FF80 File Offset: 0x0009E180
    private static void gmGmkCannonReady(OBS_OBJECT_WORK obj_work)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK = (AppMain.GMS_GMK_CANNON_WORK) obj_work;
        short angle_set = gms_GMK_CANNON_WORK.angle_set;
        short angle_now = gms_GMK_CANNON_WORK.angle_now;
        if (gms_GMK_CANNON_WORK.angle_set == gms_GMK_CANNON_WORK.angle_now)
        {
            if ((AppMain.g_gs_main_sys_info.game_flag & 1U) == 0U)
            {
                int num = (int) (AppMain._am_iphone_accel_data.sensor.x * 16384f) * 3;
                if (num > 32768)
                {
                    num = 32768;
                }
                else if (num < -32768)
                {
                    num = -32768;
                }

                num /= 2;
                if (num >= (int) (gms_GMK_CANNON_WORK.angle_now + 2730))
                {
                    num = (int) (gms_GMK_CANNON_WORK.angle_now + 2730);
                }
                else if (gms_GMK_CANNON_WORK.angle_now == 13650 && num >= 16380)
                {
                    num = 16380;
                }
                else if (num <= (int) (gms_GMK_CANNON_WORK.angle_now - 2730))
                {
                    num = (int) (gms_GMK_CANNON_WORK.angle_now - 2730);
                }
                else if (gms_GMK_CANNON_WORK.angle_now == -13650 && num <= -16380)
                {
                    num = -16380;
                }
                else
                {
                    num = (int) gms_GMK_CANNON_WORK.angle_now;
                }

                gms_GMK_CANNON_WORK.angle_set = (short) num;
            }
            else
            {
                AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK2 = gms_GMK_CANNON_WORK;
                gms_GMK_CANNON_WORK2.angle_set += AppMain.gmGmkCannon_GetAngle(gms_GMK_CANNON_WORK.ply_work.key_on);
                if (gms_GMK_CANNON_WORK.angle_set > 16380 && (ushort) gms_GMK_CANNON_WORK.angle_set < 49156)
                {
                    gms_GMK_CANNON_WORK.angle_set = 16380;
                }

                if (gms_GMK_CANNON_WORK.angle_set < -16380 && (ushort) gms_GMK_CANNON_WORK.angle_set > 16380)
                {
                    gms_GMK_CANNON_WORK.angle_set = -16380;
                }
            }

            if (angle_set != gms_GMK_CANNON_WORK.angle_set)
            {
                GmSound.PlaySE("Cannon1");
            }
        }

        if (gms_GMK_CANNON_WORK.angle_set != gms_GMK_CANNON_WORK.angle_now)
        {
            AppMain.gmGmkCannon_CannonTurn(gms_GMK_CANNON_WORK);
            obj_work.dir.z = (ushort) gms_GMK_CANNON_WORK.angle_now;
        }

        if (gms_GMK_CANNON_WORK.angle_set == gms_GMK_CANNON_WORK.angle_now &&
            angle_now == gms_GMK_CANNON_WORK.angle_now &&
            GmPlayer.KeyCheckJumpKeyPush(gms_GMK_CANNON_WORK.ply_work))
        {
            AppMain.GMS_EFFECT_3DES_WORK gms_EFFECT_3DES_WORK =
                AppMain.GmEfctCmnEsCreate(gms_GMK_CANNON_WORK.ply_work.obj_work, 20);
            gms_EFFECT_3DES_WORK.efct_com.obj_work.dir.z = obj_work.dir.z;
            OBS_OBJECT_WORK obj_work2 = gms_EFFECT_3DES_WORK.efct_com.obj_work;
            obj_work2.pos.x = obj_work2.pos.x + AppMain.mtMathSin((int) obj_work.dir.z) * 32;
            OBS_OBJECT_WORK obj_work3 = gms_EFFECT_3DES_WORK.efct_com.obj_work;
            obj_work3.pos.y = obj_work3.pos.y - AppMain.mtMathCos((int) obj_work.dir.z) * 32;
            AppMain.GmPlySeqInitCannonShoot(gms_GMK_CANNON_WORK.ply_work,
                AppMain.mtMathCos((int) (obj_work.dir.z - 16384)) * gms_GMK_CANNON_WORK.cannon_power,
                AppMain.mtMathSin((int) (obj_work.dir.z - 16384)) * gms_GMK_CANNON_WORK.cannon_power);
            AppMain.gmGmkCannonFieldColOff(obj_work);
            gms_GMK_CANNON_WORK.gmk_work.ene_com.enemy_flag |= 32768U;
            gms_GMK_CANNON_WORK.shoot_after = 0;
            obj_work.ppFunc = AppMain.gmGmkCannonShoot;
            AppMain.gmGmkCannonShoot(obj_work);
            GmSound.PlaySE("Cannon2");
            AppMain.GMM_PAD_VIB_SMALL();
        }
    }

    // Token: 0x06001250 RID: 4688 RVA: 0x000A0280 File Offset: 0x0009E480
    private static void gmGmkCannonShoot(OBS_OBJECT_WORK obj_work)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK = (AppMain.GMS_GMK_CANNON_WORK) obj_work;
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK2 = gms_GMK_CANNON_WORK;
        gms_GMK_CANNON_WORK2.shoot_after += 1;
        if (gms_GMK_CANNON_WORK.shoot_after == 16)
        {
            AppMain.gmGmkCannonFieldColOn(obj_work);
            if (gms_GMK_CANNON_WORK.angle_now == 0)
            {
                gms_GMK_CANNON_WORK.ply_work = null;
                AppMain.gmGmkCannonStart(obj_work);
                return;
            }
        }

        if (gms_GMK_CANNON_WORK.shoot_after > 32)
        {
            gms_GMK_CANNON_WORK.ply_work = null;
            gms_GMK_CANNON_WORK.angle_set = 0;
            obj_work.ppFunc = AppMain.gmGmkCannonShootEnd;
        }
    }

    // Token: 0x06001251 RID: 4689 RVA: 0x000A02F4 File Offset: 0x0009E4F4
    private static void gmGmkCannonShootEnd(OBS_OBJECT_WORK obj_work)
    {
        AppMain.GMS_GMK_CANNON_WORK gms_GMK_CANNON_WORK = (AppMain.GMS_GMK_CANNON_WORK) obj_work;
        AppMain.gmGmkCannon_CannonTurn(gms_GMK_CANNON_WORK);
        obj_work.dir.z = (ushort) gms_GMK_CANNON_WORK.angle_now;
        if (gms_GMK_CANNON_WORK.angle_now == gms_GMK_CANNON_WORK.angle_set)
        {
            AppMain.gmGmkCannonStart(obj_work);
        }
    }

    // Token: 0x06001252 RID: 4690 RVA: 0x000A0334 File Offset: 0x0009E534
    private static void gmGmkCannonHit(AppMain.OBS_RECT_WORK mine_rect, AppMain.OBS_RECT_WORK match_rect)
    {
        OBS_OBJECT_WORK parent = mine_rect.parent_obj;
        GMS_PLAYER_WORK playerWork = (GMS_PLAYER_WORK) match_rect.parent_obj;
        GMS_GMK_CANNON_WORK cannonWork = (AppMain.GMS_GMK_CANNON_WORK) parent;
        cannonWork.hitpass = false;
        if (playerWork == AppMain.g_gm_main_system.ply_work[0])
        {
            if (cannonWork.ply_work != playerWork)
            {
                int num = ((parent.pos.y >> 12) + (int) mine_rect.rect.top);
                if ((playerWork.obj_work.pos.y >> 12 < (int) num && playerWork.obj_work.move.y >= 0) ||
                    playerWork.act_state == 31)
                {
                    int num2 = 1;
                    int num3 = (AppMain.MTM_MATH_ABS((int) mine_rect.rect.left - (int) match_rect.rect.left) +
                                AppMain.MTM_MATH_ABS((int) mine_rect.rect.right - (int) match_rect.rect.right));
                    int num4 = AppMain.MTM_MATH_ABS(playerWork.obj_work.move.x >> 12);
                    if (num4 != 0)
                        num2 = ((int) num4 / (int) num3 + 1);
                    if (playerWork.obj_work.move.x < 0)
                        num3 = -num3;
                    int num5 = ((parent.pos.x >> 12) + (int) mine_rect.rect.left - (int) match_rect.rect.left);
                    int num6 = ((parent.pos.x >> 12) + (int) mine_rect.rect.right - (int) match_rect.rect.right);
                    int num7 = (playerWork.obj_work.pos.x >> 12);
                    for (; num2 != 0; --num2)
                    {
                        if ((int) num7 >= (int) num5 && (int) num7 <= (int) num6)
                        {
                            cannonWork.ply_work = playerWork;
                            AppMain.GmPlySeqInitCannon(playerWork, (AppMain.GMS_ENEMY_COM_WORK) parent);
                            GmSound.PlaySE("Cannon3");
                            break;
                        }

                        num7 += num3;
                    }
                }
            }

            cannonWork.hitpass = true;
        }

        mine_rect.flag &= 4294573823U;
    }

    // Token: 0x06001253 RID: 4691 RVA: 0x000A051C File Offset: 0x0009E71C
    private static void gmGmkCannon_CreateParts(AppMain.GMS_GMK_CANNON_WORK pwork)
    {
        OBS_OBJECT_WORK obs_OBJECT_WORK = (OBS_OBJECT_WORK) pwork;
        OBS_OBJECT_WORK obs_OBJECT_WORK2 =
            AppMain.GMM_EFFECT_CREATE_WORK(() => new AppMain.GMS_GMK_CANNONPARTS_WORK(), null, 0, "Gmk_CannonBase");
        AppMain.GMS_GMK_CANNONPARTS_WORK gms_GMK_CANNONPARTS_WORK = (AppMain.GMS_GMK_CANNONPARTS_WORK) obs_OBJECT_WORK2;
        AppMain.ObjObjectCopyAction3dNNModel(obs_OBJECT_WORK2, AppMain.gm_gmk_cannon_obj_3d_list[1],
            gms_GMK_CANNONPARTS_WORK.eff_work.obj_3d);
        obs_OBJECT_WORK2.parent_obj = obs_OBJECT_WORK;
        obs_OBJECT_WORK2.flag &= 4294966271U;
        obs_OBJECT_WORK2.pos.x = obs_OBJECT_WORK.pos.x;
        obs_OBJECT_WORK2.pos.y = obs_OBJECT_WORK.pos.y + 122880;
        obs_OBJECT_WORK2.pos.z = obs_OBJECT_WORK.pos.z + 122880;
        obs_OBJECT_WORK2.dir.y = obs_OBJECT_WORK.dir.y;
        obs_OBJECT_WORK2.move_flag |= 256U;
        obs_OBJECT_WORK2.disp_flag &= 4294967039U;
        obs_OBJECT_WORK2.flag |= 2U;
        obs_OBJECT_WORK2.ppFunc = null;
    }

    // Token: 0x06001254 RID: 4692 RVA: 0x000A062F File Offset: 0x0009E82F
    private static void GmGmkCannonBuild()
    {
        AppMain.gm_gmk_cannon_obj_3d_list = AppMain.GmGameDBuildRegBuildModel(AppMain.GmGameDatGetGimmickData(858),
            AppMain.GmGameDatGetGimmickData(859), 0U);
    }

    // Token: 0x06001255 RID: 4693 RVA: 0x000A0650 File Offset: 0x0009E850
    private static void GmGmkCannonFlush()
    {
        AMS_AMB_HEADER ams_AMB_HEADER = AppMain.GmGameDatGetGimmickData(858);
        AppMain.GmGameDBuildRegFlushModel(AppMain.gm_gmk_cannon_obj_3d_list, ams_AMB_HEADER.file_num);
    }
}