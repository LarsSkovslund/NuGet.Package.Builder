param($installPath, $toolsPath, $package, $project)

try {
	$projectFileNameWithoutExtension = [io.path]::GetFileNameWithoutExtension($project.FullName)
	$nuspecFileName = $projectFileNameWithoutExtension + ".nuspec"	
	try {
		$itemToRemove = $project.ProjectItems.Item($nuspecFileName)
		Write-Host $("Removing " + $itemToRemove.Name)
		$itemToRemove.Delete()
	} catch {
		Write-Host $("Item not found " + $nuspecFileName)
	}
} catch {
	$ErrorMessage = $_.Exception.Message
	Write-Host $ErrorMessage
}