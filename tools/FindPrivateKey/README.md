# FindPrivateKey.exe source

This tool is take from [Windows Communication Foundation (WCF) and Windows Workflow Foundation (WF) Samples for .NET Framework 4](https://www.microsoft.com/en-us/download/details.aspx?id=21459) and compiled using .NET 4.5.2 to be run as part of deployment to give correct permissions to signing certificate

## Usage
Full usage guidance can be found on [MSDN](https://msdn.microsoft.com/en-us/library/aa717039.aspx).

Example usage is:
```
FindPrivateKey.exe My LocalMachine -t "6b7acc520305bfdb4f7252daeb2177cc091faae1" -a
```

Where 6b7acc520305bfdb4f7252daeb2177cc091faae1 is the certificate thumbprint
