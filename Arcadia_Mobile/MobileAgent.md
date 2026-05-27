# Project Overview

Arcadia_Mobile is the .NET MAUI client for Arcadia. It should provide a touch-friendly mobile and desktop UI for the Arcadia adventure/creature-battle game while preserving the main game's domain rules, terminology, and save behavior.

The mobile project lives in `Arcadia_Mobile/`. The existing console game lives in `Arcadia_v2/`, and unit tests currently live in `UnitTest/`.

# Tech Stack

- Language: C#
- Runtime/framework: .NET 10.0
- App type: .NET MAUI single-project app
- UI: XAML with code-behind where appropriate
- Target frameworks: `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, and `net10.0-windows10.0.19041.0` where supported by the host OS
- Nullable reference types: Enabled
- Implicit usings: Enabled
- XAML compilation/source generation: `MauiXamlInflator` set to `SourceGen`
- Logging: `Microsoft.Extensions.Logging.Debug` in Debug builds

# Folder Structure

- `Arcadia_Mobile/Arcadia_Mobile.csproj` - MAUI project file, target frameworks, app identity, package references, and resource registration.
- `Arcadia_Mobile/MauiProgram.cs` - app builder, DI registration, fonts, logging, and MAUI startup configuration.
- `Arcadia_Mobile/App.xaml` and `App.xaml.cs` - app-level resources and application startup.
- `Arcadia_Mobile/AppShell.xaml` and `AppShell.xaml.cs` - Shell routes and navigation structure.
- `Arcadia_Mobile/MainPage.xaml` and `MainPage.xaml.cs` - current playable exploration page; keep page code-behind limited to view setup and binding context assignment.
- `Arcadia_Mobile/ViewModels/` - view models for MAUI binding, commands, screen state, and UI-facing state transitions.
- `Arcadia_Mobile/Services/` - mobile gameplay/session services such as current room state, touch-driven movement, interaction handling, navigation helpers, and future persistence/platform abstractions.
- `Arcadia_Mobile/Map/` - mobile-adapted room, direction, and map logic copied/refactored from the console reference as needed.
- `Arcadia_Mobile/Resources/` - app icons, splash screen, images, fonts, raw assets, and shared styles.
- `Arcadia_Mobile/Platforms/` - Android, iOS, MacCatalyst, and Windows platform-specific entry points, manifests, and configuration.
- `Arcadia_Mobile/Properties/launchSettings.json` - local launch profiles.

# Architecture Rules

- Treat `Arcadia_v2/` as the reference implementation for gameplay logic, not as an edit target for mobile work.
- Do not modify existing files in `Arcadia_v2/` when building or refactoring logic for the MAUI app unless explicitly instructed.
- When mobile needs logic from the console project, create an adapted copy in the appropriate `Arcadia_Mobile/` folder. For example, copy/refactor `AnimalFactory.cs` into a matching mobile-side folder instead of editing the original file in `Arcadia_v2/`.
- Keep duplicated mobile logic organized by responsibility so it can evolve separately from console-specific flow.
- Keep MAUI pages focused on UI composition, navigation, and binding. Put gameplay decisions, battle rules, save mapping, and state transitions outside page code-behind.
- Keep screen state and commands in `ViewModels/`; keep reusable game/session behavior in `Services/`; keep mobile-adapted domain models in responsibility-based folders such as `Map/`, `Creatures/`, `Battles/`, `Player/`, and `Saves/`.
- Expand the existing playable exploration flow incrementally. Add rooms, interactions, inventory, dialogue, saves, and battles as separate focused steps instead of folding them all into one page or service.
- Do not copy console-specific flow into the mobile app. Adapt the same domain concepts to screens, commands, buttons, menus, and modal flows.
- Do not call `Console.ReadLine`, `Console.WriteLine`, or console-specific `IGameIO` implementations from the mobile project.
- Prefer constructor-injected services registered in `MauiProgram.CreateMauiApp()` for navigation, game state, persistence, and platform services.
- If shared gameplay code is eventually needed, discuss that explicitly first. Do not extract or move code out of `Arcadia_v2/` as part of ordinary mobile work.
- Keep platform-specific code under `Platforms/` or behind small abstractions. Shared UI and game behavior should remain cross-platform.
- Keep Shell routes centralized in `AppShell` or a dedicated route registration helper as the app grows.
- Use async APIs for persistence, file access, navigation, and platform services so the UI thread stays responsive.
- Preserve Arcadia-specific terminology and avoid third-party monster-collector IP references in user-facing text.

# UI Rules

- Build for touch first: large enough tap targets, simple navigation paths, readable text, and clear feedback for actions.
- Use XAML resources and styles for repeated colors, typography, spacing, and controls instead of hard-coding values across pages.
- If a `StaticResource` works in build but crashes on Android at runtime, prefer a direct value or verify the merged resource dictionary is available before startup.
- Keep layout responsive across phone, tablet, desktop, and emulator windows. Avoid fixed dimensions unless the element must keep a stable size.
- Use MAUI controls, Shell navigation, data binding, and commands before adding custom platform code.
- Keep code-behind limited to view-specific event handling. Move stateful behavior into view models or services once a screen has meaningful logic.
- Include semantic properties and accessibility labels for buttons, images, battle actions, menus, and important status text.
- Replace the starter `.NET` template assets and text when building real Arcadia screens.

# Coding Style Rules

- Follow the existing C# style: namespace `Arcadia_Mobile`, PascalCase for public types/members, camelCase for local variables and parameters.
- Keep nullable annotations meaningful; fix nullability issues through clear data flow rather than suppressions.
- Use descriptive names for pages, view models, services, commands, and routes.
- Keep methods focused and move repeated UI/game orchestration into helpers or services.
- Add comments only when they clarify non-obvious MAUI lifecycle behavior, platform differences, game rules, or persistence behavior.
- Do not introduce new external packages unless the mobile feature clearly needs them.
- If at any point you have a question or need a decision about a design choice, ask directly.

# Testing and Verification Rules

- Run the existing unit tests when changes touch shared gameplay rules, save/load behavior, commands, battles, maps, or player progression.
- For MAUI-only UI changes, build the target framework available on the current machine before considering the change complete.
- On Windows, prefer:
  - `dotnet build .\Arcadia_Mobile\Arcadia_Mobile.csproj -f net10.0-windows10.0.19041.0`
- For Android work, build or deploy the Android target when the workload and emulator/device are available:
  - `dotnet build .\Arcadia_Mobile\Arcadia_Mobile.csproj -f net10.0-android`
- Add tests around extracted shared logic rather than trying to unit test MAUI page rendering directly.
- Ensure any new test created has a comment above it describing the test.
- Add new test directly related to the mobile app into Arcadia_v2\UnitTest\MobileUnitTest\

# Android Emulator Diagnostics

- If the MAUI app installs, starts, and immediately closes on the Android emulator, check Android logcat for the real runtime exception before changing code.
- `adb` may not be on the system `PATH` in this workspace. The Android SDK copy currently used by this machine is:
  - `C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe`
- From the repository root, read recent emulator logs with:
  - `& 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe' logcat -d -t 1200`
