# TOAO Ladder Bot
A Discord bot to record TOAO Ladder games

## Commands
* !report
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

* !stats
    * Gives detailed stats about the given player
    * Score, Current Streak, Wins, Losses, Games Played, Win %
    * `!stats @A`    
        * For stats about the mentioned player `@A`
    * `!stats`
        * For stats pertaining to the user invoking the command

* !history
    * Gives match history for the player
    * `!history @A`
        * history of @A's last 5 matches
    * `!history @A 20`
        * history of @A's last 20 matches

* !leaderboard
  * Shows the current ladder leaderboards