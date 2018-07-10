#Requires -RunAsAdministrator

$certpwd = ConvertTo-SecureString -String password -Force -AsPlainText

Import-PfxCertificate -FilePath $PSScriptRoot\localhost.pfx -CertStoreLocation cert://LocalMachine/My -Password $certpwd -Exportable
Import-PfxCertificate -FilePath $PSScriptRoot\SFA.Dev.Local.pfx -CertStoreLocation cert://LocalMachine/Root -Password $certpwd -Exportable
