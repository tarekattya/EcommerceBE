using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ecommerce.Core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
