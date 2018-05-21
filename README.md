# Moneybox Money Withdrawal

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible. 

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WidthdrawMoney.Execute(...).

## Guidlines

* You should spend no more than 1 hour on this task, although there is no time limit
* You should fork or copy this repository into your own public repository (Gihub, BitBucket etc.) before you do your work
* Your solution must compile and run first time
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)

Once you have completed your work, send us a link to your public repository.

Good luck!

## Notes from John Cyril
Changed the previous implementation of subtacting the "Withdrawn" amount.
It feels more correct to Add to the withdrawn amount as a guage of accumulated activity. Withdrawn != balance...
If, on the otherhand, it would be appropriate for withdrawn and paid in to net against each other, one could consider making this one variable that tracks the net activity of paid/withdrawn breach a limit

Noted there was no "Withdraw limit" in place on the Account. This could have been an extra consideration.

Enjoyed the task, not used .netcore before - learned a lot. As rhinomocks isn't compatiable, had to get to grips with Moq too, which was cool