using System.Runtime.Serialization;

namespace Ecommerce.Core;

public enum OrderStatus
{
    [EnumMember(Value = "Pending")]
    pending,
    [EnumMember(Value = "Payment Succeded")]
    PaymentSucceded,
    [EnumMember(Value = "Payment Failed")]
    PaymentFailed

}
