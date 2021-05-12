# CloudCopy - A command-line tool to manage attachments in SAP Cloud for Customer

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/cd9db6880b7b4ee5885ae7726b626c98)](https://www.codacy.com/manual/AndreasFerdinand/CloudCopy?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=AndreasFerdinand/CloudCopy&amp;utm_campaign=Badge_Grade)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Latest Release](https://img.shields.io/github/v/tag/AndreasFerdinand/CloudCopy?label=latest+release&sort=semver)](https://github.com/AndreasFerdinand/CloudCopy/releases/latest)
[![Platform Support](https://img.shields.io/badge/platform-win--64%20%7C%20linux--64-brightgreen)](https://github.com/AndreasFerdinand/CloudCopy/releases)

Tired of manually uploading a bunch of files to your service request in C4C? Or do you just need to download all files, attached to a Contact? Try CloudCopy, it’s your swiss army knife for managing attachments in SAP Cloud 4 Customer.

# Examples

Upload all pdf files from the current directory to Contact 1000000:
```
CloudCopy upload Contact:#1000000 *.pdf
```

List all attachments of Contact 1000000:
```
CloudCopy list Contact:#1000000
```

List all attachments of a ServiceRequest using the UUID of the it:
```
CloudCopy list ServiceRequest:a563df71571140899b17ed8d08d8ff4b
```

Only list jpg files of ServiceRequest 4:
```
CloudCopy list -p "*.jpg" ServiceRequest:#4
```

Download all pdf files of Contact 2000000:
```
CloudCopy download -p "*.pdf" Contact:#2000000
```

Download all files of ServiceRequest 333 using 6 parallel jobs to the current directory:
```
CloudCopy download -t 6 ServiceRequest:#333
```

To set a product image you have to provide the TypeCode 10011 using option `-c`:
```
CloudCopy upload -c 10011 Product:#10000483 product.png
```

> **ATTENTION**: The command line interface of CloudCopy has changed with release `0.5`!

## Installing
### Linux
To install CloudCopy use, the following commands. To run it, use `./CloudCopy`.

```bash
$ mkdir CloudCopy && cd CloudCopy
$ curl -LsS https://github.com/AndreasFerdinand/CloudCopy/releases/download/v0.4/CloudCopy-0.4-linux-x64.tar.gz | tar xzv
```

### Windows
Just download and extract the file `CloudCopy-0.4-win-x64.zip` from the the release-page and use the executable `CloudCopy.exe`.

## Supported Entities
Currently the following entities are supported:

* Appointment
* CodTimeReport
* CompetitorProduct
* Contact
* Contract
* CorporateAccount
* CustomerOrder
* CustomerOrderItem
* IndividualCustomer
* InstallationPoint
* JobDefinition
* Lead
* LeanInvoice
* MemoActivity
* Opportunity
* OpportunityItem
* Payment
* PhoneCall
* Product
* Promotion
* RegisteredProduct
* SalesQuote
* SalesQuoteItem
* ServiceAgent
* ServiceRequest
* SocialMediaActivity
* Tasks

## Build from Source
### Linux
If you run Linux, you can use the Makefile to build the binaries. Since the source is based on .NET Core you have to install the [.NET Core SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux). To build the binaries of the library and the console program for windows and linux use the following commands. To build platform specific binaries use the targets provided in the Makefile.

```bash
make
```

### Windows
To build the binaries on windows, the [.NET Core SDK](https://docs.microsoft.com/en-us/dotnet/core/install/windows) is needed too. Run the following command in the `src` subdirectory of the project (library or console program) you want to build.:

```bat
dotnet build
```

## Configuration File
To prevent entering the credentials and the host each time, the data can be provided through a user specific configuration file.

Properties not maintained in the file will be requested by CloudCopy during invocation. The command line options `--Hostname` and `--Username` override the data maintained.

You can either create the file manually or using the the `configure` command of CloudCopy.

### Create it manually
The configuration file allows you to maintain the following properties:

* `Hostname` only, or
* `Hostname` and `Username`, or
* `Hostname` and `Username` and `Password` (**not recommended**).

Example file:
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<CloudCopy>
	<Hostname>my000000.crm.ondemand.com</Hostname>
	<Username>username</Username>
	<Password>password</Password>
</CloudCopy>
```

Depending on your operating system the file must by created at the following path:

Windows:
```
C:\Users\<user>\AppData\Local\CloudCopy\default.xml
```

Debian / Ubuntu:
```
~/.local/share/CloudCopy/default.xml
```

### Create it using CloudCOpy

If you are migrating from CloudCopy `0.4` to `0.5` (Windows only) and you are already using a configuration file with a clear text password maintained, you can easily encrypt the password using the follwoing command:

```bat
CloudCopy configure
```

To create a new configuration file use the following command and replace the Tokens `<hostname>` and `<username>`.

```
CloudCopy configure -H <hostname> -U <username> -M
```

If you use the option `-M` CloudCopy promptes you to type the password. You cannot pass it directly using a parameter!

**ATTENTION**: Passwords are only encrypted on Windows machines!

### Security considerations
On Linux systems maintain the recommended file permissions to prevent other users from reading your password
```bash
chmod 600 ~/.local/share/CloudCopy/default.xml
```

## C4C User Configuration
CloudCopy uses the OData service provided by C4C. To access it, an application user or a technical (aka integration) user is needed.

### Creating a technical user
1) Create a `Communication Systems` (Administrator \ General Settings \ Integration \ Communication Systems)
2) Define a `Communication Arrangements` (Administrator \ General Settings \ Integration \ Communication Arrangements)
   - use communication scenario `OData Services for Business Objects`
   - use `Basic Authentication` as authentication method
   - specify a password (username is preset by C4C)
   - assign the required OData services
  

## LibCloudCopy
With release `0.5` CloudCopy is split in a reusable library and the command line client.