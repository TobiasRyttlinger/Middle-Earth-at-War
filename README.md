# Middle Earth at War

A real-time strategy game inspired by Battle for Middle-earth II, built in Unity.

## Overview

Players command armies of battalions across Middle-earth, managing resources, constructing buildings, and wielding hero powers to defeat opposing factions.

## Factions

- **Gondor** — disciplined infantry and defensive fortifications
- **Mordor** — overwhelming numbers and dark sorcery

## Core Systems

| System | Description |
|--------|-------------|
| Battalion | Groups of soldiers that move, fight, and level up together |
| Formations | Configurable soldier arrangements with speed/armor/damage modifiers |
| Combat | Damage type vs armor type multiplier table |
| Commands | Queued move, attack, attack-move, patrol, stop |
| State Machine | Per-battalion AI states (Idle, Moving, Attacking, Dying) |
| Buildings | Build-plot and free-placement construction with production queues |
| Heroes | Named units with cooldown-based abilities |
| Powers | Spell tree unlocked by earning points from combat |
| Resources | Tick-based income from captured resource nodes |
| Fog of War | Vision masking per player |
| Selection | Box and click selection with multi-unit support |

## Project Structure

```
Assets/_Project/Scripts/
├── Core/           # Interfaces, enums, constants, events, service locator
├── Units/          # Battalion, soldiers, movement, combat, formations, states, commands
├── Buildings/      # Controllers, placement, construction, walls
├── Heroes/         # Hero controller and abilities
├── Factions/       # Faction definitions and manager
├── Powers/         # Power tree and spell manager
├── Resources/      # Income and resource nodes
├── UI/             # HUD, build menu, minimap, selection panel
├── Camera/         # RTS camera and minimap camera
├── Selection/      # Click and box selection
├── Input/          # Input manager
├── FogOfWar/       # Fog of war system
├── Audio/          # Audio manager
└── AI/             # AI controller
```

## Setup

See the [setup guide](SETUP.md) for Unity layer configuration, ScriptableObject asset creation, and scene bootstrap instructions.

## Requirements

- Unity 6 (or 2022 LTS+)
- NavMesh baked on all playable terrain
- .NET Framework 4.7.1 Developer Pack (for VS Code IntelliSense)
