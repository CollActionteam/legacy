using Newtonsoft.Json;

namespace CollAction.Services.Donation
{
    public class StripeEvent<T>
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("data")]
        public StripeEventData<T> Data { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
