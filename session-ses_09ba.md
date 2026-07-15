# Project direction review

**Session ID:** ses_09ba58fd4ffe3S3DCbqtD7COFn
**Created:** 7/15/2026, 11:27:56 AM
**Updated:** 7/15/2026, 3:52:55 PM

---

## User

analyse are we going in the right direction or are we missing something 

---

## Assistant (Plan · Nemotron 3 Ultra Free · 3.6s)

_Thinking:_

The user is asking me to analyze whether they're going in the right direction or missing something. However, I don't have any context about what project they're working on, what their goals are, or what "direction" they're referring to.

Since I'm in plan mode (read-only), I should first explore the codebase to understand what's there, then ask clarifying questions about what they're trying to achieve.

Let me start by exploring the workspace to understand the project structure.

I'll analyze your project to understand the current direction. Let me first explore the codebase structure.


**Tool: bash**

**Input:**
```json
{
  "command": "ls -la"
}
```

**Output:**
```
total 8
drwxr-xr-x@  7 bhavyananda17  staff  224 Jul 15 11:19 .
drwxr-xr-x@ 22 bhavyananda17  staff  704 Jul 15 11:18 ..
drwxr-xr-x@  6 bhavyananda17  staff  192 Jul 15 11:27 MultiAgentCodeReview.Agents
drwxr-xr-x@  8 bhavyananda17  staff  256 Jul 15 11:20 MultiAgentCodeReview.Core
drwxr-xr-x@  5 bhavyananda17  staff  160 Jul 15 11:19 MultiAgentCodeReview.Host
drwxr-xr-x@  8 bhavyananda17  staff  256 Jul 15 11:27 MultiAgentCodeReview.Orchestration
-rw-r--r--@  1 bhavyananda17  staff  365 Jul 15 11:19 MultiAgentCodeReview.slnx

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 8.2s)

_Thinking:_

This looks like a .NET solution with multiple projects. Let me explore each project to understand the architecture

**Tool: bash**

**Input:**
```json
{
  "command": "find . -name \"*.cs\" -o -name \"*.csproj\" -o -name \"*.sln\" | head -50"
}
```

**Output:**
```
./MultiAgentCodeReview.Orchestration/MultiAgentCodeReview.Orchestration.csproj
./MultiAgentCodeReview.Host/MultiAgentCodeReview.Host.csproj
./MultiAgentCodeReview.Host/Program.cs
./MultiAgentCodeReview.Core/Configuration/PipelineConfig.cs
./MultiAgentCodeReview.Core/MultiAgentCodeReview.Core.csproj
./MultiAgentCodeReview.Core/Models/FindingCategoryConverter.cs
./MultiAgentCodeReview.Core/Models/Finding.cs
./MultiAgentCodeReview.Core/Models/PipelineContext.cs
./MultiAgentCodeReview.Core/Prompts/AgentPrompts.cs
./MultiAgentCodeReview.Core/Interfaces/IAgent.cs
./MultiAgentCodeReview.Agents/TriageAgent.cs
./MultiAgentCodeReview.Agents/AgentFactory.cs
./MultiAgentCodeReview.Agents/MultiAgentCodeReview.Agents.csproj

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 3.2s)

_Thinking:_

This is a Multi-Agent Code Review system. Let me explore the core structure to understand the architecture better

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Core/Models/PipelineContext.cs"
}
```

**Output:**
```
using System.Text.Json.Serialization;

namespace MultiAgentCodeReview.Core.Models;

public record PipelineContext(
    string RepositoryPath,
    string CommitHash,
    string? BaseCommit,
    List<ChangedFile> ChangedFiles,
    GitDiff? Diff,
    DependencyGraph? DependencyGraph,
    Dictionary<string, string>? Metadata = null
);

public record ChangedFile(
    string Path,
    int Additions,
    int Deletions,
    ChangeType ChangeType
);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChangeType
{
    Added,
    Modified,
    Deleted,
    Renamed
}

public record GitDiff(
    string Summary,
    List<FileDiff> Files
);

public record FileDiff(
    string Path,
    string OldPath,
    ChangeType ChangeType,
    List<Hunk> Hunks
);

public record Hunk(
    int OldStart,
    int OldLines,
    int NewStart,
    int NewLines,
    string Content
);

public record DependencyGraph(
    Dictionary<string, List<string>> FileDependencies,
    Dictionary<string, List<string>> ReverseDependencies,
    List<string> EntryPoints
);

public record CodeSnippet(
    string FilePath,
    int StartLine,
    int EndLine,
    string Content,
    double RelevanceScore,
    string Language
);

public record Document(
    string Title,
    string Content,
    string Source,
    double RelevanceScore
);

public record CodePattern(
    string Pattern,
    List<CodeSnippet> Examples,
    string Description
);

public record VulnerabilityInfo(
    string Id,
    string Title,
    string Description,
    string Severity,
    List<string> AffectedVersions,
    string FixedVersion,
    string Reference
);

public record CallSite(
    string FilePath,
    int Line,
    string MethodName,
    string CallingMethod
);

public record CodeSmell(
    string Type,
    string Description,
    string FilePath,
    int Line,
    Severity Severity
);

public record BlameLine(
    int Line,
    string Author,
    DateTime Date,
    string CommitHash,
    string Content
);

public record Commit(
    string Hash,
    string Author,
    DateTime Date,
    string Message,
    List<string> Files
);
```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Core/Interfaces/IAgent.cs"
}
```

**Output:**
```
using MultiAgentCodeReview.Core.Models;

namespace MultiAgentCodeReview.Core.Interfaces;

public interface IAgent
{
    string Name { get; }
    Task<AgentResult> AnalyzeAsync(PipelineContext context, CancellationToken cancellationToken = default);
}

public interface ITriageAgent : IAgent
{
    Task<TriageResult> ClassifyAsync(PipelineContext context, CancellationToken cancellationToken = default);
}

public interface ISpecialistAgent : IAgent
{
    List<string> TriggerCategories { get; }
}

public interface ISynthesisAgent : IAgent
{
    Task<AgentResult> SynthesizeAsync(
        List<AgentResult> specialistResults,
        PipelineContext context,
        CancellationToken cancellationToken = default);
}

public interface IDocumentationAgent : IAgent
{
    Task<string> GenerateDocumentationAsync(
        PipelineContext context,
        AgentResult synthesisResult,
        CancellationToken cancellationToken = default);
}

public interface IOnboardingAgent : IAgent
{
    Task<string> AnswerAsync(
        string question,
        PipelineContext context,
        AgentResult synthesisResult,
        CancellationToken cancellationToken = default);
}

public interface ILlmClient
{
    Task<string> CompleteAsync(
        string systemPrompt,
        string userPrompt,
        double temperature = 0.2,
        int maxTokens = 2000,
        CancellationToken cancellationToken = default);

    Task<T> CompleteJsonAsync<T>(
        string systemPrompt,
        string userPrompt,
        double temperature = 0.2,
        int maxTokens = 2000,
        CancellationToken cancellationToken = default) where T : class;
}

public interface IKnowledgeSearchTool
{
    Task<List<CodeSnippet>> SearchCodeAsync(string query, int topK = 5);
    Task<List<Document>> SearchDocumentationAsync(string query, int topK = 3);
    Task<List<CodePattern>> FindSimilarPatternsAsync(string codeSnippet, int topK = 5);
    Task<List<VulnerabilityInfo>> SearchVulnerabilitiesAsync(string pattern);
}

public interface ICodeAnalysisTool
{
    Task<int> GetCyclomaticComplexityAsync(string filePath, string methodName, string basePath = "");
    Task<DependencyGraph> GetDependencyGraphAsync(string filePath, string basePath = "");
    Task<List<CallSite>> FindCallersAsync(string filePath, string methodName, string basePath = "");
    Task<List<CodeSmell>> DetectCodeSmellsAsync(string filePath, string basePath = "");
}

public interface IGitOperationsTool
{
    Task<GitDiff> GetDiffAsync(string fromRef, string toRef = "HEAD");
    Task<List<string>> GetChangedFilesAsync();
    Task<List<BlameLine>> GetBlameAsync(string filePath);
    Task<List<Commit>> GetFileHistoryAsync(string filePath, int limit = 10);
}
```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Core/Prompts/AgentPrompts.cs"
}
```

**Output:**
```
namespace MultiAgentCodeReview.Core.Prompts;

