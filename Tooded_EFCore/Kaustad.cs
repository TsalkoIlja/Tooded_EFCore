using System;
using System.IO;

namespace Tooded_EFCore
{
    public static class Kaustad
    {
        public static readonly string Juur = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "ToodedApp");

        public static readonly string Pildid =
            Path.Combine(Juur, "Images");

        public static readonly string Arved =
            Path.Combine(Juur, "Arved");

        public static void Loo()
        {
            Directory.CreateDirectory(Pildid);
            Directory.CreateDirectory(Arved);
        }
    }
}

