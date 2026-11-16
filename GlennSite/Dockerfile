# Container recipe for Glenn's site. A Dockerfile is read top to bottom; each FROM starts a new "stage".
# We use two stages so the final image contains only what is needed to RUN the site,
# not the (much larger) tools needed to BUILD it.

# ---------- Stage 1: build ----------
# The .NET SDK image has the compiler and publish tooling. It is big, and it is thrown away at the end.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project file first and restore packages. Docker caches each step, so as long as the
# .csproj has not changed, later builds skip this slow step.
COPY GlennSite.csproj .
RUN dotnet restore

# Now copy the rest of the source (wwwroot included) and publish a release build.
COPY . .

# Safety net: if build output from a developer's machine (bin/ and obj/) slipped into the build
# context, it holds Windows paths that break on Linux. Delete it and restore again. When the
# context is clean this is a quick no-op, because the packages are already cached from above.
RUN rm -rf bin obj && dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Stage 2: run ----------
# The ASP.NET runtime image is small: just enough to run the published site.
FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS final
WORKDIR /app

# Bring in the published output from the build stage; nothing else comes along.
COPY --from=build /app/publish .

# Run as a non-root user (the runtime image provides one). If someone ever broke into the site,
# they would not have administrator rights inside the container.
USER $APP_UID

# .NET 8 and later listen on port 8080 inside the container by default. Azure Container Apps
# will take care of HTTPS in front of this, so the app itself only speaks plain HTTP.
EXPOSE 8080

ENTRYPOINT ["dotnet", "GlennSite.dll"]
