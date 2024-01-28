# total number of files to generate
$numFilesTotal = 120

# spread files over amount of folders
$numFolders = 10

# subfolder to create files/folders in
$rootFolder = "File Generation Test"
New-Item -Path $rootFolder -ItemType Directory -Force

# calculate files per folder
$filesPerFolder = [Math]::Ceiling($numFilesTotal / $numFolders)

# current file number. can be used to generate within a certain range
# (generates from $currentFileNumber to $numFilesTotal)
$currentFileNumber = 1

# loop to create folders and files inside $rootFolder
for ($folder = 1; $folder -le $numFolders; $folder++) {
    $folderPath = Join-Path $rootFolder "Folder$folder"
    New-Item -Path $folderPath -ItemType Directory -Force

    # determine the number of files to create in current folder (processed in loop)
    $filesToCreate = [Math]::Min($filesPerFolder, $numFilesTotal - $currentFileNumber + 1)
    
    for ($file = 1; $file -le $filesToCreate; $file++) {
        $filePath = Join-Path $folderPath "File$currentFileNumber.txt"
        New-Item -Path $filePath -ItemType File -Force

        # increment the file number
        $currentFileNumber++
    }

    # exit loop if all files have been created
    if ($currentFileNumber -gt $numFilesTotal) {
        break
    }
}

Write-Host "Files created successfully in '\$rootFolder'."
Write-Host "Press enter to exit."
Read-Host