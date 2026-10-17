using System;
using System.Collections.Generic;
using System.Linq;

namespace Tooded_EFCore
{
    public class OstukorviRida
    {
        public Toode Toode { get; set; } = null!;

        public int Kogus { get; set; }

        public decimal Summa =>
            Convert.ToDecimal(Toode.Hind) * Kogus;

        public override string ToString()
        {
            return $"{Toode.Toodenimetus} - {Kogus} tk - {Summa:F2} EUR";
        }
    }

    public static class Ostukorv
    {
        public static List<OstukorviRida> Ridad { get; } = new();

        public static decimal Kokku =>
            Ridad.Sum(r => r.Summa);

        public static bool Lisa(Toode toode, int kogus)
        {
            if (kogus <= 0)
                return false;

            var olemas = Ridad.FirstOrDefault(
                r => r.Toode.Id == toode.Id);

            int korvis = olemas?.Kogus ?? 0;

            if (korvis + kogus > toode.Kogus)
                return false;

            if (olemas != null)
            {
                olemas.Kogus += kogus;
            }
            else
            {
                Ridad.Add(new OstukorviRida
                {
                    Toode = toode,
                    Kogus = kogus
                });
            }

            return true;
        }

        public static void Tyhjenda()
        {
            Ridad.Clear();
        }
    }
}

