# CloudCopy - A command-line tool to manage attachments in SAP Cloud for Customer

![Alt text](./asciinema/CloudCopy.upload.svg)

## Examples

Upload the file `details.pdf` to Contact 1000000:
```
CloudCopy upload details.pdf h.maulwurf@my000000.crm.ondemand.com:Contact:#1000000
```

List all attached files of Contact 1000000:
```
CloudCopy list h.maulwurf@my000000.crm.ondemand.com:Contact:#1000000
```

The following examples require a configuration file, containing the target host and the credentials to authenticate the user.

Upload all files of the current directory to ServiceRequest 2:
```
CloudCopy upload * ServiceRequest:#2
```

List all attached files of a ServiceRequest using the UUID of the Request:
```
CloudCopy list ServiceRequest:a563df71571140899b17ed8d08d8ff4b
```

Only list jpg files of ServiceRequest 2:
```
CloudCopy list -P "*.jpg" ServiceRequest:#4
```

## Installing
### Linux
To install (the current development pre-release) CloudCopy use, the following commands. To run it, use `./CloudCopy`.

```bash
$ mkdir CloudCopy && cd CloudCopy
$ curl -LsS https://github.com/AndreasFerdinand/CloudCopy/releases/download/v0.1-alpha/CloudCopy-linux-x64.tar.gz | tar xzv
```

### Windows
Just download and extract the file `CloudCopy-win-x64.zip` from the the release-page and use the executable `CloudCopy.exe`.

## Supported Entities
Currently the following entities are allowed as target:

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

## Configuration File
To prevent entering the credentials and the host each time, the data can be provided through a user specific configuration file.

### Path to the File
Debian / Ubuntu:
```
~/.local/share/CloudCopy/default.xml
```

Windows:
```
C:\Users\<user>\AppData\Local\CloudCopy\default.xml
```

### Format
It is not recommended to use the file, since the credentials are stored unencrypted in it. At least be sure, to prevent other users to access the file by setting appropriate file permissions.
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<CloudCopy>
	<Hostname>my000000.crm.ondemand.com</Hostname>
	<Username>username</Username>
	<Password>password</Password>
</CloudCopy>
```
