namespace FlintstonesSiloAbstractions
{
    public interface IHelloWorld : IGrainWithGuidKey
    {
        Task<string> SayHelloWorld();
    }
}
