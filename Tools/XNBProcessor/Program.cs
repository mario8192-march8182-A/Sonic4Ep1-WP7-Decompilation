using System;
using System.IO;
using System.Text;

namespace XNBProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var newExt = args[1];
            var extBytes = Encoding.ASCII.GetBytes(newExt);
            foreach (var file in Directory.GetFiles(args[0], "*.xnb"))
            {
                using (var stream = File.OpenWrite(file)) {
                    stream.Seek(-8, SeekOrigin.End);
                    stream.Write(extBytes, 0, 3);
                    stream.Flush();
                }
            }
        }
    }
}
