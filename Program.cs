using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace MemoryMappedFileTest
{
    class Program
    {
        const int N = 1000000;
        const int M = 40000;
        static int i = 0;
        static int lastI = 0;
        public const string FileName = "C:\\MM\\mm.dat";

        static void Main(string[] args)
        {
            using (Timer t = new Timer(callback))
            {
                t.Change(0, 1000);

                ReadPointer();
            }
        }

        private static void callback(object state)
        {
            int currenti = i;
            int delta = currenti - lastI;
            Console.WriteLine($"{100 * (i + 1) / (float)N} | {delta}/sec | {(long)delta * M * sizeof(float) / 1000 / 1000}MiB/sec");

            lastI = currenti;
        }

        public static void ReadFileStream()
        {
            Stopwatch w = Stopwatch.StartNew();


            using var mm = File.OpenRead(FileName);

            var data = new float[M];
            Span<byte> dataSpan = MemoryMarshal.Cast<float, byte>(data);

            for (i = 0; i < N; i++)
            {
                long pos = (long)i * M * sizeof(float);

                mm.Read(dataSpan);


            }

            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds);
        }

        public static void ReadViewAccessor()
        {
            Stopwatch w = Stopwatch.StartNew();

            using var mm = MemoryMappedFile.CreateFromFile(FileName);
            using var va = mm.CreateViewAccessor();

            var data = new float[M];

            for ( i = 0; i < N; i++)
            {
                long pos = (long)i * M * sizeof(float);

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

            using var mm = MemoryMappedFile.CreateFromFile(FileName);
            using var va = mm.CreateViewStream();

            var data = new float[M];
            Span<byte> dataSpan = MemoryMarshal.Cast<float, byte>(data);

            for ( i = 0; i < N; i++)
            {
                long pos = (long)i * M * sizeof(float);

                va.Read(dataSpan);
            }

            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds);
        }

        public unsafe static void ReadPointer()
        {
            Stopwatch w = Stopwatch.StartNew();

            using var mm = MemoryMappedFile.CreateFromFile(FileName);
            using var va = mm.CreateViewAccessor();


            var ptr = (float*)va.SafeMemoryMappedViewHandle.DangerousGetHandle();


            for ( i = 0; i < N; i++)
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
            using var stream = File.Create(FileName);


            stream.SetLength((long)N * M * sizeof(float));

            for (int i = 0; i < N; i++)
            {
                Span<float> data = new Span<float>(new float[M]);
                data.Fill(i);

                stream.Write(MemoryMarshal.Cast<float, byte>(data));
            }
        }
    }
}
