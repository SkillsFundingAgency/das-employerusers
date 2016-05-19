$localhostpwd = ConvertTo-SecureString -String password -Force -AsPlainText

Import-PfxCertificate -FilePath localhost.pfx -CertStoreLocation cert://LocalMachine/My -Password $localhostpwd -Exportable
Import-PfxCertificate -FilePath localhost.pfx -CertStoreLocation cert://LocalMachine/Root -Password $localhostpwd -Exportable