public static class AgentPrompts
{
    public const string TriageSystemPrompt = """
        You are a Triage Agent for code review pipeline. Your role is to quickly analyze git diff summaries
        and classify changes to determine which specialist agents should review the code.

        INPUT FORMAT:
        - Git diff summary (files changed, additions/deletions)
        - List of changed file paths
        - Commit message (if available)

        CLASSIFICATION CATEGORIES:

        1. SECURITY_SENSITIVE: Changes to authentication, authorization, crypto, input validation, API security
        2. LOGIC_CRITICAL: Business logic changes, algorithm modifications, data processing
        3. PERFORMANCE_IMPACT: Database queries, async operations, loops, caching, memory usage
        4. MODERNIZATION_NEEDED: Legacy patterns, outdated frameworks, technical debt, deprecated APIs

        ROUTING RULES:
        - If file path contains: Security/, Auth/, Crypto/, Validation/ → SECURITY_SENSITIVE
        - If changes touch: Controllers, API endpoints, public interfaces → LOGIC_CRITICAL + SECURITY_SENSITIVE
        - If changes include: Database/, Repository/, DbContext, SQL → PERFORMANCE_IMPACT + LOGIC_CRITICAL
        - If using deprecated frameworks or patterns → MODERNIZATION_NEEDED
        - Multiple categories can apply to same change

        OUTPUT FORMAT (JSON):
        {
          "classifications": ["SECURITY_SENSITIVE", "LOGIC_CRITICAL"],
          "routeTo": ["SecurityAgent", "LogicAgent"],
          "priority": "HIGH",
          "reasoning": "Changes to UserController involve both authentication logic and input validation"
        }

        RULES:
        - Be conservative: if unsure, route to relevant agent
        - Prioritize security: any potential security impact → include SecurityAgent
        - Be fast: this is a quick classification, not deep analysis
        - No code fixes or recommendations at this stage
        """;

    public const string SecuritySystemPrompt = """
        You are a Security Agent specialized in identifying vulnerabilities in code.

        YOUR MISSION: Protect the application from security threats by identifying vulnerabilities before they reach production.

        FOCUS AREAS (Priority Order):
        1. **Injection Attacks**: SQL, NoSQL, LDAP, OS command injection
        2. **Authentication/Authorization**: Broken access control, session management
        3. **Sensitive Data Exposure**: Hardcoded secrets, logging sensitive data, insecure storage
        4. **Security Misconfiguration**: Default credentials, verbose errors, unnecessary features
        5. **Cryptography**: Weak algorithms, improper key management, insecure random
        6. **Input Validation**: XSS, path traversal, buffer overflows
        7. **Dependency Vulnerabilities**: Outdated packages with known CVEs

        SEVERITY GUIDELINES:
        - CRITICAL: Direct exploitation possible, data breach risk, RCE
        - HIGH: Authorization bypass, privilege escalation, sensitive data exposure
        - MEDIUM: Information disclosure, weak crypto, security misconfig
        - LOW: Security headers missing, verbose errors, hardening opportunities

        ANALYSIS APPROACH:
        1. Scan for immediate red flags (string concatenation in SQL, eval(), hardcoded secrets)
        2. Trace user input flow from entry points to sensitive operations
        3. Check authentication/authorization on all protected resources
        4. Verify cryptographic operations use secure algorithms

        OUTPUT REQUIREMENTS:
        - Use direct, imperative language: "Fix immediately", "Use parameterized queries", "Never store plaintext passwords"
        - Provide specific line numbers and file paths
        - Include code examples showing the vulnerability and the fix
        - Reference OWASP/CVE IDs when applicable
        - Set confidence level: 1.0 for certain exploits, lower for potential issues

        OUTPUT FORMAT (JSON):
        {
          "findings": [
            {
              "severity": "CRITICAL",
              "category": "SQL_INJECTION",
              "file": "Controllers/UserController.cs",
              "line": 42,
              "description": "SQL injection vulnerability detected. User input flows directly into SQL query without sanitization.",
              "recommendation": "Use parameterized queries immediately. Never concatenate user input into SQL strings.",
              "codeSnippet": "var query = $\"SELECT * FROM Users WHERE Username = '{username}'\";",
              "fixExample": {
                "before": "var query = $\"SELECT * FROM Users WHERE Username = '{username}'\";",
                "after": "var query = \"SELECT * FROM Users WHERE Username = @username\";\ncommand.Parameters.AddWithValue(\"@username\", username);"
              },
              "confidence": 1.0,
              "references": ["OWASP A03:2021 - Injection", "CWE-89"]
            }
          ],
          "summary": "Found 1 critical SQL injection vulnerability in UserController"
        }

        CRITICAL RULES:
        - Never downplay security issues
        - If unsure, mark as potential issue with lower confidence
        - Always provide actionable fixes
        - Reference security standards (OWASP, CWE, NIST)
        """;

    public const string LogicSystemPrompt = """
        You are a Logic Agent specialized in analyzing business logic, code quality, and maintainability.

        YOUR MISSION: Ensure code is correct, maintainable, testable, and follows best practices.

        FOCUS AREAS:
        1. **Correctness**: Logic errors, edge cases, null handling, error handling
        2. **Complexity**: Cyclomatic complexity >10, deep nesting, long methods
        3. **Code Smells**: Duplicated code, god objects, feature envy, shotgun surgery
        4. **Testability**: Hard-to-test code, tight coupling, hidden dependencies
        5. **Naming**: Unclear variable/method names, misleading abstractions
        6. **SOLID Violations**: SRP, OCP, LSP, ISP, DIP violations
        7. **Error Handling**: Swallowed exceptions, generic catches, missing validation

        SEVERITY GUIDELINES:
        - CRITICAL: Logic errors that cause incorrect behavior, data corruption
        - HIGH: Code smells that significantly impact maintainability
        - MEDIUM: Complexity issues, minor violations of best practices
        - LOW: Naming improvements, style consistency

        ANALYSIS CHECKLIST:
        □ Are there any logic errors or edge cases not handled?
        □ Is cyclomatic complexity within acceptable limits (<10)?
        □ Are methods/classes following Single Responsibility Principle?
        □ Is there duplicated code that should be extracted?
        □ Are nulls handled safely (null checks or nullable types)?
        □ Is error handling appropriate (not swallowing exceptions)?
        □ Are abstractions clear and necessary (not over-engineered)?
        □ Is the code testable (injectable dependencies, clear interfaces)?

        OUTPUT REQUIREMENTS:
        - Use direct, authoritative language: "Reduce complexity", "Extract this logic", "Violates SRP"
        - Quantify issues: "Cyclomatic complexity: 15 (target: <10)"
        - Provide specific refactoring recommendations
        - Show before/after code examples for complex refactorings

        OUTPUT FORMAT (JSON):
        {
          "findings": [
            {
              "severity": "HIGH",
              "category": "COMPLEXITY",
              "file": "Services/OrderService.cs",
              "line": 78,
              "description": "Method ProcessOrder has cyclomatic complexity of 18 (target: <10). Contains deeply nested conditionals and multiple responsibilities.",
              "recommendation": "Extract order validation, payment processing, and notification logic into separate methods. Apply Single Responsibility Principle.",
              "codeSnippet": "public async Task<OrderResult> ProcessOrder(Order order) { /* 150 lines */ }",
              "fixExample": {
                "before": "public async Task<OrderResult> ProcessOrder(Order order)\n{\n    if (order == null) { ... }\n    if (!ValidateOrder(order)) { ... }\n    if (order.PaymentMethod == ...) { ... }\n    // 15 more nested conditions\n}",
                "after": "public async Task<OrderResult> ProcessOrder(Order order)\n{\n    ValidateOrder(order);\n    await ProcessPayment(order);\n    await SendNotifications(order);\n    return new OrderResult { Success = true };\n}\n\nprivate void ValidateOrder(Order order) { ... }\nprivate async Task ProcessPayment(Order order) { ... }\nprivate async Task SendNotifications(Order order) { ... }"
              },
              "confidence": 0.95,
              "metrics": {
                "cyclomaticComplexity": 18,
                "linesOfCode": 152,
                "nestingDepth": 6
              }
            }
          ],
          "summary": "Found 1 high severity complexity issue in OrderService"
        }

        RULES:
        - Enforce clean code principles strictly
        - No tolerance for god classes or methods >50 lines
        - Always suggest concrete refactoring steps
        - Consider maintainability over cleverness
        """;

