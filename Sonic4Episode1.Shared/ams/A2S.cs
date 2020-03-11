using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;

public class A2S
{
    private static readonly Dictionary<int, A2S_AMA_NODE> nodeHash
        = new Dictionary<int, A2S_AMA_NODE>(100);
     private static readonly Dictionary<int, A2S_AMA_ACT> actHash
         = new Dictionary<int, A2S_AMA_ACT>(100);
     private static readonly Dictionary<int, A2S_AMA_MTN> mtnHash
         = new Dictionary<int, A2S_AMA_MTN>(100);
     private static readonly Dictionary<int, A2S_AMA_ANM> anmHash
         = new Dictionary<int, A2S_AMA_ANM>(100);
     private static readonly Dictionary<int, A2S_AMA_ACM> acmHash
         = new Dictionary<int, A2S_AMA_ACM>(100);
     private static readonly Dictionary<int, A2S_AMA_USR> usrHash
         = new Dictionary<int, A2S_AMA_USR>(100);
     private static readonly Dictionary<int, A2S_AMA_HIT> hitHash
         = new Dictionary<int, A2S_AMA_HIT>(100);
     private static readonly Dictionary<int, A2S_SUB_TRS[]> subtrsHash
         = new Dictionary<int, A2S_SUB_TRS[]>(100);
     private static readonly Dictionary<int, A2S_SUB_MTN[]> submtnHash
         = new Dictionary<int, A2S_SUB_MTN[]>(100);
     private static readonly Dictionary<int, A2S_SUB_ANM[]> subanmHash
         = new Dictionary<int, A2S_SUB_ANM[]>(100);
     private static readonly Dictionary<int, A2S_SUB_MAT[]> submatHash
         = new Dictionary<int, A2S_SUB_MAT[]>(100);
     private static readonly Dictionary<int, A2S_SUB_ACM[]> subacmHash
         = new Dictionary<int, A2S_SUB_ACM[]>(100);
     private static readonly Dictionary<int, A2S_SUB_USR[]> subusrHash
         = new Dictionary<int, A2S_SUB_USR[]>(100);
     private static readonly Dictionary<int, A2S_SUB_HIT[]> subhitHash
         = new Dictionary<int, A2S_SUB_HIT[]>(100);
     private static readonly Dictionary<int, A2S_SUB_KEY[]> subkeyHash
         = new Dictionary<int, A2S_SUB_KEY[]>(100);

     // Token: 0x06001867 RID: 6247 RVA: 0x000DB95C File Offset: 0x000D9B5C
    public static A2S_AMA_HEADER readAMAFile(string name)
    {
        A2S_AMA_HEADER result = null;
        using (var stream = TitleContainer.OpenStream("Content\\" + name))
        {
            using (var binaryReader = new BinaryReader(stream))
            {
                result = readAMAFile(binaryReader);
            }
        }

        return result;
    }

    // Token: 0x06001868 RID: 6248 RVA: 0x000DB9C0 File Offset: 0x000D9BC0
    public static A2S_AMA_HEADER readAMAFile(object data)
    {
        if (data is A2S_AMA_HEADER)
        {
            return (A2S_AMA_HEADER) data;
        }

        var ambChunk = (AmbChunk) data;
        return readAMAFile(ambChunk.array, ambChunk.offset);
    }

    // Token: 0x06001869 RID: 6249 RVA: 0x000DB9F4 File Offset: 0x000D9BF4
    public static A2S_AMA_HEADER readAMAFile(byte[] data, int offset)
    {
        A2S_AMA_HEADER result = null;
        using (var memoryStream = new MemoryStream(data, offset, data.Length - offset))
        {
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                result = readAMAFile(binaryReader);
            }
        }

