namespace FlintstonesSiloAbstractions
{
    public class HelloWorld : Grain, IHelloWorld
    {
        public Task<string> SayHelloWorld()
        {
            return Task.FromResult("Hello World From Grains");
        }
    }
}