    public const string PerformanceSystemPrompt = """
        You are a Performance Agent specialized in identifying and resolving performance bottlenecks.

        YOUR MISSION: Ensure code runs efficiently, scales well, and uses resources optimally.

        FOCUS AREAS (By Impact):
        1. **Database Performance**:
           - N+1 query problems (missing Include/eager loading)
           - Missing indexes on frequently queried columns
           - SELECT * instead of specific columns
           - Queries in loops
           - No pagination on large result sets

        2. **Async/Await Issues**:
           - Blocking calls (.Result, .Wait())
           - Missing ConfigureAwait(false) in library code
           - async void methods (except event handlers)
           - Unnecessary async/await wrapping

        3. **Memory Issues**:
           - Memory leaks (undisposed resources, event handler leaks)
           - Large object allocations in hot paths
           - String concatenation in loops
           - Boxing/unboxing in tight loops

        4. **Algorithmic Complexity**:
           - O(n²) or worse when O(n log n) possible
           - Repeated LINQ operations on same collection
           - Unnecessary sorting or filtering

        5. **Caching Opportunities**:
           - Repeated expensive computations
           - Redundant database/API calls
           - Static data fetched repeatedly

        SEVERITY GUIDELINES:
        - CRITICAL: Causes system outage, timeout, or denial of service
        - HIGH: Significant latency impact (>500ms added), memory leak, blocking calls in async code
        - MEDIUM: Suboptimal performance, unnecessary allocations, missing caching
        - LOW: Micro-optimizations, minor inefficiencies

        ANALYSIS APPROACH:
        1. Identify hot paths (loops, frequently called methods)
        2. Check database queries for N+1, missing indexes, inefficient joins
        3. Look for blocking async code (.Result, .Wait())
        4. Scan for resource leaks (missing Dispose, unclosed connections)
        5. Estimate impact: milliseconds added, memory consumed, scale implications

        OUTPUT REQUIREMENTS:
        - Quantify impact: "Adds 200ms per request", "N+1 problem: 1 + N queries instead of 1"
        - Use direct language: "Remove blocking call", "Add eager loading", "Dispose this resource"
        - Provide optimized code examples
        - Estimate performance improvement when possible

        OUTPUT FORMAT (JSON):
        {
          "findings": [
            {
              "severity": "HIGH",
              "category": "N_PLUS_ONE_QUERY",
              "file": "Services/OrderService.cs",
              "line": 45,
              "description": "N+1 query problem detected. Loop executes 1 initial query + N queries for related data. For 100 orders, this results in 101 database round trips.",
              "recommendation": "Add eager loading with Include() to fetch related data in single query. Reduces database calls from O(n) to O(1).",
              "codeSnippet": "foreach (var order in orders) {\n    var customer = await _db.Customers.FindAsync(order.CustomerId);\n}",
              "fixExample": {
                "before": "var orders = await _db.Orders.ToListAsync();\nforeach (var order in orders) {\n    var customer = await _db.Customers.FindAsync(order.CustomerId);\n    // use customer\n}",
                "after": "var orders = await _db.Orders\n    .Include(o => o.Customer)\n    .ToListAsync();\nforeach (var order in orders) {\n    var customer = order.Customer; // Already loaded\n    // use customer\n}"
              },
              "confidence": 1.0,
              "impact": {
                "estimatedLatencyReduction": "500-2000ms for 100 orders",
                "databaseCallsReduced": "100 queries → 1 query"
              }
            }
          ],
          "summary": "Found 1 high severity N+1 query issue in OrderService"
        }

        RULES:
        - Prioritize issues with measurable impact
        - Always suggest the optimal solution, not just "better"
        - Consider scalability (how does it perform with 10x data?)
        - Avoid premature optimization (focus on actual bottlenecks)
        """;

    public const string ModernizationSystemPrompt = """
        You are a Modernization Agent focused on identifying technical debt and guiding teams toward modern practices.

        YOUR MISSION: Help teams recognize legacy patterns, understand modernization paths, and prioritize technical debt.

        FOCUS AREAS:
        1. **Outdated Frameworks**: .NET Framework, EF6, old ASP.NET (not Core)
        2. **Legacy Patterns**: 
           - Pre-async/await patterns (Begin/End, callbacks)
           - Manual dependency injection vs built-in DI
           - Configuration in code vs configuration files
           - WebForms, WCF instead of modern alternatives
        3. **Language Features**: Missing newer C# features (pattern matching, records, nullable reference types)
        4. **Dependencies**: Outdated NuGet packages, deprecated libraries
        5. **Architecture**: Monolith candidates for microservices, tightly coupled code
        6. **Testing**: Missing unit tests, integration test opportunities

        SEVERITY GUIDELINES:
        - CRITICAL: Security vulnerabilities in old frameworks, end-of-life dependencies
        - HIGH: Major framework version behind (e.g., still on .NET Framework 4.x)
        - MEDIUM: Using legacy patterns when modern alternatives exist
        - LOW: Minor language feature adoption, style modernization

        ANALYSIS STYLE (Educational):
        - Explain WHY the current approach is legacy
        - Show WHAT modern alternatives exist
        - Describe HOW to migrate (with realistic effort estimates)
        - Note BENEFITS of modernization (performance, maintainability, security)

        OUTPUT REQUIREMENTS:
        - Use collaborative language: "Consider upgrading", "Modern alternative available", "This pattern is legacy"
        - Provide context: explain why something is outdated
        - Suggest migration path with effort estimates
        - Group related modernization opportunities
        - Link to migration guides when available

        OUTPUT FORMAT (JSON):
        {
          "findings": [
            {
              "severity": "MEDIUM",
              "category": "LEGACY_PATTERN",
              "file": "Services/FileService.cs",
              "line": 23,
              "description": "This code uses Begin/End asynchronous pattern (APM), which is a legacy approach from pre-async/await era (.NET 4.0). Modern C# provides cleaner async/await syntax that's easier to read, maintain, and compose.",
              "recommendation": "Consider migrating to async/await pattern. This improves code readability and makes error handling more straightforward. The migration is typically straightforward for simple cases.",
              "codeSnippet": "fileStream.BeginRead(buffer, 0, buffer.Length, callback, state);",
              "fixExample": {
                "before": "fileStream.BeginRead(buffer, 0, buffer.Length, callback, state);\n// Later in callback:\npublic void ReadCallback(IAsyncResult ar) {\n    var bytesRead = fileStream.EndRead(ar);\n    // process\n}",
                "after": "var bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);\n// Direct sequential flow, no callback needed"
              },
              "confidence": 0.90,
              "modernizationContext": {
                "legacyPattern": "Asynchronous Programming Model (APM) - Begin/End pattern",
                "modernAlternative": "Task-based Asynchronous Pattern (TAP) - async/await",
                "introducedIn": ".NET 4.5 (2012)",
                "benefits": [
                  "More readable and maintainable code",
                  "Better exception handling",
                  "Easier composition of async operations",
                  "Compiler-enforced correctness"
                ],
                "effort": "Low - straightforward replacement in most cases",
                "migrationGuide": "https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/"
              }
            }
          ],
          "summary": "Found 1 legacy async pattern that could be modernized"
        }

        RULES:
        - Be encouraging, not judgmental ("This is an opportunity" vs "This is bad")
        - Prioritize security and performance gains from modernization
        - Provide realistic effort estimates (Low/Medium/High/Very High)
        - Group findings: if multiple legacy patterns exist, suggest a coordinated modernization effort
        - Always explain the "why" behind modernization recommendations
        """;

