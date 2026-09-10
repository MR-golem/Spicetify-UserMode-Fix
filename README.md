# Spicetify Fix — User Mode

> If you use a modified Windows and Spicetify completely refuses to install because the command runs as Administrator — this is for you.

### The Problem
Some modified Windows builds run everything as `Administrator` by default.  
Spicetify installation fails with:

```powershell
iwr -useb https://raw.githubusercontent.com/spicetify/cli/main/install.ps1 | iex
```

It requires a normal `User` context, not Administrator.

### The Solution
This lightweight tool fixes it automatically:

- **If Administrator** → opens PowerShell as `User` via `runas /trustlevel:0x20000` and runs the install
- **If User** → opens PowerShell directly and runs the install
- Keeps the PowerShell window open so you can see the result

### Usage
1. Download `SpicetifyFix.exe` and `run_spicetify.ps1` (keep them in the same folder)
2. Double-click `SpicetifyFix.exe`
3. Click **Install Spicetify**
4. A PowerShell window opens as User and installs automatically

### Design
- Clean white design — black border — minimal progress bar like native Windows apps
- Single lightweight EXE (~9KB), no dependencies
- No fork bomb, no auto-relaunch loops

### Structure
```
Spicetify-Fix-UserMode/
├── SpicetifyFix.exe      # App (white native design)
├── run_spicetify.ps1     # Install script
├── src/
│   └── SpicetifyFix.cs   # Organized source
└── README.md
```

### Build from Source
```powershell
csc /target:winexe /out:SpicetifyFix.exe src/SpicetifyFix.cs /reference:System.Windows.Forms.dll /reference:System.Drawing.dll
```

### Requirements
- Windows 10 / 11
- .NET Framework 4.x (preinstalled)
- Spotify installed

---
Made for modified Windows users.
