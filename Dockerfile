FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN mkdir -p /root/.nuget/fallbackpackages

COPY TodoApiLite.Api/*.csproj ./TodoApiLite.Api/
RUN dotnet restore "./TodoApiLite.Api/TodoApiLite.Api.csproj"

COPY . .
WORKDIR "/src/TodoApiLite.Api"
RUN dotnet publish "TodoApiLite.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .   
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TodoApiLite.Api.dll"]