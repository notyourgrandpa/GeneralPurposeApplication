# 🐳 Docker

How the Docker setup works, what changed recently, what to improve, and what to do next.

- [1. Migration image fix (2026-10-15)](#1-migration-image-fix-2026-10-15)
- [2. Current setup and review](#2-current-setup-and-review)
- [3. What's next](#3-whats-next)

---

## 1. Migration image fix (2026-10-15)

### The error

```
docker compose -p kita-prod-test -f docker-compose.yml -f docker-compose.prod.yml build --no-cache migrate
```

failed at the `dotnet ef migrations bundle` step with:

```
A network-related or instance-specific error occurred while establishing a connection to SQL Server ... error: 40
Unable to create a 'DbContext' of type ''. ... Unable to resolve service for type 'DbContextOptions<ApplicationDbContext>'
```

### Root cause

1. To build a migration bundle, EF Core starts the app's host (`Program.cs`) so it can get `ApplicationDbContext` from it.
2. On startup, Serilog sets up the SQL Server sink with `AutoCreateSqlTable = true`, which connects to the database right away. At build time there is no database, so starting the host failed (the first error).
3. EF then tried to create `ApplicationDbContext` without the host, which isn't possible, so you got the second error.

You had already added a `SkipSqlLogging` switch in `Program.cs` and a fixed `Dockerfile.migrations` at the repo root. But there were **two** copies of `Dockerfile.migrations`, and `docker-compose.prod.yml` built the one in `GeneralPurposeApplication.Infrastructure/`, which didn't have the fix.

Once the SQL error was gone, a second error showed up: `Node.js is required to build and run this project`. `GeneralPurposeApplication.Server.csproj` references the Angular `.esproj` unless `DockerBuild=true`, and the SDK image has no Node.js.

### What changed and why

| Change | Why |
|---|---|
| Moved the fixed `Dockerfile.migrations` to `GeneralPurposeApplication.Infrastructure/` and removed the other copy | A single source of truth. It sits next to the migrations, the same way `Server/Dockerfile` and `client/Dockerfile` sit in the project they build. |
| `docker-compose.prod.yml` → `dockerfile: GeneralPurposeApplication.Infrastructure/Dockerfile.migrations` | Points compose at the fixed file. |
| Added `ENV DockerBuild=true` to `Dockerfile.migrations` | Skips the `.esproj` reference so the build doesn't need Node.js. `Server/Dockerfile` already does this with `-p:DockerBuild=true`. |
| Removed the `Dockerfile.migrations` solution item from `GeneralPurposeApplication.sln` | It pointed at the root file, which no longer exists. The file still shows up under the Infrastructure project. |
| Kept `SkipSqlLogging=true` and `JwtSettings__SecurityKey` placeholders in the bundle step | The host has to start at build time without a database. These are throwaway build-only values, not secrets. |

### Why there is no design-time DbContext factory

An `IDesignTimeDbContextFactory` would let EF skip `Program.cs` altogether. It isn't needed here:

- With `SkipSqlLogging`, the host starts fine at build time.
- A factory would be a second place where the DbContext is configured, and it can drift from `Program.cs`.
- A factory would also change how local `dotnet ef` commands find the connection string (it bypasses the host's user secrets).
- The bundle gets its real connection string at run time (`--connection`), so the build-time placeholder never touches a database.

Add a factory later only if starting the host at build time starts to need things the build can't provide.

**Verified:** `build migrate` now finishes with `Done. Migrations Bundle: /app/efbundle`. The bundle has not yet been run against a real database.

---

## 2. Current setup and review

### Files

| File | Purpose |
|---|---|
| `docker-compose.yml` | Base stack: `db`, `db-health`, `backend`, `frontend`, plus the `sqlserver_data` volume |
| `docker-compose.override.yml` | Dev settings, loaded automatically by `docker compose up` (Development environment, CORS for `localhost:4200`) |
| `docker-compose.prod.yml` | Prod overlay: removes published ports, adds restart policies, log rotation, the `migrate` job and the data-protection keys volume |
| `GeneralPurposeApplication.Server/Dockerfile` | Multi-stage .NET 8 build → `aspnet:8.0` runtime with a `curl` healthcheck on `/api/ready` |
| `GeneralPurposeApplication.Infrastructure/Dockerfile.migrations` | Builds an EF Core migration bundle → runs it on `aspnet:8.0` |
| `generalpurposeapplication.client/Dockerfile` | Multi-stage Angular build (`node:20-alpine`) → served by `nginx:alpine` |
| `generalpurposeapplication.client/nginx.conf` | Serves the SPA and proxies `/api/` to `backend:8080` |
| `.dockerignore` | Excludes `bin/`, `obj/`, `node_modules/`, `.env`, etc. Only applies to builds whose context is the repo root. |
| `.env` / `.env.example` | Secrets and settings read by compose (`.env` is gitignored) |

### Startup order

```
dev:   db → db-health (waits for SELECT 1) → backend (healthy) → frontend
prod:  db → db-health → migrate (runs bundle, exits 0) → backend (healthy) → frontend
```

### Run commands

```bash
# Dev (base + override, loaded automatically)
docker compose up --build

# Prod
docker compose -p kita-prod-test -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

### What's already good

- Every image uses a multi-stage build, so SDK and Node toolchains stay out of the runtime images.
- `Server/Dockerfile` copies the `.csproj` files and restores before `COPY . .`, so package restore is cached.
- Migrations run as a one-off job **before** the backend starts, instead of `Database.Migrate()` at app startup. That's the recommended pattern.
- Startup waits on real readiness (`service_completed_successfully`, `service_healthy`) instead of bare `depends_on`.
- In prod, `db` and `backend` publish no ports, and the frontend binds only to `127.0.0.1`, so all traffic goes through the host's reverse proxy.
- In prod, logs rotate (`max-size`/`max-file`), services have restart policies, and named volumes hold DB data and data-protection keys.
- `nginx` serves the frontend and the `/api` proxy on one origin, so the browser doesn't need CORS.
- Secrets come from a gitignored `.env`, and `.env.example` documents the keys.

### 🔴 Bad practice: fix these before deploying to a real server

#### 1. Frontend build context has the wrong case
`docker-compose.yml` uses `context: ./GeneralPurposeApplication.Client`, but the folder is `generalpurposeapplication.client`. That works on Windows (case-insensitive) and **breaks on Linux**, which is where you'll deploy.
**Fix:** use `context: ./generalpurposeapplication.client`.

#### 2. SQL Server Developer edition in production
`MSSQL_PID: "Developer"` is in the base file, so prod uses it too. Developer edition's license forbids production use.
**Fix:** override `MSSQL_PID` in `docker-compose.prod.yml`: `Express` (free, up to 10 GB per database) or a licensed edition (`Standard`/`Enterprise`) if you have one.

#### 3. The client build has no `.dockerignore`
The root `.dockerignore` doesn't apply when the build context is the client folder. Your host `node_modules`, `dist`, `.angular` and `obj` all get sent to Docker, and `COPY . .` overwrites the Linux `node_modules` from `npm ci` with your Windows one. That makes builds slow and can break native binaries (esbuild).
**Fix:** add `generalpurposeapplication.client/.dockerignore` with at least `node_modules`, `dist`, `.angular`, `obj`, `bin`, `.vscode`.

#### 4. Everything connects as `sa`
The backend, the migrations and `db-health` all log in as the server admin. If the app is compromised, the whole SQL Server is too.
**Fix:** create two SQL logins: a **migrator** with DDL rights on the app database (used only by `migrate`), and an **app** user with read/write/execute only (used by `backend`). Keep `sa` for administration only.
Note: the Serilog sink's `AutoCreateSqlTable = true` needs DDL rights. Create the `LogEvents` table in a migration and set `AutoCreateSqlTable = false` so the app user can stay least-privilege.

#### 5. Password in the `migrate` command line
`migrate` passes the connection string, with the password, as `--connection` in `command:`. Command arguments show up in `docker inspect` and process lists. It also duplicates the `ConnectionStrings__DefaultConnection` environment variable.
**Fix:** use only one of the two. Prefer the environment variable (or a secret file, see improvement 4) and drop `command: --connection`.

#### 6. Floating image tags and versions
`mssql/server:2022-latest`, `mssql-tools` (no tag = `latest`), `nginx:alpine`, `node:20-alpine`, and the NuGet `Microsoft.AspNetCore.SpaProxy 8.*-*` (which even allows prereleases). The same commit can build a different image next week.
**Fix:** pin exact versions, e.g. `mssql/server:2022-CU<n>-ubuntu-22.04`, `nginx:1.27-alpine`, `node:20.x-alpine`, and a fixed SpaProxy version. Upgrade on purpose.

### 🟡 Improvements toward best practice

1. **Replace the `db-health` container with a `healthcheck:` on `db`.** The SQL Server 2022 image already includes `sqlcmd` at `/opt/mssql-tools18/bin/sqlcmd` (needs `-C`). Add a `healthcheck` to `db` and have `backend`/`migrate` use `condition: service_healthy`. That's one less container, and it drops the outdated `mssql-tools` image.
2. **Run containers as non-root.** Add `USER app` to the runtime stages of `Server/Dockerfile` and `Dockerfile.migrations`; the `aspnet:8.0` image includes that user and already listens on 8080. For the frontend, use `nginxinc/nginx-unprivileged` (listens on 8080).
3. **Cache restores in `Dockerfile.migrations`.** It runs `COPY . .` before `dotnet restore`, so any file change triggers a full restore and the whole repo (including the client) ends up in the build stage. Copy the `.csproj` files first, as `Server/Dockerfile` does.
4. **Use Docker secrets in prod.** Environment variables show up in `docker inspect` and are inherited by child processes. Compose `secrets:` (files mounted at `/run/secrets/...`) are the safer option for `DB_SA_PASSWORD`, `JWT_SECURITY_KEY` and the default passwords. ASP.NET Core can read them with `AddKeyPerFile("/run/secrets")`.
5. **Split env files per environment.** Use `.env` for dev and a separate prod file on the server only (`--env-file`), so dev and prod never share secrets.
6. **Keep only shared settings in the base compose file.** Published ports (`1434`, `8080`, `4200`) and dev-style JWT values (`12345678-Grandpa`) live in the base file, so prod has to undo them with `!reset`/`!override`. Move dev-only ports and values to `docker-compose.override.yml`. Also note that the base `JwtSettings__Audience` env var overrides `appsettings.Production.json` in prod. Set prod issuer/audience explicitly in the prod overlay or the prod env file.
7. **Bind dev ports to localhost.** In dev, `"1434:1433"` exposes SQL Server to your whole network. Use `"127.0.0.1:1434:1433"` (and the same for 8080/4200).
8. **Handle forwarded headers in the backend.** The app runs behind nginx (and in prod, behind a host reverse proxy too), but `Program.cs` calls `UseHttpsRedirection()` without `UseForwardedHeaders()`. The app sees every request as plain HTTP from the nginx IP. Add `UseForwardedHeaders` (X-Forwarded-For/Proto, trusting the Docker network) early in the pipeline.
9. **Fix `/api/health` for containers.** It pings `www.ryadel.com`, `www.google.com` and a random non-existent domain over ICMP. Containers often can't ping out, and the random domain always fails, so health always reports degraded or unhealthy. Check things the app depends on instead (DB connectivity via `AddDbContextCheck`), and keep `/api/ready` as the liveness check.
10. **Set memory limits.** SQL Server will take most of the host's RAM. Set `MSSQL_MEMORY_LIMIT_MB` and/or a compose `mem_limit` for `db`, and reasonable limits for the other services.
11. **Remove `curl` from the runtime image (optional).** It's only there for the healthcheck and adds attack surface (and an `apt-get` layer). The cleaner option is a small static healthcheck binary copied in from a build stage. Low priority; it's fine as is.
12. **Run `migrate` in dev too.** Right now only prod runs the bundle, so dev and prod apply migrations differently. Putting `migrate` in the base file means dev tests the same path prod uses.
13. **Use a top-level `name:` instead of `-p kita-prod-test`.** Put `name: kita` in the compose file (or set `COMPOSE_PROJECT_NAME` in the env file) so nobody forgets the flag and creates a second stack with new volumes by accident.

---

## 3. What's next

Stages, in order. Each one is just what to do and what not to do.

### Stage 1: Get the current state committed and working end-to-end
- **Do** commit `Infrastructure/Dockerfile.migrations`, the `Program.cs` `SkipSqlLogging` change and the compose changes together, so the fix doesn't live only on your machine.
- **Do** run the full prod stack locally (`up`, not just `build`) and confirm that `migrate` exits 0, the tables exist, the backend turns healthy and the frontend reaches `/api` through nginx.
- **Do** test from an empty database (`down -v` on a test project name) *and* from an existing one, because migrations must work in both cases.
- **Don't** use `down -v` on any project name that holds data you care about. It deletes the volumes.

### Stage 2: Fix the bad practices from section 2
- **Do** the 🔴 items before anything leaves your machine, especially the case-sensitive path and the SQL Server edition. Both break or violate the license on a Linux server.
- **Do** create the least-privilege SQL logins now, while the database is small and nothing depends on `sa`.
- **Don't** mix this with feature work. Keep the Docker hardening in its own commits or PR so problems are easy to trace.

### Stage 3: Choose and prepare the server
- **Do** decide where prod runs (a single VPS with Docker Compose fits this app) and use a Linux host.
- **Do** put a reverse proxy on the host (or as a container) that handles HTTPS with automatic certificates and forwards to `127.0.0.1:8081`.
- **Do** lock the host down: firewall open only for 80/443 (and SSH), key-based SSH, automatic OS security updates.
- **Don't** publish SQL Server or the backend to the internet. Keep using an SSH tunnel when you need to reach the database.

### Stage 4: Build images in CI and push to a registry
- **Do** build images in CI (e.g. GitHub Actions) on push or tag, run the tests first, then push to a registry (GHCR, Docker Hub, etc.).
- **Do** tag images with a version or commit SHA, and have the prod compose file reference those tags (`image:`) instead of `build:`.
- **Don't** build images on the production server, and don't deploy `latest`. You need to know exactly what's running and be able to go back to it.

### Stage 5: Deploy
- **Do** keep only the compose files and the prod env/secret files on the server. Create the secrets there by hand or through CI secrets, never from git.
- **Do** deploy with pull → `up -d` so `migrate` runs before the new backend starts.
- **Do** write migrations to be backward compatible (add first, remove in a later release), so a rollback to the previous image still works with the new schema.
- **Don't** edit containers or files inside them by hand on the server. Every change goes through git → CI → deploy.

### Stage 6: Backups and recovery
- **Do** schedule automatic SQL Server backups (`BACKUP DATABASE` to a file), copy them **off the server**, and keep several generations.
- **Do** back up the `dataprotection_keys` volume too. If you lose it, users are logged out and protected data can't be read.
- **Do** run a real restore test into a throwaway stack. A backup you haven't restored isn't proven.
- **Don't** count on Docker volumes or VPS snapshots alone as your backup.

### Stage 7: Monitoring and maintenance
- **Do** set up an external uptime check on the public URL and alerts for when containers restart or stay unhealthy.
- **Do** watch disk space (SQL data, logs, old images) and prune unused images regularly.
- **Do** plan regular base-image updates (security patches) by bumping the pinned tags on purpose and redeploying through CI.
- **Don't** let the Serilog `LogEvents` table grow forever. Decide on a retention period.
