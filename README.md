# Klserjht
Klserjht is a simple, customisable tool that an osu! twitch streamer can run on their machine, allowing their viewers to type a command in chat and have a twitch bot respond with the current beatmap information (artist, title, etc.).

## Installing
There is no installation process as Klserjht is a portable application. Click [here](https://github.com/TheOmyNomy/Klserjht/releases/latest) to download the latest version and run the **Klserjht.exe** executable.

## Setup
Below is an example of Klserjht on its first launch.

![](https://puu.sh/Ca5o7/c38a55efb7.png "")

Below contains information regarding each text field and its use.

* **Username**: The username of the twitch account that will be sending and receiving messages into the twitch chat.
* **Token**: The twitch oauth token of the twitch account that will be sending and receiving messages into the twitch chat (found [here](https://twitchapps.com/tmi/)).
* **Channel**: The twitch channel that the twitch account will connect, send and receive messages to and from.
* **Format**: The format of the message that will be sent if a beatmap is found (can be left unchanged).
* **Command**: The command that the application will respond to (can be left unchanged).

Below are patterns available for use in the **Format** text field. Patterns will be replaced by the beatmap information related to the current beatmap.

* **!artist!**: The artist of the beatmap.
* **!title!**: The title of the beatmap.
* **!creator!**: The creator (mapper) of the beatmap.
* **!version!**: The version (difficulty) of the beatmap.
* **!link!**: The download link for the beatmap.
* **!sender!**: The username of the user who sent the command.

Below is an example with all the text fields filled out.

![](https://puu.sh/Ca5oI/9712d68804.png "")

Once all the fields have been filled out, click the **Login** button to login and the application will begin to listen for the command specified and respond with the format specified.

## Updating
Klsjerht checks for updates when the application is launched. If an update is available, the text **No updates available.** will change to **Update available! Click here!** and will become a clickable link. Once clicked, it'll open the download page for the latest version in your default browser. Download the latest version and replace all existing files (except the configuration file, **Klsjerht.json**) with the new files.

Below is an example with an update available.

![](https://puu.sh/Ca5p9/e45979614a.png "")

