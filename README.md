## Day 1: Docker + Docker Compose

**Completed Today:**
- Docker Desktop successfully installed on Windows
- WSL 2 backend enabled
- First container executed: hello-world
- Verified installation with docker version, info, images, ps
- Docker Desktop running and healthy

**Key Learnings:**
- Docker containers are lightweight, isolated environments
- Image = blueprint, Container = running instance
- docker run pulls image and starts container
- No need to install dependencies locally

## Day 2: Docker Basics with TodoApiLite

**Completed Today:**
- Built custom Dockerfile for TodoApiLite
- Created image: todo-api-lite:latest
- Ran container with port mapping 8080:8080
- Tested Swagger at http://localhost:8080/swagger
- Created volume todo-data and demonstrated persistence
- Created custom network todo-net and connected containers
- Tested inter-container communication with curl

**Key Learnings:**
- Dockerfile multi-stage for .NET apps
- Volume for persistent data
- Network for container-to-container communication
- Port mapping (-p) for external access