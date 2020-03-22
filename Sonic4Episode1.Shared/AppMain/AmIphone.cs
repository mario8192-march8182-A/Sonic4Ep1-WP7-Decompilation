using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using mpp;

public partial class AppMain
{
    // Token: 0x0200037A RID: 890
    private enum AME_IPHONE_DISPLAY_ORIENTATION
    {
        // Token: 0x04006056 RID: 24662
        AME_IPHONE_DISPLAY_ORIENTATION_NORMAL,
        // Token: 0x04006057 RID: 24663
        AME_IPHONE_DISPLAY_ORIENTATION_RIGHT,
        // Token: 0x04006058 RID: 24664
        AME_IPHONE_DISPLAY_ORIENTATION_HORIZON,
        // Token: 0x04006059 RID: 24665
        AME_IPHONE_DISPLAY_ORIENTATION_LEFT,
        // Token: 0x0400605A RID: 24666
        AME_IPHONE_DISPLAY_ORIENTATION_NUM
    }

    // Token: 0x0200037B RID: 891
    private enum AME_IPHONE_TP_TOUCH
    {
        // Token: 0x0400605C RID: 24668
        AME_IPHONE_TP_TOUCH_OFF,
        // Token: 0x0400605D RID: 24669
        AME_IPHONE_TP_TOUCH_ON,
        // Token: 0x0400605E RID: 24670
        AME_IPHONE_TP_TOUCH_NUM
    }

    // Token: 0x0200037C RID: 892
    private enum AME_IPHONE_TP_VALIDITY
    {
        // Token: 0x04006060 RID: 24672
        AME_IPHONE_TP_VALIDITY_INVALID,
        // Token: 0x04006061 RID: 24673
        AME_IPHONE_TP_VALIDITY_VALID,
        // Token: 0x04006062 RID: 24674
        AME_IPHONE_TP_VALIDITY_NUM
    }

    // Token: 0x0200037D RID: 893
    private class AMS_IPHONE_TP_DATA
    {
        // Token: 0x060026C2 RID: 9922 RVA: 0x00150338 File Offset: 0x0014E538
        internal void Assign(AppMain.AMS_IPHONE_TP_DATA data)
        {
            this.touch = data.touch;
            this.validity = data.validity;
            this.x = data.x;
            this.y = data.y;
        }

        // Token: 0x04006063 RID: 24675
        public int id;

        // Token: 0x04006064 RID: 24676
        public ushort touch;

        // Token: 0x04006065 RID: 24677
        public ushort validity;

        // Token: 0x04006066 RID: 24678
        public ushort x;

        // Token: 0x04006067 RID: 24679
        public ushort y;
    }

    // Token: 0x0200037E RID: 894
    public class AMS_IPHONE_ACCEL_DATA
    {
        // Token: 0x04006068 RID: 24680
        public readonly NNS_VECTOR core = GlobalPool<NNS_VECTOR>.Alloc();

        // Token: 0x04006069 RID: 24681
        public readonly NNS_VECTOR sensor = GlobalPool<NNS_VECTOR>.Alloc();

        // Token: 0x0400606A RID: 24682
        public int rot_x;

        // Token: 0x0400606B RID: 24683
        public int rot_y;

        // Token: 0x0400606C RID: 24684
        public int rot_z;
    }

    // Token: 0x0200037F RID: 895
    private class AMS_IPHONE_TP_CTRL_DATA
    {
        // Token: 0x0400606D RID: 24685
        public AppMain.AMS_IPHONE_TP_DATA tpdata = new AppMain.AMS_IPHONE_TP_DATA();
    }

    // Token: 0x06001852 RID: 6226 RVA: 0x000DB300 File Offset: 0x000D9500
    public static void amIPhoneAccelerate(ref Vector3 accel)
    {
        NNS_VECTOR core = _am_iphone_accel_data.core;
        NNS_VECTOR sensor = _am_iphone_accel_data.sensor;
        core.x = accel.X;
        core.y = accel.Y;
        core.z = accel.Z;
        sensor.x = -core.y;
        sensor.y = core.x;
        sensor.z = core.z;
        _am_iphone_accel_data.rot_x = nnArcTan2((double)(-(double)sensor.z), (double)(-(double)sensor.y));
        _am_iphone_accel_data.rot_z = nnArcTan2((double)sensor.x, (double)(-(double)sensor.y));
    }

    // Token: 0x06001853 RID: 6227 RVA: 0x000DB3A9 File Offset: 0x000D95A9
    private static void amIPhoneRequestTouch(AppMain.AMS_IPHONE_TP_DATA DispData, int TouchIndex)
    {
        if (DispData != null)
        {
            DispData.Assign(_am_iphone_tp_ctrl_data[TouchIndex].tpdata);
        }
    }

    // Token: 0x06001854 RID: 6228 RVA: 0x000DB3C0 File Offset: 0x000D95C0
    public static void setBackKeyRequest(bool val)
    {
        _am_is_back_key_pressed = val;
    }

    // Token: 0x06001855 RID: 6229 RVA: 0x000DB3C8 File Offset: 0x000D95C8
    public static bool isBackKeyPressed()
    {
        return _am_is_back_key_pressed;
    }

