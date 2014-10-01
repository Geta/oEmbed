
#	The Update-EPiDataBase and Update-EPiConfig uses by default EPiServerDB connection string name and $package\tools\upgrade path 
#	to find sql files and transformations file but if it needed to customize the connectionStringname 
#	then a setting.config file can be created (e.g. "setting.config" file under the $package\tools\settings.config).
#	The format of the settings file is like 
#		<settings>
#			<connectionStringName/>
#		</settings>
$setting = "settings.config"
$exportRootPackageName = "EPiUpgradePackage"
$frameworkPackageId = "EPiServer.Framework"
$tools_id = "tools"
$upgrade_id = "tools\upgrade"


#	This CommandLet upgrade DB 
#	It collects all scripts by default under $packagepath\tools\upgrade
#   By default uses EPiServerDB connection string name and if the connection string name is different from default (EPiServerDB)
#	then it needs a settings.config (See setting for more information)
Function Update-EPiDatabase
{
	Upgrade "sql"
}

#	This CommandLet upgrade web config 
#	It collects all transformation config by default under $packagepath\tools\upgrade
Function Update-EPiConfig
{
	Upgrade "config"
}

#	This command can be used in the visual studio environment
#	Try to find all packages that related to the project that needs to be upgraded  
#   Create export package that can be used to upgrade to the site
Function Export-EPiPackage
{
	$params = Getparams
	$packages = $params["packages"]
	$sitePath = $params["sitePath"]
	ExportPackages $params["sitePath"] (join-Path $sitePath "..\packages") $packages
}

#	This command can be used in the visual studio environment
#	Try to find all packages that related to the project that has upgrade  
#	Find out setting for each package
#   Call epideploy with -a config for each package
Function Upgrade ($action)
{
	$params = Getparams
	$packages = $params["packages"]
	$sitePath = $params["sitePath"]
	Upgrade-Packages $action $params["sitePath"] (join-Path $sitePath "..\packages") $packages
}


#	This command can be used in the visual studio environment
#	Export all packages that have upgrade folder under tools path and
#	Create a bat (Run.bat) that can be used to call on site
Function ExportPackages($sitePath, $packagesPath, $packages)
{
	CreateRootPackage  $exportRootPackageName
	$frameworkPath = join-Path  $exportRootPackageName (GetEpiFrameworkFromPackages($packages))
	$epiDeployPath = join-path $frameworkPath "tools\epideploy.exe"
	$batFile  = AddUsage 
	$packages |foreach-object -process {
			$packageName = $_.id + "." + $_.version
			$packagePath = join-path $packagesPath $packageName
			$packageSetting = Get-PackageSetting $packagePath
			$packageUpgradePath = join-path $packagePath $upgrade_id
			ExportPackage $packagePath  $_  $packageSetting
			if (test-Path $packageUpgradePath)
			{
				$des = join-path (join-Path $exportRootPackageName $packageName) $upgrade_id
				AddDeployCommand $batFile $epiDeployPath $des $packageSetting
			}
		}
	Add-Content $batFile.FullName "
	)"
	ExportFrameworkTools $packagesPath $packages
	Write-Host "A Run.bat file has been created in the $($exportRootPackageName)"
}

Function AddDeployCommand($batFile, $epiDeployPath, $des, $packageSetting)
{
	$command =  "..\$epiDeployPath  -a config -s ""%~f1""  -p ""..\..\..\$($des)\*"" -c ""$($packageSetting["connectionStringName"])"
	Add-Content $batFile.FullName $command
	Add-Content $batFile.FullName  "echo ...."
	$command =  "..\$epiDeployPath  -a sql -s ""%~f1""  -p ""..\..\..\$($des)\*"" -c ""$($packageSetting["connectionStringName"])"
	Add-Content $batFile.FullName $command
	Add-Content $batFile.FullName  "echo ...."
}

