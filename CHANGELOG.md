# Changelog

All notable changes to this project will be documented in this file.

## [v1.5.2] - 2026-02-15

### Fixed
- **Application Shutdown**: Fixed a bug where the application would continue running in the background after being closed with the 'X' button. The application now closes properly.
- **Startup Behavior**: Resolved an issue where the application would not start minimized to the system tray correctly when the "Start with Windows" and "Start minimized" options were enabled.
- **Language Preference**: Fixed a critical bug where the selected language was not being used for grammar correction, causing the application to default to language auto-detection and produce incorrect output.

## [v1.5.1] - 2026-02-07

### Added
- **AI Model Selection**: Users can now choose their preferred Gemini AI model from a dropdown, with a link to official documentation.
- **Language Management**: Added functionality to add, edit, and remove custom languages.
- **Custom Personas Management**: Enhanced functionality to edit and remove custom personas.
- **Comprehensive Documentation**: Updated the `README.md` to fully document all existing application features, providing a clear and complete overview of capabilities.
- **Debug Window Toggle**: Added a setting to toggle a dedicated debug window for advanced troubleshooting and logging.

### Changed
- **Revamped UI/UX**: Implemented a new compact mode that hides advanced settings for a cleaner interface, alongside an improved bento-style grid layout for expanded settings.
- **Hotkey Display**: The hotkey settings are now integrated directly into the main status bar for constant visibility.
- **'Powered by' Text**: Updated the 'About' section to a more general 'Powered by Gemini' to reflect support for multiple models.

### Removed
- **Animation Speed Slider**: Removed the non-functional 'Animation Speed' slider from the 'Appearance' settings.

### Fixed
- **Window State Persistence**: Corrected issues where window size and position were not saving correctly across sessions.
- **Minimize to Tray**: Fixed the minimize button functionality to correctly send the application to the system tray.
- **Duplicate UI Controls**: Resolved build errors caused by duplicated UI control definitions in MainWindow.xaml.
- **Nullability Warnings**: Addressed all nullability warnings in dialog files.
- **Orphaned Code References**: Removed leftover code referencing the deleted 'Animation Speed' slider, resolving build errors.
