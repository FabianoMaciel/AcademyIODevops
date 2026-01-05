using AcademyIODevops.Payments.API.Business;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Payments.API.AntiCorruption;

[ExcludeFromCodeCoverage]
public class PayPalGateway : IPayPalGateway
{
    public string GetPayPalServiceKey(string apiKey, string encriptionKey)
    {
        return new string([.. Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10).Select(s => s[new Random().Next(s.Length)])]);
    }

    public string GetCardHashKey(string serviceKey, string cartaoCredito)
    {
        return new string([.. Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10).Select(s => s[new Random().Next(s.Length)])]);
    }

    public Transaction CommitTransaction(string cardHashKey, string orderId, double amount)
    {
        var sucesso = true;
        return new Transaction
        {
            RegistrationId = Guid.Parse(orderId),
            Total = amount,
            StatusTransaction = sucesso ? StatusTransaction.Accept : StatusTransaction.Declined
        };
    }
}