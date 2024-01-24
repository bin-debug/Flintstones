// using System;
// using System.Security.Cryptography;
// using System.Text;
// using System.Numerics;
// using System.Globalization;

// //int houseEdge = 100 / 4;

// static string GetCrashHash(string gameHash, string salt)
// {
//     using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(gameHash)))
//     {
//         // Convert the salt to a byte array and compute the hash
//         byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
//         byte[] hashBytes = hmac.ComputeHash(saltBytes);

//         // Convert the byte array to a hex string
//         StringBuilder sb = new StringBuilder();
//         foreach (byte b in hashBytes)
//         {
//             sb.AppendFormat("{0:x2}", b);
//         }

//         return sb.ToString();
//     }
// }

// static bool DivisibleWithBreakdown(string hash, int mod)
// {
//     BigInteger val = 0;
//     for (int i = 0; i < hash.Length; i += 4)
//     {
//         var chunk = hash.Substring(i, Math.Min(4, hash.Length - i));
//         var chunkValue = Convert.ToInt32(chunk, 16);
//         var valBefore = val;

//         val = ((val << 16) + chunkValue) % mod;
//     }

//     return (val == 0);
// }

// static double GetCrashPoint(string hash)
// {
//     //int houseEdge = 100 / 4;
//     if(DivisibleWithBreakdown(hash, 25) == true)
//         return 1;
//     else
//     {
//         return CalculateValue(hash);
//     }
// }

// static double CalculateValue(string hash)
// {
//     BigInteger h = BigInteger.Parse("0" + hash.Substring(0, 13), System.Globalization.NumberStyles.HexNumber);
//     BigInteger power = BigInteger.Pow(2, 52);
//     double crashPoint = Math.Floor((double)(100 * power - h) / (double)(power - h)) / 100.0;
//     return crashPoint;
// }

// static string GetPreviousHash(string gameHash)
// {
//     using (SHA256 sha256 = SHA256.Create())
//     {
//         byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(gameHash));

//         // Convert the byte array to a hexadecimal string
//         StringBuilder builder = new StringBuilder();
//         for (int i = 0; i < bytes.Length; i++)
//         {
//             builder.Append(bytes[i].ToString("x2"));
//         }

//         return builder.ToString();
//     }
// }

// string gameHash = "E6CB5BC70A740C4431C8DDEE9C9815F6BD2D31D40B574D9F665E21A0EDA85CE7";
// string salt = "0xd2867566759e9158bda9bf93b343bbd9aa02ce1e0c5bc2b37a2d70d391b04f14";

// string crashHash = GetCrashHash(gameHash, salt);
// Console.WriteLine(crashHash);
// var crashPoint = GetCrashPoint(crashHash);
// Console.WriteLine(crashPoint);

// // var values = new Dictionary<string,double>();
// // var previous = GetPreviousHash(gameHash);
// // Console.WriteLine("previous : " + previous);

// // for (int i = 0; i < 11; i++)
// // {
// //     string prevCrashHash = GetCrashHash(previous, salt);
// //     var prevCrashPoint = GetCrashPoint(prevCrashHash);
// //     values.Add(previous, prevCrashPoint);
// //     previous = GetPreviousHash(previous);
// // }

// // foreach (var item in values)
// // {
// //     Console.WriteLine(item.Key + " " + item.Value);
// // }