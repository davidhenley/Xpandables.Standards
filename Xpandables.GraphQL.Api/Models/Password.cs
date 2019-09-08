using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xpandables.GraphQL.Api.Models
{
    /// <summary>
    /// The user password information.
    /// </summary>
    [Table(nameof(Password))]
    public sealed class Password : Entity
    {
        [Required]
        public string Value { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime DeactivatedOn { get; private set; }

        [NotMapped]
        public bool IsActive => DeactivatedOn == default;
    }
}