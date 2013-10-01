AspnetIdentitySample
====================
This sample demonstrate how you can easily add profile data for a user.
This is based on the ASP.NET MVC template that shipped with Visual Studio 2013 RC (http://www.asp.net/visual-studio/overview/2013/release-notes-(release-candidate)).

Following are the features in this project
- Initialize ASP.NET in App_Start by creating an Admin user and add the user to Admin Role
- Basic Role Management which is restricted to Users in Admin Roles Only. 
Admin can create, update, delete (remove all users from this role) roles and view the details of the role (Users in this role).
You can look at the RolesAdmin Controller
- Basic User Management  which is restricted to Users in Admin Roles Only. 
Admin can create user (add a user to role as well), edit user details(such as profile data and modify the roles for the user).
You can look at the UsersAdmin Controller


Following are the steps to customize profile. Note once you do this then Basic User and Role Management will not work
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