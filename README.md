# GIRBot

GIRBot - Discord bot for gamification of server moderation.
This bot is written in .NET 6.0. So it can be hosted on Windows or Linux servers.

The bot's name is inspired by the Mili song "Gunners in the Rain" and battle royale games.

The idea was this. There is a group of gunners - people who can timeout a regular user on the server with one command. 
But to ban a person, you need to use a bullet. The bot "sends" bullets to random gunner in a limited amount. 
In total, a user can survive [1-n] timeouts. The timeout duration depends on the settings in the appsettings.json file. 
If the user has been shot enough times, he will be banned forever.

For the bot to work, you need to configure roles for the administration, gunners, and the amount of ammunition on the server.

The ammunition role must be named by this template:
"Ammo: [number]"

Users with the admin role can use next commands:
-!heal
-!shoot
-!reload
-!reloadforce
-!unload

Users with the gunner role can only use !shoot command.

People can abuse the bot by leaving and returning to the server to reset their roles.
Solution: add another bot to your discord server that saves user roles(https://auttaja.io/ or https://www.dynobot.net/)

Have fun! Shoot each other and bring a justified moderation to your server.

P.S.: I won't continue working on the bot. If you want to share your ideas, add it to my code and create a pull request.
