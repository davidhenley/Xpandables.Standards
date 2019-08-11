using System;
using System.Collections.Generic;
using System.Linq;

namespace Xpandables.Database
{
    public class DataMapperEntity : IDataMapperEntity
    {
        protected const string Key = "ABCDEFG0123456789";
        public DataMapperEntity(IEnumerable<IDataMapperProperty> properties)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        public object Entity { get; protected set; }
        public string Signature { get; protected set; }
        public bool IsSigned => !string.IsNullOrWhiteSpace(Signature);
        public IEnumerable<IDataMapperProperty> Properties { get; }

        public bool IsEqualTo(string signature)
        {
            return Signature == signature;
        }

        public void BuildSignature()
        {
            var value = string.Empty;
            if (Properties.Count(property => property.IsIdentity) <= 0)
            {
                IStringGenerator stringGenerator = new StringGenerator();
                value = stringGenerator.Generate(64, Key);
            }
            else
            {
                value = Properties
                    .Where(w => w.IsIdentity)
                    .Select(property
                        => Entity
                            .GetType()
                            .GetProperty(property.PropertyName)
                            ?.GetValue(Entity, null)
                            ?.ToString())
                    .StringJoin(';');
            }

            IStringEncryptor encryptor = new StringEncryptor();
            Signature = encryptor.Encrypt(value, Key);
        }

        public void BuildSignature(Func<IEnumerable<IDataMapperProperty>, IEnumerable<string>> keysProvider)
        {
            if (Properties.Count(property => property.IsIdentity) <= 0)
            {
                BuildSignature();
                return;
            }

            var value = keysProvider(Properties).StringJoin(';');
            IStringEncryptor encryptor = new StringEncryptor();
            Signature = encryptor.Encrypt(value, Key);
        }

        public void CreateEntity(Type type) => Entity = Activator.CreateInstance(type);
        public void SetEntity(object entity) => Entity = entity;
    }

    public sealed class DataMapperEntity<T> : DataMapperEntity, IDataMapperEntity<T>
        where T : class, new()
    {
        public DataMapperEntity(IEnumerable<IDataMapperProperty<T>> properties)
            : base(properties)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        public new T Entity { get => base.Entity as T; private set => base.Entity = value; }
        public new IEnumerable<IDataMapperProperty<T>> Properties { get; }

        public void CreateEntity()
        {
            base.CreateEntity(typeof(T));
        }
        public new void CreateEntity(Type type)
        {
            base.CreateEntity(type);
        }

        public void BuildSignature(Func<IEnumerable<IDataMapperProperty<T>>, IEnumerable<string>> keysProvider)
        {
            if (Properties.Count(property => property.IsIdentity) <= 0)
                return;

            var value = keysProvider(Properties).StringJoin(';');
            IStringEncryptor encryptor = new StringEncryptor();
            Signature = encryptor.Encrypt(value, Key);
        }
    }
}
