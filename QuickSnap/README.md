# LightlyShot

A small, fast Windows screenshot tool.
**Hotkey -> screen dims -> drag to select -> annotate -> Ctrl+C or Ctrl+S.**

## Build and run

Needs the .NET 8 SDK on Windows 10/11.

    dotnet run -c Release

Fast-starting build (pre-compiled, output in `bin/Release/net8.0-windows/win-x64/publish/`):

    dotnet publish -c Release

Default capture key is PrintScreen. Change it from the tray icon -> Settings.
(If Windows 11's Snipping Tool still opens on PrintScreen, turn off
Settings > Accessibility > Keyboard > "Use the Print screen button to open screen capture".)

## Shortcuts while capturing

| Key | Action |
|---|---|
| Ctrl+C | copy the screenshot to the clipboard |
| Ctrl+S | save as PNG (no dialog) to the save folder |
| Ctrl+A | select the whole monitor |
| Ctrl+Z / Ctrl+Y | undo / redo |
| Esc or right-click | cancel (while typing text, Esc cancels only the text) |
| Shift (while dragging) | arrows snap to 45 degrees, rectangles become squares, circles stay round |
| Space (while dragging the selection) | move the selection instead of resizing it |
| Enter / Shift+Enter | finish text / new line in text |

Click outside the selection to start a new one. The tool and colors you used last are remembered for the next screenshot.

## Settings (tray icon -> Settings)

| Tab | Options |
|---|---|
| General | notifications about saving, keep the selected area position, capture the cursor, start with Windows, save folder |
| Hotkeys | the general hotkey: click the box and press ANY key (or a mouse side button) |
| Format | PNG or JPEG, and the JPEG quality |

Notes:
- "Keep the selected area position" reopens the previous selection on the same monitor. It is remembered until you quit the app.
- The format applies to saved files (Ctrl+S). Ctrl+C always copies the full-quality image.
- Any key can be the hotkey, including plain letters. While LightlyShot runs, that key triggers a screenshot instead of typing, so the window shows a heads-up for everyday keys. While the overlay is open, hotkeys are switched off so every key works normally there.

## Where things live

| Folder | Job |
|---|---|
| `Capture/` | grabs each monitor (GDI BitBlt) |
| `Overlay/` | overlay window, dimming + selection drawing, toolbar, capture session |
| `Editor/` | annotation types, undo/redo history, drawing layers, pixelate |
| `Output/` | final image render, clipboard, saving |
| `Hotkeys/` | global keyboard + mouse-button hotkeys |
| `Settings/` | settings model, JSON storage, settings window |
| `Tray/` | tray icon and notifications |
| `Interop/` | all Win32 declarations |

## Easy things to change

- Look and feel numbers (dim strength, corner radius, line thickness, text size, blur strength, palette): `Editor/EditorDefaults.cs`
- Toolbar colors and button styles: `App.xaml`
- Toolbar tool list and icons: table at the top of `Overlay/AnnotationToolbar.cs`
- New annotation tool: add a class in `Editor/Annotations/`, add it to `OverlayWindow.CreateDragAnnotation` and the toolbar table
- New hotkey action: add to `Hotkeys/HotkeyAction.cs`, give it a default in `AppSettings.DefaultTriggerFor`, handle it in `App.OnHotkeyTriggered`
- Settings file: `%APPDATA%\LightlyShot\settings.json`

## Known limits (honest list)

- A selection stays on one monitor (each monitor has its own overlay so mixed-DPI setups stay pixel-accurate).
- No resize/move handles on the selection after you release it. Click outside to redo it.
- Blur is a mosaic/pixelate (it can't be reversed like a soft blur can).
- Text is typed at the end only (no cursor movement or paste yet).
- Exclusive-fullscreen games may capture black; borderless windowed works.
- Saves PNG or JPEG only.