    public const string SynthesisSystemPrompt = """
        You are the Synthesis Agent responsible for creating the final, actionable code review report.

        YOUR MISSION: Transform raw specialist findings into a coherent, prioritized action plan with strategic insights.

        INPUT:
        You receive findings from 4 specialist agents:
        - Security Agent findings (JSON array)
        - Logic Agent findings (JSON array)
        - Performance Agent findings (JSON array)
        - Modernization Agent findings (JSON array)

        SYNTHESIS WORKFLOW:

        1. DEDUPLICATION:
           - Identify overlapping findings (same file, line, similar description)
           - Merge related findings into single recommendation
           - Keep highest severity when merging
           - Credit all agents that identified the issue

        2. META-INSIGHT EXTRACTION:
           Look for patterns across findings:
           - Multiple security issues in same module → "Authentication layer needs redesign"
           - Performance + logic issues in same area → "Data access layer needs refactoring"
           - Many modernization findings → "Consider planned technical debt sprint"
           - Repeated pattern across files → "System-wide anti-pattern detected"

        3. EFFORT ESTIMATION:
           For each finding or group:
           - Quick fix: <1 hour (typo, simple parameter change)
           - Small: 1-4 hours (single method refactor, add validation)
           - Medium: 1-2 days (refactor class, add caching, write tests)
           - Large: 3-5 days (redesign module, major migration)
           - Epic: >5 days (framework upgrade, architecture change)

        4. PRIORITIZATION STRATEGY:
           Order by:
           a) Security CRITICAL → must fix before merge
           b) Blocking bugs (HIGH severity Logic issues)
           c) High-impact Performance issues
           d) Other HIGH severity items
           e) MEDIUM severity grouped by area
           f) LOW severity and Modernization suggestions

        5. FIX SEQUENCING:
           Suggest order of fixes:
           - Fix security first (always)
           - Address blockers before optimizations
           - Group related fixes (e.g., "Fix all N+1 queries in OrderService together")
           - Note dependencies ("Fix A before B because B depends on A")

        OUTPUT FORMAT (Markdown Report):

        # Code Review Report

        ## Executive Summary
        [2-3 sentence overview: X findings across Y files, N critical issues, estimated Z days to address]

        ## 🚨 Critical Issues (Must Fix Before Merge)
        [Security CRITICAL + Logic bugs that break functionality]

        ### 1. [Issue Title]
        - **File**: `path/to/file.cs:42`
        - **Severity**: CRITICAL
        - **Agents**: Security, Logic
        - **Issue**: [Description]
        - **Impact**: [What happens if not fixed]
        - **Fix**: [Specific recommendation]
        - **Effort**: [Time estimate]

        ```csharp
        // ❌ Current (vulnerable)
        [bad code]

        // ✅ Fixed
        [good code]
        ```

        ## ⚠️ High Priority (Fix Soon)
        [HIGH severity items, grouped by area if related]

        ## 📊 Medium Priority (Address This Sprint)
        [MEDIUM items, grouped by module/pattern]

        ## 💡 Suggestions & Modernization (Future Work)
        [LOW items and Modernization recommendations]

        ## 🎯 Meta-Insights

        ### Pattern Analysis
        [Identified patterns across findings]
        - "3 N+1 query issues in data access layer suggest need for repository pattern review"
        - "Security issues concentrated in UserController - consider authentication middleware"

        ### Recommended Fix Order
        1. **Phase 1 (Before Merge)**: Security critical items [2 hours estimated]
        2. **Phase 2 (This Sprint)**: High priority fixes [1.5 days estimated]
        3. **Phase 3 (Next Sprint)**: Medium priority + refactoring [3 days estimated]
        4. **Phase 4 (Backlog)**: Modernization initiatives [2 weeks estimated]

        ### Technical Debt Score
        - **Current**: 47/100 (moderate debt)
        - **After Fixes**: 72/100 (healthy)
        - **Improvement**: +25 points

        ## 📈 Positive Findings
        [Things done well - positive reinforcement]
        - Good test coverage on new features
        - Proper async/await usage in PaymentService
        - Clean separation of concerns in new modules

        ## 📦 Affected Areas Summary
        | Module | Findings | Severity | Estimated Effort |
        |--------|----------|----------|------------------|
        | UserController | 5 | 2 Critical, 3 High | 4 hours |
        | OrderService | 3 | 1 High, 2 Medium | 1 day |
        | Database Layer | 4 | 4 High | 6 hours |

        ---
        *Report generated by Multi-Agent Code Review System*
        *Agents: Security, Logic, Performance, Modernization*

        SYNTHESIS RULES:
        - Reduce noise: 20 raw findings → 10 actionable recommendations
        - Add value: Don't just concatenate, provide strategic insight
        - Be specific: "Fix these 3 related issues together" not "fix stuff"
        - Estimate realistically: developers should trust your time estimates
        - End on a positive note: recognize good work
        - Make it scannable: busy developers should grasp key points in 30 seconds
        """;

    public const string TechnicalDocsSystemPrompt = """
        You are a Technical Documentation Generator. Create clear, comprehensive documentation for developers.

        YOUR MISSION: Produce documentation that helps developers understand, set up, and work with the project.

        INPUT:
        - Full codebase access
        - Code review findings from synthesis report
        - Existing documentation (if any)
        - Project structure analysis

        DOCUMENTATION TO GENERATE:

        1. **README.md**
        ```markdown
        # Project Name

        ## Overview
        [What the project does, its purpose]

        ## Tech Stack
        - .NET 8.0
        - ASP.NET Core
        - Entity Framework Core
        - [List all major dependencies]

        ## Prerequisites
        - .NET 8 SDK
        - SQL Server / PostgreSQL
        - [Other requirements]

        ## Quick Start
        ```bash
        # Clone and setup
        git clone [repo]
        cd [project]
        dotnet restore
        dotnet run
        ```

        ## Project Structure
        ```
        /src
          /Controllers    - API endpoints
          /Services       - Business logic
          /Data           - Database context and repositories
          /Models         - Domain entities
        ```

        ## Configuration
        [Environment variables, appsettings, secrets]

        ## Running Tests
        ```bash
        dotnet test
        ```

        ## Contributing
        [Coding standards, PR process]
        ```

        2. **API_DOCUMENTATION.md**
        ```markdown
        # API Documentation

        ## Authentication
        [How to authenticate - JWT, OAuth, etc.]

        ## Endpoints

        ### User Management

        #### GET /api/users
        **Description**: Retrieve all users
        **Auth Required**: Yes (Admin role)
        **Query Parameters**:
        - `page` (int, optional): Page number
        - `pageSize` (int, optional): Items per page

        **Response**:
        ```json
        {
          "users": [...],
          "totalCount": 100,
          "page": 1
        }
        ```

        **Status Codes**:
        - 200: Success
        - 401: Unauthorized
        - 403: Forbidden

        [Document all endpoints with examples]
        ```

        3. **ARCHITECTURE.md**
        ```markdown
        # Architecture Overview

        ## High-Level Design
        [System architecture diagram - can be mermaid]

        ## Layers
        - **Presentation**: ASP.NET Core controllers
        - **Business Logic**: Service layer
        - **Data Access**: Repository pattern with EF Core
        - **Infrastructure**: External services, caching, etc.

        ## Design Decisions
        [Key architectural decisions and rationale]

        ## Data Flow
        [Request/response flow diagrams]
        ```

        4. **INSTALLATION_GUIDE.md**
        ```markdown
        # Installation Guide

        ## Development Environment Setup

        ### 1. Install Prerequisites
        [Detailed steps for each prerequisite]

        ### 2. Database Setup
        [Migration commands, seed data]

        ### 3. Configuration
        [Detailed configuration steps]

        ### 4. IDE Setup
        [VS Code / Visual Studio settings]

        ### Troubleshooting
        [Common issues and solutions]
        ```

        GENERATION RULES:
        - Extract information from code, don't make assumptions
        - Use code examples from the actual project
        - Include diagrams where helpful (mermaid format)
        - Keep language clear and concise
        - Update existing docs rather than replacing if docs exist
        - Mark auto-generated sections for review
        """;

    public const string OnboardingSystemPrompt = """
        You are an Interactive Onboarding Assistant for this software project. Your role is to help new developers 
        understand the codebase, architecture, conventions, and workflows through natural conversation.

        YOUR MISSION: Make new developers productive quickly by answering their questions about the project.

        INTERACTION STYLE:
        - Friendly and encouraging, like a patient mentor
        - Provide context and "why" not just "what"
        - Use examples from the actual codebase
        - Anticipate follow-up questions
        - Check understanding: "Does this make sense? Want me to explain further?"

        RESPONSE STRATEGY:

        1. **For "Where is X?" questions**:
           - Provide exact file path and line numbers
           - Show relevant code snippet
           - Explain what it does and how it fits in architecture
           - Offer to show related code

        2. **For "How does X work?" questions**:
           - Explain the concept first
           - Show the implementation with code examples
           - Trace the flow through the system
           - Mention related patterns or conventions

        3. **For "How do I do X?" questions**:
           - Provide step-by-step instructions
           - Show code examples from the project
           - Mention any conventions or standards to follow
           - Offer to answer follow-up questions

        4. **For architecture questions**:
           - Explain the high-level design first
           - Show how modules interact
           - Use diagrams if helpful (describe in text or mermaid)
           - Relate to concrete code examples

        5. **For process questions** (git workflow, testing, deployment):
           - Describe the process clearly
           - Provide commands/steps
           - Mention common pitfalls
           - Point to relevant documentation

        RULES:
        - Never say "I don't know" - find information from the codebase
        - Always ground answers in the actual codebase
        - Be encouraging: "Great question!", "That's a smart observation"
        - Adapt depth to user's apparent experience level
        - Offer to elaborate: "Want more details on this?"
        - If something is missing/unclear in codebase, acknowledge it
        """;
}
```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Core/Configuration/PipelineConfig.cs"
}
```

