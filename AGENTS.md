# AGENTS.md
# MCP Server Transformation — Control File for OpenCode (Big Pickle)

> **How to use this file**
> Drop this file into the root of your repo before starting OpenCode.
> Every section marked `🔴 DECISION REQUIRED` means OpenCode MUST stop, print the question
> to the terminal, and wait for your typed answer before writing any code.
> Sections marked `✅ DECIDED` are locked — OpenCode executes them exactly as written, no improvisation.
> Sections marked `⚠️ CONSTRAINT` are hard rules OpenCode cannot override under any circumstance.

---

## 0. Ground Rules 

```
YOU ARE A CODE EXECUTOR, NOT AN ARCHITECT.
Read every DECISION REQUIRED block. Stop. Print it. Wait.
Do not infer what Bhavya probably wants.
Do not create files not listed in a phase.
Do not install packages not listed in a phase.
Do not rename existing classes or interfaces.
Do not refactor existing agents — wrap them, never rewrite them.
If you are unsure about ANYTHING, print: "DECISION NEEDED: <your question>" and stop.
```

---

## 1. What Already Exists — Read-Only Map

> ⚠️ CONSTRAINT: OpenCode must read these paths and understand the existing structure.
> It must NOT modify any file in these projects during Phase 1.

```
MultiAgentCodeReview/
├── MultiAgentCodeReview.Host/              ← CLI entry point (Program.cs, commands)
├── MultiAgentCodeReview.Orchestration/     ← CodeReviewPipeline, DI, GitOperationsTool, CodeAnalysisTool
├── MultiAgentCodeReview.Agents/            ← 8 AutoGen agents (Triage, Security, Logic, Performance,
│                                               Modernization, Synthesis, Documentation, Onboarding)
└── MultiAgentCodeReview.Core/              ← Interfaces, models, prompts, config, rate-limiting infra
```

**Existing interfaces OpenCode must know about:**
- `ITriageAgent` — classifies changes, routes to specialists
- `ISpecialistAgent` — Security, Logic, Performance, Modernization
- `ISynthesisAgent` — merges all findings into final report
- `IDocumentationAgent` — generates README/architecture docs
- `IOnboardingAgent` — answers questions about the codebase (AnswerAsync already implemented)
- `ILlmClient` — defined but NOT implemented (agents use AutoGen directly)
- `IKnowledgeSearchTool` — RAG interface, NO implementation
- `PipelineContext` — the shared context object passed through all stages
- `Finding` — individual issue model (severity, file, line, message, agent name)
- `TriageResult` — output of triage stage

**Existing pipeline stages (CodeReviewPipeline.RunReviewAsync):**
1. Filter → git diff → dependency graph → select relevant files
2. Triage → LLM classifies changes → routes to specialist agents
3. Specialists → sequential with 2s delay
4. Synthesis → merges findings → markdown report

**Separate CLI command (not in pipeline):**
- Docs → DocumentationAgent.GenerateDocumentationAsync (fire-and-forget)

**Existing CLI commands:**
- `dotnet run -- review /path/to/repo <commit-hash> [base-commit]`
- `dotnet run -- ask /path/to/repo "question"` ← stub, not wired
- `dotnet run -- docs /path/to/repo <commit-hash> [base-commit]`

---

## 2. What We Are Building — The New Project

> ✅ DECIDED: Add one new project only.

```
MultiAgentCodeReview.McpServer/   ← NEW project, this is everything we build
```

This project:
- Is a .NET 10 console app
- References `MultiAgentCodeReview.Orchestration` and `MultiAgentCodeReview.Agents`
- Uses `ModelContextProtocol` NuGet package (official Microsoft/Anthropic SDK)
- Communicates with OpenCode via **stdio transport only** (no HTTP, no SSE, no ports)
- Exposes exactly **4 MCP tools** (see Phase 3)
- Has its own `appsettings.json` for MCP-specific config

---

## 3. The 4 MCP Tools — Exact Definitions

> ✅ DECIDED: These are the only 4 tools. Do not add more without asking.

### Tool 1: `review_repo`
```
Name:        review_repo
Description: Run a full multi-agent code review on a git repository.
             Runs all 4 pipeline stages: filter, triage, specialists, synthesis.
             Returns a prioritized markdown findings report.
Parameters:
  - repo_path    (string, required) — absolute path to the git repo on disk
  - commit_hash  (string, required) — the commit to review (HEAD, sha, branch name)
  - base_commit  (string, optional) — base to diff against (defaults to commit_hash~1)
Returns:     string — the full markdown report from SynthesisAgent
```

