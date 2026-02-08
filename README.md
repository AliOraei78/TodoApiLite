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

## Day 6: Volumes Advanced, Environment Variables, DependsOn & Healthcheck

**Completed Today:**
- Advanced Volumes in Docker Compose:
  - Named Volume (`postgres-data`) for persistent PostgreSQL data
  - Bind Mount (`.:/app`) for hot reload during development
  - Anonymous Volumes for temporary data
- Professional use of Environment Variables:
  - Loaded from `.env` file (POSTGRES_USER, POSTGRES_PASSWORD, POSTGRES_DB)
  - Passed to .NET via `ConnectionStrings__DefaultConnection`
  - Priority order: docker-compose → .env → system env
- Used `depends_on` with `condition: service_healthy` for exact startup order
- Added real Healthcheck for PostgreSQL service (`pg_isready`)
- Tested real data persistence:
  - Added Todos via Swagger
  - `docker compose down`
  - `docker compose up` again → data still there (volume kept it)
- Prepared TodoApiLite for production-ready setup

**Key Commands & Features Learned:**
- `docker compose down -v` → removes volumes too (clean start)
- `docker compose up --build` → rebuilds and starts services
- Healthcheck: `pg_isready` every 5s with start_period 30s
- Environment variable substitution: `${VAR:-default}`
- Volume types: named (persistent), bind mount (dev hot reload)

## Day 7: Networks in Compose, Service Discovery, Scaling

**Completed Today:**
- Custom network `todo-net` in docker-compose.yml
- Service Discovery: API connects to `db` by name (no hard-coded IP)
- Scaling: `docker compose up --scale api=3`
- Tested with `docker compose ps`, `exec`, `logs`
- Verified inter-service communication (ping db from api)

**Commands Learned:**
- docker compose up -d --scale api=3
- docker compose ps
- docker compose exec api sh
- docker compose logs api

## Day 8: Security in Docker – Secrets, Non-Root User, Image Scanning with Trivy

**Completed Today:**
- Implemented **Docker Secrets** for secure password management:
  - Moved PostgreSQL password to a separate file (`postgres_password.txt`)
  - Used `secrets` in docker-compose.yml with `POSTGRES_PASSWORD_FILE`
  - Updated `.gitignore` to exclude secrets file
- Added **non-root user** in Dockerfile for runtime stage:
  - Created `appuser` (uid 1000) and `appgroup` (gid 1000)
  - Changed ownership of `/app` with `chown`
  - Switched to `USER appuser` → container no longer runs as root
  - Improved security: reduces attack surface if container is compromised
- Installed and used **Trivy** for image vulnerability scanning:
  - Scanned `todo-api-lite:latest` for HIGH/CRITICAL vulnerabilities
  - Command: `trivy image --severity HIGH,CRITICAL todo-api-lite:latest`
  - Verified image is clean or identified issues (if any)
- Tested full security setup:
  - `docker compose up -d --build` → db uses secret, api runs as non-root
  - Verified Swagger works and data persists
  - Checked user with `docker inspect todo-container` → uid 1000
  - Ran Trivy scan and reviewed results

**Key Security Improvements & Learnings:**
- Never hardcode passwords in compose or .env files (use Docker Secrets)
- Running containers as non-root user prevents privilege escalation attacks
- Trivy is a fast, free tool for scanning Docker images for known CVEs
- Secrets are mounted as files in `/run/secrets/` inside container
- Non-root user requires `chown` on copied files in runtime stage
- Always scan images before deploying to production

## Day 9 – Phase 6: Multi-Stage Build, Image Size Optimization, Docker Caching

**Completed Today:**
- Deep understanding of **multi-stage build** and why it's essential (reduces image size by 70–90%)
- Created an optimized **multi-stage Dockerfile** for TodoApiLite project:
  - First stage: SDK for restore + publish (heavy ~1GB)
  - Second stage: only ASP.NET runtime (light ~200MB)
  - Cached layers: only csproj copied first → fast restore if code doesn't change
- Achieved significant image size reduction:
  - Before optimization: ~500–800MB (or more)
  - After: ~200–250MB (or less with further tweaks)
- Learned Docker build caching mechanism:
  - Each RUN/COPY/ADD is a layer
  - If previous layers unchanged → Docker uses cache (build 5–10× faster)
- Added security improvements:
  - Non-root user (`appuser`) for runtime stage
  - `chown` files to non-root user
  - ENV `DOTNET_RUNNING_IN_CONTAINER=true` for better .NET container performance
- Tested the optimized image:
  - Built with `docker build -t todo-api-lite:latest .`
  - Ran with `docker run -d -p 8080:8080 todo-api-lite:latest`
  - Verified Swagger at http://localhost:8080/swagger
  - Checked image size with `docker images`
- (Optional) Pushed optimized image to Docker Hub:
  - `docker tag todo-api-lite:latest 0xencryptedx0/todo-api-lite:latest`
  - `docker push 0xencryptedx0/todo-api-lite:latest`

**Key Learnings:**
- Multi-stage build: separate build (SDK) from runtime (ASP.NET) → smaller, faster, more secure images
- Caching: COPY csproj first → restore cached if no project change → huge time saver
- Non-root user: reduces attack surface (can't gain root on host if container breached)
- Image size matters: smaller images = faster pull/deploy, less storage, lower attack surface
- Use `--no-restore` in publish when restore already done in previous stage

## Day 10: Docker Swarm Intro - Clustering & Stack Deploy

**Completed Today:**
- Initialized single-node Swarm cluster (`docker swarm init`)
- Created `docker-stack.yml` for TodoApiLite (overlay network, deploy section)
- Deployed stack with `docker stack deploy -c docker-stack.yml todo-stack`
- Scaled API service to 3/5 replicas
- Tested Swagger load balancing across replicas
- Learned `docker stack ps`, `services`, `scale`, `rm`

**Commands Learned:**
- docker swarm init
- docker stack deploy -c file.yml stack-name
- docker stack ps/rm/services/scale
- overlay network for multi-host communication

## Day 11: Monitoring Stack – Prometheus, Grafana, cAdvisor

**Completed Today:**
- Added full monitoring stack to docker-compose.yml:
  - cAdvisor: container metrics collector
  - Prometheus: time-series database & alerting
  - Grafana: beautiful dashboards & visualization
- Configured prometheus.yml for scraping cAdvisor and Prometheus
- Tested endpoints:
  - cAdvisor: http://localhost:8081
  - Prometheus: http://localhost:9090
  - Grafana: http://localhost:3000 (login: admin/admin123)
- Imported cAdvisor dashboard (ID 14282 or 193) in Grafana
- Verified live metrics: CPU, RAM, Network usage of api & db containers

**Key Learnings:**
- cAdvisor exposes metrics at /metrics endpoint
- Prometheus scrapes metrics from targets
- Grafana visualizes Prometheus data with dashboards
- Monitoring stack runs as services in the same network (todo-net)
- Easy to scale and monitor production-like apps

**Commands Used:**
- docker compose up -d --build
- docker compose ps / logs
- Access Grafana → Add Prometheus data source → Import dashboard