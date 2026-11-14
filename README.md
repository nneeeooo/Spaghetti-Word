# Spaghetti Word

<img align="right" src="logo.png" width="150px"/>

A Microsoft Word add-in for preparing documents for DTP and translation. It cleans up text after PDF-to-Word
conversion and OCR, fixes tables, checks and replaces fonts, highlights layout problems, and converts subtitles.

Built with C#, VSTO and Windows Forms on .NET Framework 4.8.1.

## Features

All tools live on the **Spaghetti** tab in Word.

**DTP and Prep**
- **Word Prep**: one-pass cleanup of text and tables. Removes column and section breaks, trims spaces, resets
  line spacing, indents, tab stops, optional hyphens and soft breaks. For tables: autofit, cell margins,
  borders, pagination. Your choices are remembered.
- **DTP Spotlight**: highlights layout problems: hanging letters, justified text, undelimited sentences,
  digits and dashes.
- **Shortcuts**: one-click dialogs and paragraph toggles (keep with next, page break before), global
  hide/unhide.

**Fonts**
- **Fonts Report**: lists every font the document uses, like InDesign's packaging step.
- **Font Changer**: replaces a font, or sets the Latin, Asian, RTL or Cyrillic font across the document.

**Utilities**
- **Subtitle Tools**: converts SRT and VTT for editing in Word and back.
- **Extract All Images**: saves every image in the document to a folder next to it.
- **Color Picker**
- **MCP**: an optional local server that lets an AI client drive Word. See [Word/Mcp/README.md](Word/Mcp/README.md).

**Right-click menus**
- **Fix table**: applies your saved Word Prep table settings to the table under the cursor only.
- **Page break before**: toggles the setting on the selected paragraphs.

Every action is a single Undo step.

## Requirements

- Windows, Microsoft Word 2016 or later (Microsoft 365 included)
- To build: Visual Studio 2022 or later with the **Office/SharePoint development** workload and the
  .NET Framework 4.8.1 targeting pack

## Build and run

1. Open `Spaghetti.sln` in Visual Studio.
2. VSTO add-ins must be signed. In **Word** project properties, **Signing**, create a test certificate
   (or pick your own).
3. Build and press **F5**. Word starts with the add-in loaded.

Command-line build:

```
MSBuild.exe Spaghetti.sln -restore -p:Configuration=Debug
```

## Tests

- `Tests`: unit tests, no Word needed.
- `Tests.Integration`: starts its own hidden Word instance. Run serially.

```
dotnet test Tests/Tests.csproj
dotnet test Tests.Integration/Tests.Integration.csproj --filter "TestCategory!=Perf"
```

Sample documents are in [samples/](samples/).

## Repository layout

| Folder | What |
|---|---|
| `Word/` | The Word add-in: `Forms/` (ribbon and windows), `Modules/` (document logic), `Mcp/` (MCP server) |
| `Tests/`, `Tests.Integration/` | Unit and Word integration tests |

## License

MIT, see [LICENSE.txt](LICENSE.txt).

Icons and third-party libraries keep their own licenses and are not covered by MIT; see
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