    // Token: 0x06001856 RID: 6230 RVA: 0x000DB3CF File Offset: 0x000D95CF
    public static void amKeyGetData()
    {
        _am_is_back_key_pressed = back_key_is_pressed;
        back_key_is_pressed = false;
    }

    // Token: 0x06001857 RID: 6231 RVA: 0x000DB3E4 File Offset: 0x000D95E4
    static bool wasPressed = false;
    public static void onTouchEvents()
    {
        // PATCH
        var state = new List<TouchLocation>();
        if (TouchPanel.GetCapabilities().IsConnected)
        {
            state.AddRange(TouchPanel.GetState());
        }
        else
        {
            var mouseState = Mouse.GetState();
            if (wasPressed)
                state.Add(new TouchLocation(0, mouseState.LeftButton == ButtonState.Released ?
                    (wasPressed ? TouchLocationState.Released : TouchLocationState.Moved) : TouchLocationState.Pressed,
                    new Vector2(mouseState.X, mouseState.Y), TouchLocationState.Invalid, new Vector2()));
            wasPressed = mouseState.LeftButton == ButtonState.Pressed;
        }

        for (int i = 0; i < 4; i++)
        {
            touchMarked[i] = false;
        }
        for (int j = 0; j < 4; j++)
        {
            int id = 0;
            TouchLocationState touchLocationState;
            if (j >= state.Count)
            {
                touchLocationState = TouchLocationState.Invalid;
                posVector.X = -1f;
                posVector.Y = -1f;
                int num = 0;
                while (num < 4 && touchMarked[num])
                {
                    num++;
                }
                int num2 = num;
                touchMarked[num2] = true;
            }
            else
            {
                TouchLocation touchLocation = state[j];
                float x = touchLocation.Position.X;
                float y = touchLocation.Position.Y;
                screen2real(ref x, ref y);
                posVector.X = x;
                posVector.Y = y;
                touchLocationState = touchLocation.State;
                id = touchLocation.Id;
                int num2 = amFindTouchIndex(id);
                touchMarked[num2] = true;
            }

            switch (touchLocationState)
            {
                case TouchLocationState.Invalid:
                    amIPhoneTouchCanceled(j);
                    break;
                case TouchLocationState.Released:
                    amIPhoneTouchEnded(j);
                    break;
                case TouchLocationState.Pressed:
                    amIPhoneTouchBegan(ref posVector, j, id);
                    break;
                case TouchLocationState.Moved:
                    amIPhoneTouchMoved(ref posVector, j, id);
                    break;
            }
        }
    }

    // Token: 0x06001858 RID: 6232 RVA: 0x000DB528 File Offset: 0x000D9728
    private static void screen2real(ref float X, ref float Y)
    {
        var bounds = m_game.Window.ClientBounds;

        // nominal 480, 288
        X /= (float)bounds.Width;
        X *= 480;

        Y /= (float)(bounds.Height);
        Y *= 288;
        Y += 16;

        // Debug.WriteLine($"{X}, {Y}");
    }

    // Token: 0x06001859 RID: 6233 RVA: 0x000DB540 File Offset: 0x000D9740
    private static int amFindTouchIndex(int id)
    {
        for (int i = 0; i < 4; i++)
        {
            if (_am_iphone_tp_ctrl_data[i].tpdata.id == id)
            {
                return i;
            }
        }
        for (int j = 0; j < 4; j++)
        {
            if (!touchMarked[j])
            {
                return j;
            }
        }
        return 0;
    }

    // Token: 0x0600185A RID: 6234 RVA: 0x000DB588 File Offset: 0x000D9788
    private static void amIPhoneTouchBegan(ref Vector2 touch, int i, int id)
    {
        AppMain.AMS_IPHONE_TP_DATA tpdata = _am_iphone_tp_ctrl_data[i].tpdata;
        tpdata.x = (ushort)touch.X;
        tpdata.y = (ushort)touch.Y;
        tpdata.id = id;
        tpdata.touch = 1;
        tpdata.validity = 1;
    }

    // Token: 0x0600185B RID: 6235 RVA: 0x000DB5D4 File Offset: 0x000D97D4
    private static void amIPhoneTouchMoved(ref Vector2 touch, int i, int id)
    {
        AppMain.AMS_IPHONE_TP_DATA tpdata = _am_iphone_tp_ctrl_data[i].tpdata;
        tpdata.x = (ushort)touch.X;
        tpdata.y = (ushort)touch.Y;
        tpdata.id = id;
        tpdata.touch = 1;
        tpdata.validity = 1;
    }

    // Token: 0x0600185C RID: 6236 RVA: 0x000DB620 File Offset: 0x000D9820
    private static void amIPhoneTouchCanceled()
    {
        for (int i = 0; i < 4; i++)
        {
            _am_iphone_tp_ctrl_data[i].tpdata.touch = 0;
            _am_iphone_tp_ctrl_data[i].tpdata.validity = 0;
        }
    }