**Output:**
```
using System.Text.Json.Serialization;

namespace MultiAgentCodeReview.Core.Configuration;

public record ModelConfig(
    string Role,
    string Provider,
    string ModelId,
    double Temperature,
    int MaxTokens,
    int RpmLimit,
    int TpmLimit
);

public class PipelineConfig
{
    public Dictionary<string, ModelConfig> Models { get; set; } = new();
    public StageConfig Pipeline { get; set; } = new();
    public FilterConfig Filtering { get; set; } = new();
    public RagConfig Rag { get; set; } = new();
    public OutputConfig Output { get; set; } = new();
}

public class StageConfig
{
    public List<string> Stages { get; set; } = ["filter", "triage", "specialists", "synthesis", "documentation"];
    public int TimeoutSeconds { get; set; } = 300;
    public int MaxConcurrentAgents { get; set; } = 1; // Sequential for rate limits
}

public class FilterConfig
{
    public int MaxFiles { get; set; } = 30;
    public int MinFiles { get; set; } = 5;
    public bool UseDependencyGraph { get; set; } = true;
    public bool IncludeTests { get; set; } = false;
}

public class RagConfig
{
    public bool Enabled { get; set; } = true;
    public string EmbeddingModel { get; set; } = "all-MiniLM-L6-v2";
    public int TopK { get; set; } = 5;
    public List<string> KnowledgeSources { get; set; } = ["codebase", "documentation", "standards", "owasp", "history"];
}

public class OutputConfig
{
    public string Format { get; set; } = "markdown";
    public bool IncludeCodeSnippets { get; set; } = true;
    public bool IncludeFixExamples { get; set; } = true;
    public bool GroupByModule { get; set; } = true;
}
```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.1s)

_Thinking:_

Let me continue exploring the agents and orchestration to understand the full architecture.

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/TriageAgent.cs"
}
```

**Output:**
```
using AutoGen;
using MultiAgentCodeReview.Core.Interfaces;
using MultiAgentCodeReview.Core.Models;

namespace MultiAgentCodeReview.Agents;

public class TriageAgent : ITriageAgent
{
    public string Name => "TriageAgent";

    private readonly AssistantAgent _agent;

    public TriageAgent(AssistantAgent agent)
    {
        _agent = agent;
    }

    public async Task<TriageResult> ClassifyAsync(PipelineContext context, CancellationToken cancellationToken = default)
    {
        var userPrompt = BuildTriagePrompt(context);
        var response = await _agent.SendMessageAsync(userPrompt, cancellationToken: cancellationToken);
        return ParseTriageResponse(response);
    }

    public Task<AgentResult> AnalyzeAsync(PipelineContext context, CancellationToken cancellationToken = default)
    {
        var triageResult = await ClassifyAsync(context, cancellationToken);
        return Task.FromResult(new AgentResult(
            new List<Finding>(),
            $"Triage: {string.Join(", ", triageResult.Classifications)} -> Route to {string.Join(", ", triageResult.RouteTo)}"
        ));
    }

    private string BuildTriagePrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Classify the following code changes to route to specialist agents:");
        sb.AppendLine();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        sb.AppendLine();

        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }

        if (context.Diff != null)
        {
            sb.AppendLine();
            sb.AppendLine("Git Diff Summary:");
            sb.AppendLine(context.Diff.Summary);
        }

        sb.AppendLine();
        sb.AppendLine("Return JSON with classifications, routeTo, priority, and reasoning.");
        sb.AppendLine("Categories: SECURITY_SENSITIVE, LOGIC_CRITICAL, PERFORMANCE_IMPACT, MODERNIZATION_NEEDED");
        sb.AppendLine("RouteTo: SecurityAgent, LogicAgent, PerformanceAgent, ModernizationAgent");
        sb.AppendLine("Priority: CRITICAL, HIGH, MEDIUM, LOW");

        return sb.ToString();
    }

    private TriageResult ParseTriageResponse(string response)
    {
        try
        {
            var cleaned = CleanJsonResponse(response);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<TriageResult>(cleaned, options) ?? CreateDefault();
        }
        catch
        {
            return CreateDefault();
        }
    }

    private static string CleanJsonResponse(string response)
    {
        var trimmed = response.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0)
                trimmed = trimmed.Substring(firstNewline + 1);
            var lastBackticks = trimmed.LastIndexOf("```");
            if (lastBackticks >= 0)
                trimmed = trimmed.Substring(0, lastBackticks);
        }
        return trimmed.Trim();
    }

    private static TriageResult CreateDefault() => new(
        new List<string> { "LOGIC_CRITICAL" },
        new List<string> { "LogicAgent" },
        "MEDIUM",
        "Default fallback classification"
    );
}
```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/AgentFactory.cs"
}
```

**Output:**
```
using System.Text.Json;
using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using Microsoft.Extensions.Options;
using MultiAgentCodeReview.Core.Configuration;
using MultiAgentCodeReview.Core.Interfaces;
using MultiAgentCodeReview.Core.Models;
using MultiAgentCodeReview.Core.Prompts;

namespace MultiAgentCodeReview.Agents;

public class AgentFactory
{
    private readonly PipelineConfig _config;
    private readonly RateLimitedHttpClient _httpClient;

    public AgentFactory(PipelineConfig config, RateLimitedHttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public AssistantAgent CreateTriageAgent()
    {
        var modelConfig = GetModelConfig("triage");
        return CreateAssistantAgent("TriageAgent", AgentPrompts.TriageSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateSecurityAgent()
    {
        var modelConfig = GetModelConfig("security");
        return CreateAssistantAgent("SecurityAgent", AgentPrompts.SecuritySystemPrompt, modelConfig);
    }

    public AssistantAgent CreateLogicAgent()
    {
        var modelConfig = GetModelConfig("logic");
        return CreateAssistantAgent("LogicAgent", AgentPrompts.LogicSystemPrompt, modelConfig);
    }

    public AssistantAgent CreatePerformanceAgent()
    {
        var modelConfig = GetModelConfig("performance");
        return CreateAssistantAgent("PerformanceAgent", AgentPrompts.PerformanceSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateModernizationAgent()
    {
        var modelConfig = GetModelConfig("modernization");
        return CreateAssistantAgent("ModernizationAgent", AgentPrompts.ModernizationSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateSynthesisAgent()
    {
        var modelConfig = GetModelConfig("synthesis");
        return CreateAssistantAgent("SynthesisAgent", AgentPrompts.SynthesisSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateDocumentationAgent()
    {
        var modelConfig = GetModelConfig("documentation");
        return CreateAssistantAgent("DocumentationAgent", AgentPrompts.TechnicalDocsSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateOnboardingAgent()
    {
        var modelConfig = GetModelConfig("onboarding");
        return CreateAssistantAgent("OnboardingAgent", AgentPrompts.OnboardingSystemPrompt, modelConfig);
    }

    private AssistantAgent CreateAssistantAgent(string name, string systemPrompt, ModelConfig modelConfig)
    {
        var openAIClient = new OpenAIClient(
            modelConfig.ModelId,
            new OpenAIClientOptions
            {
                ApiKey = _config.MultiProvider.Groq.Key1,
                Endpoint = _config.MultiProvider.Groq.BaseUrl,
                HttpClient = _httpClient.Client
            });

        return new AssistantAgent(name, openAIClient)
        {
            SystemMessage = systemPrompt
        };
    }

    private ModelConfig GetModelConfig(string role)
    {
        if (_config.MultiProvider.Models.TryGetValue(role, out var modelConfig))
        {
            return modelConfig;
        }

        // Fallback defaults
        return new ModelConfig
        {
            Role = role,
            Provider = "groq",
            ModelId = "llama-3.1-8b-instant",
            Temperature = role switch
            {
                "triage" => 0.1,
                "security" => 0.2,
                "logic" => 0.3,
                "performance" => 0.2,
                "modernization" => 0.4,
                "synthesis" => 0.4,
                "documentation" => 0.3,
                "onboarding" => 0.5,
                _ => 0.2
            },
            MaxTokens = role switch
            {
                "triage" => 500,
                "security" => 2000,
                "logic" => 3000,
                "performance" => 2000,
                "modernization" => 3000,
                "synthesis" => 4000,
                "documentation" => 4000,
                "onboarding" => 3000,
                _ => 2000
            },
            RpmLimit = 30,
            TpmLimit = 6000
        };
    }
}
```

**Tool: bash**

**Input:**
```json
{
  "command": "ls -la ./MultiAgentCodeReview.Agents/"
}
```

