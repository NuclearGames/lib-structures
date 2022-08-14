using System.Collections.Generic;


namespace Structures.NetStandard.Utils.Collections.Interfaces {
    public interface IAnyElementCollection<T> : ICollection<T> {
        /// <summary>
        /// Есть ли в колеекции элементы
        /// </summary>
        bool IsEmpty { get; }
        
        /// <summary>
        /// Любой элемент из коллекции
        /// </summary>
        T Any { get; }

        /// <summary>
        /// Есть ли в коллекции хотя бы один элемент
        /// </summary>
        /// <param name="value">Любой элемент, если он существует в коллекции</param>
        bool TryGetAny(out T value);
    }
}