- To check only Android runtime crashes, use:
  - `& 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe' logcat -d -t 1000 AndroidRuntime:E '*:S'`
- Common MAUI startup crashes may come from XAML parse errors, missing `StaticResource` keys, dependency injection constructor failures, missing assets, or platform-specific resource loading differences.
- After fixing a crash, rebuild and run the Android target, then check logcat again for fresh `FATAL EXCEPTION` entries:
  - `dotnet build .\Arcadia_Mobile\Arcadia_Mobile.csproj -f net10.0-android -t:Run /nologo /clp:ErrorsOnly`

# Restrictions / What Not To Do

- Do not commit generated build output from `bin/` or `obj/`.
- Do not modify existing files under `Arcadia_v2/` for mobile implementation work unless the user explicitly asks for it.
- Do not move files out of `Arcadia_v2/` into shared projects or libraries without explicit approval.
- Do not reference console-only classes directly from MAUI pages when the mobile app needs an adapted copy.
- Do not put game rules, battle calculations, save conversion, or command parsing directly in XAML code-behind.
- Do not duplicate large sections of console gameplay flow in mobile pages.
- Do not bypass existing save services or mappers when mobile persistence is wired in.
- Do not add platform-specific behavior in shared files without guarding or abstracting it.
- Do not make broad rewrites to the console project while working on targeted mobile UI changes unless explicitly requested.
- Do not add legacy save migration or backwards-compatibility code for major changes/refactors unless explicitly requested.

# Current Plan
- Continue bringing features from the Arcada_v2 project to the mobile version. 

# Current Task
- Begin copying over the animals/creature data from Arcadia_v2 into the mobile application, ensuring that the new files work with .Net Maui. Ensure you maintain the file architecture (for example if a Creatures folder does not exist in the mobile project you will create one)
- Stop and ask questions if needed. 