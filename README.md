# Checkers-Console-interface-Game

Checkers game implemented in C# at .NET framework to console application.
<hr/>

pick a board size (6 x 6 , 8 x 8 , 10 x 10)

enter your player's name

choose playing against another player or against the computer

<hr/>

The player makes his move by the format: ROWcol>ROWcol (for example in the picture below: Dg>Ef).

![image](https://github.com/user-attachments/assets/2e840dcd-a301-47bb-999d-3262759241ff)


The O's kings are marked with 'U', and The X's kings are marked with 'K'.
At any point, the user can quit the game by entering 'Q' instead of a valid move.
<hr/>

Each round is over when there are no more checkers on board for some player,
or when there are no more legal moves for both players.

score is calculated in each round's end as the difference between the players' remaining checkers on board
(regular checker = 1 points ; king = 4 points) and is granted to the winner,
or to both players if round ends with a tie.

# Enjoy the game! 
### Tools, Framework and language
<div>
    <img src="https://github.com/devicons/devicon/blob/master/icons/csharp/csharp-plain.svg" title="C#" **alt="C#" width="40" height="40"/>
    <img src="https://github.com/devicons/devicon/blob/master/icons/dot-net/dot-net-original-wordmark.svg" title=".NET" **alt=".NET" width="40" height="40"/>
</div>
