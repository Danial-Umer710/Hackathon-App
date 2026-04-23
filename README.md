# Hackathon Results Management System

This project is a console-based C# application developed as part of the coursework at Óbuda University. It is designed to manage hackathon project results by importing XML data, validating records, and persisting them to a database using Entity Framework Core. The system demonstrates advanced C# features including LINQ, asynchronous programming, delegates, and events.

## 🚀 Features

* **Data Processing:** Imports hackathon results from XML, validates data, and performs upsert operations (Insert/Update) into an SQL database.
* **Querying:** Implements 15 diverse LINQ queries (both method and query syntax) covering filtering, sorting, grouping, and statistical ranking.
* **Export:** Automatically exports query results to formatted JSON files.
* **Event-Driven:** Uses custom delegates and events to provide real-time summaries (inserted/updated/skipped counts and duration) upon completion of data imports.
* **Architecture:** Follows clean architecture principles with a clear separation between the Console UI and the Data access layer.

## 🛠 Technologies Used

* **Language:** C# 12 / .NET 9.0
* **ORM:** Entity Framework Core (Code-First)
* **Database:** SQL Server / LocalDB
* **Data Formats:** XML (LINQ to XML), JSON (System.Text.Json)
* **Features:** Asynchronous I/O, LINQ, Delegates/Events, ConfigurationBuilder

## 🏗 Solution Structure

* **HackathonApp**: The console application responsible for user interaction, configuration loading, event handling, and executing queries/exports.
* **HackathonData**: The class library containing the EF Core DbContext, entity definitions, and data services.

## 📋 Data Import Process

The application automatically validates records from Data/HackathonResults.xml.
* Valid records: Upserted into the database.
* Invalid records: Skipped and included in the final import summary.
* Reporting: An event is triggered post-import, displaying detailed metrics regarding the process duration and record status.

## ⚙️ Configuration

The system uses appsettings.json to manage:
* Database connection strings.
* Relative file paths for data inputs and JSON outputs.

## 🚀 Getting Started

1. Clone the repository:
   `git clone https://github.com/Danial-Umer710/Hackathon-App.git`
2. Restore Dependencies: Ensure you have .NET 9.0 SDK installed and run `dotnet restore`.
3. Database Setup: Apply EF Core migrations to set up your local SQL database:
   `dotnet ef database update`
4. Run: Execute the HackathonApp project via Visual Studio or `dotnet run`.

## 📄 License
Developed for Óbuda University coursework (2025/2026).
