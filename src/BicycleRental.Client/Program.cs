// See https://aka.ms/new-console-template for more information
using Dapr.Actors.Client;
using Dapr.Actors;
using BicycleRental.Interfaces.Cards;

const string WalletActorType = "WalletActor";
const string BicycleActorType = "BicycleActor";

Console.WriteLine("Running simulation..............");

var cardActor1 = ActorProxy.Create<IWalletActor>(new ActorId("000001"), WalletActorType);
await cardActor1.Reset();

var cardActor2 = ActorProxy.Create<IWalletActor>(new ActorId("000002"), WalletActorType);
await cardActor2.Reset();

await cardActor1.Deposit(100);
await cardActor2.Deposit(100);

var currentBalanceActor1 = await cardActor1.GetBalance();
Console.WriteLine($"Card 1: balance is {currentBalanceActor1}");

var currentBalanceActor2 = await cardActor2.GetBalance();
Console.WriteLine($"Card 2: balance is {currentBalanceActor2}");


//!! los métodos de los actores deben devolver  Task

var bicycleActorA = ActorProxy.Create<IBicycleActor>(new ActorId("A"), BicycleActorType);
await bicycleActorA.Reset();

var bicycleActorB = ActorProxy.Create<IBicycleActor>(new ActorId("B"), BicycleActorType);
await bicycleActorB.Reset();



await bicycleActorA.StartRenting("000001");
await Task.Delay(1000);
await bicycleActorB.StartRenting("000002");
await Task.Delay(2000);
await bicycleActorB.FinishRenting();
await Task.Delay(2500);
await bicycleActorA.FinishRenting();

currentBalanceActor1 = await cardActor1.GetBalance();
Console.WriteLine($"Card 1: balance is {currentBalanceActor1}");

currentBalanceActor2 = await cardActor2.GetBalance();
Console.WriteLine($"Card 2: balance is {currentBalanceActor2}");

//añadir kilometraje

//var cardActor1Id = new ActorId("1");
//var cardActor2Id = new ActorId("2");

//var cardActor1 = ActorProxy.Create<ICardActor>(cardActor1Id, "CardActor");
//var cardActor2 = ActorProxy.Create<ICardActor>(cardActor2Id, "CardActor");

//var currentBalanceActor1 = await cardActor1.GetBalance();

//currentBalanceActor1 = await cardActor1.Deposit(10);
//Console.WriteLine($"Card {cardActor1Id}: Deposits 10. Balance is {currentBalanceActor1}");

//currentBalanceActor1 = await cardActor1.Deposit(10);
//Console.WriteLine($"Card {cardActor1Id}: Deposits 10. Balance is {currentBalanceActor1}");

//var currentBalanceActor2 = await cardActor2.GetBalance();
//Console.WriteLine($"Card {cardActor2Id}: balance is {currentBalanceActor2}");

//currentBalanceActor2 = await cardActor2.Deposit(50);
//Console.WriteLine($"Card {cardActor2Id}: Deposits 50. Balance is {currentBalanceActor2}");

//currentBalanceActor1 = await cardActor1.Withdraw(15);
//Console.WriteLine($"Card {cardActor1Id}: Withdraws 15. Balance is {currentBalanceActor1}");

//currentBalanceActor2 = await cardActor2.Withdraw(55);
//Console.WriteLine($"Card {cardActor2Id}: Withdraws 55. Balance is {currentBalanceActor2}");

Console.WriteLine("Simulation finished");