# 🌐 Sociala Backend

**Sociala** is a social networking platform built in just **25 days** using the **Onion Architecture**.  
The project includes a wide range of social media features — from authentication and posts to real-time chat and group interactions.  

> ⚙️ This was one of my first large-scale backend projects. While I now see ways to make it more optimized, I challenged myself to build a fully functional, modular system under tight deadlines — and delivered it within 25 days.

---

## 🚀 Features

Sociala implements a variety of social media functionalities:

- 👤 **Authentication & Authorization** — Secure user management and login flow  
- 🤝 **Followers & Following** — Create and manage social connections  
- 📝 **Posts & Feeds** — Share updates and view others’ posts  
- ❤️ **Likes & Comments** — Interact with content  
- 💬 **Real-Time Chats** — Built using **SignalR** for instant communication  
- 👥 **Groups & Roles** — Create groups, assign roles, and manage memberships  
- 🕒 **Stories & Archives** — Temporary content and archived items  
- 🔷 **Verification Requests** — Users can apply for account verification  

---

## 🏗️ Architecture & Patterns

### 🧱 Onion Architecture
A layered approach that enforces separation of concerns and maintainability.

### 🧩 Design Patterns Used
- **Repository Pattern** — Abstracts data access logic  
- **Bridge Pattern** — Decouples abstraction from implementation  

---

## 🛠️ Tech Stack

| Layer / Component | Technology |
|--------------------|-------------|
| **Framework** | .NET API |
| **Architecture** | Onion Architecture |
| **ORM** | Entity Framework Core |
| **Real-Time Communication** | SignalR |
| **Validation** | FluentValidation |
| **Configuration** | Fluent API |
| **Database** | SQL-based (via EF Core) |
| **Patterns** | Repository, Bridge |

---

## ⚡ Key Highlights

- Fully modular structure with clear separation of domains  
- Real-time communication layer with SignalR  
- Complex social logic (followers, groups, roles, etc.)  
- Clean validation and database configuration with Fluent APIs  
- Built under strict time constraints (25 days)  

---

## 📚 Future Improvements

- Improve overall performance and scalability  
- Introduce CQRS and MediatR for cleaner separation  
- Add unit and integration testing  
- Optimize database queries  

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
