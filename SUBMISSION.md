# Final Submission - Brandon Elsinger

## Console RPG World Builder

---

## Features Implemented

### ✅ Basic Requirements (Pre-implemented + Enhanced)

| Feature | Location | Description |
|---------|----------|-------------|
| **Add Character** | Admin Menu → Option 1 | Creates a new player with name, health, experience, and optional room assignment |
| **Edit Character** | Admin Menu → Option 2 | Modify existing character's name, health, or experience |
| **Display All Characters** | Admin Menu → Option 3 | Shows all characters in a formatted table with ID, Name, Level, Health, Experience, and Location |
| **Search Character by Name** | Admin Menu → Option 4 | Case-insensitive search with partial matching, displays results in table format |
| **Logging** | Throughout | All operations logged via Microsoft.Extensions.Logging |

---

### ✅ C-Level Requirements (405/500 points)

#### 1. Add Abilities to a Character
**Location:** `AdminService.cs` → `AddAbilityToCharacter()`

**Implementation:**
- Displays list of all characters using Spectre.Console `SelectionPrompt`
- Shows available abilities from database (Shove, Fireball, Heal, Combat)
- Associates ability with character via many-to-many relationship (`PlayerAbilities` join table)
- Validates duplicate abilities - prevents adding the same ability twice
- Saves changes and displays confirmation message

#### 2. Display Character Abilities
**Location:** `AdminService.cs` → `DisplayCharacterAbilities()`

**Implementation:**
- Prompts user to select a character
- Uses `.Include(p => p.Abilities)` to eager load abilities
- Displays formatted table with: Name, Type, Description, Stats
- Handles ability-specific properties (Damage/Distance for Shove/Fireball, Heal amount for Heal)
- Gracefully handles characters with no abilities

#### 3. Execute Ability During Attack
**Location:** `PlayerService.cs` → `UseAbilityOnMonster()`

**Implementation:**
- Only available in Exploration Mode when monsters are present
- Prompts player to select from their learned abilities
- Prompts to select target monster (if multiple in room)
- Calls `ability.Activate(player, monster)` to apply damage
- Updates monster health in database
- Awards 50 XP on monster defeat with level-up system
- Handles loot drops from defeated monsters
- Displays combat results using Spectre.Console formatting

---

### ✅ B-Level Requirements (445/500 points)

#### 1. Add New Room
**Location:** `AdminService.cs` → `AddRoom()`

**Implementation:**
- Prompts for room name and description
- Selects existing room to connect FROM
- Chooses direction (North, South, East, West)
- Automatically sets bidirectional navigation (new room connects back)
- Calculates X/Y coordinates based on source room
- Validates no room exists at target coordinates
- Optional: Add a monster to the new room (select type: Goblin, Beast, Undead, Bandit)
- Optional: Move an existing character to the new room
- Expands the world map beyond the initial 3x3 grid

#### 2. Display Room Details
**Location:** `AdminService.cs` → `DisplayRoomDetails()`

**Implementation:**
- Lists all rooms for selection
- Uses `.Include()` for Players, Monsters, and all directional rooms
- Displays in a styled Panel with:
  - Room name and description
  - Available exits (North/South/East/West → destination name)
  - Players present (Name, HP)
  - Monsters present (Name, HP, Type)
- Gracefully handles empty rooms

#### 3. Navigate Rooms
**Location:** `GameEngine.cs` → `ExplorationMode()` + `PlayerService.cs` → `MoveToRoom()`

**Implementation:**
- Full exploration mode with visual map display (MapManager)
- N/S/E/W navigation commands
- Updates player's `RoomId` and persists to database
- Displays room description and monster warnings on entry
- Context-sensitive actions based on room contents

---

### ✅ A-Level Requirements (475/500 points)

#### 1. List Characters in Room by Attribute
**Location:** `AdminService.cs` → `ListCharactersInRoomByAttribute()`

**Implementation:**
- Select a room from the list
- Choose filter attribute: Level, Health, or Experience
- Enter threshold value (shows characters >= threshold)
- Uses LINQ: `.Where(p => p.RoomId == roomId && p.[Attribute] >= threshold)`
- Displays matching characters in formatted table
- Handles case where no characters match criteria

#### 2. List All Rooms with Characters
**Location:** `AdminService.cs` → `ListAllRoomsWithCharacters()`

**Implementation:**
- Queries all rooms with `.Include(r => r.Players).Include(r => r.Monsters)`
- Filters to rooms that have at least one player OR monster
- Displays as a Tree structure using Spectre.Console
- Groups by room, shows players in green, monsters in red
- Handles empty world gracefully

#### 3. Find Equipment Location
**Location:** `AdminService.cs` → `FindEquipmentLocation()`

**Implementation:**
- Prompts for item name to search
- Uses complex query with multiple `.Include()` and `.ThenInclude()`:
  - `Players → Equipment → Weapon`
  - `Players → Equipment → Armor`
  - `Players → Room`
- Searches both weapons and armor by name (partial match)
- Displays table with: Character name, Item found, Room location
- Handles not found cases

---

## Additional Features Implemented

### Combat System Enhancements
- **Monster Counter-Attacks:** After player attacks, surviving monsters strike back
- **Armor Class:** Monsters have defense that reduces incoming damage
- **Player Defense:** Equipment armor reduces damage taken
- **Death Handling:** Game over screen when player HP reaches 0
- **Loot System:** Monsters can drop items that go to player inventory

### Player Progression
- **Experience System:** Gain 50 XP per monster defeated
- **Level Up:** Every 100 XP triggers level up with max HP increase
- **Health Regeneration:** Full heal on level up

### Inventory System
- **Backpack:** Players have an inventory to store items
- **Equip Items:** Swap weapons/armor from inventory
- **Loot Collection:** Items from defeated monsters auto-added to inventory

### UI/UX
- **Spectre.Console Integration:** Rich terminal UI throughout
- **Visual Map:** 3x3+ grid showing all rooms with current location highlighted
- **Combat Summary:** Health bars and status displays during combat
- **Character Switching:** Switch between multiple player characters

---