        return result;
    }

    // Token: 0x0600186A RID: 6250 RVA: 0x000DBA54 File Offset: 0x000D9C54
    public static A2S_AMA_HEADER readAMAFile(BinaryReader br)
    {
        var header = new A2S_AMA_HEADER();
        br.BaseStream.Seek(4L, SeekOrigin.Current);
        header.version = br.ReadUInt32();
        header.node_num = br.ReadUInt32();
        header.act_num = br.ReadUInt32();
        header.node_tbl_offset = br.ReadInt32();
        header.act_tbl_offset = br.ReadInt32();
        header.node_name_tbl_offset = br.ReadInt32();
        header.act_name_tbl_offset = br.ReadInt32();
        header.node_tbl = new A2S_AMA_NODE[header.node_num];
        header.act_tbl = new A2S_AMA_ACT[header.act_num];
        if (header.node_tbl_offset != 0)
        {
            br.BaseStream.Seek((long) header.node_tbl_offset, 0);
            var num = 0;
            while ((long) num < (long) ((ulong) header.node_num))
            {
                var num2 = br.ReadInt32();
                if (num2 != 0)
                {
                    if (!nodeHash.ContainsKey(num2))
                    {
                        header.node_tbl[num] = new A2S_AMA_NODE();
                        header.node_tbl[num]._off = num2;
                        nodeHash.Add(header.node_tbl[num]._off,
                            header.node_tbl[num]);
                    }
                    else
                    {
                        header.node_tbl[num] = nodeHash[num2];
                    }
                }

                num++;
            }

            var num3 = 0;
            while ((long) num3 < (long) ((ulong) header.node_num))
            {
                br.BaseStream.Seek((long) header.node_tbl[num3]._off, 0);
                header.node_tbl[num3].flag = br.ReadUInt32();
                header.node_tbl[num3].id = br.ReadUInt32();
                header.node_tbl[num3].child_offset = br.ReadInt32();
                if (header.node_tbl[num3].child_offset != 0)
                {
                    if (!nodeHash.ContainsKey(header.node_tbl[num3].child_offset))
                    {
                        header.node_tbl[num3].child = new A2S_AMA_NODE();
                        nodeHash.Add(header.node_tbl[num3].child_offset,
                            header.node_tbl[num3].child);
                    }
                    else
                    {
                        header.node_tbl[num3].child =
                            nodeHash[header.node_tbl[num3].child_offset];
                    }
                }

                header.node_tbl[num3].sibling_offset = br.ReadInt32();
                if (header.node_tbl[num3].sibling_offset != 0)
                {
                    if (!nodeHash.ContainsKey(header.node_tbl[num3].sibling_offset))
                    {
                        header.node_tbl[num3].sibling = new A2S_AMA_NODE();
                        nodeHash.Add(header.node_tbl[num3].sibling_offset,
                            header.node_tbl[num3].sibling);
                    }
                    else
                    {
                        header.node_tbl[num3].sibling =
                            nodeHash[header.node_tbl[num3].sibling_offset];
                    }
                }

                header.node_tbl[num3].parent_offset = br.ReadInt32();
                if (header.node_tbl[num3].parent_offset != 0)
                {
                    if (!nodeHash.ContainsKey(header.node_tbl[num3].parent_offset))
                    {
                        header.node_tbl[num3].parent = new A2S_AMA_NODE();
                        nodeHash.Add(header.node_tbl[num3].parent_offset,
                            header.node_tbl[num3].parent);
                    }
                    else
                    {
                        header.node_tbl[num3].parent =
                            nodeHash[header.node_tbl[num3].parent_offset];
                    }
                }

                header.node_tbl[num3].act_offset = br.ReadInt32();
                if (header.node_tbl[num3].act_offset != 0)
                {
                    if (!actHash.ContainsKey(header.node_tbl[num3].act_offset))
                    {
                        header.node_tbl[num3].act = new A2S_AMA_ACT();
                        header.node_tbl[num3].act._off = header.node_tbl[num3].act_offset;
                        actHash.Add(header.node_tbl[num3].act_offset,
                            header.node_tbl[num3].act);
                    }
                    else
                    {
                        header.node_tbl[num3].act =
                            actHash[header.node_tbl[num3].act_offset];
                    }
                }

                br.BaseStream.Seek(8L, SeekOrigin.Current);
                num3++;
            }

            br.BaseStream.Seek((long) header.node_name_tbl_offset, 0);
            var array = new int[header.node_num];
            var num4 = 0;
            while ((long) num4 < (long) ((ulong) header.node_num))
            {
                array[num4] = br.ReadInt32();
                num4++;
            }

            var num5 = 0;
            while ((long) num5 < (long) ((ulong) header.node_num))
            {
                br.BaseStream.Seek((long) array[num5], 0);
                AmFs.skipString(br);
                num5++;
            }
        }

        if (header.act_tbl_offset != 0)
        {
            br.BaseStream.Seek((long) header.act_tbl_offset, 0);
            var num6 = 0;
            while ((long) num6 < (long) ((ulong) header.act_num))
            {
                var num7 = br.ReadInt32();
                if (!actHash.ContainsKey(num7))
                {
                    header.act_tbl[num6] = new A2S_AMA_ACT();
                    header.act_tbl[num6]._off = num7;
                    actHash.Add(header.act_tbl[num6]._off, header.act_tbl[num6]);
                }
                else
                {
                    header.act_tbl[num6] = actHash[num7];
                }

                num6++;
            }

            var num8 = 0;
            while ((long) num8 < (long) ((ulong) header.act_num))
            {
                br.BaseStream.Seek((long) header.act_tbl[num8]._off, 0);
                header.act_tbl[num8].flag = br.ReadUInt32();
                header.act_tbl[num8].id = br.ReadUInt32();
                header.act_tbl[num8].frm_num = br.ReadUInt32();
                header.act_tbl[num8].pad1 = br.ReadUInt32();
                header.act_tbl[num8].ofst.left = br.ReadSingle();
                header.act_tbl[num8].ofst.top = br.ReadSingle();
                header.act_tbl[num8].ofst.right = br.ReadSingle();
                header.act_tbl[num8].ofst.bottom = br.ReadSingle();
                header.act_tbl[num8].mtn_offset = br.ReadInt32();
                if (header.act_tbl[num8].mtn_offset != 0)
                {
                    if (!mtnHash.ContainsKey(header.act_tbl[num8].mtn_offset))
                    {
                        header.act_tbl[num8].mtn = new A2S_AMA_MTN();
                        mtnHash.Add(header.act_tbl[num8].mtn_offset,
                            header.act_tbl[num8].mtn);
                    }
                    else
                    {
                        header.act_tbl[num8].mtn =
                            mtnHash[header.act_tbl[num8].mtn_offset];
                    }
                }

                header.act_tbl[num8].anm_offset = br.ReadInt32();
                if (header.act_tbl[num8].anm_offset != 0)
                {
                    if (!anmHash.ContainsKey(header.act_tbl[num8].anm_offset))
                    {
                        header.act_tbl[num8].anm = new A2S_AMA_ANM();
                        anmHash.Add(header.act_tbl[num8].anm_offset,
                            header.act_tbl[num8].anm);
                    }
                    else
                    {
                        header.act_tbl[num8].anm =
                            anmHash[header.act_tbl[num8].anm_offset];
                    }
                }

                header.act_tbl[num8].acm_offset = br.ReadInt32();
                if (header.act_tbl[num8].acm_offset != 0)
                {
                    if (!acmHash.ContainsKey(header.act_tbl[num8].acm_offset))
                    {
                        header.act_tbl[num8].acm = new A2S_AMA_ACM();
                        acmHash.Add(header.act_tbl[num8].acm_offset,
                            header.act_tbl[num8].acm);
                    }
                    else
                    {
                        header.act_tbl[num8].acm =
                            acmHash[header.act_tbl[num8].acm_offset];
                    }
                }

                header.act_tbl[num8].usr_offset = br.ReadInt32();
                if (header.act_tbl[num8].usr_offset != 0)
                {
                    if (!usrHash.ContainsKey(header.act_tbl[num8].usr_offset))
                    {
                        header.act_tbl[num8].usr = new A2S_AMA_USR();
                        usrHash.Add(header.act_tbl[num8].usr_offset,
                            header.act_tbl[num8].usr);
                    }
                    else
                    {
                        header.act_tbl[num8].usr =
                            usrHash[header.act_tbl[num8].usr_offset];
                    }
                }

                header.act_tbl[num8].hit_offset = br.ReadInt32();
                if (header.act_tbl[num8].hit_offset != 0)
                {
                    if (!hitHash.ContainsKey(header.act_tbl[num8].hit_offset))
                    {
                        header.act_tbl[num8].hit = new A2S_AMA_HIT();
                        hitHash.Add(header.act_tbl[num8].hit_offset,
                            header.act_tbl[num8].hit);
                    }
                    else
                    {
                        header.act_tbl[num8].hit =
                            hitHash[header.act_tbl[num8].hit_offset];
                    }
                }

                header.act_tbl[num8].next_offset = br.ReadInt32();
                if (header.act_tbl[num8].next_offset != 0)
                {
                    if (!actHash.ContainsKey(header.act_tbl[num8].next_offset))
                    {
                        header.act_tbl[num8].next = new A2S_AMA_ACT();
                        actHash.Add(header.act_tbl[num8].next_offset,
                            header.act_tbl[num8].next);
                    }
                    else
                    {
                        header.act_tbl[num8].next =
                            actHash[header.act_tbl[num8].next_offset];
                    }
                }

                br.BaseStream.Seek(8L, SeekOrigin.Current);
                num8++;
            }

            br.BaseStream.Seek((long) header.act_name_tbl_offset, 0);
            var array2 = new int[header.act_num];
            var num9 = 0;
            while ((long) num9 < (long) ((ulong) header.act_num))
            {
                array2[num9] = br.ReadInt32();
                num9++;
            }

            var num10 = 0;
            while ((long) num10 < (long) ((ulong) header.act_num))
            {
                br.BaseStream.Seek((long) array2[num10], 0);
                AmFs.skipString(br);
                num10++;
            }

            foreach (var mtnVal in mtnHash)
            {
                br.BaseStream.Seek((long) mtnVal.Key, 0);
                var mtn = mtnVal.Value;
                mtn.flag = br.ReadUInt32();
                mtn.mtn_key_num = br.ReadUInt32();
                mtn.mtn_frm_num = br.ReadUInt32();
                mtn.mtn_key_tbl_offset = br.ReadInt32();
                if (mtn.mtn_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(mtn.mtn_key_tbl_offset))
                    {
                        mtn.mtn_key_tbl = new A2S_SUB_KEY[mtn.mtn_key_num + 1U];
                        subkeyHash.Add(mtn.mtn_key_tbl_offset, mtn.mtn_key_tbl);
                    }
                    else
                    {
                        mtn.mtn_key_tbl = subkeyHash[mtn.mtn_key_tbl_offset];
                    }
                }

                mtn.mtn_tbl_offset = br.ReadInt32();
                if (mtn.mtn_tbl_offset != 0)
                {
                    if (!submtnHash.ContainsKey(mtn.mtn_tbl_offset))
                    {
                        mtn.mtn_tbl = new A2S_SUB_MTN[mtn.mtn_key_num + 1U];
                        submtnHash.Add(mtn.mtn_tbl_offset, mtn.mtn_tbl);
                    }
                    else
                    {
                        mtn.mtn_tbl = submtnHash[mtn.mtn_tbl_offset];
                    }
                }

                mtn.trs_key_num = br.ReadUInt32();
                mtn.trs_frm_num = br.ReadUInt32();
                mtn.trs_key_tbl_offset = br.ReadInt32();
                if (mtn.trs_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(mtn.trs_key_tbl_offset))
                    {
                        mtn.trs_key_tbl = new A2S_SUB_KEY[mtn.trs_key_num + 1U];
                        subkeyHash.Add(mtn.trs_key_tbl_offset, mtn.trs_key_tbl);
                    }
                    else
                    {
                        mtn.trs_key_tbl = subkeyHash[mtn.trs_key_tbl_offset];
                    }
                }

                mtn.trs_tbl_offset = br.ReadInt32();
                if (mtn.trs_tbl_offset != 0)
                {
                    if (!subtrsHash.ContainsKey(mtn.trs_tbl_offset))
                    {
                        mtn.trs_tbl = new A2S_SUB_TRS[mtn.trs_key_num + 1U];
                        subtrsHash.Add(mtn.trs_tbl_offset, mtn.trs_tbl);
                    }
                    else
                    {
                        mtn.trs_tbl = subtrsHash[mtn.trs_tbl_offset];
                    }
                }
            }

            foreach (var anmVal in anmHash)
            {
                br.BaseStream.Seek((long) anmVal.Key, 0);
                var anm = anmVal.Value;
                anm.flag = br.ReadUInt32();
                anm.anm_key_num = br.ReadUInt32();
                anm.anm_frm_num = br.ReadUInt32();
                anm.anm_key_tbl_offset = br.ReadInt32();
                if (anm.anm_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(anm.anm_key_tbl_offset))
                    {
                        anm.anm_key_tbl = new A2S_SUB_KEY[anm.anm_key_num + 1U];
                        subkeyHash.Add(anm.anm_key_tbl_offset, anm.anm_key_tbl);
                    }
                    else
                    {
                        anm.anm_key_tbl = subkeyHash[anm.anm_key_tbl_offset];
                    }
                }

                anm.anm_tbl_offset = br.ReadInt32();
                if (anm.anm_tbl_offset != 0)
                {
                    if (!subanmHash.ContainsKey(anm.anm_tbl_offset))
                    {
                        anm.anm_tbl = new A2S_SUB_ANM[anm.anm_key_num + 1U];
                        subanmHash.Add(anm.anm_tbl_offset, anm.anm_tbl);
                    }
                    else
                    {
                        anm.anm_tbl = subanmHash[anm.anm_tbl_offset];
                    }
                }

                anm.mat_key_num = br.ReadUInt32();
                anm.mat_frm_num = br.ReadUInt32();
                anm.mat_key_tbl_offset = br.ReadInt32();
                if (anm.mat_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(anm.mat_key_tbl_offset))
                    {
                        anm.mat_key_tbl = new A2S_SUB_KEY[anm.mat_key_num + 1U];
                        subkeyHash.Add(anm.mat_key_tbl_offset, anm.mat_key_tbl);
                    }
                    else
                    {
                        anm.mat_key_tbl = subkeyHash[anm.mat_key_tbl_offset];
                    }
                }

                anm.mat_tbl_offset = br.ReadInt32();
                if (anm.mat_tbl_offset != 0)
                {
                    if (!submatHash.ContainsKey(anm.mat_tbl_offset))
                    {
                        anm.mat_tbl = new A2S_SUB_MAT[anm.mat_key_num + 1U];
                        submatHash.Add(anm.mat_tbl_offset, anm.mat_tbl);
                    }
                    else
                    {
                        anm.mat_tbl = submatHash[anm.mat_tbl_offset];
                    }
                }

                br.BaseStream.Seek(12L, SeekOrigin.Current);
            }

            foreach (var keyValuePair3 in acmHash)
            {
                br.BaseStream.Seek((long) keyValuePair3.Key, 0);
                var value3 = keyValuePair3.Value;
                value3.flag = br.ReadUInt32();
                value3.acm_key_num = br.ReadUInt32();
                value3.acm_frm_num = br.ReadUInt32();
                value3.acm_key_tbl_offset = br.ReadInt32();
                if (value3.acm_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(value3.acm_key_tbl_offset))
                    {
                        value3.acm_key_tbl = new A2S_SUB_KEY[value3.acm_key_num + 1U];
                        subkeyHash.Add(value3.acm_key_tbl_offset, value3.acm_key_tbl);
                    }
                    else
                    {
                        value3.acm_key_tbl = subkeyHash[value3.acm_key_tbl_offset];
                    }
                }

                value3.acm_tbl_offset = br.ReadInt32();
                if (value3.acm_tbl_offset != 0)
                {
                    if (!subacmHash.ContainsKey(value3.acm_tbl_offset))
                    {
                        value3.acm_tbl = new A2S_SUB_ACM[value3.acm_key_num + 1U];
                        subacmHash.Add(value3.acm_tbl_offset, value3.acm_tbl);
                    }
                    else
                    {
                        value3.acm_tbl = subacmHash[value3.acm_tbl_offset];
                    }
                }

                value3.trs_key_num = br.ReadUInt32();
                value3.trs_frm_num = br.ReadUInt32();
                value3.trs_key_tbl_offset = br.ReadInt32();
                if (value3.trs_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(value3.trs_key_tbl_offset))
                    {
                        value3.trs_key_tbl = new A2S_SUB_KEY[value3.trs_key_num + 1U];
                        subkeyHash.Add(value3.trs_key_tbl_offset, value3.trs_key_tbl);
                    }
                    else
                    {
                        value3.trs_key_tbl = subkeyHash[value3.trs_key_tbl_offset];
                    }
                }

                value3.trs_tbl_offset = br.ReadInt32();
                if (value3.trs_tbl_offset != 0)
                {
                    if (!subtrsHash.ContainsKey(value3.trs_tbl_offset))
                    {
                        value3.trs_tbl = new A2S_SUB_TRS[value3.trs_key_num + 1U];
                        subtrsHash.Add(value3.trs_tbl_offset, value3.trs_tbl);
                    }
                    else
                    {
                        value3.trs_tbl = subtrsHash[value3.trs_tbl_offset];
                    }
                }

                value3.mat_key_num = br.ReadUInt32();
                value3.mat_frm_num = br.ReadUInt32();
                value3.mat_key_tbl_offset = br.ReadInt32();
                if (value3.mat_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(value3.mat_key_tbl_offset))
                    {
                        value3.mat_key_tbl = new A2S_SUB_KEY[value3.mat_key_num + 1U];
                        subkeyHash.Add(value3.mat_key_tbl_offset, value3.mat_key_tbl);
                    }
                    else
                    {
                        value3.mat_key_tbl = subkeyHash[value3.mat_key_tbl_offset];
                    }
                }

                value3.mat_tbl_offset = br.ReadInt32();
                if (value3.mat_tbl_offset != 0)
                {
                    if (!submatHash.ContainsKey(value3.mat_tbl_offset))
                    {
                        value3.mat_tbl = new A2S_SUB_MAT[value3.mat_key_num + 1U];
                        submatHash.Add(value3.mat_tbl_offset, value3.mat_tbl);
                    }
                    else
                    {
                        value3.mat_tbl = submatHash[value3.mat_tbl_offset];
                    }
                }

                br.BaseStream.Seek(12L, SeekOrigin.Current);
            }

            foreach (var keyValuePair4 in usrHash)
            {
                br.BaseStream.Seek((long) keyValuePair4.Key, 0);
                var value4 = keyValuePair4.Value;
                value4.flag = br.ReadUInt32();
                value4.usr_key_num = br.ReadUInt32();
                value4.usr_frm_num = br.ReadUInt32();
                value4.usr_key_tbl_offset = br.ReadInt32();
                if (value4.usr_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(value4.usr_key_tbl_offset))
                    {
                        value4.usr_key_tbl = new A2S_SUB_KEY[value4.usr_key_num + 1U];
                        subkeyHash.Add(value4.usr_key_tbl_offset, value4.usr_key_tbl);
                    }
                    else
                    {
                        value4.usr_key_tbl = subkeyHash[value4.usr_key_tbl_offset];
                    }
                }

                value4.usr_tbl_offset = br.ReadInt32();
                if (value4.usr_tbl_offset != 0)
                {
                    if (!subusrHash.ContainsKey(value4.usr_tbl_offset))
                    {
                        value4.usr_tbl = new A2S_SUB_USR[value4.usr_key_num + 1U];
                        subusrHash.Add(value4.usr_tbl_offset, value4.usr_tbl);
                    }
                    else
                    {
                        value4.usr_tbl = subusrHash[value4.usr_tbl_offset];
                    }
                }

                br.BaseStream.Seek(12L, SeekOrigin.Current);
            }

            foreach (var keyValuePair5 in hitHash)
            {
                br.BaseStream.Seek((long) keyValuePair5.Key, 0);
                var value5 = keyValuePair5.Value;
                value5.flag = br.ReadUInt32();
                value5.hit_key_num = br.ReadUInt32();
                value5.hit_frm_num = br.ReadUInt32();
                value5.hit_key_tbl_offset = br.ReadInt32();
                if (value5.hit_key_tbl_offset != 0)
                {
                    if (!subkeyHash.ContainsKey(value5.hit_key_tbl_offset))
                    {
                        value5.hit_key_tbl = new A2S_SUB_KEY[value5.hit_key_num + 1U];
                        subkeyHash.Add(value5.hit_key_tbl_offset, value5.hit_key_tbl);
                    }
                    else
                    {
                        value5.hit_key_tbl = subkeyHash[value5.hit_key_tbl_offset];
                    }
                }

                value5.hit_tbl_offset = br.ReadInt32();
                if (value5.hit_tbl_offset != 0)
                {
                    if (!subhitHash.ContainsKey(value5.hit_tbl_offset))
                    {
                        value5.hit_tbl = new A2S_SUB_HIT[value5.hit_key_num + 1U];
                        subhitHash.Add(value5.hit_tbl_offset, value5.hit_tbl);
                    }
                    else
                    {
                        value5.hit_tbl = subhitHash[value5.hit_tbl_offset];
                    }
                }

                br.BaseStream.Seek(12L, SeekOrigin.Current);
            }

            foreach (var keyValuePair6 in subtrsHash)
            {
                br.BaseStream.Seek((long) keyValuePair6.Key, 0);
                var value6 = keyValuePair6.Value;
                var num11 = keyValuePair6.Value.Length;
                for (var i = 0; i < num11; i++)
                {
                    value6[i] = new A2S_SUB_TRS();
                    value6[i].trs_x = br.ReadSingle();
                    value6[i].trs_y = br.ReadSingle();
                    value6[i].trs_z = br.ReadSingle();
                    value6[i].trs_accele = br.ReadSingle();
                }
            }

            foreach (var keyValuePair7 in submtnHash)
            {
                br.BaseStream.Seek((long) keyValuePair7.Key, 0);
                var value7 = keyValuePair7.Value;
                var num12 = keyValuePair7.Value.Length;
                for (var j = 0; j < num12; j++)
                {
                    value7[j] = new A2S_SUB_MTN();
                    value7[j].scl_x = br.ReadSingle();
                    value7[j].scl_y = br.ReadSingle();
                    value7[j].rot = br.ReadSingle();
                    value7[j].scl_accele = br.ReadSingle();
                    value7[j].rot_accele = br.ReadSingle();
                    br.BaseStream.Seek(12L, SeekOrigin.Current);
                }
            }

            try
            {
                foreach (var keyValuePair8 in subanmHash)
                {
                    br.BaseStream.Seek((long) keyValuePair8.Key, 0);
                    var value8 = keyValuePair8.Value;
                    var num13 = keyValuePair8.Value.Length;
                    for (var k = 0; k < num13; k++)
                    {
                        value8[k] = new A2S_SUB_ANM();
                        value8[k].tex_id = br.ReadInt32();
                        value8[k].clamp = br.ReadUInt32();
                        value8[k].filter = br.ReadUInt32();
                        value8[k].texel_accele = br.ReadSingle();
                        value8[k].texel.left = br.ReadSingle();
                        value8[k].texel.top = br.ReadSingle();
                        value8[k].texel.right = br.ReadSingle();
                        value8[k].texel.bottom = br.ReadSingle();
                    }
                }
            }
            catch (EndOfStreamException)
            {
            }

            foreach (var keyValuePair9 in submatHash)
            {
                br.BaseStream.Seek((long) keyValuePair9.Key, 0);
                var value9 = keyValuePair9.Value;
                var num14 = keyValuePair9.Value.Length - 1;
                for (var l = 0; l < num14; l++)
                {
                    value9[l] = new A2S_SUB_MAT();
                    value9[l].base_.a = br.ReadByte();
                    value9[l].base_.b = br.ReadByte();
                    value9[l].base_.g = br.ReadByte();
                    value9[l].base_.r = br.ReadByte();
                    value9[l].fade.a = br.ReadByte();
                    value9[l].fade.b = br.ReadByte();
                    value9[l].fade.g = br.ReadByte();
                    value9[l].fade.r = br.ReadByte();
                    value9[l].base_accele = br.ReadSingle();
                    value9[l].fade_accele = br.ReadSingle();
                    value9[l].blend = br.ReadUInt32();
                    br.BaseStream.Seek(12L, SeekOrigin.Current);
                }
            }

            foreach (var keyValuePair10 in subacmHash)
            {
                br.BaseStream.Seek((long) keyValuePair10.Key, 0);
                var value10 = keyValuePair10.Value;
                var num15 = keyValuePair10.Value.Length;
                for (var m = 0; m < num15; m++)
                {
                    value10[m] = default(A2S_SUB_ACM);
                    value10[m].trs_scl_x = br.ReadSingle();
                    value10[m].trs_scl_y = br.ReadSingle();
                    value10[m].scl_x = br.ReadSingle();
                    value10[m].scl_y = br.ReadSingle();
                    value10[m].rot = br.ReadSingle();
                    value10[m].trs_scl_accele = br.ReadSingle();
                    value10[m].scl_accele = br.ReadSingle();
                    value10[m].rot_accele = br.ReadSingle();
                }
            }

            foreach (var keyValuePair11 in subusrHash)
            {
                br.BaseStream.Seek((long) keyValuePair11.Key, 0);
                var value11 = keyValuePair11.Value;
                var num16 = keyValuePair11.Value.Length;
                for (var n = 0; n < num16; n++)
                {
                    value11[n].usr_id = br.ReadUInt32();
                    br.BaseStream.Seek(12L, SeekOrigin.Current);
                    value11[n].usr_accele = br.ReadSingle();
                    br.BaseStream.Seek(12L, SeekOrigin.Current);
                }
            }

            foreach (var keyValuePair12 in subhitHash)
            {
                br.BaseStream.Seek((long) keyValuePair12.Key, 0);
                var value12 = keyValuePair12.Value;
                var num17 = keyValuePair12.Value.Length;
                for (var num18 = 0; num18 < num17; num18++)
                {
                    value12[num18] = new A2S_SUB_HIT();
                    value12[num18].flag = br.ReadUInt32();
                    value12[num18].type = br.ReadUInt32();
                    value12[num18].hit_accele = br.ReadSingle();
                    value12[num18].pad = br.ReadUInt32();
                    value12[num18].rect.left = br.ReadSingle();
                    value12[num18].rect.top = br.ReadSingle();
                    value12[num18].rect.right = br.ReadSingle();
                    value12[num18].rect.bottom = br.ReadSingle();
                    value12[num18].circle.center_x = value12[num18].rect.left;
                    value12[num18].circle.radius = value12[num18].rect.right;
                    value12[num18].circle.pad = (uint) value12[num18].rect.bottom;
                }
            }

            foreach (var keyValuePair13 in subkeyHash)
            {
                br.BaseStream.Seek((long) keyValuePair13.Key, 0);
                var value13 = keyValuePair13.Value;
                var num19 = keyValuePair13.Value.Length;
                for (var num20 = 0; num20 < num19; num20++)
                {
                    value13[num20] = default(A2S_SUB_KEY);
                    value13[num20].frm = br.ReadUInt32();
                    value13[num20].interpol = br.ReadUInt32();
                }
            }
        }

        nodeHash.Clear();
        actHash.Clear();
        mtnHash.Clear();
        anmHash.Clear();
        acmHash.Clear();
        usrHash.Clear();
        hitHash.Clear();
        subtrsHash.Clear();
        submtnHash.Clear();
        subanmHash.Clear();
        submatHash.Clear();
        subacmHash.Clear();
        subusrHash.Clear();
        subhitHash.Clear();
        subkeyHash.Clear();
        return header;
    }
}

