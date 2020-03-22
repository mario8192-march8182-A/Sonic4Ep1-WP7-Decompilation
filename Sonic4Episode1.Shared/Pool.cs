using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ArrayPool<T> where T : new()
{
    private readonly System.Buffers.ArrayPool<T> _pool;

    public ArrayPool()
    {
        _pool = System.Buffers.ArrayPool<T>.Shared;
    }

    // Token: 0x060026A1 RID: 9889 RVA: 0x0014FE9C File Offset: 0x0014E09C
    public T[] AllocArray(int size)
    {
        return _pool.Rent(size);
    }

    // Token: 0x060026A2 RID: 9890 RVA: 0x0014FF31 File Offset: 0x0014E131
    public void ReleaseArray(T[] array)
    {
        _pool.Return(array);
    }

    // Token: 0x060026A3 RID: 9891 RVA: 0x0014FF4C File Offset: 0x0014E14C
    public void ReleaseUsedArrays()
    {
        //AppMain.mppAssertNotImpl();
    }

    // Token: 0x060026A4 RID: 9892 RVA: 0x0014FF6A File Offset: 0x0014E16A
    public void Clear()
    {
        AppMain.mppAssertNotImpl();
    }
}

public class ArrayPoolFast<T> where T : new()
{
    private System.Buffers.ArrayPool<T> _pool;
    public ArrayPoolFast()
    {
        _pool = System.Buffers.ArrayPool<T>.Shared;
    }

    // Token: 0x060026A6 RID: 9894 RVA: 0x0014FFA0 File Offset: 0x0014E1A0
    public T[] AllocArray(int size)
    {
        return _pool.Rent(size);
    }

    // Token: 0x060026A7 RID: 9895 RVA: 0x0015000D File Offset: 0x0014E20D
    public void ReleaseArray(T[] array)
    {
        _pool.Return(array);
    }

    // Token: 0x060026A8 RID: 9896 RVA: 0x00150028 File Offset: 0x0014E228
    public void ReleaseUsedArrays()
    {
        //AppMain.mppAssertNotImpl();
    }

    // Token: 0x060026A9 RID: 9897 RVA: 0x00150046 File Offset: 0x0014E246
    public void Clear()
    {
        AppMain.mppAssertNotImpl();
    }

    // Token: 0x060026AA RID: 9898 RVA: 0x0015005E File Offset: 0x0014E25E
    public int GetFreeCount()
    {
        return 100;
    }
}

public class Pool<T> where T : class, new()
{
    // Token: 0x0600269C RID: 9884 RVA: 0x0014FC24 File Offset: 0x0014DE24
    public T Alloc()
    {
        var t = _freeObjects.TryDequeue(out var result) ? result : Activator.CreateInstance<T>();
        _usedObjects.Add(t);
        
        return t;
    }

    // Token: 0x0600269D RID: 9885 RVA: 0x0014FCD8 File Offset: 0x0014DED8
    public void Release(T obj)
    { 
        _freeObjects.Enqueue(obj);
        _usedObjects.Remove(obj);
    }

    // Token: 0x0600269E RID: 9886 RVA: 0x0014FD8C File Offset: 0x0014DF8C
    public void ReleaseUsedObjects()
    {
        foreach (var obj in _usedObjects)
        {
            _freeObjects.Enqueue(obj);
        }
        
        _usedObjects.Clear();
    }

    // Token: 0x0600269F RID: 9887 RVA: 0x0014FE0C File Offset: 0x0014E00C
    public void Clear()
    {
        _freeObjects.Clear();
        _usedObjects.Clear();
    }
    
    // Token: 0x04005FDD RID: 24541
    private readonly Queue<T> _freeObjects = new Queue<T>(1024);

    // Token: 0x04005FDE RID: 24542
    private readonly List<T> _usedObjects = new List<T>(1024);
}

public class SimplePool<T> where T : class, new()
{
    // Token: 0x06002699 RID: 9881 RVA: 0x0014FBB5 File Offset: 0x0014DDB5
    public SimplePool()
    {
        this.cache_ = new Queue<T>();
    }

    // Token: 0x0600269A RID: 9882 RVA: 0x0014FBC8 File Offset: 0x0014DDC8
    public T Alloc()
    {
        return cache_.TryDequeue(out var result) ? result : Activator.CreateInstance<T>();
    }

    // Token: 0x0600269B RID: 9883 RVA: 0x0014FC13 File Offset: 0x0014DE13
    public void Release(T obj)
    {
        this.cache_.Enqueue(obj);
    }

    // Token: 0x04005FDA RID: 24538
    private Queue<T> cache_;
}

public class GlobalPool<T> where T : class, IClearable, new()
{
    // Token: 0x06002696 RID: 9878 RVA: 0x0014FB4C File Offset: 0x0014DD4C
    public static T Alloc()
    {
        T result;
        if (cache_.TryDequeue(out result))
        {
            result.Clear();
        }
        else
        {
            result = Activator.CreateInstance<T>();
        }
        return result;
    }

    // Token: 0x06002697 RID: 9879 RVA: 0x0014FBA0 File Offset: 0x0014DDA0
    public static void Release(T obj)
    {
        cache_.Enqueue(obj);
    }

    // Token: 0x04005FD9 RID: 24537
    private static Queue<T> cache_ = new Queue<T>();
}