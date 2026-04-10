# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# SỬA TẠI ĐÂY: Trỏ đúng vào thư mục ConnectDB
COPY ["ConnectDB/ConnectDB.csproj", "ConnectDB/"]
RUN dotnet restore "ConnectDB/ConnectDB.csproj"

# Copy toàn bộ mã nguồn vào
COPY . .

# Di chuyển vào thư mục chứa file csproj để build
WORKDIR "/src/ConnectDB"
RUN dotnet publish "ConnectDB.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render dùng port 8080 mặc định cho .NET 8
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Tên file dll thường trùng với tên project
ENTRYPOINT ["dotnet", "ConnectDB.dll"]