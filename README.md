AspnetIdentitySample
====================
This sample demonstrate how you can easily add profile data for a user.
This is based on the ASP.NET MVC template that shipped with ASP.NET and Web Tools 2013 Preview Refresh(http://go.microsoft.com/fwlink/?LinkId=309552).
Once you have this Preview Refresh installed you can do the same for ASP.NET Web Forms and SPA applications.

Following are the steps to Run this project
- Open the solution
- Build and run
- Regsiter a user
---- Notice that the user registration field only has user name and password
- Let's ask for a birthdate option from the user while registering an account.
- Goto Nuget Package Manager console and run "Enable-Migrations"
- Goto Models\AppModel.cs and uncomment BirthDate property in the MyUser class
- Goto Models\AccountViewModels.cs and uncomment BirthDate property in RegisterViewModel
- Goto AccountController and in Register Action and have the following code
          var user = new MyUser() { UserName = model.UserName,BirthDate=model.BirthDate };
          //var user = new MyUser() { UserName = model.UserName };
- Goto Views\Account\Register.cshtml and uncomment the html markup to add a BirthDate column
- Goto Nuget Package Manager console and run "Add-Migration BirthDate"
- Goto Nuget Package Manager console and run "Update-Database"
- Run the application
- When you register a user then you can enter BirthDate as well 