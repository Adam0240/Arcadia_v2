Bug Discovered:
- When one of your pokemon has 0 hp and is swapped to the first pokemon into your party
if you try to start a battle it shows for example "Pikachu fainted" and the battle dialouge closes and 
your back on the map.
- Intended behavior: When the first member of your pokemon is fainted at 0 hp and a battle is started the next 
availabe pokemon in inventory that is not fainted is sent out. 
- For example if a party is listed in the order: Pikachu (0hp), Umbreon 5 hp, Espeon 30 hp, etc, then Umbreon would be sent out
- Example 2 if a party is listed in the order: Pikachu (0hp), Umbreon 0 hp, Espeon 30 hp, Charmander 20 hp, then Espeon would be sent out

QOL Improvement:
- If there are only 2 pokemon in the players inventory and the player chooses the swap pokemon then it should swap without
needing the player to input the names of the pokemon.
- For example if the player chooses the swap pokemon option through the menu and it is only "Umbreon" and "Espeon"
the 2 should swap without asking the player "Who do you want to swap?". 
- In a regular battle when the players pokemon is defeated if there are only two pokemon in the 
player's inventory the 2nd pokemon should swap in without asking the player if they want to swap. 
If the second pokemon is also fainted then no swap occurs and the battle ends as intended. 
  