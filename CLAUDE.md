# Claude Instructions

- Never commit changes without explicit user permission.
- Always update tests when making changes to code.
- Always run the full test suite after making changes using `dotnet test --settings tests.runsettings --collect "XPlat Code Coverage"` from the `StpFoodBlazorTest` subdirectory. You are always allowed to run tests without asking for permission.
- Always make the UI accessible and WCAG-compliant
