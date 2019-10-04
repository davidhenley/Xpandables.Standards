using System.Collections.Concurrent;
using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// Provides wit a method to map a data row to an entity.
    /// </summary>
    public sealed class DataMapperRow : IFluent
    {
        private readonly DataMapperEntityBuilder _entityBuilder;
        private readonly ConcurrentDictionary<string, DataMapperEntity> _mappedEntities;
        private ExecuteOptions _options = ExecuteOptions.Default;

        public DataMapperRow(ConcurrentDictionary<string, DataMapperEntity> mappedEntities)
        {
            _entityBuilder = new DataMapperEntityBuilder();
            _mappedEntities = mappedEntities ?? throw new ArgumentNullException(nameof(mappedEntities));
        }

        /// <summary>
        /// Sets the options to be used with the current instance.
        /// </summary>
        /// <param name="options">The execution options to act with.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="options"/> is null.</exception>
        public DataMapperRow WithOptions(ExecuteOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _entityBuilder.WithOptions(options);
            return this;
        }

        /// <summary>
        /// Maps the data row to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of expected result..</typeparam>
        /// <param name="source">The data row to act on.</param>
        /// <returns>An instance that contains the result of mapping.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public DataMapperEntity<T> MapFrom<T>(DataRow source)
            where T : class, new()
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var entity = _entityBuilder.Build<T>();

            DoDataMapperRow<T>(source, entity);

            _mappedEntities.AddOrUpdate(entity.Identity, entity, (k, v) => v);

            return entity;
        }

        private void DoDataMapperRow<T>(DataRow source, DataMapperEntity entity)
            where T : class, new()
        {
            DataMapperPrimitives(source, entity);

            _mappedEntities.TryGetValueExtended(entity.Identity)
                .Map(found => entity = found as DataMapperEntity<T>);

            DataMapperReferences<T>(source, entity);
        }

        private void DataMapperPrimitives(DataRow source, DataMapperEntity entity)
        {
            var primitives = entity.Properties.Where(property => property.IsPrimitive);
            foreach (var primitive in primitives)
                primitive.Map(source, entity.Entity);

            entity.BuildIdentity();
        }

        private void DataMapperReferences<T>(DataRow source, DataMapperEntity entity)
            where T : class, new()
        {
            var references = entity.Properties.Where(property => !property.IsPrimitive);
            foreach (var reference in references)
            {
                var nestedEntity = _entityBuilder.Build(reference.Type);
                DoDataMapperRow<T>(source, nestedEntity);
                reference.SetValue(entity.Entity, nestedEntity.Entity);
            }
        }

        private void ExceptionHandler(object sender, DataMapperEventArgs e)
        {
            Diagnostics.Debug.WriteLine(e.Message);
        }
    }
}

using System.Linq;
using System.Reflection;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with a method to build implementations of 
    /// <see cref="DataMapperProperty"/> and <see cref="DataMapperProperty{T}"/>.
    /// </summary>
    public class DataMapperPropertyBuilder : IFluent
    {
        /// <summary>
        /// Initializes.
        /// </summary>
        public DataMapperPropertyBuilder() { }

        /// <summary>
        /// Builds an implementation of <see cref="DataMapperProperty{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <param name="source">The property info source.</param>
        /// <param name="identityProperties">A collection of keys identity.</param>
        /// <returns>An implementation of <see cref="DataMapperProperty{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="source"/> is null.</exception>
        public virtual DataMapperProperty<T> Build<T>(PropertyInfo source, string[] identityProperties)
            where T : class, new()
        {
            Func<DataMapperProperty<T>> propertyProducer = () =>
              {
                  var parameters = BuildParameters(source, identityProperties);
                  return new DataMapperProperty<T>(
                          parameters.PropertyName,
                          parameters.DataName,
                          parameters.DataPrefix,
                          parameters.IsIdentity,
                          parameters.Type);
              };

            return propertyProducer();
        }

        /// <summary>
        /// Builds an implementation of <see cref="DataMapperProperty"/>.
        /// </summary>
        /// <param name="source">The property info source.</param>
        /// <param name="identityProperties">A collection of keys identity.</param>
        /// <returns>An implementation of <see cref="DataMapperProperty"/>.</returns>
        /// <exception cref="ArgumentNullException">the <paramref name="source"/> is null.</exception>
        public virtual DataMapperProperty Build(PropertyInfo source, string[] identityProperties)
        {
            Func<DataMapperProperty> propertyProducer = () =>
              {
                  var parameters = BuildParameters(source, identityProperties);
                  return new DataMapperProperty(
                          parameters.PropertyName,
                          parameters.DataName,
                          parameters.DataPrefix,
                          parameters.IsIdentity,
                          parameters.Type);
              };

            return propertyProducer();
        }

        private (string DataPrefix, string PropertyName, string DataName, bool IsIdentity, Type Type)
            BuildParameters(PropertyInfo source, string[] identityProperties)
        {
            var property = source ?? throw new ArgumentNullException(nameof(source));
            var keys = identityProperties ?? Array.Empty<string>();

            var ownerAttr = property.DeclaringType.GetAttribute<DataMapperPrefixAttribute>().AsOptional();
            var propertyAttr = property.GetAttribute<DataMapperPrefixAttribute>().AsOptional();
            var nameAttr = property.GetAttribute<DataMapperNameAttribute>().AsOptional();

            string dataPrefix = propertyAttr.Map(attr => attr.Prefix)
                .ReduceOptional(() => ownerAttr.Map(attr => attr.Prefix))
                .Return();
            var dataName = nameAttr.Map(attr => attr.Name).Reduce(() => property.Name).Return();
            var isIdentity = keys.Any(k => k == property.Name);
            var type = property.PropertyType;

            return (dataPrefix, property.Name, dataName, isIdentity, type);
        }
    }
}
using System.Collections;
using System.Threading;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with custom information for a mapping associated with a property.
    /// </summary>
    public class DataMapperProperty : IFluent
    {
        static SpinLock _spinLock = new SpinLock();

        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperProperty"/> with all arguments using cache.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="dataName">The data name from the source.</param>
        /// <param name="dataPrefix">The data prefix from the source.</param>
        /// <param name="isIdentity">Whether the property is an identity key or not.</param>
        /// <param name="type">The property type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public DataMapperProperty(
            string propertyName,
            string dataName,
            string dataPrefix,
            bool isIdentity,
            Type type)
        {
            DataPrefix = dataPrefix;
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            DataName = dataName ?? throw new ArgumentNullException(nameof(dataName));
            IsIdentity = isIdentity;
            Type = type ?? throw new ArgumentNullException(nameof(type));

            IsPrimitive = type.IsNullable()
                && (Nullable.GetUnderlyingType(type).IsValueType
                    || Nullable.GetUnderlyingType(type).Equals(typeof(string)))
                || type.IsEnumerable()
                    && (type.GetGenericArguments()[0].IsValueType
                    || type.GetGenericArguments()[0].Equals(typeof(string)))
                || type.IsValueType || Type.Equals(typeof(string));

            IsNullable = type.IsNullable();
            IsEnumerable = type.IsEnumerable();
        }

        /// <summary>
        /// Gets the target property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the name of the column in the data source.
        /// </summary>
        public string DataName { get; }

        /// <summary>
        /// Gets the prefix of the column in the data source.
        /// </summary>
        public string DataPrefix { get; }

        /// <summary>
        /// Gets the full name of the column in the data source.
        /// </summary>
        public string DataFullName => $"{DataPrefix}{DataName}";

        /// <summary>
        /// Determine whether the property is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Determine whether the property is a value type|string or reference type.
        /// </summary>
        public bool IsPrimitive { get; }

        /// <summary>
        /// Determine whether the property is used for uniquely identify the data source.
        /// </summary>
        public bool IsIdentity { get; protected set; }

        /// <summary>
        /// Determine whether the property is a collection.
        /// </summary>
        public bool IsEnumerable { get; }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Returns the property type.
        /// <para>If the entity type is nullable, returns <see langword="Nullable.GetUnderlyingType(Type)"/>.</para>
        /// If the entity is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public Type GetPropertyType()
        {
            Func<Type> typeProducer = () =>
              {
                  var targetType = Type;
                  if (IsNullable) targetType = Nullable.GetUnderlyingType(targetType);
                  if (IsEnumerable) targetType = targetType.GetGenericArguments()[0];

                  return targetType;
              };

            return typeProducer();
        }

        /// <summary>
        /// Returns the property type.
        /// If the entity is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public virtual Type GetPropertyStronglyType()
        {
            Func<Type> typeProducer = () =>
              {
                  var targetType = Type;
                  return IsNullable ? Nullable.GetUnderlyingType(targetType) : targetType;
              };

            return typeProducer();
        }

        /// <summary>
        /// Creates an instance of the <see cref="System.Type"/>. The type must contains a parameterless constructor.
        /// </summary>
        /// <returns>A new instance.</returns>
        public virtual object CreateProperty()
        {
            return Activator.CreateInstance(GetPropertyType());
        }

        /// <summary>
        /// Creates an instance of the exactly value to <see cref="System.Type"/>.
        /// </summary>
        /// <returns>An new instance of <see cref="System.Type"/>.</returns>
        public virtual object CreateStronglyTypedProperty()
        {
            return Activator.CreateInstance(GetPropertyStronglyType());
        }

        /// <summary>
        /// Maps the data row to the property.
        /// </summary>
        /// <param name="source">The data row to act on.</param>
        /// <param name="entity">the entity to save to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">the <paramref name="entity"/> is null.</exception>
        public virtual void Map(DataRow source, object entity)
        {
            if (source.Table.Columns.Contains(DataFullName))
            {
                var dataValue = source[DataFullName];
                SetValue(entity, dataValue);
            }
        }

        /// <summary>
        /// Sets the value to the target using the property environment.
        /// </summary>
        /// <param name="target">The target to act on.</param>
        /// <param name="value">The value to use.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        public virtual void SetValue(object target, object value)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));

            var dataValue = value;
            if (value is null || value is DBNull)
                dataValue = null;

            var lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                var propertyInfo = target.GetType().GetProperty(PropertyName);
                if (IsEnumerable)
                {
                    if (propertyInfo.GetValue(target, null) is null)
                        propertyInfo.SetValue(target, CreateStronglyTypedProperty());

                    ((IList)propertyInfo.GetValue(target, null)).Add(dataValue);
                }
                else
                {
                    propertyInfo.SetValue(target, dataValue);
                }
            }
            finally
            {
                if (lockTaken) _spinLock.Exit(false);
            }
        }
    }

    /// <summary>
    /// Implementation of a mapping associated with a property and a specify type class.
    /// </summary>
    /// <typeparam name="T">Type of the class.</typeparam>
    public class DataMapperProperty<T> : DataMapperProperty
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperProperty{T}"/> with all arguments.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="dataName">The data name from the source.</param>
        /// <param name="dataPrefix">The data prefix from the source.</param>
        /// <param name="isIdentity">Whether the property is an identity key or not.</param>
        /// <param name="type">The property type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataPrefix"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        public DataMapperProperty(
            string propertyName,
            string dataName,
            string dataPrefix,
            bool isIdentity,
            Type type)
            : base(propertyName, dataName, dataPrefix, isIdentity, type) { }

        /// <summary>
        /// Sets the value to the target using the property environment.
        /// </summary>
        /// <param name="target">The target to act on.</param>
        /// <param name="value">The value to use.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is null.</exception>
        public void SetValue(T target, object value) => base.SetValue(target, value);

        /// <summary>
        /// Creates an instance of the exactly value to <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An new instance of <typeparamref name="T"/>.</returns>
        public new T CreateProperty()
        {
            return (T)Activator.CreateInstance(GetPropertyType());
        }

        /// <summary>
        /// Creates an instance of the exactly value to <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An new instance of <typeparamref name="T"/>.</returns>
        public new T CreateStronglyTypedProperty()
        {
            return (T)Activator.CreateInstance(GetPropertyStronglyType());
        }
    }
}

