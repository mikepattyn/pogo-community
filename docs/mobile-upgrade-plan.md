# Mobile App Modernization Plan

## Goal
- Align the React Native mobile app with the current Expo/React Native ecosystem.
- Close security gaps around configuration, authentication, and secret handling.
- Improve architecture, test coverage, and developer experience across platforms (iOS, Android, Web).

## Guiding Assumptions
- Target Expo SDK `~53.x` (or latest stable at execution time) with React Native `0.76+`.
- Node `>=18`, pnpm workspace, and monorepo tooling remain the baseline.
- Backend APIs and authentication flows are being modernized in parallel; the app should read configuration from environment rather than embedding secrets.
- iOS minimum deployment target remains `15.1` unless cross-team alignment allows lowering it.
- Android should target SDK `35`, min SDK `24`, and adopt the new architecture once the upgrade stabilizes.

## Workstream A — Platform & Tooling Upgrade
- [ ] Audit current Expo CLI and renovation prerequisites (`node`, `pnpm`, `expo-cli`).
- [ ] Run `npx expo upgrade` toward the target SDK; resolve merge conflicts.
- [ ] Regenerate native projects (`android`, `ios`) using the new templates; keep backup to compare customizations.
- [ ] Replace legacy Expo config (`app.json`) with typed `app.config.ts` and align metadata.
- [ ] Enable Hermes, new architecture, and Gradle managed devices as appropriate.
- [ ] Validate Metro bundler configuration for monorepo compatibility (`metro.config.js`, `@expo/metro-runtime` updates).

## Workstream B — Native Project Modernization
- [ ] Android: align package name vs. folder structure, migrate `MainApplication` to `DefaultReactNativeHost`, and trim manifest permissions.
- [ ] Update Gradle wrapper, Kotlin, and AGP versions per Expo guidance.
- [ ] Introduce modularized `android/app/src/main/java` structure for future features (e.g., location services).
- [ ] iOS: move to `ExpoAppDelegate`, configure Apple Maps/Google Maps keys via Expo config plugins, and enable auto-linking.
- [ ] Regenerate Pods (`pod install`), ensure Swift compatibility, and verify build with Xcode 15.
- [ ] Add CI scripts for `gradlew assembleRelease` / `xcodebuild` smoke builds.

## Workstream C — Application Architecture & Code Quality
- [ ] Convert legacy class components to functional components with hooks (`useNavigation`, `useRoute`, `useState`, `useEffect`).
- [ ] Replace deprecated lifecycle methods (`UNSAFE_componentWillReceiveProps`, manual `forceUpdate`) with effect-driven logic.
- [ ] Migrate to React Navigation v7 patterns (`NavigationContainer`, typed params).
- [ ] Enable TypeScript `strict` mode; resolve resulting type gaps incrementally.
- [ ] Adopt `@react-native-async-storage/async-storage` and refactor storage utilities accordingly.
- [ ] Introduce centralized state (React Context or lightweight store) for auth/session data.

## Workstream D — Networking & Security
- [ ] Remove hard-coded API base URLs and bearer tokens; load configuration from environment (`expo-constants`, build-time `.env`).
- [ ] Rebuild Axios client to instantiate per request with interceptors scoped to that instance.
- [ ] Integrate secure credential storage for tokens (`expo-secure-store` or `react-native-keychain`).
- [ ] Implement environment-driven logging (toggleable in dev vs. production).
- [ ] Ensure HTTPS endpoints and certificate pinning strategy where feasible.
- [ ] Document onboarding for environment variables and secret rotation.

## Workstream E — Features, UX, and Accessibility
- [ ] Redesign welcome/registration flow to use form libraries (e.g., `react-hook-form`) with validation feedback.
- [ ] Audit navigation stack for unused routes and future features; define backlog items.
- [ ] Update UI components to contemporary design system (consider `react-native-paper` or custom components).
- [ ] Add accessibility labels, dynamic type scaling, and localization scaffolding.
- [ ] Review Google Maps integration for quota usage and fallback when APIs fail.
- [ ] Provide offline/low-connectivity handling where applicable.

## Workstream F — Testing, Tooling, and CI
- [ ] Introduce unit/integration tests with `@testing-library/react-native` and snapshot baselines.
- [ ] Configure Detox or Expo E2E tests for core flows (login, map, registration).
- [ ] Expand linting rules (`eslint-config-universe`, TypeScript ESLint) and enable automatic formatting.
- [ ] Add GitHub Actions (or existing CI) jobs for lint, type-check, test, and platform build verification.
- [ ] Define release checklist, including OTA updates if using Expo EAS.
- [ ] Capture metrics for crash/error reporting (Sentry or Expo Updates).

## Deliverables & Milestones
- **Phase 1 (Weeks 1-2):** Tooling upgrade, native project regeneration, initial CI checks pass.
- **Phase 2 (Weeks 3-4):** Core architecture refactor (hooks, navigation), secure configuration, TypeScript strict baseline.
- **Phase 3 (Weeks 5-6):** UX refresh, accessibility improvements, feature parity confirmation.
- **Phase 4 (Weeks 7+):** Expanded testing, CI hardening, release readiness, documentation updates.

## Dependencies & Coordination
- Backend teams for API contract validation and auth token lifecycle.
- DevOps/SRE for CI/CD pipelines, secret management, and app distribution.
- Design/UX for updated flows and component library choices.
- Security for reviewing token storage and permission scopes.

## Risks & Mitigations
- **Upgrade Breakage:** Run upgrade in stages, keep reproducible scripts, and gate merges via CI.
- **API Changes:** Implement feature flags or mocked services to decouple from backend timelines.
- **Resource Constraints:** Prioritize workstreams, schedule mob sessions for complex migrations.
- **Secret Exposure:** Move keys to environment files immediately and rotate compromised credentials.

## Next Actions
- Confirm target Expo SDK and ship date with stakeholders.
- Schedule kickoff for Phase 1 with mobile, backend, and DevOps leads.
- Allocate ownership for each workstream and establish progress checkpoints.
