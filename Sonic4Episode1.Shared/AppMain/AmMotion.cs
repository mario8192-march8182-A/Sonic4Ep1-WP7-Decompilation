using System.IO;
using System;

public partial class AppMain
{
    // Token: 0x020003AD RID: 941
    public class AMS_MOTION_BUF
    {
        // Token: 0x04006177 RID: 24951
        public int motion_id;

        // Token: 0x04006178 RID: 24952
        public float frame;

        // Token: 0x04006179 RID: 24953
        public ArrayPointer<NNS_TRS> mbuf;
    }

    // Token: 0x020003AE RID: 942
    public class AMS_MOTION_FILE
    {
        // Token: 0x0400617A RID: 24954
        public object file;

        // Token: 0x0400617B RID: 24955
        public int motion_num;

        // Token: 0x0400617C RID: 24956
        public ArrayPointer<NNS_MOTION> motion;
    }

    // Token: 0x020003AF RID: 943
    public class AMS_MOTION
    {
        // Token: 0x0400617D RID: 24957
        public NNS_OBJECT _object;

        // Token: 0x0400617E RID: 24958
        public int node_num;

        // Token: 0x0400617F RID: 24959
        public readonly AMS_MOTION_FILE[] mtnfile = New<AMS_MOTION_FILE>(4);

        // Token: 0x04006180 RID: 24960
        public int motion_num;

        // Token: 0x04006181 RID: 24961
        public NNS_MOTION[] mtnbuf;

        // Token: 0x04006182 RID: 24962
        public NNS_TRS[] data;

        // Token: 0x04006183 RID: 24963
        public ArrayPointer<NNS_TRS> mmbuf;

        // Token: 0x04006184 RID: 24964
        public readonly AMS_MOTION_BUF[] mbuf = New<AMS_MOTION_BUF>(2);

        // Token: 0x04006185 RID: 24965
        public NNS_OBJECT mmobject;

        // Token: 0x04006186 RID: 24966
        public uint mmobj_size;

        // Token: 0x04006187 RID: 24967
        public int mmotion_num;

        // Token: 0x04006188 RID: 24968
        public NNS_MOTION[] mmtn;

        // Token: 0x04006189 RID: 24969
        public int mmotion_id;

        // Token: 0x0400618A RID: 24970
        public float mmotion_frame;
    }

    // Token: 0x06001A1E RID: 6686 RVA: 0x000EA37A File Offset: 0x000E857A
    private int amMotionId(int file_id, int motion_id)
    {
        return file_id << 16 | motion_id;
    }

    // Token: 0x06001A1F RID: 6687 RVA: 0x000EA382 File Offset: 0x000E8582
    private static AMS_MOTION amMotionCreate(NNS_OBJECT _object)
    {
        return amMotionCreate(_object, 0);
    }

    // Token: 0x06001A20 RID: 6688 RVA: 0x000EA38B File Offset: 0x000E858B
    private static AMS_MOTION amMotionCreate(NNS_OBJECT _object, int flag)
    {
        return amMotionCreate(_object, 64, 16, flag);
    }

    // Token: 0x06001A21 RID: 6689 RVA: 0x000EA398 File Offset: 0x000E8598
    private static AMS_MOTION amMotionCreate(NNS_OBJECT _object, int motion_num, int mmotion_num)
    {
        return amMotionCreate(_object, motion_num, mmotion_num, 0);
    }