namespace System.Data.Common
{
    /// <summary>
    /// Defines the prefix of the property/field on the target data source.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DataMapperPrefixAttribute : Attribute
    {
        /// <summary>
        /// Defines the prefix of the property/field to be used on a data source.
        /// </summary>
        /// <param name="prefix">The prefix value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="prefix"/> is null.</exception>
        public DataMapperPrefixAttribute(string prefix)
        {
            Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }

        /// <summary>
        /// Gets the value of the prefix string used to map the data source column with.
        /// </summary>
        public string Prefix { get; }
    }
}

namespace System.Data.Common
{
    /// <summary>
    /// Defines the name of the property/field on the target data source.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DataMapperNameAttribute : Attribute
    {
        /// <summary>
        /// Defines the name of the property/field to be used on a data source.
        /// </summary>
        /// <param name="name">The name value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> is null.</exception>
        public DataMapperNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the value of the name string used to map the data source column with.
        /// </summary>
        public string Name { get; }
    }
}

namespace System.Data.Common
{
    /// <summary>
    /// Denotes one or more properties that uniquely identify the decorated class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DataMapperKeysAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperKeysAttribute"/> with the collection of keys.
        /// </summary>
        /// <param name="keys">List of keys to be used to uniquely identify the decorated class.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="keys"/> is null or empty.</exception>
        public DataMapperKeysAttribute(params string[] keys)
        {
            if (keys is null)
                throw new ArgumentNullException(nameof(keys));
            if (keys.Length <= 0)
                throw new ArgumentException("The collection of keys can not be empty.");

            Keys = keys;
        }

        /// <summary>
        /// Gets the collection of keys used to uniquely identify the decorated class.
        /// </summary>
        public string[] Keys { get; }
    }
}

namespace System.Data.Common
{
    /// <summary>
    /// Contains information an event raised in the mapper.
    /// </summary>
    public sealed class DataMapperEventArgs : EventArgs
    {
        /// <summary>
        /// Build a new instance of <see cref="DataMapperEventArgs"/> with the specified message.
        /// </summary>
        /// <param name="message">The message to registered.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is null.</exception>
        public DataMapperEventArgs(string message)
            => Message = message ?? throw new ArgumentException(nameof(message));
        public string Message { get; private set; }
    }
}

