# 🌱 HabiCheck — Desktop Fabric Scanner & Comfort Auditor

HabiCheck is a modern, eco-conscious C# WPF desktop application that analyzes fabric weave composition and scores its comfort suitability against Cebu's tropical realities (high heat and high humidity). Using a local SQLite database and Google's Gemini AI, HabiCheck helps users build a sustainable, sweat-friendly wardrobe customized to their personal perspiration profiles.

---

## 📷 Screenshots & Interface
HabiCheck features a premium, eco-friendly styling package featuring warm cream tones, sage greens, and terracotta highlights with responsive resizing, custom drop-zones for image picking, and scrollable, interactive layouts.

---

## ⚙️ What to Install
Before running this application, ensure you have the following installed on your Windows machine:

1. **.NET 10.0 SDK** (or later) — [Download .NET SDK](https://dotnet.microsoft.com/download)
2. **Visual Studio 2022** (with the **.NET Desktop Development** workload checked during installation) — [Download Visual Studio](https://visualstudio.microsoft.com/)

---

## 🔐 Setup Instructions

To enable the AI features (wearer guides, fabric chat, and ecological digests), you must supply your own Google Gemini API key.

1. Go to the **[Google AI Studio](https://aistudio.google.com/)** console.
2. Click **Get API key** and generate a new API key.
3. In the root directory of this project, create a file named `config.txt`.
4. Paste your API key into `config.txt` using the format:
   ```text
   GEMINI_API_KEY="AIzaSyYourNewActualKeyHere..."
   ```
5. **Security Note**: The `config.txt` file is explicitly ignored in [.gitignore](file:///c:/Users/CCS/source/repos/HabiCheck/.gitignore) so your API credentials will never be committed or uploaded to GitHub.

---

## 📂 File Structure

The project follows a clean **MVVM (Model-View-ViewModel)** architectural pattern:

```text
📂 HabiCheck/
├── 📂 HabiCheck/                 # WPF Project Folder
│   ├── 📂 Models/                # Data Transfer Objects & Domain Models
│   │   ├── ScanRecord.cs         # SQLite database table mapping for scans
│   │   ├── FabricData.cs         # Breathability & Sustainability metrics
│   │   ├── WeatherInfo.cs        # Cebu climate API responses
│   │   └── HulasLevel.cs         # Sweat persona enum (Pawisin, Normal, Chill)
│   │
│   ├── 📂 Services/              # Backend and external APIs
│   │   ├── DatabaseService.cs    # Local SQLite CRUD (Users, Profiles, Scans)
│   │   ├── HabiService.cs        # Cebu weather simulator & sweat persona parser
│   │   └── AntigravityService.cs # REST Client for Google Gemini API
│   │
│   ├── 📂 ViewModels/            # Observable bindings & logic (CommunityToolkit.Mvvm)
│   │   ├── DashboardViewModel.cs # Loads weather cards and recent scan history
│   │   ├── ScannerViewModel.cs   # Triggers file pickers and simulates image analysis
│   │   ├── ResultViewModel.cs    # Fetches AI fabric tips
│   │   └── ScanDetailViewModel.cs# Powers side-drawer chat conversation
│   │
│   ├── 📂 Converters/            # WPF value converters
│   │   ├── BoolToVisibilityConverter.cs # Handles non-empty states & dropzones
│   │   └── ChatAlignmentConverter.cs    # Bubble layout side alignment (User vs AI)
│   │
│   └── 📂 Views/                 # WPF XAML Windows
│       ├── LoginWindow.xaml      # User sign-in and profile creation portal
│       ├── OnboardingWindow.xaml # Cebu climate profile selector
│       ├── DashboardWindow.xaml  # Main summary page
│       ├── ScannerWindow.xaml    # Multi-state drag & select scanning zone
│       ├── ResultWindow.xaml     # Post-scan analysis report and insights
│       ├── ScanDetailWindow.xaml # Detailed scan viewer with collapsible AI chat drawer
│       └── 📂 Controls/          # Reusable UserControls
│           ├── BottomNavBar.xaml # Seamless window switcher
│           └── FabricChatControl.xaml # Scroll-to-bottom chat bubble container
│
├── HabiCheck.slnx                # XML-based Visual Studio Solution
├── config.txt                    # (Local only) Your secret Gemini API key
└── .gitignore                    # Ensures keys/binaries are never committed
```

---

## 🚀 How to Run

### Method A: Using Visual Studio (Recommended)
1. Double-click the `HabiCheck.slnx` solution file in the root folder to open the project in Visual Studio.
2. In the top toolbar, ensure the startup project is set to **HabiCheck** and debug target is set to **Debug / Any CPU**.
3. Press **F5** (or click the green **Start** button) to compile and run.

### Method B: Using dotnet CLI
Open your terminal (PowerShell or Command Prompt) in the repository root and run:
```bash
dotnet build
dotnet run --project HabiCheck\HabiCheck.csproj
```