### Tool 2: `ask_codebase`
```
Name:        ask_codebase
Description: Ask a natural language question about a codebase.
             Uses Roslyn dependency graph + OnboardingAgent to answer.
             Good for: "where is auth handled?", "what calls IUserRepository?",
             "which files would break if I change X?"
Parameters:
  - repo_path  (string, required) — absolute path to the git repo on disk
  - question   (string, required) — the natural language question
Returns:     string — OnboardingAgent's answer grounded in Roslyn analysis
```

### Tool 3: `get_last_report`
```
Name:        get_last_report
Description: Returns the most recent review report for a repo path without re-running the pipeline.
             Returns null/empty if no review has been run yet for this path.
Parameters:
  - repo_path  (string, required) — absolute path to the git repo on disk
Returns:     string — cached markdown report, or "No report found for this repo."
```

### Tool 4: `generate_docs`
```
Name:        generate_docs
Description: Generate project documentation for a codebase.
             Uses DocumentationAgent to create README, API docs, architecture docs.
             Use when user asks to generate/update documentation.
Parameters:
  - repo_path    (string, required) — absolute path to the git repo on disk
  - commit_hash  (string, required) — the commit to document (HEAD, sha, branch name)
  - base_commit  (string, optional) — base to diff against (defaults to commit_hash~1)
Returns:     string — generated documentation in markdown format
```

---

## 4. Phase Plan — Exact Execution Order

> ⚠️ CONSTRAINT: OpenCode executes one phase at a time.
> It must print "✅ Phase N complete. Ready for Phase N+1?" after each phase and wait.

---

### Phase 1 — Read and Verify (No code written)

OpenCode must:
1. Read `MultiAgentCodeReview.slnx` and list all projects found
2. Read `MultiAgentCodeReview.Orchestration/Pipeline/CodeReviewPipeline.cs` — print the public method signatures only
3. Read `MultiAgentCodeReview.Agents/DocumentationAgent.cs` — confirm `AnswerAsync` exists and print its signature
4. Read `MultiAgentCodeReview.Core/Models/` — list all model classes found
5. Read `MultiAgentCodeReview.Host/Program.cs` — print how DI is currently configured

Then print:
```
PHASE 1 COMPLETE.
Found projects: [list]
CodeReviewPipeline public methods: [list]
OnboardingAgent.AnswerAsync signature: [signature]
Models found: [list]
DI setup summary: [summary]

🔴 DECISION REQUIRED:
Does this match what you expected?
Type YES to continue to Phase 2, or describe what's wrong.
```

---

### Phase 2 — Scaffold the New Project

> ✅ DECIDED: Create the project with this exact structure.

Files to create (nothing else):
```
MultiAgentCodeReview.McpServer/
├── MultiAgentCodeReview.McpServer.csproj
├── Program.cs
├── appsettings.json
└── Tools/
    └── CodeReviewMcpTools.cs    ← stub only, empty tool methods
```

`MultiAgentCodeReview.McpServer.csproj` must:
- Target `net10.0`
- Reference `ModelContextProtocol` (latest stable, check NuGet — do NOT hardcode a version you invented)
- Reference `MultiAgentCodeReview.Orchestration`
- Reference `MultiAgentCodeReview.Agents`
- Reference `Microsoft.Extensions.Hosting`

`Program.cs` must:
- Use .NET Generic Host
- Register `AddMcpServer().WithStdioServerTransport().WithTools<CodeReviewMcpTools>()`
- Wire DI from Orchestration (reuse the same DI setup as Host if possible)
- Load `.env` using DotNetEnv (already in the solution)

`CodeReviewMcpTools.cs` must:
- Be a class with `[McpServerToolType]` attribute
- Have 4 stub methods matching the tool definitions in Section 3
- Each stub returns `Task<string>` and just returns `"NOT_IMPLEMENTED"` for now
- Have `[McpServerTool]` and `[Description("...")]` attributes on each method

After creating these files, print:
```
PHASE 2 COMPLETE. New project scaffolded.

🔴 DECISION REQUIRED — Before wiring DI:
The existing Host project sets up DI. I can either:
  A) Copy the DI setup into McpServer/Program.cs (duplicated but isolated)
  B) Extract shared DI setup to a new static class in Orchestration (cleaner, touches existing code)

Which do you prefer? Type A or B.
```

---

### Phase 3 — Wire Tool 1: `review_repo`

> ⚠️ CONSTRAINT: Do not modify CodeReviewPipeline. Inject it, call it.

OpenCode must:
1. Inject `CodeReviewPipeline` into `CodeReviewMcpTools` via constructor
2. Implement `review_repo` to:
   - Build a `PipelineContext` from `repo_path`, `commit_hash`, `base_commit`
   - Call the orchestrator's existing run method (exact method name from Phase 1 discovery)
   - Return the synthesis markdown string
