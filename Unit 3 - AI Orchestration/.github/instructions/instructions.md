---
description: Copilot instructions - Best Python practices and standards
# applyTo: '**/*.py'  # when provided, instructions will be automatically added to requests matching .py files
---

# Copilot — Python Best Practices & Standards 🔧🐍

## Purpose
Provide clear, project-focused guidelines for generating, reviewing, and modifying Python code.

## When to load
Load these instructions for any request that involves creating, editing, or reviewing Python code, tests, or CI configuration.

---

## Style & Formatting ✅
- Follow **PEP 8** for naming and layout (use `snake_case` for functions/variables, `PascalCase` for classes).  
- Use **Black** for formatting and **isort** for imports. Keep lines <= 88 chars (Black default).  
- Keep functions small and single-responsibility.

## Typing & Static Analysis 🔎
- Add **type hints** for public functions and complex internal APIs. Prefer `typing` primitives (`List`, `Dict`, `Optional`) or `from __future__ import annotations`.
- Run **mypy** and fix reported issues. Aim for gradual typing where full typing is not yet possible.

## Linting & CI ✅
- Ensure code passes **flake8** (or repository linter) and **mypy** before proposing changes.  
- Include/adjust CI config when necessary to run tests, lint, and type checks.

## Testing 🧪
- Add **pytest** tests for new features and bug fixes. Include happy-path and edge-case tests.  
- Use fixtures for common setup/teardown. Prefer parameterized tests for similar cases.

## Documentation & Docstrings 📚
- Use **Google-style** or **NumPy-style** docstrings consistently. Provide short summary, args, return, raises examples.
- Update `README` or module-level docs for notable behavior changes.

## Exceptions & Logging ⚠️
- Raise specific exceptions (custom ones if needed) rather than generic `Exception`.  
- Use structured logging with appropriate log levels (`debug`, `info`, `warning`, `error`, `critical`). Avoid printing to stdout in libraries.

## Dependency Management & Packaging 📦
- Prefer `venv`, `pip-tools`, `poetry`, or the repo’s existing method. Pin test/CI extras where applicable.  
- Update `pyproject.toml` / `requirements.txt` / `setup.cfg` when adding runtime or dev dependencies.

## Security & Secrets 🔒
- Never hard-code secrets or credentials. Use environment variables or secret stores.  
- Validate and sanitize external inputs. Follow OWASP basics for web-facing code.

## Performance & Concurrency ⚡
- Profile before optimizing. Prefer clarity over micro-optimizations.  
- For concurrency, prefer `asyncio` or well-understood libraries; ensure proper synchronization when using threads or processes.

## Notebooks & Data Science 🧮
- Keep notebooks for exploration only. Extract production code into `.py` modules with tests.  
- Make results reproducible: set seeds, record package versions, and avoid side effects.

---

## For Copilot — How to Generate or Modify Code 💡
When you produce code changes, follow this checklist:
1. Follow the project's existing style and patterns. Keep diffs minimal and focused. ✅
2. Add or update **unit tests** covering new logic and edge cases. ✅
3. Include **docstrings** and short usage examples for public functions/classes. ✅
4. Ensure **linting** and **type checks** will pass (fix issues you introduce). ✅
5. If adding dependencies, update dependency files and **CI** accordingly. ✅
6. Add a short **CHANGELOG** entry or PR description summarizing behavior changes. ✅

### Don'ts ❌
- Do not change formatting rules arbitrarily (e.g., turning off Black) unless justified and approved.  
- Avoid introducing global mutable state.  
- Don’t commit secrets, large data files, or build artifacts.

---

## Examples
Google-style docstring example:

```python
def add(a: int, b: int) -> int:
    """Add two integers.

    Args:
        a: First integer.
        b: Second integer.

    Returns:
        The sum of `a` and `b`.
    """
    return a + b
```

Pytest example:

```python
import pytest

from mypkg import add

@pytest.mark.parametrize("a,b,expected", [(1, 2, 3), (0, 0, 0), (-1, 1, 0)])
def test_add(a, b, expected):
    assert add(a, b) == expected
```

---

## PR Checklist for Reviewers 🔍
- [ ] Tests added and passing
- [ ] Linting and formatting applied
- [ ] Type checks pass
- [ ] Docs updated where applicable
- [ ] No secrets or large binaries committed

---

## Quick AI Prompts (for internal use) ✍️
- "Create a well-tested, type-annotated Python function that does X and follows project style."  
- "Refactor Y module to small functions, add tests and docstrings, ensure lint and mypy pass."  

---

Keep guidance pragmatic and minimal: prefer readable, well-tested, and typed code over clever or noisy solutions.  

