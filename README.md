# Recruitment ATS

A full-stack Applicant Tracking System (ATS) built with .NET 10, Blazor Server, and SQLite.

## Features

- **Job Management**: Create, update, and manage job postings
- **Application Tracking**: Track candidate applications with status management
- **Resume Parsing**: Automatic extraction of text from PDF and DOCX resumes
- **ATS Scoring**: Intelligent keyword matching to score candidate resumes against job descriptions
- **Authentication & Authorization**: Role-based access control (Admin, Recruiter, Candidate)
- **Candidate Dashboard**: View application history and ATS scores
- **Admin Dashboard**: Manage jobs, view applications, and access parsed resumes
- **Email Notifications**: Automated confirmation emails (file-based for development)

## Tech Stack

- **Backend**: ASP.NET Core 10 Web API
- **Frontend**: Blazor Server
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **File Processing**: iTextSharp (PDF), DocumentFormat.OpenXml (DOCX)
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Testing**: xUnit, Moq, FluentAssertions
- **CI/CD**: GitHub Actions

## Architecture

The solution follows Clean Architecture principles:

- **Domain**: Core entities and enums
- **Application**: DTOs, interfaces, validators, and mappings
- **Infrastructure**: Data access, services, and external integrations
- **API**: RESTful API endpoints
- **WebUI**: Blazor Server UI
- **UnitTests**: Unit tests for services

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Git

### Installation

1. Clone the repository:
```bash
git clone https://github.com/MuhammadFasihArif/RecruitmentATS.git
cd RecruitmentATS
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Apply database migrations:
```bash
dotnet ef database update --project src/RecruitmentATS.Infrastructure --startup-project src/RecruitmentATS.Api
```

4. Run the API:
```bash
dotnet run --project src/RecruitmentATS.Api/RecruitmentATS.Api.csproj
```

5. Run the WebUI (in a separate terminal):
```bash
dotnet run --project src/RecruitmentATS.WebUI/RecruitmentATS.WebUI.csproj
```

6. Access the application:
   - WebUI: http://localhost:5275
   - API: http://localhost:5240/swagger

### Default Credentials

- **Email**: admin@recruitmentats.com
- **Password**: Admin@123

## Project Structure

```
RecruitmentATS/
├── src/
│   ├── RecruitmentATS.Domain/          # Core entities
│   ├── RecruitmentATS.Application/     # Business logic
│   ├── RecruitmentATS.Infrastructure/  # Data & services
│   ├── RecruitmentATS.Api/            # REST API
│   ├── RecruitmentATS.WebUI/          # Blazor UI
│   └── RecruitmentATS.UnitTests/      # Tests
├── .github/workflows/                  # CI/CD
├── docker-compose.yml                  # Docker setup
└── RecruitmentATS.sln                 # Solution file
```

## Key Features Explained

### ATS Scoring
The system analyzes resumes against job descriptions using keyword matching:
- Extracts keywords from job requirements
- Compares with parsed resume text
- Calculates match percentage (0-100%)
- Provides detailed feedback on matched/missing keywords

### Resume Parsing
Supports PDF and DOCX formats:
- Extracts text content automatically
- Stores parsed text in database
- Displays in admin dashboard

### Role-Based Access
- **Admin**: Full access to all features
- **Recruiter**: Manage jobs and view applications
- **Candidate**: Apply for jobs and track applications

## Testing

Run unit tests:
```bash
dotnet test
```

## Docker Support

Build and run with Docker:
```bash
docker-compose up
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.

## Contact

Muhammad Fasih Arif - fasiharif201@gmail.com

Project Link: [https://github.com/MuhammadFasihArif/RecruitmentATS](https://github.com/MuhammadFasihArif/RecruitmentATS)
