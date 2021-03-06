<#
New-SelfSignedCertificate -Subject localhost -CertStoreLocation cert:\LocalMachine\My -DnsName localhost
#>

#Requires -RunAsAdministrator

$localhostpwd = ConvertTo-SecureString -String password -Force -AsPlainText
Import-PfxCertificate -FilePath $PSScriptRoot\localhost.pfx -CertStoreLocation cert://LocalMachine/My -Password $localhostpwd -Exportable
Import-PfxCertificate -FilePath $PSScriptRoot\localhost.pfx -CertStoreLocation cert://LocalMachine/Root -Password $localhostpwd -Exportable

$idppwd = ConvertTo-SecureString -String idsrv3test -Force -AsPlainText
Import-PfxCertificate -FilePath $PSScriptRoot\DasIDPCert.pfx -CertStoreLocation cert://LocalMachine/My -Password $idppwd -Exportable