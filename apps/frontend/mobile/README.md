# 📱 POGO Community Mobile App

A React Native/Expo mobile application for the POGO Community ecosystem, providing cross-platform access to raid coordination, player management, and community features.

## 📋 Prerequisites

### General Requirements

- **Node.js** >= 18.0.0
- **pnpm** >= 8.0.0 (recommended) or npm
- **Expo CLI** (`npm install -g @expo/cli`)
- **API Backend** running (for data operations)

### Platform-Specific Requirements

#### Web Development

- Modern web browser (Chrome, Firefox, Safari, Edge)
- No additional setup required

#### iOS Development

- **macOS** (required for iOS development)
- **Xcode** >= 14.0 (from Mac App Store)
- **iOS Simulator** (included with Xcode)
- **CocoaPods** (`sudo gem install cocoapods`)
- **Apple Developer Account** (for device testing)

#### Android Development

- **Android Studio** (latest stable version)
- **Android SDK** (API level 33+)
- **Java Development Kit (JDK)** 11 or 17
- **Android Emulator** or physical device
- **Android Keystore** (for production builds)

## 🛠️ Installation

1. **Navigate to the mobile app directory:**

   ```bash
   cd apps/frontend/mobile
   ```

2. **Install dependencies:**

   ```bash
   pnpm install
   # or
   npm install
   ```

3. **Set up environment variables:**
   Create a `.env` file in the mobile app directory:

   ```env
   # API Configuration
   API_BASE_URL=http://localhost:8080/api/v1

   # Google Maps Configuration
   GOOGLE_MAPS_API_KEY=your-google-maps-api-key

   # Google Cloud Configuration
   GOOGLE_CLOUD_PROJECT_ID=your-google-cloud-project-id

   # App Configuration
   APP_NAME=POGO Community
   APP_VERSION=1.0.0
   ```

