# GeneralPurposeApplication  
A collection of applications I develop.  

## Technologies Used  
- **.NET 8 SDK** (Version 8.0.101)  
- **TypeScript** (Version 5.2)  
- **NuGet Package Manager** (Version 6.8.0)  
- **Node.js** (Version 20.10.0)  
- **Angular** (Version 17.0.3)

## Dependencies

| Package            | Version     | Description                    |
|--------------------|-------------|--------------------------------|
| Serilog            | 2.12.0      | Logging framework              |
| EF Core            | 8.0.14      | ORM for .NET Core              |
| EF Core Sql Server | 8.0.14      | Database provider for EF Core  |
| Moq                | 13.0.1      | Mocking framework              |
| EPPlus             | 4.5.3.3     | Spreadsheet library            |
| EF Core InMemory   | 8.0.14      | Database Mock                  |

## Description  
- This repository contains various simple applications I have developed/been developing using .NET and Angular.
- All applications listed are those I think will be useful on my/other's everyday life.
- This will be part of my study, knowledge acquisition, and familiarization for Angular and TypeScript




## Applications
1. Kita - Variety Store Management System - Under Development
   - _My mother have a sari-sari store. If I make a sari-sari store management system, it might help her._
   - For Sari-Sari store owners to track item stocks, as well as track daily revenues.

2. Motorcycle Maintenance Tracker - PLANNED
   - _I can't remember when I last changed my engine oil. I should make an application for this!!!_
   - For Motorcycle owners to track timely maintenance for their motorcycles
   - Might include - Tracking of mileage of user for change engine oil

3. Password Manager & Vault - PLANNED
   - _I have so many accounts; there's my 10+ gmail account, my old crossfire account, lol accounts..... What's my password again?!!_
   - For users like me who have many accounts and forgetful enough to remember their 10+ accounts but also wants it to be secure.

4. Water me, Plantito - PLANNED
   - _I love watering my plant, I love taking care of my plants, I love my plants! I should create an app because I love plants!!!!!!!!!!_
   - Track plant watering intervals.
   - Note every plant details.
   - Contains plants dictionary. It might contain informations like "Beware, this plant is p%^&%^*^&."

5. Budget tracker??? - PLANNED
   - _I need an application to allocate my funds per day so I can budget my money more efficiently._
   - Allocate funds per day.
   - Calendar view for each added budget allocation.
   - Chartts for viewing budget allocation and how much spent.
  
6. What's the Fodo for today???? - PLANNED
   - _I want to track my food stocks on my refrigerator; I need an app for this.
   - List the ingredients when planming to from the list of food stocks
   - Automatically manage the food stocks based on the added ingredients for the planned meal
   - Ingredients for planned meal are not on the stock? Add a temporary ingredients for that
  
## Getting Started  
1. Clone the repository:  
   ```sh
   git clone https://github.com/your-username/GeneralPurposeApplication.git
   cd GeneralPurposeApplication
   ```
2. Install dependencies
   ```sh

   cd generalpurposeapplication.client
   ```
   ```sh
   npm install
   ```

3. Migrate (Or use provided sample database)
   ```
   dotnet ef migrations add "Initial" -o "Data/Migrations"
   ```
   ```
   dotnet ef database update
   ```
3. Run the application:
   - Start the backend (ASP.NET Core):
      ```sh
      dotnet run
      ```
   - Start the frontend (Angular)
      ```sh
      ng serve
      ```
    