public struct A2S_SUB_KEY
{
    // Token: 0x040060FE RID: 24830
    public uint frm;

    // Token: 0x040060FF RID: 24831
    public uint interpol;
}

public class A2S_SUB_HIT
{
    // Token: 0x060026DC RID: 9948 RVA: 0x00150B5C File Offset: 0x0014ED5C
    internal void Assign(A2S_SUB_HIT old)
    {
        this.flag = old.flag;
        this.type = old.type;
        this.hit_accele = old.hit_accele;
        this.pad = old.pad;
        this.rect = old.rect;
        this.circle = old.circle;
    }

    // Token: 0x040060F8 RID: 24824
    public uint flag;

    // Token: 0x040060F9 RID: 24825
    public uint type;

    // Token: 0x040060FA RID: 24826
    public float hit_accele;

    // Token: 0x040060FB RID: 24827
    public uint pad;

    // Token: 0x040060FC RID: 24828
    public A2S_SUB_RECT rect;

    // Token: 0x040060FD RID: 24829
    public A2S_SUB_CIRCLE circle;
}

public struct A2S_SUB_USR
{
    // Token: 0x040060F6 RID: 24822
    public uint usr_id;

    // Token: 0x040060F7 RID: 24823
    public float usr_accele;
}

public struct A2S_SUB_ACM
{
    // Token: 0x040060EE RID: 24814
    public float trs_scl_x;