    // Token: 0x06001A22 RID: 6690 RVA: 0x000EA3A4 File Offset: 0x000E85A4
    public static AMS_MOTION amMotionCreate(NNS_OBJECT _object, int motion_num, int mmotion_num, int flag)
    {
        motion_num = (motion_num + 3 & -4);
        mmotion_num = (mmotion_num + 3 & -4);
        int nNode = _object.nNode;
        AMS_MOTION ams_MOTION = new AMS_MOTION();
        ams_MOTION.mtnbuf = new NNS_MOTION[motion_num];
        ams_MOTION.mmtn = new NNS_MOTION[mmotion_num];
        ams_MOTION.data = New<NNS_TRS>(((flag & 1) != 0) ? (4 * nNode) : (2 * nNode));
        ams_MOTION._object = _object;
        ams_MOTION.node_num = nNode;
        int i;
        for (i = 0; i < 4; i++)
        {
            ams_MOTION.mtnfile[i].file = null;
            ams_MOTION.mtnfile[i].motion = null;
            ams_MOTION.mtnfile[i].motion_num = 0;
        }

        ams_MOTION.motion_num = motion_num;
        for (i = 0; i < motion_num; i++)
        {
            ams_MOTION.mtnbuf[i] = null;
        }

        ArrayPointer<AMS_MOTION_BUF> pointer = ams_MOTION.mbuf;
        i = 0;
        while (i < 2)
        {
            (~pointer).motion_id = 0;
            (~pointer).frame = 0f;
            if (i == 0)
            {
                (~pointer).mbuf = new ArrayPointer<NNS_TRS>(ams_MOTION.data, nNode);
            }
            else if ((flag & 1) != 0)
            {
                (~pointer).mbuf = ams_MOTION.mbuf[0].mbuf + nNode;
                ams_MOTION.mmbuf = ams_MOTION.mbuf[1].mbuf + nNode;
                nnCalcTRSList(ams_MOTION.mbuf[1].mbuf.array, ams_MOTION.mbuf[1].mbuf.offset, _object);
            }
            else
            {
                (~pointer).mbuf = null;
                ams_MOTION.mmbuf = null;
            }

            i++;
            pointer = ++pointer;
        }

        nnCalcTRSList(ams_MOTION.mbuf[0].mbuf.array, ams_MOTION.mbuf[0].mbuf.offset, _object);
        nnCalcTRSList(ams_MOTION.data, 0, _object);
        ams_MOTION.mmobject = null;
        ams_MOTION.mmobj_size = 0U;
        ams_MOTION.mmotion_num = mmotion_num;
        return ams_MOTION;
    }

    // Token: 0x06001A23 RID: 6691 RVA: 0x000EA595 File Offset: 0x000E8795
    public static void amMotionDelete(AMS_MOTION motion)
    {
    }

    // Token: 0x06001A24 RID: 6692 RVA: 0x000EA598 File Offset: 0x000E8798
    public static void amMotionRegistFile(AMS_MOTION motion, int file_id, AMS_AMB_HEADER amb)
    {
        int num = 0;
        AMS_MOTION_FILE ams_MOTION_FILE = motion.mtnfile[0];
        ArrayPointer<NNS_MOTION> arrayPointer = ams_MOTION_FILE.motion + ams_MOTION_FILE.motion_num;
        num++;
        int i = 1;
        while (i < 4)
        {
            ArrayPointer<NNS_MOTION> arrayPointer2 = motion.mtnfile[num].motion + motion.mtnfile[num].motion_num;
            if (arrayPointer < arrayPointer2)
            {
                arrayPointer = arrayPointer2;
            }

            i++;
            num++;
        }

        if (arrayPointer == null)
        {
            arrayPointer = new ArrayPointer<NNS_MOTION>(motion.mtnbuf, 0);
        }

        ams_MOTION_FILE = motion.mtnfile[file_id];
        ams_MOTION_FILE.file = amb;
        ams_MOTION_FILE.motion = arrayPointer;
        ams_MOTION_FILE.motion_num = amMotionSetup(arrayPointer, amb);
    }

    // Token: 0x06001A25 RID: 6693 RVA: 0x000EA644 File Offset: 0x000E8844
    public static void amMotionRegistFile(AMS_MOTION motion, int file_id, object buf)
    {
        int num = 0;
        AMS_MOTION_FILE ams_MOTION_FILE = motion.mtnfile[0];
        ArrayPointer<NNS_MOTION> arrayPointer = ams_MOTION_FILE.motion + ams_MOTION_FILE.motion_num;
        num++;
        int i = 1;
        while (i < 4)
        {
            ArrayPointer<NNS_MOTION> arrayPointer2 = motion.mtnfile[num].motion + motion.mtnfile[num].motion_num;
            if (arrayPointer < arrayPointer2)
            {
                arrayPointer = arrayPointer2;
            }

            i++;
            num++;
        }

        if (arrayPointer == null)
        {
            arrayPointer = new ArrayPointer<NNS_MOTION>(motion.mtnbuf, 0);
        }

        ams_MOTION_FILE = motion.mtnfile[file_id];
        ams_MOTION_FILE.file = buf;
        ams_MOTION_FILE.motion = arrayPointer;
        if (buf is NNS_MOTION)
        {
            ams_MOTION_FILE.motion_num = 1;
            motion.mtnbuf[0] = (NNS_MOTION) buf;
            return;
        }

        ams_MOTION_FILE.motion_num = amMotionSetup(arrayPointer, buf);
    }

