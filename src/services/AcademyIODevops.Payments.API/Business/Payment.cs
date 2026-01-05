using AcademyIODevops.Core.DomainObjects;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Payments.API.Business;

[ExcludeFromCodeCoverage]
public class Payment : Entity, IAggregateRoot
{
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public double Value { get; set; }
    public string CardName { get; set; }
    public string CardNumber { get; set; }
    public string CardExpirationDate { get; set; }
    public string CardCVV { get; set; }
    public Transaction Transaction { get; set; }
}