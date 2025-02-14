using System.Collections.Generic;

namespace PAC.Interfaces
{
    /// <summary>
    /// Adds <see cref="Contains(T)"/> to <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    public interface IReadOnlyContains<T> : IReadOnlyCollection<T>
    {
        public bool Contains(T item);
    }
}
