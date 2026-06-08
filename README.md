# CoreLoot

Pure vanilla loot multiplier for Rust with aggressive single-pass processing for maximum performance and reliability.

CoreLoot modifies existing loot amounts without replacing vanilla loot tables, keeping the game feeling completely vanilla while allowing full control over loot scaling.

---

[Features](#features) | [Commands](#commands) | [Configuration](#configuration) | [Container-Multipliers](#container-multipliers) | [Blacklist](#blacklist) | [Installation](#installation) | [Notes](#notes)

---

## Features

- Pure vanilla loot system
- No custom loot tables
- Aggressive single-pass processing
- Per-container multipliers
- Per-item multipliers
- Item blacklist support
- Automatic container detection
- High performance design
- Supports existing and newly spawned containers
- Admin refresh commands

---

## How It Works

CoreLoot does **not** replace Rust's vanilla loot system.

Instead, it:
1. Lets vanilla loot spawn normally
2. Processes the container once
3. Multiplies item amounts
4. Permanently marks the container to prevent duplication

This ensures:
- Better performance
- No infinite stacking bugs
- No repeated processing
- Fully vanilla-compatible loot behavior

---

## Commands

### Chat Commands

| Command | Description |
|---|---|
| `/cl.refresh` | Refreshes all loot containers |

### Console Commands

| Command | Description |
|---|---|
| `coreloot.refresh` | Refreshes all loot containers from console |

---

## Permissions

This plugin currently uses admin-only command checks.

No Oxide permissions are required.

---

## Configuration

```json
{
  "Global Multiplier": 2.0,
  "ItemListMultipliers": {
    "scrap": 3.0,
    "rifle.ak": 1.5
  },
  "Containers": {
    "crate_normal": {
      "Enabled": true,
      "Multiplier": 2.0
    },
    "crate_elite": {
      "Enabled": true,
      "Multiplier": 3.0
    }
  },
  "Blacklist": [
    "explosive.timed"
  ]
}
```

---

## Configuration Options

| Setting | Description |
|---|---|
| `Global Multiplier` | Default multiplier used for all containers |
| `ItemListMultipliers` | Overrides multiplier for specific items |
| `Containers` | Individual container settings |
| `Blacklist` | Prevents selected items from being modified |

---

## Container Multipliers

Each loot container can have its own multiplier.

Example:

```json
"crate_elite": {
  "Enabled": true,
  "Multiplier": 5.0
}
```

This allows:
- Elite crates to give more loot
- Basic crates to stay balanced
- Fine-tuned progression

---

## Item Multipliers

Specific items can override container multipliers.

Example:

```json
"ItemListMultipliers": {
  "scrap": 5.0,
  "ammo.rifle": 2.0
}
```

This is useful for:
- Increasing resources
- Reducing weapon inflation
- Adjusting economy balance

---

## Blacklist

Prevent certain items from being modified.

Example:

```json
"Blacklist": [
  "explosive.timed",
  "rocket.launcher"
]
```

Useful for:
- Preventing explosive inflation
- Protecting server balance
- Controlling progression

---

## Automatic Container Detection

CoreLoot automatically detects loot containers currently on the map and populates the config with them.

No manual setup required.

---

## Performance

CoreLoot was designed for performance-first servers.

Features include:
- Single-pass loot processing
- Permanent container markers
- Minimal hook usage
- No loot table rebuilding
- No unnecessary item creation

This makes it ideal for:
- Large population servers
- Modded servers
- PvP servers
- Performance-sensitive environments

---

## Installation

1. Download `CoreLoot.cs`
2. Place into:

```bash
oxide/plugins/
```

3. Reload plugin:

```bash
oxide.reload CoreLoot
```

4. Edit configuration file if needed
5. Use refresh command if required

---

## Notes

- Existing containers may require refresh after config changes
- Multipliers are capped by Rust item stack sizes
- Containers are only processed once
- Plugin preserves vanilla loot behavior

---

## Example Use Cases

### Vanilla+

```json
"Global Multiplier": 1.5
```

### 2x Server

```json
"Global Multiplier": 2.0
```

### High Loot PvP

```json
"crate_elite": {
  "Enabled": true,
  "Multiplier": 6.0
}
```

---

## Support

Created by Corevalence

Report issues or suggestions through uMod discussions.
