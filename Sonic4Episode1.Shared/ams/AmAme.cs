using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.IO;
using System.Runtime.CompilerServices;
using mpp;

public class AmAme
{
    // Token: 0x0600114D RID: 4429 RVA: 0x0009755C File Offset: 0x0009575C
    public static AMS_AME_HEADER readAMEfile(string filename)
    {
        filename = string.Format("Content/{0}", filename);
        BinaryReader binaryReader = new BinaryReader(TitleContainer.OpenStream(filename));
        AMS_AME_HEADER result = readAMEfile(binaryReader);
        binaryReader.Dispose();
        return result;
    }

    // Token: 0x0600114E RID: 4430 RVA: 0x00097590 File Offset: 0x00095790
    public static AMS_AME_HEADER readAMEfile(AmbChunk data)
    {
        AMS_AME_HEADER result = null;
        using (MemoryStream memoryStream = new MemoryStream(data.array, data.offset, data.array.Length - data.offset))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream))
            {
                result = readAMEfile(binaryReader);
            }
        }

        return result;
    }

    // Token: 0x0600114F RID: 4431 RVA: 0x00097604 File Offset: 0x00095804
    private static AMS_AME_TEX_ANIM readTexAnim(BinaryReader br)
    {
        AMS_AME_TEX_ANIM ams_AME_TEX_ANIM = new AMS_AME_TEX_ANIM();
        ams_AME_TEX_ANIM.time = br.ReadSingle();
        ams_AME_TEX_ANIM.key_num = br.ReadInt32();
        ams_AME_TEX_ANIM.key_buf = new AMS_AME_TEX_ANIM_KEY[ams_AME_TEX_ANIM.key_num];
        for (int i = 0; i < ams_AME_TEX_ANIM.key_num; i++)
        {
            ams_AME_TEX_ANIM.key_buf[i].time = br.ReadSingle();
            ams_AME_TEX_ANIM.key_buf[i].l = br.ReadSingle();
            ams_AME_TEX_ANIM.key_buf[i].t = br.ReadSingle();
            ams_AME_TEX_ANIM.key_buf[i].r = br.ReadSingle();
            ams_AME_TEX_ANIM.key_buf[i].b = br.ReadSingle();
        }

        return ams_AME_TEX_ANIM;
    }

    // Token: 0x06001150 RID: 4432 RVA: 0x000976C5 File Offset: 0x000958C5
    private static void fillAMENodeBegin(AMS_AME_NODE node, short id, short type, uint flag, char[] name,
        int child_offset, int sibling_offset, int parent_offset)
    {
        node.id = id;
        node.type = type;
        node.flag = flag;
        Array.Copy(name, 0, node.name, 0, 12);
        node.child_offset = child_offset;
        node.sibling_offset = sibling_offset;
        node.parent_offset = parent_offset;
    }

    // Token: 0x06001151 RID: 4433 RVA: 0x00097705 File Offset: 0x00095905
    private static void readVector4(BinaryReader br, NNS_VECTOR4D v)
    {
        v.x = br.ReadSingle();
        v.y = br.ReadSingle();
        v.z = br.ReadSingle();
        v.w = br.ReadSingle();
    }

    // Token: 0x06001152 RID: 4434 RVA: 0x00097737 File Offset: 0x00095937
    private static void readQuaternion(BinaryReader br, ref NNS_QUATERNION q)
    {
        q.x = br.ReadSingle();
        q.y = br.ReadSingle();
        q.z = br.ReadSingle();
        q.w = br.ReadSingle();
    }

    // Token: 0x06001153 RID: 4435 RVA: 0x0009776C File Offset: 0x0009596C
    public static AMS_AME_HEADER readAMEfile(BinaryReader br)
    {
        AMS_AME_HEADER ame_header = new AMS_AME_HEADER();
        ame_header.file_id = br.ReadBytes(4);
        ame_header.file_version = br.ReadInt32();
        ame_header.node_num = br.ReadInt32();
        ame_header.node_ofst = br.ReadUInt32();
        ame_header.bounding.center.x = br.ReadSingle();
        ame_header.bounding.center.y = br.ReadSingle();
        ame_header.bounding.center.z = br.ReadSingle();
        ame_header.bounding.center.w = br.ReadSingle();
        ame_header.bounding.radius = br.ReadSingle();
        ame_header.bounding.radius2 = br.ReadSingle();
        br.BaseStream.Seek(8L, SeekOrigin.Current);
        br.BaseStream.Seek(16L, SeekOrigin.Current);
        ame_header.node = new AMS_AME_NODE[ame_header.node_num];
        br.BaseStream.Seek((long) ((ulong) ame_header.node_ofst), 0);
        Dictionary<uint, AMS_AME_NODE> dictionary =
            new Dictionary<uint, AMS_AME_NODE>(ame_header.node_num);
        for (int i = 0; i < ame_header.node_num; i++)
        {
            uint num = (uint) br.BaseStream.Position;
            num += 15U;
            num &= 4294967280U;
            if ((ulong) num >= (ulong) br.BaseStream.Length)
            {
                break;
            }

            br.BaseStream.Seek((long) ((ulong) num), 0);
            short id = br.ReadInt16();
            AME_AME_NODE_TYPE nodeType = (AME_AME_NODE_TYPE) br.ReadInt16();
            uint flag = br.ReadUInt32();
            char[] name = br.ReadChars(12);
            int child_offset = br.ReadInt32();
            int sibling_offset = br.ReadInt32();
            int parent_offset = br.ReadInt32();
            switch (nodeType)
            {
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_OMNI:
                {
                    AMS_AME_NODE_OMNI ams_AME_NODE_OMNI = new AMS_AME_NODE_OMNI();
                    fillAMENodeBegin(ams_AME_NODE_OMNI, id, (short) nodeType, flag, name, child_offset,
                        sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_OMNI.translate);
                    readQuaternion(br, ref ams_AME_NODE_OMNI.rotate);
                    ams_AME_NODE_OMNI.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_OMNI.life = br.ReadSingle();
                    ams_AME_NODE_OMNI.start_time = br.ReadSingle();
                    ams_AME_NODE_OMNI.offset = br.ReadSingle();
                    ams_AME_NODE_OMNI.offset_chaos = br.ReadSingle();
                    ams_AME_NODE_OMNI.speed = br.ReadSingle();
                    ams_AME_NODE_OMNI.speed_chaos = br.ReadSingle();
                    ams_AME_NODE_OMNI.max_count = br.ReadSingle();
                    ams_AME_NODE_OMNI.frequency = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_OMNI;
                    ame_header.node[i] = ams_AME_NODE_OMNI;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_DIRECTIONAL:
                {
                    AMS_AME_NODE_DIRECTIONAL ams_AME_NODE_DIRECTIONAL = new AMS_AME_NODE_DIRECTIONAL();
                    fillAMENodeBegin(ams_AME_NODE_DIRECTIONAL, id, (short) nodeType, flag, name,
                        child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_DIRECTIONAL.translate);
                    readQuaternion(br, ref ams_AME_NODE_DIRECTIONAL.rotate);
                    ams_AME_NODE_DIRECTIONAL.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.life = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.start_time = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.offset = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.offset_chaos = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.speed = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.speed_chaos = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.max_count = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.frequency = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.spread = br.ReadSingle();
                    ams_AME_NODE_DIRECTIONAL.spread_variation = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_DIRECTIONAL;
                    ame_header.node[i] = ams_AME_NODE_DIRECTIONAL;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_SURFACE:
                {
                    AMS_AME_NODE_SURFACE ams_AME_NODE_SURFACE = new AMS_AME_NODE_SURFACE();
                    fillAMENodeBegin(ams_AME_NODE_SURFACE, id, (short) nodeType, flag, name,
                        child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_SURFACE.translate);
                    readQuaternion(br, ref ams_AME_NODE_SURFACE.rotate);
                    ams_AME_NODE_SURFACE.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_SURFACE.life = br.ReadSingle();
                    ams_AME_NODE_SURFACE.start_time = br.ReadSingle();
                    ams_AME_NODE_SURFACE.offset = br.ReadSingle();
                    ams_AME_NODE_SURFACE.offset_chaos = br.ReadSingle();
                    ams_AME_NODE_SURFACE.speed = br.ReadSingle();
                    ams_AME_NODE_SURFACE.speed_chaos = br.ReadSingle();
                    ams_AME_NODE_SURFACE.max_count = br.ReadSingle();
                    ams_AME_NODE_SURFACE.frequency = br.ReadSingle();
                    ams_AME_NODE_SURFACE.width = br.ReadSingle();
                    ams_AME_NODE_SURFACE.width_variation = br.ReadSingle();
                    ams_AME_NODE_SURFACE.height = br.ReadSingle();
                    ams_AME_NODE_SURFACE.height_variation = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_SURFACE;
                    ame_header.node[i] = ams_AME_NODE_SURFACE;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_CIRCLE:
                {
                    AMS_AME_NODE_CIRCLE ams_AME_NODE_CIRCLE = new AMS_AME_NODE_CIRCLE();
                    fillAMENodeBegin(ams_AME_NODE_CIRCLE, id, (short) nodeType, flag, name,
                        child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_CIRCLE.translate);
                    readQuaternion(br, ref ams_AME_NODE_CIRCLE.rotate);
                    ams_AME_NODE_CIRCLE.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.life = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.start_time = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.offset = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.offset_chaos = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.speed = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.speed_chaos = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.max_count = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.frequency = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.spread = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.spread_variation = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.radius = br.ReadSingle();
                    ams_AME_NODE_CIRCLE.radius_variation = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_CIRCLE;
                    ame_header.node[i] = ams_AME_NODE_CIRCLE;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_SIMPLE_SPRITE:
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_SPRITE:
                {
                    AMS_AME_NODE_SPRITE ams_AME_NODE_SPRITE = new AMS_AME_NODE_SPRITE();
                    fillAMENodeBegin(ams_AME_NODE_SPRITE, id, (short) nodeType, flag, name,
                        child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_SPRITE.translate);
                    readQuaternion(br, ref ams_AME_NODE_SPRITE.rotate);
                    ams_AME_NODE_SPRITE.z_bias = br.ReadSingle();
                    ams_AME_NODE_SPRITE.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_SPRITE.life = br.ReadSingle();
                    ams_AME_NODE_SPRITE.start_time = br.ReadSingle();
                    ams_AME_NODE_SPRITE.size = br.ReadSingle();
                    ams_AME_NODE_SPRITE.size_chaos = br.ReadSingle();
                    ams_AME_NODE_SPRITE.scale_x_start = br.ReadSingle();
                    ams_AME_NODE_SPRITE.scale_x_end = br.ReadSingle();
                    ams_AME_NODE_SPRITE.scale_y_start = br.ReadSingle();
                    ams_AME_NODE_SPRITE.scale_y_end = br.ReadSingle();
                    ams_AME_NODE_SPRITE.twist_angle = br.ReadSingle();
                    ams_AME_NODE_SPRITE.twist_angle_chaos = br.ReadSingle();
                    ams_AME_NODE_SPRITE.twist_angle_speed = br.ReadSingle();
                    ams_AME_NODE_SPRITE.color_start.color = br.ReadUInt32();
                    ams_AME_NODE_SPRITE.color_end.color = br.ReadUInt32();
                    ams_AME_NODE_SPRITE.blend = br.ReadInt32();
                    ams_AME_NODE_SPRITE.texture_slot = br.ReadInt16();
                    ams_AME_NODE_SPRITE.texture_id = br.ReadInt16();
                    ams_AME_NODE_SPRITE.cropping_l = br.ReadSingle();
                    ams_AME_NODE_SPRITE.cropping_t = br.ReadSingle();
                    ams_AME_NODE_SPRITE.cropping_r = br.ReadSingle();
                    ams_AME_NODE_SPRITE.cropping_b = br.ReadSingle();
                    ams_AME_NODE_SPRITE.scroll_u = br.ReadSingle();
                    ams_AME_NODE_SPRITE.scroll_v = br.ReadSingle();
                    ams_AME_NODE_SPRITE.tex_anim = readTexAnim(br);
                    dictionary[num] = ams_AME_NODE_SPRITE;
                    ame_header.node[i] = ams_AME_NODE_SPRITE;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_LINE:
                {
                    AMS_AME_NODE_LINE ams_AME_NODE_LINE = new AMS_AME_NODE_LINE();
                    fillAMENodeBegin(ams_AME_NODE_LINE, id, (short) nodeType, flag, name,
                        child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_LINE.translate);
                    readQuaternion(br, ref ams_AME_NODE_LINE.rotate);
                    ams_AME_NODE_LINE.z_bias = br.ReadSingle();
                    ams_AME_NODE_LINE.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_LINE.life = br.ReadSingle();
                    ams_AME_NODE_LINE.start_time = br.ReadSingle();
                    ams_AME_NODE_LINE.length_start = br.ReadSingle();
                    ams_AME_NODE_LINE.length_end = br.ReadSingle();
                    ams_AME_NODE_LINE.inside_width_start = br.ReadSingle();
                    ams_AME_NODE_LINE.inside_width_end = br.ReadSingle();
                    ams_AME_NODE_LINE.outside_width_start = br.ReadSingle();
                    ams_AME_NODE_LINE.outside_width_end = br.ReadSingle();
                    ams_AME_NODE_LINE.inside_color_start.color = br.ReadUInt32();
                    ams_AME_NODE_LINE.inside_color_end.color = br.ReadUInt32();
                    ams_AME_NODE_LINE.outside_color_start.color = br.ReadUInt32();
                    ams_AME_NODE_LINE.outside_color_end.color = br.ReadUInt32();
                    ams_AME_NODE_LINE.blend = br.ReadInt32();
                    ams_AME_NODE_LINE.texture_slot = br.ReadInt16();
                    ams_AME_NODE_LINE.texture_id = br.ReadInt16();
                    ams_AME_NODE_LINE.cropping_l = br.ReadSingle();
                    ams_AME_NODE_LINE.cropping_t = br.ReadSingle();
                    ams_AME_NODE_LINE.cropping_r = br.ReadSingle();
                    ams_AME_NODE_LINE.cropping_b = br.ReadSingle();
                    ams_AME_NODE_LINE.scroll_u = br.ReadSingle();
                    ams_AME_NODE_LINE.scroll_v = br.ReadSingle();
                    ams_AME_NODE_LINE.tex_anim = readTexAnim(br);
                    dictionary[num] = ams_AME_NODE_LINE;
                    ame_header.node[i] = ams_AME_NODE_LINE;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_PLANE:
                {
                    AMS_AME_NODE_PLANE ams_AME_NODE_PLANE = new AMS_AME_NODE_PLANE();
                    fillAMENodeBegin(ams_AME_NODE_PLANE, id, (short) nodeType, flag, name,
                        child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_PLANE.translate);
                    readQuaternion(br, ref ams_AME_NODE_PLANE.rotate);
                    readVector4(br, ams_AME_NODE_PLANE.rotate_axis);
                    ams_AME_NODE_PLANE.z_bias = br.ReadSingle();
                    ams_AME_NODE_PLANE.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_PLANE.life = br.ReadSingle();
                    ams_AME_NODE_PLANE.start_time = br.ReadSingle();
                    ams_AME_NODE_PLANE.size = br.ReadSingle();
                    ams_AME_NODE_PLANE.size_chaos = br.ReadSingle();
                    ams_AME_NODE_PLANE.scale_x_start = br.ReadSingle();
                    ams_AME_NODE_PLANE.scale_x_end = br.ReadSingle();
                    ams_AME_NODE_PLANE.scale_y_start = br.ReadSingle();
                    ams_AME_NODE_PLANE.scale_y_end = br.ReadSingle();
                    ams_AME_NODE_PLANE.color_start.color = br.ReadUInt32();
                    ams_AME_NODE_PLANE.color_end.color = br.ReadUInt32();
                    ams_AME_NODE_PLANE.blend = br.ReadInt32();
                    ams_AME_NODE_PLANE.texture_slot = br.ReadInt16();
                    ams_AME_NODE_PLANE.texture_id = br.ReadInt16();
                    ams_AME_NODE_PLANE.cropping_l = br.ReadSingle();
                    ams_AME_NODE_PLANE.cropping_t = br.ReadSingle();
                    ams_AME_NODE_PLANE.cropping_r = br.ReadSingle();
                    ams_AME_NODE_PLANE.cropping_b = br.ReadSingle();
                    ams_AME_NODE_PLANE.scroll_u = br.ReadSingle();
                    ams_AME_NODE_PLANE.scroll_v = br.ReadSingle();
                    ams_AME_NODE_PLANE.tex_anim = readTexAnim(br);
                    dictionary[num] = ams_AME_NODE_PLANE;
                    ame_header.node[i] = ams_AME_NODE_PLANE;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_MODEL:
                {
                    AMS_AME_NODE_MODEL ams_AME_NODE_MODEL = new AMS_AME_NODE_MODEL();
                    fillAMENodeBegin(ams_AME_NODE_MODEL, id, (short) nodeType, flag, name,
                        child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_MODEL.translate);
                    readQuaternion(br, ref ams_AME_NODE_MODEL.rotate);
                    readVector4(br, ams_AME_NODE_MODEL.rotate_axis);
                    readVector4(br, ams_AME_NODE_MODEL.scale_start);
                    readVector4(br, ams_AME_NODE_MODEL.scale_end);
                    ams_AME_NODE_MODEL.z_bias = br.ReadSingle();
                    ams_AME_NODE_MODEL.inheritance_rate = br.ReadSingle();
                    ams_AME_NODE_MODEL.life = br.ReadSingle();
                    ams_AME_NODE_MODEL.start_time = br.ReadSingle();
                    ams_AME_NODE_MODEL.model_name = br.ReadChars(8);
                    ams_AME_NODE_MODEL.lod = br.ReadInt32();
                    ams_AME_NODE_MODEL.color_start.color = br.ReadUInt32();
                    ams_AME_NODE_MODEL.color_end.color = br.ReadUInt32();
                    ams_AME_NODE_MODEL.blend = br.ReadInt32();
                    ams_AME_NODE_MODEL.scroll_u = br.ReadSingle();
                    ams_AME_NODE_MODEL.scroll_v = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_MODEL;
                    ame_header.node[i] = ams_AME_NODE_MODEL;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_GRAVITY:
                {
                    AMS_AME_NODE_GRAVITY ams_AME_NODE_GRAVITY =
                        new AMS_AME_NODE_GRAVITY();
                    fillAMENodeBegin(ams_AME_NODE_GRAVITY, id, (short) nodeType, flag,
                        name, child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_GRAVITY.direction);
                    ams_AME_NODE_GRAVITY.magnitude = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_GRAVITY;
                    ame_header.node[i] = ams_AME_NODE_GRAVITY;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_UNIFORM:
                {
                    AMS_AME_NODE_UNIFORM ams_AME_NODE_UNIFORM =
                        new AMS_AME_NODE_UNIFORM();
                    fillAMENodeBegin(ams_AME_NODE_UNIFORM, id, (short) nodeType, flag,
                        name, child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_UNIFORM.direction);
                    ams_AME_NODE_UNIFORM.magnitude = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_UNIFORM;
                    ame_header.node[i] = ams_AME_NODE_UNIFORM;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_RADIAL:
                {
                    AMS_AME_NODE_RADIAL ams_AME_NODE_RADIAL = new AMS_AME_NODE_RADIAL();
                    fillAMENodeBegin(ams_AME_NODE_RADIAL, id, (short) nodeType, flag,
                        name, child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_RADIAL.position);
                    ams_AME_NODE_RADIAL.magnitude = br.ReadSingle();
                    ams_AME_NODE_RADIAL.attenuation = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_RADIAL;
                    ame_header.node[i] = ams_AME_NODE_RADIAL;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_VORTEX:
                {
                    AMS_AME_NODE_VORTEX ams_AME_NODE_VORTEX = new AMS_AME_NODE_VORTEX();
                    fillAMENodeBegin(ams_AME_NODE_VORTEX, id, (short) nodeType, flag,
                        name, child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_VORTEX.position);
                    readVector4(br, ams_AME_NODE_VORTEX.axis);
                    dictionary[num] = ams_AME_NODE_VORTEX;
                    ame_header.node[i] = ams_AME_NODE_VORTEX;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_DRAG:
                {
                    AMS_AME_NODE_DRAG ams_AME_NODE_DRAG = new AMS_AME_NODE_DRAG();
                    fillAMENodeBegin(ams_AME_NODE_DRAG, id, (short) nodeType, flag,
                        name, child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_DRAG.position);
                    ams_AME_NODE_DRAG.magnitude = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_DRAG;
                    ame_header.node[i] = ams_AME_NODE_DRAG;
                    break;
                }
                case AME_AME_NODE_TYPE.AME_AME_NODE_TYPE_NOISE:
                {
                    AMS_AME_NODE_NOISE ams_AME_NODE_NOISE = new AMS_AME_NODE_NOISE();
                    fillAMENodeBegin(ams_AME_NODE_NOISE, id, (short) nodeType, flag,
                        name, child_offset, sibling_offset, parent_offset);
                    readVector4(br, ams_AME_NODE_NOISE.axis);
                    ams_AME_NODE_NOISE.magnitude = br.ReadSingle();
                    dictionary[num] = ams_AME_NODE_NOISE;
                    ame_header.node[i] = ams_AME_NODE_NOISE;
                    break;
                }
            }
        }

        foreach (KeyValuePair<uint, AMS_AME_NODE> keyValuePair in dictionary)
        {
            AMS_AME_NODE value = keyValuePair.Value;
            if (value.child_offset != 0)
            {
                value.child = dictionary[(uint) value.child_offset];
            }

            if (value.parent_offset != 0)
            {
                value.parent = dictionary[(uint) value.parent_offset];
            }

            if (value.sibling_offset != 0)
            {
                value.sibling = dictionary[(uint) value.sibling_offset];
            }
        }

        return ame_header;
    }


    // Token: 0x06001154 RID: 4436 RVA: 0x0009854C File Offset: 0x0009674C
    private static int amAMEConv(byte[] pFile)
    {
        AppMain.mppAssertNotImpl();
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Token: 0x06001A4B RID: 6731 RVA: 0x000EB31B File Offset: 0x000E951B
    public static int NODE_TYPE(AMS_AME_NODE node)
    {
        return (int) node.type;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Token: 0x06001A4C RID: 6732 RVA: 0x000EB323 File Offset: 0x000E9523
    public static int SUPER_CLASS_ID(AMS_AME_NODE node)
    {
        return (int) ((ushort) node.type & 65280);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Token: 0x06001A4D RID: 6733 RVA: 0x000EB332 File Offset: 0x000E9532
    public static int CLASS_ID(AMS_AME_NODE node)
    {
        return (int) ((ushort) node.type & 255);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Token: 0x06001A4E RID: 6734 RVA: 0x000EB341 File Offset: 0x000E9541
    public static bool IS_EMITTER(AMS_AME_NODE node)
    {
        return SUPER_CLASS_ID(node) == 256;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Token: 0x06001A4F RID: 6735 RVA: 0x000EB350 File Offset: 0x000E9550
    public static bool IS_PARTICLE(AMS_AME_NODE node)
    {
        return SUPER_CLASS_ID(node) == 512;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Token: 0x06001A50 RID: 6736 RVA: 0x000EB35F File Offset: 0x000E955F
    public static bool IS_FIELD(AMS_AME_NODE node)
    {
        return SUPER_CLASS_ID(node) == 768;
    }
}