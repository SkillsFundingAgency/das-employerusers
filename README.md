## Development Machine Setup
You will require the Azure SDK v2.8 upwards to run the solution.

As the solution also requires SSL, you will need to run DevInstall.ps1 to install the test certificate in your local stores. This must be ran as administrator

### Running local

In azure local storage a table called Configuration needs creating

A new column Data needs creating.


The following should be entered in data

````

{"IdentityServer":{"EmployerPortalUrl":"http://localhost","ApplicationBaseUrl":"https://localhost:44334/","UnsecureApplicationBaseUrl":"http://localhost:59507/","CertificateStore":"LocalMachine","CertificateThumbprint":"1124CAEF67E07633DD23A75F2E76F8732EE0F6DC"},"DataStorage":{"DocumentDbUri":"","DocumentDbAccessToken":""},"Account":{"ActivePasswordProfileId":"b1fae38b-2325-4aa9-b0c3-3a31ef367210","AllowedFailedLoginAttempts":"3","UnlockCodeLength":8}}

````

The row key is SFA.DAS.EmployerUsers.Web_1.0
Parition key is LOCAL

OR

You can set the following config value

````
<add key="LocalConfig" value="true"/>
````

In the web.config of the SFA.DAS.EmployerUsers.Web project