**Output:**
```
total 56
drwxr-xr-x@ 7 bhavyananda17  staff    224 Jul 15 11:28 .
drwxr-xr-x@ 7 bhavyananda17  staff    224 Jul 15 11:19 ..
-rw-r--r--@ 1 bhavyananda17  staff   4153 Jul 15 11:27 AgentFactory.cs
-rw-r--r--@ 1 bhavyananda17  staff    426 Jul 15 11:27 MultiAgentCodeReview.Agents.csproj
-rw-r--r--@ 1 bhavyananda17  staff  10186 Jul 15 11:28 SpecialistAgents.cs
-rw-r--r--@ 1 bhavyananda17  staff   3506 Jul 15 11:27 TriageAgent.cs
drwxr-xr-x@ 7 bhavyananda17  staff    224 Jul 15 11:27 obj

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.2s)

_Thinking:_

Let me see more of the agents and orchestration layers

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/SpecialistAgents.cs"
}
```

**Output:**
```
using AutoGen;
using MultiAgentCodeReview.Core.Interfaces;
using MultiAgentCodeReview.Core.Models;

namespace MultiAgentCodeReview.Agents;

public abstract class BaseSpecialistAgent : ISpecialistAgent
{
    protected readonly AssistantAgent _agent;
    protected readonly string _agentName;

    public string Name => _agentName;
    public abstract List<string> TriggerCategories { get; }

    protected BaseSpecialistAgent(AssistantAgent agent, string name)
    {
        _agent = agent;
        _agentName = name;
    }

    public async Task<AgentResult> AnalyzeAsync(PipelineContext context, CancellationToken cancellationToken = default)
    {
        var userPrompt = BuildPrompt(context);
        var response = await _agent.SendMessageAsync(userPrompt, cancellationToken: cancellationToken);
        return ParseResponse(response);
    }

    protected abstract string BuildPrompt(PipelineContext context);

    protected virtual AgentResult ParseResponse(string response)
    {
        try
        {
            var cleaned = CleanJsonResponse(response);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<AgentResult>(cleaned, options) 
                   ?? new AgentResult(new List<Finding>(), "Failed to parse response");
        }
        catch
        {
            return new AgentResult(new List<Finding>(), "Parse error");
        }
    }

    private static string CleanJsonResponse(string response)
    {
        var trimmed = response.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0)
                trimmed = trimmed.Substring(firstNewline + 1);
            var lastBackticks = trimmed.LastIndexOf("```");
            if (lastBackticks >= 0)
                trimmed = trimmed.Substring(0, lastBackticks);
        }
        return trimmed.Trim();
    }
}

public class SecurityAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Security/", "Auth/", "Crypto/", "Validation/"];

    public SecurityAgent(AssistantAgent agent) : base(agent, "SecurityAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for security vulnerabilities:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Focus on: SQL injection, XSS, auth bypass, sensitive data exposure, crypto weaknesses, input validation, dependency vulnerabilities.");
        sb.AppendLine("Use RAG to check against OWASP Top 10 and known CVEs.");
        sb.AppendLine("Return JSON with findings array and summary.");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}

public class LogicAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Controllers/", "Services/", "Models/", "Logic/"];

    public LogicAgent(AssistantAgent agent) : base(agent, "LogicAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for logic correctness, code quality, and maintainability:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Check: Logic errors, edge cases, null handling, complexity (>10), code smells, SOLID violations, testability, naming, error handling.");
        sb.AppendLine("Use RAG for company coding standards, Clean Code principles, design patterns.");
        sb.AppendLine("Return JSON with findings array and summary. Include metrics (complexity, LOC, nesting).");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}

public class PerformanceAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Database/", "Repository/", "DataAccess/", "Services/"];

    public PerformanceAgent(AssistantAgent agent) : base(agent, "PerformanceAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for performance bottlenecks:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Focus: N+1 queries, missing indexes, SELECT *, queries in loops, no pagination, blocking calls (.Result/.Wait()), missing ConfigureAwait, async void, memory leaks, large allocations, string concat in loops, O(n²) algorithms, repeated LINQ, missing caching.");
        sb.AppendLine("Use RAG for EF Core optimization patterns, async/await best practices, memory profiling.");
        sb.AppendLine("Quantify impact: 'Adds 200ms', 'N+1: 1+N queries instead of 1'. Return JSON with findings, impact estimates, optimized code examples.");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}

public class ModernizationAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Legacy/", "Old/", "Deprecated/"];

    public ModernizationAgent(AssistantAgent agent) : base(agent, "ModernizationAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for technical debt and modernization opportunities:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Focus: Outdated frameworks (.NET Framework, EF6, old ASP.NET), legacy patterns (Begin/End APM, manual DI, config in code, WebForms/WCF), missing modern C# (pattern matching, records, nullable refs), outdated NuGet packages, architecture debt, missing tests.");
        sb.AppendLine("Use RAG for framework migration guides, modern C# patterns, dependency vulnerabilities.");
        sb.AppendLine("Style: Educational/collaborative - explain WHY legacy, WHAT modern alternative, HOW to migrate, BENEFITS. Group related findings. Include effort estimates (Low/Medium/High/Very High) and migration guide links.");
        sb.AppendLine("Return JSON with findings, modernizationContext for each finding.");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}
```

**Tool: bash**

**Input:**
```json
{
  "command": "ls -la ./MultiAgentCodeReview.Orchestration/"
}
```

**Output:**
```
total 8
drwxr-xr-x@ 8 bhavyananda17  staff  256 Jul 15 11:27 .
drwxr-xr-x@ 7 bhavyananda17  staff  224 Jul 15 11:19 ..
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 DI
-rw-r--r--@ 1 bhavyananda17  staff  750 Jul 15 11:27 MultiAgentCodeReview.Orchestration.csproj
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 Pipeline
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 RateLimiting
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 Tools
drwxr-xr-x@ 7 bhavyananda17  staff  224 Jul 15 11:27 obj

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.8s)

_Thinking:_

Let me explore the orchestration layer to understand the full pipeline.

**Tool: bash**

**Input:**
```json
{
  "command": "ls -la ./MultiAgentCodeReview.Orchestration/Pipeline/"
}
```

**Output:**
```
total 0
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 .
drwxr-xr-x@ 8 bhavyananda17  staff  256 Jul 15 11:27 ..

```

**Tool: bash**

**Input:**
```json
{
  "command": "ls -la ./MultiAgentCodeReview.Orchestration/Tools/"
}
```

**Output:**
```
total 0
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 .
drwxr-xr-x@ 8 bhavyananda17  staff  256 Jul 15 11:27 ..

```

**Tool: bash**

**Input:**
```json
{
  "command": "ls -la ./MultiAgentCodeReview.Orchestration/DI/"
}
```

**Output:**
```
total 0
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 .
drwxr-xr-x@ 8 bhavyananda17  staff  256 Jul 15 11:27 ..

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.3s)

_Thinking:_

The orchestration directories seem to be empty. Let me check the Agents project and the Host project

**Tool: bash**

**Input:**
```json
{
  "command": "ls -la ./MultiAgentCodeReview.Agents/"
}
```

**Output:**
```
total 56
drwxr-xr-x@ 7 bhavyananda17  staff    224 Jul 15 11:28 .
drwxr-xr-x@ 7 bhavyananda17  staff    224 Jul 15 11:19 ..
-rw-r--r--@ 1 bhavyananda17  staff   4153 Jul 15 11:27 AgentFactory.cs
-rw-r--r--@ 1 bhavyananda17  staff    426 Jul 15 11:27 MultiAgentCodeReview.Agents.csproj
-rw-r--r--@ 1 bhavyananda17  staff  10186 Jul 15 11:28 SpecialistAgents.cs
-rw-r--r--@ 1 bhavyananda17  staff   3506 Jul 15 11:27 TriageAgent.cs
drwxr-xr-x@ 7 bhavyananda17  staff    224 Jul 15 11:27 obj

```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/TriageAgent.cs"
}
```

