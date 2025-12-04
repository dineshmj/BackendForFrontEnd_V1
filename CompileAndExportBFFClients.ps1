cls;

function Execute-SyncBuild {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Directory,

        [Parameter(Mandatory=$true)]
        [string]$NpmCommand
    )

    cd $Directory
    
    Write-Host "`n`n`t########################################################" -ForegroundColor Cyan  
    Write-Host "`t1. Installing dependencies in '$Directory' (npm install)..." -ForegroundColor Cyan
    Write-Host "`t########################################################`n`n" -ForegroundColor Cyan  

    npm install

    if ($LASTEXITCODE -ne 0) {
        Write-Error "npm install failed in '$Directory'. Stopping script."
        exit 1
    }
   
    Write-Host "`n`n`t########################################################" -ForegroundColor Cyan  
    Write-Host "`t2. Running '$NpmCommand' in '$Directory'..." -ForegroundColor Cyan
    Write-Host "`t########################################################`n`n" -ForegroundColor Cyan  

    npm run $NpmCommand

    if ($LASTEXITCODE -ne 0) {
        Write-Error "'npm run $NpmCommand' failed in '$Directory'. Stopping script."
        exit 1
    }
   
    Write-Host "Command complete for '$Directory'." -ForegroundColor Green
}

cls

## $visualStudioCodePath = "C:\Users\Dinesh\AppData\Local\Programs\Microsoft VS Code\Code.exe";
$visualStudioCodePath = "P:\Users\dines_y5ddmdz\AppData\Local\Programs\Microsoft VS Code\Code.exe"

$codeRootFolder = $PSScriptRoot;

$shellSpaAppFolder = "$($codeRootFolder)\src\Shell\client-app";
$productsMicroserviceSpaAppFolder = "$($codeRootFolder)\src\Microservices\Products\BFF.Web\client-app";
$ordersMicroserviceBffAppFolder = "$($codeRootFolder)\src\Microservices\Orders\BFF.Web";


Write-Host "======================================================================";
Write-Host "==           Step # 1: Exporting Shell NextJS SPA for BFF           ==" -ForegroundColor Yellow;
Write-Host "======================================================================";

Execute-SyncBuild -Directory $shellSpaAppFolder -NpmCommand "export"

Write-Host "`n`n======================================================================";
Write-Host "==  Step # 2: Exporting Products Microservice NextJS SPA for BFF    ==" -ForegroundColor Yellow;
Write-Host "======================================================================";

Execute-SyncBuild -Directory $productsMicroserviceSpaAppFolder -NpmCommand "export"

Write-Host "`n`n======================================================================";
Write-Host "==    Step # 3: Invoking OrdersMicroservice NextJS SPA for BFF      ==" -ForegroundColor Yellow;
Write-Host "======================================================================";

Start-Process -FilePath $visualStudioCodePath -ArgumentList $ordersMicroserviceBffAppFolder