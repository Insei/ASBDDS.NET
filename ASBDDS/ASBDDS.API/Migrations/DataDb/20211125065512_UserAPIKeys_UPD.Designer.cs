﻿// <auto-generated />
using System;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASBDDS.API.Migrations.DataDb
{
    [DbContext(typeof(DataDbContext))]
    [Migration("20211125065512_UserAPIKeys_UPD")]
    partial class UserAPIKeys_UPD
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("Disabled")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("EditorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserToken", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.DbConsole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Disabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("SerialSettingsId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("SerialSettingsId");

                    b.ToTable("Consoles");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ConsoleId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MacAddress")
                        .HasColumnType("text");

                    b.Property<int>("MachineState")
                        .HasColumnType("integer");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("PowerControlType")
                        .HasColumnType("integer");

                    b.Property<int>("PowerState")
                        .HasColumnType("integer");

                    b.Property<string>("Serial")
                        .HasColumnType("text");

                    b.Property<Guid?>("SwitchPortId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ConsoleId");

                    b.HasIndex("SwitchPortId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.DeviceRent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Closed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DeviceId")
                        .HasColumnType("uuid");

                    b.Property<string>("IpxeUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("DeviceId");

                    b.HasIndex("ProjectId");

                    b.ToTable("DeviceRents");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.DHCPLeaseDb", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("MacAddress")
                        .HasColumnType("text");

                    b.Property<bool>("Static")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("DHCPLeases");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AllowCustomBootloaders")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<int>("DefaultVlan")
                        .HasColumnType("integer");

                    b.Property<bool>("Disabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

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
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Manufacturer")
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .HasColumnType("text");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectDeviceLimits");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ProjectUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectUsers");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Router", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Routers");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.SerialPortSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("BaudRate")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DataBits")
                        .HasColumnType("integer");

                    b.Property<int>("Handshake")
                        .HasColumnType("integer");

                    b.Property<int>("Parity")
                        .HasColumnType("integer");

                    b.Property<string>("PortName")
                        .HasColumnType("text");

                    b.Property<int>("StopBits")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("SerialPortsSettings");
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
                        .HasColumnType("timestamp with time zone");

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
                        .HasColumnType("timestamp with time zone");

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

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.UserApiKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid>("Key")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("UserApiKeys");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationRoleClaim", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserClaim", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserLogin", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserRole", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ApplicationUserToken", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.DbConsole", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.SerialPortSettings", "SerialSettings")
                        .WithMany()
                        .HasForeignKey("SerialSettingsId");

                    b.Navigation("SerialSettings");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Device", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.DbConsole", "Console")
                        .WithMany()
                        .HasForeignKey("ConsoleId");

                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.SwitchPort", "SwitchPort")
                        .WithMany()
                        .HasForeignKey("SwitchPortId");

                    b.Navigation("Console");

                    b.Navigation("SwitchPort");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.DeviceRent", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId");

                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.Navigation("Creator");

                    b.Navigation("Device");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.Project", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ProjectDeviceLimit", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Project", "Project")
                        .WithMany("ProjectDeviceLimits")
                        .HasForeignKey("ProjectId");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.ProjectUser", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Project");

                    b.Navigation("User");
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

            modelBuilder.Entity("ASBDDS.Shared.Models.Database.DataDb.UserApiKey", b =>
                {
                    b.HasOne("ASBDDS.Shared.Models.Database.DataDb.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.Navigation("Creator");
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
