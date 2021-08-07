﻿// <auto-generated />
using System;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ASBDDS.API.Migrations.DataDb
{
    [DbContext(typeof(DataDbContext))]
    [Migration("20210807161806_addRents")]
    partial class addRents
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("BaseModel")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("ExternalId")
                        .HasColumnType("uuid");

                    b.Property<string>("MacAddress")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<string>("Serial")
                        .HasColumnType("text");

                    b.Property<int>("StateEnum")
                        .HasColumnType("integer");

                    b.Property<Guid?>("SwitchPortId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SwitchPortId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.DeviceRent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("RentEnd")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("DeviceRents");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AllowCustomBootloaders")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("DefaultVlan")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ProjectDeviceLimit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectDeviceLimits");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Router", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Routers");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Switch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessProtocol")
                        .HasColumnType("integer");

                    b.Property<int>("AuthMethod")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Ip")
                        .HasColumnType("text");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<Guid?>("RouterId")
                        .HasColumnType("uuid");

                    b.Property<string>("Serial")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RouterId");

                    b.ToTable("Switches");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.SwitchPort", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Number")
                        .HasColumnType("text");

                    b.Property<Guid?>("SwitchId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SwitchId");

                    b.ToTable("SwitchPorts");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Device", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.SwitchPort", "SwitchPort")
                        .WithMany()
                        .HasForeignKey("SwitchPortId");

                    b.Navigation("Project");

                    b.Navigation("SwitchPort");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.DeviceRent", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ProjectDeviceLimit", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Project", "Project")
                        .WithMany("ProjectDeviceLimits")
                        .HasForeignKey("ProjectId");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Switch", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Router", "Router")
                        .WithMany("Switches")
                        .HasForeignKey("RouterId");

                    b.Navigation("Router");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.SwitchPort", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Switch", "Switch")
                        .WithMany("Ports")
                        .HasForeignKey("SwitchId");

                    b.Navigation("Switch");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Project", b =>
                {
                    b.Navigation("ProjectDeviceLimits");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Router", b =>
                {
                    b.Navigation("Switches");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Switch", b =>
                {
                    b.Navigation("Ports");
                });
#pragma warning restore 612, 618
        }
    }
}