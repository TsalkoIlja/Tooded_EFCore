using System;
using System.Collections.Generic;
using System.Text;

namespace Tooded_EFCore
{
    public class Toode
    {
        public int Id { get; set; }
        public string Toodenimetus { get; set; }
        public int Kogus { get; set; }
        public float Hind { get; set; }
        public string Pilt { get; set; }

        public int KategooriaId { get; set; }
        public Kategooria Kategooria { get; set; }
    }
}