    // Token: 0x06001A26 RID: 6694 RVA: 0x000EA710 File Offset: 0x000E8910
    public static int amMotionSetup(ArrayPointer<NNS_MOTION> motion, AMS_AMB_HEADER amb)
    {
        if (amb.files.Length == 0)
        {
            return 0;
        }

        ArrayPointer<NNS_MOTION> pointer = motion;
        int num = 0;
        for (int i = 0; i < Math.Min(amb.buf.Length, pointer.array.Length); i++)
        {
            if (amb.buf[i] != null && amb.buf[i] is NNS_MOTION)
            {
                pointer.SetPrimitive((NNS_MOTION) amb.buf[i]);
                pointer = ++pointer;
                num++;
            }
        }

        return num;
    }

    // Token: 0x06001A27 RID: 6695 RVA: 0x000EA778 File Offset: 0x000E8978
    public static void amMotionSetup(out NNS_MOTION motion, AmbChunk buf)
    {
        motion = null;
        using (MemoryStream memoryStream = new MemoryStream(buf.array, buf.offset, buf.array.Length - buf.offset))
        {
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            NNS_BINCNK_FILEHEADER nns_BINCNK_FILEHEADER = NNS_BINCNK_FILEHEADER.Read(binaryReader);
            long num;
            binaryReader.BaseStream.Seek(num = nns_BINCNK_FILEHEADER.OfsData, 0x0);
            NNS_BINCNK_DATAHEADER nns_BINCNK_DATAHEADER = NNS_BINCNK_DATAHEADER.Read(binaryReader);
            long num2 = num;
            binaryReader.BaseStream.Seek(nns_BINCNK_FILEHEADER.OfsNOF0, 0x0);
            NNS_BINCNK_NOF0HEADER.Read(binaryReader);
            int i = nns_BINCNK_FILEHEADER.nChunk;
            while (i > 0x0)
            {
                uint id = nns_BINCNK_DATAHEADER.Id;
                if (id <= 0x434D494E)
                {
                    if (id == 0x414D494E || id == 0x434D494E)
                    {
                        goto LOAD_NMO;
                    }
                }
                else
                {
                    if (id == 0x444E454E) // NEND
                    {
                        break;
                    }

                    if (id == 0x4F4D494E || id == 0x4F4D5A4E) // NIMO, NZMO
                    {
                        goto LOAD_NMO;
                    }
                }

                IL_D4:
                i++;
                binaryReader.BaseStream.Seek(num += 0x8 + nns_BINCNK_DATAHEADER.OfsNextId, 0x0);
                nns_BINCNK_DATAHEADER = NNS_BINCNK_DATAHEADER.Read(binaryReader);
                continue;

                LOAD_NMO:
                binaryReader.BaseStream.Seek(num2 + nns_BINCNK_DATAHEADER.OfsMainData, 0x0);
                motion = NNS_MOTION.Read(binaryReader, num2);
                goto IL_D4;
            }
        }
    }

