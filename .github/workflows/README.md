# GitHub Actions Workflows

This repository contains GitHub Actions workflows for automated building and releasing of the ZombieSpeed plugin.

## Workflows

### 1. Build and Release (`build-and-release.yml`)

This workflow handles both building and releasing the plugin:

- **Triggers:**
  - Push to `main` or `master` branches
  - Push tags starting with `v*` (e.g., `v1.0.0`)
  - Pull requests to `main` or `master`
  - Manual workflow dispatch

- **Jobs:**
  - **Build:** Builds the plugin in Release configuration
  - **Release:** Creates a GitHub Release when a tag is pushed (only runs for tags)

### 2. Build (`build.yml`)

This workflow only builds the plugin without creating releases:

- **Triggers:**
  - Push to `main` or `master` branches
  - Pull requests to `main` or `master`

- **Job:**
  - **Build:** Builds the plugin in Release configuration

## How to Use

### Building

The workflows automatically build the plugin when you:
- Push code to `main` or `master` branch
- Create a pull request to `main` or `master` branch

Build artifacts are uploaded and can be downloaded from the Actions tab.

### Releasing

To create a release:

1. Create and push a tag with version format:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. The workflow will automatically:
   - Build the plugin
   - Create a ZIP archive
   - Create a GitHub Release
   - Upload the ZIP file to the release

### Release Package Contents

The release ZIP file contains:
- `ZombieSpeed.dll` - Main plugin file
- `translations/` - Language files (en, zh-Hans, zh-Hant, jp, pt-BR, ru)
- All required dependencies

## Installation from Release

1. Download the ZIP file from the GitHub Releases page
2. Extract the contents to your CounterStrikeSharp plugins directory
3. Restart your server

## Requirements

- .NET 8.0 SDK
- CounterStrikeSharp.API

