// using System;
// using System.Security.Cryptography;
// using System.Text;
// using System.Numerics;
// using System.Globalization;


// string serverSeed = "8b963345a9d0ae102c030d51128e023043f00b82724f2d31114b9193b45e1a09";
// string salt = "0xd2867566759e9158bda9bf93b343bbd9aa02ce1e0c5bc2b37a2d70d391b04f14";

//         double result = CrashPointFromHash(serverSeed, salt);
//         Console.WriteLine($"Result: {result:0.00}");

//    static double CrashPointFromHash(string serverSeed, string salt)
//     {
//             Console.WriteLine("Copied from the game: " + serverSeed);

//         using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(serverSeed)))
//         {
//             byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(salt));
//             string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

//             Console.WriteLine("This is the hash that is used to get the crash: " + hash);

//             BigInteger h = BigInteger.Parse("0" + hash.Substring(0, 13), System.Globalization.NumberStyles.HexNumber);
//             Console.WriteLine("BigInt: " + h);
//             BigInteger power = BigInteger.Pow(2, 52);
//             Console.WriteLine("power: " + power);
//             double crashPoint = Math.Floor((double)(100 * power - h) / (double)(power - h)) / 100.0;
//             Console.WriteLine("crashPoint: " + crashPoint);
//             return 1;


//             // double value = (double)(100 * e - h) / (double)(e - h);
//             // return Math.Floor(value * 100) / 100.0;
//         }
//     }


// // string gameHash = "cebb36c50d700e6a21f0ea92c47799af06522ce5db58fcee65dc83b748a70c20";
// // string salt = "0xd2867566759e9158bda9bf93b343bbd9aa02ce1e0c5bc2b37a2d70d391b04f14";
// // int houseEdge = 100 / 4;

// // static string GetCrashHash(string gameHash, string salt)
// // {
// //     using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(gameHash)))
// //     {
// //         // Convert the salt to a byte array and compute the hash
// //         byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
// //         byte[] hashBytes = hmac.ComputeHash(saltBytes);

// //         // Convert the byte array to a hex string
// //         StringBuilder sb = new StringBuilder();
// //         foreach (byte b in hashBytes)
// //         {
// //             sb.AppendFormat("{0:x2}", b);
// //         }

// //         return sb.ToString();
// //     }
// // }

// // static bool DivisibleWithBreakdown(string hash, int mod)
// // {
// //     BigInteger val = 0;
// //     int index = 0;

// //     for (int i = 0; i < hash.Length; i += 4)
// //     {
// //         var chunk = hash.Substring(i, Math.Min(4, hash.Length - i));
// //         var chunkValue = Convert.ToInt32(chunk, 16);
// //         var valBefore = val;

// //         val = ((val << 16) + chunkValue) % mod;
// //     }

// //     return (val == 0);
// // }

// // static double GetCrashPoint(string hash)
// // {
// //     if(DivisibleWithBreakdown(hash, 25) == true)
// //         return 1;
// //     else
// //     {
// //         return CalculateValue(hash);
// //     }
// // }
 

// // string crashHash = GetCrashHash(gameHash, salt);
// // Console.WriteLine(crashHash);
// // var crashPoint = GetCrashPoint(crashHash);
// // Console.WriteLine(crashPoint);