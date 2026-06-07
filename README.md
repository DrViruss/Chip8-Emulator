
# 🕹️ CHIP-8 Emulator + Assembler Suite

> **A complete CHIP‑8 toolkit: run ROMs in a terminal, debug, assemble, and disassemble.**  
> *Two projects in one – emulator core + assembler/disassembler.*

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux-lightgrey)]()

## 📦 What's inside

This repository contains two tightly integrated subprojects:

| Component | Purpose |
|-----------|---------|
| **CHIP-8 Emulator** | Full interpreter with terminal graphics – runs `.ch8` ROMs, implements all 35 opcodes, timers, and a console display. |
| **CHIP-8 Assembler / Disassembler** | Convert assembly source (`.asm`) into binary ROMs (`.ch8`) and disassemble existing ROMs back to readable assembly. |

Together they form a complete **development and reverse‑engineering environment** for the CHIP‑8 platform.

## ✨ Features

### 🖥️ Emulator
- ✅ **All 35 original CHIP‑8 instructions** implemented in `CPU.cs`
- ✅ **4KB RAM**, 16 general‑purpose registers (V0–VF), stack, program counter
- ✅ **Delay and sound timers** (60 Hz)
- ✅ **64×32 monochrome display** rendered in the terminal (using `*` for active pixels)
- ✅ **Keyboard input** support (mapping configurable)
- ✅ Clean separation: `Base-Lib` (core logic) + `Console Emulator` (terminal frontend)

### ⚙️ Assembler & Disassembler (`AD`)
- 📝 **Assemble** (`AD.exe a source.asm`) – turns text mnemonics into binary `.ch8` ROM
- 🔍 **Disassemble** (`AD.exe d program.ch8`) – reconstructs assembly source from a binary ROM
- 🧠 Supports all standard CHIP‑8 mnemonics (`CLS`, `LD`, `JP`, `DRAW`, etc.)

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or later)

### Build

```bash
git clone https://github.com/DrViruss/Chip8-Emulator.git
cd Chip8-Emulator
dotnet build
```

After build, executables appear in:
- `Console Emulator/bin/Debug/net8.0/`
- `AD/bin/Debug/net8.0/`

## 🎮 Usage

### Run the Emulator
> The emulator core is fully implemented and ready to run any standard CHIP‑8 ROM.

#### Default
```bash
cd "Console Emulator/bin/Debug/net8.0"
dotnet "Console Emulator.exe"
```
> Uses standart CHIP-8 ROM

#### Run with custom ROM
```bash
cd "Console Emulator/bin/Debug/net8.0"
dotnet "Console Emulator.exe" /path/to/rom
```
> Will use your custom ROM

#### Debug
```bash
cd "Console Emulator/bin/Debug/net8.0"
dotnet "Console Emulator.exe" -debug
```
> Running in debug mode

### Assembler / Disassembler (`AD`)

The `AD` tool works from the command line with two modes:

| Mode | Command | Description |
|------|---------|-------------|
| **Assemble** | `AD.exe a input.asm` | Assembles `input.asm` → `input.ch8` |
| **Disassemble** | `AD.exe d input.ch8` | Disassembles `input.ch8` → `input.asm` |

**Example:**

```bash
> AD.exe a my_game.asm
Assembling... my_game.asm
1: LD V0, 0x01 -> 6001
2: ADD V0, 0x01 -> 7001
...
Writing to my_game.ch8... Done.
```

## 🏗️ Project Structure

```
Chip8-Emulator/
├── Base-Lib/                 # Core emulation logic
├── Console Emulator/         # Terminal frontend
├── AD/                       # Assembler & Disassembler
├── .idea/                    # JetBrains Rider settings
└── Chip-8 Emulator.sln       # Solution file
```

## 📚 Learning Resources

- [Cowgod's Chip‑8 Technical Reference](http://devernay.free.fr/hacks/chip8/C8TECH10.HTM) – The classic spec.
- [Timendus CHIP‑8 test suite](https://github.com/Timendus/chip8-test-suite) – Verify your emulator accuracy.

## 🤝 Contributing

Contributions are welcome! Feel free to open an issue or submit a pull request – whether it's fixing a subtle opcode bug, adding SUPER‑CHIP support, or improving the disassembler output.

## 📄 License

Distributed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

**Built with ❤️ by [DrViruss](https://github.com/DrViruss)**  
*Your complete CHIP‑8 workshop – from assembly to emulation and back.*
