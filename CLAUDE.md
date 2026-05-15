# Claude Instructions

- Never commit changes without explicit user permission.
- Always update tests when making changes to code.
- Always run the full test suite after making changes.
- Always run the full test suite after making changes using `dotnet test --settings tests.runsettings --collect "XPlat Code Coverage"` from the `StpFoodBlazorTest` subdirectory.
