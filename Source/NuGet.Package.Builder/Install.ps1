param($installPath, $toolsPath, $package, $project)


try {
	$projectFileNameWithoutExtension = [io.path]::GetFileNameWithoutExtension($project.FullName)
	$nuspecFileName = $projectFileNameWithoutExtension + ".nuspec"
	$destinationFile = $toolsPath + "\\" + $nuspecFileName
	$sourceFile = $toolsPath + "\\Package.template"

	# Only update if package does not exists already
	try {
		$existingItem = $project.ProjectItems.Item($nuspecFileName)
	} catch {
		Copy-Item $sourceFile $destinationFile

		# Add new item to project.
		$project.ProjectItems.AddFromFileCopy($destinationFile)
	}
} catch {
	$ErrorMessage = $_.Exception.Message
	Write-Host $ErrorMessage
}
