using System;
using System.Runtime.CompilerServices;

namespace mpp
{
	// Token: 0x020003E0 RID: 992
	internal class MppBitConverter
	{
		// Token: 0x0600286D RID: 10349 RVA: 0x00152D34 File Offset: 0x00150F34
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetBytes(float value, byte[] dst, int offset)
		{
            var val = BitConverter.GetBytes(value);
            Buffer.BlockCopy(val, 0, dst, offset, 4);
		}

        // Token: 0x0600286E RID: 10350 RVA: 0x00152D4C File Offset: 0x00150F4C
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(uint value, byte[] dst, int offset)
        {
            var val = BitConverter.GetBytes(value);
            Buffer.BlockCopy(val, 0, dst, offset, 4);
        }

        // Token: 0x0600286F RID: 10351 RVA: 0x00152D64 File Offset: 0x00150F64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(int value, byte[] dst, int offset)
        {
            var val = BitConverter.GetBytes(value);
            Buffer.BlockCopy(val, 0, dst, offset, 4);
        }

        // Token: 0x06002870 RID: 10352 RVA: 0x00152D7C File Offset: 0x00150F7C
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float Int32ToSingle(int value)
		{
            return *(float*)&value;
		}

        // Token: 0x06002871 RID: 10353 RVA: 0x00152D9F File Offset: 0x00150F9F
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SingleToInt32(float value)
        {
            return *(int*)&value;
        }

        // Token: 0x06002872 RID: 10354 RVA: 0x00152DC2 File Offset: 0x00150FC2
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float UInt32ToSingle(uint value)
        {
            return *(float*)&value;
        }

        // Token: 0x06002873 RID: 10355 RVA: 0x00152DE5 File Offset: 0x00150FE5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint SingleToUInt32(float value)
        {
            return *(uint*)&value;
        }

        // Token: 0x06002874 RID: 10356 RVA: 0x00152E08 File Offset: 0x00151008
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(byte[] src, int offset)
		{
            return BitConverter.ToUInt32(src, offset);
		}

        // Token: 0x06002875 RID: 10357 RVA: 0x00152E1F File Offset: 0x0015101F
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32(byte[] src, int offset)
        {
            return BitConverter.ToInt32(src, offset);
        }

        // Token: 0x06002876 RID: 10358 RVA: 0x00152E36 File Offset: 0x00151036
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToSingle(byte[] src, int offset)
        {
            return BitConverter.ToSingle(src, offset);
        }
	}
}
