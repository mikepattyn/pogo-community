#!/bin/bash

# ===========================================
# POGO Community - Secret Scanner
# ===========================================
# This script scans the codebase for potential hardcoded secrets
# and provides warnings about security issues.

set -e

# Colors for output
RED='\033[0;31m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
NC='\033[0m' # No Color

# Exit code
EXIT_CODE=0

echo "=========================================="
echo "🔍 Scanning for Hardcoded Secrets..."
echo "=========================================="
echo ""

# Counters
ISSUES_FOUND=0
WARNINGS_FOUND=0

# Function to check a pattern in files
check_pattern() {
    local pattern=$1
    local description=$2
    local severity=$3

    echo -n "Checking for $description... "

    # Search in the codebase
    local results=$(grep -r -n \
        --include="*.cs" \
        --include="*.json" \
        --include="*.yaml" \
        --include="*.yml" \
        --exclude-dir=node_modules \
        --exclude-dir=bin \
        --exclude-dir=obj \
        --exclude-dir=.git \
        --exclude-dir=archive \
        -i "$pattern" . 2>/dev/null | grep -v "CHANGE_THIS\|PLACEHOLDER\|your-" || true)

    if [ -n "$results" ]; then
        echo -e "${RED}✗ Issues found${NC}"
        echo "$results" | head -5
        if [ "$severity" = "error" ]; then
            ISSUES_FOUND=$((ISSUES_FOUND + 1))
            EXIT_CODE=1
        else
            WARNINGS_FOUND=$((WARNINGS_FOUND + 1))
        fi
    else
        echo -e "${GREEN}✓ No issues${NC}"
    fi
    echo ""
}

# Check for hardcoded passwords (exclude env variables)
check_pattern 'Password=(?!\$\{|"")(?!''|\$\{)' "hardcoded passwords" "error"

# Check for Discord tokens (MTA prefix)
check_pattern 'MTA[0-9A-Za-z._-]{40,}' "Discord bot tokens" "error"

# Check for potential API keys or tokens
check_pattern 'api.*key.*=.*[A-Za-z0-9_-]{20,}' "API keys" "warning"

# Check for SQL connection strings with hardcoded passwords (excluding env vars)
check_pattern 'Password=(?!\$\{|_PASSWORD)(?!'')\w+' "hardcoded passwords in connection strings" "error"

echo "=========================================="
echo "Scan Summary:"
echo -e "  ${RED}Errors: $ISSUES_FOUND${NC}"
echo -e "  ${YELLOW}Warnings: $WARNINGS_FOUND${NC}"
echo "=========================================="

if [ $EXIT_CODE -eq 0 ]; then
    echo -e "${GREEN}✓ No critical issues found${NC}"
    echo ""
    echo "Note: This script checks for common patterns."
    echo "Always review your code manually before committing."
else
    echo -e "${RED}✗ Critical issues found${NC}"
    echo ""
    echo "⚠️  Please fix the issues above before committing to git."
fi
echo "=========================================="

exit $EXIT_CODE
