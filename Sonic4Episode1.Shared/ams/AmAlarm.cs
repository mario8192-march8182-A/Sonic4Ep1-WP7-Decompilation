using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class AmAlarm
{

    // Token: 0x06000645 RID: 1605 RVA: 0x00038259 File Offset: 0x00036459
    public static AMS_ALARM CreateTimer(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
        if (alarm == null)
        {
            alarm = new AMS_ALARM();
        }

        return alarm;
    }

    // Token: 0x06000646 RID: 1606 RVA: 0x0003826B File Offset: 0x0003646B
    public static AMS_ALARM Create(AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
        return alarm;
    }

    // Token: 0x06000647 RID: 1607 RVA: 0x00038273 File Offset: 0x00036473
    public static void Delete(AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
        if (alarm.delete_flag == 1)
        {
            alarm = null;
            return;
        }

        alarm = null;
    }

    // Token: 0x06000648 RID: 1608 RVA: 0x0003828A File Offset: 0x0003648A
    public static void SetTimerVSync(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x06000649 RID: 1609 RVA: 0x00038291 File Offset: 0x00036491
    public static void SetTimer(AMS_ALARM alarm, uint interval)
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x0600064A RID: 1610 RVA: 0x00038298 File Offset: 0x00036498
    public static void Set(AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x0600064B RID: 1611 RVA: 0x0003829F File Offset: 0x0003649F
    public static void Clear(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x0600064C RID: 1612 RVA: 0x000382A6 File Offset: 0x000364A6
    public static void WaitTimer(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
        alarm.spin_wait.SpinOnce();
    }

    // Token: 0x0600064D RID: 1613 RVA: 0x000382AD File Offset: 0x000364AD
    public static void WaitVSync(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x0600064E RID: 1614 RVA: 0x000382B4 File Offset: 0x000364B4
    public static void Wait(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x0600064F RID: 1615 RVA: 0x000382BB File Offset: 0x000364BB
    public static void UpdateTimer(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x06000650 RID: 1616 RVA: 0x000382C4 File Offset: 0x000364C4
    public static int CheckTimer(ref AMS_ALARM alarm)
    {
        AppMain.mppAssertNotImpl();
        return 0;
    }

    // Token: 0x06000651 RID: 1617 RVA: 0x000382DC File Offset: 0x000364DC
    public static int Check(AMS_ALARM alarm)
    {
        int result = 1;
        AppMain.mppAssertNotImpl();
        return result;
    }

    // Token: 0x06000652 RID: 1618 RVA: 0x000382F1 File Offset: 0x000364F1
    public static void Handler(int signal_id)
    {
        AppMain.mppAssertNotImpl();
    }
}

public class AMS_ALARM
{
    public AMS_ALARM()
    {
        spin_wait = new SpinWait();
    }
    
    // Token: 0x04004EA3 RID: 20131
    public int delete_flag;

    // Token: 0x04004EA4 RID: 20132
    public uint alarm_id;

    // Token: 0x04004EA5 RID: 20133
    public uint timer_id;

    // Token: 0x04004EA6 RID: 20134
    public AppMain.AMS_TIMER timer;

    // Token: 0x04004EA7 RID: 20135
    public ulong count_end;

    // Token: 0x04004EA8 RID: 20136
    public ulong count_interval;

    public SpinWait spin_wait;
}