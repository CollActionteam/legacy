using Newtonsoft.Json;

namespace CollAction.Services.Donation
{
    public class StripeEventData<T>
    {
        [JsonProperty("object")]
        public T Object { get; set; }
    }
}