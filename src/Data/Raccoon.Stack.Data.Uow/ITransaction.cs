using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Raccoon.Stack.Data.Uow;

public interface ITransaction
{
    [NotMapped]
    [JsonIgnore]
    IUnitOfWork? UnitOfWork { get; set; }
}
