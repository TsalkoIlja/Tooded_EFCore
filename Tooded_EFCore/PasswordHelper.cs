using System;
using System.Collections.Generic;
using System.Text;

namespace Tooded_EFCore
{
    public static class PasswordHelper
    {
        public static string Hash(string parool)
        {
            return BCrypt.Net.BCrypt.HashPassword(parool);
        }

        public static bool Verify(string parool, string rasi)
        {
            return BCrypt.Net.BCrypt.Verify(parool, rasi);
        }
    }
}
