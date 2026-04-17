# 🚀 CleanCrudDemo - Microservice Notes

---

## 🧠 1. Tổng quan hệ thống

```mermaid
graph TD
    A[Client / Swagger] --> B[API (.NET)]
    B --> C[SQL Server]
    B --> D[RabbitMQ]
    D --> E[Worker Service]
    E --> F[MongoDB]

👉 Kiến trúc: Clean Architecture + Event-driven

⚙️ 2. Thành phần chính
🔹 API (.NET Core)
CRUD Menu, News
EF Core + LINQ
MediatR (CQRS)
Publish event → RabbitMQ
🔹 RabbitMQ
Message Broker
Trung gian giữa service
Không lưu dữ liệu lâu dài
🔹 Worker (BackgroundService)
Consume RabbitMQ
Parse JSON
Lưu MongoDB
🔹 MongoDB
Lưu audit log
NoSQL (document)
🔹 SQL Server
Lưu dữ liệu chính
Quan hệ many-to-many (Menu – News)
🔁 3. Flow hệ thống
1. POST /api/News
2. API → SQL Server
3. API → RabbitMQ (publish event)
4. Worker nhận message
5. Worker parse JSON
6. Worker lưu MongoDB
🧪 4. Lệnh chạy
▶️ Run API

dotnet run --project src\CleanCrudDemo.Api

▶️ Run Worker

dotnet run --project src\CleanCrudDemo.Worker

🧱 5. Test
Swagger → tạo News/Menu
RabbitMQ → http://localhost:15672
MongoDB → database: CleanCrudDemoAuditDb
🧠 6. Parse JSON
❌ Raw (sai)
{
  "eventRaw": "{...}"
}
✅ Structured (đúng)
{
  "event": "MenuCreated",
  "entityId": "...",
  "entityName": "Menu",
  "action": "CREATE",
  "payload": {...}
}
🔥 7. Mapping Event
Event	Action	Entity
NewsCreated	CREATE	News
MenuCreated	CREATE	Menu
NewsDeleted	DELETE	News
🧠 8. Kiến thức sử dụng
Clean Architecture
CQRS (MediatR)
EF Core + LINQ
RabbitMQ (Producer/Consumer)
BackgroundService
MongoDB
Event-driven architecture
💬 9. Câu phỏng vấn

Em sử dụng RabbitMQ để xử lý bất đồng bộ.
API publish event, Worker consume và lưu log vào MongoDB.

🎯 10. Tóm tắt nhanh
API → SQL
API → Rabbit
Rabbit → Worker
Worker → Mongo
🚀 11. Level đạt được
✔ CRUD
✔ CQRS
✔ RabbitMQ
✔ MongoDB
✔ Microservice cơ bản