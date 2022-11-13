using System;
using System.Collections.Generic;
using NuclearGames.StructuresUnity.Utils.Collections.Interfaces;

namespace NuclearGames.StructuresUnity.Utils.Collections {
    /// <summary>
    /// Список на массиве, поддердживающий интерфейс <seealso cref="IAnyElementCollection{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnyElementList<T> : List<T>, IAnyElementCollection<T> {
        /// <summary>
        /// Является ли дерево пустым
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Любой элемент из коллекции
        /// </summary>
        public T Any {
            get {
                if (IsEmpty) {
                    throw new NullReferenceException("Tree is empty!");
                }

                return this[0];
            }
        }
        
        /// <summary>
        /// Есть ли в коллекции хотя бы один элемент
        /// </summary>
        /// <param name="value">Любой элемент, если он существует в коллекции</param>
        public bool TryGetAny(out T value) {
            if (IsEmpty) {
                value = default;
                return false;
            }

            value = this[0];
            return true;
        }
    }
}