using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with a method to build implementations of 
    /// <see cref="DataMapperEntity"/> and <see cref="DataMapperEntity{T}"/>.
    /// </summary>
    public class DataMapperEntityBuilder : IFluent
    {
        private readonly DataMapperPropertyBuilder _propertyBuilder;
        private ExecuteOptions _options = ExecuteOptions.Default;

        public DataMapperEntityBuilder()
        {
            _propertyBuilder = new DataMapperPropertyBuilder();
        }

        /// <summary>
        /// Sets the options to be used with the current instance.
        /// </summary>
        /// <param name="options">The execution options to act with.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="options"/> is null.</exception>
        public DataMapperEntityBuilder WithOptions(ExecuteOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            return this;
        }

        /// <summary>
        /// Builds an implementation of <see cref="DataMapperEntity{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns>An implementation of <see cref="DataMapperEntity{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">Unable to build an instance. See inner exception.</exception>
        public DataMapperEntity<T> Build<T>() where T : class, new()
        {
            Func<DataMapperEntity<T>> entityProducer = () =>
              {
                  var parameters = BuildParameters(typeof(T));
                  var properties = parameters
                      .Type
                      .GetProperties()
                      .Select(property => _propertyBuilder.Build<T>(property, parameters.IdentityProperties))
                      .Where(_options.ConditionalMapperTyped<T>());

                  var dataMapperEntity = new DataMapperEntity<T>(typeof(T), properties);
                  dataMapperEntity.SetEntity(dataMapperEntity.CreateEntity());

                  return dataMapperEntity;
              };

            return entityProducer();
        }

        /// <summary>
        /// Builds an implementation of <see cref="DataMapperEntity"/>.
        /// </summary>
        /// <param name="type">The type of the class.</param>
        /// <returns>An instance of <see cref="DataMapperEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Unable to build an instance. See inner exception.</exception>
        public DataMapperEntity Build(Type type)
        {
            Func<DataMapperEntity> entityProducer = () =>
              {
                  var parameters = BuildParameters(type);
                  var properties = parameters
                      .Type
                      .GetProperties()
                      .Select(property => _propertyBuilder.Build(property, parameters.IdentityProperties))
                      .Where(_options.ConditionalMapper);

                  var dataMapperEntity = new DataMapperEntity(type, properties);
                  dataMapperEntity.SetEntity(dataMapperEntity.CreateEntity());

                  return dataMapperEntity;
              };

            return entityProducer();
        }

        protected (string[] IdentityProperties, Type Type) BuildParameters(Type source)
        {
            var type = source ?? throw new ArgumentNullException(nameof(source));

            if (type.IsEnumerable())
                type = type.GetGenericArguments()[0];
            else if (type.IsNullable())
                type = Nullable.GetUnderlyingType(type);

            var keys = type.GetAttribute<DataMapperKeysAttribute>()
                    .AsOptional()
                    .Map(attr => attr.Keys)
                    .Reduce(() => Array.Empty<string>())
                    .Return();

            return (keys, type);
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// Implementation of a mapping associated with a class.
    /// </summary>
    public class DataMapperEntity : IFluent
    {
        static readonly string Key = "ABCDEFG0123456789";
        static readonly IStringGenerator stringGenerator = new StringGenerator();
        static readonly IStringEncryptor stringEncryptor = new StringEncryptor();

        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperEntity"/> that contains a type and a collection of properties.
        /// </summary>
        /// <param name="type">the type of the entity.</param>
        /// <param name="properties">The collection of properties.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        public DataMapperEntity(
            Type type,
            IEnumerable<DataMapperProperty> properties)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            IsNullable = type.IsNullable();
            IsEnumerable = type.IsEnumerable();
        }

        /// <summary>
        /// Gets the entity instance.
        /// </summary>
        public object Entity { get; protected set; }

        /// <summary>
        /// Gets the type full name of the instance.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the collection of properties from the type.
        /// </summary>
        public IEnumerable<DataMapperProperty> Properties { get; }

        /// <summary>
        /// Gets the identity that is unique for the entity.
        /// </summary>
        public string Identity { get; protected set; } = string.Empty;
        /// <summary>
        /// Determine whether or not the underlying object is already signed.
        /// </summary>
        public bool IsIdentified => !string.IsNullOrWhiteSpace(Identity);

        /// <summary>
        /// Determine whether the property is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Determine whether the property is a collection.
        /// </summary>
        public bool IsEnumerable { get; }

        /// <summary>
        /// Builds the identity using the current entity instance.
        /// The properties must be already assigned.
        /// </summary>
        public void BuildIdentity()
        {
            if (IsIdentified)
                return;

            string value = stringGenerator.Generate(32, Key);
            if (Properties.Count(property => property.IsIdentity) <= 0)
            {
                value = stringGenerator.Generate(32, Key);
            }
            else
            {
                if (Entity is null)
                    return;

                value = Properties
                    .Where(property => property.IsIdentity)
                    .Select(property =>
                        Entity
                            .GetType()
                            .GetProperty(property.PropertyName)
                            ?.GetValue(Entity, null)
                            ?.ToString())
                    .StringJoin(';');
            }

            DoBuildIdentity(() => new[] { value });
        }

        /// <summary>
        /// Builds the identity using the keys provided.
        /// </summary>
        /// <param name="keysProvider">Identity keys provider.</param>
        /// <exception cref="ArgumentNullException">the <paramref name="keysProvider"/> is null.</exception>
        public void BuildIdentity(Func<IEnumerable<DataMapperProperty>, IEnumerable<string>> keysProvider)
        {
            if (Properties.Count(property => property.IsIdentity) <= 0)
            {
                BuildIdentity();
                return;
            }

            DoBuildIdentity(() => keysProvider(Properties));
        }

        protected void DoBuildIdentity(Func<IEnumerable<string>> keysProvider)
        {
            var value = keysProvider().StringJoin(';').Trim();
            if (string.IsNullOrWhiteSpace(value))
                value = stringGenerator.Generate(32, Key);

            Identity = stringEncryptor.Encrypt(value, Key).Return();
        }

        /// <summary>
        /// Returns the entity type.
        /// <para>If the entity type is nullable, returns <see langword="Nullable.GetUnderlyingType(Type)"/>.</para>
        /// If the entity is a collection, returns <see langword="Type.GetGenericArguments()[0]"/>.
        /// </summary>
        public virtual Type GetEntityType()
        {
            Func<Type> typeProducer = () =>
              {
                  var targetType = Type;
                  if (IsNullable) targetType = Nullable.GetUnderlyingType(targetType);
                  if (IsEnumerable) targetType = targetType.GetGenericArguments()[0];

                  return targetType;
              };

            return typeProducer();
        }

        /// <summary>
        /// Sets an instance value to the entity.
        /// </summary>
        /// <param name="instance">The instance to be set.</param>
        public void SetEntity(object instance) => Entity = instance;

        /// <summary>
        /// Creates an instance of the exactly value to <see cref="Type"/>.
        /// </summary>
        public virtual object CreateEntity()
        {
            Func<object> instanceProducer = () => Activator.CreateInstance(GetEntityType());

            return instanceProducer();
        }
    }

    /// <summary>
    /// Implementation of a mapping associated with a specific type class.
    /// </summary>
    /// <typeparam name="T">Type of the class.</typeparam>
    public class DataMapperEntity<T> : DataMapperEntity
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataMapperEntity{T}"/> that contains a type and a collection of properties.
        /// </summary>
        /// <param name="type">the type of the entity.</param>
        /// <param name="properties">The collection of properties.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> is null.</exception>
        public DataMapperEntity(
            Type type,
            IEnumerable<DataMapperProperty<T>> properties)
            : base(type, properties)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        public new T Entity => (T)base.Entity;
        public new IEnumerable<DataMapperProperty<T>> Properties { get; }
        public void BuildIdentity(Func<IEnumerable<DataMapperProperty<T>>, IEnumerable<string>> keysProvider)
        {
            if (Properties.Count(property => property.IsIdentity) <= 0)
            {
                BuildIdentity();
                return;
            }

            DoBuildIdentity(() => keysProvider(Properties));
        }
        public virtual new T CreateEntity() => (T)base.CreateEntity();
    }
}

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.Common
{
    /// <summary>
    /// Implementation of mapping a <see cref="DataTable"/> to an entities.
    /// </summary>
    public sealed class DataMapper : IDataMapper
    {
        private readonly DataMapperRow _dataMapperRow;
        private readonly ConcurrentDictionary<string, DataMapperEntity> _mappedEntites;

        public DataMapper()
        {
            _mappedEntites = new ConcurrentDictionary<string, DataMapperEntity>();
            _dataMapperRow = new DataMapperRow(_mappedEntites);
        }

        /// <summary>
        /// Maps the data table to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of expected result..</typeparam>
        /// <param name="source">The data table to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <returns>An enumerable that contains the result of mapping.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        public Optional<List<T>> MapFrom<T>(DataTable source, ExecuteOptions options)
            where T : class, new()
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (options is null) throw new ArgumentNullException(nameof(options));

            _dataMapperRow.WithOptions(options);

            var exceptions = new ConcurrentQueue<Exception>();
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = options.CancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount * 20
            };

            try
            {
                switch (options.ThreadOptions)
                {
                    case ThreadOptions.Normal:
                        foreach (DataRow row in source.Rows)
                        {
                            _dataMapperRow.MapFrom<T>(row);
                            parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        }
                        break;
                    case ThreadOptions.SpeedUp:
                        var rowPartitioner = Partitioner
                            .Create(source.Rows.Cast<DataRow>(), EnumerablePartitionerOptions.None);

                        Parallel.ForEach(
                            rowPartitioner,
                            parallelOptions,
                            row =>
                            {
                                _dataMapperRow.MapFrom<T>(row);
                                parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                            });
                        break;
                    case ThreadOptions.Expensive:
                        Parallel.ForEach(
                           source.Rows.Cast<DataRow>(),
                           parallelOptions,
                           row =>
                           {
                               _dataMapperRow.MapFrom<T>(row);
                               parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                           });
                        break;
                }
            }
            catch (Exception exception)
            {
                exceptions.Enqueue(exception);
            }

            if (exceptions.Count > 0)
                return Optional<List<T>>.Exception(new AggregateException(exceptions));

            return Optional<List<T>>.Some(
                _mappedEntites.Select(entity => entity.Value.Entity as T).ToList());
        }
    }
}

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// Implements the <see cref="IDataBaseSettingsAccessor"/>.
    /// </summary>
    public class DataBaseSettingsAccessor : IDataBaseSettingsAccessor
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of <see cref="DataBaseSettingsAccessor"/> with the configuration value.
        /// </summary>
        /// <param name="configuration">The configuration to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration"/> is null.</exception>
        public DataBaseSettingsAccessor(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IEnumerable<DataBaseSettings> GetDataBaseSettings()
        {
            return _configuration.GetSection(nameof(DataBaseSettings))
                .Get<DataBaseSettings[]>()
                ?? Enumerable.Empty<DataBaseSettings>();
        }
    }
}

