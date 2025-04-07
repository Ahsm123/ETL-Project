[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

Write-Host "Starter Docker Compose (Kafka, Zookeeper)..."
cd "source/repos/ETL-Project"
docker compose -f docker-compose.yml up -d

Write-Host "Venter 5 sekunder på at Kafka starter op..."
Start-Sleep -Seconds 5

Write-Host "Opretter Kafka topics..."
.\create-topics.ps1

Write-Host "Starter Config API..."
Start-Process "cmd.exe" "/c set DOTNET_ENVIRONMENT=Development && set ASPNETCORE_URLS=https://localhost:7027 && dotnet run --project ../ETLConfig/ETLConfig.API/ETLConfig.API.csproj"

Write-Host "Starter Test Source API..."
Start-Process "cmd.exe" "/c set DOTNET_ENVIRONMENT=Development && set ASPNETCORE_URLS=https://localhost:7112 && dotnet run --project ../ETLTestSource/ETLTestSource.API/ETLTestSource.API.csproj"

Write-Host "Starter ExtractAPI..."
Start-Process "cmd.exe" "/c set DOTNET_ENVIRONMENT=Development && set ASPNETCORE_URLS=https://localhost:7087 && dotnet run --project ./ExtractAPI/ExtractAPI.csproj"

Write-Host "Starter Transform..."
Start-Process "cmd.exe" "/c set DOTNET_ENVIRONMENT=Development && dotnet run --project ./Transoform/Transform.csproj"

Write-Host "Starter Load..."
Start-Process "cmd.exe" "/c set DOTNET_ENVIRONMENT=Development && dotnet run --project ./Load/Load.csproj"

Write-Host "Venter 10 sekunder før vi trigger extraction..."
Start-Sleep -Seconds 10

function Trigger-Extract {
    param ([string]$configId)

    try {
        $response = Invoke-RestMethod -Uri "https://localhost:7087/api/Extract/$configId" `
                                      -Method POST `
                                      -ContentType "application/json"
        Write-Host "`nExtract triggered for ${configId}:"

        $response | ConvertTo-Json -Depth 10
    }
    catch {
        Write-Warning "Fejl ved extract af '$configId': $_"
    }
}

Trigger-Extract "pipeline_1"
Trigger-Extract "pipeline_5"
