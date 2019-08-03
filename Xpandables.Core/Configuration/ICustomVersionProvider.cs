using Microsoft.AspNetCore.Mvc;

namespace Xpandables.Core.Configuration
{
    /// <summary>
    /// Provides with method to access the current version of an application.
    /// </summary>
    public interface ICustomVersionProvider
    {
        /// <summary>
        /// Returns the ambient version of the underlying application.
        /// </summary>
        /// <returns>A instance of <see cref="ApiVersion"/>.</returns>
        ApiVersion GetVersion();
    }
}
