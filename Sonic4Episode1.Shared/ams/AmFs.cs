using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public delegate void FsBackgroundReadComplete(AMS_FS fs);

public class AmFs
{
    static AmFs()
    {
        ams_fsList = new LinkedList<AMS_FS>();
    }

    // Token: 0x04004173 RID: 16755
    private static LinkedList<AMS_FS> ams_fsList;

    // Token: 0x04004175 RID: 16757
    public static AMS_FS lastReadAMS_FS;

    // Token: 0x06000A78 RID: 2680 RVA: 0x0005C9A2 File Offset: 0x0005ABA2
    public static bool amFsIsComplete(AMS_FS cdfsp)
    {
        return cdfsp.stat == 3;
    }

    // Token: 0x06000A79 RID: 2681 RVA: 0x0005C9AD File Offset: 0x0005ABAD
    public static AMS_FS amFsReadBackground(string file_name)
    {
        return amFsReadBackground(file_name, null);
    }

    // Token: 0x06000A7A RID: 2682 RVA: 0x0005C9C0 File Offset: 0x0005ABC0
    public static AMS_FS amFsReadBackground(string file_name, int BytesPerFrame)
    {
        return amFsReadBackground(file_name, null);
    }

    // Token: 0x06000A7B RID: 2683 RVA: 0x0005C9D0 File Offset: 0x0005ABD0
    public static AMS_FS amFsReadBackground(string file_name, FsBackgroundReadComplete callback)
    {
        AMS_FS ams_FS;
        if (ams_fsList.Count > 0 && lastReadAMS_FS != null &&
            lastReadAMS_FS.file_name == file_name)
        {
            ams_FS = ams_fsList.First.Value;
            amFsExecuteBackgroundRead();
        }
        else
        {
            ams_FS = new AMS_FS();
            ams_FS.callback = callback;
            ams_FS.file_name = file_name;
            ams_FS.stat = 2;
            ams_fsList.AddLast(ams_FS);
            lastReadAMS_FS = ams_FS;
        }

        return ams_FS;
    }

    // Token: 0x06000A7C RID: 2684 RVA: 0x0005CA48 File Offset: 0x0005AC48
    public static AMS_AMB_HEADER readAMBFile(AMS_FS fs)
    {
        if (fs.amb_header == null)
        {
            fs.makeAmbHeader();
        }

        return fs.amb_header;
    }

    // Token: 0x06000A7D RID: 2685 RVA: 0x0005CA5E File Offset: 0x0005AC5E
    public static AMS_AMB_HEADER readAMBFile(object data)
    {
        if (data is AMS_AMB_HEADER)
        {
            return (AMS_AMB_HEADER) data;
        }

        if (data is AmbChunk)
        {
            AmbChunk ambChunk = (AmbChunk) data;
            return readAMBFile(data);
        }

        return readAMBFile((AMS_FS) data);
    }

