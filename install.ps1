Set-ExecutionPolicy Bypass -Scope Process -Force;
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072;

# Check all the commands to make sure we have all the needed commands to run the docker code!
$choco = choco -v
$docker = docker -v
$git = git --version
if ($choco -eq $null) {
    Write-Host "Installing Chocolatey"    
    iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
}
if ($docker -eq $null){
    Write-Host "Installing Docker"
    choco install docker-cli -y
}
if ($git -eq $null){
    Write-Host "Installing Git"
    choco install git -y
}

# Run the commands to build and create the docker container
git clone https://github.com/IBM/MAX-Toxic-Comment-Classifier.git
cd MAX-Toxic-Comment-Classifier
docker build -t max-toxic-comment-classifier .
docker run -it --name toxic-chat -p 5000:5000 max-toxic-comment-classifier
docker stop toxic-chat
