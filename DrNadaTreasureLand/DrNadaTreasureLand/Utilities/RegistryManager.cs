using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DrNadaTreasureLand.Utilities
{
    class RegistryManager
    {
        public RegistryManager()
        {
            
        }

        private static void CreateOrOpenKey(string keyName)
        {
            Registry.CurrentUser.CreateSubKey(keyName);
        }

        private static void SetValue(string keyName, string valueName, object value, RegistryValueKind valueKind)
        {
            CreateOrOpenKey(keyName); //Ensures the value exists

            Registry.SetValue(@"HKEY_CURRENT_USER\" + keyName, valueName, value, valueKind);
        }

        private static object GetValue(string keyName, string valueName)
        {
            CreateOrOpenKey(keyName);

            return Registry.GetValue(@"HKEY_CURRENT_USER\" + keyName, valueName, null);
        }

        public static void SaveEncryptedPass(byte[] bytes)
        {
            SetValue("DrNadaTreasureLand", "encryption", bytes, RegistryValueKind.Binary);
        }

        public static void SaveEntropy(byte[] bytes)
        {
            SetValue("DrNadaTreasureLand", "entropy", bytes, RegistryValueKind.Binary);
        }

        public static byte[] GetEncryptedPass()
        {
            return (byte[]) GetValue("DrNadaTreasureLand", "encryption");
        }

        public static byte[] GetEntropy()
        {
            return (byte[]) GetValue("DrNadaTreasureLand", "entropy");
        }

        public static string GetDecryptedPass()
        {
            return Security.DecryptPass(GetEncryptedPass(), GetEntropy());
        }

    }
}
