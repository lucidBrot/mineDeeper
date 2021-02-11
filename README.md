# mineDeeper
Deterministically-Solvable Minesweeper in 3D, with useful hints

## Keys

* <kbd>W</kbd><kbd>A</kbd><kbd>S</kbd><kbd>D</kbd> for strafing
* hold right click and mouse move to rotate
* click a block to reveal it (must be unmarked)
* right click a block to toggle its marking
* <kbd>H</kbd> for a hint location
  * <kbd>H</kbd> for the reasoning behind that hint
* <kbd>S</kbd> to save the current state to `Application.persistentDataPath`
  * See [here](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) for the location on anything and on linux it's apparently in `~/.config` or `~/.local/share` .
* <kbd>L</kbd> to load it again.