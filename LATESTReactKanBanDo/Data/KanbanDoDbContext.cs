using LATESTReactKanBanDo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LATESTReactKanBanDo.Data
{
    public class KanbanDoDbContext : DbContext
    {
        public DbSet<View> Views { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        public KanbanDoDbContext(DbContextOptions<KanbanDoDbContext> options) : base(options)
        {

        }
    }
}
