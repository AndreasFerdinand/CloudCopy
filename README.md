# CloudCopy - A command-line tool to manage attachments in SAP Cloud for Customer

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/cd9db6880b7b4ee5885ae7726b626c98)](https://www.codacy.com/manual/AndreasFerdinand/CloudCopy?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=AndreasFerdinand/CloudCopy&amp;utm_campaign=Badge_Grade)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Latest Release](https://img.shields.io/github/v/tag/AndreasFerdinand/CloudCopy?label=latest+release&sort=semver)](https://github.com/AndreasFerdinand/CloudCopy/releases/latest)
[![Platform Support](https://img.shields.io/badge/platform-win--64%20%7C%20linux--64-brightgreen)](https://github.com/AndreasFerdinand/CloudCopy/releases)

Tired of manually uploading a bunch of files to your service request in C4C? Or do you just need to download all files, attached to a Contact? Try CloudCopy, it’s your swiss army knife for managing attachments in SAP Cloud 4 Customer. Watch the screencast, to see, how CloudCopy can help you to improve your service process. 

![Alt text](./asciinema/CloudCopy.upload.svg)

## Some more Examples

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

Download all pdf files of Contact 2000000:
```
CloudCopy download -P "*.pdf" Contact:#2000000
```

Download all files of ServiceRequest 333 using 6 parallel jobs:
```
CloudCopy download -T 6 ServiceRequest:#333
```

To set a product image you have to provide the TypeCode 10011 using option `-C`:
```
CloudCopy upload -C 10011 product.png Product:#10000483
```

## Installing
### Linux
To install CloudCopy (latest development release) use, the following commands. To run it, use `./CloudCopy`.

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
