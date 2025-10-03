# GTA11Y Mod Setup Instructions

## Overview
I've fixed the compilation errors in your GTA11Y accessibility mod. The main issues were:

1. ✅ **Fixed conflicting ScriptHookVDotNet references** - Removed duplicate references
2. ✅ **Fixed missing Tolk library** - Added proper C# wrapper based on the official Tolk library
3. ✅ **Fixed VehicleHash ambiguity** - Used fully qualified names
4. ✅ **Removed invalid Davykager namespace** - This was causing compilation errors

## Required External Dependencies

To complete the setup, you need to download and install these external dependencies:

### 1. ScriptHookVDotNet
- **Download from**: https://github.com/scripthookvdotnet/scripthookvdotnet/releases
- **Files needed**: `ScriptHookVDotNet.dll` (for .NET Framework 4.8)
- **Install location**: Your GTA V directory (same folder as `GTA5.exe`)
- **Update path in .csproj**: Change the HintPath to point to your actual GTA V installation

### 2. Tolk Library
- **Download from**: https://github.com/dkager/tolk
- **Files needed**: 
  - `Tolk.dll` (the main library)
  - Screen reader API DLLs from the `libs` directory
- **Install location**: Your GTA V directory or somewhere in your PATH
- **Note**: You'll need to compile Tolk from source or find pre-built binaries

### 3. Update Project References

In `GTA11Y.csproj`, update this line to point to your actual GTA V installation:
```xml
<Reference Include="ScriptHookVDotNet">
  <HintPath>C:\Path\To\Your\GTA5\ScriptHookVDotNet.dll</HintPath>
</Reference>
```

## Building the Project

1. Make sure you have Visual Studio 2019 or later installed
2. Open the solution file `GTA.sln` in Visual Studio
3. Update the ScriptHookVDotNet reference path to your GTA V installation
4. Build the project (Build → Build Solution)

## Installation

After successful compilation:
1. Copy the generated `GrandTheftAccessibility.dll` to your GTA V `scripts` folder
2. Make sure `Tolk.dll` and its dependencies are accessible to GTA V
3. Ensure you have Script Hook V installed in your GTA V directory

## What I Fixed

### Code Changes Made:
- **Removed conflicting references**: Kept only ScriptHookVDotNet (not both v2 and v3)
- **Added proper Tolk wrapper**: Created a complete C# wrapper based on the official Tolk library
- **Fixed namespace issues**: Removed invalid `Davykager` using statement
- **Fixed VehicleHash ambiguity**: Used `GTA.VehicleHash` explicitly
- **Updated using statements**: Added `DavyKager` namespace for Tolk

### Files Modified:
- `GTA11Y.csproj` - Fixed references
- `GTA11Y.cs` - Updated using statements, restored Tolk functionality
- `VehicleSpawn.cs` - Fixed VehicleHash ambiguity
- `Tolk.cs` - Created proper Tolk C# wrapper
- `packages.config` - Cleaned up package references

The mod should now compile successfully once you have the required external dependencies installed.