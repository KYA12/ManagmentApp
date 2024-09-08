﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BakeryOrderManagmentSystem.API.Migrations
{
    [DbContext(typeof(BakeryDbContext))]
    [Migration("20240907173832_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BakeryOrderManagmentSystem.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.ToTable("Orders");

                    b.HasData(
                        new
                        {
                            OrderId = 1,
                            CustomerId = 1,
                            OrderDate = new DateTime(2024, 9, 7, 20, 38, 31, 434, DateTimeKind.Local).AddTicks(7338),
                            Status = 0
                        },
                        new
                        {
                            OrderId = 2,
                            CustomerId = 2,
                            OrderDate = new DateTime(2024, 9, 7, 20, 38, 31, 434, DateTimeKind.Local).AddTicks(7418),
                            Status = 2
                        });
                });

            modelBuilder.Entity("OrdersProducts", b =>
                {
                    b.Property<int>("OrderProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderProductId"));

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("OrderProductId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrdersProducts");

                    b.HasData(
                        new
                        {
                            OrderProductId = 1,
                            OrderId = 1,
                            ProductId = 1,
                            Quantity = 2
                        },
                        new
                        {
                            OrderProductId = 2,
                            OrderId = 1,
                            ProductId = 2,
                            Quantity = 3
                        },
                        new
                        {
                            OrderProductId = 3,
                            OrderId = 2,
                            ProductId = 3,
                            Quantity = 1
                        });
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ProductId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            Description = "Delicious chocolate cake",
                            IsActive = true,
                            Name = "Chocolate Cake",
                            Price = 15.99m
                        },
                        new
                        {
                            ProductId = 2,
                            Description = "Buttery croissant",
                            IsActive = false,
                            Name = "Croissant",
                            Price = 2.99m
                        },
                        new
                        {
                            ProductId = 3,
                            Description = "Sweet apple pie",
                            IsActive = true,
                            Name = "Apple Pie",
                            Price = 12.99m
                        });
                });

            modelBuilder.Entity("OrdersProducts", b =>
                {
                    b.HasOne("BakeryOrderManagmentSystem.Models.Order", "Order")
                        .WithMany("OrdersProducts")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Product", "Product")
                        .WithMany("OrdersProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("BakeryOrderManagmentSystem.Models.Order", b =>
                {
                    b.Navigation("OrdersProducts");
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.Navigation("OrdersProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