Function AddUsage ()
{
	$content = "@echo off
	if '%1' ==''  (
		echo  USAGE: %0  web application path ""[..\episerversitepath or c:\episerversitepath]""
	) else (
	" 
	New-Item (join-path $exportRootPackageName "Run.bat") -type file -force -value $content
}

Function CreateRootPackage ($deployPackagePath)
{
	if (test-path $deployPackagePath)
	{
		remove-Item -path $deployPackagePath -Recurse
	}
	$directory = New-Item -ItemType directory -Path $deployPackagePath
	Write-Host "An Export package is created $($directory.Fullname)"
}

Function ExportPackage($packagePath, $package, $setting)
{
	$packageUpgradePath = join-path $packagePath $upgrade_id
	$packageName = $package.id + "." + $package.version
	if (test-path $packageUpgradePath)
	{
		$packageRootPath = join-Path $exportRootPackageName $packageName
		write-Host "Exporting  $($packageName) into $($packageRootPath)"
		$destinationUpgradePath  = join-Path $packageRootPath  $upgrade_id
		copy-Item $packageUpgradePath  -Destination $destinationUpgradePath  -Recurse
		if ($setting["settingPath"])
		{
			copy-Item $setting["settingPath"]  -Destination $packageRootPath 
		}
	} 
}

Function GetEpiFrameworkFromPackages($packages)
{
	$framework = $packages |where-object  {$_.id -eq $frameworkPackageId} | select-Object -First 1
	if ($framework -ne $null)
	{
		return $framework.id + "." + $framework.version 
	}
}

Function ExportFrameworkTools($packagePath, $packages)
{
	$frameworkName = GetEpiFrameworkFromPackages $packages
	$frameworkToolsPath = join-Path $frameworkName $tools_id
	$packageToolsPath = join-path $packagePath $frameworkToolsPath
	if (test-path $packageToolsPath)
	{
		$destinationToolPath  = join-Path $exportRootPackageName $frameworkToolsPath
		copy-Item $packageToolsPath  -Destination $destinationToolPath  -Recurse
	}
}
 
Function Upgrade-Packages($action, $sitePath, $packagesPath, $packages)
{
	$epiDeployPath = GetDeployExe $packagesPath  $packages
	$packages |foreach-object -process {
			$packagePath = join-path $packagesPath ($_.id + "." + $_.version)
			$settings = Get-PackageSetting $packagePath 
			Upgrade-Package $packagePath $action $sitePath $epiDeployPath  $settings  
		}
}
 
 Function Upgrade-Package($packagePath, $action, $sitePath, $epiDeployPath, $settings)
 {
	$upgradePath = join-path $packagePath $upgrade_id
    if (test-Path $upgradePath)
	{
		Write-Host "*************************************************************"
		Write-Host "$epiDeployPath  -a $action -s $sitePath  -p $($upgradePath) -c $($settings["connectionStringName"]) "
		&$epiDeployPath  -a $action -s $sitePath  -p $upgradePath -c $settings["connectionStringName"] 
		Write-Host "*************************************************************"
	}
}
 
#	Find out EPiDeploy from frameworkpackage
Function GetDeployExe($packagesPath, $packages)
 {
	$frameWorkPackage = $packages | Where-Object {$_.id -match $frameworkPackageId }
	$frameWorkPackagePath = join-Path $packagesPath ($frameWorkPackage.id + "." + $frameWorkPackage.version)
	join-Path  $frameWorkPackagePath "tools\epideploy.exe"
 }

#	Find "setting.config" condig file under the package  
#	The format of the settings file is like 
#		<settings>
#			<connectionStringName/>
#		</settings>
Function Get-PackageSetting($packagePath)
{
	$packageSettings = gci -Recurse $packagePath -Include $setting | select -first 1
	if ($packageSettings -ne $null)
	{
		$xml = [xml](gc $packageSettings)
		if ($xml.settings.SelectSingleNode("connectionStringName") -eq $null)
		{
			$connectionStringName = $xml.CreateElement("connectionStringName")
			$xml.DocumentElement.AppendChild($connectionStringName)
		}
		if ([String]::IsNullOrEmpty($xml.settings.connectionStringName))
		{
			$xml.settings.connectionStringName  = "EPiServerDB"
		}
	}
	else
	{
		$xml = [xml] "<settings><connectionStringName>EPiServerDB</connectionStringName></settings>"
	}
	 @{"connectionStringName" = $($xml.settings.connectionStringName);"settingPath" = $packageSettings.FullName}
}

# Get base params
Function GetParams()
{
	#Get The current Project
	$project  = Get-project
	$projectPath = Get-ChildItem $project.Fullname
	#site path
	$sitePath = $projectPath.Directory.FullName
	#Get project packages 
	$packages = Get-Package -ProjectName $project.Name
	@{"project" = $project; "packages" = $packages; "sitePath" = $sitePath}
}

#Exported functions are Update-EPiDataBase Update-EPiConfig
export-modulemember -function  Update-EPiDatabase, Update-EPiConfig, Export-EPiPackage 