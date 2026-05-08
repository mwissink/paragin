# Copilot Instructions

## General Guidelines
- Namespaces should start with `Charged.Cloud.Headless.DevicesWithErrorsToCrm`.
- All required project settings (nullable reference types, warnings as errors, NuGet security/outdated-package audit directives, and central package management) are configured globally in `Directory.Build.props` and `Directory.Packages.props`. Do not duplicate these settings in individual project files.
- Use `var`. By default don't use explicit types. The only exception is when passing lambda's to OneOf.Match / OneOf.Switch, where the type of the lambda parameter should be explicitly declared to ensure compiler errors on type change.
- Don't babble, be to the point.
- Don't make changes the user didn't explicitly ask for. Keep changes strictly to what was requested.
- Do not use `TimeProvider`. Times can be obtained from `DateTime.UtcNow` and similar methods.
- Only use latest stable versions of packages. Do not use pre-release versions, even if they seem to have features that would be helpful for the task at hand.
- Only use latest stable version of azure portal resources, functions etc. Do not use preview versions, even if they seem to have features that would be helpful for the task at hand. Check if needed with official docs or release notes to confirm if a feature is in preview or generally available. Check with the Azure platform if they are implemented. Do not downgrade versions unless specifically asked to do so, even if it seems like a newer version has breaking changes that would make the task easier. If a newer version has breaking changes, assume that the breaking changes are intentional and required, and work with them rather than against them.

## When making changes
- Keep changes as minimal as possible to meet the requirements of the task. Don't do extra work, even if it seems "obvious" or "helpful".
- Don't sacrifice readability for brevity. If a change makes the code less readable, don't make it, even if it seems more efficient.

## Code patterns
- Use primary constructors for all classes that take dependencies. Do not use field declarations and constructor bodies unless there is initialization logic.
- Prefer instance-based helpers/services, etc over static, and inject dependencies into the helper/service rather than passing them as method parameters.
- Classes and records should be `sealed` by default unless there is an explicit reason to inherit.
- Concrete service and adapter implementations should be `internal`. Only interfaces are `public`.
- Azure Functions trigger classes are exempt from the `internal` rule and must be `public`, but must still be `sealed`.
- Use `OneOf` for result types. Do not throw exceptions for expected failure cases. Error types are `sealed record`s.
- Use the The Clean Code paradigm.
- Use linq where it would make code more readable, but not for complex queries where it would make code less readable. When in doubt, prefer readability over brevity.
