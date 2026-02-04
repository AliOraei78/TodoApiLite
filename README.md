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

## Day 3: Dockerfile Deep Dive & Docker Hub

**Completed Today:**
- Multi-stage Dockerfile for TodoApiLite (SDK → ASP.NET runtime)
- Built image: todo-api-lite:latest (size ~210MB)
- Tagged and pushed to Docker Hub: 0xencryptedx0/todo-api-lite:latest
- Ran container and verified Swagger at http://localhost:8080/swagger

**Key Learnings:**
- Multi-stage build reduces final image size dramatically
- docker build -t name:tag .
- docker tag & docker push to Docker Hub
- EXPOSE and port mapping (-p host:container)

Here is the **English translation**, with only the Persian parts translated:

## Day 4: Advanced Docker Commands & Scripts

**Commands Learned:**

- docker inspect [container/image]`     # Full details
- docker logs -f [container]`           # Follow live logs
- docker exec -it [container] sh`       # Enter container shell
- docker stats [container]`             # Monitor CPU / RAM / Network

**Scripts:**

- docker-utils.ps1`: Container management (start / stop / logs / exec / stats / build / clean)
- cleanup.ps1`: Quick cleanup of unused containers and images

## Day 5: First Docker Compose - API + PostgreSQL

**Completed Today:**
- Created docker-compose.yml with two services: api (TodoApiLite) + db (PostgreSQL)
- Used volumes for persistent PostgreSQL data
- Ran multi-container app with `docker compose up --build`
- Tested Swagger at http://localhost:8080/swagger
- Used `docker compose down`, `logs`, `ps`

**Commands Learned:**
- docker compose up --build
- docker compose down
- docker compose logs [service]
- docker compose ps