namespace System.Data.Common
{
    /// <summary>
    /// Provides <see cref="DataBaseContext"/> settings.
    /// </summary>
    public class DataBaseSettings
    {
        /// <summary>
        /// Initializes a default instance.
        /// </summary>
        public DataBaseSettings() { }

        /// <summary>
        /// Returns the built connection string value.
        /// </summary>
        /// <exception cref="ArgumentException">The instance is invalid.</exception>
        public string GetConnectionString()
        {
            if (!IsValid())
                throw new ArgumentException($"The current {GetType().Name} contains invalid information.");

            return $"{ConnectionStringSource}User Id={UserId};Password={UserPassword};";
        }

        /// <summary>
        /// Adds the connection string source (without security information).
        /// </summary>
        /// <param name="connectionStringSource">The connection string.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="connectionStringSource"/> is null.</exception>
        public DataBaseSettings WithConnectionString(string connectionStringSource)
        {
            ConnectionStringSource = connectionStringSource ?? throw new ArgumentNullException(nameof(connectionStringSource));
            if (connectionStringSource.Contains("User Id") || connectionStringSource.Contains("Password"))
                throw new ArgumentException("Connection String Source contains security information.");
            return this;
        }

        /// <summary>
        /// Adds the pool name to be used.
        /// </summary>
        /// <param name="poolName">The pool name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="poolName"/> is null.</exception>
        public DataBaseSettings WithPoolName(string poolName)
        {
            PoolName = poolName ?? throw new ArgumentNullException(nameof(poolName));
            return this;
        }

        /// <summary>
        /// Adds the known provider name.
        /// </summary>
        /// <param name="providerName">The provider name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="providerName"/> is null.</exception>
        public DataBaseSettings WithProviderName(string providerName)
        {
            ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
            return this;
        }

        /// <summary>
        /// Adds the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="userId"/> is null.</exception>
        public DataBaseSettings WithUserId(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            return this;
        }

        /// <summary>
        /// Adds the user password.
        /// </summary>
        /// <param name="userPassword">The user password.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="userPassword"/> is null.</exception>
        public DataBaseSettings WithUserPassword(string userPassword)
        {
            UserPassword = userPassword ?? throw new ArgumentNullException(nameof(userPassword));
            return this;
        }

        /// <summary>
        /// Determines whether or not the instance contains valid information.
        /// If so, returns <see langword="true"/> otherwise returns <see langword="false"/>.
        /// </summary>
        public bool IsValid()
            => !string.IsNullOrWhiteSpace(ConnectionStringSource)
                && !string.IsNullOrWhiteSpace(PoolName)
                && !string.IsNullOrWhiteSpace(ProviderName)
                && !string.IsNullOrWhiteSpace(UserId)
                && !string.IsNullOrWhiteSpace(UserPassword);

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionStringSource { get; set; }

        /// <summary>
        /// Gets or sets the pool name.
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the connection string user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the connection string user password.
        /// </summary>
        public string UserPassword { get; set; }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;

namespace System.Design.DependencyInjection
{
    /// <summary>
    /// Service collection registration methods for <see cref="DataBaseContext"/>
    /// </summary>
    public static class DataBaseServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default <see cref="IDataBaseContextAccessor"/> and <see cref="IDataBaseSettingsAccessor"/> to the services.
        /// </summary>
        /// <param name="services">the collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddDataBaseContext(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IDataBaseContextAccessor, DataBaseContextAccessor>();
            services.AddTransient<IDataBaseSettingsAccessor, DataBaseSettingsAccessor>();
            return services;
        }

        /// <summary>
        /// Adds custom implementations for <see cref="IDataBaseContextAccessor"/> and <see cref="IDataBaseSettingsAccessor"/> to the services.
        /// </summary>
        /// <typeparam name="TDataBaseContextAccessor">The reference type that implements <see cref="IDataBaseContextAccessor"/>.</typeparam>
        /// <typeparam name="TDataBaseSettingsAccessor">The reference type that implements <see cref="IDataBaseSettingsAccessor"/>.</typeparam>
        /// <param name="services">The collection of services.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        public static IServiceCollection AddDataBaseContext<TDataBaseContextAccessor, TDataBaseSettingsAccessor>(this IServiceCollection services)
            where TDataBaseContextAccessor : class, IDataBaseContextAccessor
            where TDataBaseSettingsAccessor : class, IDataBaseSettingsAccessor
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<IDataBaseContextAccessor, TDataBaseContextAccessor>();
            services.AddTransient<IDataBaseSettingsAccessor, TDataBaseSettingsAccessor>();
            return services;
        }

