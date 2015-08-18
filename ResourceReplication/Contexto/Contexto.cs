using ResourceReplication.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceReplication.Contexto
{
    public class Contexto : DbContext
    {
        static Contexto()
        {
            Database.SetInitializer<Contexto>(null);
        }

        public Contexto(): base("Traducao")
        {
        }

        public DbSet<Textos> Textos { get; set; }
    }
}
