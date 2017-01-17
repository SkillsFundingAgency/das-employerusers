function Install-MSIFile {

[CmdletBinding()]
 Param(
  [parameter(mandatory=$true,ValueFromPipeline=$true,ValueFromPipelinebyPropertyName=$true)]
        [ValidateNotNullorEmpty()]
        [string]$msiFile,

        [parameter()]
        [ValidateNotNullorEmpty()]
        [string]$targetDir
 )
if (!(Test-Path $msiFile)){
    throw "Path to the MSI File $($msiFile) is invalid. Please supply a valid MSI file"
}
$arguments = @(
    "/i"
    "`"$msiFile`""
    "/qn"
)
if ($targetDir){
    if (!(Test-Path $targetDir)){
        throw "Path to the Installation Directory $($targetDir) is invalid. Please supply a valid installation directory"
    }
    $arguments += "INSTALLDIR=`"$targetDir`""
}
Write-Verbose "Installing $msiFile....."
$process = Start-Process -FilePath msiexec.exe -ArgumentList $arguments -Wait -PassThru
if ($process.ExitCode -eq 0){
    Write-Verbose "$msiFile has been successfully installed"
}
else {
    Write-Verbose "installer exit code  $($process.ExitCode) for file  $($msifile)"
}
}
#Function to get the location of the cert#
Function GetLocation {
$cmdOutput = $($Command = "E:\sitesroot\0\StartUp\FindPrivateKey.exe"
$Parms = "My LocalMachine -t ""$thumbprint"" -a "
$Prms = $Parms.Split(" ")
& "$Command" $Prms
return $cmdOutput
)
}

#Set Permissions on the cert#
Function Permissions {
$location=GetLocation
$acl = Get-Acl "$location"
$permission = "USERS","FullControl","Allow"
$accessRule = new-object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.SetAccessRule($accessRule)
$acl | Set-Acl "$location"
}

#Redis Logging Function
Function RedisLogging {

$ne = New-Object NLog.Config.XmlLoggingConfiguration("web.config")
([NLog.LogManager]::Configuration) = $ne
$logger = [NLog.LogManager]::GetCurrentClassLogger()
}
#Import DLL to enable logging
Function ImportDll
{
Import-Module -Name ".\bin\SFA.DAS.NLog.Targets.Redis.dll"
Import-Module -Name ".\bin\StackExchange.Redis.dll"
Import-Module -Name ".\bin\NLog.dll"
Import-Module -Name ".\bin\Newtonsoft.Json.dll"
Import-Module -Name ".\bin\Microsoft.WindowsAzure.Configuration.dll"
}


$Azure="Azure"

if(Get-Module -ListAvailable | Where-Object{ $_.Name -eq $Azure }) 
{  

ImportDll
#Static Variables#

$checkcert=(Get-ChildItem cert:\LocalMachine\My | Where thumbprint -eq $thumbprint).Thumbprint
$Ctx = New-AzureStorageContext -ConnectionString "$ConfigurationStorageConnectionString"
$BlobName = "$environmentname/DasIDPCert.pfx"
$ContainerName = "certs"
$localTargetDirectory = "C:\Cert"

[Reflection.Assembly]::LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime")
$ConfigurationStorageConnectionString = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("ConfigurationStorageConnectionString")
$EnvironmentName = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("EnvironmentName")
$Thumbprint = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("TokenCertificateThumbprint")
#$thumbprint="6B7ACC520305BFDB4F7252DAEB2177CC091FAAE1"
             

if ($checkcert)
{
Write-Warning -Message "Certificate already installed"
#Apply Permissions Just Incase#
permissions 
$logger.Info("Certificate already Installed with thumbprint $Thumbprint")
}
else{
#If Certificate doesnt exist do the below#

#Create Folder#
New-Item $localTargetDirectory -type directory -ErrorAction SilentlyContinue -WarningAction SilentlyContinue

#Download Certifacte#

$error.clear()
Try {Get-AzureStorageBlobContent -Blob $BlobName -Container $ContainerName -Destination $localTargetDirectory -Context $ctx -Force }
catch {
"There has been an error downloading the file"
$logger.Error("There has been an error downloading the file")
break
}

#Gets PFX Password from table storage#
$TableName = "Configuration"
$table = Get-AzureStorageTable –Name $TableName -Context $Ctx -ErrorAction SilentlyContinue
$query = New-Object Microsoft.WindowsAzure.Storage.Table.TableQuery 

if (!$table)
{
write-host "Configuration table does not exist"
$logger.Error("Configuration table does not exist")
break
}

#Define columns to select.
$list = New-Object System.Collections.Generic.List[string]
$list.Add("PartitionKey")
$list.Add("RowKey")
$list.Add("Data")

$query.FilterString =  "RowKey eq 'SFA.DAS.EmployerUser.CertPassword' and PartitionKey eq '$EnvironmentName' "
$query.SelectColumns = $list

$entities = $table.CloudTable.ExecuteQuery($query)

$CertPassword=((($entities.Properties).Values).PropertyAsObject)

if (!$CertPassword)
{
write-host "Password not available"
$logger.Error("Password not available in the table storage")
break
}

$mypwd = ConvertTo-SecureString -String "$CertPassword" -Force –AsPlainText

Import-PfxCertificate –FilePath C:\Cert\$environmentname\DasIDPCert.pfx cert:\localMachine\my -Password $mypwd

#Setting Permission on the Cert for ReadAccess#

Permissions

#Removing the Certificate download location#

Remove-Item $localTargetDirectory -Force -Recurse

}

}
else  
{  
 "StartUp\Powershell.msi" | Install-MSIFile
}