    // Token: 0x040060EF RID: 24815
    public float trs_scl_y;

    // Token: 0x040060F0 RID: 24816
    public float scl_x;

    // Token: 0x040060F1 RID: 24817
    public float scl_y;

    // Token: 0x040060F2 RID: 24818
    public float rot;

    // Token: 0x040060F3 RID: 24819
    public float trs_scl_accele;

    // Token: 0x040060F4 RID: 24820
    public float scl_accele;

    // Token: 0x040060F5 RID: 24821
    public float rot_accele;
}

public class A2S_SUB_MAT
{
    // Token: 0x060026DA RID: 9946 RVA: 0x00150B16 File Offset: 0x0014ED16
    internal void Assign(A2S_SUB_MAT old)
    {
        this.base_ = old.base_;
        this.fade = old.fade;
        this.base_accele = old.base_accele;
        this.fade_accele = old.fade_accele;
        this.blend = old.blend;
    }

    // Token: 0x040060E9 RID: 24809
    public A2S_SUB_COL base_;

    // Token: 0x040060EA RID: 24810
    public A2S_SUB_COL fade;

    // Token: 0x040060EB RID: 24811
    public float base_accele;

    // Token: 0x040060EC RID: 24812
    public float fade_accele;

    // Token: 0x040060ED RID: 24813
    public uint blend;
}

