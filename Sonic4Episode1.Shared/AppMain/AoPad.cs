using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sonic4Episode1.Abstraction;

public partial class AppMain
{
    private static IControllerSource controllerSource;

    // Token: 0x060018EC RID: 6380 RVA: 0x000E3C82 File Offset: 0x000E1E82
    private static ControllerConsts AoPadDirect()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_DIRECT(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018ED RID: 6381 RVA: 0x000E3C98 File Offset: 0x000E1E98
    private static ControllerConsts AoPadStand()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_STAND(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018EE RID: 6382 RVA: 0x000E3CAE File Offset: 0x000E1EAE
    private static ControllerConsts AoPadRepeat()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_REPEAT(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018EF RID: 6383 RVA: 0x000E3CC4 File Offset: 0x000E1EC4
    private static ControllerConsts AoPadRelease()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_RELEASE(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F0 RID: 6384 RVA: 0x000E3CDA File Offset: 0x000E1EDA
    private static ControllerConsts AoPadADirect()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_ADIRECT(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    private static bool AoPadADirect(ControllerConsts key)
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return (AppMain.PAD_ADIRECT(AppMain.AoAccountGetCurrentId()) & key) != 0;
        }
        return false;
    }

    // Token: 0x060018F1 RID: 6385 RVA: 0x000E3CF0 File Offset: 0x000E1EF0
    private static ControllerConsts AoPadAStand()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_ASTAND(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F2 RID: 6386 RVA: 0x000E3D06 File Offset: 0x000E1F06
    private static ControllerConsts AoPadARepeat()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_AREPEAT(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F3 RID: 6387 RVA: 0x000E3D1C File Offset: 0x000E1F1C
    private static ControllerConsts AoPadARelease()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_ARELEASE(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F4 RID: 6388 RVA: 0x000E3D32 File Offset: 0x000E1F32
    private static ControllerConsts AoPadMDirect()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return (ControllerConsts)AppMain.PAD_MDIRECT(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F5 RID: 6389 RVA: 0x000E3D49 File Offset: 0x000E1F49
    private static ControllerConsts AoPadMStand()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return (ControllerConsts)AppMain.PAD_MSTAND(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F6 RID: 6390 RVA: 0x000E3D60 File Offset: 0x000E1F60
    private static ControllerConsts AoPadMRepeat()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return (ControllerConsts)AppMain.PAD_MREPEAT(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F7 RID: 6391 RVA: 0x000E3D77 File Offset: 0x000E1F77
    private static ControllerConsts AoPadMRelease()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return (ControllerConsts)AppMain.PAD_MRELEASE(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F8 RID: 6392 RVA: 0x000E3D8E File Offset: 0x000E1F8E
    private static short AoPadAnalogLX()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_A_LX(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018F9 RID: 6393 RVA: 0x000E3DA4 File Offset: 0x000E1FA4
    private static short AoPadAnalogLY()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_A_LY(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018FA RID: 6394 RVA: 0x000E3DBA File Offset: 0x000E1FBA
    private static short AoPadAnalogRX()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_A_RX(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x060018FB RID: 6395 RVA: 0x000E3DD0 File Offset: 0x000E1FD0
    private static short AoPadAnalogRY()
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            return AppMain.PAD_A_RY(AppMain.AoAccountGetCurrentId());
        }
        return 0;
    }

    // Token: 0x06001909 RID: 6409 RVA: 0x000E3F10 File Offset: 0x000E2110
    private static int AoPadSomeoneDirect(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_DIRECT(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x0600190A RID: 6410 RVA: 0x000E3F38 File Offset: 0x000E2138
    private static int AoPadSomeoneStand(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_STAND(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x0600190B RID: 6411 RVA: 0x000E3F60 File Offset: 0x000E2160
    private static int AoPadSomeoneRepeat(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_REPEAT(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x0600190C RID: 6412 RVA: 0x000E3F88 File Offset: 0x000E2188
    private static int AoPadSomeoneRelease(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_RELEASE(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x0600190D RID: 6413 RVA: 0x000E3FB0 File Offset: 0x000E21B0
    private static int AoPadSomeoneADirect(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_ADIRECT(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x0600190E RID: 6414 RVA: 0x000E3FD8 File Offset: 0x000E21D8
    private static int AoPadSomeoneAStand(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_ASTAND(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x0600190F RID: 6415 RVA: 0x000E4000 File Offset: 0x000E2200
    private static int AoPadSomeoneARepeat(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_AREPEAT(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x06001910 RID: 6416 RVA: 0x000E4028 File Offset: 0x000E2228
    private static int AoPadSomeoneARelease(ControllerConsts key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_ARELEASE(i) & key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x06001911 RID: 6417 RVA: 0x000E4050 File Offset: 0x000E2250
    private static int AoPadSomeoneMDirect(ushort key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_MDIRECT(i) & (int)key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x06001912 RID: 6418 RVA: 0x000E4078 File Offset: 0x000E2278
    private static int AoPadSomeoneMStand(ushort key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_MSTAND(i) & (int)key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x06001913 RID: 6419 RVA: 0x000E40A0 File Offset: 0x000E22A0
    private static int AoPadSomeoneMRepeat(ushort key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_MREPEAT(i) & (int)key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x06001914 RID: 6420 RVA: 0x000E40C8 File Offset: 0x000E22C8
    private static int AoPadSomeoneMRelease(ushort key)
    {
        for (int i = 0; i < (controllerSource?.Count ?? 0); i++)
        {
            if ((AppMain.PAD_MRELEASE(i) & (int)key) != 0)
            {
                return i;
            }
        }
        return -1;
    }

    // Token: 0x06001915 RID: 6421 RVA: 0x000E40EE File Offset: 0x000E22EE
    private static ControllerConsts AoPadPortDirect(uint port)
    {
        return AppMain.PAD_DIRECT((int)port);
    }

    // Token: 0x06001916 RID: 6422 RVA: 0x000E40F6 File Offset: 0x000E22F6
    private static ControllerConsts AoPadPortStand(uint port)
    {
        return AppMain.PAD_STAND((int)port);
    }

    // Token: 0x06001917 RID: 6423 RVA: 0x000E40FE File Offset: 0x000E22FE
    private static ControllerConsts AoPadPortRepeat(uint port)
    {
        return AppMain.PAD_REPEAT((int)port);
    }

    // Token: 0x06001918 RID: 6424 RVA: 0x000E4106 File Offset: 0x000E2306
    private static ControllerConsts AoPadPortRelease(uint port)
    {
        return AppMain.PAD_RELEASE((int)port);
    }

    // Token: 0x06001919 RID: 6425 RVA: 0x000E410E File Offset: 0x000E230E
    private static ControllerConsts AoPadPortADirect(uint port)
    {
        return AppMain.PAD_ADIRECT((int)port);
    }

    // Token: 0x0600191A RID: 6426 RVA: 0x000E4116 File Offset: 0x000E2316
    private static ControllerConsts AoPadPortAStand(uint port)
    {
        return AppMain.PAD_ASTAND((int)port);
    }

    // Token: 0x0600191B RID: 6427 RVA: 0x000E411E File Offset: 0x000E231E
    private static ControllerConsts AoPadPortARepeat(uint port)
    {
        return AppMain.PAD_AREPEAT((int)port);
    }

    // Token: 0x0600191C RID: 6428 RVA: 0x000E4126 File Offset: 0x000E2326
    private static ControllerConsts AoPadPortARelease(uint port)
    {
        return AppMain.PAD_ARELEASE((int)port);
    }

    // Token: 0x0600191D RID: 6429 RVA: 0x000E412E File Offset: 0x000E232E
    private static ControllerConsts AoPadPortMDirect(uint port)
    {
        return (ControllerConsts)AppMain.PAD_MDIRECT((int)port);
    }

    // Token: 0x0600191E RID: 6430 RVA: 0x000E4137 File Offset: 0x000E2337
    private static ControllerConsts AoPadPortMStand(uint port)
    {
        return (ControllerConsts)AppMain.PAD_MSTAND((int)port);
    }

    // Token: 0x0600191F RID: 6431 RVA: 0x000E4140 File Offset: 0x000E2340
    private static ControllerConsts AoPadPortMRepeat(uint port)
    {
        return (ControllerConsts)AppMain.PAD_MREPEAT((int)port);
    }

    // Token: 0x06001920 RID: 6432 RVA: 0x000E4149 File Offset: 0x000E2349
    private static ControllerConsts AoPadPortMRelease(uint port)
    {
        return (ControllerConsts)AppMain.PAD_MRELEASE((int)port);
    }

    // Token: 0x06001921 RID: 6433 RVA: 0x000E4152 File Offset: 0x000E2352
    private static short AoPadPortAnalogLX(uint port)
    {
        return AppMain.PAD_A_LX((int)port);
    }

    // Token: 0x06001922 RID: 6434 RVA: 0x000E415A File Offset: 0x000E235A
    private static short AoPadPortAnalogLY(uint port)
    {
        return AppMain.PAD_A_LY((int)port);
    }

    // Token: 0x06001923 RID: 6435 RVA: 0x000E4162 File Offset: 0x000E2362
    private static short AoPadPortAnalogRX(uint port)
    {
        return AppMain.PAD_A_RX((int)port);
    }

    // Token: 0x06001924 RID: 6436 RVA: 0x000E416A File Offset: 0x000E236A
    private static short AoPadPortAnalogRY(uint port)
    {
        return AppMain.PAD_A_RY((int)port);
    }

    // Token: 0x06001932 RID: 6450 RVA: 0x000E41E0 File Offset: 0x000E23E0
    private static bool AoPadIsConnected(uint port)
    {
        return AppMain.PAD_CONNECT((int)port) != 0;
    }

    // Token: 0x06001933 RID: 6451 RVA: 0x000E41F0 File Offset: 0x000E23F0
    private static bool AoPadIsConnected()
    {
        int num = AppMain.AoAccountGetCurrentId();
        return num >= 0 && AppMain.AoPadIsConnected((uint)num);
    }

    // Token: 0x06001934 RID: 6452 RVA: 0x000E420F File Offset: 0x000E240F
    private static void AoPadEnableVibration(bool flag)
    {

    }

    // Token: 0x06001935 RID: 6453 RVA: 0x000E422A File Offset: 0x000E242A
    private static void AoPadSetVibration(ushort left, ushort right)
    {
        if (AppMain.AoAccountGetCurrentId() < (controllerSource?.Count ?? 0))
        {
            controllerSource[AppMain.AoAccountGetCurrentId()].SetVibration(left, right);
        }
    }

    // Token: 0x06000F77 RID: 3959 RVA: 0x000885AC File Offset: 0x000867AC
    public static int PAD_CONNECT(int _port)
    {
        return (int)(AppMain.controllerSource[_port] != null ? 1 : 0);
    }

    // Token: 0x06000F78 RID: 3960 RVA: 0x000885BC File Offset: 0x000867BC
    public static ControllerConsts PAD_DIRECT(int _port)
    {
        return (ControllerConsts)AppMain.controllerSource[_port].reading.direct;
    }

    // Token: 0x06000F79 RID: 3961 RVA: 0x000885CA File Offset: 0x000867CA
    public static ControllerConsts PAD_STAND(int _port)
    {
        return (ControllerConsts)AppMain.controllerSource[_port].reading.stand;
    }

    // Token: 0x06000F7A RID: 3962 RVA: 0x000885D8 File Offset: 0x000867D8
    public static ControllerConsts PAD_REPEAT(int _port)
    {
        return (ControllerConsts)AppMain.controllerSource[_port].reading.repeat;
    }

    // Token: 0x06000F7B RID: 3963 RVA: 0x000885E6 File Offset: 0x000867E6
    public static ControllerConsts PAD_RELEASE(int _port)
    {
        return (ControllerConsts)AppMain.controllerSource[_port].reading.release;
    }

    // Token: 0x06000F7C RID: 3964 RVA: 0x000885F4 File Offset: 0x000867F4
    public static ControllerConsts PAD_ADIRECT(int _port)
    {
        if ((controllerSource?.Count ?? 0) == 0)
            return 0;

        return (ControllerConsts)controllerSource[_port].reading.adirect;
    }

    // Token: 0x06000F7D RID: 3965 RVA: 0x00088602 File Offset: 0x00086802
    public static ControllerConsts PAD_ASTAND(int _port)
    {
        return (ControllerConsts)controllerSource[_port].reading.astand;
    }

    // Token: 0x06000F7E RID: 3966 RVA: 0x00088610 File Offset: 0x00086810
    public static ControllerConsts PAD_AREPEAT(int _port)
    {
        return (ControllerConsts)controllerSource[_port].reading.arepeat;
    }

    // Token: 0x06000F7F RID: 3967 RVA: 0x0008861E File Offset: 0x0008681E
    public static ControllerConsts PAD_ARELEASE(int _port)
    {
        return (ControllerConsts)controllerSource[_port].reading.arelease;
    }

    // Token: 0x06000F80 RID: 3968 RVA: 0x0008862C File Offset: 0x0008682C
    public static int PAD_MDIRECT(int _port)
    {
        return (int)(AppMain.controllerSource[_port].reading.direct | AppMain.controllerSource[_port].reading.adirect);
    }

    // Token: 0x06000F81 RID: 3969 RVA: 0x00088647 File Offset: 0x00086847
    public static int PAD_MSTAND(int _port)
    {
        return (int)(AppMain.controllerSource[_port].reading.stand | AppMain.controllerSource[_port].reading.astand);
    }

    // Token: 0x06000F82 RID: 3970 RVA: 0x00088662 File Offset: 0x00086862
    public static int PAD_MREPEAT(int _port)
    {
        return (int)(AppMain.controllerSource[_port].reading.repeat | AppMain.controllerSource[_port].reading.arepeat);
    }

    // Token: 0x06000F83 RID: 3971 RVA: 0x0008867D File Offset: 0x0008687D
    public static int PAD_MRELEASE(int _port)
    {
        return (int)(AppMain.controllerSource[_port].reading.release | AppMain.controllerSource[_port].reading.arelease);
    }

    // Token: 0x06000F84 RID: 3972 RVA: 0x00088698 File Offset: 0x00086898
    public static short PAD_A_LX(int _port)
    {
        return AppMain.controllerSource[_port].reading.alx;
    }

    // Token: 0x06000F85 RID: 3973 RVA: 0x000886A6 File Offset: 0x000868A6
    public static short PAD_A_LY(int _port)
    {
        return AppMain.controllerSource[_port].reading.aly;
    }

    // Token: 0x06000F86 RID: 3974 RVA: 0x000886B4 File Offset: 0x000868B4
    public static short PAD_A_RX(int _port)
    {
        return AppMain.controllerSource[_port].reading.arx;
    }

    // Token: 0x06000F87 RID: 3975 RVA: 0x000886C2 File Offset: 0x000868C2
    public static short PAD_A_RY(int _port)
    {
        return AppMain.controllerSource[_port].reading.ary;
    }

    // Token: 0x06000F95 RID: 3989 RVA: 0x00088786 File Offset: 0x00086986
    public static int PAD_KEEP_TIME(int _port, int _key_id)
    {
        return AppMain.controllerSource[_port].reading.keep_time[_key_id];
    }

    // Token: 0x06000F96 RID: 3990 RVA: 0x00088796 File Offset: 0x00086996
    public static int PAD_LAST_TIME(int _port, int _key_id)
    {
        return AppMain.controllerSource[_port].reading.last_time[_key_id];
    }

    // Token: 0x06000F97 RID: 3991 RVA: 0x000887A6 File Offset: 0x000869A6
    public static int PAD_KEEP_ATIME(int _port, int _key_id)
    {
        return AppMain.controllerSource[_port].reading.keep_atime[_key_id];
    }

    // Token: 0x06000F98 RID: 3992 RVA: 0x000887B6 File Offset: 0x000869B6
    public static int PAD_LAST_ATIME(int _port, int _key_id)
    {
        return AppMain.controllerSource[_port].reading.last_atime[_key_id];
    }

}
