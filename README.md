# Xpandables
Contains some useful interface definitions and implementations classes for .NetStandard, mostly following the spirit of SOLID principles

Use of [Contracts](https://github.com/Francescolis/Xpandables/tree/master/Xpandables.Standards/Contracts)

```C#
public class User
{
  public static CreateUser() => new User();
  
  private User() {}
  public string Name {get; private set; }
  public DateTime BirthDate {get; private set; }
  
  /// <summary>
  /// Sets the name of the user.
  /// </summary>
  /// <exception cref="ArgumentNullException">The <see cref="name"/> is null.</exception>
  /// <exception cref="ArgumentException">The <see cref="name"/> should be less than 
  /// <see cref="byte.MaxValue"/> characters.</exception>
  public User WithName(string name)
  {
    Name = name
            .WhenNull(nameof(name))
              .ThrowArgumentNullException()
            .WhenConditionFailed(
                v => v.Length < byte.MaxValue,
                $"{nameof(name)} should be less than {byte.MaxValue} characters")
              .ThrowArgumentException();
    
    // You can replace a value.
    Name = name
            .WhenConditionFailed(..., ...)
              .Return("New value"); // or use a delegate.
              
    return this;
  }
  
  /// <summary>
  /// Sets the birth date of the user.
  /// </summary>
  /// <exception cref="ArgumentException">The <see cref="birthDate"/> should be 
  /// lower or equal to the actual date.</exception>
  public User BirthOn(DateTime birthDate)
  {
    BirthDate = birthDate
                .WhenConditionFailed(
                    d => d.Date <= DateTime.UtcNow.Date,
                    $"{nameof(birthDate)} should be lower or equal to {DateTime.UtcNow.Date}.")
                  .ThrowArgumentException();
    return this;
  }  
}

```
