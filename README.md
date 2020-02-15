# CloudCopy - Upload Files to SAP C4C from Commandline


> ATTENTION: Software is in an early development stage!

Upload a the file `details.pdf` to Contact 1000000:
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
