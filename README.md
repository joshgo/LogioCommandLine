# LogioCommandLine

A simple commandline tool for interacting with a [logio](http://logio.org) server. The tool gives you the ability to do the following:

1. Send log messages
2. Add nodes and streams to the server
3. Store multiple server details (i.e. address & port) in a config

## Examples
- ```logio send-log -m "This is a message"```
- ```logio add-node machine1```
- ```logio add-stream app1```
- ```logio list-server```
- ```logio add-server server1 192.168.0.1 28777```

## How to use 
For first time users, the logio commandline tool must be configured with a server. Therefore the first call must be to add/configure a server:

```logio add-server <name> <host> <port>```
- ```name``` - A name to refer to this server detail
- ```host``` - Host or IP information of the server
- ```port``` - Port where logio is running

After a configuring a server, then other commands can be specified. See the list below for other commands.

## Commands
The following are commands that can be called with the logio commandline tool.

### add-server <name> <host> <port>
Adds a server to your user config. The ```name``` allows you to reference the server configuration in other calls.

### remove-server <name>
Removes a server configuration from the config file.

### use-server <name>
Selects a default server to use for send-log, add-node, etc.

### list-server
Lists the servers in the config file.

### send-log -m message [-n node1] [-s stream1]
Sends a log message to the default server. The node and stream are not required and will default to the application (logio), and machine name.