    // Token: 0x06001A28 RID: 6696 RVA: 0x000EA8A8 File Offset: 0x000E8AA8
    public static int amMotionSetup(ArrayPointer<NNS_MOTION> motion, object _buf)
    {
        AmbChunk ambChunk = (AmbChunk) _buf;
        int result;
        using (MemoryStream memoryStream =
            new MemoryStream(ambChunk.array, ambChunk.offset, ambChunk.array.Length - ambChunk.offset))
        {
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            ArrayPointer<NNS_MOTION> pointer = motion;
            int num = 0;
            pointer.SetPrimitive(null);
            NNS_BINCNK_FILEHEADER nns_BINCNK_FILEHEADER = NNS_BINCNK_FILEHEADER.Read(binaryReader);
            long num2;
            binaryReader.BaseStream.Seek(num2 = nns_BINCNK_FILEHEADER.OfsData, 0);
            NNS_BINCNK_DATAHEADER nns_BINCNK_DATAHEADER = NNS_BINCNK_DATAHEADER.Read(binaryReader);
            long num3 = num2;
            binaryReader.BaseStream.Seek(nns_BINCNK_FILEHEADER.OfsNOF0, 0);
            NNS_BINCNK_NOF0HEADER.Read(binaryReader);
            int i = nns_BINCNK_FILEHEADER.nChunk;
            while (i > 0)
            {
                uint id = nns_BINCNK_DATAHEADER.Id;
                if (id <= 1129138510U)
                {
                    if (id == 1095584078U || id == 1129138510U)
                    {
                        goto IL_C8;
                    }
                }
                else
                {
                    if (id == 1145980238U)
                    {
                        break;
                    }

                    if (id == 1330465102U)
                    {
                        goto IL_C8;
                    }
                }

                IL_FF:
                i++;
                binaryReader.BaseStream.Seek(num2 += 8 + nns_BINCNK_DATAHEADER.OfsNextId, 0);
                nns_BINCNK_DATAHEADER = NNS_BINCNK_DATAHEADER.Read(binaryReader);
                continue;
                IL_C8:
                binaryReader.BaseStream.Seek(num3 + nns_BINCNK_DATAHEADER.OfsMainData, 0);
                pointer.SetPrimitive(NNS_MOTION.Read(binaryReader, num3));
                pointer = ++pointer;
                num++;
                goto IL_FF;
            }

            result = num;
        }

        return result;
    }

    // Token: 0x06001A29 RID: 6697 RVA: 0x000EAA18 File Offset: 0x000E8C18
    public static void amMotionSet(AMS_MOTION motion, int mbuf_id, int motion_id)
    {
        AMS_MOTION_BUF ams_MOTION_BUF = motion.mbuf[mbuf_id];
        ams_MOTION_BUF.motion_id = motion_id;
        ams_MOTION_BUF.frame = motion.mtnfile[motion_id >> 16].motion[motion_id & 65535].StartFrame;
    }

    // Token: 0x06001A2A RID: 6698 RVA: 0x000EAA5C File Offset: 0x000E8C5C
    public static void amMotionSetFrame(AMS_MOTION motion, int mbuf_id, float frame)
    {
        AMS_MOTION_BUF ams_MOTION_BUF = motion.mbuf[mbuf_id];
        ams_MOTION_BUF.frame = frame;
    }

    // Token: 0x06001A2B RID: 6699 RVA: 0x000EAA79 File Offset: 0x000E8C79
    public static void amMotionCalc(AMS_MOTION motion)
    {
        amMotionCalc(motion, -1);
    }

    // Token: 0x06001A2C RID: 6700 RVA: 0x000EAA84 File Offset: 0x000E8C84
    public static void amMotionCalc(AMS_MOTION motion, int mbuf_id)
    {
        int i = 0;
        while (i < 2)
        {
            if ((mbuf_id & 1) != 0 && !(motion.mbuf[i].mbuf == null))
            {
                int num = motion.mbuf[i].motion_id;
                int num2 = num >> 16;
                num &= 65535;
                nnCalcTRSListMotion(motion.mbuf[i].mbuf.array, motion.mbuf[i].mbuf.offset, motion._object,
                    motion.mtnfile[num2].motion[num], motion.mbuf[i].frame);
            }

            i++;
            mbuf_id >>= 1;
        }
    }

    // Token: 0x06001A2D RID: 6701 RVA: 0x000EAB34 File Offset: 0x000E8D34
    private void amMotionApply(AMS_MOTION motion)
    {
        amMotionApply(motion, 0f, 1f);
    }

    // Token: 0x06001A2E RID: 6702 RVA: 0x000EAB46 File Offset: 0x000E8D46
    public static void amMotionApply(AMS_MOTION motion, float marge)
    {
        amMotionApply(motion, marge, 1f);
    }

