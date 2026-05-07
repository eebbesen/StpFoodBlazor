#!/bin/bash
# Reads the PostToolUse hook JSON from stdin and generates an HTML coverage
# report if the triggering Bash command was a dotnet test run.

# When invoked by the Claude Code hook, stdin has JSON describing the Bash command.
# When run directly by a person, skip the stdin check entirely.
if ! [ -t 0 ]; then
    INPUT=$(cat)
    COMMAND=$(echo "$INPUT" | python3 -c "import sys,json; d=json.load(sys.stdin); print(d.get('tool_input',{}).get('command',''))" 2>/dev/null)
    if ! echo "$COMMAND" | grep -q "dotnet test"; then
        exit 0
    fi
fi

PROJECT_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
REPORT_DIR="$PROJECT_ROOT/StpFoodBlazorTest/coveragereport"

# Search the whole project for the most recently modified coverage file,
# so the script works regardless of which directory dotnet test was run from.
LATEST_XML=$(find "$PROJECT_ROOT" -name "coverage.opencover.xml" -newer "$PROJECT_ROOT/StpFoodBlazorTest/StpFoodBlazorTest.csproj" 2>/dev/null \
    | xargs ls -t 2>/dev/null | head -1)

if [ -z "$LATEST_XML" ]; then
    echo "Coverage report: no coverage.opencover.xml found under $PROJECT_ROOT" >&2
    exit 0
fi

REPORTGEN=$(which reportgenerator 2>/dev/null || echo "$HOME/.dotnet/tools/reportgenerator")

echo "Generating coverage report from: $LATEST_XML"
"$REPORTGEN" \
    "-reports:$LATEST_XML" \
    "-targetdir:$REPORT_DIR" \
    "-reporttypes:Html" 2>&1

echo "Coverage report ready: $REPORT_DIR/index.html"
