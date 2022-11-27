namespace DemoClientApi.Models
{
    public class BetRequest
    {
        public string ClientID { get; set; }
        public string Token { get; set; }
        public double StakeAmount { get; set; }
        public string Market { get; set; }
        public int Selection { get; set; }
        public double SelectionOdd { get; set; }
        public int Duration { get; set; }
    }
}
