using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CrookedPortal.Models
{
    public partial class CrookedPortalContext : DbContext
    {
        public CrookedPortalContext()
        {
        }

        public CrookedPortalContext(DbContextOptions<CrookedPortalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<ChangedProduct> ChangedProducts { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<CrossPlatformProductListing> CrossPlatformProductListings { get; set; }
        public virtual DbSet<DataTransfer> DataTransfers { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<EtsyConnectionDatum> EtsyConnectionData { get; set; }
        public virtual DbSet<EtsyListing> EtsyListings { get; set; }
        public virtual DbSet<EtsyListingImage> EtsyListingImages { get; set; }
        public virtual DbSet<EtsyListingProduct> EtsyListingProducts { get; set; }
        public virtual DbSet<GcconnectionDatum> GcconnectionData { get; set; }
        public virtual DbSet<Gcproduct> Gcproducts { get; set; }
        public virtual DbSet<GcproductImage> GcproductImages { get; set; }
        public virtual DbSet<GcproductVariant> GcproductVariants { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<MctconnectionDatum> MctconnectionData { get; set; }
        public virtual DbSet<Mctproduct> Mctproducts { get; set; }
        public virtual DbSet<MctproductImage> MctproductImages { get; set; }
        public virtual DbSet<MctproductVariant> MctproductVariants { get; set; }
        public virtual DbSet<ProductChange> ProductChanges { get; set; }
        public virtual DbSet<Schema> Schemas { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<ShopifyConnectionDatum> ShopifyConnectionData { get; set; }
        public virtual DbSet<State> States { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Properties.Resources.SQLServerConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("cragga")
                .HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ChangedProduct>(entity =>
            {
                entity.ToTable("ChangedProducts", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key, "CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<CrossPlatformProductListing>(entity =>
            {
                entity.ToTable("CrossPlatformProductListings", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EtsyListingId).HasColumnName("EtsyListingID");

                entity.Property(e => e.GcproductId).HasColumnName("GCProductID");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.MctproductId).HasColumnName("MCTProductID");

                entity.HasOne(d => d.EtsyListing)
                    .WithMany(p => p.CrossPlatformProductListings)
                    .HasForeignKey(d => d.EtsyListingId)
                    .HasConstraintName("FK_CrossPlatformProductListings_EtsyListings");

                entity.HasOne(d => d.Gcproduct)
                    .WithMany(p => p.CrossPlatformProductListings)
                    .HasForeignKey(d => d.GcproductId)
                    .HasConstraintName("FK_CrossPlatformProductListings_GCProducts");

                entity.HasOne(d => d.Mctproduct)
                    .WithMany(p => p.CrossPlatformProductListings)
                    .HasForeignKey(d => d.MctproductId)
                    .HasConstraintName("FK_CrossPlatformProductListings_MCTProducts");
            });

            modelBuilder.Entity<DataTransfer>(entity =>
            {
                entity.ToTable("DataTransfers", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateOfTransfer)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2021-11-27 16:26:00.000')");

                entity.Property(e => e.GcproductsTransferred).HasColumnName("GCProductsTransferred");

                entity.Property(e => e.MctproductsTransferred).HasColumnName("MCTProductsTransferred");
            });

            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.ToTable("ErrorLog", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateOfError).HasColumnType("datetime");

                entity.Property(e => e.Message).IsRequired();
            });

            modelBuilder.Entity<EtsyConnectionDatum>(entity =>
            {
                entity.ToTable("EtsyConnectionData", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.EtsyApiaccessToken)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyAPIAccessToken");

                entity.Property(e => e.EtsyApikey)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyAPIKey");

                entity.Property(e => e.EtsyApirefreshToken)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyAPIRefreshToken");

                entity.Property(e => e.EtsyLastReceiptPoll).HasColumnType("datetime");

                entity.Property(e => e.EtsyListingInventoryUrl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyListingInventoryURL");

                entity.Property(e => e.EtsyListingUrl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyListingURL");

                entity.Property(e => e.EtsyListingsUrl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyListingsURL");

                entity.Property(e => e.EtsyReceiptsUrl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyReceiptsURL");

                entity.Property(e => e.EtsyShopId).HasColumnName("EtsyShopID");

                entity.Property(e => e.EtsyTokenDataUrl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("EtsyTokenDataURL");
            });

            modelBuilder.Entity<EtsyListing>(entity =>
            {
                entity.ToTable("EtsyListings", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('01/01/2021 00:00:00')");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<EtsyListingImage>(entity =>
            {
                entity.ToTable("EtsyListingImages", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.ListingId).HasColumnName("ListingID");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL");

                entity.HasOne(d => d.Listing)
                    .WithMany(p => p.EtsyListingImages)
                    .HasForeignKey(d => d.ListingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EtsyListingImages_EtsyListings");
            });

            modelBuilder.Entity<EtsyListingProduct>(entity =>
            {
                entity.ToTable("EtsyListingProducts", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Colour).HasMaxLength(50);

                entity.Property(e => e.GcvariantId).HasColumnName("GCVariantID");

                entity.Property(e => e.ListingId).HasColumnName("ListingID");

                entity.Property(e => e.MctvariantId).HasColumnName("MCTVariantID");

                entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.HasOne(d => d.Listing)
                    .WithMany(p => p.EtsyListingProducts)
                    .HasForeignKey(d => d.ListingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EtsyListingProducts_EtsyListings");
            });

            modelBuilder.Entity<GcconnectionDatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GCConnectionData", "dbo");

                entity.Property(e => e.Gcapikey)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("GCAPIKey");

                entity.Property(e => e.Gcapiurl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("GCAPIURL");

                entity.Property(e => e.GclocationId).HasColumnName("GCLocationID");

                entity.Property(e => e.Gcpassword)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("GCPassword");
            });

            modelBuilder.Entity<Gcproduct>(entity =>
            {
                entity.ToTable("GCProducts", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('01/01/2021 00:00:00')");

                entity.Property(e => e.PublishedScope).HasMaxLength(10);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<GcproductImage>(entity =>
            {
                entity.ToTable("GCProductImages", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.GcproductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GCProductImages_GCProducts");
            });

            modelBuilder.Entity<GcproductVariant>(entity =>
            {
                entity.ToTable("GCProductVariants", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Colour).HasMaxLength(50);

                entity.Property(e => e.CompareAtPrice).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.EtsyProductId).HasColumnName("EtsyProductID");

                entity.Property(e => e.InventoryItemId).HasColumnName("InventoryItemID");

                entity.Property(e => e.InventoryPolicy)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.MctvariantId).HasColumnName("MCTVariantID");

                entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.GcproductVariants)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GCProductVariants_GCProducts");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MctconnectionDatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MCTConnectionData", "dbo");

                entity.Property(e => e.Mctapikey)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("MCTAPIKey");

                entity.Property(e => e.Mctapiurl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("MCTAPIURL");

                entity.Property(e => e.MctlocationId).HasColumnName("MCTLocationID");

                entity.Property(e => e.Mctpassword)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("MCTPassword");
            });

            modelBuilder.Entity<Mctproduct>(entity =>
            {
                entity.ToTable("MCTProducts", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('01/01/2021 00:00:00')");

                entity.Property(e => e.PublishedScope).HasMaxLength(10);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<MctproductImage>(entity =>
            {
                entity.ToTable("MCTProductImages", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.MctproductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MCTProductImages_MCTProducts");
            });

            modelBuilder.Entity<MctproductVariant>(entity =>
            {
                entity.ToTable("MCTProductVariants", "dbo");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Colour).HasMaxLength(50);

                entity.Property(e => e.CompareAtPrice).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.EtsyProductId).HasColumnName("EtsyProductID");

                entity.Property(e => e.GcvariantId).HasColumnName("GCVariantID");

                entity.Property(e => e.InventoryItemId).HasColumnName("InventoryItemID");

                entity.Property(e => e.InventoryPolicy)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Price).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.MctproductVariants)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MCTProductVariants_MCTProducts");
            });

            modelBuilder.Entity<ProductChange>(entity =>
            {
                entity.ToTable("ProductChanges", "dbo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Operation)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShopifyConnectionDatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ShopifyConnectionData", "dbo");

                entity.Property(e => e.ShopifyProductsUrl)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("ShopifyProductsURL");

                entity.Property(e => e.ShopifyProductsUrlfields)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("ShopifyProductsURLFields");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
