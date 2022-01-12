# Employer Users ![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/72/badge)

### Development Machine Setup

Instructions for setting up a development machine to run/debug the solution. 

## Pre-requisites

Install the Azure SDK v2.8 (or higher).

Run the `DevInstall.ps1` script (run as administrator) to install the test certificate in your local stores. Certificates are required for SSL. 

A new localhost certificate was generated as of 23-Nov-2018. 

You should re-run DevInstall.ps1  (As Administrator) once per local development environment. After this date.

When a new certificate is needed please run GenCert.ps1 (As Administrator) Take a note of the Thumprint displayed, and paste those into all of the WebSslCert entries Thumbprint values in App Service local config files and reset the above certificate generation date. 

Please also warn other team developers through normal channels that they should re-run DevInstall.ps1.

Note: you may need to set your execution policy to unrestricted in PowerShell - if so, run the following in an administrator PowerShell window:

	Set-ExecutionPolicy Unrestricted

... and then run the `DevInstall.ps1` script again.

## Running locally

Execute RunBuild.bat to build the solution.

You can run the solution from Visual Studio (ensure it's running in administrator mode) by setting the `SFA.DAS.EmployerUsers` cloud project as start-up.

The solution needs configuration data at runtime which can be read from Azure storage (either remote or using the local emulator) or from the file system. 

### Configuration data (emulator)

In azure local storage:

- create a table named Configuration
- add a new column named Data
- add a row:
	- set partition key = "LOCAL"
	- set row key = "SFA.DAS.EmployerUsers.Web_1.0"
	- set data = 

	{
		"IdentityServer": {
			"EmployerPortalUrl":"http://localhost",
			"ApplicationBaseUrl":"https://localhost:44334/",
			"UnsecureApplicationBaseUrl":"http://localhost:59507/",
			"CertificateStore":"LocalMachine", 
			"CertificateThumbprint":"1124CAEF67E07633DD23A75F2E76F8732EE0F6DC"
		},
		"DataStorage": {
			"DocumentDbUri":"",
			"DocumentDbAccessToken":""
		},
		"Account": {
			"ActivePasswordProfileId":"b1fae38b-2325-4aa9-b0c3-3a31ef367210",
			"AllowedFailedLoginAttempts":"3",
			"UnlockCodeLength":8
		}
	}

### Configuration data (file system)

As an alternative to reading configuration from Azure storage, the local file system can be used instead. To do this, set the following `appSettings` value in the `SFA.DAS.EmployerUsers.Web` web.config file as follows:

	<add key="LocalConfig" value="true"/>

In this case, the configuration will be read from a local .json file located in the App_Data folder (this file is in the repo and contains the same data as above).
