using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

public static class Authentication
{
    public static bool RegisterUser(User user, UserRepository userRepo)
    {
        HashSet<string> logins = userRepo.GetAllLogins();
        if(logins.Contains(user.login))
        {
            return false;
        }
        SHA256 sha256 = SHA256.Create();
        user.password =  GetHash(sha256, user.password);
        sha256.Dispose();
        int id = userRepo.Insert(user);
        user.id = id;
        return true;
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
       byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
 
       var sb = new StringBuilder();

       for (int i = 0; i < data.Length; i++)
       {
            sb.Append(data[i].ToString("x2"));
       }
 
       return sb.ToString();
    }


    public static User LoginUser(string username, string password, UserRepository userRepo)
    {
        User potentialUser = userRepo.GetByLogin(username);
        if(potentialUser == null)
        {
            return null;
        }
        SHA256 sha256 = SHA256.Create();
        string  inputPassword =  GetHash(sha256, password);
        sha256.Dispose();
        if(potentialUser.password == inputPassword)
        {
            return potentialUser;
        }
        return null;
    }
}