# Everly-MemberDirectory
This is the .Net code exercise for creating a Member Directory application, with the ability to search for potential friends that have a shared interest as the member.
It's implemented as an <b>API</b>.

<b><ins>Setup requirements</ins></b>

Database: this was done using Microsoft SQL Server, but would also work with LocalDb (installed with Visual Studio). The database should be version 2017 or newer. 
Run the database script `CreateDbSchema.sql` in the Database folder to create the database and schema (tables, etc.).

Visual Studio: this was done using 2019, but might work with 2017. The projects in the solutions use some Nuget packages, but Visual Studio will restore those for you.

<b><ins>Architecture</ins></b>

.Net version 5.

Database access is done using SQL and the open-source _Dapper_ library, which is a micro ORM, and uses ADO.Net underneath. _Dapper_ (<b>D</b>ata access M<b>apper</b>) maps database records to your model classes. Microsoft Entity Framework is good and I use it sometimes, but mostly I perfer the combination of SQL and a lightweight ORM.

I used a basic repository pattern for database access. I put the repositories and data models in a separate project, called _MemberDirectory.Data_.

The _EverlyHealth.Core_ project contains common things for all Everly Health .Net projects, for now this only has a couple of things.

The _MemberDirectory.App.Api_ project is the main API.  The _Startup.cs_ file in that project is what the .Net runtime calls to setup the dependency-injection container. I put comments in that file explaining things.

I added [Swagger (openAPI)](https://swagger.io/docs/specification/2-0/what-is-swagger). There's a built-in endpoint you can access with a browser to see documentation of the app's API, and you can test the endpoints.

![](https://github.com/Dean-NC/Everly-MemberDirectory/raw/main/Swagger.png)

![](https://raw.githubusercontent.com/Dean-NC/Everly-MemberDirectory/main/Swagger2.png)
![](https://raw.githubusercontent.com/Dean-NC/Everly-MemberDirectory/main/Swagger3.png)