3. Add a simple in-memory cache (`Dictionary<string, string>`) to store the last report per `repo_path`
   — this serves `get_last_report` in Phase 5

After implementing, print:
```
PHASE 3 COMPLETE.

🔴 DECISION REQUIRED — Report caching:
The in-memory cache resets when the MCP server process restarts.
Should I also write the report to a file at: {repo_path}/.codereview/last_report.md ?
This makes get_last_report work across server restarts.
Type YES to write to file too, or NO for in-memory only.
```

---

### Phase 4 — Wire Tool 2: `ask_codebase`

> ⚠️ CONSTRAINT: OnboardingAgent.AnswerAsync is already implemented. Call it exactly as-is.

OpenCode must:
1. Inject `IOnboardingAgent` (or `OnboardingAgent` directly — check what's registered in DI from Phase 1)
2. Implement `ask_codebase` to:
   - Build a `PipelineContext` with `repo_path` (no commit needed — just the repo)
   - Run `CodeAnalysisTool` to populate the Roslyn dependency graph into context
   - Call `onboardingAgent.AnswerAsync(context, question)`
   - Return the answer string

After implementing, print:
```
PHASE 4 COMPLETE.

🔴 DECISION REQUIRED — Roslyn analysis cost:
Running CodeAnalysisTool on every ask_codebase call re-parses the entire codebase.
For a large repo this could take 5-15 seconds per question.
Should I:
  A) Re-run analysis on every call (always fresh, slower)
  B) Cache the Roslyn graph per repo_path, invalidate on next review_repo call (faster)

Type A or B.
```

---

### Phase 5 — Wire Tool 3: `get_last_report`

> ✅ DECIDED: This is the simplest tool. No LLM calls, no pipeline.

OpenCode must:
1. Check the in-memory cache first
2. If YES was chosen for file caching in Phase 3, also check `{repo_path}/.codereview/last_report.md`
3. Return the report string or `"No report found for this repo path."`

After implementing, print:
```
PHASE 5 COMPLETE.
```

---

### Phase 6 — Wire Tool 4: `generate_docs`

> ⚠️ CONSTRAINT: DocumentationAgent.GenerateDocumentationAsync is already implemented. Call it exactly as-is.

OpenCode must:
1. Inject `AgentFactory` into `CodeReviewMcpTools` (already available from Phase 3)
2. Implement `generate_docs` to:
   - Build a `PipelineContext` from `repo_path`, `commit_hash`, `base_commit`
   - Call `RunReviewAsync()` to get `ReviewOutput` (needed for synthesis result)
   - Create `DocumentationAgent` via `_agentFactory.CreateDocumentationAgent()`
   - Call `docAgent.GenerateDocumentationAsync(context, result)`
   - Return the documentation string

After implementing, print:
```
PHASE 6 COMPLETE. All 4 tools implemented.
```

---

### Phase 7 — OpenCode Config

> ✅ DECIDED: Generate the opencode.json snippet and a README section.

OpenCode must create:

**`opencode.snippet.json`** (not a full config, just the MCP block to paste):
```json
{
  "mcp": {
    "code-review-agents": {
      "type": "local",
      "command": ["dotnet", "run", "--project", "./MultiAgentCodeReview.McpServer"],
      "enabled": true,
      "environment": {
        "MULTIAGENT_GROQ_API_KEY": "YOUR_GROQ_KEY_HERE",
        "MULTIAGENT_MODEL": "llama-3.3-70b-versatile"
      }
    }
  }
}
```

**`MCP_SETUP.md`** — a short setup guide:
- How to add to opencode.json
- How to test the server starts: `dotnet run --project MultiAgentCodeReview.McpServer`
- Example OpenCode prompts for each of the 4 tools
- Note about Groq rate limits (1000 RPD, 8 agents per review = ~125 reviews/day)

After creating both files, print:
```
PHASE 7 COMPLETE.

🔴 DECISION REQUIRED — Final check before done:
I have not run the project yet. Should I:
  A) Run: dotnet build MultiAgentCodeReview.McpServer and show you the output
  B) Stop here — you'll build it yourself

Type A or B.
```

---

## 5. What OpenCode Must NEVER Do

> ⚠️ HARD CONSTRAINTS — violations mean stopping immediately and asking Bhavya

```
❌ Never rename existing interfaces (ITriageAgent, ISpecialistAgent, etc.)
❌ Never rewrite existing agent classes — only inject and call them
❌ Never add NuGet packages not listed in a phase without asking
❌ Never create more than 4 files per phase without asking
❌ Never make architectural decisions about DI, caching, or file structure without asking
❌ Never invent method signatures — read the actual source first (Phase 1)
❌ Never hardcode API keys in any file
❌ Never add HTTP endpoints — stdio only
❌ Never modify CodeReviewPipeline.cs, any Agent class, or any Core model
❌ Never run dotnet run without being asked to in a DECISION REQUIRED block
```

---

## 6. Hallucination Guardrails

> These are the most likely points where OpenCode will invent things. Check these explicitly.

**ModelContextProtocol SDK:**
- The attribute is `[McpServerTool]` on methods and `[McpServerToolType]` on the class
- Check the actual NuGet package version before writing the `.csproj` — do not invent "0.1.0"
- The correct NuGet package name is `ModelContextProtocol` — verify with `dotnet add package ModelContextProtocol --dry-run`
- Transport registration: `.WithStdioServerTransport()` — verify this method exists in the version you install

**CodeReviewPipeline:**
- Do not assume the method is called `RunAsync` — it's `RunReviewAsync`
- Do not assume `PipelineContext` constructor parameters — it's a record with named parameters
- Do not assume `OnboardingAgent` is registered as `IOnboardingAgent` in DI — it's created via `AgentFactory.CreateOnboardingAgent()`

**OpenCode config:**
- The correct config key is `"mcp"` not `"mcpServers"` (that's Claude Desktop format)
- OpenCode uses `"type": "local"` and `"command": [...]` array format
- Environment variables go under `"environment"` not `"env"` in OpenCode config

---

## 7. Lessons from Production System Prompts (from system_prompts_leaks repo)

> These patterns come from studying Claude Code's actual system prompt structure.
> They are why this AGENTS.md is structured this way.

**Claude Code's actual approach (claude-code-2.1.172-fable-5.md):**
- Tools run behind a user-selected permission mode — a denied call means the user declined, adjust don't retry
- Agents are spawned for specific sub-tasks (Explore agent = read-only search, Guide agent = Q&A)
- The system distinguishes between the orchestrator (Claude Code itself) and sub-agents (spawned for tasks)
- `<system-reminder>` tags are injected by the harness, not the user — treat them differently

**Applied to your MCP server:**
- Your `review_repo` tool IS the orchestrator call — it fans out to all 8 agents
- Your `ask_codebase` tool is equivalent to Claude Code's Guide agent pattern
- Your `get_last_report` tool is equivalent to Claude Code's read-only Explore agent pattern
- Your `generate_docs` tool is a specialized task agent for documentation generation
- If OpenCode calls `review_repo` and it fails partway through, it should return partial results
  with a clear error note — not crash the MCP server process

**Tool description quality (from studying production prompts):**
- Every tool description must say WHEN to use it AND when NOT to use it
- The description is what the LLM reads to decide which tool to call — be specific
- Bad: `"Reviews code"` — Good: `"Run a full multi-agent code review on a git repository. Use this when the user asks to review a commit, check a PR, or audit recent changes. Do NOT use for single-file questions — use ask_codebase instead."`

---

## 8. Quick Reference — Decisions You Will Be Asked

For your records, here are all the DECISION REQUIRED stops in order:

| # | Phase | Question | Options |
|---|-------|----------|---------|
| 1 | Phase 1 | Does the discovered structure match what you expected? | YES / describe issue |
| 2 | Phase 2 | How to share DI setup between Host and McpServer? | A (copy) / B (extract) |
| 3 | Phase 3 | Should reports also be written to disk? | YES / NO |
| 4 | Phase 4 | How to handle Roslyn analysis cost for ask_codebase? | A (always fresh) / B (cache) |
| 5 | Phase 7 | Should I run dotnet build to verify? | A (run build) / B (stop here) |

---

## 9. Definition of Done

The MCP server transformation is complete when:

- [ ] `MultiAgentCodeReview.McpServer` project builds with `dotnet build` — zero errors
- [ ] `dotnet run --project MultiAgentCodeReview.McpServer` starts without crashing
- [ ] `opencode.snippet.json` exists and has the correct OpenCode config format
- [ ] `MCP_SETUP.md` exists with setup instructions
- [ ] All 4 tools return real data (not `"NOT_IMPLEMENTED"`)
- [ ] No existing project files were modified
- [ ] No new packages were added without being listed in this file or explicitly approved

---

*AGENTS.md — Generated for Bhavya's multi-agent code review MCP transformation*
*Study references: asgeirtj/system_prompts_leaks — Claude Code bundled-skills/code-review.md,*
*claude-code-2.1.172-fable-5.md tool definition patterns, OpenCode MCP local server config format*
