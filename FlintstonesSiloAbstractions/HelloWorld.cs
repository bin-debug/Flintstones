using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
