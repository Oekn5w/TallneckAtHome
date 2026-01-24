## Tallneck At Home

This repository contains helper programs that helped developing the PC autosplitters for Horizon games (by Guerrilla games). The name is derived from the in-game machine that collects data about the in-game universe. Based on the memory access ability for LiveSplit.

Don't expect much flexibility like persistent configuration or something similar

### Tallneck At Home

Displays data on the Forms window at low intervals suitable to be captured by OBS alongside the recording. Also provides the ability to logs a few minutes of the in-game data at these small intervals.

### Run Logger

Provides the ability to log the in-game data at 0.5 second intervals, written out to csv files each hour at the latest. The Forms window doesn't provide any information for the in-game data. Suitable for run visualizations.

The program has the ability to sync the logging to the recording status of OBS through its websocket. For configuration (especially the OBS Websocket password) a few command line arguments can be used (not really parsed in a safe way):
* `--dir`: next argument is the Textbox entry for the logpath
* `--obsurl`: next argument is the websocket address, default `ws://127.0.0.1:4455`
* `--obspw`: next argument is the websocket password

Example:
```
.\Runlogger.exe --dir "C:\myfolder" --obsurl "ws://localhost:4456" --obspw "hunter4"
```

For the Websocket password, if no command line argument is given, the environment variable `OBS_WS_PW` will also be checked.
