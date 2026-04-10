# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file và restore
COPY ["ConnectDB/ConnectDB.csproj", "ConnectDB/"]
RUN dotnet restore "ConnectDB/ConnectDB.csproj"

# Copy toàn bộ mã nguồn
COPY . .

# Build dự án
WORKDIR "/src/ConnectDB"
RUN dotnet publish "ConnectDB.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run (Sử dụng bản runtime ổn định)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Cài đặt thư viện ICU để tránh lỗi Globalization (Nguyên nhân phổ biến gây lỗi 139)
RUN apt-get update && apt-get install -y libicu-dev && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Cấu hình môi trường
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 8080

# ĐẢM BẢO: "ConnectDB.dll" phải viết đúng chữ hoa/thường y hệt tên project của bạn
ENTRYPOINT ["dotnet", "ConnectDB.dll"]