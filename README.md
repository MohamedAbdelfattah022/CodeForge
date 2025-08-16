<h1 align="center">CodeForge</h1>

<p align="center">
  <a href="#about">About</a> &#xa0; | &#xa0; 
  <a href="#features">Features</a> &#xa0; | &#xa0;
  <a href="#technologies">Technologies</a> &#xa0; | &#xa0;
  <a href="#getting-started">Getting Started</a> &#xa0; | &#xa0;
  <a href="#contributing">Contributing</a> &#xa0; | &#xa0;
  <a href="#license">License</a> &#xa0; | &#xa0;
  <a href="https://github.com/MohamedAbdelfattah022" target="_blank">Author</a>
</p>

<br>

## About

Codeforge is an Online Judge System designed to facilitate coding practice. It allows users to create, manage, and solve
coding problems with various constraints and difficulty levels. The system supports user authentication, problem
submission, and evaluation of solutions against predefined test cases.

## Features

- **Problem Management**: Create, update, and manage coding problems with associated constraints, difficulty levels, and
  tags
- **Submission Handling**: Users can submit solutions to problems, which are evaluated for correctness and performance
- **User Management**: User authentication and role-based access control using ASP.NET Core Identity
- **Tag System**: Problems can be tagged for better categorization and searchability
- **Test Cases**: Define input/output test cases for problems to validate submissions
- **Multi-Language Support**: Supports C++, Python, and C# programming languages
- **Asynchronous Processing**: Utilizes RabbitMQ and background jobs for handling long-running tasks
- **File Storage**: Integration with Supabase for secure file storage and management
- **API Documentation**: Interactive API documentation using Scalar and Swagger
- **Comprehensive Testing**: Extensive unit testing with xUnit, NSubstitute, and FluentAssertions

## Technologies

### Backend Framework

- **.NET 9.0**: The latest version of the .NET Framework
- **ASP.NET Core**: For building the web API with modern features
- **Entity Framework Core**: For database management and migrations
- **SQL Server**: Primary database for data persistence

### Validation & Logging

- **FluentValidation**: For comprehensive input validation
- **Serilog**: Structured logging with multiple sinks (Console, File, Seq)

### Messaging & Storage

- **RabbitMQ**: Message queuing for asynchronous task processing
- **Supabase**: Cloud storage for file management and test cases

### Testing

- **xUnit**: Unit testing framework for .NET
- **NSubstitute**: For creating mock objects in tests
- **FluentAssertions**: For more expressive assertions in tests
- **AutoFixture**: For generating test data

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server
- Docker and Docker Compose
- RabbitMQ (via Docker)

### Setup

1. Clone the repository
2. Provide your connection strings and other configurations in `appsettings.json` or via User Secrets

3. Run database migrations:
4. Start the development docker services:
   ```bash
   docker-compose up -d
   ```
5. Run the application:
   ```bash
   dotnet run --project src/Codeforge.Api
   ```

### Docker Services

The project includes Docker Compose configuration for:

- **RabbitMQ**: Message queuing service
- **C++ Environment**: GCC 13 for C++ code execution
- **Python Environment**: Python 3.12 for Python code execution
- **C# Environment**: .NET 9.0 SDK for C# code execution

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is under MIT license. For more details, see the [LICENSE](LICENSE) file.

Made with ❤️ by <a href="https://github.com/MohamedAbdelfattah022" target="_blank">Mohamed Abdelfattah</a>

<a href="#top">Back to top</a>