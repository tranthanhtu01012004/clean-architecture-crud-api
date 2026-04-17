1. Mô hình tổng quát
Kiến trúc: Clean Architecture + CQRS + Event-Driven.

Giao thức: REST API & gRPC.

Luồng dữ liệu: Client -> API -> SQL Server & RabbitMQ -> Worker -> MongoDB.

2. Cấu trúc Layer
Presentation: Controller, gRPC Service (Tiếp nhận request).

Application: MediatR (Command/Query), Handlers (Xử lý nghiệp vụ).

Domain: Entities (News, Menu, User) (Cốt lõi).

Infrastructure: EF Core, SQL Server, RabbitMQ Publisher (Hạ tầng).

Worker: Background Service (Consume event, ghi log).

3. Lưu trữ & Công nghệ
SQL Server: Dữ liệu chính (quan hệ).

MongoDB: Lưu worker_audit_logs (phi cấu trúc).

RabbitMQ: Message Broker trung chuyển sự kiện.

Backend: .NET 8, MediatR, JWT.

4. Quy trình thực thi (Flow)
Ghi: API -> MediatR -> SQL -> RabbitMQ.

Đọc: API/gRPC -> MediatR -> SQL.

Audit: RabbitMQ -> Worker -> MongoDB (worker_audit_logs).

5. Vận hành & Kiểm tra
Lệnh chạy: dotnet run cho từng project API và Worker.

Swagger: localhost:54422/swagger

RabbitMQ UI: localhost:15672

MongoDB: Kiểm tra collection worker_audit_logs trong DB CleanCrudDemoAuditDb.

6. Kết quả
Hệ thống xử lý CRUD chuẩn, bảo mật JWT, tách biệt luồng đọc/ghi (CQRS) và lưu log bất đồng bộ qua Worker Service.