        /// <summary>
        /// Adds the logger <see cref="ILogger"/> to database using the <see cref="DataBaseLoggingSettings"/> specified settings found in configuration.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration"/> is null.</exception>
        /// <exception cref="ArgumentException">The configuration not found.</exception>
        public static IServiceCollection AddDataBaseContextLogger(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var settings = configuration.GetSection(nameof(DataBaseLoggingSettings)).Get<DataBaseLoggingSettings>()
                ?? throw new ArgumentException($"{nameof(DataBaseLoggingSettings)} configuration section not found.");

            if (!settings.IsValid()) throw new ArgumentException("Data base logging settings is not valid.");

            var columnOptions = new ColumnOptions { AdditionalDataColumns = new Collection<DataColumn>() };

            foreach (var column in settings.Remove)
                columnOptions.Store.Remove(column);

            foreach (var column in settings.Add)
                columnOptions.AdditionalDataColumns.Add(column);

            services.AddTransient<ILogger>(provider =>
                new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.MSSqlServer(settings.GetConnectionString(), settings.LogTableName, columnOptions: columnOptions, autoCreateSqlTable: true)
                    .CreateLogger());

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services;
        }
    }
}

using Serilog.Sinks.MSSqlServer;
using System.Collections.Generic;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with database logging settings.
    /// </summary>
    public class DataBaseLoggingSettings : DataBaseSettings
    {
        /// <summary>
        /// Initializes a default instance.
        /// </summary>
        public DataBaseLoggingSettings() { }

        /// <summary>
        /// Adds the log table name.
        /// </summary>
        /// <param name="logTableName">The log table name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="logTableName"/> is null.</exception>
        public DataBaseLoggingSettings WithLogTableName(string logTableName)
        {
            LogTableName = logTableName ?? throw new ArgumentNullException(nameof(logTableName));
            return this;
        }

        /// <summary>
        /// Adds the collection of columns to the log table.
        /// </summary>
        /// <param name="added">The collection of columns to be added.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="added"/> is null.</exception>
        public DataBaseLoggingSettings WithAddedColumns(List<DataColumn> added)
        {
            Add = added ?? throw new ArgumentNullException(nameof(added));
            return this;
        }

        /// <summary>
        /// Removes the collection of columns to the log table.
        /// </summary>
        /// <param name="removed">The collection of columns to be added.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="removed"/> is null.</exception>
        public DataBaseLoggingSettings WithRemovedColumns(List<StandardColumn> removed)
        {
            Remove = removed ?? throw new ArgumentNullException(nameof(removed));
            return this;
        }

        /// <summary>
        /// Determines whether or not the instance contains valid information.
        /// If so, returns <see langword="true"/> otherwise returns <see langword="false"/>.
        /// </summary>
        public new bool IsValid()
            => !string.IsNullOrWhiteSpace(ConnectionStringSource)
                && !string.IsNullOrWhiteSpace(ProviderName)
                && !string.IsNullOrWhiteSpace(LogTableName)
                && !string.IsNullOrWhiteSpace(ProviderName)
                && !string.IsNullOrWhiteSpace(UserId)
                && !string.IsNullOrWhiteSpace(UserPassword);

        /// <summary>
        /// Gets or sets the logs table name.
        /// </summary>
        public string LogTableName { get; set; }

        /// <summary>
        /// Gets or sets a collection of columns to be added to the log table.
        /// </summary>
        public List<DataColumn> Add { get; set; } = new List<DataColumn>();

        /// <summary>
        /// Gets or sets a collection of standard columns to be removed from the log table.
        /// </summary>
        public List<StandardColumn> Remove { get; set; } = new List<StandardColumn>();
    }
}

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// <see cref="IDataBaseContextAccessor"/> default implementation.
    /// </summary>
    public class DataBaseContextAccessor : IDataBaseContextAccessor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataBaseContextAccessor"/> with collection of settings.
        /// </summary>
        /// <param name="dataBaseSettingsAccessor">The settings accessor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataBaseSettingsAccessor"/> is null.</exception>
        public DataBaseContextAccessor(IDataBaseSettingsAccessor dataBaseSettingsAccessor)
        {
            if (dataBaseSettingsAccessor is null) throw new ArgumentNullException(nameof(dataBaseSettingsAccessor));
            Settings = dataBaseSettingsAccessor.GetDataBaseSettings();
        }

        public IEnumerable<DataBaseSettings> Settings { get; }

        public IDataBaseContext GetDataBaseContext(string poolName)
        {
            var settings = Settings
                .FirstOrDefault(s => s.PoolName.Equals(poolName, StringComparison.InvariantCultureIgnoreCase))
                ?? throw new ArgumentException($"{poolName} not found in the available collection of providers.");
            return GetDataBaseContext(settings);
        }

        public IDataBaseContext GetDataBaseContext(DataBaseSettings settings)
        {
            return new DataBaseContext(settings);
        }
    }
}

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with implementation of <see cref="IDataBaseContext"/>.
    /// </summary>
    public class DataBaseContext : IDataBaseContext
    {
        private readonly DataProviderFactoryProvider _dataProviderFactoryProvider
            = new DataProviderFactoryProvider();
        private readonly DataParameterBuilder _dataParameterBuilder = new DataParameterBuilder();
        private readonly IDataMapper _dataMapper = new DataMapper();
        private readonly DataBaseSettings _settings;
        private readonly Lazy<DbProviderFactory> _dbProviderFactory;
        private DbTransaction _transaction;

        /// <summary>
        /// Initializes a new instance of <see cref="DataBaseContext"/> with the specified settings.
        /// </summary>
        /// <param name="settings">The settings to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="settings"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="settings"/> is not valid.</exception>
        public DataBaseContext(DataBaseSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            var datapProviderType = EnumerationType.FromDisplayName<DataProviderType>(_settings.ProviderName)
                ?? throw new ArgumentException($"The {_settings.ProviderName} not found in the available collection of providers.");
            _dbProviderFactory = new Lazy<DbProviderFactory>(
                () => _dataProviderFactoryProvider.GetProviderFactory(datapProviderType));
        }

        /// <summary>
        /// Executes the specify query to the database using options and returns a result of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        public async Task<Optional<T>> ExecuteQueryAsync<T>(
            ExecuteOptions options,
            string sqlQuery,
            params object[] parameters)
            where T : class, new()
        {
            var searchResult = await ExecuteQueriesAsync<T>(options, sqlQuery, parameters).ConfigureAwait(false);
            return searchResult.MapOptional(result => result.FirstOrEmpty());
        }

        /// <summary>
        /// Executes the specify query to the database using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        public async Task<Optional<List<T>>> ExecuteQueriesAsync<T>(
            ExecuteOptions options,
            string sqlQuery,
            params object[] parameters)
            where T : class, new()
        {
            try
            {
                using (var connection = CreateDbConnection(_dbProviderFactory.Value, _settings.GetConnectionString()))
                using (var command = connection.CreateCommand())
                using (var adapter = _dbProviderFactory.Value.CreateDataAdapter())
                using (var dataSet = new DataSet())
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync().ConfigureAwait(false);

                    command.CommandType = CommandType.Text;
                    command.CommandText = ParseSql(sqlQuery);
                    _dataParameterBuilder.Build(command, parameters);
                    adapter.SelectCommand = command;
                    adapter.AcceptChangesDuringFill = false;
                    adapter.FillLoadOption = LoadOption.OverwriteChanges;
                    adapter.Fill(dataSet);

                    using (var dataTable = dataSet.Tables[0])
                    {
                        return _dataMapper.MapFrom<T>(dataTable, options);
                    }
                }
            }
            catch (Exception exception)
            {
                return Optional<List<T>>.Exception(exception);
            }
            finally
            {
                if (options.CloseTransaction)
                {
                    _transaction?.Dispose();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Executes the stored procedure by its name using options and returns the number of affected rows.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <param name="storedProcedureName">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProcedureName"/> is null.</exception>
        public async Task<Optional<int>> ExecuteProcedureAsync(
            ExecuteOptions options,
            string storedProcedureName,
            params object[] parameters)
        {
            try
            {
                using (var connection = CreateDbConnection(_dbProviderFactory.Value, _settings.GetConnectionString()))
                using (var command = connection.CreateCommand())
                using (var adapter = _dbProviderFactory.Value.CreateDataAdapter())
                using (var dataSet = new DataSet())
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync().ConfigureAwait(false);

                    if (options.UseTransaction)
                        if (_transaction is null)
                        {
                            _transaction = connection.BeginTransaction();
                            command.Transaction = _transaction;
                        }

                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    _dataParameterBuilder.Build(command, parameters);

                    command.CommandText = storedProcedureName.Split('@')[0].Trim();
                    command.CommandTimeout = 0;

                    var returnValParameter = command.CreateParameter();
                    returnValParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnValParameter);

                    if (command.Connection is SqlConnection)
                        command.Prepare();

                    _ = await command.ExecuteNonQueryAsync(options.CancellationToken).ConfigureAwait(false);

                    if (options.UseTransaction && options.CloseTransaction)
                        _transaction?.Commit();

                    return int.Parse(returnValParameter.Value.ToString());
                }
            }
            catch (Exception exception)
            {
                if (options.UseTransaction && options.CloseTransaction)
                {
                    _transaction?.Rollback();
                }

                return Optional<int>.Exception(exception);
            }
            finally
            {
                if (options.UseTransaction && options.CloseTransaction)
                {
                    _transaction?.Dispose();
                    _transaction = null;

                }
            }
        }

        /// <summary>
        /// Executes the stored procedure by its name using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="storedProcedureName">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProcedureName"/> is null.</exception>
        public async Task<Optional<List<T>>> ExecuteProceduresAsync<T>(
            ExecuteOptions options,
            string storedProcedureName,
            params object[] parameters)
            where T : class, new()
        {
            try
            {
                using (var connection = CreateDbConnection(_dbProviderFactory.Value, _settings.GetConnectionString()))
                using (var command = connection.CreateCommand())
                using (var adapter = _dbProviderFactory.Value.CreateDataAdapter())
                using (var dataSet = new DataSet())
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync().ConfigureAwait(false);

                    if (options.UseTransaction)
                        if (_transaction is null)
                        {
                            _transaction = connection.BeginTransaction();
                            command.Transaction = _transaction;
                        }

                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    _dataParameterBuilder.Build(command, parameters);

                    command.CommandTimeout = 0;
                    command.CommandText = storedProcedureName.Split('@')[0].Trim();

                    if (command.Connection is SqlConnection)
                        command.Prepare();

                    adapter.SelectCommand = command;
                    adapter.AcceptChangesDuringFill = false;
                    adapter.FillLoadOption = LoadOption.OverwriteChanges;
                    adapter.Fill(dataSet);

                    var result = Optional<List<T>>.Empty();
                    using (var dataTable = dataSet.Tables[0])
                    {
                        result = _dataMapper.MapFrom<T>(dataTable, options);
                    }

                    if (options.CloseTransaction)
                        _transaction?.Commit();

                    return result;
                }

            }
            catch (Exception exception)
            {
                if (options.UseTransaction && options.CloseTransaction)
                {
                    _transaction?.Rollback();
                }

                return Optional<List<T>>.Exception(exception);
            }
            finally
            {
                if (options.CloseTransaction)
                {
                    _transaction?.Dispose();
                    _transaction = null;
                }
            }
        }

        /// <summary>
        /// Builds the connection to data base.
        /// </summary>
        /// <param name="dbProviderFactory">The provider factory.</param>
        /// <param name="connectionString">The connection string.</param>
        protected DbConnection CreateDbConnection(DbProviderFactory dbProviderFactory, string connectionString)
        {
            var dbConnection = dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            SpeedSqlServerResult(dbConnection);
            return dbConnection;
        }

        /// <summary>
        /// Speeds the connection result for Sql server.
        /// </summary>
        /// <param name="connection">the connection to speed.</param>
        protected void SpeedSqlServerResult(DbConnection connection)
        {
            if (connection is SqlConnection)
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        @"
                    SET ANSI_NULLS ON
                    SET ANSI_PADDING ON
                    SET ANSI_WARNINGS ON
                    SET ARITHABORT ON
                    SET CONCAT_NULL_YIELDS_NULL ON
                    SET QUOTED_IDENTIFIER ON
                    SET NUMERIC_ROUNDABORT OFF";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Parses queries for using old format.
        /// </summary>
        /// <param name="query">The query to be formatted.</param>
        /// <returns>A parsed query.</returns>
        protected string ParseSql(string query)
        {
            string[] parts = query.Split('?');
            if (parts.Length > 0)
            {
                var output = new StringBuilder();
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    output.Append(parts[i]);
                    output.Append("@P").Append(i);
                }

                output.Append(parts[parts.Length - 1]);
                query = output.ToString();
            }

            return query;
        }
    }
}
namespace System.Data.Common
{
    /// <summary>
    /// Allows an application author to return a data provider factory from provider type.
    /// </summary>
    public interface IDataProviderFactoryProvider
    {
        /// <summary>
        /// Returns an instance of the data provider factory matching the specified type.
        /// </summary>
        /// <param name="providerType">The provider type to find factory.</param>
        /// <returns>An instance of <see cref="DbProviderFactory"/> if found, otherwise an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType"/> is null.</exception>
        Optional<DbProviderFactory> GetProviderFactory(DataProviderType providerType);
    }
}


