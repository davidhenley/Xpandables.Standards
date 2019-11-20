# Xpandables

### ValueObject

```C#
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string ZipCode { get; }

    public Address(string street, string city, string zipCode)
        => (Street, City, ZipCode) = (street, city, zipCode);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return ZipCode;
    }
}
```

### RangeValue{T}
Defines a pair of values, representing a segment. This class uses a **RangeValueConverter** as type converter.

### EncryptedValue
Defines a representation of an encrypted value with its key. This class uses an **EncryptedValueConverter"** as type converter.

### NotifyPropertyChanged{T}
Implementation of **INotifyPropertyChanged** that can be combined with th use of **NotifyPropertyChangedDependOnAttribute** to propagate notification.

```C#
public class User : NotifyPropertyChanged<User>
{
    public User() {}
    public User(string firstName, string lastName) => (FirstName, LastName) = (firstName, lastName);
    
    private string _firstName;
    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }   
    
    private string _lastName;
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    
    [NotifyPropertyChangedDependOn(nameof(FirstName))]
    [NotifyPropertyChangedDependOn(nameof(LastName))]
    public string FullName => $"{LastName} {FirstName}";
    
    // FullName will get notified everytime FirstName or LastName change.
}
```

### IInstanceCreator
Provides with methods to create instance of specific type at runtime with cache, using lambda constructor.

### IStringGeneratorEncryptor
Provides with a string generator and encryptor using default interface implementation.

```C#
public interface IStringGeneratorEncryptor : IFluent
{
    bool Equals(EncryptedValue encrypted, string value)
    {
    
    }

    Optional<EncryptedValue> Encrypt(string value, int keySize = 12)
    {
    
    }
}
```

## Command/Query pattern
### Query
```C#
public interface IQuery<out TResult> { }

public class SignIn : QueryExpression<User>, IQuery<string>, IPersistenceBehavior
{
    public SignIn(string phone, string password) => (Phone, Password) = (phone, password);

    protected override Expression<Func<User, bool>> BuildExpression()
        => PredicateBuilder.New<User>(u => u.Phone == Phone);

    [Required, Phone, DataType(DataType.PhoneNumber), StringLength(15, MinimumLength = 3)]
    public string Phone { get; set; }

    [Required, DataType(DataType.Password), StringLength(byte.MaxValue, MinimumLength = 3)]
    public string Password { get; set; }
}
```
**QueryExpression{T}** provides with an **Expression{Func{T, bool}}** to be used as predicate and contains implicit operators.
**IPersistenceBehavior** is a marker interface specifying that the query handler will use database persistence.

Use of [Contracts](https://github.com/Francescolis/Xpandables/tree/master/Xpandables.Standards/Contracts)

Use of [Optional{T}](https://github.com/Francescolis/Xpandables/tree/master/Xpandables.Standards/Optionals)

There is a specific implementation of F# Options you can find in **Optional<T>** with asynchronous behavior.

Use of specific implementation of [F# Option](https://docs.microsoft.com/fr-fr/dotnet/fsharp/language-reference/options) to avoid null checks.

Without option :

```C#
public User FindUser(string userName, string password)
{
        var foundUser = userRepo.FindUserByName(userName);
        if(foundUser != null)
         {
             var isValidPWD = foundUser.IsPasswordValid(password);
             if(isValidPWD)
                  return foundUser;
             else ....
         }
         ...
}
```
With option :

```C#
public Optional<User> TryFindUser(string userName, string password)
{
  return repo
    .TryFindUserByName(userName)
    .WhenException(...action)
    .When(user => user.IsPasswordValid(password), _ => throw an exception for example...)
    .WhenEmpty(()=> throw an exception / return another value....);
}

// The user...
public bool IsPasswordValid(string password) // return boolean

// The repo...
public Optional<User> TryFindUserByName(string name)
{
  ....search code
  // If found
  return user; // implicit operator
  // else
  return Optional<User>.Empty();
  
  // You can also use Optional<User>.Exception(exception) in case of exception.
}

```

Use of [EnumerationType](https://github.com/Francescolis/Xpandables/tree/master/Xpandables.Standards/Enumerations),
a helper class to implement custom enumeration.

```C#
public abstract class Enumeration :
  IEqualityComparer<EnumerationType>, IEquatable<EnumerationType>, IComparable<EnumerationType>
{
    protected EnumerationType(string displayName, int value)
    {
        Value = value;
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
    }
    ....
    
    with methods :
    
    public static IEnumerable<TEnumeration> GetAll<TEnumeration>()...
    
    public static TEnumeration FromDisplayName<TEnumeration>(string displayName)
        where TEnumeration : Enumeration ...        
}

// You can use the EnumerationTypeConverter to convert "EnumerationType" objects to
and from string representations.

```


Feel free to fork this project, make your own changes and create a pull request.
