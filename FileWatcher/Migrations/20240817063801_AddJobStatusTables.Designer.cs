﻿// <auto-generated />
using System;
using FileWatcherApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FileWatcherApp.Migrations
{
    [DbContext(typeof(FileWatcherContext))]
    [Migration("20240817063801_AddJobStatusTables")]
    partial class AddJobStatusTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FileWatcherApp.Models.Box", b =>
                {
                    b.Property<int>("BoxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BoxId"));

                    b.Property<string>("BoxName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ScheduleTime")
                        .HasColumnType("datetime2");

                    b.HasKey("BoxId");

                    b.ToTable("Boxes");
                });

            modelBuilder.Entity("FileWatcherApp.Models.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("JobId"));

                    b.Property<int>("BoxId")
                        .HasColumnType("int");

                    b.Property<int>("CheckIntervalMinutes")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpectedArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("JobId");

                    b.HasIndex("BoxId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("FileWatcherApp.Models.JobStatus", b =>
                {
                    b.Property<int>("JobStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("JobStatusId"));

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StatusDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("StatusMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("JobStatusId");

                    b.HasIndex("JobId");

                    b.ToTable("JobStatuses");
                });

            modelBuilder.Entity("FileWatcherApp.Models.Job", b =>
                {
                    b.HasOne("FileWatcherApp.Models.Box", "Box")
                        .WithMany("Jobs")
                        .HasForeignKey("BoxId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Box");
                });

            modelBuilder.Entity("FileWatcherApp.Models.JobStatus", b =>
                {
                    b.HasOne("FileWatcherApp.Models.Job", "Job")
                        .WithMany("JobStatuses")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");
                });

            modelBuilder.Entity("FileWatcherApp.Models.Box", b =>
                {
                    b.Navigation("Jobs");
                });

            modelBuilder.Entity("FileWatcherApp.Models.Job", b =>
                {
                    b.Navigation("JobStatuses");
                });
#pragma warning restore 612, 618
        }
    }
}
