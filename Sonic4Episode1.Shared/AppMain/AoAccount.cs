using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AoAccount
{
    // Token: 0x040042AE RID: 17070
    private static int g_ao_account_current_id = -1;
    
    // Token: 0x06000CFC RID: 3324 RVA: 0x00074950 File Offset: 0x00072B50
    public static void AoAccountInit()
    {
        g_ao_account_current_id = 0;
    }

    // Token: 0x06000CFD RID: 3325 RVA: 0x00074958 File Offset: 0x00072B58
    public static void AoAccountDebugInit()
    {
    }

    // Token: 0x06000CFE RID: 3326 RVA: 0x0007495A File Offset: 0x00072B5A
    public static void AoAccountExit()
    {
    }

    // Token: 0x06000CFF RID: 3327 RVA: 0x0007495C File Offset: 0x00072B5C
    public static void AoAccountClearCurrentId()
    {
        g_ao_account_current_id = -1;
    }

    // Token: 0x06000D00 RID: 3328 RVA: 0x00074964 File Offset: 0x00072B64
    public static void AoAccountSetCurrentIdStart( uint id )
    {
        g_ao_account_current_id = ( int )id;
    }

    // Token: 0x06000D01 RID: 3329 RVA: 0x0007496C File Offset: 0x00072B6C
    public static bool AoAccountSetCurrentIdIsFinished()
    {
        return true;
    }

    // Token: 0x06000D02 RID: 3330 RVA: 0x0007496F File Offset: 0x00072B6F
    public static int AoAccountGetCurrentId()
    {
        return g_ao_account_current_id;
    }

    // Token: 0x06000D03 RID: 3331 RVA: 0x00074976 File Offset: 0x00072B76
    public static bool AoAccountIsCurrentSignin()
    {
        return AoAccountGetCurrentId() >= 0;
    }

    // Token: 0x06000D04 RID: 3332 RVA: 0x00074983 File Offset: 0x00072B83
    public static bool AoAccountIsCurrentOnline()
    {
        return true; 
    }

    // Token: 0x06000D05 RID: 3333 RVA: 0x0007498D File Offset: 0x00072B8D
    public static bool AoAccountIsCurrentEnable()
    {
        return true;
    }
}