namespace System.Data.Common
{
    /// <summary>
    /// Allows an application author to return a data provider factory from provider type.
    /// </summary>
    public interface IDataProviderFactoryProvider
    {
        /// <summary>
        /// Returns an instance of the data provider factory matching the specified type.
        /// </summary>
        /// <param name="providerType">The provider type to find factory.</param>
        /// <returns>An instance of <see cref="DbProviderFactory"/> if found, otherwise an empty optional.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="providerType"/> is null.</exception>
        Optional<DbProviderFactory> GetProviderFactory(DataProviderType providerType);
    }
}

using System.Collections.Generic;

namespace System.Data.Common
{
    /// <summary>
    /// Provides wit a method to map a data table to an entities.
    /// </summary>
    public interface IDataMapper
    {
        /// <summary>
        /// Maps the data table to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of expected result..</typeparam>
        /// <param name="source">The data table to act on.</param>
        /// <param name="options">Defines the execution options.</param>
        /// <returns>An enumerable that contains the result of mapping.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        Optional<List<T>> MapFrom<T>(DataTable source, ExecuteOptions options)
            where T : class, new();
    }
}

using System.Collections.Generic;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with a method to access a collection of <see cref="DataBaseSettings"/>.
    /// </summary>
    public interface IDataBaseSettingsAccessor
    {
        /// <summary>
        /// Returns a collection of <see cref="DataBaseSettings"/> found in the system.
        /// If not found, returns an enumerable empty list.
        /// </summary>
        IEnumerable<DataBaseSettings> GetDataBaseSettings();
    }
}

