using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesModels
{
    public class FeedModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; set; }
        [JsonProperty(PropertyName = "clientid")]
        public string Date { get; set; }
        public decimal LastPrice { get; set; }
        public string TimeStamp { get; set; }
    }
}