    // Token: 0x06001A2F RID: 6703 RVA: 0x000EAB54 File Offset: 0x000E8D54
    public static void amMotionApply(AMS_MOTION motion, float marge, float per)
    {
        ArrayPointer<NNS_TRS> arrayPointer = motion.mbuf[0].mbuf;
        if (per <= 0f)
        {
            return;
        }

        if (motion.mbuf[1].mbuf != null)
        {
            if (marge >= 1f)
            {
                arrayPointer = motion.mbuf[1].mbuf;
            }
            else if (marge > 0f)
            {
                if (per >= 1f)
                {
                    nnLinkMotion(motion.data, motion.mbuf[0].mbuf, motion.mbuf[1].mbuf, motion.node_num, marge);
                    return;
                }

                arrayPointer = motion.mmbuf;
                nnLinkMotion(arrayPointer, motion.mbuf[0].mbuf, motion.mbuf[1].mbuf, motion.node_num, marge);
            }
        }

        if (per >= 1f)
        {
            for (int i = 0; i < motion.node_num; i++)
            {
                motion.data[i].Assign(arrayPointer[i]);
            }

            return;
        }

        nnLinkMotion(motion.data, motion.data, arrayPointer, motion.node_num, per);
    }

    // Token: 0x06001A30 RID: 6704 RVA: 0x000EAC72 File Offset: 0x000E8E72
    public static void amMotionGet(AMS_MOTION motion)
    {
        amMotionGet(motion, 0f, 1f);
    }

    // Token: 0x06001A31 RID: 6705 RVA: 0x000EAC84 File Offset: 0x000E8E84
    public static void amMotionGet(AMS_MOTION motion, float marge)
    {
        amMotionGet(motion, marge, 1f);
    }

    // Token: 0x06001A32 RID: 6706 RVA: 0x000EAC92 File Offset: 0x000E8E92
    public static void amMotionGet(AMS_MOTION motion, float marge, float per)
    {
        amMotionCalc(motion);
        amMotionApply(motion, marge, per);
    }

    // Token: 0x06001A33 RID: 6707 RVA: 0x000EACA4 File Offset: 0x000E8EA4
    public static void amMotionMaterialRegistFile(AMS_MOTION motion, int file_id, AMS_AMB_HEADER amb)
    {
        int file_num = amb.file_num;
        for (int i = 0; i < file_num; i++)
        {
            motion.mmtn[file_id + i] = (NNS_MOTION) amb.buf[i];
        }

        motion.mmotion_id = file_id;
        motion.mmotion_frame = 0f;
        motion.mmobj_size = 0U;
        motion.mmobject = new NNS_OBJECT();
        nnInitMaterialMotionObject(motion.mmobject, motion._object, motion.mmtn[motion.mmotion_id]);
    }

    // Token: 0x06001A34 RID: 6708 RVA: 0x000EAD20 File Offset: 0x000E8F20
    private static void amMotionMaterialRegistFile(AMS_MOTION motion, int file_id, object file)
    {
        motion.mmtn[file_id] = (NNS_MOTION) file;
        motion.mmotion_id = file_id;
        motion.mmotion_frame = 0f;
        motion.mmobj_size = 0U;
        motion.mmobject = new NNS_OBJECT();
        nnInitMaterialMotionObject(motion.mmobject, motion._object, motion.mmtn[motion.mmotion_id]);
    }

    // Token: 0x06001A35 RID: 6709 RVA: 0x000EAD7D File Offset: 0x000E8F7D
    public static void amMotionMaterialSet(AMS_MOTION motion, int motion_id)
    {
        motion.mmotion_id = motion_id;
        motion.mmotion_frame = 0f;
        nnInitMaterialMotionObject(motion.mmobject, motion._object, motion.mmtn[motion.mmotion_id]);
    }

    // Token: 0x06001A36 RID: 6710 RVA: 0x000EADAF File Offset: 0x000E8FAF
    public static void amMotionMaterialSetFrame(AMS_MOTION motion, float frame)
    {
        motion.mmotion_frame = frame;
    }

    // Token: 0x06001A37 RID: 6711 RVA: 0x000EADB8 File Offset: 0x000E8FB8
    public static void amMotionMaterialCalc(AMS_MOTION motion)
    {
        if (!amThreadCheckDraw())
        {
            return;
        }

        nnCalcMaterialMotion(motion.mmobject, motion._object, motion.mmtn[motion.mmotion_id], motion.mmotion_frame);
    }

    // Token: 0x06001A38 RID: 6712 RVA: 0x000EADE6 File Offset: 0x000E8FE6
    private void amMotionDraw(uint state, AMS_MOTION motion, NNS_TEXLIST texlist)
    {
        amMotionDraw(state, motion, texlist, 0U, null);
    }

