# docker-utils.ps1

param (
    [string]$Command = "help"
)

switch ($Command.ToLower()) {
    "start" {
        Write-Host "Starting todo-container..."
        docker run -d -p 8080:8080 --name todo-container todo-api-lite:latest
    }
    "stop" {
        Write-Host "Stopping todo-container..."
        docker stop todo-container
        docker rm todo-container
    }
    "logs" {
        Write-Host "Showing logs (last 100 lines)..."
        docker logs --tail 100 todo-container
    }
    "exec" {
        Write-Host "Entering container shell..."
        docker exec -it todo-container sh
    }
    "stats" {
        Write-Host "Showing live stats..."
        docker stats todo-container
    }
    "build" {
        Write-Host "Building new image..."
        docker build -t todo-api-lite:latest .
    }
    "clean" {
        Write-Host "Cleaning stopped containers and unused images..."
        docker container prune -f
        docker image prune -f
    }
    "help" {
        Write-Host "Commands:"
        Write-Host "  .\docker-utils.ps1 start     - Start container"
        Write-Host "  .\docker-utils.ps1 stop      - Stop and remove container"
        Write-Host "  .\docker-utils.ps1 logs      - Show logs"
        Write-Host "  .\docker-utils.ps1 exec      - Enter container shell"
        Write-Host "  .\docker-utils.ps1 stats     - Show live resource usage"
        Write-Host "  .\docker-utils.ps1 build     - Rebuild image"
        Write-Host "  .\docker-utils.ps1 clean     - Remove stopped containers/images"
    }
    default {
        Write-Host "Unknown command. Use 'help' for list."
    }
}