public class A2S_SUB_ANM
{
    // Token: 0x060026D8 RID: 9944 RVA: 0x00150AD0 File Offset: 0x0014ECD0
    internal void Assign(A2S_SUB_ANM old)
    {
        this.tex_id = old.tex_id;
        this.clamp = old.clamp;
        this.filter = old.filter;
        this.texel_accele = old.texel_accele;
        this.texel = old.texel;
    }

    // Token: 0x040060E4 RID: 24804
    public int tex_id;

    // Token: 0x040060E5 RID: 24805
    public uint clamp;

    // Token: 0x040060E6 RID: 24806
    public uint filter;

    // Token: 0x040060E7 RID: 24807
    public float texel_accele;

    // Token: 0x040060E8 RID: 24808
    public A2S_SUB_RECT texel;
}

public class A2S_SUB_MTN
{
    // Token: 0x060026D6 RID: 9942 RVA: 0x00150A8A File Offset: 0x0014EC8A
    internal void Assign(A2S_SUB_MTN old)
    {
        this.scl_x = old.scl_x;
        this.scl_y = old.scl_y;
        this.rot = old.rot;
        this.scl_accele = old.scl_accele;
        this.rot_accele = old.rot_accele;
    }

    // Token: 0x040060DF RID: 24799
    public float scl_x;