**Output:**
```
using AutoGen;
using MultiAgentCodeReview.Core.Interfaces;
using MultiAgentCodeReview.Core.Models;

namespace MultiAgentCodeReview.Agents;

public class TriageAgent : ITriageAgent
{
    public string Name => "TriageAgent";

    private readonly AssistantAgent _agent;

    public TriageAgent(AssistantAgent agent)
    {
        _agent = agent;
    }

    public async Task<TriageResult> ClassifyAsync(PipelineContext context, CancellationToken cancellationToken = default)
    {
        var userPrompt = BuildTriagePrompt(context);
        var response = await _agent.SendMessageAsync(userPrompt, cancellationToken: cancellationToken);
        return ParseTriageResponse(response);
    }

    public Task<AgentResult> AnalyzeAsync(PipelineContext context, CancellationToken cancellationToken = default)
    {
        var triageResult = await ClassifyAsync(context, cancellationToken);
        return Task.FromResult(new AgentResult(
            new List<Finding>(),
            $"Triage: {string.Join(", ", triageResult.Classifications)} -> Route to {string.Join(", ", triageResult.RouteTo)}"
        ));
    }

    private string BuildTriagePrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Classify the following code changes to route to specialist agents:");
        sb.AppendLine();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        sb.AppendLine();

        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }

        if (context.Diff != null)
        {
            sb.AppendLine();
            sb.AppendLine("Git Diff Summary:");
            sb.AppendLine(context.Diff.Summary);
        }

        sb.AppendLine();
        sb.AppendLine("Return JSON with classifications, routeTo, priority, and reasoning.");
        sb.AppendLine("Categories: SECURITY_SENSITIVE, LOGIC_CRITICAL, PERFORMANCE_IMPACT, MODERNIZATION_NEEDED");
        sb.AppendLine("RouteTo: SecurityAgent, LogicAgent, PerformanceAgent, ModernizationAgent");
        sb.AppendLine("Priority: CRITICAL, HIGH, MEDIUM, LOW");

        return sb.ToString();
    }

    private TriageResult ParseTriageResponse(string response)
    {
        try
        {
            var cleaned = CleanJsonResponse(response);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<TriageResult>(cleaned, options) ?? CreateDefault();
        }
        catch
        {
            return CreateDefault();
        }
    }

    private static string CleanJsonResponse(string response)
    {
        var trimmed = response.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0)
                trimmed = trimmed.Substring(firstNewline + 1);
            var lastBackticks = trimmed.LastIndexOf("```");
            if (lastBackticks >= 0)
                trimmed = trimmed.Substring(0, lastBackticks);
        }
        return trimmed.Trim();
    }

    private static TriageResult CreateDefault() => new(
        new List<string> { "LOGIC_CRITICAL" },
        new List<string> { "LogicAgent" },
        "MEDIUM",
        "Default fallback classification"
    );
}
```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/AgentFactory.cs"
}
```

**Output:**
```
using System.Text.Json;
using AutoGen;
using AutoGen.Core;
using AutoGen.OpenAI;
using Microsoft.Extensions.Options;
using MultiAgentCodeReview.Core.Configuration;
using MultiAgentCodeReview.Core.Interfaces;
using MultiAgentCodeReview.Core.Models;
using MultiAgentCodeReview.Core.Prompts;

namespace MultiAgentCodeReview.Agents;

public class AgentFactory
{
    private readonly PipelineConfig _config;
    private readonly RateLimitedHttpClient _httpClient;

    public AgentFactory(PipelineConfig config, RateLimitedHttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public AssistantAgent CreateTriageAgent()
    {
        var modelConfig = GetModelConfig("triage");
        return CreateAssistantAgent("TriageAgent", AgentPrompts.TriageSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateSecurityAgent()
    {
        var modelConfig = GetModelConfig("security");
        return CreateAssistantAgent("SecurityAgent", AgentPrompts.SecuritySystemPrompt, modelConfig);
    }

    public AssistantAgent CreateLogicAgent()
    {
        var modelConfig = GetModelConfig("logic");
        return CreateAssistantAgent("LogicAgent", AgentPrompts.LogicSystemPrompt, modelConfig);
    }

    public AssistantAgent CreatePerformanceAgent()
    {
        var modelConfig = GetModelConfig("performance");
        return CreateAssistantAgent("PerformanceAgent", AgentPrompts.PerformanceSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateModernizationAgent()
    {
        var modelConfig = GetModelConfig("modernization");
        return CreateAssistantAgent("ModernizationAgent", AgentPrompts.ModernizationSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateSynthesisAgent()
    {
        var modelConfig = GetModelConfig("synthesis");
        return CreateAssistantAgent("SynthesisAgent", AgentPrompts.SynthesisSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateDocumentationAgent()
    {
        var modelConfig = GetModelConfig("documentation");
        return CreateAssistantAgent("DocumentationAgent", AgentPrompts.TechnicalDocsSystemPrompt, modelConfig);
    }

    public AssistantAgent CreateOnboardingAgent()
    {
        var modelConfig = GetModelConfig("onboarding");
        return CreateAssistantAgent("OnboardingAgent", AgentPrompts.OnboardingSystemPrompt, modelConfig);
    }

    private AssistantAgent CreateAssistantAgent(string name, string systemPrompt, ModelConfig modelConfig)
    {
        var openAIClient = new OpenAIClient(
            modelConfig.ModelId,
            new OpenAIClientOptions
            {
                ApiKey = _config.MultiProvider.Groq.Key1,
                Endpoint = _config.MultiProvider.Groq.BaseUrl,
                HttpClient = _httpClient.Client
            });

        return new AssistantAgent(name, openAIClient)
        {
            SystemMessage = systemPrompt
        };
    }

    private ModelConfig GetModelConfig(string role)
    {
        if (_config.MultiProvider.Models.TryGetValue(role, out var modelConfig))
        {
            return modelConfig;
        }

        // Fallback defaults
        return new ModelConfig
        {
            Role = role,
            Provider = "groq",
            ModelId = "llama-3.1-8b-instant",
            Temperature = role switch
            {
                "triage" => 0.1,
                "security" => 0.2,
                "logic" => 0.3,
                "performance" => 0.2,
                "modernization" => 0.4,
                "synthesis" => 0.4,
                "documentation" => 0.3,
                "onboarding" => 0.5,
                _ => 0.2
            },
            MaxTokens = role switch
            {
                "triage" => 500,
                "security" => 2000,
                "logic" => 3000,
                "performance" => 2000,
                "modernization" => 3000,
                "synthesis" => 4000,
                "documentation" => 4000,
                "onboarding" => 3000,
                _ => 2000
            },
            RpmLimit = 30,
            TpmLimit = 6000
        };
    }
}
```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.7s)

_Thinking:_

Now let me look at the Host project and the Orchestration DI/Tools folders to see how this all ties together.

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Host/Program.cs"
}
```

**Output:**
```
Console.WriteLine("Hello, World!");

```

**Tool: bash**

**Input:**
```json
{
  "command": "ls -la ./MultiAgentCodeReview.Orchestration/DI/"
}
```

**Output:**
```
total 0
drwxr-xr-x@ 2 bhavyananda17  staff   64 Jul 15 11:27 .
drwxr-xr-x@ 8 bhavyananda17  staff  256 Jul 15 11:27 ..

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 3.3s)

_Thinking:_

The Orchestration project is mostly empty, and the Host project has just a basic "Hello World" program. Let me check the project files to understand the structure and dependencies.

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Host/MultiAgentCodeReview.Host.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\MultiAgentCodeReview.Orchestration\MultiAgentCodeReview.Orchestration.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="DotNetEnv" Version="3.2.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.10" />
  </ItemGroup>

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Orchestration/MultiAgentCodeReview.Orchestration.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\MultiAgentCodeReview.Core\MultiAgentCodeReview.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="AutoGen" Version="0.2.0" />
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="5.6.0" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.10" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Core/MultiAgentCodeReview.Core.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.10" />
  </ItemGroup>

</Project>

```

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/MultiAgentCodeReview.Agents.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\MultiAgentCodeReview.Core\MultiAgentCodeReview.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="AutoGen" Version="0.2.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 3.4s)

_Thinking:_

Now let me check the SpecialistAgents.cs file and any configuration files to understand the full picture.

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/SpecialistAgents.cs"
}
```

**Output:**
```
using AutoGen;
using MultiAgentCodeReview.Core.Interfaces;
using MultiAgentCodeReview.Core.Models;

namespace MultiAgentCodeReview.Agents;

public abstract class BaseSpecialistAgent : ISpecialistAgent
{
    protected readonly AssistantAgent _agent;
    protected readonly string _agentName;

    public string Name => _agentName;
    public abstract List<string> TriggerCategories { get; }

    protected BaseSpecialistAgent(AssistantAgent agent, string name)
    {
        _agent = agent;
        _agentName = name;
    }

    public async Task<AgentResult> AnalyzeAsync(PipelineContext context, CancellationToken cancellationToken = default)
    {
        var userPrompt = BuildPrompt(context);
        var response = await _agent.SendMessageAsync(userPrompt, cancellationToken: cancellationToken);
        return ParseResponse(response);
    }

    protected abstract string BuildPrompt(PipelineContext context);

    protected virtual AgentResult ParseResponse(string response)
    {
        try
        {
            var cleaned = CleanJsonResponse(response);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<AgentResult>(cleaned, options) 
                   ?? new AgentResult(new List<Finding>(), "Failed to parse response");
        }
        catch
        {
            return new AgentResult(new List<Finding>(), "Parse error");
        }
    }

    private static string CleanJsonResponse(string response)
    {
        var trimmed = response.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0)
                trimmed = trimmed.Substring(firstNewline + 1);
            var lastBackticks = trimmed.LastIndexOf("```");
            if (lastBackticks >= 0)
                trimmed = trimmed.Substring(0, lastBackticks);
        }
        return trimmed.Trim();
    }
}

