using AcademyIODevops.Core.DomainObjects;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Payments.API.Business;

[ExcludeFromCodeCoverage]
public class Transaction : Entity
{
    public Guid RegistrationId { get; set; }
    public Guid PaymentId { get; set; }
    public double Total { get; set; }
    public StatusTransaction StatusTransaction { get; set; }
    public Payment Payment { get; set; }
}