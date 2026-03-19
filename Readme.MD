# SmartCV Analyzer API 🎯

> AI-powered REST API that analyzes resumes and provides actionable feedback for job seekers.  
> Built with .NET 9, Clean Architecture, and OpenAI GPT-4o.

![Build](https://github.com/YOUR_USERNAME/SmartCVAnalyzer/actions/workflows/build.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-ready-2496ED?logo=docker)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Project Structure](#-project-structure)
- [Running Tests](#-running-tests)
- [Configuration](#-configuration)

---

## 🔍 Overview

SmartCV Analyzer is a REST API that allows users to upload their CV in PDF format
and receive a detailed AI-generated analysis. The system extracts text from the PDF,
sends it to GPT-4o with a structured prompt, and returns scored feedback including
strengths, improvement suggestions, missing keywords, and an executive summary —
all tailored to a specific target industry.

**Who is it for?** Job seekers, freelancers, and recent graduates who want to
understand why their CVs get rejected by ATS systems and recruiters.

---

## ✨ Features

- 🔐 **JWT Authentication** — Register, login, and secure endpoints with Bearer tokens
- 📄 **PDF Parsing** — Extracts text from uploaded CVs using PdfPig
- 🤖 **AI Analysis** — Sends CV content to GPT-4o and returns structured feedback
- 📊 **Scoring System** — Overall score + per-section scores (Experience, Education, Skills, Format)
- 📝 **Analysis History** — Users can retrieve all their previous analyses
- 🛡️ **Rate Limiting** — Maximum 5 analyses per user per day
- ⚠️ **Global Error Handling** — Consistent JSON error responses across all endpoints
- ✅ **Input Validation** — FluentValidation on all requests (file size, type, format)
- 🐳 **Docker Ready** — Run the full stack with a single command

---

## 🏗️ Architecture

This project follows **Clean Architecture**, ensuring a clear separation of concerns
and making each layer independently testable.
```
┌─────────────────────────────────────────┐
│                   API                   │  Controllers, Middleware, Extensions
├─────────────────────────────────────────┤
│              Infrastructure             │  EF Core, OpenAI, PdfPig, JWT, BCrypt
├─────────────────────────────────────────┤
│              Application                │  CQRS Handlers, Interfaces, DTOs
├─────────────────────────────────────────┤
│                 Domain                  │  Entities, Value Objects, Exceptions
└─────────────────────────────────────────┘
```

**Dependency rule:** Each layer only depends on the layer directly below it.
Domain has zero external dependencies.

**Key patterns used:**
- **CQRS** with MediatR — Commands and Queries are fully separated
- **Repository + Unit of Work** — Data access abstracted behind interfaces
- **Result Pattern** — Errors returned as values, not thrown as exceptions
- **Options Pattern** — Strongly typed configuration with `IOptions<T>`
- **Factory Method** — Entities created only through validated factory methods

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 9 / ASP.NET Core |
| Database | SQL Server + Entity Framework Core 9 |
| AI Integration | OpenAI API (GPT-4o) |
| PDF Parsing | PdfPig |
| Authentication | JWT Bearer Tokens |
| Password Hashing | BCrypt.Net |
| CQRS | MediatR 12 |
| Validation | FluentValidation |
| API Docs | Scalar (OpenAPI 3.0) |
| Testing | xUnit + Moq + FluentAssertions |
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions |

---

## 🚀 Getting Started

### Option A — Docker (Recommended)

The easiest way to run the project. No need to install .NET or SQL Server.

**Prerequisites:** [Docker Desktop](https://www.docker.com/products/docker-desktop)
```bash
# 1. Clone the repository
git clone https://github.com/YOUR_USERNAME/SmartCVAnalyzer.git
cd SmartCVAnalyzer

# 2. Create your environment file
cp .env.example .env
```

Edit `.env` with your values:
```env
DB_PASSWORD=YourStrong@Password123
JWT_SECRET_KEY=your-secret-key-minimum-32-characters-long
OPENAI_API_KEY=sk-your-openai-api-key
```

> **No OpenAI key?** The project includes a mock AI service for testing.
> See [Configuration](#-configuration) for details.
```bash
# 3. Start everything
docker-compose up --build
```

The API will be available at:
- **API:** `http://localhost:8080`
- **Health check:** `http://localhost:8080/health`
- **Documentation:** `http://localhost:8080/scalar/v1`

---

### Option B — Local Development

**Prerequisites:**
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [OpenAI API Key](https://platform.openai.com/api-keys)
```bash
# 1. Clone the repository
git clone https://github.com/YOUR_USERNAME/SmartCVAnalyzer.git
cd SmartCVAnalyzer

# 2. Create your local config
# Create src/SmartCVAnalyzer.API/appsettings.Development.json
# based on appsettings.json and fill in your values

# 3. Apply database migrations
dotnet ef database update \
  --project src/SmartCVAnalyzer.Infrastructure/SmartCVAnalyzer.Infrastructure.csproj \
  --startup-project src/SmartCVAnalyzer.API/SmartCVAnalyzer.API.csproj

# 4. Run the API
dotnet run --project src/SmartCVAnalyzer.API/SmartCVAnalyzer.API.csproj
```

---

## 📡 API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/health` | ❌ | Health check |
| `POST` | `/api/v1/auth/register` | ❌ | Create a new account |
| `POST` | `/api/v1/auth/login` | ❌ | Login and get JWT token |
| `POST` | `/api/v1/cv/analyze` | ✅ | Upload CV PDF and get AI analysis |
| `GET` | `/api/v1/cv/history` | ✅ | Get all analyses for current user |
| `GET` | `/api/v1/analysis/{id}` | ✅ | Get full details of a specific analysis |

### Example Response — CV Analysis
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fileName": "john-doe-cv.pdf",
  "industryTarget": "software",
  "status": "Completed",
  "overallScore": 78,
  "feedback": {
    "overallScore": 78,
    "sectionScores": {
      "experience": 80,
      "education": 70,
      "skills": 85,
      "format": 75
    },
    "strengths": [
      "Clear demonstration of technical skills relevant to the industry",
      "Good progression of responsibilities across positions"
    ],
    "improvements": [
      "Add more measurable results and metrics to experience section",
      "Include relevant certifications for the target industry"
    ],
    "missingKeywords": ["CI/CD", "Docker", "Agile", "REST API"],
    "executiveSummary": "This CV shows solid foundational experience for the software industry. With targeted improvements in metrics and keywords, it has strong potential to pass ATS filters."
  },
  "createdAt": "2025-01-01T10:00:00Z"
}
```

---

## 📁 Project Structure
```
SmartCVAnalyzer/
├── src/
│   ├── SmartCVAnalyzer.Domain/
│   │   ├── Entities/            # User, CvAnalysis, BaseEntity
│   │   ├── ValueObjects/        # AnalysisScore
│   │   ├── Enums/               # AnalysisStatus
│   │   └── Exceptions/          # DomainException, NotFoundException
│   ├── SmartCVAnalyzer.Application/
│   │   ├── Features/            # CQRS Commands and Queries
│   │   │   ├── Auth/            # Register, Login
│   │   │   ├── UploadCv/        # Upload and analyze CV
│   │   │   ├── GetAnalysis/     # Get analysis by ID
│   │   │   └── GetUserHistory/  # Get user's analysis history
│   │   ├── Interfaces/          # Contracts for Infrastructure
│   │   ├── DTOs/                # Request and response models
│   │   └── Common/              # Result<T> pattern
│   ├── SmartCVAnalyzer.Infrastructure/
│   │   ├── Persistence/         # EF Core, repositories, Unit of Work
│   │   └── ExternalServices/    # OpenAI, PdfPig, JWT, BCrypt
│   └── SmartCVAnalyzer.API/
│       ├── Controllers/         # Auth, Cv, Analysis
│       ├── Middleware/          # Global exception handling
│       └── Extensions/          # JWT, CORS configuration
├── tests/
│   └── SmartCVAnalyzer.UnitTests/
│       ├── Domain/              # Value Object and Entity tests
│       └── Application/         # Handler and Validator tests
├── Dockerfile
├── docker-compose.yml
└── .env.example
```

---

## 🧪 Running Tests
```bash
# Run all unit tests
dotnet test tests/SmartCVAnalyzer.UnitTests/SmartCVAnalyzer.UnitTests.csproj

# Run with detailed output
dotnet test tests/SmartCVAnalyzer.UnitTests/SmartCVAnalyzer.UnitTests.csproj \
  --verbosity detailed
```

---

## ⚙️ Configuration

### OpenAI Mock Service

If you don't have an OpenAI API key, the project includes a mock service
that returns realistic sample data for testing.

To enable it, change one line in
`src/SmartCVAnalyzer.Infrastructure/DependencyInjection.cs`:
```csharp
// Use this for testing without an OpenAI API key:
services.AddScoped<IAiAnalysisService, MockAiAnalysisService>();

// Use this for production with real OpenAI:
// services.AddScoped<IAiAnalysisService, OpenAiService>();
```

### Environment Variables

| Key | Description | Required |
|-----|-------------|----------|
| `DB_PASSWORD` | SQL Server SA password | ✅ Docker only |
| `JWT_SECRET_KEY` | Secret key for JWT signing (min 32 chars) | ✅ |
| `OPENAI_API_KEY` | Your OpenAI API key | ✅ |
| `ConnectionStrings__DefaultConnection` | Full SQL Server connection string | ✅ Local only |

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
