# 📱 POGO Community Apps - Framework & Package Inventory

## 🏗️ **App 1: POGO Community API (Node.js Backend)**
**Location:** `POGO-Community-API-with-NodeJS-master/`

### **Core Framework & Runtime**
- **Node.js** (v10.19.0) - JavaScript runtime
- **TypeScript** (v3.6.4) - Type-safe JavaScript
- **Express.js** (v4.17.1) - Web framework

### **Key Dependencies**
- **Authentication & Security:**
  - `jsonwebtoken` (v8.5.1) - JWT token handling
  - `password-hash` (v1.2.2) - Password hashing
  - `express-session` (v1.17.0) - Session management

- **Database & ORM:**
  - `mssql` (v5.1.0) - Microsoft SQL Server client
  - `mysql` (v2.17.1) - MySQL client

- **Google Cloud Services:**
  - `@google-cloud/logging` (v7.1.0) - Cloud logging
  - `@google-cloud/vision` (v1.8.0) - Image analysis

- **Dependency Injection:**
  - `inversify` (v5.0.1) - IoC container
  - `inversify-express-utils` (v6.3.2) - Express integration

- **Utilities:**
  - `axios` (v0.19.2) - HTTP client
  - `moment` (v2.24.0) - Date manipulation
  - `dotenv` (v8.2.0) - Environment variables
  - `email-validator` (v2.0.4) - Email validation
  - `pug` (v2.0.4) - Template engine

---

## 📱 **App 2: POGO Community App (React Native Mobile)**
**Location:** `POGO-Community-App-with-ReactNative-master/`

### **Core Framework & Runtime**
- **React Native** (v0.71.4) - Mobile app framework
- **React** (v18.2.0) - UI library
- **Expo** (v48.0.7) - Development platform
- **TypeScript** (v5.0.2) - Type-safe JavaScript

### **Key Dependencies**
- **Navigation:**
  - `@react-navigation/bottom-tabs` (v6.5.7) - Tab navigation
  - `@react-navigation/drawer` (v6.6.2) - Drawer navigation
  - `react-navigation` (v4.1.1) - Navigation library

- **Maps & Location:**
  - `react-native-maps` (v1.4.0) - Native maps
  - `@googlemaps/google-maps-services-js` (v3.3.27) - Google Maps API
  - `react-google-maps` (v9.4.5) - Google Maps React components

- **UI Components:**
  - `react-native-elements` (v3.4.3) - UI component library
  - `react-native-vector-icons` (v9.2.0) - Icon library
  - `@expo/vector-icons` (v13.0.0) - Expo icons
  - `react-native-popup-dialog` (v0.18.3) - Modal dialogs
  - `react-native-numeric-input` (v1.9.1) - Number input

- **Animation & Gestures:**
  - `react-native-animatable` (v1.3.3) - Animations
  - `react-native-gesture-handler` (v2.9.0) - Gesture handling
  - `react-native-reanimated` (v3.0.2) - Advanced animations

- **Platform Support:**
  - `react-native-web` (v0.18.12) - Web support
  - `react-native-unimodules` (v0.6.0) - Universal modules

- **Google Cloud & Services:**
  - `@google-cloud/logging` (v10.4.0) - Cloud logging
  - `inversify` (v6.0.1) - Dependency injection

- **Development Tools:**
  - `babel-preset-expo` (v9.3.0) - Babel configuration
  - `jetifier` (v2.0.0) - Android compatibility

---

## 🤖 **App 3: POGO Community Bot (Discord Bot)**
**Location:** `POGO-Community-Bot-with-Discord.JS-master/`

### **Core Framework & Runtime**
- **Node.js** (v12.14.1) - JavaScript runtime
- **TypeScript** (v3.7.5) - Type-safe JavaScript
- **Discord.js** (v11.5.1) - Discord API wrapper

### **Key Dependencies**
- **Discord Integration:**
  - `discord.js` (v11.5.1) - Discord bot framework
  - `discord-message-handler` (v2.1.1) - Message handling

- **Google Cloud Services:**
  - `@google-cloud/datastore` (v5.0.3) - NoSQL database
  - `@google-cloud/logging` (v7.1.0) - Cloud logging
  - `@google-cloud/vision` (v1.7.2) - Image analysis

- **Database & Storage:**
  - `mssql` (v5.1.0) - Microsoft SQL Server client

- **HTTP & API:**
  - `axios` (v0.19.2) - HTTP client
  - `node-fetch` (v2.6.0) - Fetch API for Node.js

- **Utilities & Helpers:**
  - `moment` (v2.24.0) - Date manipulation
  - `uuid` (v3.4.0) - UUID generation
  - `winston` (v3.2.1) - Logging
  - `yamljs` (v0.3.0) - YAML parsing
  - `geolocation` (v0.2.0) - Location services

- **Development & Code Quality:**
  - `tslint` (v5.20.1) - TypeScript linting
  - `prettier` - Code formatting

---

## 🔄 **Common Technologies Across All Apps**

### **Shared Dependencies:**
- **TypeScript** - All apps use TypeScript for type safety
- **Inversify** - Dependency injection container (API v5.0.1, App v6.0.1, Bot v5.0.1)
- **Axios** - HTTP client (API v0.19.2, App v1.3.4, Bot v0.19.2)
- **Moment.js** - Date manipulation (API v2.24.0, Bot v2.24.0)
- **Google Cloud Services** - Logging and Vision API across all apps
- **Dotenv** - Environment variable management
- **MSSQL** - Database connectivity (API & Bot)

### **Architecture Patterns:**
- **Dependency Injection** - All apps use Inversify for IoC
- **TypeScript** - Strong typing across the entire ecosystem
- **Modular Design** - Clean separation of concerns
- **Google Cloud Integration** - Centralized logging and AI services

### **Development Workflow:**
- **TypeScript Compilation** - All apps compile TS to JS
- **Source Maps** - Debugging support
- **Strict Type Checking** - Enhanced code quality
- **Decorator Support** - Modern JavaScript features

---

Cursor:
```text
*This is a well-architected Pokemon GO community ecosystem with a Node.js API backend, React Native mobile app, and Discord bot, all sharing common technologies and design patterns! 🎮✨*
```