    // Token: 0x040060E0 RID: 24800
    public float scl_y;

    // Token: 0x040060E1 RID: 24801
    public float rot;

    // Token: 0x040060E2 RID: 24802
    public float scl_accele;

    // Token: 0x040060E3 RID: 24803
    public float rot_accele;
}

public class A2S_SUB_TRS
{
    // Token: 0x060026D4 RID: 9940 RVA: 0x00150A50 File Offset: 0x0014EC50
    internal void Assign(A2S_SUB_TRS old)
    {
        this.trs_x = old.trs_x;
        this.trs_y = old.trs_y;
        this.trs_z = old.trs_z;
        this.trs_accele = old.trs_accele;
    }

    // Token: 0x040060DB RID: 24795
    public float trs_x;

    // Token: 0x040060DC RID: 24796
    public float trs_y;

    // Token: 0x040060DD RID: 24797
    public float trs_z;

    // Token: 0x040060DE RID: 24798
    public float trs_accele;
}

public class A2S_AMA_HIT
{
    // Token: 0x060026D2 RID: 9938 RVA: 0x001509A0 File Offset: 0x0014EBA0
    internal void Assign(A2S_AMA_HIT old)
    {
        this.flag = old.flag;
        this.hit_key_num = old.hit_key_num;
        this.hit_frm_num = old.hit_frm_num;
        if (old.hit_key_tbl != null)
        {
            this.hit_key_tbl = new A2S_SUB_KEY[old.hit_key_tbl.Length];
            Array.Copy((Array) old.hit_key_tbl, (Array) this.hit_key_tbl, (int) old.hit_key_tbl.Length);
        }

        if (old.hit_tbl != null)
        {
            this.hit_tbl = AppMain.New<A2S_SUB_HIT>(old.hit_tbl.Length);
            for (var i = 0; i < this.hit_tbl.Length; i++)
            {
                this.hit_tbl[i].Assign(old.hit_tbl[i]);
            }
        }
    }

    // Token: 0x040060D4 RID: 24788
    public uint flag;

    // Token: 0x040060D5 RID: 24789
    public uint hit_key_num;

    // Token: 0x040060D6 RID: 24790
    public uint hit_frm_num;

    // Token: 0x040060D7 RID: 24791
    public int hit_key_tbl_offset;

    // Token: 0x040060D8 RID: 24792
    public A2S_SUB_KEY[] hit_key_tbl;

    // Token: 0x040060D9 RID: 24793
    public int hit_tbl_offset;

    // Token: 0x040060DA RID: 24794
    public A2S_SUB_HIT[] hit_tbl;
}

public class A2S_AMA_USR
{
    // Token: 0x060026D0 RID: 9936 RVA: 0x001508FC File Offset: 0x0014EAFC
    internal void Assign(A2S_AMA_USR old)
    {
        this.flag = old.flag;
        this.usr_key_num = old.usr_key_num;
        this.usr_frm_num = old.usr_frm_num;
        if (old.usr_key_tbl != null)
        {
            this.usr_key_tbl = new A2S_SUB_KEY[old.usr_key_tbl.Length];
            Array.Copy((Array) old.usr_key_tbl, (Array) this.usr_key_tbl, (int) old.usr_key_tbl.Length);
        }

        if (old.usr_tbl != null)
        {
            this.usr_tbl = new A2S_SUB_USR[old.usr_tbl.Length];
            Array.Copy((Array) old.usr_tbl, (Array) this.usr_tbl, (int) old.usr_tbl.Length);
        }
    }

    // Token: 0x040060CD RID: 24781
    public uint flag;

    // Token: 0x040060CE RID: 24782
    public uint usr_key_num;

    // Token: 0x040060CF RID: 24783
    public uint usr_frm_num;

    // Token: 0x040060D0 RID: 24784
    public int usr_key_tbl_offset;

    // Token: 0x040060D1 RID: 24785
    public A2S_SUB_KEY[] usr_key_tbl;

    // Token: 0x040060D2 RID: 24786
    public int usr_tbl_offset;

    // Token: 0x040060D3 RID: 24787
    public A2S_SUB_USR[] usr_tbl;
}

