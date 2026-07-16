# MultiAgentCodeReview
A multi-agent code review system that utilizes AI assistants to improve code quality and reduce manual review time.
[![Build Status](https://img.shields.io/badge/Build-Passing-green.svg)](https://github.com/bhavyananda17/MultiAgentCodeReview/actions)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/bhavyananda17/MultiAgentCodeReview/blob/master/LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Overview
The MultiAgentCodeReview system is designed to solve the problem of manual code reviews, which can be time-consuming and prone to human error. This project utilizes AI assistants to analyze code and provide feedback, reducing the need for manual reviews and improving overall code quality. The system is designed for development teams who want to improve their code review process and reduce the time spent on manual reviews.

Key features and capabilities of the system include:

* AI-powered code analysis and feedback
* Support for multiple programming languages
* Integration with popular version control systems
* Customizable pipeline for code review and analysis

The system is designed for development teams who want to improve their code review process and reduce the time spent on manual reviews.

## Architecture
```mermaid
graph LR
    A[Code Repository] -->|Push|> B[Version Control System]
    B -->|Trigger|> C[MCP Server]
    C -->|Request|> D[AI Assistant]
    D -->|Analysis|> E[Code Review Pipeline]
    E -->|Feedback|> F[Code Repository]
```
The high-level architecture of the system consists of the following components:

* Code Repository: The central repository for code storage and version control.
* Version Control System: The system used to manage code changes and trigger the code review pipeline.
* MCP Server: The server that manages the AI assistants and triggers the code review pipeline.
* AI Assistant: The AI-powered component that analyzes code and provides feedback.
* Code Review Pipeline: The customizable pipeline that defines the code review and analysis process.

The project structure is organized into the following directories:

* `MultiAgentCodeReview.Agents`: Contains the AI assistant implementations.
* `MultiAgentCodeReview.Core`: Contains the core logic for the code review pipeline.
* `MultiAgentCodeReview.Host`: Contains the host application for the MCP server.
* `MultiAgentCodeReview.Orchestration`: Contains the pipeline definitions for code review and analysis.

The data flow through the system is as follows:

1. Code changes are pushed to the version control system.
2. The version control system triggers the MCP server.
3. The MCP server requests the AI assistant to analyze the code.
4. The AI assistant analyzes the code and provides feedback.
5. The feedback is sent to the code review pipeline.
6. The code review pipeline processes the feedback and updates the code repository.

Key design decisions and patterns used in the system include:

* Microservices architecture for scalability and flexibility.
* Event-driven architecture for decoupling components.
* Pipeline-based architecture for customizable code review and analysis.

## Tech Stack
The system utilizes the following frameworks, libraries, and tools:

* .NET 8.0
* ASP.NET Core 8.0
* Entity Framework Core 8.0
* ML.NET 2.0
* GitHub Actions for CI/CD

## Prerequisites
The following SDKs and tools are required to run the system:

* .NET 8.0 SDK
* ASP.NET Core 8.0
* Entity Framework Core 8.0
* ML.NET 2.0
* GitHub Actions

The following external services are required:

* Version control system (e.g. GitHub, GitLab)
* MCP server (e.g. Azure Functions, AWS Lambda)

## Getting Started
To get started with the system, follow these steps:

1. Clone the repository: `git clone https://github.com/bhavyananda17/MultiAgentCodeReview.git`
2. Install the required SDKs and tools: `dotnet tool install -g dotnet-ef`
3. Configure the environment variables: `cp .env.example .env`
4. Run the system: `dotnet run`

## Configuration
The system uses the following configuration options:

* `MCP_SERVER_URL`: The URL of the MCP server.
* `AI_ASSISTANT_URL`: The URL of the AI assistant.
* `CODE_REPOSITORY_URL`: The URL of the code repository.
* `VERSION_CONTROL_SYSTEM_URL`: The URL of the version control system.

The following environment variables are required:

| Variable | Description | Example Value |
| --- | --- | --- |
| MCP_SERVER_URL | The URL of the MCP server | `https://example.com/mcp` |
| AI_ASSISTANT_URL | The URL of the AI assistant | `https://example.com/ai` |
| CODE_REPOSITORY_URL | The URL of the code repository | `https://example.com/repo` |
| VERSION_CONTROL_SYSTEM_URL | The URL of the version control system | `https://example.com/vcs` |

An example `.env` file is provided in the repository:
```makefile
MCP_SERVER_URL=https://example.com/mcp
AI_ASSISTANT_URL=https://example.com/ai
CODE_REPOSITORY_URL=https://example.com/repo
VERSION_CONTROL_SYSTEM_URL=https://example.com/vcs
```

## Usage
The system provides the following CLI commands:

* `dotnet run`: Runs the system.
* `dotnet test`: Runs the unit tests.

The system also provides a MCP server setup for AI assistants:

* `dotnet run --mcp-server`: Runs the MCP server.

Common workflows include:

* Pushing code changes to the version control system.
* Triggering the MCP server to analyze the code.
* Reviewing the feedback provided by the AI assistant.

## API Reference
The system provides the following key interfaces:

* `IAIAssistant`: The interface for the AI assistant.
* `ICodeReviewPipeline`: The interface for the code review pipeline.

The system also provides the following API endpoints:

* `POST /api/analyze`: Analyzes the code and provides feedback.
* `GET /api/feedback`: Retrieves the feedback provided by the AI assistant.

## Development
To add new agents, follow these steps:

1. Create a new class that implements the `IAIAssistant` interface.
2. Register the new agent in the `Startup.cs` file.
3. Update the `appsettings.json` file to include the new agent.

To modify the pipeline, follow these steps:

1. Create a new class that implements the `ICodeReviewPipeline` interface.
2. Register the new pipeline in the `Startup.cs` file.
3. Update the `appsettings.json` file to include the new pipeline.

The system uses the following testing approach:

* Unit tests: Test individual components and interfaces.
* Integration tests: Test the interactions between components.
* End-to-end tests: Test the entire system.

## Troubleshooting
Common issues and solutions include:

* `MCP_SERVER_URL` is not set: Set the `MCP_SERVER_URL` environment variable.
* `AI_ASSISTANT_URL` is not set: Set the `AI_ASSISTANT_URL` environment variable.
* Code analysis fails: Check the AI assistant logs for errors.

Rate limiting considerations include:

* Limiting the number of requests to the MCP server.
* Limiting the number of requests to the AI assistant.

## Contributing
The system uses the following code style and conventions:

* C# 8.0
* ASP.NET Core 8.0
* Entity Framework Core 8.0

The PR process includes:

1. Create a new branch for the feature or bug fix.
2. Implement the feature or bug fix.
3. Run the unit tests and integration tests.
4. Create a PR and assign it to a reviewer.
5. Review the PR and provide feedback.
6. Merge the PR into the main branch.