using System.Threading.Tasks;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with a method to log information to the database
    /// </summary>
    /// <typeparam name="T">The type of the event to log.</typeparam>
    public interface IDataBaseLogger<T>
        where T : class
    {
        /// <summary>
        /// Asynchronously logs the event to the database.
        /// </summary>
        /// <param name="event">The event to log.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="event"/> is null.</exception>
        Task Log(T @event);
    }
}

using System.Collections.Generic;

namespace System.Data.Common
{
    /// <summary>
    /// Provides methods to access the <see cref="IDataBaseContext"/>.
    /// </summary>
    public interface IDataBaseContextAccessor
    {
        /// <summary>
        /// Contains a collection of settings.
        /// </summary>
        IEnumerable<DataBaseSettings> Settings { get; }

        /// <summary>
        /// Returns the database context matching the pool name settings.
        /// You can use the <see cref="Settings"/> to find the available settings.
        /// </summary>
        /// <param name="poolName">The target pool name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="poolName"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="poolName"/> not found.</exception>
        IDataBaseContext GetDataBaseContext(string poolName);

        /// <summary>
        /// Returns the database context matching the settings.
        /// You can use the <see cref="Settings"/> to find the available settings.
        /// </summary>
        /// <param name="settings">The settings to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="settings"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="settings"/> is invalid.</exception>
        IDataBaseContext GetDataBaseContext(DataBaseSettings settings);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Data.Common
{
    /// <summary>
    /// Provides with methods to query database.
    /// </summary>
    public interface IDataBaseContext
    {
        /// <summary>
        /// Executes the specify query to the database using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        Task<Optional<List<T>>> ExecuteQueriesAsync<T>(
            ExecuteOptions options,
            string sqlQuery,
            params object[] parameters)
            where T : class, new();

        /// <summary>
        /// Executes the specify query to the database using options and returns a result of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="sqlQuery">The query to be executed.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sqlQuery"/> is null.</exception>
        Task<Optional<T>> ExecuteQueryAsync<T>(
            ExecuteOptions options,
            string sqlQuery,
            params object[] parameters)
            where T : class, new();

        /// <summary>
        /// Executes the stored procedure by its name using options and returns the number of affected rows.
        /// </summary>
        /// <param name="options">The database options.</param>
        /// <param name="storedProcedureName">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProcedureName"/> is null.</exception>
        Task<Optional<int>> ExecuteProcedureAsync(
            ExecuteOptions options,
            string storedProcedureName,
            params object[] parameters);

        /// <summary>
        /// Executes the stored procedure by its name using options and returns a collection of results of
        /// the specific-type.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="options">The database options.</param>
        /// <param name="storedProcedureName">The store procedure name.</param>
        /// <param name="parameters">The parameters to be applied.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="options"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="storedProcedureName"/> is null.</exception>
        Task<Optional<List<T>>> ExecuteProceduresAsync<T>(
            ExecuteOptions options,
            string storedProcedureName,
            params object[] parameters)
            where T : class, new();
    }
}

using System.Threading;

namespace System.Data.Common
{
    /// <summary>
    /// Contains execution behaviors.
    /// </summary>
    public sealed class ExecuteOptions
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ExecuteOptions"/> with default behaviors.
        /// <para>No transaction, no conditional mapping, no cancellation and use of normal for each...</para>
        /// </summary>
        public static ExecuteOptions Default => new ExecuteOptions();

        /// <summary>
        /// Enables use of transaction.
        /// </summary>
        public ExecuteOptions EnableTransaction()
        {
            UseTransaction = true;
            return this;
        }

        /// <summary>
        /// Disables use of transaction.
        /// </summary>
        public ExecuteOptions DisableTransaction()
        {
            UseTransaction = false;
            return this;
        }

        /// <summary>
        /// Enables use of auto commit/rollback transaction.
        /// Transaction should be enabled.
        /// </summary>
        public ExecuteOptions EnableCloseTransaction()
        {
            CloseTransaction = true;
            return this;
        }

        /// <summary>
        /// Enables use of auto commit/rollback transaction.
        /// Use with caution.
        /// </summary>
        public ExecuteOptions DisableCloseTransaction()
        {
            CloseTransaction = false;
            return this;
        }

        /// <summary>
        /// Defines a delegate that determines whether or not a property should be mapped.
        /// The definition here takes priority over the mapper attributes.
        /// </summary>
        /// <param name="conditional">The delegate to act with.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="conditional"/> is null.</exception>
        public ExecuteOptions WithConditionalMapper(Func<DataMapperProperty, bool> conditional)
        {
            ConditionalMapper = conditional ?? throw new ArgumentNullException(nameof(conditional));
            return this;
        }

        public ExecuteOptions WithEntityIdentityBuilder(Func<DataMapperEntity, string> identityBuilder)
        {
            return this;
        }

        /// <summary>
        /// Adds a delegate to be used for converting data row value to the target type.
        /// You can add many converters. if the delegate throws an exception when converting, the default conversion will be used.
        /// </summary>
        /// <typeparam name="TType">The type of the property the value should be converted to.</typeparam>
        /// <param name="converter">The delegate to be used to convert a data row value to <typeparamref name="TType"/> type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="converter"/> is null.</exception>
        public ExecuteOptions AddCustomConverter<TType>(Func<TType, object> converter)
        {
            return this;
        }

        /// <summary>
        /// Defines the execution thread options.
        /// </summary>
        /// <param name="threadOptions">the thread options to be used.</param>
        public ExecuteOptions WithThreadOptions(ThreadOptions threadOptions)
        {
            ThreadOptions = threadOptions;
            return this;
        }

        /// <summary>
        /// Defines the cancellation token for the execution.
        /// The default used value is <see cref="CancellationToken.None"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to be used.</param>
        public ExecuteOptions WithCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Determines whether or not to use transaction.
        /// The default value is <see langword="false"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        public bool UseTransaction { get; private set; }

        /// <summary>
        /// Determines whether or not to automatically close transaction : apply commit/rollback.
        /// The default value is <see langword="true"/>.
        /// if so, contains <see langword="true"/>, otherwise contains <see langword="false"/>.
        /// </summary>
        public bool CloseTransaction { get; private set; } = true;

        /// <summary>
        /// Defines a delegate that determines whether or not a property should be mapped.
        /// Its default behavior return <see langword="true"/>.
        /// </summary>
        public Func<DataMapperProperty, bool> ConditionalMapper { get; private set; } = _ => true;

        /// <summary>
        /// Returns the generic type of conditional mapper.
        /// </summary>
        /// <typeparam name="T">The type of property.</typeparam>
        public Func<DataMapperProperty<T>, bool> ConditionalMapperTyped<T>()
            where T : class, new() => ConditionalMapper;

        /// <summary>
        /// Defines the thread execution options.
        /// </summary>
        public ThreadOptions ThreadOptions { get; private set; } = ThreadOptions.Normal;

        /// <summary>
        /// Contains the cancellation token to be used.
        /// The default value is <see cref="CancellationToken.None"/>.
        /// </summary>
        public CancellationToken CancellationToken { get; private set; } = CancellationToken.None;

        private ExecuteOptions() { }
    }

    /// <summary>
    /// Determines the algorithm to be applied.
    /// </summary>
    public enum ThreadOptions
    {
        /// <summary>
        /// A normal for each...
        /// </summary>
        Normal,

