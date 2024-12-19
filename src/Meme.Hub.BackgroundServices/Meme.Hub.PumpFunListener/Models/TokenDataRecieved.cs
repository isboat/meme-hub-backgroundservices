namespace Meme.Hub.PumpFunListener.Models
{
    public class TokenDataRecieved
    {
        public string? Signature { get; set; }
        public string? Mint { get; set; }
        public string? TraderPublicKey { get; set; }
        public string? TxType { get; set; }
        public double InitialBuy { get; set; }
        public string? BondingCurveKey { get; set; }
        public double VTokensInBondingCurve { get; set; }
        public double VSolInBondingCurve { get; set; }
        public double MarketCapSol { get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public string? Uri { get; set; }
    }

}
