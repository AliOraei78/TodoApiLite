Write-Host "Removing stopped containers..."
docker container prune -f

Write-Host "Removing unused images..."
docker image prune -f

Write-Host "Done!"