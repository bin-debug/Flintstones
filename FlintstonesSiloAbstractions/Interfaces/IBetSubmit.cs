namespace FlintstonesSiloAbstractions.Interfaces
{
    public interface IBetSubmit : IGrainWithGuidKey
    {
        public Task<BetDTO> SubmitBet(BetDTO bet);
    }
}
