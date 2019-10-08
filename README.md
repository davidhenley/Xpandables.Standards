# Xpandables
Contains some useful interface definitions and implementations classes for .NetStandard, mostly following the spirit of SOLID principles

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

# Xpandables.GraphQL

A starting point to generate GraphQL schema without *ObjectGraphType* implementation.
Use of [GraphQL.Net](https://github.com/graphql-dotnet/graphql-dotnet).

A working example can be found
[here](https://github.com/Francescolis/Xpandables/tree/master/Xpandables.GraphQL.Api).

Feel free to fork this project, make your own changes and create a pull request.
