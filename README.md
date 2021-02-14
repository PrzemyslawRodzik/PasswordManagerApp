
## General info
The PasswordManagerApp project is web application written in C# using .NET Core 3.1 with ASP.NET Core MVC. This app enable users to store sensitive data in a secure way. The main feature of this app is to check if stored password has previously appeared in data breach. 

The application can be developed, built and run cross-platform on Windows, and Linux distribution(*macOS not tested*).

## Table of contents
* [General info](#general-info)
* [Features](#features)
* [Security](#security)
* [Configuration](#configuration)
* [Setup](#setup)
    * [Requirements](#requirements)
    * [Build](#to-run-this-project)
* [Usage](#usage)


## Features

### Password pwned check



## Security

* After successfull authentication [server](https://github.com/PrzemyslawRodzik/PasswordManagerAppServer) generates tenable [JWT Token](https://jwt.io/) which gives user permission to access sensitive data
* [JWT Token](https://jwt.io/) payload is additionally encrypted
* Web vault check authenticity and integrity of JWT token
* Two-factor authentication
* Sensitive user data are encrypted with symmetric key( based on user's password hash) 
* User is notified in case of password breach or unauthorized login attempt
* User's passwords breach are checked in a secure way( More in [this](#password-pwned-check) paragraph)




## Configuration 
 To run this app first of all you''ll need to run *backend* [PasswordManagerApp server](https://github.com/PrzemyslawRodzik/PasswordManagerAppServer).
 

 
## Setup
### Requirements

- [.NET Core 3.1 SDK](https://www.microsoft.com/net/download/core)

#### To run this project:
* restore, build and run using commands:

```
cd PasswordManagerApp
dotnet run
```
* type localhost:5004 in the browser



## Usage

#### Next move is up to you

* Store any data you wish

<p align="center">
<img src="https://i.ibb.co/TL5s60r/storedatayouwish.gif" alt="storedatayouwish">
</p>


* Customize your profile
<p align="center">
<img src="https://i.ibb.co/RP38tCc/image.png" alt="profileWebvault">
</p>
* Check if your password was previously exposed in data breaches 

<p align="center">
<img src="https://i.ibb.co/F03NgvK/ezgif-com-gif-maker.gif" alt="ezgif-com-gif-maker">
</p>









