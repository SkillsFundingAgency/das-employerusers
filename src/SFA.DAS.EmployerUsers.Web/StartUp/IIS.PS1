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
$Azure="Azure"


if(Get-Module -ListAvailable | Where-Object{ $_.Name -eq $Azure }) 
{  
[Reflection.Assembly]::LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime")

$ConfigurationStorageConnectionString = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("CertConfigurationStorageConnectionString")
$EnvironmentName = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("EnvironmentName")
$EnvironmentNameLower= $EnvironmentName.ToLower()
#Query
$Ctx = New-AzureStorageContext -ConnectionString $ConfigurationStorageConnectionString

$BlobName = "DasIDPCert.pfx"
$ContainerName = "$EnvironmentNameLower"
$localTargetDirectory = "C:\Cert"

New-Item $localTargetDirectory -type directory -ErrorAction SilentlyContinue -WarningAction SilentlyContinue
 
Get-AzureStorageBlobContent -Blob $BlobName -Container $ContainerName -Destination $localTargetDirectory -Context $ctx -Force 

$mypwd = ConvertTo-SecureString -String "idsrv3test" -Force –AsPlainText

Import-PfxCertificate –FilePath C:\Cert\DasIDPCert.pfx cert:\localMachine\my -Password $mypwd


}  
else  
{  
 "StartUp\Powershell.msi" | Install-MSIFile
}

