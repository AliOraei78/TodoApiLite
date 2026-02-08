FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN mkdir -p /root/.nuget/fallbackpackages

COPY TodoApiLite.Api/*.csproj ./TodoApiLite.Api/
RUN dotnet restore "./TodoApiLite.Api/TodoApiLite.Api.csproj" -r linux-musl-x64

COPY . .
WORKDIR "/src/TodoApiLite.Api"
RUN dotnet publish "TodoApiLite.Api.csproj" -c Release -o /app/publish \
    --no-restore \
    -r linux-musl-x64 \
    --self-contained false \
    /p:PublishTrimmed=true \
    /p:TrimMode=link

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

RUN addgroup -S appgroup && adduser -S appuser -G appgroup
RUN chown -R appuser:appgroup /app

COPY --from=build /app/publish . 

USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["dotnet", "TodoApiLite.Api.dll"]