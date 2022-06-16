using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesSiloAbstractions
{
    public interface IHelloWorld : IGrainWithGuidKey
    {
        Task<string> SayHelloWorld();
    }
}
