# In the Shadows 🎸

> *"I've been watching, I've been waiting, in the shadows for my time..."*
> — The Rasmus

A 3D shadow puzzle game built with Unity, inspired by [Shadowmatic](http://www.shadowmatic.com/).

---

## 🎮 Concept

The player manipulates one or several 3D objects placed in the foreground. By rotating and moving them, they must create a shadow on the background wall that matches a recognizable silhouette. Each puzzle has a name that gives a subtle hint about the expected shape — without ever revealing it directly.

---

## 🕹️ Controls

| Action | Input |
|---|---|
| Horizontal rotation | Click + Drag |
| Vertical rotation | Ctrl + Click + Drag |
| Move object | Shift + Click + Drag |

> The game is designed to be played with a **mouse only**.

---

## 📋 Game Modes

### Normal Mode
- Puzzles are unlocked progressively
- Player advancement is saved between sessions
- Solved and locked puzzles are visually distinct

### Test Mode
- All puzzles are unlocked from the start
- Designed for evaluation and testing purposes

---

## 🔢 Difficulty Levels

| Level | Description |
|---|---|
| **Level 1** | One object — horizontal rotation only |
| **Level 2** | One object — horizontal and vertical rotations |
| **Level 3** | Multiple objects — rotations and free movement |

---

## ✨ Features

- **Shadow validation** — the puzzle is solved when the shadow matches the target shape closely enough, held for a brief moment
- **Cinematic on solve** — the camera zooms toward the shadow with a smooth animation and letterbox effect
- **Music with wind effect** — ambient soundtrack with a low-pass filter that clears when the puzzle is solved
- **Holographic UI** — styled interface with animated buttons and custom fonts
- **Progress saving** — one save per device in Normal mode

---

## 🗂️ Project Structure

```
Assets/
├── Materials/        # All materials and textures
├── Models/           # 3D models (.fbx, .obj)
├── Scenes/           # Unity scenes
├── Scripts/          # All C# scripts
    └── Level1/
    └── Level2/
    └── Level3/
├── Settings/         # Project settings
├── Sounds/           # Music and audio files
└── Textures/         # UI and environment textures
    └── UI/
        └── Hologrammes/
```

---

## 📜 Scripts

| Script | Description |
|---|---|
| `PuzzleManager.cs` | Handles shadow validation, puzzle solve logic and cinematic trigger |
| `ObjectRotator.cs` | Mouse-based object rotation around its visual center |
| `CameraZoom.cs` | Smooth camera zoom cinematic toward the shadow on solve |
| `Letterbox.cs` | Animated black bars appearing during the cinematic |
| `MusicManager.cs` | Audio low-pass filter transition on puzzle solve |

---

## 🛠️ Built With

- **Unity 6** (6000.4.4f1)
- **Universal Render Pipeline (URP)**
- **TextMeshPro**
- **Unity Input System**
- **Unity Audio Mixer**

---

## 🎨 Assets & Credits

- UI Pack : [Free UI Hologram Interface by Wenrexa](https://wenrexa.itch.io/holoui) — CC0
- Environment textures : [ambientCG.com](https://ambientcg.com) — CC0
- 3D Models : various free assets from [Sketchfab](https://sketchfab.com) and [Kenney.nl](https://kenney.nl)

---

## 🚀 How to Build & Run

1. Clone the repository :
```bash
git clone git@github.com:eleaunord/in-the-shadows.git
```
2. Open the project in **Unity 6**
3. Open the `MainMenu` scene
4. Go to **File → Build Settings** and build for your platform
5. Run the build — no additional configuration required

---

## 📚 Project Context

This project was developed as part of the **In the shadows** game engine project. The goal was to implement a complete game from A to Z using a game engine, with a focus on shadow-based puzzle gameplay, clean code architecture and progressive difficulty.

---

*eleaunord — 2026*
