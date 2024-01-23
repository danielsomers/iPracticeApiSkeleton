﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using iPractice.DataAccess;

#nullable disable

namespace iPractice.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("ClientPsychologist", b =>
                {
                    b.Property<long>("ClientsId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PsychologistsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ClientsId", "PsychologistsId");

                    b.HasIndex("PsychologistsId");

                    b.ToTable("ClientPsychologist");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Availability", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<long>("PsychologistId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PsychologistId");

                    b.ToTable("Availabilities");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Client", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Psychologist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Psychologists");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.TimeSlot", b =>
                {
                    b.Property<long>("TimeSlotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("AvailabilityId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("ClientId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsBooked")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("TimeSlotId");

                    b.HasIndex("AvailabilityId");

                    b.HasIndex("ClientId");

                    b.ToTable("TimeSlots");
                });

            modelBuilder.Entity("ClientPsychologist", b =>
                {
                    b.HasOne("iPractice.DataAccess.Models.Client", null)
                        .WithMany()
                        .HasForeignKey("ClientsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("iPractice.DataAccess.Models.Psychologist", null)
                        .WithMany()
                        .HasForeignKey("PsychologistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Availability", b =>
                {
                    b.HasOne("iPractice.DataAccess.Models.Psychologist", null)
                        .WithMany("Availabilities")
                        .HasForeignKey("PsychologistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.TimeSlot", b =>
                {
                    b.HasOne("iPractice.DataAccess.Models.Availability", "Availability")
                        .WithMany("TimeSlots")
                        .HasForeignKey("AvailabilityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("iPractice.DataAccess.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId");

                    b.Navigation("Availability");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Availability", b =>
                {
                    b.Navigation("TimeSlots");
                });

            modelBuilder.Entity("iPractice.DataAccess.Models.Psychologist", b =>
                {
                    b.Navigation("Availabilities");
                });
#pragma warning restore 612, 618
        }
    }
}
