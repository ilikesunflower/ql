using System;
using System.ComponentModel;
using System.Reflection;
using CMS_EF.Models;
using CMS_EF.Models.Articles;
using CMS_EF.Models.Categories;
using CMS_EF.Models.Customers;
using CMS_EF.Models.Identity;
using CMS_EF.Models.Orders;
using CMS_EF.Models.PreOrders;
using CMS_EF.Models.Products;
using CMS_EF.Models.Ship;
using CMS_EF.Models.WareHouse;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMS_EF.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, int,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #region Articles

        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<ArticleType> ArticleType { get; set; }

        #endregion

        #region Categories

        public virtual DbSet<Banner> Banner { get; set; }
        public virtual DbSet<Commune> Commune { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<PartnerShip> PartnerShip { get; set; }
        public virtual DbSet<Province> Province { get; set; }

        #endregion

        #region Customer

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddress { get; set; }
        public virtual DbSet<CustomerCard> CustomerCard { get; set; }
        public virtual DbSet<OrderAddress> OrderAddress { get; set; }
        public virtual DbSet<CustomerCoupon> CustomerCoupon { get; set; }
        public virtual DbSet<CustomerCouponLog> CustomerCouponLog { get; set; }
        public virtual DbSet<CustomerPoint> CustomerPoint { get; set; }
        public virtual DbSet<CustomerPointLog> CustomerPointLog { get; set; }
        public virtual DbSet<CustomerTracking> CustomerTracking { get; set; }
        public virtual DbSet<FeNotification> FeNotification { get; set; }
        public virtual DbSet<FeNotificationCustomer> FeNotificationCustomer { get; set; }
        public virtual DbSet<HistoryFileChargePoint> HistoryFileChargePoint { get; set; }
        public virtual DbSet<HistoryFileCoupon> HistoryFileCoupon { get; set; }

        #endregion

        #region Orders

        public virtual DbSet<OrderLog> OrderLog { get; set; }
        public virtual DbSet<OrderProduct> OrderProduct { get; set; }
        public virtual DbSet<OrderProductRateComment> OrderProductRateComment { get; set; }
        public virtual DbSet<OrderProductSimilarProperty> OrderProductSimilarProperty { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderPartnerShipLog> OrderPartnerShipLog { get; set; }
        public virtual DbSet<OrderPoint> OrderPoint { get; set; }

        #endregion

        #region Products

        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductCategoryProduct> ProductCategoryProduct { get; set; }
        public virtual DbSet<ProductGroup> ProductGroup { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<ProductPropertieValue> ProductPropertieValue { get; set; }
        public virtual DbSet<ProductProperties> ProductProperties { get; set; }
        public virtual DbSet<ProductPurpose> ProductPurpose { get; set; }
        public virtual DbSet<ProductSimilar> ProductSimilar { get; set; }
        public virtual DbSet<ProductSimilarProperty> ProductSimilarProperty { get; set; }
        public virtual DbSet<Products> Products { get; set; }

        #endregion

        #region Ship

        public virtual DbSet<ShipGhn> ShipGhn { get; set; }
        public virtual DbSet<ShipVnPost> ShipVnPost { get; set; }

        #endregion

        #region PreOrders

        public virtual DbSet<PreOrder> PreOrder { get; set; }
        public virtual DbSet<PreOrderAddress> PreOrderAddress { get; set; }

        #endregion

        #region WareHouse

        public virtual DbSet<WhKiotViet> WhKiotViet { get; set; }
        public virtual DbSet<WhTransaction> WhTransaction { get; set; }


        #endregion

        #region Base Entity

        public virtual DbSet<ApplicationController> ApplicationControllers { get; set; }
        public virtual DbSet<ApplicationAction> ApplicationActions { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Logging> Logging { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationUser> NotificationUsers { get; set; }
        public virtual DbSet<NotificationUserViewTime> NotificationUserViewTimes { get; set; }

        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        [Obsolete]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            #region base

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var memberInfo = property.PropertyInfo ?? (MemberInfo)property.FieldInfo;
                    if (memberInfo == null) continue;
                    if (!(Attribute.GetCustomAttribute(memberInfo, typeof(DefaultValueAttribute)) is
                            DefaultValueAttribute defaultValue)) continue;
                    property.SetDefaultValueSql(defaultValue.Value?.ToString());
                }
            }

            #endregion

            #region Articles

            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasOne(d => d.ArticleTypeNavigation)
                    .WithMany(p => p.Article)
                    .HasForeignKey(d => d.ArticleType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Article__Article__5FB337D6");
            });

            #endregion

            #region Customer

            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.HasOne(d => d.CommuneCodeNavigation)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.CommuneCode)
                    .HasConstraintName("FK__CustomerA__Commu__4242D080");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__CustomerA__Custo__24E777C3");

                entity.HasOne(d => d.DistrictCodeNavigation)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.DistrictCode)
                    .HasConstraintName("FK__CustomerA__Distr__4336F4B9");

                entity.HasOne(d => d.ProvinceCodeNavigation)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.ProvinceCode)
                    .HasConstraintName("FK__CustomerA__Provi__442B18F2");
            });

            modelBuilder.Entity<CustomerCard>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCard)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__CustomerC__Custo__23F3538A");
            });

            modelBuilder.Entity<CustomerCoupon>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCoupon)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__CustomerC__Custo__24B26D99");
            });

            modelBuilder.Entity<CustomerCouponLog>(entity =>
            {
                entity.HasOne(d => d.CustomerCoupon)
                    .WithMany(p => p.CustomerCouponLog)
                    .HasForeignKey(d => d.CustomerCouponId)
                    .HasConstraintName("FK__CustomerC__Custo__2882FE7D");
            });

            modelBuilder.Entity<CustomerPoint>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerPoint)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__CustomerP__Custo__30242045");
            });

            modelBuilder.Entity<CustomerPointLog>(entity =>
            {
                entity.HasOne(d => d.CustomerPoint)
                    .WithMany(p => p.CustomerPointLog)
                    .HasForeignKey(d => d.CustomerPointId)
                    .HasConstraintName("FK__CustomerP__Custo__2F2FFC0C");
            });

            modelBuilder.Entity<CustomerTracking>(entity =>
            {
                //entity.Property(e => e.Id).ValueGeneratedNever();

                //entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerTracking)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerT__Custo__0C50D423");
            });

            modelBuilder.Entity<FeNotificationCustomer>(entity =>
            {
                entity.HasOne(d => d.FeNotification)
                    .WithMany(p => p.FeNotificationCustomer)
                    .HasForeignKey(d => d.FeNotificationId)
                    .HasConstraintName("FK__FeNotific__FeNot__7A8729A3");
            });

            #endregion

            #region Categories

            modelBuilder.Entity<Commune>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__Commune__A25C5AA6536F893B");

                entity.HasOne(d => d.DistrictCodeNavigation)
                    .WithMany(p => p.Commune)
                    .HasForeignKey(d => d.DistrictCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Commune__Distric__160F4887");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__District__A25C5AA6B56D1C8F");

                entity.HasOne(d => d.ProvinceCodeNavigation)
                    .WithMany(p => p.District)
                    .HasForeignKey(d => d.ProvinceCode)
                    .HasConstraintName("FK__District__Provin__151B244E");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__Province__A25C5AA67B90F136");
            });

            #endregion

            #region Orders
            
            modelBuilder.Entity<OrderLog>(entity =>
            {
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderLog)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderLog__OrderI__27C3E46E");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderProduct)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderProd__Order__28B808A7");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProduct)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderProduct_Products");
            });
            
            modelBuilder.Entity<OrderProductRateComment>(entity =>
            {
                entity.HasOne(d => d.OrderProduct)
                    .WithMany(p => p.OrderProductRateComment)
                    .HasForeignKey(d => d.OrderProductId)
                    .HasConstraintName("FK__OrderProd__Order__0539C240");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProductRateComment)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__OrderProd__Produ__0AF29B96");
            });

            modelBuilder.Entity<OrderProductSimilarProperty>(entity =>
            {
                entity.HasOne(d => d.OrderProduct)
                    .WithMany(p => p.OrderProductSimilarProperty)
                    .HasForeignKey(d => d.OrderProductId)
                    .HasConstraintName("FK__OrderProd__Order__29AC2CE0");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Orders__Customer__26CFC035");
            });

            modelBuilder.Entity<OrderPoint>(entity =>
            {
                entity.HasOne(d => d.CustomerPoint)
                    .WithMany(p => p.OrderPoint)
                    .HasForeignKey(d => d.CustomerPointId)
                    .HasConstraintName("FK__OrderPoin__Custo__041093DD");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderPoint)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderPoin__Order__031C6FA4");
            });

            #endregion

            #region Products

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.Property(e => e.Ord).HasDefaultValueSql("((1000))");
            });

            modelBuilder.Entity<ProductCategoryProduct>(entity =>
            {
                entity.HasOne(d => d.Pcategory)
                    .WithMany(p => p.ProductCategoryProduct)
                    .HasForeignKey(d => d.PcategoryId)
                    .HasConstraintName("FK__ProductCa__PCate__603D47BB");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductCategoryProduct)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductCa__Produ__5F492382");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImage)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__Produ__59904A2C");
            });

            modelBuilder.Entity<ProductPropertieValue>(entity =>
            {
                entity.HasOne(d => d.ProductProperties)
                    .WithMany(p => p.ProductPropertieValue)
                    .HasForeignKey(d => d.ProductPropertiesId)
                    .HasConstraintName("FK__ProductPr__Produ__595B4002");
            });

            modelBuilder.Entity<ProductProperties>(entity =>
            {
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductProperties)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductPr__Produ__58671BC9");
            });

            modelBuilder.Entity<ProductSimilar>(entity =>
            {
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductSimilar)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductSi__Produ__5A846E65");
            });

            modelBuilder.Entity<ProductSimilarProperty>(entity =>
            {
                entity.HasOne(d => d.ProductPropertiesValue)
                    .WithMany(p => p.ProductSimilarProperty)
                    .HasForeignKey(d => d.ProductPropertiesValueId)
                    .HasConstraintName("FK__ProductSi__Produ__5B78929E");

                entity.HasOne(d => d.ProductSimilar)
                    .WithMany(p => p.ProductSimilarProperty)
                    .HasForeignKey(d => d.ProductSimilarId)
                    .HasConstraintName("FK__ProductSi__Produ__5C6CB6D7");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasOne(d => d.ProductGroup)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductGroupId)
                    .HasConstraintName("FK__Products__Produc__55BFB948");

                entity.HasOne(d => d.ProductPurpose)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductPurposeId)
                    .HasConstraintName("FK__Products__Produc__5E54FF49");
            });

            #endregion

            #region PreOrder

            modelBuilder.Entity<PreOrder>(entity =>
            {
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.PreOrder)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__PreOrder__Custom__04459E07");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PreOrder)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__PreOrder__Produc__6C390A4C");

                entity.HasOne(d => d.ProductSimilar)
                    .WithMany(p => p.PreOrder)
                    .HasForeignKey(d => d.ProductSimilarId)
                    .HasConstraintName("FK__PreOrder__Produc__6D2D2E85");
            });

            modelBuilder.Entity<PreOrderAddress>(entity =>
            {
                entity.HasOne(d => d.PreOrder)
                    .WithOne(p => p.PreOrderAddress)
                    .HasForeignKey<PreOrderAddress>(d => d.PreOrderId)
                    .HasConstraintName("FK__PreOrderA__PreOr__035179CE");
            });

            #endregion

            // OnModelCreatingPartial(modelBuilder);
        }
    }
}