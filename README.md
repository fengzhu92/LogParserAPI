# Log File Parsing Backend

This project is a backend implementation for parsing log files, built using .NET 8 with Visual Studio 2022 Community. The application exposes two REST API endpoints to upload log files and return structured data.

## Features
- Parses log files containing host, timestamp, request, reply code, and reply bytes information.
- Two endpoints available:
  - `/api/logs/host-summary`
  - `/api/logs/resource-summary`

## Technologies Used
- **.NET 8**: Backend framework for building web APIs.
- **ASP.NET Core**: To manage HTTP endpoints.
- **C#**: Main programming language for controllers.
- **Regex**: Regular expressions to parse log entries.

## NuGet Package Requirements

- Required NuGet packages will be automatically restored, but you can manually install them with:
  ```sh
  dotnet restore
  ```

  Below are the main NuGet packages used:
  - `Microsoft.NET.Test.Sdk`
  - `Swashbuckle.AspNetCore`
  - `xunit`
  - `xunit.runner.visualstudio`

## Setup and Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/fengzhu92/LogParserAPI
   cd FileParserAPI
   ```

2. Build the project:
   ```sh
   dotnet build
   ```

3. Run the project:
   ```sh
   dotnet run
   ```
3. Run the project unit tests:
   ```sh
   dotnet test
   ```
By default, the application will be available on `http://localhost:5105` and `https://localhost:7226`.

## API Endpoints

### 1. Parse Host Access Logs
- **URL**: `/api/logs/host-summary`
- **Method**: POST
- **Description**: Parses a log file where each line contains a log containing information about host, timestamp, request, reply code, and reply bytes.
- **Request**:
  - Form-data with key: `file`, containing the `.txt` or `.log` file to be parsed.
- **Response**: A list of host and its number of accesses to webserver, sorted, host with most accessees to webserver is on top.

### 2. Parse Resource Access Summary
- **URL**: `/api/resource-summary`
- **Method**: POST
- **Description**: Parses a resource log file and returns structured information including resources accessed, timestamps, and request details.
- **Request**:
  - Form-data with key: `file`, containing the `.txt` or `.log` file to be parsed.
- **Response**: A list of URI and the number of successful (reply code 200) GET accesses to it, sorted, URI with most incoming access is on top.

### Host Summary Example
```sh
curl -X 'POST' \
  'http://localhost:5105/api/logs/host-summary' \
  -H 'accept: */*' \
  -H 'Content-Type: multipart/form-data' \
  -F 'file=@epa-http.txt;type=text/plain'
```

### Resource Summary Example
```sh
curl -X 'POST' \
  'http://localhost:5105/api/logs/resource-summary' \
  -H 'accept: */*' \
  -H 'Content-Type: multipart/form-data' \
  -F 'file=@epa-http.txt;type=text/plain'
```

## Project Structure
- **`LogFileParserAPI`**
  - *Program.cs*: Entry point to set up the application, configure middleware, and map controllers.
  - *Controllers/LogController.cs*: Handles requests for parsing log files.
  - *Services/LogParserService.cs*: The logic or business layer for processing the requests.
  - *Models/LogEntry.cs*: The data model representing each entry in the log file.
  - *Models/Summeries.cs*: The data models representing each entry in the replies.
- **`LogFileParserAPI.Tests`**
  - *LogParserServiceTests.cs*: The unit tests for functions in logic layer.

## Error Handling
- If no file is uploaded, the API returns a message: "Please select a file to upload."
- The application expects valid log entries; malformed logs will be ignored.

