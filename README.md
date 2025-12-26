# 📺🔔 OdidoAlerter
Receive an alert when your favorite show will be broadcast on television.

Odido Alerter allows you to create a RSS feed of Odido broadcasts that match the given seachterm in the broadcast name.

## ⚙️ Configuration
| Name       | Description |
| ---------- | ----------- |
| SearchTerm | Term on which to search and alert |
| Base_Url   | Url where the RSS feed will be hosted |
| Odido__UserId | Odido TV user id |
| Odido__ClientPassword | Odido TV password |
| Odido__DeviceId | Odido Device Id |
| Odido__DeviceModel | Odido Device Model |

## 🌐 Usage

docker-compose.yaml
```
services:
  odidoalerter:
    image: sboulema/odidoalerter
    container_name: odidoalerter
    restart: unless-stopped
    environment:
      BASE_URL: https://odidoalerter.example.com
      SEARCHTERM: Miraculous World
      Odido__UserId: <number>,
      Odido__ClientPassword: <number>,
      Odido__DeviceId: <string>,
      Odido__DeviceModel: <string>,
    ports:
      - 8080
    logging:
      options:
        max-size: 10m
```

## ⬇️ Dependencies
- .NET 10.0