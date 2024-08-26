using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Raccoon.Stack.Data.Uow;

namespace Raccoon.Stack.Uow.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
public class Transaction : ITransaction
{
    [NotMapped]
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    public Transaction(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;
}
