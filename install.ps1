$res = choco -v
if ($res -ne $null) {

    Write-Host "Installing Docker"
    choco install docker-cli -y

    Write-Host "Installing Git"
    choco install git -y

    git clone https://github.com/IBM/MAX-Toxic-Comment-Classifier.git
    cd MAX-Toxic-Comment-Classifier
    docker build -t max-toxic-comment-classifier .
    docker run -it --name toxic-chat -p 5000:5000 max-toxic-comment-classifier
    docker stop toxic-chat

}
else {
    Write-Host "Installing Chocolatey"

    Set-ExecutionPolicy Bypass -Scope Process -Force;
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072;
    iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

    Write-Host "Installing Docker"
    choco install docker-cli -y

    Write-Host "Installing Git"
    choco install git -y

    git clone https://github.com/IBM/MAX-Toxic-Comment-Classifier.git
    cd MAX-Toxic-Comment-Classifier
    docker build -t max-toxic-comment-classifier .
    docker run -it --name toxic-chat -p 5000:5000 max-toxic-comment-classifier
}


