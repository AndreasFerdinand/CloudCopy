# CloudCopy - A command-line tool to manage attachments in SAP Cloud for Customer

> ATTENTION: Software is in an early development stage!

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

List all attached files of a ServiceRequest using the UUID of the Request:
```
CloudCopy list ServiceRequest:a563df71571140899b17ed8d08d8ff4b
```

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
It is not recommended to use the file, since the credentials are stored unencrypted in the file. At least be sure, to prevent other users to access the file by setting appropriate file permissions.
```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<CloudCopy>
	<Hostname>my000000.crm.ondemand.com</Hostname>
	<Username>username</Username>
	<Password>password</Password>
</CloudCopy>
```
