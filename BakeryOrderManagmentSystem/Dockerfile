# Base image for .NET SDK and Node
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 80

# Stage 1: Build the .NET API
FROM base AS build
WORKDIR /src
COPY ["BakeryOrderManagmentSystem/BakeryOrderManagmentSystem.API.csproj", "BakeryOrderManagmentSystem/"]
RUN dotnet restore "./BakeryOrderManagmentSystem/BakeryOrderManagmentSystem.API.csproj"
COPY . .
WORKDIR "/src/BakeryOrderManagmentSystem"
RUN dotnet build "./BakeryOrderManagmentSystem.API.csproj" -c Release -o /app/build

# Stage 2: Publish the .NET API
FROM build AS publish
RUN dotnet publish "BakeryOrderManagmentSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Build the Angular application
FROM node:18 AS angular-build
WORKDIR /src
COPY Client/package*.json ./
RUN npm install
COPY Client ./
RUN npm run build --prod

# Stage 4: Serve Angular application with NGINX
FROM nginx:alpine AS frontend
COPY --from=angular-build /src/dist/your-angular-app /usr/share/nginx/html
EXPOSE 80

# Stage 5: Final stage for .NET API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BakeryOrderManagmentSystem.API.dll"]