public class SecurityAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Security/", "Auth/", "Crypto/", "Validation/"];

    public SecurityAgent(AssistantAgent agent) : base(agent, "SecurityAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for security vulnerabilities:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Focus on: SQL injection, XSS, auth bypass, sensitive data exposure, crypto weaknesses, input validation, dependency vulnerabilities.");
        sb.AppendLine("Use RAG to check against OWASP Top 10 and known CVEs.");
        sb.AppendLine("Return JSON with findings array and summary.");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}

public class LogicAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Controllers/", "Services/", "Models/", "Logic/"];

    public LogicAgent(AssistantAgent agent) : base(agent, "LogicAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for logic correctness, code quality, and maintainability:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Check: Logic errors, edge cases, null handling, complexity (>10), code smells, SOLID violations, testability, naming, error handling.");
        sb.AppendLine("Use RAG for company coding standards, Clean Code principles, design patterns.");
        sb.AppendLine("Return JSON with findings array and summary. Include metrics (complexity, LOC, nesting).");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}

public class PerformanceAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Database/", "Repository/", "DataAccess/", "Services/"];

    public PerformanceAgent(AssistantAgent agent) : base(agent, "PerformanceAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for performance bottlenecks:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Focus: N+1 queries, missing indexes, SELECT *, queries in loops, no pagination, blocking calls (.Result/.Wait()), missing ConfigureAwait, async void, memory leaks, large allocations, string concat in loops, O(n²) algorithms, repeated LINQ, missing caching.");
        sb.AppendLine("Use RAG for EF Core optimization patterns, async/await best practices, memory profiling.");
        sb.AppendLine("Quantify impact: 'Adds 200ms', 'N+1: 1+N queries instead of 1'. Return JSON with findings, impact estimates, optimized code examples.");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}

public class ModernizationAgent : BaseSpecialistAgent
{
    public override List<string> TriggerCategories { get; } = ["Legacy/", "Old/", "Deprecated/"];

    public ModernizationAgent(AssistantAgent agent) : base(agent, "ModernizationAgent") { }

    protected override string BuildPrompt(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Analyze the following code changes for technical debt and modernization opportunities:");
        sb.AppendLine();
        sb.AppendLine(BuildContextSummary(context));
        sb.AppendLine();

        if (context.Diff != null)
        {
            sb.AppendLine("Code Changes:");
            foreach (var fileDiff in context.Diff.Files)
            {
                sb.AppendLine($"--- {fileDiff.Path} ---");
                foreach (var hunk in fileDiff.Hunks)
                {
                    sb.AppendLine(hunk.Content);
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("Focus: Outdated frameworks (.NET Framework, EF6, old ASP.NET), legacy patterns (Begin/End APM, manual DI, config in code, WebForms/WCF), missing modern C# (pattern matching, records, nullable refs), outdated NuGet packages, architecture debt, missing tests.");
        sb.AppendLine("Use RAG for framework migration guides, modern C# patterns, dependency vulnerabilities.");
        sb.AppendLine("Style: Educational/collaborative - explain WHY legacy, WHAT modern alternative, HOW to migrate, BENEFITS. Group related findings. Include effort estimates (Low/Medium/High/Very High) and migration guide links.");
        sb.AppendLine("Return JSON with findings, modernizationContext for each finding.");

        return sb.ToString();
    }

    private static string BuildContextSummary(PipelineContext context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Repository: {context.RepositoryPath}");
        sb.AppendLine($"Commit: {context.CommitHash}");
        sb.AppendLine($"Changed Files: {context.ChangedFiles.Count}");
        foreach (var file in context.ChangedFiles)
        {
            sb.AppendLine($"  {file.Path} (+{file.Additions}/-{file.Deletions}) [{file.ChangeType}]");
        }
        return sb.ToString();
    }
}
```

**Tool: bash**

**Input:**
```json
{
  "command": "find . -name \"*.json\" -o -name \"appsettings*\" -o -name \"*.env\" | head -20"
}
```

**Output:**
```
./MultiAgentCodeReview.Orchestration/obj/MultiAgentCodeReview.Orchestration.csproj.nuget.dgspec.json
./MultiAgentCodeReview.Orchestration/obj/project.assets.json
./MultiAgentCodeReview.Host/obj/project.assets.json
./MultiAgentCodeReview.Host/obj/MultiAgentCodeReview.Host.csproj.nuget.dgspec.json
./MultiAgentCodeReview.Core/obj/MultiAgentCodeReview.Core.csproj.nuget.dgspec.json
./MultiAgentCodeReview.Core/obj/project.assets.json
./MultiAgentCodeReview.Agents/obj/MultiAgentCodeReview.Agents.csproj.nuget.dgspec.json
./MultiAgentCodeReview.Agents/obj/project.assets.json

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.4s)

_Thinking:_

Now I have a good picture of the project structure. Let me check the solution file and see if there's any configuration or other files I might be missing.

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.slnx"
}
```

**Output:**
```
<Solution>
  <Project Path="MultiAgentCodeReview.Agents/MultiAgentCodeReview.Agents.csproj" />
  <Project Path="MultiAgentCodeReview.Core/MultiAgentCodeReview.Core.csproj" />
  <Project Path="MultiAgentCodeReview.Host/MultiAgentCodeReview.Host.csproj" />
  <Project Path="MultiAgentCodeReview.Orchestration/MultiAgentCodeReview.Orchestration.csproj" />
</Solution>

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.4s)

_Thinking:_

Let me check if there's any configuration file or appsettings.json, and also check the Host project for the entry point.

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Host/Program.cs"
}
```

**Output:**
```
Console.WriteLine("Hello, World!");

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 3.8s)

_Thinking:_

The host is just a basic Hello World. Let me also check if there's any configuration in the Core project, and look at the solution structure more carefully.

**Tool: bash**

**Input:**
```json
{
  "command": "find . -name \"*.json\" -not -path \"*/obj/*\" -not -path \"*/bin/*\""
}
```

**Output:**
```
(no output)
```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 3.9s)

_Thinking:_

No JSON configuration files found. Let me check for appsettings or config files.

**Tool: bash**

**Input:**
```json
{
  "command": "find . -name \"appsettings*\" -o -name \"*.config\" -o -name \"*.yaml\" -o -name \"*.yml\" -not -path \"*/obj/*\" -not -path \"*/bin/*\""
}
```

**Output:**
```
(no output)
```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.2s)

_Thinking:_

Let me look for all project files to see dependencies and understand what packages are used

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Core/MultiAgentCodeReview.Core.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.10" />
  </ItemGroup>

</Project>

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.4s)

_Thinking:_

Now let me check the Agents project file and the Orchestration project to see the dependencies

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Agents/MultiAgentCodeReview.Agents.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\MultiAgentCodeReview.Core\MultiAgentCodeReview.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="AutoGen" Version="0.2.0" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.4s)

_Thinking:_

Let me check the Orchestration and Host project files as well.

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Orchestration/MultiAgentCodeReview.Orchestration.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\MultiAgentCodeReview.Core\MultiAgentCodeReview.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="AutoGen" Version="0.2.0" />
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="5.6.0" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.10" />
  </ItemGroup>

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.0s)

_Thinking:_

Now I have a clear picture. Let me also check the Host project

**Tool: bash**

**Input:**
```json
{
  "command": "cat ./MultiAgentCodeReview.Host/MultiAgentCodeReview.Host.csproj"
}
```

**Output:**
```
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="..\MultiAgentCodeReview.Orchestration\MultiAgentCodeReview.Orchestration.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="DotNetEnv" Version="3.2.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.10" />
  </ItemGroup>

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

---

## Assistant (Plan · Nemotron 3 Ultra Free · 2.4s)

_Thinking:_

Now I have a good picture of the architecture. Let me summarize:

**Project Structure:**
- **MultiAgentCodeReview.Core** - Core models, interfaces, prompts, configuration
- **MultiAgentCodeReview.Agents** - Agent implementations (Triage,

---

