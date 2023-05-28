using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Obb.Models;

namespace Obb.Data;

public partial class ObbContext : DbContext
{
    public ObbContext(DbContextOptions<ObbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<ObbUser> ObbUser { get; set; }
}