        /// <summary>
        /// Use of partitioner with parallel execution.
        /// </summary>
        SpeedUp,

        /// <summary>
        /// Use of parallel.
        /// </summary>
        Expensive
    }
}
namespace System.Data.Common
{
    /// <summary>
    /// Provides with a list of data providers.
    /// You can derive from this class in order to extend type.
    /// </summary>
    public class DataProviderType : EnumerationType
    {
        /// <summary>
        /// Construct a new data provider type with an index and the invariant name.
        /// </summary>
        /// <param name="index">The index for the date provider.</param>
        /// <param name="assemblyName">The invariant assembly name to be used.</param>
        /// <param name="providerFactoryTypeName">The provider factory type name.</param>
        /// <exception cref="ArgumentException">The <paramref name="assemblyName"/> is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="providerFactoryTypeName"/> is null.</exception>
        protected DataProviderType(int index, string assemblyName, string providerFactoryTypeName)
            : base(assemblyName, index)
        {
            DisplayName = assemblyName;
            ProviderFactoryTypeName = providerFactoryTypeName ?? throw new ArgumentNullException(nameof(providerFactoryTypeName));
        }

        /// <summary>
        /// Gets the invariant (assembly) name that can be used programmatically to refer to the data provider.
        /// </summary>
        public new string DisplayName { get; }

        /// <summary>
        /// Gets the provider factory type name.
        /// </summary>
        public string ProviderFactoryTypeName { get; }

        /// <summary>
        /// Determines whether or not the provider type refers to an instance factory.
        /// </summary>
        public bool IsInstance => DisplayName.Contains("Instance", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Provides data access for Microsoft SQL Server.
        /// </summary>
        public static DataProviderType MSSQL => new DataProviderType(
            0, "System.Data.SqlClient", "System.Data.SqlClient.SqlClientFactory");

        /// <summary>
        /// For Oracle data sources version 8.1.7 and later.
        /// </summary>
        public static DataProviderType ORACLE => new DataProviderType(
            1, "System.Data.OracleClient", "Oracle.DataAccess.Client.OracleClientFactory");

#if NET48
        /// <summary>
        /// For data sources exposed by using OLE DB.
        /// </summary>
        public static DataProviderType OLEDB => new DataProviderType(2, "System.Data.OleDb", "System.Data.OleDb");
#endif

        /// <summary>
        /// For data sources exposed by using ODBC.
        /// </summary>
        public static DataProviderType ODBC => new DataProviderType(3, "System.Data.Odbc", "System.Data.Odbc");

        /// <summary>
        /// Provides data access for Entity Data Model (EDM) applications.
        /// </summary>
        public static DataProviderType ENTITY => new DataProviderType(4, "System.Data.EntityClient", "System.Data.Entity");

#if NET48
        /// <summary>
        /// Provides data access for Microsoft SQL Lite.
        /// </summary>
        public static DataProviderType SQLITE => new DataProviderType(
            5, "System.Data.SQLite", "System.Data.SQLite.SQLiteFactory");
#else
        /// <summary>
        ///  Provides data access for Microsoft SQL Lite.
        /// </summary>
        public static DataProviderType SQLITE => new DataProviderType(
            5, "Microsoft.Data.Sqlite", "Microsoft.Data.Sqlite.SqliteFactory");
#endif
        /// <summary>
        /// Provides data access for MySQL.
        /// </summary>
        public static DataProviderType MYSQL => new DataProviderType(
            6, "MySql.Data", "MySql.Data.MySqlClient.MySqlClientFactory");

        /// <summary>
        /// Provides data access for PostgreSQL.
        /// </summary>
        public static DataProviderType NPGSQL => new DataProviderType(7, "Npgsql", "Npgsql.NpgsqlFactory");
    }
}


using System.Reflection;

namespace System.Data.Common
{
    /// <summary>
    /// The default implementation to return data provider factory from provider type.
    /// </summary>
    public sealed class DataProviderFactoryProvider : IDataProviderFactoryProvider
    {
        public Optional<DbProviderFactory> GetProviderFactory(DataProviderType providerType)
        {
            if (providerType is null) throw new ArgumentNullException(nameof(providerType));

            return GetproviderFactoryInstance()
                .Map(result => result as DbProviderFactory);

            Optional<object> GetproviderFactoryInstance()
                => Type.GetType(providerType.ProviderFactoryTypeName, false, true)
                        .AsOptional()
                        .Map(type => TypeInvokeMember(type, "Instance"))
                        .Reduce(() => AssemblyLoadFromString(providerType.DisplayName) as Assembly)
                        .Map(obj => obj as Assembly)
                        .MapOptional(ass => ass.GetExportedTypes()
                            .FirstOrEmpty(t => t.FullName == providerType.ProviderFactoryTypeName))
                        .Map(type => TypeInvokeMember(type, "Instance"));

            Assembly AssemblyLoadFromString(string assemblyName)
            {
                try { return Assembly.Load(assemblyName); }
                catch { return default; }
            }

            object TypeInvokeMember(Type type, string member)
            {
                try
                {
                    return type.InvokeMember(
                                member,
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty,
                                null, type, null);
                }
                catch { return default; }
            }
        }
    }
}

using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// Implementation of a command parameter builder.
    /// </summary>
    public sealed class DataParameterBuilder : IFluent
    {
        /// <summary>
        /// Adds the specified parameters to the current command instance.
        /// </summary>
        /// <param name="command">The command to act on.</param>
        /// <param name="parameters">The parameters to be added.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="parameters"/> can not be a mix of DbParameters and values.</exception>
        public void Build(DbCommand command, params object[] parameters)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (parameters is null || parameters.Length == 0)
                return;

            var dbParameters = new DbParameter[parameters.Length];
            if (parameters.All(p => p is DbParameter))
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    dbParameters[i] = (DbParameter)parameters[i];
                }
            }
            else
            if (!parameters.Any(p => p is DbParameter))
            {
                var commandSplit = command.CommandText.Split('@').ToList();
                var parameterSourceNames = Array.Empty<string>();
                if (commandSplit.Count > 1)
                {
                    commandSplit.RemoveAt(0);
                    parameterSourceNames = commandSplit.Select(param => param.Split(' ')[0].Replace(",", "").Trim()).ToArray();
                    if (parameterSourceNames.Length != parameters.Length)
                        throw new ArgumentException("Arguments provided must match in number expected.");
                }

                var parameterNames = new string[parameters.Length];
                var parameterSql = new string[parameters.Length];

                // We check for the friendly representation of the parameter.
                // We only manage SQL Server and Oracle.
                var friendlyRepresentation = command.Connection is SqlConnection ? "@" : ":";

                for (var i = 0; i < parameters.Length; i++)
                {
                    parameterNames[i] = string.Format(CultureInfo.InvariantCulture, parameterSourceNames?[i] ?? "p{0}", i);
                    dbParameters[i] = command.CreateParameter();
                    dbParameters[i].ParameterName = parameterNames[i];
                    dbParameters[i].Value = parameters[i] ?? DBNull.Value;

                    parameterSql[i] = friendlyRepresentation + parameterNames[i];
                }

                command.CommandText = string.Format(CultureInfo.InvariantCulture, command.CommandText, parameterSql);
            }
            else
            {
                throw new ArgumentException("Arguments provided can not be a mix of DbParameters and values.");
            }

            command.Parameters.AddRange(dbParameters);
        }
    }
}
