<#
New-SelfSignedCertificate -Subject localhost -CertStoreLocation cert:\LocalMachine\My -DnsName localhost
#>

#Requires -RunAsAdministrator

$localhostpwd = ConvertTo-SecureString -String password -Force -AsPlainText
Import-PfxCertificate -FilePath localhost.pfx -CertStoreLocation cert://LocalMachine/My -Password $localhostpwd -Exportable
Import-PfxCertificate -FilePath SFA.Dev.Local.cer -CertStoreLocation cert://LocalMachine/Root -Password $localhostpwd -Exportable

$idppwd = ConvertTo-SecureString -String idsrv3test -Force -AsPlainText
Import-PfxCertificate -FilePath DasIDPCert.pfx -CertStoreLocation cert://LocalMachine/My -Password $idppwd -Exportable