using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoryMappedFileTest
{
    public interface IMemoryMappedHandle
    {

    }

    /// <summary>
    /// Container to store a pointer into a memory mapped file view.
    /// </summary>
    public unsafe readonly struct MemoryMappedHandle : IMemoryMappedHandle
    {
        /// <summary>
        /// Gets the base pointer.
        /// </summary>
        private readonly nint _pointer;

        /// <summary>
        /// The length of the memory in the stored data format.
        /// </summary>
        private readonly int _length;

        /// <summary>
        /// Creates a new instance of <see cref="MemoryMappedHandle"/>.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="length"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public MemoryMappedHandle(nint pointer, int length)
        {
            _pointer = pointer;
            _length = length;
        }

        /// <summary>
        /// Creates a <see cref="ReadOnlySpan{T}"/> out of the <see cref="MemoryMappedHandle"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ReadOnlySpan<T> AsReadOnlySpan<T>() where T : unmanaged => new((void*)_pointer, _length * sizeof(T));

        /// <summary>
        /// Creates a <see cref="Span{T}"/> out of the <see cref="MemoryMappedHandle"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Span<T> AsSpan<T>() where T : unmanaged => new((void*)_pointer, _length * sizeof(T));

    }
}
