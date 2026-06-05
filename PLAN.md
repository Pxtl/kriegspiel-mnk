# PLAN

This document contains incomplete plans for new features.  When ready, they will be moved to TODO.

## Name

Rename to ksttt?  KriegspielTicTacToe is too long. Ksttt for C# package?

## AI player
Implementing an AI player will require several steps

 - Clean API for AI player that only gives them legal commands available.
 - UI for adding an AI player.

## Predefined gametypes
 - Define play-modes, list of boards.
 - Serializable config to json.
 - CLI commands
    - load (loads existing saved game, gametype is persisted in there)
    - new
        - type (starts new game with predefined game-type)
        - typefile (starts new game from game-type file)
        - custom (starts new game usinq quick params for game-type)

## GUI
Use Uno or Avalonia for a GUI. Google recommends Avalonia for sliding animations.

## Server
Currently the game is served by a simple JSON file, which is obviously a dumb
hack. Need a proper API.

 - Need a way to extract a player-view of the gamestate, that state can be sent
   to clients and renderers. KriegspielTicTacToe.PlayerViewModel?

## Drop Mode
A mode where pieces are placed in from the top, Drop-4 style.

## Emoji
Currently player spaces are 1 char long.  Emoji are 2 char long in most
monospaced fonts. Allow the players to pick an emoji or a 2-char-long
Mark.  Custom boardwidth?
- Will have to block Marks that are a substring of other Marks.

## BSky bot?  Chat Bot?
- Simple emoji mode without borders?  Render a black square for empty spaces? Or U+3000 space?
- API for chatbots?  Is there a standard one?  Needs standard API including @s,
  DMs, Likes (for voting-based games)
    - There is not.  Have to DIY if I want to support multiple platforms.
- Chatbot could be stateless if there's bidirectional serialization of gamestate.