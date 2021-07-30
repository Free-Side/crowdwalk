# Walkable Map App

A browser based mapping app for discovering walkable streets.

## Building & Running

This application is build using .Net 5.0 and Angular 10 and is designed to be runnable on [Replit](https://www.replit.com/).
 
Original Source: [@shrek231/DOTNET-5](https://replit.com/@shrek231/DOTNET-5)

The `dotnet` script in the root of this repository will automatically install a local copy of the .Net SDK that will work on Replit.

Clicking the Run command in Replit checks if latest sdk is installed (if not it is installed) and attempts to run current project. To run dotnet from the command prompt use the supplied script as follows.

To verify the sdk version installed
```
$ ./dotnet --list-sdks
5.0.103 [/home/runner/dotnet/sdk]
```

If you are building & running this application locally with an existing installation of the .Net SDK, you can simply use `dotnet` instead of `./dotnet`.

To run the application run:

First install dependencies for the SPA:

```
cd walkable-mpa-app/ClientApp
npm install
```

The launch the ASP.Net App (which serves both the Backend API and the SPA):

```
./dotnet run -p walkable-map-app --environment Development
```