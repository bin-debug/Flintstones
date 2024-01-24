using System;
using System.Text;
using System.Security.Cryptography;

var hash = ComputeSHA256("17555205140148116567474855137667923114878434092627888279067931966791955067626");
//Console.WriteLine(hash);

for (int i = 0; i < 1000; i++)
{
    var newHash = ComputeSHA256(hash);
    var sb = new StringBuilder();
    sb.Append(@"games.Add(""");
    sb.Append(newHash);
    sb.Append(@""");");
    Console.WriteLine(sb.ToString());
    hash = newHash;
}

// var prev = GetPreviousHash("4981D13EC92EF45A22DFEE70F2497626F506B07490D21E95CDD096AE61B7A694");
// Console.WriteLine(prev);

static string ComputeSHA256(string s)
{
    string hash = String.Empty;

    // Initialize a SHA256 hash object
    using (SHA256 sha256 = SHA256.Create())
    {
        // Compute the hash of the given string
        byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

        // Convert the byte array to string format
        foreach (byte b in hashValue)
        {
            hash += $"{b:X2}";
        }
    }

    return hash;
}

static string GetPreviousHash(string gameHash)
{
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(gameHash));

        // Convert the byte array to a hexadecimal string
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }

        return builder.ToString();
    }
}
