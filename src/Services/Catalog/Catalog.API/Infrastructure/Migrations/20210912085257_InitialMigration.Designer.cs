﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Infrastructure.Migrations
{
    [DbContext(typeof(CatalogDbContext))]
    [Migration("20210912085257_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.HasSequence("catalog_brand_hilo")
                .IncrementsBy(10);

            modelBuilder.HasSequence("catalog_hilo")
                .IncrementsBy(10);

            modelBuilder.HasSequence("catalog_type_hilo")
                .IncrementsBy(10);

            modelBuilder.Entity("Microsoft.eShopOnDapr.Services.Catalog.API.Model.CatalogBrand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:HiLoSequenceName", "catalog_brand_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("CatalogBrand");

                    // b.HasData(
                    //     new
                    //     {
                    //         Id = 1,
                    //         Name = ".NET"
                    //     },
                    //     new
                    //     {
                    //         Id = 2,
                    //         Name = "Dapr"
                    //     },
                    //     new
                    //     {
                    //         Id = 3,
                    //         Name = "Other"
                    //     });
                });

            modelBuilder.Entity("Microsoft.eShopOnDapr.Services.Catalog.API.Model.CatalogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:HiLoSequenceName", "catalog_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<int>("AvailableStock")
                        .HasColumnType("int");

                    b.Property<int>("CatalogBrandId")
                        .HasColumnType("int");

                    b.Property<int>("CatalogTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PictureFileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasPrecision(4, 2)
                        .HasColumnType("decimal(4,2)");

                    b.HasKey("Id");

                    b.HasIndex("CatalogBrandId");

                    b.HasIndex("CatalogTypeId");

                    b.ToTable("CatalogItem");

                    // b.HasData(
                    //     new
                    //     {
                    //         Id = 1,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 5,
                    //         Name = ".NET Bot Black Hoodie",
                    //         PictureFileName = "1.png",
                    //         Price = 19.5m
                    //     },
                    //     new
                    //     {
                    //         Id = 2,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 2,
                    //         Name = ".NET Black & White Mug",
                    //         PictureFileName = "2.png",
                    //         Price = 8.5m
                    //     },
                    //     new
                    //     {
                    //         Id = 3,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 3,
                    //         CatalogTypeId = 5,
                    //         Name = "Prism White T-Shirt",
                    //         PictureFileName = "3.png",
                    //         Price = 12m
                    //     },
                    //     new
                    //     {
                    //         Id = 4,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 5,
                    //         Name = ".NET Foundation T-shirt",
                    //         PictureFileName = "4.png",
                    //         Price = 14.99m
                    //     },
                    //     new
                    //     {
                    //         Id = 5,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 3,
                    //         CatalogTypeId = 3,
                    //         Name = "Roslyn Red Pin",
                    //         PictureFileName = "5.png",
                    //         Price = 8.5m
                    //     },
                    //     new
                    //     {
                    //         Id = 6,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 5,
                    //         Name = ".NET Blue Hoodie",
                    //         PictureFileName = "6.png",
                    //         Price = 12m
                    //     },
                    //     new
                    //     {
                    //         Id = 7,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 3,
                    //         CatalogTypeId = 5,
                    //         Name = "Roslyn Red T-Shirt",
                    //         PictureFileName = "7.png",
                    //         Price = 12m
                    //     },
                    //     new
                    //     {
                    //         Id = 8,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 3,
                    //         CatalogTypeId = 5,
                    //         Name = "Kudu Purple Hoodie",
                    //         PictureFileName = "8.png",
                    //         Price = 8.5m
                    //     },
                    //     new
                    //     {
                    //         Id = 9,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 3,
                    //         CatalogTypeId = 2,
                    //         Name = "Cup<T> White Mug",
                    //         PictureFileName = "9.png",
                    //         Price = 12m
                    //     },
                    //     new
                    //     {
                    //         Id = 10,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 3,
                    //         Name = ".NET Foundation Pin",
                    //         PictureFileName = "10.png",
                    //         Price = 9m
                    //     },
                    //     new
                    //     {
                    //         Id = 11,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 3,
                    //         Name = "Cup<T> Pin",
                    //         PictureFileName = "11.png",
                    //         Price = 8.5m
                    //     },
                    //     new
                    //     {
                    //         Id = 12,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 3,
                    //         CatalogTypeId = 5,
                    //         Name = "Prism White TShirt",
                    //         PictureFileName = "12.png",
                    //         Price = 12m
                    //     },
                    //     new
                    //     {
                    //         Id = 13,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 2,
                    //         Name = "Modern .NET Black & White Mug",
                    //         PictureFileName = "13.png",
                    //         Price = 8.5m
                    //     },
                    //     new
                    //     {
                    //         Id = 14,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 1,
                    //         CatalogTypeId = 2,
                    //         Name = "Modern Cup<T> White Mug",
                    //         PictureFileName = "14.png",
                    //         Price = 12m
                    //     },
                    //     new
                    //     {
                    //         Id = 15,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 2,
                    //         CatalogTypeId = 1,
                    //         Name = "Dapr Cap",
                    //         PictureFileName = "15.png",
                    //         Price = 9.99m
                    //     },
                    //     new
                    //     {
                    //         Id = 16,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 2,
                    //         CatalogTypeId = 5,
                    //         Name = "Dapr Zipper Hoodie",
                    //         PictureFileName = "16.png",
                    //         Price = 14.99m
                    //     },
                    //     new
                    //     {
                    //         Id = 17,
                    //         AvailableStock = 100,
                    //         CatalogBrandId = 2,
                    //         CatalogTypeId = 4,
                    //         Name = "Dapr Logo Sticker",
                    //         PictureFileName = "17.png",
                    //         Price = 1.99m
                    //     });
                });

            modelBuilder.Entity("Microsoft.eShopOnDapr.Services.Catalog.API.Model.CatalogType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:HiLoSequenceName", "catalog_type_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("CatalogType");

                    // b.HasData(
                    //     new
                    //     {
                    //         Id = 1,
                    //         Name = "Cap"
                    //     },
                    //     new
                    //     {
                    //         Id = 2,
                    //         Name = "Mug"
                    //     },
                    //     new
                    //     {
                    //         Id = 3,
                    //         Name = "Pin"
                    //     },
                    //     new
                    //     {
                    //         Id = 4,
                    //         Name = "Sticker"
                    //     },
                    //     new
                    //     {
                    //         Id = 5,
                    //         Name = "T-Shirt"
                    //     });
                });

            modelBuilder.Entity("Microsoft.eShopOnDapr.Services.Catalog.API.Model.CatalogItem", b =>
                {
                    b.HasOne("Microsoft.eShopOnDapr.Services.Catalog.API.Model.CatalogBrand", "CatalogBrand")
                        .WithMany()
                        .HasForeignKey("CatalogBrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.eShopOnDapr.Services.Catalog.API.Model.CatalogType", "CatalogType")
                        .WithMany()
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CatalogBrand");

                    b.Navigation("CatalogType");
                });
#pragma warning restore 612, 618
        }
    }
}