    // Token: 0x06001A39 RID: 6713 RVA: 0x000EADF3 File Offset: 0x000E8FF3
    private void amMotionDraw(uint state, AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag)
    {
        amMotionDraw(state, motion, texlist, drawflag, null);
    }

    // Token: 0x06001A3A RID: 6714 RVA: 0x000EAE04 File Offset: 0x000E9004
    private void amMotionDraw(uint state, AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag,
        NNS_MATERIALCALLBACK_FUNC func)
    {
        int node_num = motion.node_num;
        AMS_PARAM_DRAW_MOTION_TRS ams_PARAM_DRAW_MOTION_TRS = amDrawAlloc_AMS_PARAM_DRAW_MOTION_TRS();
        NNS_MATRIX nns_MATRIX = ams_PARAM_DRAW_MOTION_TRS.mtx = amDrawAlloc_NNS_MATRIX();
        nnCopyMatrix(nns_MATRIX, amMatrixGetCurrent());
        ams_PARAM_DRAW_MOTION_TRS._object = motion._object;
        ams_PARAM_DRAW_MOTION_TRS.mtx = nns_MATRIX;
        ams_PARAM_DRAW_MOTION_TRS.sub_obj_type = 0U;
        ams_PARAM_DRAW_MOTION_TRS.flag = drawflag;
        ams_PARAM_DRAW_MOTION_TRS.texlist = texlist;
        ams_PARAM_DRAW_MOTION_TRS.trslist = new NNS_TRS[node_num];
        ams_PARAM_DRAW_MOTION_TRS.material_func = func;
        for (int i = 0; i < node_num; i++)
        {
            ams_PARAM_DRAW_MOTION_TRS.trslist[i] = amDrawAlloc_NNS_TRS();
            ams_PARAM_DRAW_MOTION_TRS.trslist[i].Assign(motion.data[i]);
        }

        int motion_id = motion.mbuf[0].motion_id;
        ams_PARAM_DRAW_MOTION_TRS.motion = motion.mtnfile[motion_id >> 16].motion[motion_id & 65535];
        ams_PARAM_DRAW_MOTION_TRS.frame = motion.mbuf[0].frame;
        amDrawRegistCommand(state, -11, ams_PARAM_DRAW_MOTION_TRS);
    }

    // Token: 0x06001A3B RID: 6715 RVA: 0x000EAEF5 File Offset: 0x000E90F5
    private void amMotionMaterialDraw(uint state, AMS_MOTION motion, NNS_TEXLIST texlist)
    {
        amMotionMaterialDraw(state, motion, texlist, 0U, null);
    }

    // Token: 0x06001A3C RID: 6716 RVA: 0x000EAF02 File Offset: 0x000E9102
    private void amMotionMaterialDraw(uint state, AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag)
    {
        amMotionMaterialDraw(state, motion, texlist, drawflag, null);
    }

    // Token: 0x06001A3D RID: 6717 RVA: 0x000EAF10 File Offset: 0x000E9110
    private void amMotionMaterialDraw(uint state, AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag,
        NNS_MATERIALCALLBACK_FUNC func)
    {
        if (motion.mmobject == null)
        {
            amMotionDraw(state, motion, texlist, drawflag);
            return;
        }

        int node_num = motion.node_num;
        AMS_PARAM_DRAW_MOTION_TRS ams_PARAM_DRAW_MOTION_TRS = new AMS_PARAM_DRAW_MOTION_TRS();
        NNS_MATRIX nns_MATRIX = ams_PARAM_DRAW_MOTION_TRS.mtx = GlobalPool<NNS_MATRIX>.Alloc();
        nnCopyMatrix(nns_MATRIX, amMatrixGetCurrent());
        ams_PARAM_DRAW_MOTION_TRS.mtx = nns_MATRIX;
        ams_PARAM_DRAW_MOTION_TRS.sub_obj_type = 0U;
        ams_PARAM_DRAW_MOTION_TRS.flag = drawflag;
        ams_PARAM_DRAW_MOTION_TRS.texlist = texlist;
        ams_PARAM_DRAW_MOTION_TRS.trslist = new NNS_TRS[node_num];
        ams_PARAM_DRAW_MOTION_TRS.material_func = func;
        for (int i = 0; i < node_num; i++)
        {
            ams_PARAM_DRAW_MOTION_TRS.trslist[i] = new NNS_TRS(motion.data[i]);
        }

        ams_PARAM_DRAW_MOTION_TRS._object = motion._object;
        ams_PARAM_DRAW_MOTION_TRS.mmotion = motion.mmtn[motion.mmotion_id];
        ams_PARAM_DRAW_MOTION_TRS.mframe = motion.mmotion_frame;
        int motion_id = motion.mbuf[0].motion_id;
        if (motion.mtnfile[motion_id >> 16].file != null)
        {
            ams_PARAM_DRAW_MOTION_TRS.motion = motion.mtnfile[motion_id >> 16].motion[motion_id & 65535];
            ams_PARAM_DRAW_MOTION_TRS.frame = motion.mbuf[0].frame;
        }
        else
        {
            ams_PARAM_DRAW_MOTION_TRS.motion = null;
            ams_PARAM_DRAW_MOTION_TRS.frame = 0f;
        }

        amDrawRegistCommand(state, -12, ams_PARAM_DRAW_MOTION_TRS);
    }

