using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xpandables.Database
{
    public interface IDataMapperEntity
    {
        object Entity { get; }
        IEnumerable<IDataMapperProperty> Properties { get; }
        string Signature { get; }
        bool IsSigned { get; }
        bool IsEqualTo(string signature);
        void BuildSignature();
        void BuildSignature(Func<IEnumerable<IDataMapperProperty>, IEnumerable<string>> keysProvider);
        void CreateEntity(Type type);
        void SetEntity(object entity);
    }

    public interface IDataMapperEntity<T> : IDataMapperEntity
        where T : class, new()
    {
        new T Entity { get; }
        new IEnumerable<IDataMapperProperty<T>> Properties { get; }
        void CreateEntity();
        void BuildSignature(Func<IEnumerable<IDataMapperProperty<T>>, IEnumerable<string>> keysProvider);
        [EditorBrowsable(EditorBrowsableState.Never)]
        new void CreateEntity(Type type);
    }

}