public class A2S_AMA_ACM
{
    // Token: 0x060026CE RID: 9934 RVA: 0x0015073C File Offset: 0x0014E93C
    internal void Assign(A2S_AMA_ACM old)
    {
        this.flag = old.flag;
        this.acm_key_num = old.acm_key_num;
        this.acm_frm_num = old.acm_frm_num;
        if (old.acm_key_tbl != null)
        {
            this.acm_key_tbl = new A2S_SUB_KEY[old.acm_key_tbl.Length];
            Array.Copy((Array) old.acm_key_tbl, (Array) this.acm_key_tbl, (int) old.acm_key_tbl.Length);
        }

        if (old.acm_tbl != null)
        {
            this.acm_tbl = new A2S_SUB_ACM[old.acm_tbl.Length];
            Array.Copy((Array) old.acm_tbl, (Array) this.acm_tbl, (int) old.acm_tbl.Length);
        }

        this.trs_key_num = old.trs_key_num;
        this.trs_frm_num = old.trs_frm_num;
        if (old.trs_key_tbl != null)
        {
            this.trs_key_tbl = new A2S_SUB_KEY[old.trs_key_tbl.Length];
            Array.Copy((Array) old.trs_key_tbl, (Array) this.trs_key_tbl, (int) old.trs_key_tbl.Length);
        }

        if (old.trs_tbl != null)
        {
            this.trs_tbl = AppMain.New<A2S_SUB_TRS>(old.trs_tbl.Length);
            for (var i = 0; i < this.trs_tbl.Length; i++)
            {
                this.trs_tbl[i].Assign(old.trs_tbl[i]);
            }
        }

        this.mat_key_num = old.mat_key_num;
        this.mat_frm_num = old.mat_frm_num;
        if (old.mat_key_tbl != null)
        {
            this.mat_key_tbl = new A2S_SUB_KEY[old.mat_key_tbl.Length];
            Array.Copy((Array) old.mat_key_tbl, (Array) this.mat_key_tbl, (int) old.mat_key_tbl.Length);
        }

        if (old.mat_tbl != null)
        {
            this.mat_tbl = AppMain.New<A2S_SUB_MAT>(old.mat_tbl.Length);
            for (var j = 0; j < this.mat_tbl.Length; j++)
            {
                this.mat_tbl[j].Assign(old.mat_tbl[j]);
            }
        }
    }

    // Token: 0x040060BA RID: 24762
    public uint flag;

    // Token: 0x040060BB RID: 24763
    public uint acm_key_num;

    // Token: 0x040060BC RID: 24764
    public uint acm_frm_num;

    // Token: 0x040060BD RID: 24765
    public int acm_key_tbl_offset;

    // Token: 0x040060BE RID: 24766
    public A2S_SUB_KEY[] acm_key_tbl;

    // Token: 0x040060BF RID: 24767
    public int acm_tbl_offset;

    // Token: 0x040060C0 RID: 24768
    public A2S_SUB_ACM[] acm_tbl;

    // Token: 0x040060C1 RID: 24769
    public uint trs_key_num;

    // Token: 0x040060C2 RID: 24770
    public uint trs_frm_num;

    // Token: 0x040060C3 RID: 24771
    public int trs_key_tbl_offset;

    // Token: 0x040060C4 RID: 24772
    public A2S_SUB_KEY[] trs_key_tbl;

    // Token: 0x040060C5 RID: 24773
    public int trs_tbl_offset;

    // Token: 0x040060C6 RID: 24774
    public A2S_SUB_TRS[] trs_tbl;

    // Token: 0x040060C7 RID: 24775
    public uint mat_key_num;

    // Token: 0x040060C8 RID: 24776
    public uint mat_frm_num;

    // Token: 0x040060C9 RID: 24777
    public int mat_key_tbl_offset;

    // Token: 0x040060CA RID: 24778
    public A2S_SUB_KEY[] mat_key_tbl;

    // Token: 0x040060CB RID: 24779
    public int mat_tbl_offset;

    // Token: 0x040060CC RID: 24780
    public A2S_SUB_MAT[] mat_tbl;
}

public class A2S_AMA_ANM
{
    // Token: 0x060026CC RID: 9932 RVA: 0x001505FC File Offset: 0x0014E7FC
    internal void Assign(A2S_AMA_ANM old)
    {
        this.flag = old.flag;
        this.anm_key_num = old.anm_key_num;
        this.anm_frm_num = old.anm_frm_num;
        if (old.anm_key_tbl != null)
        {
            this.anm_key_tbl = new A2S_SUB_KEY[old.anm_key_tbl.Length];
            Array.Copy((Array) old.anm_key_tbl, (Array) this.anm_key_tbl, (int) old.anm_key_tbl.Length);
        }

        if (old.anm_tbl != null)
        {
            this.anm_tbl = AppMain.New<A2S_SUB_ANM>(old.anm_tbl.Length);
            for (var i = 0; i < this.anm_tbl.Length; i++)
            {
                this.anm_tbl[i].Assign(old.anm_tbl[i]);
            }
        }

        this.mat_key_num = old.mat_key_num;
        this.mat_frm_num = old.mat_frm_num;
        if (old.mat_key_tbl != null)
        {
            this.mat_key_tbl = new A2S_SUB_KEY[old.mat_key_tbl.Length];
            Array.Copy((Array) old.mat_key_tbl, (Array) this.mat_key_tbl, (int) old.mat_key_tbl.Length);
        }

        if (old.mat_tbl != null)
        {
            this.mat_tbl = AppMain.New<A2S_SUB_MAT>(old.mat_tbl.Length);
            var num = 0;
            while ((long) num < (long) ((ulong) old.mat_key_num))
            {
                this.mat_tbl[num].Assign(old.mat_tbl[num]);
                num++;
            }
        }
    }

    // Token: 0x040060AD RID: 24749
    public uint flag;

    // Token: 0x040060AE RID: 24750
    public uint anm_key_num;

    // Token: 0x040060AF RID: 24751
    public uint anm_frm_num;

    // Token: 0x040060B0 RID: 24752
    public int anm_key_tbl_offset;

    // Token: 0x040060B1 RID: 24753
    public A2S_SUB_KEY[] anm_key_tbl;

    // Token: 0x040060B2 RID: 24754
    public int anm_tbl_offset;

    // Token: 0x040060B3 RID: 24755
    public A2S_SUB_ANM[] anm_tbl;

    // Token: 0x040060B4 RID: 24756
    public uint mat_key_num;

    // Token: 0x040060B5 RID: 24757
    public uint mat_frm_num;

    // Token: 0x040060B6 RID: 24758
    public int mat_key_tbl_offset;

    // Token: 0x040060B7 RID: 24759
    public A2S_SUB_KEY[] mat_key_tbl;

    // Token: 0x040060B8 RID: 24760
    public int mat_tbl_offset;

    // Token: 0x040060B9 RID: 24761
    public A2S_SUB_MAT[] mat_tbl;
}

public class A2S_AMA_MTN
{
    // Token: 0x060026CA RID: 9930 RVA: 0x001504BC File Offset: 0x0014E6BC
    internal void Assign(A2S_AMA_MTN old)
    {
        this.flag = old.flag;
        this.mtn_key_num = old.mtn_key_num;
        this.mtn_frm_num = old.mtn_frm_num;
        if (old.mtn_key_tbl != null)
        {
            this.mtn_key_tbl = new A2S_SUB_KEY[old.mtn_key_tbl.Length];
            Array.Copy(old.mtn_key_tbl, old.mtn_key_tbl, old.mtn_key_tbl.Length);
        }

        if (old.mtn_tbl != null)
        {
            this.mtn_tbl = AppMain.New<A2S_SUB_MTN>(old.mtn_tbl.Length);
            for (var i = 0; i < this.mtn_tbl.Length; i++)
            {
                this.mtn_tbl[i].Assign(old.mtn_tbl[i]);
            }
        }

        this.trs_key_num = old.trs_key_num;
        this.trs_frm_num = old.trs_frm_num;
        if (old.trs_key_tbl != null)
        {
            this.trs_key_tbl = new A2S_SUB_KEY[old.trs_key_tbl.Length];
            Array.Copy(old.trs_key_tbl, this.trs_key_tbl, old.trs_key_tbl.Length);
        }

        if (old.trs_tbl != null)
        {
            this.trs_tbl = AppMain.New<A2S_SUB_TRS>(old.trs_tbl.Length);
            for (var j = 0; j < this.trs_tbl.Length; j++)
            {
                this.trs_tbl[j].Assign(old.trs_tbl[j]);
            }
        }
    }

