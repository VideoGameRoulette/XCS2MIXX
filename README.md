# XCS2MIXX

**XCS2MIXX** is a Windows Forms utility that automates the conversion of `.xcs` files to `.pgmx` and `.mixx` formats using the external **XConverter.exe** tool. It allows you to manage multiple template configurations, group files by folder, and produce both CSV and MIX file outputs in one click.

---

## Features

* Browse & set the path to **XConverter.exe**
* Define multiple `.tlgx` tool templates with custom labels
* Persist converter, root folder, and templates in `settings.json`
* Recursively scan a root directory for `.xcs` files
* Process each folder:

  1. Convert all `.xcs` → `.pgmx`
  2. Generate `Mixx_XCS-PGMX-MIXX.csv` listing the `.pgmx` files
  3. Produce `Mixx_XCS-PGMX-MIXX.mixx` via `-m 11` mode
* Overwrite existing CSV & MIX files on re-run
* Detailed real-time logging in-app
* Publish as a single, self-contained executable (no external deps)

---

## Prerequisites

* Windows 10 or above
* .NET 8.0 runtime (bundled in single-file EXE)
* Maestro by SCM Group (must be installed; includes `XConverter.exe` and its DLL dependencies)

---

## Installation

1. Clone or download this repository.
2. Ensure **XConverter.exe** is installed and note its full path.
3. Open the solution in Visual Studio or your preferred IDE.

---

## Configuration

1. **Run** the app (`XCS2MIXX.exe`).
2. In "Converter Path", browse to your `XConverter.exe` location.
3. In "Root Folder", browse to the parent directory containing folders with `.xcs` files.
4. To add a tool template:

   * In "Template Path", browse to a `.tlgx` file.
   * Enter a label (e.g. `Pella_Impervia`).
   * Click **Add Template**.
5. Select the desired template from the list.
6. These settings (converter path, root folder, templates) are saved in `settings.json` and reloaded automatically on next launch.

---

## Usage

1. Click **Browse...** next to each field to set:

   * **Converter Path**: path to `XConverter.exe`
   * **Root Folder**: base folder containing `.xcs` files
2. Add one or more `.tlgx` templates and select one.
3. Click **RUN**.
4. Monitor progress in the log pane.
5. Each subfolder under the root will contain:

   * `*.pgmx` files matching each `.xcs`
   * `Mixx_XCS-PGMX-MIXX.csv`
   * `Mixx_XCS-PGMX-MIXX.mixx`

---

## Building & Publishing

The project is configured to produce a single, self-contained executable:

```xml
<!-- In XCS2MIXX.csproj -->
<PropertyGroup>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWindowsForms>true</UseWindowsForms>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <PublishTrimmed>false</PublishTrimmed>
  <IncludeAllContentForSelfExtract>false</IncludeAllContentForSelfExtract>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
</PropertyGroup>
```

Publish with:

```bash
dotnet publish -c Release
```

Output will appear in:

```
bin/Release/net8.0-windows/win-x64/publish/XCS2MIXX.exe
```

---

## Troubleshooting

* **Windows Forms trimming warning**: Trimming is disabled in the project file.
* **Missing converter or template**: Ensure paths are correct and files exist.
* **No `.xcs` files found**: Verify your root folder structure.
* Check the in-app log for detailed error messages.

---

## License

Distributed under the MIT License. See `LICENSE` for details.
