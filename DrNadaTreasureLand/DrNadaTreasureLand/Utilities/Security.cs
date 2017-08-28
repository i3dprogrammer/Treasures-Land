using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DrNadaTreasureLand.Utilities
{
    class Security
    {

        public static byte[] EncryptPass(string pass)
        {
            //Checks if there's entropy in the registry
            byte[] entropy = RegistryManager.GetEntropy();

            if (entropy == null) //If there's none, create new entropy
            {
                entropy = new byte[20];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(entropy);
                }
                RegistryManager.SaveEntropy(entropy); //Save the newly created entropy in the registry.
            }

            return ProtectedData.Protect(Encoding.UTF8.GetBytes(pass), entropy, DataProtectionScope.CurrentUser); //Return the encrypted password.
        }

        public static string DecryptPass(byte[] encPass, byte[] entropy)
        {
            if (encPass == null || entropy == null)
                return "";

            return Encoding.UTF8.GetString(ProtectedData.Unprotect(encPass, entropy, DataProtectionScope.CurrentUser));
        }


    }
}
