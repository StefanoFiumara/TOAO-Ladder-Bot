# TOAO Ladder Bot
A Discord bot to record ladder standings on a competitive leaderboard.

The bot tracks players in a competitive gaming ladder, players report matches and the bot maintains an internal calculation of everyone's standing using a simple points system.

## Points System
The reported winner of a game is awarded points according to the scoring table below based on their opponent's rank tier, while the loser loses the same amount of points:

|                           | Tier S   | Tier A   | Tier B   | Tier C   | Tier D   | Tier F   |
|---------------------------|----------|----------|----------|----------|----------|----------|
| Tier S: 155+ points       | 6        | 3        | 3        | 1        | 1        | 1        |
| Tier A: 106-154 points    | 9        | 6        | 3        | 3        | 1        | 1        |
| Tier B: 81-105 points     | 12       | 9        | 6        | 3        | 3        | 1        |
| Tier C: 61-80 points      | 15       | 12       | 9        | 6        | 3        | 3        |
| Tier D: 31-60 points      | 18       | 15       | 12       | 9        | 6        | 3        |
| Tier F: 20-30 points      | 21       | 18       | 15       | 12       | 9        | 6        |

**Example**: You currently have 108 points and are thus ranked as **Tier A**. You play, and beat another player who has 166 points and is therefore ranked as **Tier S**. You can see in the scoring table that the Tier S ranked players are worth 9 points to you. Hence, you gain 9 points for defeating that player, and they in turn lose 9 points as a result.

### Team Games
Team games are scored by averaging the ranks of the team, and then doing a lookup in the same scoring table above, but it is scaled down according to the number of players using the following formula:

`points_awarded * (1 - (0.05 * number_of_players)`

**Example**: A 2v2, you and your teammate's rank average is **Tier C** and the losing team rank average is **Tier B**. According to the scoring table, you would have gained 9 points, but because it is a 2v2, it is scaled down to: `9 * (1 - (0.05 x 4)) = 7` points.

### Free For All
In Free for all games, all losing players give up a point and they will be given to the winner.

**Example**: A 5 player FFA match, all of the losing players lose 1 point each, but the winner is awarded 4 points.

## Commands
The following commands are used to report games to the ladder bot, discord users should be tagged in the command so that the bot can identify them by their unique discord ID and keep track of their placement across multiple games.
* `!report`
    * Reports a ladder match
    * `!report @A defeats @B`
        * 1v1 match
        * A is the winner and B is the loser
        * can use 'defeats' or 'defeat' as team delimeter
        * Reporter must be a player that participated in the match
        * Included players must be mentioned by their discord name
    * `!report @A @B defeat @X @Y`
        * Team match with each team player separated by a space
        * can use 'defeats' or 'defeat' as team delimeter
        * Reporter must be a player that participated in the match
        * Included players must be mentioned by their discord name

* `!stats`
    * Gives detailed stats about the given player
    * Score, Current Streak, Wins, Losses, Games Played, Win %
    * `!stats @A`    
        * For stats about the mentioned player `@A`
    * `!stats`
        * For stats pertaining to the user invoking the command

* `!history`
    * Gives match history for the player
    * `!history @A`
        * history of @A's last 5 matches
    * `!history @A 20`
        * history of @A's last 20 matches

* `!leaderboard`
  * Shows the current ladder standings
 
 ## Attribution
 These rules were designed by the TOAO gaming community, the original writeup can be found here: http://ladder.toaoclan.net/rules.php
