# VS Code Extensions Guide

This document describes the recommended VS Code extensions for the POGO Community monorepo and their specific use cases.

## 🔧 Essential Extensions

### Code Quality & Formatting

#### **ESLint** (`dbaeumer.vscode-eslint`)

- **Purpose**: JavaScript/TypeScript linting and code quality enforcement
- **Use Cases**:
  - Catches syntax errors and code quality issues
  - Enforces coding standards across the monorepo
  - Provides real-time feedback while coding
  - Auto-fixes common issues on save
- **Why Essential**: Ensures consistent code quality across API, Bot, and Mobile apps

#### **Prettier** (`esbenp.prettier-vscode`)

- **Purpose**: Code formatting and style consistency
- **Use Cases**:
  - Automatically formats code on save
  - Enforces consistent indentation, spacing, and line breaks
  - Works with TypeScript, JavaScript, JSON, and Markdown
  - Resolves formatting conflicts in team development
- **Why Essential**: Maintains consistent code style across the entire monorepo

### React Native & Mobile Development

#### **React Native Tools** (`msjsdiag.vscode-react-native`)

- **Purpose**: React Native development support and debugging
- **Use Cases**:
  - Debug React Native apps directly in VS Code
  - Integrated Metro bundler support
  - Device and simulator management
  - Performance profiling tools
  - Hot reload and fast refresh
- **Why Essential**: Critical for mobile app development and debugging

#### **Expo Tools** (`expo.vscode-expo-tools`)

- **Purpose**: Expo development workflow integration
- **Use Cases**:
  - Quick access to Expo commands
  - QR code scanning for device testing
  - Expo project management
  - Development server controls
- **Why Essential**: Streamlines Expo-based mobile development

### Testing

#### **Jest** (`orta.vscode-jest`)

- **Purpose**: Jest testing framework integration
- **Use Cases**:
  - Run tests directly from VS Code
  - Real-time test results and coverage
  - Debug tests with breakpoints
  - Test discovery and execution
  - Coverage reporting
- **Why Essential**: Enables efficient testing workflow for all apps

### Git & Version Control

#### **GitLens** (`eamodio.gitlens`)

- **Purpose**: Enhanced Git capabilities and code history
- **Use Cases**:
  - Inline blame annotations
  - Code authorship tracking
  - Commit history visualization
  - Branch comparison tools
  - Repository insights
- **Why Essential**: Improves collaboration and code review process

### Development Productivity

#### **Turbo Console Log** (`chakrounanas.turbo-console-log`)

- **Purpose**: Automated console.log statement generation
- **Use Cases**:
  - Quick debugging with console statements
  - Automatic variable logging
  - Comment and uncomment logs easily
  - Clean up logs before commits
- **Why Essential**: Speeds up debugging across all apps

#### **Path Intellisense** (`christian-kohler.path-intellisense`)

- **Purpose**: File path autocompletion
- **Use Cases**:
  - Auto-complete file paths in imports
  - Reduce typos in file references
  - Navigate project structure easily
  - Works with relative and absolute paths
- **Why Essential**: Prevents import errors and improves navigation

### UI & Styling

#### **Tailwind CSS IntelliSense** (`bradlc.vscode-tailwindcss`)

- **Purpose**: Tailwind CSS class autocompletion and validation
- **Use Cases**:
  - Auto-complete Tailwind classes
  - Class validation and suggestions
  - Hover previews for styles
  - Color picker integration
- **Why Essential**: If using Tailwind CSS in web components

### TypeScript & JavaScript

#### **TypeScript Importer** (Built-in)

- **Purpose**: Enhanced TypeScript support
- **Use Cases**:
  - Auto-import TypeScript modules
  - Type checking and error highlighting
  - IntelliSense for TypeScript
  - Refactoring tools
- **Why Essential**: Core TypeScript development across all apps

#### **Auto Rename Tag** (`formulahendry.auto-rename-tag`)

- **Purpose**: Automatically rename paired HTML/JSX tags
- **Use Cases**:
  - Rename opening and closing tags simultaneously
  - Prevents mismatched tags
  - Works with JSX in React Native
- **Why Essential**: Prevents JSX tag mismatches in mobile app

#### **JSON** (`ms-vscode.vscode-json`)

- **Purpose**: JSON file support and validation
- **Use Cases**:
  - JSON syntax highlighting
  - JSON validation and error detection
  - JSON formatting
  - Schema validation
- **Why Essential**: Handles configuration files (package.json, tsconfig.json, etc.)

## 🎯 Extension Categories by App

### API Backend Development

- **ESLint** - Code quality
- **Prettier** - Code formatting
- **Jest** - Testing
- **GitLens** - Version control
- **TypeScript** - Type safety

### Discord Bot Development

- **ESLint** - Code quality
- **Prettier** - Code formatting
- **Jest** - Testing
- **GitLens** - Version control
- **TypeScript** - Type safety
- **Turbo Console Log** - Debugging

### Mobile App Development

- **ESLint** - Code quality
- **Prettier** - Code formatting
- **Jest** - Testing
- **React Native Tools** - Mobile debugging
- **Expo Tools** - Expo workflow
- **Auto Rename Tag** - JSX support
- **GitLens** - Version control
- **TypeScript** - Type safety

## 🚀 Getting Started

1. **Install Extensions**: VS Code will prompt you to install recommended extensions when you open the workspace
2. **Manual Installation**: Use `Ctrl+Shift+X` (or `Cmd+Shift+X` on Mac) to open the Extensions panel
3. **Workspace Settings**: The `.vscode/settings.json` file configures these extensions automatically
4. **Recommended Extensions**: The `.vscode/extensions.json` file contains the essential extensions for this project

## 📝 Configuration

The extensions are pre-configured in `.vscode/settings.json` with:

- Auto-format on save with Prettier
- ESLint auto-fix on save
- TypeScript validation settings
- File associations for proper syntax highlighting
- Organize imports on save
- Prettier as default formatter for all supported file types

### Essential Extensions (Auto-Recommended)

The following extensions are automatically recommended when opening the workspace:

- **Prettier** (`esbenp.prettier-vscode`) - Code formatting
- **ESLint** (`dbaeumer.vscode-eslint`) - Linting and code quality
- **EditorConfig** (`editorconfig.editorconfig`) - Editor configuration consistency

## 🔄 Updates

Extensions will be updated automatically by VS Code. To manually update:

1. Open Extensions panel (`Ctrl+Shift+X`)
2. Click the "Update All" button or update individual extensions

---

_This extension setup is optimized for the POGO Community monorepo development workflow, supporting TypeScript, React Native, Node.js, and modern development practices._
