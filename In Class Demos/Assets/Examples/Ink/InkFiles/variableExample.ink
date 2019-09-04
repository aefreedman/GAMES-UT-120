VAR dice = false
VAR chalk = false
VAR appleWatch = false
VAR wallet = 25
VAR diceCost = 5
VAR chalkCost = 6
VAR appleWatchCost = 300

->BEGIN
===BEGIN
Hi! Welcome to the shop!
What do you wanna buy? You have {wallet} bucks.
{dice : You have dice.}
{chalk : You have chalk.}
{appleWatch : You have an apple watch.}

* [dice - {diceCost}]
{ diceCost < wallet:
	~ wallet = wallet - diceCost
	~ dice = true
	->Bought
- else:
	->Broke
}
* [chalk - {chalkCost}]
{ chalkCost < wallet:
	~ wallet = wallet - chalkCost
	~ chalk = true
	->Bought
- else:
	->Broke
}
+ [apple watch - {appleWatchCost}]
{ appleWatchCost < wallet:
	~ wallet = wallet - appleWatchCost
	~ appleWatch = true
	->Bought
- else:
	->Broke
}

=Broke
You don't have enough cash.
+ [keep shopping] ->BEGIN

=Bought
You bought it. Wow.
+ [keep shopping] ->BEGIN

->DONE