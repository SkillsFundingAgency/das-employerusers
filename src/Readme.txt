Simplified SFA Dev Localhost certificates.

The certificates generated with these command scripts are valid until the end of the century! No more certificate expiries interfering with dev progress.

First perform a one off install of the Cert Authority certificate on your machine.
Then perform a one off install of the localhost certificate on your machine.
Then use the thumbprint in the localhsot.cer.thumbprint.txt file to change all of your Cloud configuration or Web.Config files to use that thumbprint.

Delete any and all excess localhost certificates from your repositories as requried, or,
copy this readme file and the SFA.Dev.Local.Cer and localhost.pfx, and the localhost.cer.thumbprint.txt Files into your repo.

Install local certificate authority!

Double click SFA.Dev.Local.cer
Click Install
Choose Local Machine
Select Folder and Trusted Root Certificate Authorities

Install localhost certificate

Double click localhost.pfx
Click Install
Select local Machine
Select folder Personal

Or you may use the Certificates snap-in to import the files directly into those Certificate stores.


Finally:

To generated a new certificate (e.g. commonname ) based on this SFA.Dev.Local Authority, run the following command
Change 'commonname' as required.

CreateSSLCert commonname