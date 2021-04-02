# BlazorApp
**Production-ready Blazor Application (Server)**
 
Purpose of this app to create a production-ready base template for a Web App.
 
The high-level architecture will be as below. 

![architecture](Code/BlazorAppDemoUI/wwwroot/img/BlazorAppDemo-architectures.jpg)

In This Application we used below compnents


1. Azure App Service Plan
2. Azure Web App / App Service
  -    Application UI layer
  -    Application API layer
3. App Service logs
4. Azure Monitor Log Analytics
5. Azure Alerts
6. Azure SQL Database
7. Azure Application Insights
8. Azure SendGrid 
9. Visual Studio 2019
10. .Net Core 5
11. Blazor Framework
12. Entity Framework Core


# We divied this application in 4 parts.

1. Database
   1. Database first approch
   2. Azure SQL Database
2. API layer
   1. Asp. net Core Web API
   2. JWT Token Authentication
   3. Entity Framework Core 
3. Web UI layer
   1. Blazor Framework
   2. Multilingual
   3. Bootstrap responsive UI 
4. Azure
   1. App Service Plan
   2. Azure Web App / App Service
   3. App Service logs
   4. Azure Monitor Log Analytics
   5. Azure Alerts
   6. Azure Application Insights
   7. Azure SendGrid
 
 #  Lets Build the application 
**Part 1**
 Since we are using db first approch, we will create database first.
 we need to create below is the database schema in Azure SQL DB, for now we will use basic Configuretion for Azure SQL DB. based on your application requirement we can go for higher ones

 ![Azure DB Setting](SupportingFiles/Create%20Azure%20DB.png)

 **Note** *We need to add our development system IP in SQL server Firewall to allow the connection, for demo purpose I am allowing all the Ips but it’s not a good idea Please add only required Ips only.*
   
![Azure DB Setting](SupportingFiles/AzureDBFirewall.png)

**DB Schema**

 ![DB Schema](SupportingFiles/DBSchema.png)

 You can refer DB script [here](Code/BlazorAppDemoUI/wwwroot/img/BlazorAppDemo-architectures.jpg)

 **Part 2**
 To connect with DB and expose as API we will use .Net Web API template

 ![API Project](SupportingFiles/BlazorDemoAPI%201.png) 

We will add three API controllers in API project for all three tables in DB

![Contorller Type](SupportingFiles/API%20Controller.png)

![Coltorllers](SupportingFiles/API%203%20Controller.png)

You can refer the code of all three contorllers [here](Code/BlazorDemoAPI/Contorllers/) 