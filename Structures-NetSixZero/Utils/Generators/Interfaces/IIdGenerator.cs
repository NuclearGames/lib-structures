namespace Structures.NetSixZero.Utils.Generators.Interfaces {
    public interface IIdGenerator<T> where T : IComparable<T> {
        /// <summary>
        /// Генерирует новый Id
        /// </summary>
        T NewId();
        
        /// <summary>
        /// Возвращает в буффер Id
        /// </summary>
        void Release(T id);
    }
}