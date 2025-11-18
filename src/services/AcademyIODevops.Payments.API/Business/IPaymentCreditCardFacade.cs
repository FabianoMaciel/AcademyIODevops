namespace AcademyIODevops.Payments.API.Business;

public interface IPaymentCreditCardFacade
{
    Transaction MakePayment(Payment payment);
}