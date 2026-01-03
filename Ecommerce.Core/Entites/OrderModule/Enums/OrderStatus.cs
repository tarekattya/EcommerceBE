using System.Runtime.Serialization;

namespace Ecommerce.Core
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending, 

        [EnumMember(Value = "Payment Succeeded")]
        PaymentSucceeded, 

        [EnumMember(Value = "Payment Failed")]
        PaymentFailed, 

        [EnumMember(Value = "Processing")]
        Processing, 

        [EnumMember(Value = "Shipped")]
        Shipped,

        [EnumMember(Value = "Delivered")]
        Delivered,  

        [EnumMember(Value = "Cancelled")]
        Cancelled 
    }
}