    // Token: 0x040060A0 RID: 24736
    public uint flag;

    // Token: 0x040060A1 RID: 24737
    public uint mtn_key_num;

    // Token: 0x040060A2 RID: 24738
    public uint mtn_frm_num;

    // Token: 0x040060A3 RID: 24739
    public int mtn_key_tbl_offset;

    // Token: 0x040060A4 RID: 24740
    public A2S_SUB_KEY[] mtn_key_tbl;

    // Token: 0x040060A5 RID: 24741
    public int mtn_tbl_offset;

    // Token: 0x040060A6 RID: 24742
    public A2S_SUB_MTN[] mtn_tbl;

    // Token: 0x040060A7 RID: 24743
    public uint trs_key_num;

    // Token: 0x040060A8 RID: 24744
    public uint trs_frm_num;

    // Token: 0x040060A9 RID: 24745
    public int trs_key_tbl_offset;

    // Token: 0x040060AA RID: 24746
    public A2S_SUB_KEY[] trs_key_tbl;

    // Token: 0x040060AB RID: 24747
    public int trs_tbl_offset;

    // Token: 0x040060AC RID: 24748
    public A2S_SUB_TRS[] trs_tbl;
}

public class A2S_AMA_ACT
{
    // Token: 0x060026C8 RID: 9928 RVA: 0x001503B4 File Offset: 0x0014E5B4
    public void Assign(A2S_AMA_ACT old)
    {
        this.flag = old.flag;
        this.id = old.flag;
        this.frm_num = old.frm_num;
        this.pad1 = old.pad1;
        this.ofst = old.ofst;
        if (old.mtn != null)
        {
            this.mtn = new A2S_AMA_MTN();
            this.mtn.Assign(old.mtn);
        }

        if (old.anm != null)
        {
            this.anm = new A2S_AMA_ANM();
            this.anm.Assign(old.anm);
        }

        if (old.acm != null)
        {
            this.acm = new A2S_AMA_ACM();
            this.acm.Assign(old.acm);
        }

        if (old.usr != null)
        {
            this.usr = new A2S_AMA_USR();
            this.usr.Assign(old.usr);
        }

        if (old.hit != null)
        {
            this.hit = new A2S_AMA_HIT();
            this.hit.Assign(old.hit);
        }
    }

    // Token: 0x0400608E RID: 24718
    public int _off;

    // Token: 0x0400608F RID: 24719
    public uint flag;

    // Token: 0x04006090 RID: 24720
    public uint id;

    // Token: 0x04006091 RID: 24721
    public uint frm_num;

    // Token: 0x04006092 RID: 24722
    public uint pad1;

    // Token: 0x04006093 RID: 24723
    public A2S_SUB_RECT ofst;

    // Token: 0x04006094 RID: 24724
    public int mtn_offset;

    // Token: 0x04006095 RID: 24725
    public A2S_AMA_MTN mtn;

    // Token: 0x04006096 RID: 24726
    public int anm_offset;

    // Token: 0x04006097 RID: 24727
    public A2S_AMA_ANM anm;

    // Token: 0x04006098 RID: 24728
    public int acm_offset;

    // Token: 0x04006099 RID: 24729
    public A2S_AMA_ACM acm;

    // Token: 0x0400609A RID: 24730
    public int usr_offset;

    // Token: 0x0400609B RID: 24731
    public A2S_AMA_USR usr;

    // Token: 0x0400609C RID: 24732
    public int hit_offset;

    // Token: 0x0400609D RID: 24733
    public A2S_AMA_HIT hit;

    // Token: 0x0400609E RID: 24734
    public int next_offset;

    // Token: 0x0400609F RID: 24735
    public A2S_AMA_ACT next;
}

public class A2S_AMA_NODE
{
    // Token: 0x04006083 RID: 24707
    public int _off;

    // Token: 0x04006084 RID: 24708
    public uint flag;

    // Token: 0x04006085 RID: 24709
    public uint id;

    // Token: 0x04006086 RID: 24710
    public int child_offset;

    // Token: 0x04006087 RID: 24711
    public A2S_AMA_NODE child;

    // Token: 0x04006088 RID: 24712
    public int sibling_offset;

    // Token: 0x04006089 RID: 24713
    public A2S_AMA_NODE sibling;

    // Token: 0x0400608A RID: 24714
    public int parent_offset;

    // Token: 0x0400608B RID: 24715
    public A2S_AMA_NODE parent;

    // Token: 0x0400608C RID: 24716
    public int act_offset;

    // Token: 0x0400608D RID: 24717
    public A2S_AMA_ACT act;
}

public struct A2S_SUB_COL
{
    // Token: 0x0400607F RID: 24703
    public byte a;

    // Token: 0x04006080 RID: 24704
    public byte b;

    // Token: 0x04006081 RID: 24705
    public byte g;

    // Token: 0x04006082 RID: 24706
    public byte r;
}

public struct A2S_SUB_CIRCLE
{
    // Token: 0x0400607B RID: 24699
    public float center_x;

    // Token: 0x0400607C RID: 24700
    public float center_y;

    // Token: 0x0400607D RID: 24701
    public float radius;

    // Token: 0x0400607E RID: 24702
    public uint pad;
}

public struct A2S_SUB_RECT
{
    // Token: 0x04006077 RID: 24695
    public float left;

    // Token: 0x04006078 RID: 24696
    public float top;

    // Token: 0x04006079 RID: 24697
    public float right;

    // Token: 0x0400607A RID: 24698
    public float bottom;
}

public class A2S_AMA_HEADER
{
    // Token: 0x0400606E RID: 24686
    public uint version;

    // Token: 0x0400606F RID: 24687
    public uint node_num;

    // Token: 0x04006070 RID: 24688
    public uint act_num;

    // Token: 0x04006071 RID: 24689
    public int node_tbl_offset;

    // Token: 0x04006072 RID: 24690
    public A2S_AMA_NODE[] node_tbl;

    // Token: 0x04006073 RID: 24691
    public int act_tbl_offset;

    // Token: 0x04006074 RID: 24692
    public A2S_AMA_ACT[] act_tbl;

    // Token: 0x04006075 RID: 24693
    public int node_name_tbl_offset;

    // Token: 0x04006076 RID: 24694
    public int act_name_tbl_offset;
}