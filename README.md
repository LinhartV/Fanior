# Fanior
## Play here: [https://fanior.herokuapp.com](https://fanior.azurewebsites.net)
Welcome to my online game for bachelors project.
Fanior is casual action multiplayer game.
There are currently no running servers where you could play this game (hopefully this will change in the future).
## Rules
You are a hero fighting other evil oponents trying to stay alive for as long as you can while defeating others.
With every kill you gain points - when you reach certain amount of points, you can upgrade you character, or ignore it to wait for a greater upgrade of your weapon or to gain new ability.
You start with basic weapon, which you can further upgrade. There are countless combinations of weapons, abilities and upgrades - feel free to experiment to find the strongest combination!
## Author
My name is Vojtěch Linhart. I'm living in the Czech Republic (small state in the middle of Europe) and I currently study Technical University of Liberec, which I'll hopefully finish soon - this game should help it ;)
I love playing PC games, so I decided to created my own.
## Notes
### 06/10/2023
Starting keeping notes.
Online communication: Main idea was that code runs on server and on client too. When client executes an action (e.g. moves to left), it will send message to server, server broadcasts it to all other players and then everyone knows "this player moved to left". This way server and clients communicate using only these short messages. But the problem is synchronization. Code runs a bit differently on each machine (mainly time it takes to execute a frame). So on one computer the game looks a bit differently than on another computer, which is unaccaptable.
So far I solved this way: Server each frame distributes received actions (as explained) plus basic info about each player (angle he's looking and his coordinates). Rest of the code still runs both on server and on client.
