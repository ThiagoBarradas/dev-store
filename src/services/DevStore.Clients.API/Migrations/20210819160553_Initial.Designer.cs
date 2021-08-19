﻿// <auto-generated />
using System;
using DevStore.Clients.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevStore.Clients.API.Migrations
{
    [DbContext(typeof(ClientContext))]
    [Migration("20210819160553_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DevStore.Clients.API.Models.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BuildingNumber")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Neighborhood")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("SecondaryAddress")
                        .HasColumnType("varchar(250)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("StreetAddress")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("DevStore.Clients.API.Models.Client", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("SocialNumber")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("DevStore.Clients.API.Models.Address", b =>
                {
                    b.HasOne("DevStore.Clients.API.Models.Client", "Client")
                        .WithOne("Address")
                        .HasForeignKey("DevStore.Clients.API.Models.Address", "ClientId")
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("DevStore.Clients.API.Models.Client", b =>
                {
                    b.OwnsOne("DevStore.Core.DomainObjects.Email", "Email", b1 =>
                        {
                            b1.Property<Guid>("ClientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasColumnType("varchar(254)")
                                .HasColumnName("Email");

                            b1.HasKey("ClientId");

                            b1.ToTable("Clients");

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.Navigation("Email");
                });

            modelBuilder.Entity("DevStore.Clients.API.Models.Client", b =>
                {
                    b.Navigation("Address");
                });
#pragma warning restore 612, 618
        }
    }
}
