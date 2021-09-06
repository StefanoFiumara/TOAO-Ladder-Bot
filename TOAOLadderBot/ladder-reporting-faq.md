TOAO Ladder Bot FAQ
v1.0

Here are the available commands you can issue to the ladder bot!

Use `!report` to record a ladder game, the bot will automatically perform a calculation and give back the results.

You must tag each match participant and delimit each team with the text "defeat" or "defeats"  

ex:
> !report `@Fano` defeats `@1nsane`
```
 TOAO Ladder Bot: Thank you @Fano! Your game was reported successfully!
Match Details:

Fano
---- DEFEATS ----
TOAO_1nsane

This match was recorded on 2021-08-30 12:21 AM UTC
The winner takes 6 points from the loser, GG!
```

When reporting team games, simply tag all involved participants on each side of "defeats"
> !report `@Fano` `@Nemo` defeat `@1nsane` `@JCulling`
```
 TOAO Ladder Bot: Thank you @Fano! Your game was reported successfully!
Match Details:

Fano
Nemo
---- DEFEAT ----
TOAO_1nsane
JCulling

This match was recorded on 2021-08-30 12:25 AM UTC
The winners take 4 points from the losers, GG!
```

Use `!stats` to view your stats, or use `!stats @User` to view another player's.
ex:
> !stats `@Fano`
```
TOAO Ladder Bot: @Fano Here are the stats for Fano:
Score: 61
Rank: Grook
Wins: 6
Losses: 9
Streak: 3
Win %: 40.00%
```

Use `!history` to view a player's match history, you can provide a number to view that player's last `X` matches.
ex:
> !history `@Fano` 3
```
TOAO Ladder Bot: @Fano History of Fano's last 3 Matches:
Won vs JCulling | +4
Won vs JCulling | +4
Won vs JCulling | +4
```

Use `!leaderboard` to view the ladder's current standings.
ex:
> !leaderboard
```
Rank | Name            | Score | Streak | Win %  
==== | =============== | ===== | ====== | =======
#1   | Nemo            | 107   | 7      | 78.57% 
#2   | Fano            | 61    | 3      | 40.00% 
#3   | JCulling        | 56    | -4     | 0.00%  
```