    // Token: 0x06001A3E RID: 6718 RVA: 0x000EB04D File Offset: 0x000E924D
    private void amMotionDraw(AMS_MOTION motion, NNS_TEXLIST texlist)
    {
        amMotionDraw(motion, texlist, 0U, null);
    }

    // Token: 0x06001A3F RID: 6719 RVA: 0x000EB059 File Offset: 0x000E9259
    private void amMotionDraw(AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag)
    {
        amMotionDraw(motion, texlist, drawflag, null);
    }

    // Token: 0x06001A40 RID: 6720 RVA: 0x000EB068 File Offset: 0x000E9268
    private void amMotionDraw(AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag, NNS_MATERIALCALLBACK_FUNC func)
    {
        AMS_COMMAND_HEADER ams_COMMAND_HEADER = new AMS_COMMAND_HEADER();
        AMS_PARAM_DRAW_MOTION_TRS ams_PARAM_DRAW_MOTION_TRS =
            (AMS_PARAM_DRAW_MOTION_TRS) (ams_COMMAND_HEADER.param = new AMS_PARAM_DRAW_MOTION_TRS());
        ams_COMMAND_HEADER.command_id = -11;
        ams_COMMAND_HEADER.param = ams_PARAM_DRAW_MOTION_TRS;
        ams_PARAM_DRAW_MOTION_TRS._object = motion._object;
        ams_PARAM_DRAW_MOTION_TRS.mtx = null;
        ams_PARAM_DRAW_MOTION_TRS.sub_obj_type = 0U;
        ams_PARAM_DRAW_MOTION_TRS.flag = drawflag;
        ams_PARAM_DRAW_MOTION_TRS.texlist = texlist;
        ams_PARAM_DRAW_MOTION_TRS.trslist = motion.data;
        ams_PARAM_DRAW_MOTION_TRS.material_func = func;
        int motion_id = motion.mbuf[0].motion_id;
        ams_PARAM_DRAW_MOTION_TRS.motion = motion.mtnfile[motion_id >> 16].motion[motion_id & 65535];
        ams_PARAM_DRAW_MOTION_TRS.frame = motion.mbuf[0].frame;
        _amDrawMotionTRS(ams_COMMAND_HEADER, drawflag);
    }

    // Token: 0x06001A41 RID: 6721 RVA: 0x000EB11D File Offset: 0x000E931D
    private void amMotionMaterialDraw(AMS_MOTION motion, NNS_TEXLIST texlist)
    {
        amMotionMaterialDraw(motion, texlist, 0U, null);
    }

    // Token: 0x06001A42 RID: 6722 RVA: 0x000EB129 File Offset: 0x000E9329
    private void amMotionMaterialDraw(AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag)
    {
        amMotionMaterialDraw(motion, texlist, drawflag, null);
    }