    // Token: 0x06000A7E RID: 2686 RVA: 0x0005CA90 File Offset: 0x0005AC90
    public static AMS_AMB_HEADER readAMBFile(AmbChunk buf)
    {
        byte[] array = buf.array;
        int offset = buf.offset;
        AMS_AMB_HEADER ams_AMB_HEADER = searchPreloadedAmb(buf.amb, buf.offset);
        if (ams_AMB_HEADER != null)
        {
            return ams_AMB_HEADER;
        }

        ams_AMB_HEADER = new AMS_AMB_HEADER();
        using (Stream stream = new MemoryStream(array, offset, array.Length - offset))
        {
            if (offset == 0)
            {
                ams_AMB_HEADER.data = new byte[stream.Length];
                stream.Read(ams_AMB_HEADER.data, 0, ams_AMB_HEADER.data.Length);
            }
            else
            {
                ams_AMB_HEADER.data = array;
            }

            stream.Position = 0L;
            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                var tmp = binaryReader.ReadInt32();
                if (tmp == 0x424d4123)
                {
                    binaryReader.BaseStream.Seek(12, SeekOrigin.Current);
                    ams_AMB_HEADER.file_num = binaryReader.ReadInt32();
                    var entryTableOffset = binaryReader.ReadInt32();
                    binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                    var stringTableOffset = binaryReader.ReadInt32();

                    ams_AMB_HEADER.files = new string[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.types = new sbyte[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.offsets = new int[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.lengths = new int[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.buf = new object[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.flag = new sbyte[0];
                    for (int i = 0; i < ams_AMB_HEADER.file_num; i++)
                    {
                        binaryReader.BaseStream.Seek(entryTableOffset + (i * 0x10), SeekOrigin.Begin);
                        ams_AMB_HEADER.offsets[i] = binaryReader.ReadInt32();
                        ams_AMB_HEADER.lengths[i] = binaryReader.ReadInt32();

                        binaryReader.BaseStream.Seek(stringTableOffset + (i * 0x20), SeekOrigin.Begin);
                        ams_AMB_HEADER.files[i] = readChars(binaryReader);
                    }
                }
                else
                {
                    ams_AMB_HEADER.file_num = tmp;
                    ams_AMB_HEADER.files = new string[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.types = new sbyte[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.offsets = new int[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.lengths = new int[ams_AMB_HEADER.file_num];
                    ams_AMB_HEADER.buf = new object[ams_AMB_HEADER.file_num];
                    int num = binaryReader.ReadInt32();
                    ams_AMB_HEADER.flag = new sbyte[num];
                    for (int i = 0; i < num; i++)
                    {
                        ams_AMB_HEADER.flag[i] = binaryReader.ReadSByte();
                    }

                    for (int j = 0; j < ams_AMB_HEADER.file_num; j++)
                    {
                        ams_AMB_HEADER.files[j] = binaryReader.ReadString();
                        ams_AMB_HEADER.types[j] = binaryReader.ReadSByte();
                    }

                    for (int k = 0; k < ams_AMB_HEADER.file_num; k++)
                    {
                        ams_AMB_HEADER.offsets[k] = binaryReader.ReadInt32();
                    }

                    for (int l = 0; l < ams_AMB_HEADER.file_num; l++)
                    {
                        ams_AMB_HEADER.lengths[l] = binaryReader.ReadInt32();
                    }
                }
            }

            ams_AMB_HEADER.parent = buf.amb;
            amPreLoadAmbItems(ams_AMB_HEADER);
        }

        return ams_AMB_HEADER;
    }

    // Token: 0x06000A7F RID: 2687 RVA: 0x0005CC94 File Offset: 0x0005AE94
    public static AMS_AMB_HEADER searchPreloadedAmb(AMS_AMB_HEADER amb, int offset)
    {
        for (int i = 0; i < amb.file_num; i++)
        {
            if (amb.offsets[i] == offset && amb.buf[i] != null)
            {
                return (AMS_AMB_HEADER) amb.buf[i];
            }
        }

        for (int j = 0; j < amb.file_num; j++)
        {
            if (amb.buf[j] is AMS_AMB_HEADER)
            {
                AMS_AMB_HEADER ams_AMB_HEADER =
                    searchPreloadedAmb((AMS_AMB_HEADER) amb.buf[j], offset);
                if (ams_AMB_HEADER != null)
                {
                    return ams_AMB_HEADER;
                }
            }
        }

        return null;
    }

    // Token: 0x06000A80 RID: 2688 RVA: 0x0005CD10 File Offset: 0x0005AF10
    public static void amPreLoadAmbItems(AMS_AMB_HEADER amb)
    {
        AmbChunk ambChunk = new AmbChunk(amb.data, 0, 0, amb);
        for (int i = 0; i < amb.files.Length; i++)
        {
            ambChunk.offset = amb.offsets[i];
            ambChunk.length = amb.lengths[i];
            string extension = Path.GetExtension(amb.files[i]);
            if (extension == ".INM" || extension == ".INV")
            {
                AppMain.NNS_MOTION nns_MOTION;
                AppMain.amMotionSetup(out nns_MOTION, ambChunk);
                amb.buf[i] = nns_MOTION;
            }
            else if (extension == ".AMB")
            {
                amb.buf[i] = readAMBFile(ambChunk);
                amPreLoadAmbItems((AMS_AMB_HEADER) amb.buf[i]);
            }
            else if (extension == ".AME")
            {
                amb.buf[i] = AmAme.readAMEfile(ambChunk);
            }
        }
    }

    public static void amFsExecuteBackgroundRead()
    {
        if (ams_fsList.Count > 0)
        {
            AMS_FS value = ams_fsList.First.Value;
            if (value.stream == null)
            {
                var stream = TitleContainer.OpenStream("Content\\" + value.file_name);
                value.stream = new MemoryStream();
                value.loadTask = stream.CopyToAsync(value.stream);
            }

            if (value.loadTask != null && !value.loadTask.IsCompleted)
            {
                return;
            }

            value.data = (value.stream as MemoryStream).ToArray();
            value.stream.Position = 0L;

            using (BinaryReader binaryReader = new BinaryReader(value.stream))
            {
                //23 41 4D 42
                var tmp = binaryReader.ReadInt32();
                if (tmp == 0x424d4123)
                {
                    binaryReader.BaseStream.Seek(12, SeekOrigin.Current);
                    value.count = binaryReader.ReadInt32();
                    var entryTableOffset = binaryReader.ReadInt32();
                    binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                    var stringTableOffset = binaryReader.ReadInt32();

                    value.files = new string[value.count];
                    value.types = new sbyte[value.count];
                    value.offsets = new int[value.count];
                    value.lengths = new int[value.count];
                    value.flag = new sbyte[0];
                    for (int i = 0; i < value.count; i++)
                    {
                        binaryReader.BaseStream.Seek(entryTableOffset + (i * 0x10), SeekOrigin.Begin);
                        value.offsets[i] = binaryReader.ReadInt32();
                        value.lengths[i] = binaryReader.ReadInt32();

                        binaryReader.BaseStream.Seek(stringTableOffset + (i * 0x20), SeekOrigin.Begin);
                        value.files[i] = readChars(binaryReader);
                    }
                }
                else
                {
                    value.count = tmp;
                    value.files = new string[value.count];
                    value.types = new sbyte[value.count];
                    value.offsets = new int[value.count];
                    value.lengths = new int[value.count];
                    int num2 = binaryReader.ReadInt32();
                    value.flag = new sbyte[num2];
                    for (int i = 0; i < num2; i++)
                    {
                        value.flag[i] = binaryReader.ReadSByte();
                    }

                    for (int j = 0; j < value.count; j++)
                    {
                        value.files[j] = binaryReader.ReadString();
                        value.types[j] = binaryReader.ReadSByte();
                    }

                    for (int k = 0; k < value.count; k++)
                    {
                        value.offsets[k] = binaryReader.ReadInt32();
                    }

                    for (int l = 0; l < value.count; l++)
                    {
                        value.lengths[l] = binaryReader.ReadInt32();
                    }
                }
            }

            value.makeAmbHeader();
            value.stat = 3;
            value.stream = null;
            ams_fsList.RemoveFirst();
            if (value.callback != null)
            {
                value.callback(value);
            }
        }
    }

    public AMS_FS amFsReadBackground(int file_id, byte[] buf, int cache)
    {
        AppMain.mppAssertNotImpl();
        return null;
    }

    private AMS_FS amFsReadBackground(int afs_id, string file_name, byte[] buf, int cache)
    {
        AppMain.mppAssertNotImpl();
        return null;
    }

    private AMS_FS amFsReadFileList(int afs_id, string file_name, byte[] buf)
    {
        AppMain.mppAssertNotImpl();
        return null;
    }

    public static int amFsRead(string file_name, out byte[] buf)
    {
        int num = 0;
        using (var stream = TitleContainer.OpenStream(file_name))
        using (var altStream = new MemoryStream())
        {
            stream.CopyTo(altStream);
            buf = altStream.ToArray();
        }

        return buf.Length;
    }

    public static byte[] amFsRead(string file_name)
    {
        int num = 0;
        using (var stream = TitleContainer.OpenStream(file_name))
        using (var altStream = new MemoryStream())
        {
            stream.CopyTo(altStream);
            return altStream.ToArray();
        }
    }

    public static int amFsRead(string file_name, out Texture2D buf)
    {
        AppMain.mppAssertNotImpl();
        buf = null;
        return 0;
    }

    public static void amFsClearRequest(AMS_FS cdfsp)
    {
        ams_fsList.Remove(cdfsp);
    }

    public int amFsSearchPartition(string pname)
    {
        return -1;
    }

    public int amFsSearchName(string fname)
    {
        return -1;
    }

    public int amFsReadStart()
    {
        return 1;
    }

    private void amFsReadListNext(AMS_FS cdfsp)
    {
    }

    private void amFsConvertPath(ref string out_name, ref string in_name)
    {
    }

    private string amFsGetColumn(string cp)
    {
        return cp;
    }

    public string amFsGetNextColumn(string cp)
    {
        return this.amFsGetColumn(cp);
    }

    public string amFsGetNumber(string cp, int num)
    {
        return cp;
    }

    public string amFsGetString(string cp, string str)
    {
        return cp;
    }

    public static void skipString(BinaryReader br)
    {
        while (br.ReadChar() != '\0')
        {
        }
    }

    public static string readChars(BinaryReader br)
    {
        var builder = new StringBuilder();
        while (true)
        {
            char c = br.ReadChar();
            if (c == '\0')
            {
                break;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }
}

public class AMS_FS
{
    // Token: 0x0600230D RID: 8973 RVA: 0x00147D6A File Offset: 0x00145F6A
    public static explicit operator AMS_AMB_HEADER(AMS_FS fs)
    {
        return AmFs.readAMBFile(fs);
    }

    // Token: 0x0600230E RID: 8974 RVA: 0x00147D74 File Offset: 0x00145F74
    public void makeAmbHeader()
    {
        this.amb_header = new AMS_AMB_HEADER();
        this.amb_header.dir = this.dir;
        this.amb_header.file_num = this.count;
        this.amb_header.files = new string[this.files.Length];
        this.amb_header.types = new sbyte[this.types.Length];
        this.amb_header.flag = new sbyte[this.flag.Length];
        this.amb_header.offsets = new int[this.files.Length];
        this.amb_header.lengths = new int[this.files.Length];
        this.amb_header.data = this.data;
        this.amb_header.buf = new object[this.count];
        Array.Copy(this.files, 0, this.amb_header.files, 0, this.files.Length);
        Buffer.BlockCopy(this.types, 0, this.amb_header.types, 0, this.types.Length);
        Buffer.BlockCopy(this.flag, 0, this.amb_header.flag, 0, this.flag.Length);
        Buffer.BlockCopy(this.offsets, 0, this.amb_header.offsets, 0, this.offsets.Length * 4);
        Buffer.BlockCopy(this.lengths, 0, this.amb_header.lengths, 0, this.lengths.Length * 4);
        AmFs.amPreLoadAmbItems(this.amb_header);
    }

    // Token: 0x04005507 RID: 21767
    public string dir;

    // Token: 0x04005508 RID: 21768
    public int count;

    // Token: 0x04005509 RID: 21769
    public string[] files;

    // Token: 0x0400550A RID: 21770
    public sbyte[] types;

    // Token: 0x0400550B RID: 21771
    public sbyte[] flag;

    // Token: 0x0400550C RID: 21772
    public sbyte type;

    // Token: 0x0400550D RID: 21773
    public byte[] data;

    // Token: 0x0400550E RID: 21774
    public int[] offsets;

    // Token: 0x0400550F RID: 21775
    public int[] lengths;

    // Token: 0x04005510 RID: 21776
    public sbyte stat;

    // Token: 0x04005511 RID: 21777
    public string file_name;

    // Token: 0x04005512 RID: 21778
    public Stream stream;

    public Task loadTask;

    // Token: 0x04005513 RID: 21779
    public FsBackgroundReadComplete callback;

    // Token: 0x04005514 RID: 21780
    public AMS_AMB_HEADER amb_header;
}