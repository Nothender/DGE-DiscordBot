# REPOSOIRE
C'est un reposoire

# Discord "Game Engine"
A game engine using discord's Unofficial .NET API to make bots

will contain multiple features to be explained later

# Documentation :
Command string prefix = "//"

Debug/Test Methods :
- when entering the command `ping` the bot replies with `pong`
- when entering the command `logTest` with argument 1 : the logMode, and other arguments the logInfo the bot will reply with the DGE logMode + Concatenation of logInfos

FrameBuffer(s) manipulations :
- there is currently only one FrameBuffer : StringFrameBuffer<FBType => SFB> to execute the following commands use <the_command> + <FBType>
- the command `init<FBType>` Creates the FrameBuffer in memory and initializes the FrameBuffer
- the command `display<FBType>` Displays the FrameBuffer via the bot sending messages in the current channel