    // Token: 0x06001A43 RID: 6723 RVA: 0x000EB138 File Offset: 0x000E9338
    private void amMotionMaterialDraw(AMS_MOTION motion, NNS_TEXLIST texlist, uint drawflag,
        NNS_MATERIALCALLBACK_FUNC func)
    {
        if (motion.mmobject == null)
        {
            amMotionDraw(motion, texlist, drawflag);
            return;
        }

        AMS_COMMAND_HEADER ams_COMMAND_HEADER = new AMS_COMMAND_HEADER();
        AMS_PARAM_DRAW_MOTION_TRS ams_PARAM_DRAW_MOTION_TRS =
            (AMS_PARAM_DRAW_MOTION_TRS) (ams_COMMAND_HEADER.param = new AMS_PARAM_DRAW_MOTION_TRS());
        ams_COMMAND_HEADER.command_id = -12;
        ams_COMMAND_HEADER.param = ams_PARAM_DRAW_MOTION_TRS;
        ams_PARAM_DRAW_MOTION_TRS._object = motion.mmobject;
        ams_PARAM_DRAW_MOTION_TRS.mtx = null;
        ams_PARAM_DRAW_MOTION_TRS.sub_obj_type = 0U;
        ams_PARAM_DRAW_MOTION_TRS.flag = drawflag;
        ams_PARAM_DRAW_MOTION_TRS.texlist = texlist;
        ams_PARAM_DRAW_MOTION_TRS.trslist = motion.data;
        ams_PARAM_DRAW_MOTION_TRS.material_func = func;
        ams_PARAM_DRAW_MOTION_TRS.mmotion = null;
        ams_PARAM_DRAW_MOTION_TRS.mframe = 0f;
        int motion_id = motion.mbuf[0].motion_id;
        if (motion.mtnfile[motion_id >> 16].file != null)
        {
            ams_PARAM_DRAW_MOTION_TRS.motion = motion.mtnfile[motion_id >> 16].motion[motion_id & 65535];
            ams_PARAM_DRAW_MOTION_TRS.frame = motion.mbuf[0].frame;
        }
        else
        {
            ams_PARAM_DRAW_MOTION_TRS.motion = null;
            ams_PARAM_DRAW_MOTION_TRS.frame = 0f;
        }

        _amDrawMotionTRS(ams_COMMAND_HEADER, drawflag);
    }

    // Token: 0x06001A44 RID: 6724 RVA: 0x000EB237 File Offset: 0x000E9437
    public static float amMotionGetStartFrame(AMS_MOTION motion, int motion_id)
    {
        return motion.mtnfile[motion_id >> 16].motion[motion_id & 65535].StartFrame;
    }

    // Token: 0x06001A45 RID: 6725 RVA: 0x000EB25A File Offset: 0x000E945A
    public static float amMotionGetEndFrame(AMS_MOTION motion, int motion_id)
    {
        return motion.mtnfile[motion_id >> 16].motion[motion_id & 65535].EndFrame;
    }

    // Token: 0x06001A46 RID: 6726 RVA: 0x000EB280 File Offset: 0x000E9480
    private void amMotionGetFrames(AMS_MOTION motion, int motion_id, out float start, out float end)
    {
        NNS_MOTION nns_MOTION = motion.mtnfile[motion_id >> 16].motion[motion_id & 65535];
        start = nns_MOTION.StartFrame;
        end = nns_MOTION.EndFrame;
    }

    // Token: 0x06001A47 RID: 6727 RVA: 0x000EB2BB File Offset: 0x000E94BB
    public static float amMotionMaterialGetStartFrame(AMS_MOTION motion, int motion_id)
    {
        return motion.mmtn[motion.mmotion_id].StartFrame;
    }

    // Token: 0x06001A48 RID: 6728 RVA: 0x000EB2CF File Offset: 0x000E94CF
    public static float amMotionMaterialGetEndFrame(AMS_MOTION motion, int motion_id)
    {
        return motion.mmtn[motion.mmotion_id].EndFrame;
    }

    // Token: 0x06001A49 RID: 6729 RVA: 0x000EB2E4 File Offset: 0x000E94E4
    private void amMotionMaterialGetFrames(AMS_MOTION motion, int motion_id, out float start, out float end)
    {
        NNS_MOTION nns_MOTION = motion.mmtn[motion.mmotion_id];
        start = nns_MOTION.StartFrame;
        end = nns_MOTION.EndFrame;
    }

    // Token: 0x06001A4A RID: 6730 RVA: 0x000EB310 File Offset: 0x000E9510
    public static int AMM_BLEND(int equ, int a, int b)
    {
        return equ << 8 | b << 4 | a;
    }
}