// using System;
// using System.Numerics;

// //string hash = "2694c20d3fbca07f385284d5510ba3bf9bc9756969b6df98dd65560433d6a2dd";
// //string hash = "6898b0e39e6b02d87bd614a1b7a32fd8280edc986eff0e5f0eda2a33087b809d";
// //string hash = "018abaa1f5a0de5490977a5ce52a46df0e7ab3a1e1d564cca5c3e345c0efb383";
// //string hash = "c0228fcfb955b8dd3da03126533723b926c902da460262a50c21cf12e9e8223c";


// string hash = "ec2280a5f0b3100852097ca0aa81c5dc4a260cb062b7fb55e52a8b6febe8ac7c";
// int mod = 25; // Modulus value

// var result = DivisibleWithBreakdown(hash, mod);
// Console.WriteLine($"Is divisible: {result.IsDivisible}");

// foreach (var step in result.Breakdown)
// {
//     Console.WriteLine($"Chunk: {step.Chunk}, Value: {step.ChunkValue}, Val before: {step.ValBefore}, Val after: {step.ValAfter}");
// }

// static (bool IsDivisible, (string Chunk, int ChunkValue, BigInteger ValBefore, BigInteger ValAfter)[] Breakdown) DivisibleWithBreakdown(string hash, int mod)
// {
//     BigInteger val = 0;
//     var breakdown = new (string Chunk, int ChunkValue, BigInteger ValBefore, BigInteger ValAfter)[hash.Length / 4 + (hash.Length % 4 == 0 ? 0 : 1)];
//     int index = 0;

//     for (int i = 0; i < hash.Length; i += 4)
//     {
//         var chunk = hash.Substring(i, Math.Min(4, hash.Length - i));
//         var chunkValue = Convert.ToInt32(chunk, 16);
//         var valBefore = val;

//         val = ((val << 16) + chunkValue) % mod;
//         breakdown[index++] = (chunk, chunkValue, valBefore, val);
//     }

//     return (val == 0, breakdown);
// }