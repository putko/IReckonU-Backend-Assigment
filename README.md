# IReckonU-Backend-Assigment

Here is my implementation for IReckonU Backend Assigment implemented in Asp.net Core with targeting v2.2. It consists of 3 different logical component, namely API, Business layer and Data Access Layer. Dependencies between them is managed by the DI container that comes out of the box with Asp.Net Core web projects. 

# To Run

You need to download the source, restore the packages from nuget, have a .net core 2.2 sdk installed on your machine. than build the solution. To make it run, you need to have a Sql server instance and change connection string appsettings.json file under web project. 

## Web API

The requirements are to implement a web api to upload large csv files and process it. 

In asp.net core, for file uploads, either there are limitations or lack of online resources, so the best resource that I found was microsoft own documentation below:
https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.2

First thing you notice is, it is basically an MVC site with MVC controllers. There should be no problem with it, but I had to remove return types which I didn't yet. Also a cleanup is needed to get rid of Views and some of the controllers. 
Since we expect large files to be uploaded, it is considered to be wise to save it. It is under StreamingController with the name Upload. so, the url is https://localhost:44378/Api/Import by default. 

## Business

This is a classic business layer where you get your input from endpoints, and process it. In this case, we get the path to the uploaded file, with CSVHelper library we parse line by line, and create an ArtikelDTO instance for each record. Then using the automapper, we map them our domain objects and call the repository functions using unit of work pattern. there are no specific mappings for ArtikelDTO and ArtikelEntity because at that point they are basically the same. 
To seperate the concern of the csv parsing from business logic, I implemented a CVSParser class. It utilize the CSVHelper library and make an abstraction to our business function.

## Data Access Layer

The requirements are to store the same data into two different medium. I choose them to be MS Sql Server and json file. This component utilizes the repository and unit of work patterns using EF as ORM tool. 

EF out of the box, does not support to file system. To make it possible, I used a third party library called FileContextCore. The details can be seen here : https://github.com/morrisjdev/FileContextCore
That is one of the regrets of mine, because I figured out a bug. eventhough I explicitly register my File based context to DI transient way, FileContextCore does not support it. It means, when you upload data, then delete your json file and re-upload the same record with the same key, you get duplicate key error with InvalidOperationException. But inbetween, if you restart your process, it doesn't cause a problem. 

I didn't need a specific repository implementation for Artikels, since I used customization of GenericRepository<ArticleEntity>. 

To support multidatabases, I created two context very similar to each other, and register them to DI container with different options, one to use Sql Server and one to use FileContext. 

UnitofWork class is the place to access them. It takes two contexts, and create two different instances of GenericRepository<ArticleEntity> using them. 
The transaction is managed also here in save function. 

# Missing points

I spent a lot of time to find a workaround the Transient FileContext bug. I would have unit test and integration tests instead. It was low hanging fruit. 

I started to clean the UI code and remove some pages that comes from microsoft sample. I would have remove all unneccasary pages and controllers. Also exception handling is according to MVC standard. What I had in my mind was to provide a Web API. Now when an exception occurrs, you get MVC like error message then simple 500 status code. 