4. **Set up Google Cloud Services:**
   - Enable Google Cloud Logging API
   - Create a service account with appropriate permissions
   - Set up authentication (see [Google Cloud Authentication](https://cloud.google.com/docs/authentication))

## 🔧 Environment Variables

| Variable                  | Description             | Required | Default                        |
| ------------------------- | ----------------------- | -------- | ------------------------------ |
| `API_BASE_URL`            | Backend API base URL    | Yes      | `http://localhost:8080/api/v1` |
| `GOOGLE_MAPS_API_KEY`     | Google Maps API key     | Yes      | -                              |
| `GOOGLE_CLOUD_PROJECT_ID` | Google Cloud project ID | Yes      | -                              |
| `APP_NAME`                | Application name        | No       | `POGO Community`               |
| `APP_VERSION`             | Application version     | No       | `1.0.0`                        |

## 🚀 Running the Application

### Web Development (Recommended for Quick Start)

```bash
# Start Expo development server for web
pnpm run web
# or
pnpm run dev
```

The app will open in your default browser at `http://localhost:19006`

### iOS Development

#### Prerequisites Setup

1. **Install Xcode:**

   - Download from Mac App Store
   - Install Xcode Command Line Tools: `xcode-select --install`

2. **Install CocoaPods:**

   ```bash
   sudo gem install cocoapods
   ```

3. **Install iOS dependencies:**
   ```bash
   cd ios
   pod install
   cd ..
   ```

#### Running on iOS

```bash
# Start iOS simulator
pnpm run ios

# Run on specific device
pnpm run ios-device
```

### Android Development

#### Prerequisites Setup

1. **Install Android Studio:**

   - Download from [developer.android.com](https://developer.android.com/studio)
   - Install Android SDK (API level 33+)
   - Set up Android emulator or connect physical device

2. **Configure Environment:**

   ```bash
   # Add to your ~/.bashrc or ~/.zshrc
   export ANDROID_HOME=$HOME/Android/Sdk
   export PATH=$PATH:$ANDROID_HOME/emulator
   export PATH=$PATH:$ANDROID_HOME/tools
   export PATH=$PATH:$ANDROID_HOME/tools/bin
   export PATH=$PATH:$ANDROID_HOME/platform-tools
   ```

3. **Enable Developer Options:**
   - On Android device: Settings > About Phone > Tap "Build Number" 7 times
   - Enable "USB Debugging" in Developer Options

#### Running on Android

```bash
# Start Android emulator or connect device
pnpm run android
```

## 🗺️ Google Maps Setup

### Getting Google Maps API Key

1. **Go to Google Cloud Console:**

   - Visit [console.cloud.google.com](https://console.cloud.google.com)
   - Create a new project or select existing one

2. **Enable Maps APIs:**

   - Go to "APIs & Services" > "Library"
   - Enable "Maps JavaScript API"
   - Enable "Places API"
   - Enable "Geocoding API"

3. **Create API Key:**

   - Go to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "API Key"
   - Restrict the key to your domains/IPs for security

4. **Configure for React Native:**
   - For Android: Add package name and SHA-1 fingerprint
   - For iOS: Add bundle identifier

### Maps Configuration

The app uses `react-native-maps` with Google Maps integration:

```typescript
// Example configuration
const mapConfig = {
  apiKey: process.env.GOOGLE_MAPS_API_KEY,
  region: {
    latitude: 50.8503, // Default to Belgium
    longitude: 4.3517,
    latitudeDelta: 0.0922,
    longitudeDelta: 0.0421,
  },
};
```

## 🏗️ Architecture

### Navigation Structure

The app uses React Navigation with multiple navigation patterns:

- **Bottom Tabs** - Main app navigation
- **Drawer Navigation** - Side menu for additional features
- **Stack Navigation** - Screen transitions and modals

### Dependency Injection

The app uses **Inversify** for dependency injection:

- Clean separation of concerns
- Easy testing and mocking
- Loose coupling between components

### State Management

- **React Context** - Global app state
- **Local State** - Component-specific state
- **Async Storage** - Persistent data storage

### Screen Structure

- **Home Screen** - Main dashboard
- **Raids Screen** - Raid management and coordination
- **Players Screen** - Player directory and profiles
- **Gyms Screen** - Gym locations and information
- **Settings Screen** - App configuration and preferences

## 📱 Platform-Specific Features

### Web Platform

- **Responsive Design** - Adapts to different screen sizes
- **PWA Support** - Progressive Web App capabilities
- **Browser Compatibility** - Works on all modern browsers

### iOS Platform

- **Native Performance** - Optimized for iOS devices
- **iOS Design Guidelines** - Follows Apple's Human Interface Guidelines
- **Device Integration** - Camera, location services, notifications

### Android Platform

- **Material Design** - Follows Google's Material Design principles
- **Android Permissions** - Proper permission handling
- **Device Integration** - Camera, location services, notifications

## 🐳 Docker

> **TODO**: Docker configuration will be added in a future update.
>
> This section will include:
>
> - Dockerfile for the mobile app
> - Docker Compose configuration
> - Environment variable handling
> - Multi-platform build support
> - Development environment setup

## 🧪 Testing

```bash
# Run all tests
pnpm test

# Run tests in watch mode
pnpm test:watch

# Run tests with coverage
pnpm test:coverage
```

## 🔍 Linting & Formatting

```bash
# Lint code
pnpm lint

# Fix linting issues
pnpm lint:fix

# Format code
pnpm format

# Check formatting
pnpm format:check
```

## 📦 Building for Production

### Web Build

```bash
# Build for web
pnpm run build:web

# The built files will be in the web-build directory
```

### iOS Build

```bash
# Build for iOS (requires macOS)
# Open ios/POGOCommunityApp.xcworkspace in Xcode
# Archive and distribute through Xcode
```

### Android Build

```bash
# Build APK
cd android
./gradlew assembleRelease

# Build AAB (for Google Play Store)
./gradlew bundleRelease
```

## 🚨 Troubleshooting

### Common Issues

1. **Metro Bundler Issues**

   ```bash
   # Clear Metro cache
   npx expo start --clear
   # or
   pnpm start --clear
   ```

2. **iOS Build Issues**

   - Clean Xcode build folder: Product > Clean Build Folder
   - Reset iOS Simulator: Device > Erase All Content and Settings
   - Reinstall pods: `cd ios && pod install && cd ..`

3. **Android Build Issues**

   - Clean Gradle cache: `cd android && ./gradlew clean && cd ..`
   - Check Android SDK installation
   - Verify environment variables

4. **Google Maps Not Loading**

   - Verify API key is correct
   - Check API restrictions and quotas
   - Ensure proper domain/bundle ID configuration

5. **API Connection Issues**

   - Verify API backend is running
   - Check `API_BASE_URL` configuration
   - Test API endpoints directly

6. **Expo/React Native Issues**
   - Update Expo CLI: `npm install -g @expo/cli@latest`
   - Clear npm cache: `npm cache clean --force`
   - Delete node_modules and reinstall

### Platform-Specific Debugging

#### iOS Debugging

- Use Xcode debugger for native issues
- Check iOS Simulator logs
- Use React Native Debugger

#### Android Debugging

- Use Android Studio debugger
- Check logcat: `adb logcat`
- Use React Native Debugger

#### Web Debugging

- Use browser developer tools
- Check console for errors
- Use React Developer Tools

## 📚 Additional Resources

- [React Native Documentation](https://reactnative.dev/)
- [Expo Documentation](https://docs.expo.dev/)
- [React Navigation](https://reactnavigation.org/)
- [React Native Maps](https://github.com/react-native-maps/react-native-maps)
- [Google Maps Platform](https://developers.google.com/maps)
- [Inversify Documentation](https://inversify.io/)
- [React Native Elements](https://reactnativeelements.com/)

## 🔄 Development Workflow

### Recommended Development Flow

1. **Start with Web** - Quick iteration and testing
2. **Test on iOS Simulator** - iOS-specific features
3. **Test on Android Emulator** - Android-specific features
4. **Test on Physical Devices** - Real-world performance

### Code Organization

```
src/
├── components/     # Reusable UI components
├── screens/        # Screen components
├── navigation/     # Navigation configuration
├── services/       # API and external services
├── stores/         # State management
├── utils/          # Utility functions
└── types/          # TypeScript type definitions
```
