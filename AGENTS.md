# AGENTS.md

See [CONTRIB.md](CONTRIB.md) for coding style guide.

# Dev Environment

Codebase is using dotnet 10.0.  Use `dotnet build` to compile, and `dotnet test`
to run unit test suite.

# Testing

Unit tests are in `tests/KriegspielTicTacToe.Model.Tests`. Do not attempt
automated testing on the command-line tool. Test improvement can be achieved by
codifying more logic into model.

# Refactoring

Do not attempt to refactor or otherwise clean-up the code unless specifically
instructed to do so. Do not remove comments unless they are untrue.

# Scratch space

Create any needed working files such as to-do lists within the `temp` dir, which is gitignored.