    // Token: 0x0600185D RID: 6237 RVA: 0x000DB65D File Offset: 0x000D985D
    private static void amIPhoneTouchCanceled(int i)
    {
        _am_iphone_tp_ctrl_data[i].tpdata.touch = 0;
        _am_iphone_tp_ctrl_data[i].tpdata.validity = 0;
    }

    // Token: 0x0600185E RID: 6238 RVA: 0x000DB683 File Offset: 0x000D9883
    private static void amIPhoneTouchEnded(int i)
    {
        _am_iphone_tp_ctrl_data[i].tpdata.touch = 0;
    }

    public static void ScaleProportions(ref float currentWidth, ref float currentHeight, float maxWidth, float maxHeight, out float ratio)
    {
        var ratioX = maxWidth / currentWidth;
        var ratioY = maxHeight / currentHeight;
        ratio = (float)Math.Min(ratioX, ratioY);

        currentWidth = (currentWidth * ratio);
        currentHeight = (currentHeight * ratio);
    }

    // Token: 0x06001543 RID: 5443 RVA: 0x000B90C8 File Offset: 0x000B72C8
    public void amIPhoneInitNN(Rectangle port)
    {
        OpenGL.init(m_game, m_graphicsDevice);

        var scalar = 1f;
        var scaledWidth = 480f;
        var scaledHeight = 320f;
       
        ScaleProportions(ref scaledWidth, ref scaledHeight, port.Width, port.Height, out scalar);

        OpenGL.glViewport(0, 0, (int)port.Width, (int)port.Height);

        amRenderInit();
        NNS_MATRIX nns_MATRIX = GlobalPool<NNS_MATRIX>.Alloc();
        NNS_MATRIX nns_MATRIX_Viewport = GlobalPool<NNS_MATRIX>.Alloc();
        NNS_MATRIX nns_MATRIX_Scale = GlobalPool<NNS_MATRIX>.Alloc();

        new NNS_VECTOR(0f, 0f, -1f);
        new NNS_RGBA(1f, 1f, 1f, 1f);
        AppMain.NNS_CONFIG_GL config;
        config.WindowWidth = (int)port.Width;
        config.WindowHeight = (int)port.Height;
        this.nnConfigureSystemGL(config);

        nnMakePerspectiveMatrix(nns_MATRIX_Viewport, NNM_DEGtoA32(45f), (float)port.Height / (float)port.Width, 1f, 10000f);
        nnMakeScaleMatrix(nns_MATRIX_Scale, scalar, scalar, scalar);
        nnMultiplyMatrix(nns_MATRIX, nns_MATRIX_Viewport, nns_MATRIX_Scale);
        nnSetProjection(nns_MATRIX, 0);

        _am_draw_video.draw_aspect = (float)port.Height / (float)port.Width;
        _am_draw_video.draw_width = (float)port.Width;
        _am_draw_video.draw_height = (float)port.Height;
        _am_draw_video.disp_width = (float)port.Width;
        _am_draw_video.disp_height = (float)port.Height;
        _am_draw_video.width_2d = (float)scaledWidth;
        _am_draw_video.height_2d = (float)scaledHeight;

        _am_draw_video.scale_x_2d = scaledWidth / port.Width;
        _am_draw_video.scale_y_2d = scaledHeight / port.Height;
        _am_draw_video.base_x_2d = (port.Width - scaledWidth) / 2;
        _am_draw_video.base_y_2d = (port.Height - scaledHeight) / 2;

        _am_draw_video.wide_screen = true;
        _am_draw_video.refresh_rate = 60f;
        _am_draw_video.scalar = scalar * 0.66f;
        
        g_gs_main_sys_info.sys_disp_width = scaledWidth;
        g_gs_main_sys_info.sys_disp_height = scaledHeight;

        if (AppMain.g_gs_main_sys_info.stage_id == 9)
        {
            ObjInitDispParams((short)((double)AppMain.GMD_OBJ_LCD_X * 1.42), (short)((double)AppMain.GMD_OBJ_LCD_X * 1.42), (float)AppMain.GSD_DISP_WIDTH, (float)AppMain.GSD_DISP_HEIGHT);
        }
        else
        {
            ObjInitDispParams((short)AppMain.GSD_DISP_WIDTH, (short)AppMain.GSD_DISP_HEIGHT, (float)AppMain.GSD_DISP_WIDTH, (float)AppMain.GSD_DISP_HEIGHT);
        }  
        
        amRenderInit();
        GlobalPool<NNS_MATRIX>.Release(nns_MATRIX);
    }

    // Token: 0x06001544 RID: 5444 RVA: 0x000B9230 File Offset: 0x000B7430
    public static void amIPhoneExitNN()
    {
        mppAssertNotImpl();
    }

    // Token: 0x06001545 RID: 5445 RVA: 0x000B9232 File Offset: 0x000B7432
    public static void amIPhoneSetTextureAttribute(AppMain.AMS_PARAM_LOAD_TEXTURE param)
    {
        mppAssertNotImpl();
    }

    // Token: 0x06001546 RID: 5446 RVA: 0x000B9234 File Offset: 0x000B7434
    public static bool IsGLExtensionSupported(string extension)
    {
        mppAssertNotImpl();
        return true;
    }
}
