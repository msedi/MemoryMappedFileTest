using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemoryMappedFileTest
{
    class Program
    {
        const int N = 250000;
        const int M = 40000;

        static void Main(string[] args)
        {
            ReadPointer();
        }

        public static void ReadViewAccessor()
        {
            Stopwatch w = Stopwatch.StartNew();

            using var mm = MemoryMappedFile.CreateFromFile("D:\\mm.dat");
            using var va = mm.CreateViewAccessor();

            var data = new float[M];

            for (int i = 0; i < N; i++)
            {
                long pos = (long)i *  M * sizeof(float);

                   va.ReadArray(pos, data, 0, M);

                if (data[0] != i)
                    Console.WriteLine("!EGG");
            }

            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds);
        }

        public static void ReadViewStream()
        {
            Stopwatch w = Stopwatch.StartNew();

            using var mm = MemoryMappedFile.CreateFromFile("D:\\mm.dat");
            using var va = mm.CreateViewStream();

            var data = new float[M];
            Span<byte> dataSpan = MemoryMarshal.Cast<float, byte>(data);

            for (int i = 0; i < N; i++)
            {
                long pos = (long)i * M * sizeof(float);

                va.Read(dataSpan);

                if (data[0] != i)
                    Console.WriteLine("!EGG");
            }

            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds);
        }

        public unsafe static void ReadPointer()
        {
            Stopwatch w = Stopwatch.StartNew();

            using var mm = MemoryMappedFile.CreateFromFile("D:\\mm.dat");
            using var va = mm.CreateViewAccessor();


            var ptr = (float*)va.SafeMemoryMappedViewHandle.DangerousGetHandle();


            for (int i = 0; i < N; i++)
            {
                long pos = (long)i * M;

                var offset = ptr + pos;

                Span<float> data = new Span<float>(ptr + pos, M);

                if (data[0] != i)
                    Console.WriteLine("!EGG");
            }

            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds);
        }

        public static void Create()
        {
            using var stream = File.Create("D:\\mm.dat");


            stream.SetLength((long)N * M * sizeof(float));

            for (int i=0; i<N; i++)
            {
                Span<float> data = new Span<float>(new float[M]);
                data.Fill(i);

                stream.Write(MemoryMarshal.Cast<float, byte>(data));
            }
        }
    }
}
